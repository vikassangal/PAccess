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

namespace PatientAccess.UI.InsuranceViews
{
    [Serializable]
    public class MedicaidPayorView : ControlView
    {
        #region Rule Event Handlers        

        private void MedicaidPolicyCINNumberPreferredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetPreferredBgColor( this.maskEditPolicyNumber );
        }

        private void MedicaidPolicyCINNumberRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor( this.maskEditPolicyNumber );
        }

        private void MedicaidIssueDatePreferredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetPreferredBgColor( this.maskEditIssueDate );
        }

        private void MedicaidIssueDateRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor( this.maskEditIssueDate );
        }
        #endregion

        #region Event Handlers

        private void maskEditGroupCode_TextChanged( object sender, EventArgs e )
        {
            if( maskEditGroupCode.TextLength == MAX_GROUP_CODE_INPUT_LEN )
            {   // Advance the control focus to the policy text box
                maskEditClinicCode.Focus();
            }
        }

        public event EventHandler ResetButtonClicked;
        /// <summary>
        /// On disposing, remove any event handlers we have wired to rules
        /// </summary>
        private void MedicaidPayorView_Disposed(object sender, EventArgs e)
        {
            UnRegisterEvents();
        }

        /// <summary>
        /// End of Event handlers for Required/Preferred fields
        /// </summary>

        private void maskEditPolicyNumber_Validating(object sender, CancelEventArgs e)
        {
            UIColors.SetNormalBgColor(this.maskEditPolicyNumber);
            if( Model_Coverage == null && savedGovernmentMedicaidCoverage != null )
            {
                Model_Coverage = savedGovernmentMedicaidCoverage;
            }
            Model_Coverage.PolicyCINNumber = maskEditPolicyNumber.Text;

            RuleEngine.GetInstance().EvaluateRule( typeof( MedicaidPolicyCINNumberPreferred ), this.Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( MedicaidPolicyCINNumberRequired ), this.Model );
        }
        
        private void maskEditIssueDate_Validating(object sender, CancelEventArgs e)
        {
            if( this.blnLeaveRun )
                return;

            if( !VerifyDate() )
            {
                e.Cancel = true;
            }
        }

        private bool VerifyDate( )
        {
            UIColors.SetNormalBgColor( this.maskEditIssueDate );
            if( maskEditIssueDate.UnMaskedText.Length == 0
                || maskEditIssueDate.UnMaskedText == "01010001" )
            {
                this.Model_Coverage.IssueDate = DateTime.MinValue;
                RuleEngine.GetInstance().EvaluateRule( typeof( MedicaidIssueDatePreferred ), this.Model, User.GetCurrent().Facility );
                RuleEngine.GetInstance().EvaluateRule( typeof( MedicaidIssueDateRequired ), this.Model, User.GetCurrent().Facility );
                return true;
            }

            if( maskEditIssueDate.Text.Length != 10 )
            {
                UIColors.SetErrorBgColor( maskEditIssueDate );
                MessageBox.Show( UIErrorMessages.MEDICAID_SEARCH_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                maskEditIssueDate.Focus();
                return false;
            }

            string month = maskEditIssueDate.Text.Substring( 0, 2 );
            string day = maskEditIssueDate.Text.Substring( 3, 2 );
            string year = maskEditIssueDate.Text.Substring( 6, 4 );

            try
            {   // Check the date entered is not in the future
                medicaidIssueDate = new DateTime( Convert.ToInt32( year ),
                    Convert.ToInt32( month ),
                    Convert.ToInt32( day ) );

                if( medicaidIssueDate > DateTime.Today )
                {
                    UIColors.SetErrorBgColor( maskEditIssueDate );
                    MessageBox.Show( UIErrorMessages.MEDICAID_FUTURE_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    maskEditIssueDate.Focus();
                    return false;
                }
                else if( DateValidator.IsValidDate( medicaidIssueDate ) == false )
                {
                    UIColors.SetErrorBgColor( maskEditIssueDate );
                    MessageBox.Show( UIErrorMessages.MEDICAID_INVALID_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    maskEditIssueDate.Focus();
                    return false;
                }
                else
                {
                    Model_Coverage.IssueDate = medicaidIssueDate;
                    return true;
                }
            }
            catch
            {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                // an invalid year, month, or day.  Simply set field to error color.
                UIColors.SetErrorBgColor( maskEditIssueDate );
                MessageBox.Show( UIErrorMessages.MEDICAID_INVALID_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                maskEditIssueDate.Focus();
                return false;
            }
        }

        private void maskEditGroupCode_Validating(object sender, CancelEventArgs e)
        {
            if (maskEditGroupCode.Text == String.Empty && maskEditClinicCode.Text == String.Empty)
            {
                UIColors.SetNormalBgColor(maskEditGroupCode);
                UIColors.SetNormalBgColor(maskEditClinicCode);
                return;
            }
            UIColors.SetNormalBgColor(this.maskEditGroupCode);
            if( Model_Coverage == null && savedGovernmentMedicaidCoverage != null )
            {
                Model_Coverage = savedGovernmentMedicaidCoverage;
            }
        }

        private void maskEditClinicCode_Validating(object sender, CancelEventArgs e)
        {
            if (maskEditGroupCode.Text == String.Empty && maskEditClinicCode.Text == String.Empty)
            {
                UIColors.SetNormalBgColor(maskEditGroupCode);
                UIColors.SetNormalBgColor(maskEditClinicCode);
            }
        }
        
        private void btnReset_Click( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( maskEditIssueDate );
            UIColors.SetNormalBgColor( maskEditClinicCode );
            UIColors.SetNormalBgColor( maskEditGroupCode );

            maskEditIssueDate.UnMaskedText       = String.Empty;
            maskEditClinicCode.UnMaskedText      = String.Empty;
            maskEditGroupCode.UnMaskedText       = String.Empty;
            maskEditPolicyNumber.UnMaskedText    = String.Empty;

            lblName.Text                         = String.Empty;
            lblClinic.Text                       = String.Empty;
           
            MedicalGroupIPA mgipa = new MedicalGroupIPA();
            this.Account.MedicalGroupIPA = mgipa;

            this.UpdateModel();

            if( ResetButtonClicked != null )
            {
                ResetButtonClicked( sender, e );
            }

            //parentForm.ResetView();
            savedGovernmentMedicaidCoverage = Model_Coverage;
            UpdateView();
        }

        private void VerifyButtonClick( object sender, EventArgs e )
        {
            if( maskEditClinicCode.Text.Length == 0 && maskEditGroupCode.Text.Length == 0 )
            {
                MessageBox.Show( UIErrorMessages.MED_GRP_IPA_ERRMSG, "Error", MessageBoxButtons.OK,
                                 MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1  );

                maskEditGroupCode.BackColor  = Color.FromArgb( 255, 36, 36 );  
                maskEditClinicCode.BackColor = Color.FromArgb( 255, 36, 36 );
                maskEditGroupCode.Focus();
            }
            else if( maskEditGroupCode.Text.Length == 0 )
            {
                MessageBox.Show( UIErrorMessages.MED_GRP_IPA_ERRMSG,  "Error", MessageBoxButtons.OK,
                                 MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1  );

                maskEditGroupCode.BackColor = Color.FromArgb( 255, 36, 36 );  
            }
            else if( maskEditClinicCode.Text.Length == 0 )
            {
                MessageBox.Show( UIErrorMessages.MED_GRP_IPA_ERRMSG, "Error", MessageBoxButtons.OK,
                                 MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1  );

                maskEditClinicCode.BackColor = Color.FromArgb( 255, 36, 36 );  
            }
            else
            {
                IInsuranceBroker broker =
                    BrokerFactory.BrokerOfType<IInsuranceBroker>();

                MedicalGroupIPA medicalGroupIPA = 
                            (MedicalGroupIPA)broker.IPAWith( User.GetCurrent().Facility.Oid, 
                                                             maskEditGroupCode.Text,
                                                             maskEditClinicCode.Text );

                if( medicalGroupIPA == null )
                {
                    MessageBox.Show( UIErrorMessages.MED_GRP_IPA_SEARCH_ERRMSG, "Error",
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button1 );
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

        private void SearchButtonClick( object sender, EventArgs e )
        {
            SelectMedicalGroupIPA selectMedicalGroupIPA = new SelectMedicalGroupIPA();
            try
            {
                selectMedicalGroupIPA.ShowDialog( this );

                if( selectMedicalGroupIPA.DialogResult == DialogResult.OK )
                {
                    CheckCoverageType( selectMedicalGroupIPA );
                }
            }
            finally
            {
                selectMedicalGroupIPA.Dispose();
            }
        }


        #endregion

        #region Methods

        public override void UpdateModel()
        {
            Model_Coverage.PolicyCINNumber = maskEditPolicyNumber.UnMaskedText;

            this.VerifyDate();
        }

        public override void UpdateView()
        {
            if( loadingModelData )
            {   // Initial entry to the form -- initialize controls and get the data from the model.
                loadingModelData = false;

                if( Model_Coverage == null )
                {
                    return;
                }
                if( Model_Coverage.PolicyCINNumber != null )
                {
                    maskEditPolicyNumber.Text = Model_Coverage.PolicyCINNumber;
                }
                if( Model_Coverage.IssueDate.ToString() != String.Empty )
                {   // Format is 12/25/2004
                    maskEditIssueDate.Text = String.Format("{0:D2}/{1:D2}/{2:D4}",
                        Model_Coverage.IssueDate.Month,
                        Model_Coverage.IssueDate.Day,
                        Model_Coverage.IssueDate.Year );
                }

                if( Model_Coverage.IssueDate == DateTime.MinValue )
                {
                    this.maskEditIssueDate.Text = string.Empty;
                }

                if( this.Account.MedicalGroupIPA != null )
                {
                    this.PopulateMedicalGroupIPA(this.Account.MedicalGroupIPA);
                }

                RuleEngine.LoadRules( Account );

                RuleEngine.GetInstance().RegisterEvent( typeof( MedicaidPolicyCINNumberPreferred ), this.Model, new EventHandler( MedicaidPolicyCINNumberPreferredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( MedicaidPolicyCINNumberRequired ), this.Model, new EventHandler( MedicaidPolicyCINNumberRequiredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( MedicaidIssueDateRequired ), this.Model, new EventHandler( MedicaidIssueDateRequiredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( MedicaidIssueDatePreferred ), this.Model, new EventHandler( MedicaidIssueDatePreferredEventHandler ) );

            }

            runRules();
        }
        #endregion

        #region Properties
        [Browsable(false)]
        public GovernmentMedicaidCoverage Model_Coverage
        {
            set
            {
                this.Model = value;
            }
            private get
            {
                return (GovernmentMedicaidCoverage)this.Model;
            }
        }

        [Browsable(false)]
        public Account Account
        {
            private get
            {
                return i_Account;
            }
            set
            {
                i_Account = value;
            }
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
        /// runRules - determine if the user has entered all required fields
        /// Some are conditional based on other fields.  Returns true or false;
        /// </summary>
        /// <returns></returns>
        private void runRules()
        {
            UIColors.SetNormalBgColor( maskEditPolicyNumber );

            RuleEngine.EvaluateRule( typeof( MedicaidPolicyCINNumberPreferred ), this.Model_Coverage );
            RuleEngine.EvaluateRule( typeof( MedicaidPolicyCINNumberRequired ), this.Model_Coverage );
            RuleEngine.EvaluateRule( typeof( MedicaidIssueDatePreferred ), this.Model_Coverage, User.GetCurrent().Facility );
            RuleEngine.EvaluateRule( typeof( MedicaidIssueDateRequired ), this.Model_Coverage, User.GetCurrent().Facility );
        }

        private void CheckCoverageType( SelectMedicalGroupIPA selectMedicalGroupIPA )
        {
            if( Model_Coverage == null && savedGovernmentMedicaidCoverage != null )
            {
                Model_Coverage = savedGovernmentMedicaidCoverage;
            }
            this.Account.MedicalGroupIPA = selectMedicalGroupIPA.i_MedicalGroupIPA;
            this.Account.MedicalGroupIPA.AddClinic( selectMedicalGroupIPA.i_Clinic );
            lblName.Text = selectMedicalGroupIPA.i_MedicalGroupIPA.Name;
            lblClinic.Text = selectMedicalGroupIPA.i_Clinic.Name;
        }

        private void PopulateMedicalGroupIPA( MedicalGroupIPA medicalGroupIPA )
        {
            if( Model_Coverage == null && savedGovernmentMedicaidCoverage != null )
            {
                Model_Coverage = savedGovernmentMedicaidCoverage;
            }
            this.Account.MedicalGroupIPA = medicalGroupIPA;
            lblName.Text = medicalGroupIPA.Name;
            maskEditGroupCode.Text = medicalGroupIPA.Code != string.Empty ? medicalGroupIPA.Code : String.Empty;
            PopulateClinic( medicalGroupIPA );
        }

        private void PopulateClinic( MedicalGroupIPA medicalGroupIPA )
        {
            foreach( Clinic clinic in medicalGroupIPA.Clinics )
            {
                lblClinic.Text = clinic.Name;
                break;
            }
        }

        private void UnRegisterEvents()
        {
            RuleEngine.GetInstance().UnregisterEvent( typeof( MedicaidPolicyCINNumberPreferred ), this.Model, new EventHandler( MedicaidPolicyCINNumberPreferredEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( MedicaidPolicyCINNumberRequired ), this.Model, new EventHandler( MedicaidPolicyCINNumberRequiredEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( MedicaidIssueDateRequired ), this.Model, new EventHandler( MedicaidIssueDateRequiredEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( MedicaidIssueDatePreferred ), this.Model, new EventHandler( MedicaidIssueDatePreferredEventHandler ) );
        }
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MedicaidPayorView));
            this.controlPanel = new System.Windows.Forms.Panel();
            this.maskEditIssueDate = new Extensions.UI.Winforms.MaskedEditTextBox();
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
            this.lblMedicaidDate = new System.Windows.Forms.Label();
            this.maskEditPolicyNumber = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblPolicyNumber = new System.Windows.Forms.Label();
            this.authorizationPanel = new System.Windows.Forms.Panel();
            this.btnReset = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lineLabel = new PatientAccess.UI.CommonControls.LineLabel();
            this.controlPanel.SuspendLayout();
            this.grpMedicalGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // controlPanel
            // 
            this.controlPanel.Controls.Add(this.maskEditIssueDate);
            this.controlPanel.Controls.Add(this.grpMedicalGroup);
            this.controlPanel.Controls.Add(this.lblMedicaidDate);
            this.controlPanel.Controls.Add(this.maskEditPolicyNumber);
            this.controlPanel.Controls.Add(this.lblPolicyNumber);
            this.controlPanel.Location = new System.Drawing.Point(12, 32);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(516, 168);
            this.controlPanel.TabIndex = 1;
            // 
            // maskEditIssueDate
            // 
            this.maskEditIssueDate.KeyPressExpression = "^\\d*$";
            this.maskEditIssueDate.Location = new System.Drawing.Point(103, 35);
            this.maskEditIssueDate.Mask = "  /  /";
            this.maskEditIssueDate.MaxLength = 10;
            this.maskEditIssueDate.Name = "maskEditIssueDate";
            this.maskEditIssueDate.Size = new System.Drawing.Size(70, 20);
            this.maskEditIssueDate.TabIndex = 3;
            this.maskEditIssueDate.ValidationExpression = resources.GetString("maskEditIssueDate.ValidationExpression");
            this.maskEditIssueDate.Validating += new System.ComponentModel.CancelEventHandler(this.maskEditIssueDate_Validating);
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
            this.btnSearch.Location = new System.Drawing.Point(347, 55);
            this.btnSearch.Message = null;
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 8;
            this.btnSearch.Text = "Sear&ch...";
            this.btnSearch.Click += new System.EventHandler(this.SearchButtonClick);
            // 
            // lblSearchByName
            // 
            this.lblSearchByName.Location = new System.Drawing.Point(257, 60);
            this.lblSearchByName.Name = "lblSearchByName";
            this.lblSearchByName.Size = new System.Drawing.Size(100, 23);
            this.lblSearchByName.TabIndex = 15;
            this.lblSearchByName.Text = "Search by name";
            this.lblSearchByName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // maskEditGroupCode
            // 
            this.maskEditGroupCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.maskEditGroupCode.KeyPressExpression = "^[a-zA-Z0-9]*";
            this.maskEditGroupCode.Location = new System.Drawing.Point(298, 28);
            this.maskEditGroupCode.Mask = "";
            this.maskEditGroupCode.MaxLength = 5;
            this.maskEditGroupCode.Name = "maskEditGroupCode";
            this.maskEditGroupCode.Size = new System.Drawing.Size(65, 20);
            this.maskEditGroupCode.TabIndex = 5;
            this.maskEditGroupCode.ValidationExpression = "^[a-zA-Z0-9]*";
            this.maskEditGroupCode.TextChanged += new System.EventHandler(this.maskEditGroupCode_TextChanged);
            this.maskEditGroupCode.Validating += new System.ComponentModel.CancelEventHandler(this.maskEditGroupCode_Validating);
            // 
            // lblCodes
            // 
            this.lblCodes.Location = new System.Drawing.Point(258, 32);
            this.lblCodes.Name = "lblCodes";
            this.lblCodes.Size = new System.Drawing.Size(41, 23);
            this.lblCodes.TabIndex = 10;
            this.lblCodes.Text = "Codes:";
            this.lblCodes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblHyphenCharacter
            // 
            this.lblHyphenCharacter.Location = new System.Drawing.Point(368, 30);
            this.lblHyphenCharacter.Name = "lblHyphenCharacter";
            this.lblHyphenCharacter.Size = new System.Drawing.Size(8, 23);
            this.lblHyphenCharacter.TabIndex = 12;
            this.lblHyphenCharacter.Text = "-";
            this.lblHyphenCharacter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // maskEditClinicCode
            // 
            this.maskEditClinicCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.maskEditClinicCode.KeyPressExpression = "^[a-zA-Z0-9]*";
            this.maskEditClinicCode.Location = new System.Drawing.Point(376, 28);
            this.maskEditClinicCode.Mask = "";
            this.maskEditClinicCode.MaxLength = 2;
            this.maskEditClinicCode.Name = "maskEditClinicCode";
            this.maskEditClinicCode.Size = new System.Drawing.Size(30, 20);
            this.maskEditClinicCode.TabIndex = 6;
            this.maskEditClinicCode.ValidationExpression = "^[a-zA-Z0-9]*";
            this.maskEditClinicCode.Validating += new System.ComponentModel.CancelEventHandler(this.maskEditClinicCode_Validating);
            // 
            // btnVerify
            // 
            this.btnVerify.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnVerify.Location = new System.Drawing.Point(416, 27);
            this.btnVerify.Message = null;
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.Size = new System.Drawing.Size(75, 23);
            this.btnVerify.TabIndex = 7;
            this.btnVerify.Text = "&Verify";
            this.btnVerify.Click += new System.EventHandler(this.VerifyButtonClick);
            // 
            // lblClinic
            // 
            this.lblClinic.Location = new System.Drawing.Point(48, 60);
            this.lblClinic.Name = "lblClinic";
            this.lblClinic.Size = new System.Drawing.Size(195, 23);
            this.lblClinic.TabIndex = 9;
            // 
            // lblName
            // 
            this.lblName.Location = new System.Drawing.Point(48, 26);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(195, 23);
            this.lblName.TabIndex = 7;
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStaticClinic
            // 
            this.lblStaticClinic.Location = new System.Drawing.Point(8, 60);
            this.lblStaticClinic.Name = "lblStaticClinic";
            this.lblStaticClinic.Size = new System.Drawing.Size(48, 23);
            this.lblStaticClinic.TabIndex = 8;
            this.lblStaticClinic.Text = "Clinic:";
            this.lblStaticClinic.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStaticName
            // 
            this.lblStaticName.Location = new System.Drawing.Point(10, 26);
            this.lblStaticName.Name = "lblStaticName";
            this.lblStaticName.Size = new System.Drawing.Size(48, 23);
            this.lblStaticName.TabIndex = 6;
            this.lblStaticName.Text = "Name:";
            this.lblStaticName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblMedicaidDate
            // 
            this.lblMedicaidDate.Location = new System.Drawing.Point(0, 38);
            this.lblMedicaidDate.Name = "lblMedicaidDate";
            this.lblMedicaidDate.Size = new System.Drawing.Size(112, 13);
            this.lblMedicaidDate.TabIndex = 3;
            this.lblMedicaidDate.Text = "Medicaid issue date:";
            // 
            // maskEditPolicyNumber
            // 
            this.maskEditPolicyNumber.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.maskEditPolicyNumber.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.maskEditPolicyNumber.Location = new System.Drawing.Point(103, 8);
            this.maskEditPolicyNumber.Mask = "";
            this.maskEditPolicyNumber.MaxLength = 20;
            this.maskEditPolicyNumber.Name = "maskEditPolicyNumber";
            this.maskEditPolicyNumber.Size = new System.Drawing.Size(140, 20);
            this.maskEditPolicyNumber.TabIndex = 2;
            this.maskEditPolicyNumber.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.maskEditPolicyNumber.Validating += new System.ComponentModel.CancelEventHandler(this.maskEditPolicyNumber_Validating);
            // 
            // lblPolicyNumber
            // 
            this.lblPolicyNumber.Location = new System.Drawing.Point(0, 11);
            this.lblPolicyNumber.Name = "lblPolicyNumber";
            this.lblPolicyNumber.Size = new System.Drawing.Size(104, 13);
            this.lblPolicyNumber.TabIndex = 1;
            this.lblPolicyNumber.Text = "Policy/CIN number:";
            // 
            // authorizationPanel
            // 
            this.authorizationPanel.Location = new System.Drawing.Point(544, 32);
            this.authorizationPanel.Name = "authorizationPanel";
            this.authorizationPanel.Size = new System.Drawing.Size(328, 72);
            this.authorizationPanel.TabIndex = 2;
            // 
            // btnReset
            // 
            this.btnReset.CausesValidation = false;
            this.btnReset.Location = new System.Drawing.Point(768, 180);
            this.btnReset.Message = null;
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 12;
            this.btnReset.Text = "Clear All";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // lineLabel
            // 
            this.lineLabel.Caption = "Plan Information";
            this.lineLabel.Location = new System.Drawing.Point(12, 5);
            this.lineLabel.Name = "lineLabel";
            this.lineLabel.Size = new System.Drawing.Size(864, 16);
            this.lineLabel.TabIndex = 13;
            this.lineLabel.TabStop = false;
            // 
            // MedicaidPayorView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lineLabel);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.authorizationPanel);
            this.Controls.Add(this.controlPanel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MedicaidPayorView";
            this.Size = new System.Drawing.Size(880, 215);
            this.Disposed += new System.EventHandler(this.MedicaidPayorView_Disposed);
            this.controlPanel.ResumeLayout(false);
            this.controlPanel.PerformLayout();
            this.grpMedicalGroup.ResumeLayout(false);
            this.grpMedicalGroup.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        #region Construction and Finalization
        public MedicaidPayorView( InsDetailPlanDetails parent )
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
                if( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        private Container             components = null;
        
        private LoggingButton                 btnReset;
        private LoggingButton                 btnSearch;
        private LoggingButton                 btnVerify;

        private GroupBox grpMedicalGroup;
        private Label                  lblClinic;
        private Label lblCodes;
        private Label                  lblHyphenCharacter;
        private Label                  lblMedicaidDate;
        private Label                  lblName;
        private Label                  lblPolicyNumber;
        private Label                  lblSearchByName;
        private Label                  lblStaticClinic;
        private Label                  lblStaticName;

        private Panel                  controlPanel;
        private Panel authorizationPanel;
        private MaskedEditTextBox maskEditClinicCode;
        private MaskedEditTextBox    maskEditGroupCode;
        private MaskedEditTextBox    maskEditPolicyNumber;
        private MaskedEditTextBox    maskEditIssueDate;

        private LineLabel   lineLabel;

        private Account                                     i_Account;
        private DateTime                                    medicaidIssueDate;
        private GovernmentMedicaidCoverage                  savedGovernmentMedicaidCoverage;
        private InsDetailPlanDetails                        parentForm;
        private bool                                        loadingModelData = true;
        private RuleEngine                                  i_RuleEngine;
        public bool blnLeaveRun;
        #endregion

        #region Constants
        
        private int MAX_GROUP_CODE_INPUT_LEN = 5;

        #endregion

        
    }
}
