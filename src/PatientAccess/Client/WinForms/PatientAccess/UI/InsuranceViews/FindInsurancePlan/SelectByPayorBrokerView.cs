using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.InsuranceViews.FindInsurancePlan
{
    /// <summary>
    /// Summary description for SelectByPayorBrokerView.
    /// </summary>
    public class SelectByPayorBrokerView : SelectByView
    {
        #region Events
        public event EventHandler   CloseFormOnEnterKeyEvent;
        #endregion

        #region Event Handlers
        private void CloseFormOnEnterKeyEventHandler(object sender, EventArgs e)
        {
            if( CloseFormOnEnterKeyEvent != null )
            {
                CloseFormOnEnterKeyEvent( this, e );
            }
        }

        private void mtbBoxSpecifyOther_Enter(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor( mtb );
            mtb.Refresh();
        }

        private void mtbBoxSpecifyOther_TextChanged(object sender, EventArgs e)
        {
            if( !ResettingControls )
            {
                base.OtherPayorBroker = this.mtbBoxSpecifyOther.Text;
                OnSelectedInsurancePlanChanged();
            }
        }

        private void searchField_PerformSearch(object sender, EventArgs e)
        {
            PerformSearch( false );
        }

        private void checkBoxOther_CheckedChanged(object sender, EventArgs e)
        {
            PerformSpecialSearch();
            RunRules();
        }

        private void payorBrokersView_SelectedProviderChanged(object sender, EventArgs e)
        {
            if( payorBrokersView.SelectedProvider != null )
            {
                base.PopulateInsurancePlans( payorBrokersView.SelectedProvider.InsurancePlans );
            }
            else
            {
                base.PopulateInsurancePlans( null );
            }
        }


        private void comboBoxPlanCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearProviderResults();

            searchField.SearchText = string.Empty;
            searchField.SearchText = CategorySearchString();
            if( searchField.SearchText == MEDICAID_SEARCHSTRING
                || searchField.SearchText == MEDICARE_SEARCHSTRING
                || comboBoxPlanCategories.SelectedItem.ToString() == SELF_PAY)
            {
                searchField.TextBoxEnabled = false;
            }
            else
            {
                searchField.TextBoxEnabled = true;
            }

            payorBrokersView.DisplayProviders( null );
            PerformSpecialSearch();
        }

        private void FindPlanSpecifyRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor( this.mtbBoxSpecifyOther );
        }

        private void comboBoxPlanCategories_KeyDown(object sender, KeyEventArgs e)
        {
            if( e.KeyData.Equals( Keys.Enter ) )
            {
                PerformSearch( false );
            }
        }
        #endregion

        #region Methods
        public override void Activate()
        {
            LineLabelCaption = "Plans from selected payor/broker";
            ResetControls();
            PerformSpecialSearch();
        }

        public override void ActivateSearchField()
        {
            searchField.Activate();
        }
        #endregion

        #region Properties
        public override bool PlanCanBeApplied
        {
            get
            {
                bool otherPlanOk = 
                    (checkBoxOther.Checked) || 
                    (mtbBoxSpecifyOther.Text != "");

                return (otherPlanOk && listView.SelectedItems.Count != 0) || (SelectedInsurancePlan != null);
            }
        }

        public override string SearchText
        {
            get
            {
                return searchField.SearchText;
            }
            set
            {
                searchField.SearchText = value;
            }
        }
        #endregion

        #region Private Methods
        private void InitializePlanCategories()
        {
            if( comboBoxPlanCategories.Items.Count == 0 )
            {
                InsurancePlanCategoriesHelper.PopulateWithCollection( InsuranceBroker.AllTypesOfCategories(User.GetCurrent().Facility.Oid) );
                comboBoxPlanCategories.Items.Insert( 0, "All" );
            }
        }

        private InsurancePlanCategory SearchCategory()
        {
            object selectedObj = InsurancePlanCategoriesHelper.SelectedItem;
            if( selectedObj is InsurancePlanCategory )
            {
                return (InsurancePlanCategory)selectedObj;
            }
            else
            {
                return null;
            }
        }

        private bool IsSearchDataEntered()
        {
             if( searchField != null
             && searchField.SearchText != null
             && searchField.SearchText.Length > 0 )
             {
                 return true;
             }
             return false;
        }

        private bool IsSelfPay()
        {
            if( this.comboBoxPlanCategories.SelectedItem != null
                 && this.comboBoxPlanCategories.SelectedItem.ToString() == SELF_PAY )
            {
                return true;
            }
            return false;
        }
        private void PerformSearch( bool specialCategorySearch )
        {
            searchRadioButton = radioButtonBeginsWith.Checked;
            insurancePlanCategory = SearchCategory();
            searchFieldText = searchField.SearchText;
            if( IsSearchDataEntered() || IsSelfPay() )
            {
                if( ( !searchRadioButton && searchField.SearchText.Length < 2 ) )
                {
                    MessageBox.Show( "For a \"Contains\" search, a minimum of three characters are required.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    searchField.Activate();
                    listView.Items.Clear();
                    return;
                }

                else
                {
                    if( this.searchbackgroundWorker == null
                    ||
                    ( this.searchbackgroundWorker != null
                    && !this.searchbackgroundWorker.IsBusy )
                    )
                    {
                        this.BeforeSearch();
                        this.searchbackgroundWorker = new BackgroundWorker();
                        this.searchbackgroundWorker.WorkerSupportsCancellation = true;

                        searchbackgroundWorker.DoWork +=
                            new DoWorkEventHandler( GetProvidersWith );
                        searchbackgroundWorker.RunWorkerCompleted +=
                            new RunWorkerCompletedEventHandler( AfterSearchWork );

                        searchbackgroundWorker.RunWorkerAsync();

                    }
                }
                PreviousCategoryWasSpecialSearch = specialCategorySearch;
            }
            else if( searchField != null
             && searchField.SearchText != null && searchField.SearchText.Length == 0 )
            {   // Close dialog if ENTER key pressed
                if( CloseFormOnEnterKeyEvent != null )
                {
                    CloseFormOnEnterKeyEvent( this, null );
                }
            }
            
        
        }

        private void BeforeSearch()
        {
            checkBoxOther.Enabled = false;
            this.progressPanel2.Visible = true;
            this.progressPanel2.BringToFront();
        }
       
        private void GetProvidersWith( object sender, DoWorkEventArgs e )
        {
            providersCollection = null;   
            if( searchRadioButton )
            {
                providersCollection = InsuranceBroker.ProvidersStartingWith(
                  searchFieldText, PatientAccount.Facility.Oid, PatientAccount.AdmitDate, insurancePlanCategory );
             
            }
            else
            {
                providersCollection = InsuranceBroker.ProvidersContaining(
                      searchFieldText, PatientAccount.Facility.Oid, PatientAccount.AdmitDate, insurancePlanCategory );
                
            }

            // poll CancellationPending and set e.Cancel to true and return 
            if( this.searchbackgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            }
        }

        private void AfterSearchWork( object sender, RunWorkerCompletedEventArgs e )
        {
            if( this.IsDisposed || this.Disposing )
                return;

            if( e.Cancelled )
            {
                this.Refresh();
                // user cancelled
                // Note that due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.
            }
            else if( e.Error != null )
            {
                // handle errors/exceptions thrown from within the DoWork method here
                throw e.Error;
            }
            else
            {
                payorBrokersView.DisplayProviders( providersCollection );
                if( payorBrokersView.SelectedProvider == null )
                {
                    InsurancePlanCategory category = SearchCategory();
                    if( category == null )
                    {
                        payorBrokersView.CoverMessage = "No items found for all categories.";
                    }
                    else
                    {
                        payorBrokersView.CoverMessage = String.Format(
                            "No items found for the {0} category.", category.DisplayString );
                    }
                    payorBrokersView.ShowCover = true;
                    listView.Items.Clear();
                    this.progressPanel2.Visible = false;
                }
                else
                {
                    payorBrokersView.CoverMessage = String.Empty;
                    payorBrokersView.ShowCover = false;
                    this.progressPanel2.Visible = false;
                }
                payorBrokersView.Activate();
                checkBoxOther.Enabled = true;
            }


        }
  

        private void GetProvidersStartingWith( object sender, DoWorkEventArgs e )
        {
             providers = InsuranceBroker.ProvidersStartingWith( OTHER_SEARCHSTRING,
                                     PatientAccount.Facility.Oid, PatientAccount.AdmitDate, null );

            // poll CancellationPending and set e.Cancel to true and return 
            if( this.backgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            }
        }
        private void CancelBackgroundWorker()
        {
            // cancel the background worker(s) here...
            if( null != this.backgroundWorker && 
                this.backgroundWorker.WorkerSupportsCancellation )
            {
                this.backgroundWorker.CancelAsync();
            }
            if( null != this.searchbackgroundWorker &&
                this.searchbackgroundWorker.WorkerSupportsCancellation )
            {
                this.searchbackgroundWorker.CancelAsync();
            }

        }

        private void AfterWork( object sender, RunWorkerCompletedEventArgs e )
        {
            if( this.IsDisposed || this.Disposing )
                return;

            if( e.Cancelled )
            {
                this.Refresh();
                // user cancelled
                // Note that due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.
            }
            else if( e.Error != null )
            {
                // handle errors/exceptions thrown from within the DoWork method here
                throw e.Error;
            }
            else
            {
            if( providers.Count > 0 )
            {
                IEnumerator providersEnum = providers.GetEnumerator();
                
                if( providersEnum.MoveNext() )
                {
                    AbstractProvider provider = (AbstractProvider)providersEnum.Current;
                    ICollection plans = provider.InsurancePlans;
                    base.PopulateInsurancePlans( plans );
                }
                this.progressPanel1.Visible = false;
            }
            else
            {
                listView.Items.Clear();
                this.progressPanel1.Visible = false;
            }
            checkBoxOther.Enabled = true;
        }

        }
        private void PerformOtherSearch()
        {
            searchField.SearchText = String.Empty;

            if( this.backgroundWorker == null
                 ||
                 ( this.backgroundWorker != null
                 && !this.backgroundWorker.IsBusy )
                 )
            {
                this.BeforeOtherSearch();
                this.backgroundWorker = new BackgroundWorker();
                this.backgroundWorker.WorkerSupportsCancellation = true;

                backgroundWorker.DoWork += this.GetProvidersStartingWith;
                backgroundWorker.RunWorkerCompleted += this.AfterWork;

                backgroundWorker.RunWorkerAsync();

            }


           
        }

        private void BeforeOtherSearch()
        {
            checkBoxOther.Enabled = false;
            this.progressPanel1.Visible = true;
            this.progressPanel1.BringToFront();
        }


        private string CategorySearchString()
        {
            string result = searchField.SearchText;

            if( PreviousCategoryWasSpecialSearch )
            {
                result = string.Empty;
            }

            InsurancePlanCategory category = SearchCategory();

            if( category != null )
            {
                if ( category.Oid == InsurancePlanCategory.GOVERNMENT_MEDICAID_OID )
                {
                    result = MEDICAID_SEARCHSTRING;
                }
                else if ( category.Oid == InsurancePlanCategory.GOVERNMENT_MEDICARE_OID )
                {
                    result = MEDICARE_SEARCHSTRING;
                }
                else if ( category.Oid == InsurancePlanCategory.SELFPAY_OID )
                {
                    result = SELFPAY_SEARCHSTRING;
                }
            }
            
            return result;
        }


        private void PerformSpecialSearch()
        {
            if( !ResettingControls )
            {
                UpdateControlState();

                if( checkBoxOther.Checked )
                {
                    comboBoxPlanCategories.SelectedIndex = 0;
                    searchField.SearchText = String.Empty;
                    payorBrokersView.ListView.Items.Clear();
                    payorBrokersView.ShowCover = false;
                    panelMain.Enabled = false;
                    panelSpecify.Enabled = true;

                    PerformOtherSearch();
                }
                else
                {
                    listView.Items.Clear();
                    mtbBoxSpecifyOther.Clear();
                    radioButtonBeginsWith.Checked = true;
                    panelMain.Enabled = true;
                    panelSpecify.Enabled = false;
                    base.PopulateInsurancePlans( null );
                }
            }
        }

        private bool CategoryHasControledSearch()
        {
            InsurancePlanCategory category = SearchCategory();
            return (category != null) && 
                (
                ( category.Oid == InsurancePlanCategory.GOVERNMENT_MEDICAID_OID ) ||
                ( category.Oid == InsurancePlanCategory.GOVERNMENT_MEDICARE_OID ) ||
                ( category.Oid == InsurancePlanCategory.SELFPAY_OID )
                );
        }

        private void UpdateControlState()
        {
            var findInsurancePlanView = new FindInsurancePlanView();

            if( CategoryHasControledSearch() )
            {
                radioButtonBeginsWith.Checked = true;
                radioButtonBeginsWith.Enabled = false;
                radioButtonContains.Enabled   = false;
                searchField.Button.Focus();
                findInsurancePlanView.EnableOkButton = PlanCanBeApplied;
            }
            else
            {
                radioButtonBeginsWith.Enabled = true;
                radioButtonContains.Enabled   = true;
                findInsurancePlanView.EnableOkButton = PlanCanBeApplied;
            }
        }

        private void ClearProviderResults()
        {
            payorBrokersView.DisplayProviders( null );
        }

        protected override void ResetControls()
        {
            ResettingControls = true;
            if( this.comboBoxPlanCategories.Items.Count > 0 )
            {
                comboBoxPlanCategories.SelectedIndex = 0;
            }
            checkBoxOther.Checked = false;
            radioButtonBeginsWith.Checked = true;
            mtbBoxSpecifyOther.Text = "";
            ResettingControls = false;

            ClearProviderResults();

            base.ResetControls();
        }

        private void SelectByPayorBrokerView_Load(object sender, EventArgs e)
        {
            if( !this.DesignMode )
            {
                InsurancePlanCategoriesHelper = new ReferenceValueComboBox( comboBoxPlanCategories );
                InitializePlanCategories();
                if( comboBoxPlanCategories.Items.Count > 0 )
                {
                    comboBoxPlanCategories.SelectedIndex = 0;
                }

                ResettingControls = false;

                UpdateControlState();
            }

            this.RegisterRulesEvents();
            this.RunRules();
        }

        private void RegisterRulesEvents()
        {
            //RuleEngine.LoadRules( AccountView.GetInstance().Model_Account );

            RuleEngine.GetInstance().RegisterEvent( typeof(FindPlanSpecifyRequired), this.checkBoxOther, new EventHandler(FindPlanSpecifyRequiredEventHandler));            
        }

        private void RunRules()
        {
            // reset all fields that might have error, preferred, or required backgrounds
            if( mtbBoxSpecifyOther.Enabled )
            {
                UIColors.SetNormalBgColor( this.mtbBoxSpecifyOther );
            }
            else
            {
                this.mtbBoxSpecifyOther.BackColor = SystemColors.Control;
                this.Refresh();
            }
            Refresh();

            RuleEngine.GetInstance().EvaluateRule( typeof(FindPlanSpecifyRequired), AccountView.GetInstance().Model_Account );
        }
        #endregion

        #region Private Properties
        private ReferenceValueComboBox InsurancePlanCategoriesHelper
        {
            get
            {
                return i_InsurancePlanCategoriesHelper;
            }
            set
            {
                i_InsurancePlanCategoriesHelper = value;
            }
        }

        private bool ResettingControls
        {
            get
            {
                return i_ResettingControls;
            }
            set
            {
                i_ResettingControls = value;
            }
        }

        private bool PreviousCategoryWasSpecialSearch
        {
            get
            {
                return i_PreviousCategoryWasSpecialSearch;
            }
            set
            {
                i_PreviousCategoryWasSpecialSearch = value;
            }
        }

        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelMain = new System.Windows.Forms.Panel();
            this.progressPanel2 = new PatientAccess.UI.CommonControls.ProgressPanel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.pnlRadioButtons = new System.Windows.Forms.Panel();
            this.radioButtonBeginsWith = new System.Windows.Forms.RadioButton();
            this.radioButtonContains = new System.Windows.Forms.RadioButton();
            this.searchField = new PatientAccess.UI.CommonControls.SearchField();
            this.comboBoxPlanCategories = new System.Windows.Forms.ComboBox();
            this.lblShowPayors = new System.Windows.Forms.Label();
            this.lineLblPayorBrokers = new PatientAccess.UI.CommonControls.LineLabel();
            this.payorBrokersView = new PatientAccess.UI.InsuranceViews.FindInsurancePlan.PayorBrokersView();
            this.panelSpecifyControls = new System.Windows.Forms.Panel();
            this.checkBoxOther = new System.Windows.Forms.CheckBox();
            this.panelSpecify = new System.Windows.Forms.Panel();
            this.mtbBoxSpecifyOther = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.labelSpecifyOther = new System.Windows.Forms.Label();
            this.progressPanel1 = new PatientAccess.UI.CommonControls.ProgressPanel();
            this.panelMain.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.pnlRadioButtons.SuspendLayout();
            this.panelSpecifyControls.SuspendLayout();
            this.panelSpecify.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add( this.progressPanel2 );
            this.panelMain.Controls.Add( this.panelTop );
            this.panelMain.Controls.Add( this.payorBrokersView );
            this.panelMain.Location = new System.Drawing.Point( 8, 0 );
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size( 668, 320 );
            this.panelMain.TabIndex = 1;
            // 
            // progressPanel2
            // 
            this.progressPanel2.BackColor = System.Drawing.Color.White;
            this.progressPanel2.Location = new System.Drawing.Point( 0,101 );
            this.progressPanel2.Name = "progressPanel2";
            this.progressPanel2.Size = new System.Drawing.Size( 668, 219 );
            this.progressPanel2.TabIndex = 6;
            this.progressPanel2.Visible = false;
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add( this.pnlRadioButtons );
            this.panelTop.Controls.Add( this.searchField );
            this.panelTop.Controls.Add( this.comboBoxPlanCategories );
            this.panelTop.Controls.Add( this.lblShowPayors );
            this.panelTop.Controls.Add( this.lineLblPayorBrokers );
            this.panelTop.Location = new System.Drawing.Point( 0, 0 );
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size( 668, 100 );
            this.panelTop.TabIndex = 0;
            // 
            // pnlRadioButtons
            // 
            this.pnlRadioButtons.Controls.Add( this.radioButtonBeginsWith );
            this.pnlRadioButtons.Controls.Add( this.radioButtonContains );
            this.pnlRadioButtons.Location = new System.Drawing.Point( 5, 46 );
            this.pnlRadioButtons.Name = "pnlRadioButtons";
            this.pnlRadioButtons.Size = new System.Drawing.Size( 183, 22 );
            this.pnlRadioButtons.TabIndex = 6;
            // 
            // radioButtonBeginsWith
            // 
            this.radioButtonBeginsWith.Location = new System.Drawing.Point( 6, 8 );
            this.radioButtonBeginsWith.Name = "radioButtonBeginsWith";
            this.radioButtonBeginsWith.Size = new System.Drawing.Size( 80, 16 );
            this.radioButtonBeginsWith.TabIndex = 2;
            this.radioButtonBeginsWith.TabStop = true;
            this.radioButtonBeginsWith.Text = "Begins with";
            // 
            // radioButtonContains
            // 
            this.radioButtonContains.Location = new System.Drawing.Point( 91, 8 );
            this.radioButtonContains.Name = "radioButtonContains";
            this.radioButtonContains.Size = new System.Drawing.Size( 104, 16 );
            this.radioButtonContains.TabIndex = 3;
            this.radioButtonContains.TabStop = true;
            this.radioButtonContains.Text = "Contains";
            // 
            // searchField
            // 
            this.searchField.KeyPressExpression = "^[a-zA-Z][a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.searchField.Location = new System.Drawing.Point( 0, 76 );
            this.searchField.Mask = "";
            this.searchField.MaxFieldLength = 20;
            this.searchField.Model = null;
            this.searchField.Name = "searchField";
            this.searchField.SearchText = "";
            this.searchField.Size = new System.Drawing.Size( 568, 23 );
            this.searchField.TabIndex = 4;
            this.searchField.TextBoxEnabled = true;
            this.searchField.ValidationExpression = "^[a-zA-Z][a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.searchField.PerformSearch += new System.EventHandler( this.searchField_PerformSearch );
            // 
            // comboBoxPlanCategories
            // 
            this.comboBoxPlanCategories.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPlanCategories.Location = new System.Drawing.Point( 200, 28 );
            this.comboBoxPlanCategories.Name = "comboBoxPlanCategories";
            this.comboBoxPlanCategories.Size = new System.Drawing.Size( 185, 21 );
            this.comboBoxPlanCategories.TabIndex = 1;
            this.comboBoxPlanCategories.SelectionChangeCommitted += new System.EventHandler( this.comboBoxPlanCategories_SelectedIndexChanged );
            this.comboBoxPlanCategories.KeyDown += new System.Windows.Forms.KeyEventHandler( this.comboBoxPlanCategories_KeyDown );
            // 
            // lblShowPayors
            // 
            this.lblShowPayors.Location = new System.Drawing.Point( 0, 29 );
            this.lblShowPayors.Name = "lblShowPayors";
            this.lblShowPayors.Size = new System.Drawing.Size( 210, 23 );
            this.lblShowPayors.TabIndex = 0;
            this.lblShowPayors.Text = "Show only payors with plans in category:";
            // 
            // lineLblPayorBrokers
            // 
            this.lineLblPayorBrokers.Caption = "Payors/Brokers";
            this.lineLblPayorBrokers.Location = new System.Drawing.Point( 0, 0 );
            this.lineLblPayorBrokers.Name = "lineLblPayorBrokers";
            this.lineLblPayorBrokers.Size = new System.Drawing.Size( 668, 18 );
            this.lineLblPayorBrokers.TabIndex = 0;
            this.lineLblPayorBrokers.TabStop = false;
            // 
            // payorBrokersView
            // 
            this.payorBrokersView.CoverPadding = 40;
            this.payorBrokersView.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.payorBrokersView.Location = new System.Drawing.Point( 0, 101 );
            this.payorBrokersView.Model = null;
            this.payorBrokersView.Name = "payorBrokersView";
            this.payorBrokersView.SelectedProvider = null;
            this.payorBrokersView.ShowCover = true;
            this.payorBrokersView.Size = new System.Drawing.Size( 668, 219 );
            this.payorBrokersView.TabIndex = 5;
            this.payorBrokersView.CloseFormOnEnterKeyEvent += new System.EventHandler( this.CloseFormOnEnterKeyEventHandler );
            this.payorBrokersView.SelectedProviderChanged += new System.EventHandler( this.payorBrokersView_SelectedProviderChanged );
            // 
            // panelSpecifyControls
            // 
            this.panelSpecifyControls.Controls.Add( this.checkBoxOther );
            this.panelSpecifyControls.Controls.Add( this.panelSpecify );
            this.panelSpecifyControls.Location = new System.Drawing.Point( 8, 326 );
            this.panelSpecifyControls.Name = "panelSpecifyControls";
            this.panelSpecifyControls.Size = new System.Drawing.Size( 568, 25 );
            this.panelSpecifyControls.TabIndex = 6;
            // 
            // checkBoxOther
            // 
            this.checkBoxOther.Location = new System.Drawing.Point( 0, 0 );
            this.checkBoxOther.Name = "checkBoxOther";
            this.checkBoxOther.Size = new System.Drawing.Size( 168, 24 );
            this.checkBoxOther.TabIndex = 0;
            this.checkBoxOther.Text = "Payor/Broker is Other";
            this.checkBoxOther.CheckedChanged += new System.EventHandler( this.checkBoxOther_CheckedChanged );
            // 
            // panelSpecify
            // 
            this.panelSpecify.Controls.Add( this.mtbBoxSpecifyOther );
            this.panelSpecify.Controls.Add( this.labelSpecifyOther );
            this.panelSpecify.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelSpecify.Location = new System.Drawing.Point( 194, 0 );
            this.panelSpecify.Name = "panelSpecify";
            this.panelSpecify.Size = new System.Drawing.Size( 374, 25 );
            this.panelSpecify.TabIndex = 1;
            // 
            // mtbBoxSpecifyOther
            // 
            this.mtbBoxSpecifyOther.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbBoxSpecifyOther.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbBoxSpecifyOther.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbBoxSpecifyOther.Location = new System.Drawing.Point( 55, 2 );
            this.mtbBoxSpecifyOther.Mask = "";
            this.mtbBoxSpecifyOther.MaxLength = 20;
            this.mtbBoxSpecifyOther.Name = "mtbBoxSpecifyOther";
            this.mtbBoxSpecifyOther.Size = new System.Drawing.Size( 318, 20 );
            this.mtbBoxSpecifyOther.TabIndex = 1;
            this.mtbBoxSpecifyOther.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbBoxSpecifyOther.TextChanged += new System.EventHandler( this.mtbBoxSpecifyOther_TextChanged );
            this.mtbBoxSpecifyOther.Enter += new System.EventHandler( this.mtbBoxSpecifyOther_Enter );
            // 
            // labelSpecifyOther
            // 
            this.labelSpecifyOther.Location = new System.Drawing.Point( 6, 5 );
            this.labelSpecifyOther.Name = "labelSpecifyOther";
            this.labelSpecifyOther.Size = new System.Drawing.Size( 52, 23 );
            this.labelSpecifyOther.TabIndex = 0;
            this.labelSpecifyOther.Text = "Specify:";
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = System.Drawing.Color.White;
            this.progressPanel1.Location = new System.Drawing.Point( 10, 394 );
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new System.Drawing.Size( 665, 79 );
            this.progressPanel1.TabIndex = 6;
            this.progressPanel1.Visible = false;
            // 
            // SelectByPayorBrokerView
            // 
            this.Controls.Add( this.progressPanel1 );
            this.Controls.Add( this.panelSpecifyControls );
            this.Controls.Add( this.panelMain );
            this.Name = "SelectByPayorBrokerView";
            this.Size = new System.Drawing.Size( 684, 541 );
            this.Load += new System.EventHandler( this.SelectByPayorBrokerView_Load );
            this.Controls.SetChildIndex( this.panelMain, 0 );
            this.Controls.SetChildIndex( this.panelSpecifyControls, 0 );
            this.Controls.SetChildIndex( this.progressPanel1, 0 );
            this.panelMain.ResumeLayout( false );
            this.panelTop.ResumeLayout( false );
            this.pnlRadioButtons.ResumeLayout( false );
            this.panelSpecifyControls.ResumeLayout( false );
            this.panelSpecify.ResumeLayout( false );
            this.panelSpecify.PerformLayout();
            this.ResumeLayout( false );

		}
        #endregion

        #region Constructors and Finalization
        public SelectByPayorBrokerView()
        {
            ResettingControls = true;
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
			this.searchField.Button.Text = "Sear&ch";
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
            CancelBackgroundWorker();
        }
        #endregion

        #region Data Elements

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container             components = null;

        private CheckBox               checkBoxOther;

        private ComboBox               comboBoxPlanCategories;

        private Panel                  panelMain;
        private Panel                  panelTop;
        private Panel                  panelSpecify;
        private Panel                  panelSpecifyControls;

        private Label                  lblShowPayors;
        private Label                  labelSpecifyOther;

        private SearchField searchField;
        private LineLabel   lineLblPayorBrokers;
        private PayorBrokersView payorBrokersView;
        private MaskedEditTextBox    mtbBoxSpecifyOther;

        private ReferenceValueComboBox                      i_InsurancePlanCategoriesHelper;
        private bool                                        i_ResettingControls = false;
        private bool                                        searchRadioButton;
        private bool                                        i_PreviousCategoryWasSpecialSearch;
        private ICollection                                 providersCollection = null;
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private BackgroundWorker searchbackgroundWorker = new BackgroundWorker();
      
        ICollection providers = null;
        #endregion

        #region Constants
        private const string 
            OTHER_SEARCHSTRING      = "OTHER",
            MEDICAID_SEARCHSTRING   = "MEDICAID",
            MEDICARE_SEARCHSTRING   = "MEDICARE",
            SELFPAY_SEARCHSTRING    = "",
            SELF_PAY                = "Self-pay";
        #endregion
		private Panel pnlRadioButtons;
		private RadioButton radioButtonBeginsWith;
        private ProgressPanel progressPanel1;
		private RadioButton radioButtonContains;
        private InsurancePlanCategory insurancePlanCategory = new InsurancePlanCategory();
        private ProgressPanel progressPanel2;
        private string searchFieldText = String.Empty;

    }
}
