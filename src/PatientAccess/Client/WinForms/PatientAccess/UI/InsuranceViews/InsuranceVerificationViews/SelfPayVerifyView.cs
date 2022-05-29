using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.InsuranceViews.InsuranceVerificationViews
{
    /// <summary>
    /// Summary description for SelfPayVerifyView.
    /// </summary>
    public class SelfPayVerifyView : ControlView
    {
        #region Event Handlers

        private void SelfPayPatientHasMedicaidPreferredEventHandler (object sender, EventArgs e)
        {
            UIColors.SetPreferredBgColor( this.cmbMedicaid );
        }

        private void SelfPayPatientInfoUnavailablePreferredEventHandler (object sender, EventArgs e)
        {
            UIColors.SetPreferredBgColor( this.cmbInsuranceInfo );
        }

        #endregion

        #region Methods

        private void cmbMedicaid_SelectedIndexChanged(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor( this.cmbMedicaid );

            ComboBox cb = sender as ComboBox;

            patientHasMedicaid = cb.SelectedItem as YesNoFlag;
            if( this.patientHasMedicaid == null )
            {
                patientHasMedicaid = new YesNoFlag();
            }

            if( this.Model_Coverage != null
                && this.Model_Coverage.CoverageOrder != null
                && this.Model_Coverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID 
                && this.patientHasMedicaid.Code == YesNoFlag.CODE_YES )
            {
                MessageBox.Show( UIErrorMessages.MEDICAID_BUT_SELFPAY, "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1 );
            }

            Model_Coverage.PatientHasMedicaid = patientHasMedicaid;

            RuleEngine.GetInstance().EvaluateRule( typeof(SelfPayPatientHasMedicaidPreferred), this.Model_Coverage );            
        }

        private void cmbInsuranceInfo_SelectedIndexChanged(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor( this.cmbInsuranceInfo );

            ComboBox cb = sender as ComboBox;
            patientInsuranceUnavailable = cb.SelectedItem as YesNoFlag;

            this.Model_Coverage.InsuranceInfoUnavailable    = patientInsuranceUnavailable;
            RuleEngine.GetInstance().EvaluateRule( typeof(SelfPayPatientInfoUnavailablePreferred), this.Model_Coverage );
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            ResetView();        
        }

        public override void UpdateView()
        {
            if( loadingModelData )
            {
                loadingModelData = false;

                PopulateMedicaidInfoComboBox();
                PopulateInsuranceInfoComboBox();
                
                cmbMedicaid.SelectedItem            = Model_Coverage.PatientHasMedicaid;
                this.patientHasMedicaid             = Model_Coverage.PatientHasMedicaid;


                cmbInsuranceInfo.SelectedItem       = Model_Coverage.InsuranceInfoUnavailable;
                this.patientInsuranceUnavailable    = Model_Coverage.InsuranceInfoUnavailable;

                RuleEngine.GetInstance().RegisterEvent( typeof(SelfPayPatientHasMedicaidPreferred), new EventHandler( SelfPayPatientHasMedicaidPreferredEventHandler));
                RuleEngine.GetInstance().RegisterEvent( typeof(SelfPayPatientInfoUnavailablePreferred), new EventHandler( SelfPayPatientInfoUnavailablePreferredEventHandler));

                RuleEngine.GetInstance().EvaluateRule( typeof(SelfPayPatientHasMedicaidPreferred), this.Model_Coverage );
                RuleEngine.GetInstance().EvaluateRule( typeof(SelfPayPatientInfoUnavailablePreferred), this.Model_Coverage );
            }
        }

        public override void UpdateModel()
        {
            if( patientHasMedicaid != null )
            {
                Model_Coverage.PatientHasMedicaid = patientHasMedicaid;
            }
            if( patientInsuranceUnavailable != null )
            {
                Model_Coverage.InsuranceInfoUnavailable = patientInsuranceUnavailable;
            }
        }
        #endregion

        #region Properties
        [Browsable(false)]
        private SelfPayCoverage Model_Coverage
        {
            set
            {
                this.Model = value;
            }
            get
            {
                return (SelfPayCoverage)this.Model;
            }
        }

        [Browsable(false)]
        public Account Account
        {
            get
            {
                return i_Account;
            }
            set
            {
                i_Account = value;
            }
        }
        #endregion

        #region Private Methods
        private void PopulateInsuranceInfoComboBox()
        {
            cmbInsuranceInfo.Items.Add( blankYesNoFlag );
            cmbInsuranceInfo.Items.Add( yesYesNoFlag );
            cmbInsuranceInfo.Items.Add( noYesNoFlag );
        }

        private void PopulateMedicaidInfoComboBox()
        {
            cmbMedicaid.Items.Add( blankYesNoFlag );
            cmbMedicaid.Items.Add( yesYesNoFlag );
            cmbMedicaid.Items.Add( noYesNoFlag );
        }

        private void ResetView()
        {
            //cmbInsuranceInfo.SelectedIndex = -1;
            //cmbMedicaid.SelectedIndex      = -1;
            this.Model_Coverage.RemoveCoverageVerificationData();
            loadingModelData = true;
            this.UpdateView();
            
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.lblMedicare = new System.Windows.Forms.Label();
            this.cmbMedicaid = new System.Windows.Forms.ComboBox();
            this.lblInsurance = new System.Windows.Forms.Label();
            this.cmbInsuranceInfo = new System.Windows.Forms.ComboBox();
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnClearAll = new LoggingButton();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(640, 23);
            this.label2.TabIndex = 0;
            this.label2.Text = "Click the Initiate button for System Electronic Verification Results to ensure th" +
                "at the Insured does not have Medicaid coverage.";
            // 
            // lblMedicare
            // 
            this.lblMedicare.Location = new System.Drawing.Point(8, 37);
            this.lblMedicare.Name = "lblMedicare";
            this.lblMedicare.Size = new System.Drawing.Size(113, 23);
            this.lblMedicare.TabIndex = 0;
            this.lblMedicare.Text = "Patient has Medicaid:";
            // 
            // cmbMedicaid
            // 
            this.cmbMedicaid.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMedicaid.Location = new System.Drawing.Point(119, 34);
            this.cmbMedicaid.Name = "cmbMedicaid";
            this.cmbMedicaid.Size = new System.Drawing.Size(50, 21);
            this.cmbMedicaid.TabIndex = 1;
            this.cmbMedicaid.SelectedIndexChanged += new System.EventHandler(this.cmbMedicaid_SelectedIndexChanged);
            // 
            // lblInsurance
            // 
            this.lblInsurance.Location = new System.Drawing.Point(8, 63);
            this.lblInsurance.Name = "lblInsurance";
            this.lblInsurance.Size = new System.Drawing.Size(236, 23);
            this.lblInsurance.TabIndex = 0;
            this.lblInsurance.Text = "Patient\'s insurance information is unavailable:";
            // 
            // cmbInsuranceInfo
            // 
            this.cmbInsuranceInfo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInsuranceInfo.Location = new System.Drawing.Point(236, 60);
            this.cmbInsuranceInfo.Name = "cmbInsuranceInfo";
            this.cmbInsuranceInfo.Size = new System.Drawing.Size(50, 21);
            this.cmbInsuranceInfo.TabIndex = 2;
            this.cmbInsuranceInfo.SelectedIndexChanged += new System.EventHandler(this.cmbInsuranceInfo_SelectedIndexChanged);
            // 
            // lblInfo
            // 
            this.lblInfo.Location = new System.Drawing.Point(8, 89);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(496, 23);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "If the patient has no additional unverified coverage, proceed with assessing Pati" +
                "ent Liability.";
            // 
            // btnClearAll
            // 
            this.btnClearAll.Location = new System.Drawing.Point(740, 85);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.TabIndex = 3;
            this.btnClearAll.Text = "&Clear All";
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            // 
            // SelfPayVerifyView
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnClearAll);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.cmbInsuranceInfo);
            this.Controls.Add(this.lblInsurance);
            this.Controls.Add(this.cmbMedicaid);
            this.Controls.Add(this.lblMedicare);
            this.Controls.Add(this.label2);
            this.Name = "SelfPayVerifyView";
            this.Size = new System.Drawing.Size(847, 130);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Construction and Finalization
        public SelfPayVerifyView()
        {
            InitializeComponent();
            loadingModelData = true;
            blankYesNoFlag = new YesNoFlag();
            blankYesNoFlag.SetBlank();
            yesYesNoFlag = new YesNoFlag();
            yesYesNoFlag.SetYes();
            noYesNoFlag = new YesNoFlag();
            noYesNoFlag.SetNo();
            base.EnableThemesOn( this );
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
        }
        #endregion

        #region Data Elements
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container     components = null;

        private Label          label2;
        private Label          lblMedicare;
        private Label          lblInsurance;
        private Label          lblInfo;
        private LoggingButton         btnClearAll;

        private ComboBox       cmbInsuranceInfo;
        private ComboBox       cmbMedicaid;

        private bool                                loadingModelData;
        private Account                             i_Account;
        private YesNoFlag                           patientHasMedicaid;
        private YesNoFlag                           patientInsuranceUnavailable;
        private YesNoFlag                           blankYesNoFlag;
        private YesNoFlag                           yesYesNoFlag;
        private YesNoFlag                           noYesNoFlag;
        #endregion

        #region Constants
        #endregion
    }
}
