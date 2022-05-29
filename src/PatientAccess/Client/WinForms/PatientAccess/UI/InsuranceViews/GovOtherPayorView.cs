using System;
using System.ComponentModel;
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
    /// Summary description for GovernmentOtherPayorView.
    /// </summary>
    [Serializable]
    public class GovernmentOtherPayorView : ControlView, IMBIView

    {
        #region Event Handlers

        public event EventHandler ResetButtonClicked;
        
        /// <summary>
        /// On disposing, remove any event handlers we have wired to rules
        /// </summary>
        private void GovOtherPayorView_Disposed(object sender, EventArgs e)
        {
            UnRegisterEvents();
            if (!MBINumberPresenter.ValidateMBI())
            {
                mtbMBINumber.UnMaskedText = "";
                MBINumberPresenter.UpdateModel();
            }
        }
        private void MedicareMBINumberPreferredEventHandler(object sender, EventArgs e)
        {
            SetMedicareMBINumberPreferred();
        }

        private void SetMedicareMBINumberPreferred()
        {
            UIColors.SetPreferredBgColor(mtbMBINumber);
        }

        private void SetMBINumberError()
        {
            UIColors.SetErrorBgColor(mtbMBINumber);
        }

        public void SetMBINumberNormalColor()
        {
            UIColors.SetNormalBgColor(this.mtbMBINumber);
        }
 
        public void setFocusToMBINumber()
        {
            mtbMBINumber.Focus();
        }

        private void MedicareMBINumberRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor(mtbMBINumber);
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

        private void InsurancePlanSSNPreferredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetPreferredBgColor(this.maskEditPolicyNumber);
        }

        private void InsurancePlanSSNRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor(this.maskEditPolicyNumber);
        }
 
        /// <summary>
        /// End of Event handlers for Required/Preferred fields
        /// </summary>

        private void maskEditPolicyNumber_Enter(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor( maskEditPolicyNumber );
        }

		private void maskEditPolicyNumber_Validating(object sender, CancelEventArgs e)
		{
			if( Model_Coverage == null && savedGovernmentOtherCoverage != null )
			{
				Model_Coverage = savedGovernmentOtherCoverage;
			}
			Model_Coverage.CertSSNID = maskEditPolicyNumber.Text.Trim();

			this.CheckForRequiredFields();
		}
        
        private void MBINumber_Validating(object sender, CancelEventArgs e)
        {
            if (MBINumberPresenter.ValidateMBINumber())
            {

                if (Model_Coverage == null && savedGovernmentOtherCoverage != null)
                {
                    Model_Coverage = savedGovernmentOtherCoverage;
                }

            }

        }

		private void maskEditGroupNumber_Validating(object sender, CancelEventArgs e)
		{
			if( Model_Coverage == null && savedGovernmentOtherCoverage != null )
			{
				Model_Coverage = savedGovernmentOtherCoverage;
			}
			Model_Coverage.GroupNumber = maskEditGroupNumber.Text;
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

        private void btnReset_Click( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor(maskEditClinicCode);
            UIColors.SetNormalBgColor(maskEditGroupCode);
            
            SetMBINumberNormalColor();
            maskEditGroupNumber.UnMaskedText     = String.Empty;
            maskEditPolicyNumber.UnMaskedText    = String.Empty;
           
            maskEditClinicCode.UnMaskedText      = String.Empty;
            maskEditGroupCode.UnMaskedText       = String.Empty;
            mtbMBINumber.UnMaskedText = String.Empty;

            lblName.Text                         = String.Empty;
            lblClinic.Text                       = String.Empty;
            
            if( ResetButtonClicked != null )
            {
                ResetButtonClicked( sender, e );
            }
            //parentForm.ResetView();
            savedGovernmentOtherCoverage = Model_Coverage;
            
            this.Account.MedicalGroupIPA = new MedicalGroupIPA();
            UpdateModel();
            CheckForRequiredFields();
        }

        private void SearchButtonClick( object sender, EventArgs e )
        {
            SelectMedicalGroupIPA selectMedicalGroupIPA = new SelectMedicalGroupIPA();
            try
            {
                selectMedicalGroupIPA.ShowDialog( this );

                if( selectMedicalGroupIPA.DialogResult == DialogResult.OK )
                {
                    this.Account.MedicalGroupIPA = selectMedicalGroupIPA.i_MedicalGroupIPA;
                    this.Account.MedicalGroupIPA.AddClinic( selectMedicalGroupIPA.i_Clinic );

                    lblName.Text   = selectMedicalGroupIPA.i_MedicalGroupIPA.Name;
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
                MessageBox.Show( UIErrorMessages.MED_GRP_IPA_ERRMSG, "Error", MessageBoxButtons.OK,
                                 MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1 );

                UIColors.SetErrorBgColor(maskEditGroupCode);
                UIColors.SetErrorBgColor(maskEditClinicCode);
            }
            else if( maskEditGroupCode.Text.Length == 0 )
            {
                MessageBox.Show( UIErrorMessages.MED_GRP_IPA_ERRMSG, "Error", MessageBoxButtons.OK,
                                 MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1 );

                UIColors.SetErrorBgColor(maskEditGroupCode);  
            }
            else if( maskEditClinicCode.Text.Length == 0 )
            {
                MessageBox.Show( UIErrorMessages.MED_GRP_IPA_ERRMSG,  "Error", MessageBoxButtons.OK,
                                 MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1 );

                UIColors.SetErrorBgColor(maskEditClinicCode);
            }
            else
            {
                IInsuranceBroker broker = 
                    BrokerFactory.BrokerOfType<IInsuranceBroker>();

                MedicalGroupIPA medicalGroupIPA = (MedicalGroupIPA)broker.IPAWith( 
                    User.GetCurrent().Facility.Oid, maskEditGroupCode.Text, maskEditClinicCode.Text );

                if( medicalGroupIPA == null )
                {
                    MessageBox.Show( UIErrorMessages.MED_GROUP_MATCH_ERROR, "Error", MessageBoxButtons.OK,
                                     MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1 );
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

        public override void UpdateModel()
        {
            this.Model_Coverage.GroupNumber = maskEditGroupNumber.UnMaskedText.Trim();
            this.Model_Coverage.CertSSNID = maskEditPolicyNumber.UnMaskedText.Trim();
          
            MBINumberPresenter.UpdateModel();
        }

        public override void UpdateView()
        {
            if (loadingModelData)
            {
                // Initial entry to the form -- initialize controls and get the data from the model.
                loadingModelData = false;
                MBINumberPresenter = new MBIPresenter(this, new MessageBoxAdapter(), Model_Coverage,
                    RuleEngine.GetInstance());

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
                
                MBINumberPresenter.UpdateView();

                if (this.Account.MedicalGroupIPA != null)
                {
                    this.PopulateMedicalGroupIPA(this.Account.MedicalGroupIPA);
                }

                RuleEngine.LoadRules(Account);

                RuleEngine.RegisterEvent(typeof(InsurancePlanSSNPreferred), this.Model, this.maskEditPolicyNumber,
                    new EventHandler(InsurancePlanSSNPreferredEventHandler));
                RuleEngine.RegisterEvent(typeof(InsurancePlanSSNRequired), this.Model, this.maskEditPolicyNumber,
                    new EventHandler(InsurancePlanSSNRequiredEventHandler));
                RuleEngine.GetInstance()
                    .RegisterEvent(typeof(MBINumberRequired), this.Model,
                        new EventHandler(MedicareMBINumberRequiredEventHandler));
                RuleEngine.GetInstance()
                    .RegisterEvent(typeof(MBINumberPreferred), this.Model,
                        new EventHandler(MedicareMBINumberPreferredEventHandler));

            }
 
            MBINumberPresenter.SetMBINumberStateForFinancialClass();
            CheckForRequiredFields();
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
        #endregion

        #region Properties
        [Browsable(false)]
        public GovernmentOtherCoverage Model_Coverage
        {
            set
            {
                this.Model = value;
            }
            private get
            {
                return (GovernmentOtherCoverage)this.Model;
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
            UIColors.SetNormalBgColor(maskEditPolicyNumber);
           
            UIColors.SetNormalBgColor(mtbMBINumber);
            RegisterRules();
            RuleEngine.EvaluateRule(typeof (InsurancePlanSSNPreferred), this.Model);
            RuleEngine.EvaluateRule(typeof (InsurancePlanSSNRequired), this.Model); 
            RuleEngine.EvaluateRule(typeof(MBINumberPreferred), this.Model);
            RuleEngine.EvaluateRule(typeof(MBINumberRequired), this.Model);
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
            if( Model_Coverage == null && savedGovernmentOtherCoverage != null )
            {
                Model_Coverage = savedGovernmentOtherCoverage;
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
            this.mtbMBINumber = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblMBINumber = new System.Windows.Forms.Label();
            this.lblGroupNumber = new System.Windows.Forms.Label();
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
       
            this.controlPanel.Controls.Add(this.grpMedicalGroup);
            this.controlPanel.Controls.Add(this.maskEditGroupNumber);
            this.controlPanel.Controls.Add(this.maskEditPolicyNumber);
            this.controlPanel.Controls.Add(this.lblPolicyNumber);
            this.controlPanel.Controls.Add(this.mtbMBINumber);
            this.controlPanel.Controls.Add(this.lblMBINumber);
            this.controlPanel.Controls.Add(this.lblGroupNumber);
            this.controlPanel.Location = new System.Drawing.Point(12, 32);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(508, 168);
            this.controlPanel.TabIndex = 15;
          
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
            this.grpMedicalGroup.Size = new System.Drawing.Size(496, 95);
            this.grpMedicalGroup.TabIndex = 4;
            this.grpMedicalGroup.TabStop = false;
            this.grpMedicalGroup.Text = "IPA/Primary Medical Group";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(347, 55);
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
            this.maskEditGroupCode.Location = new System.Drawing.Point(292, 28);
            this.maskEditGroupCode.Mask = "";
            this.maskEditGroupCode.MaxLength = 5;
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
            this.lblCodes.Location = new System.Drawing.Point(253, 32);
            this.lblCodes.Name = "lblCodes";
            this.lblCodes.Size = new System.Drawing.Size(41, 23);
            this.lblCodes.TabIndex = 0;
            this.lblCodes.Text = "Codes:";
            this.lblCodes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblHyphenCharacter
            // 
            this.lblHyphenCharacter.Location = new System.Drawing.Point(362, 30);
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
            this.maskEditClinicCode.Location = new System.Drawing.Point(371, 28);
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
            this.btnVerify.Location = new System.Drawing.Point(408, 27);
            this.btnVerify.Message = null;
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.Size = new System.Drawing.Size(84, 23);
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
            // mtbMBINumber
            // 
            this.mtbMBINumber.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.mtbMBINumber.KeyPressExpression = "^[a-zA-Z0-9]*$";
            this.mtbMBINumber.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbMBINumber.Location = new System.Drawing.Point(374, 0);
            this.mtbMBINumber.Mask = "    -   -    ";
            this.mtbMBINumber.MaxLength = 13;
            this.mtbMBINumber.Name = "mtbMBINumber";
            this.mtbMBINumber.Size = new System.Drawing.Size(130, 38);
            this.mtbMBINumber.TabIndex = 2;
            this.mtbMBINumber.ValidationExpression = "^[a-zA-Z0-9]*$";
            this.mtbMBINumber.Validating += new System.ComponentModel.CancelEventHandler(this.MBINumber_Validating);
            // 
            // lblMBINumber
            // 
            this.lblMBINumber.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblMBINumber.Location = new System.Drawing.Point(304, 3);
            this.lblMBINumber.Name = "lblMBINumber";
            this.lblMBINumber.Size = new System.Drawing.Size(107, 29);
            this.lblMBINumber.TabIndex = 17;
            this.lblMBINumber.Text = "MBI:";
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
            // topRightPanel
            // 
            this.topRightPanel.Location = new System.Drawing.Point(544, 32);
            this.topRightPanel.Name = "topRightPanel";
            this.topRightPanel.Size = new System.Drawing.Size(328, 122);
            this.topRightPanel.TabIndex = 16;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(768, 180);
            this.btnReset.Message = null;
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 17;
            this.btnReset.Text = "Clear All";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // GovernmentOtherPayorView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.topRightPanel);
            this.Controls.Add(this.controlPanel);
            this.Controls.Add(this.planLineLabel);
            this.Name = "GovernmentOtherPayorView";
            this.Size = new System.Drawing.Size(880, 215);
            this.Disposed += new System.EventHandler(this.GovOtherPayorView_Disposed);
            this.controlPanel.ResumeLayout(false);
            this.controlPanel.PerformLayout();
            this.grpMedicalGroup.ResumeLayout(false);
            this.grpMedicalGroup.PerformLayout();
            this.ResumeLayout(false);

		}
        #endregion

        #region Construction and Finalization
        public GovernmentOtherPayorView( InsDetailPlanDetails parent )
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
 
        private MaskedEditTextBox    maskEditGroupNumber;
        private MaskedEditTextBox    maskEditPolicyNumber;
     
        private Panel                  controlPanel;
        private Panel                  topRightPanel;
        
        private MaskedEditTextBox    maskEditClinicCode;
        private MaskedEditTextBox    maskEditGroupCode;

        private Account                                     i_Account;
        private GovernmentOtherCoverage                     savedGovernmentOtherCoverage;
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
