using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.Remoting;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.PatientSearch
{
    /// <summary>
    /// Summary description for FindPatientByGuarantorView.
    /// </summary>
    //TODO: Create XML summary comment for FindPatientByGuarantorView
    [Serializable]
    public class FindPatientByGuarantorView : ControlView
    {
        #region Events
        public event EventHandler PatientsFound;
        public event EventHandler SearchReset;
        #endregion

        #region Event Handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindPatientByGuarantorView_Load(object sender, EventArgs e)
        {
            InitializeGendersComboBox();
        }

        private void FindPatientByGuarantorView_Layout(object sender, LayoutEventArgs e)
        {
            // TODO:  Evaluate.  This may not be the best place to do this... but it works.
            if (this.ParentForm != null && FirstTimeLayout)
            {
                //this.ParentForm.AcceptButton = btnSearch;
                base.AcceptButton = btnSearch;
                this.mtbGuarantorLastName.Select();
                FirstTimeLayout = false;
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            ValidateGuarantorSsn();
            if (i_ValidSsn)
            {
                // need to set the search button to disabled once the search begins. 
                // Why: because once a search is activated if you change 
                // the search criteria (eg. FirstName ) and hit the search button
                // the search does not recognize the change made to FirstName
                // and delivers the results for the first query.
                FindPatientsMatchingCriteria();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            i_Reset = true;

            ResetBackGroundColor();

            //Reset the Guarantor search results in listview from prior search.
            this.mtbGuarantorLastName.ResetText();
            this.mtbGuarantorFirstName.ResetText();

            this.btnSearch.Enabled = false;
            this.axMskGuarantorSsn.ResetText();
            //Reset the combobox.
            this.cmbGender.SelectedIndex = 0;
            this.SearchReset(this, null);
            this.Invalidate();
            this.Update();
            this.mtbGuarantorLastName.Select();
            i_Reset = false;
        }

        private void mtbGuarantorLastName_TextChanged(object sender, EventArgs e)
        {
            CheckForDataEntered();
        }

        private void mtbGuarantorFirstName_TextChanged(object sender, EventArgs e)
        {
            CheckForDataEntered();
        }

        private void cmbGender_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckForDataEntered();
            UpdateModel();
            UpdateView();
        }

        private void axMskGuarantorSsn_Change(object sender, EventArgs e)
        {
            CheckForDataEntered();
        }

        private void axMskGuarantorSsn_Validating(object sender, CancelEventArgs e)
        {
            ValidateGuarantorSsn();
        }

        private void mtbGuarantorLastName_Validating(object sender, CancelEventArgs e)
        {
            //Remove trailing spaces on lose focus
            mtbGuarantorLastName.Text = mtbGuarantorLastName.Text.TrimEnd(null);
        }

        private void mtbGuarantorFirstName_Validating(object sender, CancelEventArgs e)
        {
            //Remove trailing spaces on lose focus
            mtbGuarantorFirstName.Text = mtbGuarantorFirstName.Text.TrimEnd(null);
        }

        private void mtbGuarantorLastName_Enter(object sender, EventArgs e)
        {
            this.mtbGuarantorLastName.SelectionStart = this.mtbGuarantorLastName.TextLength;
        }

        private void mtbGuarantorFirstName_Enter(object sender, EventArgs e)
        {
            this.mtbGuarantorFirstName.SelectionStart = this.mtbGuarantorFirstName.TextLength;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Dispose method.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                // cancel the background worker here...
                if (this.backgroundWorker != null)
                {
                    this.backgroundWorker.CancelAsync();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// UpdateView method.
        /// </summary>
        public override void UpdateView()
        {
            //this.axMskGuarantorSsn.UnMaskedText = string.Empty;
            if (i_ValidSearchArgs & !i_Reset)
            {
                this.btnSearch.Enabled = true;
            }
            /*
            else
            {
                this.btnSearch.Enabled = false;
            }
            */
        }

        /// <summary>
        /// UpdateModel method.
        /// </summary>
        public override void UpdateModel()
        {
            if (!i_Reset)
            {
                ValidateSearchArgs();
            }
        }

        /// <summary>
        /// Set default focus.
        /// </summary>
        public void SetFocus()
        {
            mtbGuarantorLastName.Focus();
            //this.ParentForm.AcceptButton = btnSearch;
            base.AcceptButton = btnSearch;
        }
        #endregion

        #region Properties
        public GuarantorSearchCriteria guarantorSearchCriteria
        {
            get
            {
                return i_guarantorSearchCriteria;
            }
        }
        #endregion

        #region Private Methods

        private void BeforeWork()
        {
            if (this.Parent != null)
            {
                ((SearchView)this.Parent.Parent.Parent.Parent.Parent).ShowPanel();
            }
        }

        private void AfterWork(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.IsDisposed || this.Disposing)
                return;

            if (e.Cancelled)
            {
                // user cancelled
                // Note that due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.
            }
            else if (e.Error != null)
            {
                // handle errors/exceptions thrown from within the DoWork method here
                if (e.Error.GetType() == typeof(RemotingTimeoutException))
                {
                    MessageBox.Show(UIErrorMessages.PATIENT_SEARCH_TIMEOUT_MSG);
                    this.SearchReset(this, null);
                    this.DisplaySearchView();
                }
                else
                {
                    throw e.Error;
                }
            }
            else
            {
                // success
                if (PatientsFound != null)
                {
                    // Raise PatientsFound Event
                    PatientsFound(this, new LooseArgs(collectionOfPatients));
                }
                this.DisplaySearchView();
            }

            this.Cursor = Cursors.Default;

            //Reset Background colors on successful search criteria.
            ResetBackGroundColor();
        }

        private void DoFindMatchingPatients(object sender, DoWorkEventArgs e)
        {
            PatientBrokerProxy broker = new PatientBrokerProxy();

            //Call PatientsMatching on broker (returns patients).
            //collectionOfPatients = broker.PatientsMatchingGuarantor(this.guarantorCriteria);

            this.guarantorSearchResponse = broker.GetPatientSearchResponseFor(this.guarantorCriteria);
            collectionOfPatients = guarantorSearchResponse.PatientSearchResults;

            // poll CancellationPending and set e.Cancel to true and return 
            if (this.backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            /*
                        try
                        {
                            PatientBrokerProxy broker = new PatientBrokerProxy();

                            //Call PatientsMatching on broker (returns patients).
                            collectionOfPatients = broker.PatientsMatchingGuarantor( this.guarantorCriteria );
                        }
                        catch
                        {
                            throw;
                        }
             */
        }

        private void FindPatientsMatchingCriteria()
        {
            Gender gender = null;
            if (cmbGender.SelectedItem != null && !cmbGender.SelectedItem.Equals(String.Empty))//cmbGender.SelectedItem != ""
            {
                gender = (Gender)((DictionaryEntry)cmbGender.SelectedItem).Key;
            }

            //Create PatientSearchCriteria and pass in all args.
            guarantorCriteria = new GuarantorSearchCriteria(
                User.GetCurrent().Facility.Code,
                this.mtbGuarantorFirstName.Text,
                this.mtbGuarantorLastName.Text,
                gender,
                this.axMskGuarantorSsn.UnMaskedText
                );

            ValidationResult result = guarantorCriteria.Validate();

            if (result.IsValid)
            {
                if (this.backgroundWorker == null
                    ||
                    (this.backgroundWorker != null
                    && !this.backgroundWorker.IsBusy)
                    )
                {
                    this.BeforeWork();

                    this.backgroundWorker = new BackgroundWorker();
                    this.backgroundWorker.WorkerSupportsCancellation = true;

                    backgroundWorker.DoWork += DoFindMatchingPatients;
                    backgroundWorker.RunWorkerCompleted += AfterWork;

                    backgroundWorker.RunWorkerAsync();
                }
            }
            else
            {
                this.SearchReset(this, null);
                string errorMsg = result.Message;
                if (errorMsg != string.Empty)
                {
                    MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    SearchError(result.AspectInError);
                }
            }
        }

        private void ValidateGuarantorSsn()
        {
            i_ValidSsn = true;
            if (this.axMskGuarantorSsn.UnMaskedText.Length == 0)
            {
                UIColors.SetNormalBgColor(axMskGuarantorSsn);
                return;
            }

            Gender gender = null;
            if (cmbGender.SelectedItem != null && !cmbGender.SelectedItem.Equals(String.Empty))//cmbGender.SelectedItem != ""
            {
                gender = (Gender)((DictionaryEntry)cmbGender.SelectedItem).Key;
            }

            //Create PatientSearchCriteria and pass in all args.
            guarantorCriteria = new GuarantorSearchCriteria(
                User.GetCurrent().Facility.Code,
                this.mtbGuarantorFirstName.Text,
                this.mtbGuarantorLastName.Text,
                gender,
                this.axMskGuarantorSsn.UnMaskedText
                );

            ValidationResult result = guarantorCriteria.Validate();
            if (result.IsValid)
            {
                i_ValidSsn = true;
                UIColors.SetNormalBgColor(axMskGuarantorSsn);
            }
            else
            {
                string errorMsg = result.Message;
                i_ValidSearchArgs = false;
                UIColors.SetErrorBgColor(axMskGuarantorSsn);
                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                axMskGuarantorSsn.Focus();
                //ResetBackGroundColor();
                //e.Cancel = true;
                i_ValidSsn = false;
            }
        }

        private void CheckForDataEntered()
        {
            if (this.mtbGuarantorLastName.Text == string.Empty && this.mtbGuarantorFirstName.Text == string.Empty &&
                (this.cmbGender.SelectedItem == null || this.cmbGender.SelectedItem.Equals(String.Empty))
                && this.axMskGuarantorSsn.UnMaskedText ==   string.Empty)  
            {
                this.btnSearch.Enabled = false;
            }
            else
            {
                this.btnSearch.Enabled = true;
                if (this.ParentForm != null)
                {
                    //this.ParentForm.AcceptButton = btnSearch;
                    base.AcceptButton = btnSearch;
                }
            }
        }

        private void SearchError(string AspectInError)
        {
            switch (AspectInError)
            {
                case "FirstName":
                    this.mtbGuarantorFirstName.Select();
                    break;
                case "LastName":
                    this.mtbGuarantorLastName.Select();
                    break;
                case "Gender":
                    this.cmbGender.Select();
                    break;
                default:
                    break;
            }
        }

        private void InitializeGendersComboBox()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }
            if (cmbGender.Items.Count == 0)
            {
                //IGenderBroker broker = BrokerFactory.BrokerOfType<IGenderBroker >();
                DemographicsBrokerProxy broker = new DemographicsBrokerProxy();

                ICollection genders = (ICollection)broker.AllTypesOfGenders(User.GetCurrent().Facility.Oid);

                cmbGender.ValueMember = "Key";
                cmbGender.DisplayMember = "Value";

                //                cmbGender.Items.Add("");
                //                cmbGender.SelectedItem = "";

                foreach (Gender gender in genders)
                {
                    cmbGender.Items.Add(gender.AsDictionaryEntry());
                }
            }
        }

        private void ResetBackGroundColor()
        {
            //Set background color back to white.
            UIColors.SetNormalBgColor(axMskGuarantorSsn);
            UIColors.SetNormalBgColor(mtbGuarantorFirstName);
            UIColors.SetNormalBgColor(mtbGuarantorLastName);
            UIColors.SetNormalBgColor(cmbGender);
        }

        private void ValidateSearchArgs()
        {
            Gender gender = null;

            if (cmbGender.SelectedItem != null && !cmbGender.SelectedItem.Equals(String.Empty)) //cmbGender.SelectedItem != ""
            {
                gender = (Gender)((DictionaryEntry)cmbGender.SelectedItem).Key;
            }

            guarantorCriteria = new GuarantorSearchCriteria(
                User.GetCurrent().Facility.Code,
                this.mtbGuarantorFirstName.Text,
                this.mtbGuarantorLastName.Text,
                gender,
                this.axMskGuarantorSsn.UnMaskedText
                );

            //Check to see if search data entered is valid.
            i_ValidSearchArgs = true;
            ValidationResult result = guarantorCriteria.Validate();
            if (result.IsValid)
            {
                i_ValidSearchArgs = true;
            }
            else
            {
                i_ValidSearchArgs = false;
            }
        }

        private void DisplaySearchView()
        {
            if (this.Parent != null)
            {
                ((SearchView)this.Parent.Parent.Parent.Parent.Parent).HidePanel();
            }
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureFirstNameAndLastName( mtbGuarantorFirstName );
            MaskedEditTextBoxBuilder.ConfigureFirstNameAndLastName( mtbGuarantorLastName );
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Initialize FindPatientByGuarantorView, call FindPatientByGuarantorView_Load in Event Handlers.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblInstructions = new System.Windows.Forms.Label();
            this.lblLastName = new System.Windows.Forms.Label();
            this.lblSsn = new System.Windows.Forms.Label();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.lblGender = new System.Windows.Forms.Label();
            this.cmbGender = new System.Windows.Forms.ComboBox();
            this.btnSearch = new LoggingButton();
            this.btnReset = new LoggingButton();
            this.mtbGuarantorLastName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbGuarantorFirstName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.axMskGuarantorSsn = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.SuspendLayout();
            // 
            // lblInstructions
            // 
            this.lblInstructions.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblInstructions.Location = new System.Drawing.Point(0, 17);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(933, 31);
            this.lblInstructions.TabIndex = 0;
            this.lblInstructions.Text = "The more information provided, the narrower the search.  Partial searches are not" +
                " supported for SSN.  When searching without a Guarantor SSN, the Guarantor Last " +
                "Name, First Name, and Gender are required.";
            // 
            // lblLastName
            // 
            this.lblLastName.Location = new System.Drawing.Point(7, 59);
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size(120, 16);
            this.lblLastName.TabIndex = 0;
            this.lblLastName.Text = "Guarantor last name:";
            // 
            // lblSsn
            // 
            this.lblSsn.Location = new System.Drawing.Point(7, 100);
            this.lblSsn.Name = "lblSsn";
            this.lblSsn.Size = new System.Drawing.Size(96, 16);
            this.lblSsn.TabIndex = 0;
            this.lblSsn.Text = "Guarantor SSN:";
            // 
            // lblFirstName
            // 
            this.lblFirstName.Location = new System.Drawing.Point(409, 59);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(118, 16);
            this.lblFirstName.TabIndex = 0;
            this.lblFirstName.Text = "Guarantor first name:";
            // 
            // lblGender
            // 
            this.lblGender.Location = new System.Drawing.Point(703, 59);
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size(102, 16);
            this.lblGender.TabIndex = 0;
            this.lblGender.Text = "Guarantor gender:";
            // 
            // cmbGender
            // 
            this.cmbGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGender.Location = new System.Drawing.Point(798, 57);
            this.cmbGender.Name = "cmbGender";
            this.cmbGender.Size = new System.Drawing.Size(84, 21);
            this.cmbGender.TabIndex = 3;
            this.cmbGender.SelectedIndexChanged += new System.EventHandler(this.cmbGender_SelectedIndexChanged);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearch.Enabled = false;
            this.btnSearch.Location = new System.Drawing.Point(801, 145);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Text = "Sear&ch";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnReset.CausesValidation = false;
            this.btnReset.Location = new System.Drawing.Point(883, 145);
            this.btnReset.Name = "btnReset";
            this.btnReset.TabIndex = 6;
            this.btnReset.Text = "Rese&t";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // mtbGuarantorLastName
            // 
            this.mtbGuarantorLastName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbGuarantorLastName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbGuarantorLastName.Location = new System.Drawing.Point(114, 57);
            this.mtbGuarantorLastName.Mask = "";
            this.mtbGuarantorLastName.MaxLength = 25;
            this.mtbGuarantorLastName.Name = "mtbGuarantorLastName";
            this.mtbGuarantorLastName.Size = new System.Drawing.Size(282, 20);
            this.mtbGuarantorLastName.TabIndex = 1;
            this.mtbGuarantorLastName.Validating += new System.ComponentModel.CancelEventHandler(this.mtbGuarantorLastName_Validating);
            this.mtbGuarantorLastName.TextChanged += new System.EventHandler(this.mtbGuarantorLastName_TextChanged);
            this.mtbGuarantorLastName.Enter += new System.EventHandler(this.mtbGuarantorLastName_Enter);
            // 
            // mtbGuarantorFirstName
            // 
            this.mtbGuarantorFirstName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbGuarantorFirstName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbGuarantorFirstName.Location = new System.Drawing.Point(518, 57);
            this.mtbGuarantorFirstName.Mask = "";
            this.mtbGuarantorFirstName.MaxLength = 15;
            this.mtbGuarantorFirstName.Name = "mtbGuarantorFirstName";
            this.mtbGuarantorFirstName.Size = new System.Drawing.Size(172, 20);
            this.mtbGuarantorFirstName.TabIndex = 2;
            this.mtbGuarantorFirstName.Validating += new System.ComponentModel.CancelEventHandler(this.mtbGuarantorFirstName_Validating);
            this.mtbGuarantorFirstName.TextChanged += new System.EventHandler(this.mtbGuarantorFirstName_TextChanged);
            this.mtbGuarantorFirstName.Enter += new System.EventHandler(this.mtbGuarantorFirstName_Enter);
            // 
            // axMskGuarantorSsn
            // 
            this.axMskGuarantorSsn.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.axMskGuarantorSsn.KeyPressExpression = "^\\d*$";
            this.axMskGuarantorSsn.Location = new System.Drawing.Point(88, 98);
            this.axMskGuarantorSsn.Mask = "   -  -";
            this.axMskGuarantorSsn.MaxLength = 11;
            this.axMskGuarantorSsn.Name = "axMskGuarantorSsn";
            this.axMskGuarantorSsn.Size = new System.Drawing.Size(75, 20);
            this.axMskGuarantorSsn.TabIndex = 4;
            this.axMskGuarantorSsn.ValidationExpression = "^\\d*$";
            this.axMskGuarantorSsn.Validating += new System.ComponentModel.CancelEventHandler(this.axMskGuarantorSsn_Validating);
            this.axMskGuarantorSsn.TextChanged += new System.EventHandler(this.axMskGuarantorSsn_Change);
            // 
            // FindPatientByGuarantorView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.axMskGuarantorSsn);
            this.Controls.Add(this.mtbGuarantorLastName);
            this.Controls.Add(this.mtbGuarantorFirstName);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.cmbGender);
            this.Controls.Add(this.lblGender);
            this.Controls.Add(this.lblLastName);
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.lblFirstName);
            this.Controls.Add(this.lblSsn);
            this.Name = "FindPatientByGuarantorView";
            this.Size = new System.Drawing.Size(990, 176);
            this.Load += new System.EventHandler(this.FindPatientByGuarantorView_Load);
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.FindPatientByGuarantorView_Layout);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties

        private GuarantorSearchCriteria guarantorCriteria
        {
            get
            {
                return this.i_guarantorSearchCriteria;
            }
            set
            {
                this.i_guarantorSearchCriteria = value;
            }
        }

        private bool FirstTimeLayout
        {
            get
            {
                return i_FirstTimeLayout;
            }
            set
            {
                i_FirstTimeLayout = value;
            }
        }

        #endregion

        #region Construction and Finalization
        /// <summary>
        /// Contruction for FindPatientByGuarantorView,
        /// calls InitializeComponent in Private Methods region.
        /// </summary>
        public FindPatientByGuarantorView()
        {
            InitializeComponent();

            ConfigureControls();
        }

        #endregion

        #region Data Elements

        private Container components = null;

        private BackgroundWorker backgroundWorker;

        private GuarantorSearchCriteria i_guarantorSearchCriteria;

        private Label lblInstructions;
        private Label lblLastName;
        private Label lblFirstName;
        private Label lblGender;
        private Label lblSsn;

        private ComboBox cmbGender;
        private LoggingButton btnSearch;
        private LoggingButton btnReset;

        private MaskedEditTextBox mtbGuarantorLastName;
        private MaskedEditTextBox mtbGuarantorFirstName;
        private MaskedEditTextBox axMskGuarantorSsn;

        private bool i_FirstTimeLayout = true;
        private bool i_ValidSearchArgs;
        private bool i_Reset = false;
        private bool i_ValidSsn = false;

        private Regex i_LastNameRegex = new Regex(@"^[a-z].*");
        private Regex i_FirstNameRegex = new Regex(@"^[a-z].*");

        private ICollection collectionOfPatients = null;
        private PatientSearchResponse guarantorSearchResponse = null;
        #endregion

        #region Constants
        #endregion
    }
}
