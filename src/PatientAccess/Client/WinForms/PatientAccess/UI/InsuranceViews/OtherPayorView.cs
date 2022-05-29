using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.InsuranceViews.MedicalGroupIPAViews;
using PatientAccess.UI.InsuranceViews.Presenters;
using PatientAccess.UI.InsuranceViews.Views;

namespace PatientAccess.UI.InsuranceViews
{
    /// <summary>
    /// Summary description for OtherPayorView.
    /// </summary>
    [Serializable]
    public class OtherPayorView : ControlView, IMBIView

    {
        #region Event Handlers

        public event EventHandler ResetButtonClicked;
        
        /// <summary>
        /// On disposing, remove any event handlers we have wired to rules
        /// </summary>
        private void OtherPayorView_Disposed(object sender, EventArgs e)
        {
            UnRegisterEvents();
            if (!MBINumberPresenter.ValidateMBI())
            {
                mtbMBINumber.UnMaskedText = "";
                MBINumberPresenter.UpdateModel();
            }
        }

        /// <summary>
        /// Event handlers for Required/Preferred fields
        /// </summary>
        private void InsurancePlanSSNPreferredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetPreferredBgColor( this.maskEditPolicyNumber );
        }

        private void InsurancePlanSSNRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor( maskEditPolicyNumber );
        }
         
        /// <summary>
        /// End of Event handlers for Required/Preferred fields
        /// </summary>

        private void maskEditPolicyNumber_Enter(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor( maskEditPolicyNumber );
        }
        private void MedicareMBINumberPreferredEventHandler(object sender, EventArgs e)
        {
            SetMedicareMBINumberPreferred();
        }

        private void SetMedicareMBINumberPreferred()
        {
            UIColors.SetPreferredBgColor(mtbMBINumber);
        }
        public string MBINumber
        {
            get { return mtbMBINumber.UnMaskedText.Trim(); }
            set { mtbMBINumber.UnMaskedText = value; }
        }

        void IMBIView.SetMBINumberError()
        {
            SetMBINumberError();

        }

        private void SetMBINumberError()
        {
            UIColors.SetErrorBgColor(mtbMBINumber);
        }

        public void SetMBINumberNormalColor()
        {
            UIColors.SetNormalBgColor(this.mtbMBINumber);
        }

        public void SetHICNumberNormalColor()
        {
            UIColors.SetNormalBgColor(this.maskedEditHICNumber);
        }

        public void setFocusToMBINumber()
        {
            mtbMBINumber.Focus();
        }

        private void MedicareMBINumberRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor(mtbMBINumber);
        }

        private void maskEditPolicyNumber_Validating(object sender, CancelEventArgs e)
        {
            if( Model_Coverage == null && savedOtherCoverage != null )
            {
                Model_Coverage = savedOtherCoverage;
            }
            Model_Coverage.CertSSNID = maskEditPolicyNumber.Text.Trim();

            this.CheckForRequiredFields();
        }

        private void maskEditClinicCode_Validating(object sender, CancelEventArgs e)
        {
            if (maskEditGroupCode.Text == String.Empty && maskEditClinicCode.Text == String.Empty)
            {
                UIColors.SetNormalBgColor(maskEditGroupCode);
                UIColors.SetNormalBgColor(maskEditClinicCode);
            }
        }

        private void maskEditGroupCode_Validating(object sender, CancelEventArgs e)
        {
            if (maskEditGroupCode.Text == String.Empty && maskEditClinicCode.Text == String.Empty)
            {
                UIColors.SetNormalBgColor(maskEditGroupCode);
                UIColors.SetNormalBgColor(maskEditClinicCode);
            }
        }
  
        private void MBINumber_Validating(object sender, CancelEventArgs e)
        {
            if (MBINumberPresenter.ValidateMBINumber())
            {

                if (Model_Coverage == null && savedOtherCoverage != null)
                {
                    Model_Coverage = savedOtherCoverage;
                }
            }
        }


        private void maskEditGroupNumber_Validating(object sender, CancelEventArgs e)
        {
            if( Model_Coverage == null && savedOtherCoverage != null )
            {
                Model_Coverage = savedOtherCoverage;
            }
            Model_Coverage.GroupNumber = maskEditGroupNumber.Text;
        }

        private void btnReset_Click( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor(maskEditClinicCode);
            UIColors.SetNormalBgColor(maskEditGroupCode);

            maskEditGroupNumber.UnMaskedText    = String.Empty;
            maskEditPolicyNumber.UnMaskedText   = String.Empty;            
            maskedEditHICNumber.UnMaskedText     = String.Empty;
            maskEditClinicCode.UnMaskedText     = String.Empty;
            maskEditGroupCode.UnMaskedText      = String.Empty;
            mtbMBINumber.UnMaskedText = String.Empty;

            lblName.Text                        = String.Empty;
            lblClinic.Text                      = String.Empty;

            savedOtherCoverage                  = Model_Coverage;

            if( ResetButtonClicked != null )
            {
                ResetButtonClicked( sender, e );
            }
            
            this.Account.MedicalGroupIPA = new MedicalGroupIPA();
            UpdateModel();
            this.CheckForRequiredFields();
        }

        private void SearchButtonClick( object sender, EventArgs e )
        {
            SelectMedicalGroupIPA selectMedicalGroupIPA = new SelectMedicalGroupIPA();
            try
            {
                selectMedicalGroupIPA.ShowDialog( this );

                if( selectMedicalGroupIPA.DialogResult == DialogResult.OK )
                {
                    if( Model_Coverage == null && savedOtherCoverage != null )
                    {
                        Model_Coverage = savedOtherCoverage;
                    }
                    this.Account.MedicalGroupIPA = selectMedicalGroupIPA.i_MedicalGroupIPA;
                    this.Account.MedicalGroupIPA.AddClinic( selectMedicalGroupIPA.i_Clinic );
                    lblName.Text = selectMedicalGroupIPA.i_MedicalGroupIPA.Name;
                    lblClinic.Text = selectMedicalGroupIPA.i_Clinic.Name;
                }
            }
            finally
            {
                selectMedicalGroupIPA.Dispose();
            }
        }

        private void EditGroupCodeEnter( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( maskEditGroupCode );
        }

        private void EditGroupCodeTextChanged( object sender, EventArgs e )
        {
            if( maskEditGroupCode.TextLength == MAX_GROUP_CODE_INPUT_LEN )
            {   // Advance the control focus to the policy text box
                maskEditClinicCode.Focus();
            }
        }

        private void EditClinicCodeEnter( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( maskEditGroupCode );
        }

        private void VerifyButtonClick( object sender, EventArgs e )
        {
            if( maskEditClinicCode.Text.Length == 0 && maskEditGroupCode.Text.Length == 0 )
            {
                MessageBox.Show( UIErrorMessages.MED_GRP_IPA_ERRMSG, "Error",
                                 MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                 MessageBoxDefaultButton.Button1 );

                maskEditGroupCode.BackColor  = Color.FromArgb( 255, 36, 36 );
                maskEditClinicCode.BackColor = Color.FromArgb( 255, 36, 36 );
            }
            else if( maskEditGroupCode.Text.Length == 0 )
            {
                MessageBox.Show( UIErrorMessages.MED_GRP_IPA_ERRMSG,  "Error", MessageBoxButtons.OK,
                                 MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1 );

                maskEditGroupCode.BackColor = Color.FromArgb( 255, 36, 36 );  
            }
            else if( maskEditClinicCode.Text.Length == 0 )
            {
                MessageBox.Show( UIErrorMessages.MED_GRP_IPA_ERRMSG, "Error", MessageBoxButtons.OK,
                                 MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1 );

                maskEditClinicCode.BackColor = Color.FromArgb( 255, 36, 36 );  
            }
            else
            {
                IInsuranceBroker broker = 
                    BrokerFactory.BrokerOfType<IInsuranceBroker>();
                MedicalGroupIPA medicalGroupIPA = (MedicalGroupIPA)broker.IPAWith( 
                    User.GetCurrent().Facility.Oid, maskEditGroupCode.Text, maskEditClinicCode.Text );

                if( medicalGroupIPA == null )
                {
                    MessageBox.Show( UIErrorMessages.MED_GROUP_MATCH_ERROR, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1 );
                    UIColors.SetErrorBgColor(maskEditGroupCode);
                    UIColors.SetErrorBgColor(maskEditClinicCode);
                    maskEditGroupCode.Focus();
                }
                else
                {
                    PopulateMedicalGroupIPA( medicalGroupIPA );
                }
            }
        }
        #endregion

        #region Methods
        public override void UpdateView()
        {
            if (loadingModelData)
            {
                // Initial entry to the form -- initialize controls and get the data from the model.
                MBINumberPresenter = new MBIPresenter(this, new MessageBoxAdapter(), Model_Coverage,
                    RuleEngine.GetInstance());

                loadingModelData = false;
                
                if (Model_Coverage == null)
                {
                    return;
                }
                if (Model_Coverage.CertSSNID != null)
                {
                    maskEditPolicyNumber.Text = Model_Coverage.CertSSNID;
                }
                if (Model_Coverage.GroupNumber != null)
                {
                    maskEditGroupNumber.Text = Model_Coverage.GroupNumber;
                }
                if (this.Account.MedicalGroupIPA != null)
                {
                    this.PopulateMedicalGroupIPA(this.Account.MedicalGroupIPA);
                }
           
                RuleEngine.LoadRules(Account);
                RegisterRules();
            }

            SetMedicareHICNumberStateForFinancialClass();
            MBINumberPresenter.SetMBINumberStateForFinancialClass();
            CheckForRequiredFields();
        }

        private void RegisterRules()
        {
            RuleEngine.RegisterEvent(typeof(InsurancePlanSSNPreferred), this.Model, this.maskEditPolicyNumber,
                new EventHandler(InsurancePlanSSNPreferredEventHandler));
            RuleEngine.RegisterEvent(typeof(InsurancePlanSSNRequired), this.Model, this.maskEditPolicyNumber,
                new EventHandler(InsurancePlanSSNRequiredEventHandler));
         
            RuleEngine.RegisterEvent(typeof(MBINumberRequired), this.Model,
                new EventHandler(MedicareMBINumberRequiredEventHandler));
            RuleEngine.RegisterEvent(typeof(MBINumberPreferred), this.Model,
                new EventHandler(MedicareMBINumberPreferredEventHandler));
        }

        public void EnbleMBINumber()
        {
            this.lblMBINumber.Enabled = true;
            this.mtbMBINumber.Enabled = true;
        }
        public void DisableMBINumber()
        {
            this.lblMBINumber.Enabled = false;
            this.mtbMBINumber.Enabled = false;
            this.mtbMBINumber.UnMaskedText = string.Empty;
        }

        public override void UpdateModel()
        {
            Model_Coverage.CertSSNID = maskEditPolicyNumber.UnMaskedText.Trim();
            Model_Coverage.GroupNumber = maskEditGroupNumber.UnMaskedText.Trim();
        }

        #endregion
       
        #region Properties
        [Browsable(false)]
        public OtherCoverage Model_Coverage
        {
            set
            {
                this.Model = value;
            }
            private get
            {
                return (OtherCoverage)this.Model;
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

        public void ResetMBINumber()
        {
            MBINumber = string.Empty;
        }
 
        [Browsable(false)]
        private RuleEngine RuleEngine
        {
            get
            {
                if( i_RuleEngine == null )
                {
                    i_RuleEngine = RuleEngine.GetInstance();
                }
                return i_RuleEngine;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// CheckForRequiredFields - determine if the user has entered all required fields
        /// Some are conditional based on other fields.  Returns true or false;
        /// </summary>
        /// <returns></returns>
        public void CheckForRequiredFields()
        {
            UIColors.SetNormalBgColor( maskEditPolicyNumber );
            UIColors.SetNormalBgColor( maskedEditHICNumber );
            UIColors.SetNormalBgColor(mtbMBINumber);
            RegisterRules();
            RuleEngine.EvaluateRule( typeof( InsurancePlanSSNPreferred ), this.Model );
            RuleEngine.EvaluateRule( typeof( InsurancePlanSSNRequired ), this.Model ); 
            RuleEngine.EvaluateRule(typeof(MBINumberPreferred), this.Model);
            RuleEngine.EvaluateRule(typeof(MBINumberRequired), this.Model);


        }
        
        private void SetMedicareHICNumberStateForFinancialClass()
        {
            Account anAccount = this.Account;
            if ( anAccount != null
                && anAccount.FinancialClass != null
                && anAccount.FinancialClass.IsSignedOverMedicare() )
            {
                this.lblHICNumber.Enabled = true;
                this.maskedEditHICNumber.Enabled = true;
            }
            else
            {
                this.lblHICNumber.Enabled = false;
                this.maskedEditHICNumber.Enabled = false;
                this.maskedEditHICNumber.UnMaskedText = string.Empty;
            }
        }
       
        private void PopulateClinic( MedicalGroupIPA medicalGroupIPA )
        {
            foreach( Clinic clinic in medicalGroupIPA.Clinics )
            {
                lblClinic.Text = clinic.Name;
                break;
            }
        }

        private void PopulateMedicalGroupIPA( MedicalGroupIPA medicalGroupIPA )
        {
            if( Model_Coverage == null && savedOtherCoverage != null )
            {
                Model_Coverage = savedOtherCoverage;
            }
            this.Account.MedicalGroupIPA = medicalGroupIPA;
            lblName.Text = medicalGroupIPA.Name;
            maskEditGroupCode.Text = medicalGroupIPA.Code != string.Empty ? medicalGroupIPA.Code : String.Empty;
            PopulateClinic( medicalGroupIPA );
        }

        private void UnRegisterEvents()
        {
            RuleEngine.UnregisterEvent( typeof( InsurancePlanSSNPreferred ), this.Model, new EventHandler( InsurancePlanSSNPreferredEventHandler ) );
            RuleEngine.UnregisterEvent( typeof( InsurancePlanSSNRequired ), this.Model, new EventHandler( InsurancePlanSSNRequiredEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent(typeof(MBINumberRequired), this.Model, new EventHandler(MedicareMBINumberRequiredEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof(MBINumberPreferred), this.Model, new EventHandler(MedicareMBINumberPreferredEventHandler));
        }
       
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.planLineLabel = new PatientAccess.UI.CommonControls.LineLabel();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.maskedEditHICNumber = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblHICNumber = new System.Windows.Forms.Label();
            this.grpMedicalGroup = new System.Windows.Forms.GroupBox();
            this.btnSearch = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblSearchByName = new System.Windows.Forms.Label();
            this.maskEditGroupCode = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblCodes = new System.Windows.Forms.Label();
            this.lblHyphenCharacter = new System.Windows.Forms.Label();
            this.maskEditClinicCode = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.btnVerify = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblClinic = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblStaticClinic = new System.Windows.Forms.Label();
            this.lblStaticName = new System.Windows.Forms.Label();
            this.maskEditGroupNumber = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.maskEditPolicyNumber = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblPolicyNumber = new System.Windows.Forms.Label();
            this.lblGroupNumber = new System.Windows.Forms.Label();
            this.mtbMBINumber = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblMBINumber = new System.Windows.Forms.Label();
            this.topRightPanel = new System.Windows.Forms.Panel();
            this.btnReset = new PatientAccess.UI.CommonControls.LoggingButton();
            this.controlPanel.SuspendLayout();
            this.grpMedicalGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // planLineLabel
            // 
            this.planLineLabel.Caption = "Plan Information";
            this.planLineLabel.Location = new System.Drawing.Point(8, 0);
            this.planLineLabel.Name = "planLineLabel";
            this.planLineLabel.Size = new System.Drawing.Size(864, 18);
            this.planLineLabel.TabIndex = 14;
            this.planLineLabel.TabStop = false;
            // 
            // controlPanel
            // 
            this.controlPanel.Controls.Add(this.maskedEditHICNumber);
            this.controlPanel.Controls.Add(this.lblHICNumber);
            this.controlPanel.Controls.Add(this.grpMedicalGroup);
            this.controlPanel.Controls.Add(this.maskEditGroupNumber);
            this.controlPanel.Controls.Add(this.maskEditPolicyNumber);
            this.controlPanel.Controls.Add(this.lblPolicyNumber);
            this.controlPanel.Controls.Add(this.lblGroupNumber);
            this.controlPanel.Controls.Add(this.mtbMBINumber);
            this.controlPanel.Controls.Add(this.lblMBINumber);
            this.controlPanel.Location = new System.Drawing.Point(12, 32);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(516, 168);
            this.controlPanel.TabIndex = 15;
            // 
            // maskedEditHICNumber
            // 
            this.maskedEditHICNumber.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.maskedEditHICNumber.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.maskedEditHICNumber.Location = new System.Drawing.Point(381, 32);
            this.maskedEditHICNumber.Mask = "";
            this.maskedEditHICNumber.MaxLength = 12;
            this.maskedEditHICNumber.Name = "maskedEditHICNumber";
            this.maskedEditHICNumber.Size = new System.Drawing.Size(132, 38);
            this.maskedEditHICNumber.TabIndex = 2;
            this.maskedEditHICNumber.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
       
            // 
            // lblHICNumber
            // 
            this.lblHICNumber.Location = new System.Drawing.Point(311, 35);
            this.lblHICNumber.Name = "lblHICNumber";
            this.lblHICNumber.Size = new System.Drawing.Size(72, 23);
            this.lblHICNumber.TabIndex = 8;
            this.lblHICNumber.Text = "HIC Number:";
            // 
            // grpMedicalGroup
            // 
            this.grpMedicalGroup.Controls.Add(this.btnSearch);
            this.grpMedicalGroup.Controls.Add(this.lblSearchByName);
            this.grpMedicalGroup.Controls.Add(this.maskEditGroupCode);
            this.grpMedicalGroup.Controls.Add(this.lblCodes);
            this.grpMedicalGroup.Controls.Add(this.lblHyphenCharacter);
            this.grpMedicalGroup.Controls.Add(this.maskEditClinicCode);
            this.grpMedicalGroup.Controls.Add(this.btnVerify);
            this.grpMedicalGroup.Controls.Add(this.lblClinic);
            this.grpMedicalGroup.Controls.Add(this.lblName);
            this.grpMedicalGroup.Controls.Add(this.lblStaticClinic);
            this.grpMedicalGroup.Controls.Add(this.lblStaticName);
            this.grpMedicalGroup.Location = new System.Drawing.Point(0, 72);
            this.grpMedicalGroup.Name = "grpMedicalGroup";
            this.grpMedicalGroup.Size = new System.Drawing.Size(504, 95);
            this.grpMedicalGroup.TabIndex = 4;
            this.grpMedicalGroup.TabStop = false;
            this.grpMedicalGroup.Text = "IPA/Primary Medical Group";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(347, 58);
            this.btnSearch.Message = null;
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 7;
            this.btnSearch.Text = "Sear&ch...";
            this.btnSearch.Click += new System.EventHandler(this.SearchButtonClick);
            // 
            // lblSearchByName
            // 
            this.lblSearchByName.Location = new System.Drawing.Point(257, 60);
            this.lblSearchByName.Name = "lblSearchByName";
            this.lblSearchByName.Size = new System.Drawing.Size(100, 23);
            this.lblSearchByName.TabIndex = 0;
            this.lblSearchByName.Text = "Search by name";
            this.lblSearchByName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // maskEditGroupCode
            // 
            this.maskEditGroupCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.maskEditGroupCode.KeyPressExpression = "^[a-zA-Z0-9]*";
            this.maskEditGroupCode.Location = new System.Drawing.Point(298, 28);
            this.maskEditGroupCode.Mask = "";
            this.maskEditGroupCode.MaxLength = 10;
            this.maskEditGroupCode.Name = "maskEditGroupCode";
            this.maskEditGroupCode.Size = new System.Drawing.Size(65, 38);
            this.maskEditGroupCode.TabIndex = 5;
            this.maskEditGroupCode.ValidationExpression = "^[a-zA-Z0-9]*";
            this.maskEditGroupCode.TextChanged += new System.EventHandler(this.EditGroupCodeTextChanged);
            this.maskEditGroupCode.Enter += new System.EventHandler(this.EditGroupCodeEnter);
            this.maskEditGroupCode.Validating += new System.ComponentModel.CancelEventHandler(this.maskEditGroupCode_Validating);
            // 
            // lblCodes
            // 
            this.lblCodes.Location = new System.Drawing.Point(257, 31);
            this.lblCodes.Name = "lblCodes";
            this.lblCodes.Size = new System.Drawing.Size(41, 23);
            this.lblCodes.TabIndex = 0;
            this.lblCodes.Text = "Codes:";
            this.lblCodes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblHyphenCharacter
            // 
            this.lblHyphenCharacter.Location = new System.Drawing.Point(370, 30);
            this.lblHyphenCharacter.Name = "lblHyphenCharacter";
            this.lblHyphenCharacter.Size = new System.Drawing.Size(8, 23);
            this.lblHyphenCharacter.TabIndex = 0;
            this.lblHyphenCharacter.Text = "-";
            this.lblHyphenCharacter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // maskEditClinicCode
            // 
            this.maskEditClinicCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.maskEditClinicCode.KeyPressExpression = "^[a-zA-Z0-9]*";
            this.maskEditClinicCode.Location = new System.Drawing.Point(381, 28);
            this.maskEditClinicCode.Mask = "";
            this.maskEditClinicCode.MaxLength = 2;
            this.maskEditClinicCode.Name = "maskEditClinicCode";
            this.maskEditClinicCode.Size = new System.Drawing.Size(30, 38);
            this.maskEditClinicCode.TabIndex = 6;
            this.maskEditClinicCode.ValidationExpression = "^[a-zA-Z0-9]*";
            this.maskEditClinicCode.Enter += new System.EventHandler(this.EditClinicCodeEnter);
            this.maskEditClinicCode.Validating += new System.ComponentModel.CancelEventHandler(this.maskEditClinicCode_Validating);
            // 
            // btnVerify
            // 
            this.btnVerify.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnVerify.Location = new System.Drawing.Point(416, 30);
            this.btnVerify.Message = null;
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.Size = new System.Drawing.Size(75, 23);
            this.btnVerify.TabIndex = 6;
            this.btnVerify.Text = "&Verify";
            this.btnVerify.Click += new System.EventHandler(this.VerifyButtonClick);
            // 
            // lblClinic
            // 
            this.lblClinic.Location = new System.Drawing.Point(48, 60);
            this.lblClinic.Name = "lblClinic";
            this.lblClinic.Size = new System.Drawing.Size(195, 23);
            this.lblClinic.TabIndex = 0;
            this.lblClinic.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblName
            // 
            this.lblName.Location = new System.Drawing.Point(48, 26);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(195, 23);
            this.lblName.TabIndex = 0;
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStaticClinic
            // 
            this.lblStaticClinic.Location = new System.Drawing.Point(8, 60);
            this.lblStaticClinic.Name = "lblStaticClinic";
            this.lblStaticClinic.Size = new System.Drawing.Size(48, 23);
            this.lblStaticClinic.TabIndex = 0;
            this.lblStaticClinic.Text = "Clinic:";
            this.lblStaticClinic.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStaticName
            // 
            this.lblStaticName.Location = new System.Drawing.Point(10, 26);
            this.lblStaticName.Name = "lblStaticName";
            this.lblStaticName.Size = new System.Drawing.Size(48, 23);
            this.lblStaticName.TabIndex = 0;
            this.lblStaticName.Text = "Name:";
            this.lblStaticName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // maskEditGroupNumber
            // 
            this.maskEditGroupNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.maskEditGroupNumber.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.maskEditGroupNumber.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.maskEditGroupNumber.Location = new System.Drawing.Point(108, 32);
            this.maskEditGroupNumber.Mask = "";
            this.maskEditGroupNumber.MaxLength = 17;
            this.maskEditGroupNumber.Name = "maskEditGroupNumber";
            this.maskEditGroupNumber.Size = new System.Drawing.Size(132, 38);
            this.maskEditGroupNumber.TabIndex = 3;
            this.maskEditGroupNumber.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.maskEditGroupNumber.Validating += new System.ComponentModel.CancelEventHandler(this.maskEditGroupNumber_Validating);
            // 
            // maskEditPolicyNumber
            // 
            this.maskEditPolicyNumber.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.maskEditPolicyNumber.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.maskEditPolicyNumber.Location = new System.Drawing.Point(108, 0);
            this.maskEditPolicyNumber.Mask = "";
            this.maskEditPolicyNumber.MaxLength = 20;
            this.maskEditPolicyNumber.Name = "maskEditPolicyNumber";
            this.maskEditPolicyNumber.Size = new System.Drawing.Size(132, 38);
            this.maskEditPolicyNumber.TabIndex = 1;
            this.maskEditPolicyNumber.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.maskEditPolicyNumber.Enter += new System.EventHandler(this.maskEditPolicyNumber_Enter);
            this.maskEditPolicyNumber.Validating += new System.ComponentModel.CancelEventHandler(this.maskEditPolicyNumber_Validating);
            // 
            // lblPolicyNumber
            // 
            this.lblPolicyNumber.Location = new System.Drawing.Point(0, 3);
            this.lblPolicyNumber.Name = "lblPolicyNumber";
            this.lblPolicyNumber.Size = new System.Drawing.Size(115, 23);
            this.lblPolicyNumber.TabIndex = 0;
            this.lblPolicyNumber.Text = "Cert/SSN/ID number:";
            // 
            // lblGroupNumber
            // 
            this.lblGroupNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblGroupNumber.Location = new System.Drawing.Point(0, 35);
            this.lblGroupNumber.Name = "lblGroupNumber";
            this.lblGroupNumber.Size = new System.Drawing.Size(112, 23);
            this.lblGroupNumber.TabIndex = 0;
            this.lblGroupNumber.Text = "Group number:";
            // 
            // mtbMBINumber
            // 
            this.mtbMBINumber.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.mtbMBINumber.KeyPressExpression = "^[a-zA-Z0-9]*$";
            this.mtbMBINumber.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbMBINumber.Location = new System.Drawing.Point(381, 0);
            this.mtbMBINumber.Mask = "    -   -    ";
            this.mtbMBINumber.MaxLength = 13;
            this.mtbMBINumber.Name = "mtbMBINumber";
            this.mtbMBINumber.Size = new System.Drawing.Size(130, 38);
            this.mtbMBINumber.ValidationExpression = "^[a-zA-Z0-9]*$";
            this.mtbMBINumber.TabIndex = 16;
            this.mtbMBINumber.Validating += new System.ComponentModel.CancelEventHandler(this.MBINumber_Validating);
            // 
            // lblMBINumber
            // 
            this.lblMBINumber.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblMBINumber.Location = new System.Drawing.Point(311, 3);
            this.lblMBINumber.Name = "lblMBINumber";
            this.lblMBINumber.Size = new System.Drawing.Size(107, 29);
            this.lblMBINumber.TabIndex = 17;
            this.lblMBINumber.Text = "MBI:";
            // 
            // topRightPanel
            // 
            this.topRightPanel.Location = new System.Drawing.Point(536, 32);
            this.topRightPanel.Name = "topRightPanel";
            this.topRightPanel.Size = new System.Drawing.Size(328, 122);
            this.topRightPanel.TabIndex = 16;
            // 
            // btnReset
            // 
            this.btnReset.CausesValidation = false;
            this.btnReset.Location = new System.Drawing.Point(768, 180);
            this.btnReset.Message = null;
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 17;
            this.btnReset.Text = "Clear All";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // OtherPayorView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.topRightPanel);
            this.Controls.Add(this.controlPanel);
            this.Controls.Add(this.planLineLabel);
            this.Name = "OtherPayorView";
            this.Size = new System.Drawing.Size(880, 215);
            this.Disposed += new System.EventHandler(this.OtherPayorView_Disposed);
            this.controlPanel.ResumeLayout(false);
            this.controlPanel.PerformLayout();
            this.grpMedicalGroup.ResumeLayout(false);
            this.grpMedicalGroup.PerformLayout();
            this.ResumeLayout(false);

		}
        #endregion

        #region Construction and Finalization
        public OtherPayorView( InsDetailPlanDetails parent )
        {
            parentForm = parent;
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
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
        private Container             components = null;
        private GroupBox               grpMedicalGroup;
        private LineLabel   planLineLabel;

        private LoggingButton                               btnReset;
        private LoggingButton                               btnSearch;
        private LoggingButton                               btnVerify;

        private Label                  lblClinic;
        private Label                  lblCodes;
        private Label                  lblGroupNumber;
        private Label                  lblHyphenCharacter;
        private Label                  lblName;
        private Label                  lblPolicyNumber;
        private Label                  lblSearchByName;
        private Label                  lblStaticClinic;
        private Label                  lblStaticName;
        private Label                  lblHICNumber;

        private MaskedEditTextBox    maskEditGroupNumber;
        private MaskedEditTextBox    maskEditPolicyNumber;
        private MaskedEditTextBox    maskedEditHICNumber;

        private Panel                  controlPanel;
        private Panel                  topRightPanel;
        
        private MaskedEditTextBox    maskEditClinicCode;
        private MaskedEditTextBox    maskEditGroupCode;

        private Account                                     i_Account;
        private OtherCoverage                               savedOtherCoverage;
        private InsDetailPlanDetails                        parentForm;
        private bool                                        loadingModelData = true;
        private MaskedEditTextBox mtbMBINumber;
        private Label lblMBINumber;
        private RuleEngine i_RuleEngine;
        private MBIPresenter MBINumberPresenter;

        #endregion

        #region Constants
        private int MAX_GROUP_CODE_INPUT_LEN = 5;
        #endregion

       

        
    }
}
