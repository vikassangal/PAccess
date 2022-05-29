using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.InsuranceViews.SelectBillingInfo;

namespace PatientAccess.UI.InsuranceViews
{
    [Serializable]
    public class BillingInformationView : ControlView
    {
        #region Events
        public event EventHandler BillingAddressSelectedEvent;
        #endregion

        #region Event Handlers
        private void chkBoxCopyFrom_CheckedChanged( object sender, EventArgs e )
        {
            CheckBox cb = sender as CheckBox;

            if( cb.Checked )
            {
                UIColors.SetNormalBgColor( this.maskEditBillingCoName );
                UIColors.SetNormalBgColor( this.maskEditBillingName );

                maskEditBillingCoName.Enabled = false;
                maskEditBillingName.Enabled = false;
                btnSelect.Enabled = false;

                this.lblBillingInfoAvailable.Visible = true;

                maskEditBillingCoName.Text = String.Empty;
                maskEditBillingName.Text = String.Empty;
            }
            else
            {
                this.lblBillingInfoAvailable.Visible = false;

                maskEditBillingCoName.Enabled = true;
                maskEditBillingName.Enabled = true;

                this.CheckForPlanBillingInformations();

                this.RunRules();
            }
        }

        /// <summary>
        /// billingInfoDialog_BillingAddressSelected - this method handles the selected item from the
        /// popup billingInfoDialog (SelectBillingInfoView).  It's purpose is to update the UI textboxes
        /// with the appropriate info AND pass the selected address on to the AddressView 
        /// (via the BillingAddressSelectedEvent).
        /// </summary>
        private void billingInfoDialog_BillingAddressSelected( object sender, EventArgs e )
        {
            LooseArgs args = (LooseArgs)e;
            selectedBillingInformation = (BillingInformation)args.Context;

            if( selectedBillingInformation != null )
            {
                this.Model_Coverage.BillingInformation = selectedBillingInformation;

                maskEditBillingCoName.Text = selectedBillingInformation.BillingCOName;
                maskEditBillingName.Text = selectedBillingInformation.BillingName;

                // throw the event up to the parent (InsDetailPlanDetails) so that it can update
                // the address view

                if( BillingAddressSelectedEvent != null )
                {
                    LooseArgs la = new LooseArgs( this.Model_Coverage.BillingInformation );
                    BillingAddressSelectedEvent( this, la );
                }
            }

            this.RunRules();
        }

        private void btnSelect_Click( object sender, EventArgs e )
        {
            billingInfoDialog.Model_Coverage = this.Model_Coverage;
            billingInfoDialog.ShowDialog( this );
        }

        private void maskEditBillingCoName_Validating( object sender, CancelEventArgs e )
        {
            if( Model_Coverage.BillingInformation != null )
            {
                this.Model_Coverage.BillingInformation.BillingCOName = this.maskEditBillingCoName.Text;
                this.RunRules();
            }
        }

        private void maskEditBillingName_Validating( object sender, CancelEventArgs e )
        {
            if( Model_Coverage.BillingInformation != null )
            {
                this.Model_Coverage.BillingInformation.BillingName = this.maskEditBillingName.Text;
                this.RunRules();
            }
        }

        private void BillingCONamePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( this.maskEditBillingCoName );
        }

        private void BillingCONameRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( this.maskEditBillingCoName );
        }

        private void BillingNamePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( this.maskEditBillingName );
        }

        private void BillingNameRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( this.maskEditBillingName );
        }

        #endregion

        #region Methods

        public override void UpdateModel()
        {
            Model_Coverage.BillingInformation.BillingCOName = this.maskEditBillingCoName.UnMaskedText.Trim();
            Model_Coverage.BillingInformation.BillingName = this.maskEditBillingName.UnMaskedText.Trim();
        }

        public override void UpdateView()
        {
            RegisterRulesEvents();

            selectedBillingInformation = null;
            if( Model_Coverage == null )
            {
                return;
            }
            // Copy from Guarantor CheckBox is disabled unless coverage is Self Pay
            chkBoxCopyFrom.Enabled = Model_Coverage.InsurancePlan.GetType() == typeof( SelfPayInsurancePlan );
            lblBillingInfoAvailable.Visible = chkBoxCopyFrom.Enabled;

            this.CheckForPlanBillingInformations();

            if( Model_Coverage.BillingInformation != null )
            {
                maskEditBillingCoName.UnMaskedText = this.Model_Coverage.BillingInformation.BillingCOName;
                maskEditBillingName.UnMaskedText = this.Model_Coverage.BillingInformation.BillingName;
            }
            else
            {
                // If we have only 1 billing information, display it
                if( Model_Coverage.InsurancePlan.BillingInformations.Count == 1 )
                {
                    ICollection billingCollection = Model_Coverage.InsurancePlan.BillingInformations;
                    foreach( BillingInformation billing in billingCollection )
                    {
                        if( billing == null )
                        {
                            break;
                        }

                        maskEditBillingCoName.Text = billing.BillingCOName;
                        maskEditBillingName.Text = billing.BillingName;

                        // Format an address to display on the AddressView form.
                        if( BillingAddressSelectedEvent != null )
                        {
                            BillingAddressSelectedEvent( this, new LooseArgs( this.Model_Coverage.BillingInformation ) );
                        }
                    }
                }
            }

            this.RunRules();
        }

        public void ResetView()
        {
            chkBoxCopyFrom.Checked = false;

            maskEditBillingCoName.UnMaskedText = String.Empty;
            maskEditBillingName.UnMaskedText = String.Empty;
        }
        #endregion

        #region Properties
        public Coverage Model_Coverage
        {
            set
            {
                this.Model = value;
            }
            get
            {
                return (Coverage)this.Model;
            }
        }

        #endregion

        #region Private Methods

        private void CheckForPlanBillingInformations()
        {
            if( Model_Coverage.InsurancePlan != null &&
                Model_Coverage.InsurancePlan.BillingInformations != null )
            {
                btnSelect.Enabled = Model_Coverage.InsurancePlan.BillingInformations.Count > 0;

                if( btnSelect.Enabled )
                {
                    this.lblBillingInfoAvailable.Visible = true;
                }
                else
                {
                    this.lblBillingInfoAvailable.Visible = false;
                }
            }

        }

        private void RegisterRulesEvents()
        {
            RuleEngine.GetInstance().RegisterEvent( typeof( BillingCONamePreferred ), this.Model_Coverage, new EventHandler( BillingCONamePreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( BillingCONameRequired ), this.Model_Coverage, new EventHandler( BillingCONameRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( BillingNamePreferred ), this.Model_Coverage, new EventHandler( BillingNamePreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( BillingNameRequired ), this.Model_Coverage, new EventHandler( BillingNameRequiredEventHandler ) );
        }

        private void UnRegisterRulesEvents()
        {
            // UNREGISTER EVENTS                        
            RuleEngine.GetInstance().UnregisterEvent( typeof( BillingCONamePreferred ), this.Model_Coverage, new EventHandler( BillingCONamePreferredEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( BillingCONameRequired ), this.Model_Coverage, new EventHandler( BillingCONameRequiredEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( BillingNamePreferred ), this.Model_Coverage, new EventHandler( BillingNamePreferredEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( BillingNameRequired ), this.Model_Coverage, new EventHandler( BillingNameRequiredEventHandler ) );
        }

        public void RunRules()
        {
            // reset all fields that might have error, preferred, or required backgrounds

            UIColors.SetNormalBgColor( this.maskEditBillingCoName );
            UIColors.SetNormalBgColor( this.maskEditBillingName );

            this.Refresh();

            RuleEngine.GetInstance().EvaluateRule( typeof( BillingCONamePreferred ), this.Model_Coverage );
            RuleEngine.GetInstance().EvaluateRule( typeof( BillingCONameRequired ), this.Model_Coverage );
            RuleEngine.GetInstance().EvaluateRule( typeof( BillingNamePreferred ), this.Model_Coverage );
            RuleEngine.GetInstance().EvaluateRule( typeof( BillingNameRequired ), this.Model_Coverage );
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelMain = new System.Windows.Forms.Panel();
            this.maskEditBillingName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblIncludeIfDifferent = new System.Windows.Forms.Label();
            this.lblBillingName2 = new System.Windows.Forms.Label();
            this.maskEditBillingCoName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblBillingName1 = new System.Windows.Forms.Label();
            this.lblBillingInfoAvailable = new System.Windows.Forms.Label();
            this.btnSelect = new LoggingButton();
            this.chkBoxCopyFrom = new System.Windows.Forms.CheckBox();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add( this.maskEditBillingName );
            this.panelMain.Controls.Add( this.lblIncludeIfDifferent );
            this.panelMain.Controls.Add( this.lblBillingName2 );
            this.panelMain.Controls.Add( this.maskEditBillingCoName );
            this.panelMain.Controls.Add( this.lblBillingName1 );
            this.panelMain.Controls.Add( this.lblBillingInfoAvailable );
            this.panelMain.Controls.Add( this.btnSelect );
            this.panelMain.Controls.Add( this.chkBoxCopyFrom );
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point( 0, 0 );
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size( 452, 130 );
            this.panelMain.TabIndex = 0;
            // 
            // maskEditBillingName
            // 
            this.maskEditBillingName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.maskEditBillingName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.maskEditBillingName.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.maskEditBillingName.Location = new System.Drawing.Point( 83, 102 );
            this.maskEditBillingName.Mask = "";
            this.maskEditBillingName.MaxLength = 40;
            this.maskEditBillingName.Name = "maskEditBillingName";
            this.maskEditBillingName.Size = new System.Drawing.Size( 365, 20 );
            this.maskEditBillingName.TabIndex = 4;
            this.maskEditBillingName.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.maskEditBillingName.Validating += new System.ComponentModel.CancelEventHandler( this.maskEditBillingName_Validating );
            // 
            // lblIncludeIfDifferent
            // 
            this.lblIncludeIfDifferent.Location = new System.Drawing.Point( 83, 84 );
            this.lblIncludeIfDifferent.Name = "lblIncludeIfDifferent";
            this.lblIncludeIfDifferent.Size = new System.Drawing.Size( 176, 23 );
            this.lblIncludeIfDifferent.TabIndex = 6;
            this.lblIncludeIfDifferent.Text = "Include if different from above";
            // 
            // lblBillingName2
            // 
            this.lblBillingName2.Location = new System.Drawing.Point( 0, 105 );
            this.lblBillingName2.Name = "lblBillingName2";
            this.lblBillingName2.Size = new System.Drawing.Size( 72, 23 );
            this.lblBillingName2.TabIndex = 7;
            this.lblBillingName2.Text = "Billing name:";
            // 
            // maskEditBillingCoName
            // 
            this.maskEditBillingCoName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.maskEditBillingCoName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.maskEditBillingCoName.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.maskEditBillingCoName.Location = new System.Drawing.Point( 83, 59 );
            this.maskEditBillingCoName.Mask = "";
            this.maskEditBillingCoName.MaxLength = 25;
            this.maskEditBillingCoName.Name = "maskEditBillingCoName";
            this.maskEditBillingCoName.Size = new System.Drawing.Size( 365, 20 );
            this.maskEditBillingCoName.TabIndex = 3;
            this.maskEditBillingCoName.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.maskEditBillingCoName.Validating += new System.ComponentModel.CancelEventHandler( this.maskEditBillingCoName_Validating );
            // 
            // lblBillingName1
            // 
            this.lblBillingName1.Location = new System.Drawing.Point( 0, 62 );
            this.lblBillingName1.Name = "lblBillingName1";
            this.lblBillingName1.Size = new System.Drawing.Size( 88, 23 );
            this.lblBillingName1.TabIndex = 4;
            this.lblBillingName1.Text = "Billing c/o name:";
            // 
            // lblBillingInfoAvailable
            // 
            this.lblBillingInfoAvailable.Location = new System.Drawing.Point( 96, 31 );
            this.lblBillingInfoAvailable.Name = "lblBillingInfoAvailable";
            this.lblBillingInfoAvailable.Size = new System.Drawing.Size( 272, 23 );
            this.lblBillingInfoAvailable.TabIndex = 3;
            this.lblBillingInfoAvailable.Text = "Billing information is available to select";
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point( 0, 27 );
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.TabIndex = 2;
            this.btnSelect.Text = "&Select...";
            this.btnSelect.Click += new System.EventHandler( this.btnSelect_Click );
            // 
            // chkBoxCopyFrom
            // 
            this.chkBoxCopyFrom.Enabled = false;
            this.chkBoxCopyFrom.Location = new System.Drawing.Point( 0, 0 );
            this.chkBoxCopyFrom.Name = "chkBoxCopyFrom";
            this.chkBoxCopyFrom.Size = new System.Drawing.Size( 304, 24 );
            this.chkBoxCopyFrom.TabIndex = 1;
            this.chkBoxCopyFrom.Text = "Copy from validated Guarantor Information";
            this.chkBoxCopyFrom.CheckedChanged += new System.EventHandler( this.chkBoxCopyFrom_CheckedChanged );
            // 
            // BillingInformationView
            // 
            this.Controls.Add( this.panelMain );
            this.Name = "BillingInformationView";
            this.Size = new System.Drawing.Size( 452, 130 );
            this.panelMain.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion

        #endregion

        #region Construction and Finalization
        public BillingInformationView()
        {
            InitializeComponent();
            billingInfoDialog = new SelectBillingInfoView();
            billingInfoDialog.AddressSelectedEvent += new EventHandler( this.billingInfoDialog_BillingAddressSelected );
        }

        protected override void Dispose( bool disposing )
        {
            this.UnRegisterRulesEvents();

            if( disposing )
            {
                if( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );

            if( billingInfoDialog != null )
            {
                billingInfoDialog.Dispose();
            }
        }
        #endregion

        #region Data Elements
        private Container components = null;
        private CheckBox chkBoxCopyFrom;
        private LoggingButton btnSelect;
        private Label lblBillingInfoAvailable;
        private Label lblBillingName2;
        private Label lblBillingName1;
        private Label lblIncludeIfDifferent;
        private MaskedEditTextBox maskEditBillingCoName;
        private MaskedEditTextBox maskEditBillingName;
        private Panel panelMain;
        private SelectBillingInfoView billingInfoDialog;
        private BillingInformation selectedBillingInformation;

        #endregion

        #region Constants
        #endregion
    }
}
