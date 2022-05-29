using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.PhysicianSearchViews
{
    /// <summary>
    /// Summary description for RecordNonStaffPhysicianView.
    /// </summary>
    //TODO: Create XML summary comment for RecordNonStaffPhysicianView
    [Serializable]
    public class RecordNonStaffPhysicianView : ControlView
    {
        #region Events

        public event EventHandler EnableOKButton;

        #endregion

        #region Event Handlers

        void RecordNonStaffPhysicianView_Enter( object sender, EventArgs e )
        {
            blnLeaving = false;
        }

        private void mtbLastName_Validating( object sender, CancelEventArgs e )
        {
            CheckRequiredFields();

            NonStaffPhysician = NonstaffPhysician();

            UIColors.SetNormalBgColor( mtbLastName );
            RuleEngine.GetInstance().EvaluateRule( typeof( PhysicianLastNameRequired ), NonStaffPhysician );
        }

        private void mtbFirstName_Validating( object sender, CancelEventArgs e )
        {
            CheckRequiredFields();

            NonStaffPhysician = NonstaffPhysician();
            UIColors.SetNormalBgColor( mtbFirstName );
            RuleEngine.GetInstance().EvaluateRule( typeof( PhysicianFirstNameRequired ), NonStaffPhysician );
        }

        private void mtbUPINnumber_Validating( object sender, CancelEventArgs e )
        {
            if ( ContainsFocus
                || mtbUPINnumber.BackColor != UIColors.TextFieldBackgroundError )
            {
                if ( mtbUPINnumber.Text == FAUX_UPIN )
                {
                    UIColors.SetNormalBgColor( mtbUPINnumber );

                    int index = cmbStatus.FindString( UNKNOWN_STATUS );
                    if ( index != -1 )
                    {
                        cmbStatus.SelectedIndex = index;
                    }
                    mtbUPINnumber.Enabled = false;
                }
                else
                {
                    if ( IsUPINValid() )
                    {
                        CheckRequiredFields();
                        NonStaffPhysician = NonstaffPhysician();
                        UIColors.SetNormalBgColor( mtbUPINnumber );
                        RuleEngine.GetInstance().EvaluateRule( typeof( PhysicianUPINNumberRequired ), NonStaffPhysician );
                    }
                }
            }
        }

        private void RecordNonStaffPhysicianView_Leave( object sender, EventArgs e )
        {
            blnLeaving = true;

            if ( phoneNumberControl.HasAreaCodeError() )
            {
                phoneNumberControl.FocusAreaCode();
            }
            else if ( phoneNumberControl.HasPhoneNumberError() )
            {
                phoneNumberControl.FocusPhoneNumber();
            }
        }

        private void mtbStateLicenseNbr_Validating( object sender, CancelEventArgs e )
        {
            CheckRequiredFields();

            NonStaffPhysician = NonstaffPhysician();

            UIColors.SetNormalBgColor( mtbStateLicenseNbr );
            RuleEngine.GetInstance().EvaluateRule( typeof( PhysicianStateLicenseNumberPreferred ), NonStaffPhysician );
        }

        private void cmbStatus_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( cmbStatus.SelectedIndex >= 0 )
            {
                if ( ( string )cmbStatus.SelectedItem == UNKNOWN_STATUS )
                {
                    UIColors.SetNormalBgColor( mtbUPINnumber );
                    mtbUPINnumber.Text = FAUX_UPIN;
                    mtbUPINnumber.Enabled = false;

                    MessageBox.Show( UIErrorMessages.OBTAIN_UPIN,
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1 );
                }
                else
                {
                    mtbUPINnumber.Enabled = true;
                    mtbUPINnumber.Text = String.Empty;
                }
            }

            CheckRequiredFields();

            NonStaffPhysician = NonstaffPhysician();

            UIColors.SetNormalBgColor( mtbUPINnumber );
            UIColors.SetNormalBgColor( cmbStatus );
            RuleEngine.GetInstance().EvaluateRule( typeof( PhysicianUPINStatusRequired ), NonStaffPhysician );
            RuleEngine.GetInstance().EvaluateRule( typeof( PhysicianUPINNumberRequired ), NonStaffPhysician );
        }

        private void mtbNationalProviderIdentifier_Validating( object sender, CancelEventArgs e )
        {
            npiValidated = false;
            CheckRequiredFields();
            NonStaffPhysician.NPI = mtbNationalProviderIdentifier.Text;

            if ( !blnLeaving || ( ParentForm != null && ParentForm.ActiveControl.Text == "Cancel" ) )
            {
                if ( mtbNationalProviderIdentifier.Text.Trim() != string.Empty
                && !IsNationalProviderIdentifierTheCorrectLength() )
                {
                    MessageBox.Show( UIErrorMessages.INVALID_NPI_LENGTH,
                     "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1 );
                    UIColors.SetErrorBgColor( mtbNationalProviderIdentifier );
                    mtbNationalProviderIdentifier.Focus();
                    return;
                }

                UIColors.SetNormalBgColor( mtbNationalProviderIdentifier );
                RuleEngine.GetInstance().EvaluateRule( typeof( NonStaffNPIIsValid ), NonStaffPhysician );
            }
            else
            {
                UIColors.SetNormalBgColor( mtbNationalProviderIdentifier );
            }
            RuleEngine.GetInstance().EvaluateRule<NonStaffPhysicianNPIRequired>( NonStaffPhysician, Model.Activity );
        }

        #region Rule event handlers
        private void PhysicianLastNameRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbLastName );
            Refresh();
        }

        private void PhysicianFirstNameRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbFirstName );
            Refresh();
        }

        private void PhysicianStateLicenseNumberPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( mtbStateLicenseNbr );
            Refresh();
        }

        private void PhysicianUPINStatusRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbStatus );
            Refresh();
        }

        private void PhysicianUPINNumberRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbUPINnumber );
            Refresh();
        }

        private void NonStaffNPIIsValidHandler( object sender, EventArgs e )
        {
            //The NPI check degit was not valid
            UIColors.SetErrorBgColor( mtbNationalProviderIdentifier );
            if ( !npiValidated )
            {
                MessageBox.Show( UIErrorMessages.INVALID_NPI_CHECKDIGIT,
                     "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1 );
                mtbNationalProviderIdentifier.Focus();
                npiValidated = true;
            }
        }

        private void NonStaffPhysicianNPIRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbNationalProviderIdentifier );
            Refresh();
        }

        #endregion
        #endregion

        #region Methods

        public bool IsPhoneNumberValid()
        {
            return !phoneNumberControl.HasError();
        }

        public override void UpdateView()
        {
            RegisterRulesEvents();

            if ( CallingObject == "VIEWDETAIL" )
            {
                if ( PhysicianRelationshipToView == PhysicianRelationship.REFERRING_PHYSICIAN )
                {
                    physicianRelationship = Model.PhysicianRelationshipWithRole( PhysicianRole.Referring().Role() );
                }
                if ( PhysicianRelationshipToView == PhysicianRelationship.ADMITTING_PHYSICIAN )
                {
                    physicianRelationship = Model.PhysicianRelationshipWithRole( PhysicianRole.Admitting().Role() );
                }
                if ( PhysicianRelationshipToView == PhysicianRelationship.ATTENDING_PHYSICIAN )
                {
                    physicianRelationship = Model.PhysicianRelationshipWithRole( PhysicianRole.Attending().Role() );
                }
                if ( PhysicianRelationshipToView == PhysicianRelationship.OPERATING_PHYSICIAN )
                {
                    physicianRelationship = Model.PhysicianRelationshipWithRole( PhysicianRole.Operating().Role() );
                }
                if ( PhysicianRelationshipToView == PhysicianRelationship.PRIMARYCARE_PHYSICIAN )
                {
                    physicianRelationship = Model.PhysicianRelationshipWithRole( PhysicianRole.PrimaryCare().Role() );
                }

                PopulatePhysicianInformation( physicianRelationship.Physician );
            }
            else
            {
                int index = cmbStatus.FindString( "KNOWN" );
                if ( index != -1 )
                {
                    cmbStatus.SelectedIndex = index;
                }
            }

            CheckRequiredFields();

            NonStaffPhysician = NonstaffPhysician();
            RunRules();

            mtbLastName.Focus();
        }

        public override void UpdateModel()
        {
        }

        public Physician NonstaffPhysician()
        {

            NonStaffPhysician.Name.LastName = mtbLastName.Text;
            NonStaffPhysician.LastName = mtbLastName.Text;
            NonStaffPhysician.Name.FirstName = mtbFirstName.Text;
            NonStaffPhysician.FirstName = mtbFirstName.Text;
            NonStaffPhysician.Name.MiddleInitial = mtbMiddleInitial.Text;
            NonStaffPhysician.PhysicianNumber = NONSTAFFPHYSICIAN_NBR;
            NonStaffPhysician.NPI = mtbNationalProviderIdentifier.Text;

            NonStaffPhysician.StateLicense = mtbStateLicenseNbr.Text;
            NonStaffPhysician.UPIN = mtbUPINnumber.Text;

            if ( phoneNumberControl.PhoneNumber != null )
            {
                NonStaffPhysician.PhoneNumber = phoneNumberControl.Model;
            }
            else
            {
                NonStaffPhysician.PhoneNumber = new PhoneNumber();
            }

            return NonStaffPhysician;
        }
        #endregion

        #region Properties
        public new Account Model
        {
            private get
            {
                return ( Account )base.Model;
            }
            set
            {
                base.Model = value;
            }
        }

        public string CallingObject
        {
            private get
            {
                return i_CallingObject;
            }
            set
            {
                i_CallingObject = value;
            }
        }

        public string PhysicianRelationshipToView
        {
            private get
            {
                return i_PhysicianRelationshipToView;
            }
            set
            {
                i_PhysicianRelationshipToView = value;
            }
        }
        #endregion

        #region Private Methods
        private bool IsNationalProviderIdentifierTheCorrectLength()
        {
            return ( mtbNationalProviderIdentifier.Text.Trim().Length == NPI_LENGTH );
        }

        private void PopulatePhysicianInformation( Physician physician )
        {
            int index = -1;

            mtbLastName.Text = physician.Name.LastName;
            mtbFirstName.Text = physician.Name.FirstName;
            mtbMiddleInitial.Text = physician.Name.MiddleInitial;

            mtbStateLicenseNbr.Text = physician.StateLicense;

            mtbUPINnumber.Text = physician.UPIN;
            mtbNationalProviderIdentifier.Text = physician.NPI;

            if ( physician.UPIN == FAUX_UPIN )
            {
                index = cmbStatus.FindString( "UNKNOWN/NONE" );
                if ( index != -1 )
                {
                    cmbStatus.SelectedIndex = index;
                }
                mtbUPINnumber.Enabled = false;
            }
            else
            {
                index = cmbStatus.FindString( "KNOWN" );
                if ( index != -1 )
                {
                    cmbStatus.SelectedIndex = index;
                }
            }

            mtbUPINnumber.Text = physician.UPIN;

            if ( physician.PhoneNumber != null )
            {
                phoneNumberControl.Model = physician.PhoneNumber;
            }
        }

        public bool IsUPINValid()
        {
            if ( mtbUPINnumber.UnMaskedText.Length < 6 && mtbUPINnumber.UnMaskedText.Length > 0 )
            {
                SetUPINErrBgColor();
                MessageBox.Show( UIErrorMessages.INVALID_UPIN_NUMBER,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1 );

                mtbUPINnumber.Focus();

                return false;
            }
            return true;
        }

        private void SetUPINErrBgColor()
        {
            UIColors.SetErrorBgColor( mtbUPINnumber );
            Refresh();
        }

        private void CheckRequiredFields()
        {
            if ( mtbLastName.Text.Length > 0 &&
                mtbFirstName.Text.Length > 0 &&
                mtbUPINnumber.Text.Length > 0 &&
                mtbNationalProviderIdentifier.Text.Length > 0 &&
                cmbStatus.SelectedIndex != -1 )
            {
                YesNoFlag yesNo = new YesNoFlag();
                yesNo.SetYes();
                EnableOKButton( this, new LooseArgs( yesNo ) );
            }
            else
            {
                YesNoFlag yesNo = new YesNoFlag();
                yesNo.SetNo();
                EnableOKButton( this, new LooseArgs( yesNo ) );
            }

            phoneNumberControl.RunRules();
        }

        private void RegisterRulesEvents()
        {
            if ( !blnRulesRegistered )
            {
                blnRulesRegistered = true;

                RuleEngine.GetInstance().RegisterEvent( typeof( PhysicianFirstNameRequired ), NonStaffPhysician, new EventHandler( PhysicianFirstNameRequiredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( PhysicianLastNameRequired ), NonStaffPhysician, new EventHandler( PhysicianLastNameRequiredEventHandler ) );

                RuleEngine.GetInstance().RegisterEvent( typeof( PhysicianStateLicenseNumberPreferred ), NonStaffPhysician, new EventHandler( PhysicianStateLicenseNumberPreferredEventHandler ) );

                RuleEngine.GetInstance().RegisterEvent( typeof( PhysicianUPINStatusRequired ), cmbStatus.SelectedIndex, new EventHandler( PhysicianUPINStatusRequiredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( PhysicianUPINNumberRequired ), NonStaffPhysician, new EventHandler( PhysicianUPINNumberRequiredEventHandler ) );

                RuleEngine.GetInstance().RegisterEvent( typeof( NonStaffNPIIsValid ), NonStaffPhysician, new EventHandler( NonStaffNPIIsValidHandler ) );
                RuleEngine.GetInstance().RegisterEvent<NonStaffPhysicianNPIRequired>( NonStaffPhysician, Model.Activity, NonStaffPhysicianNPIRequiredEventHandler );
            }
        }

        private void UnRegisterRulesEvents()
        {
            blnRulesRegistered = false;

            RuleEngine.GetInstance().UnregisterEvent( typeof( PhysicianFirstNameRequired ), NonStaffPhysician, PhysicianFirstNameRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PhysicianLastNameRequired ), NonStaffPhysician, PhysicianLastNameRequiredEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( PhysicianStateLicenseNumberPreferred ), NonStaffPhysician, PhysicianStateLicenseNumberPreferredEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( PhysicianUPINStatusRequired ), cmbStatus, PhysicianUPINStatusRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PhysicianUPINNumberRequired ), NonStaffPhysician, PhysicianUPINNumberRequiredEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( NonStaffNPIIsValid ), NonStaffPhysician, NonStaffNPIIsValidHandler );
            RuleEngine.GetInstance().UnregisterEvent<NonStaffPhysicianNPIRequired>( NonStaffPhysician, NonStaffPhysicianNPIRequiredEventHandler );
        }

        public bool RunRules()
        {
            // reset all fields that might have error, preferred, or required backgrounds

            if ( mtbNationalProviderIdentifier.Text.Trim() != string.Empty
                   && !IsNationalProviderIdentifierTheCorrectLength() )
            {
                MessageBox.Show( UIErrorMessages.INVALID_NPI_LENGTH,
                 "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1 );
                UIColors.SetErrorBgColor( mtbNationalProviderIdentifier );
                mtbNationalProviderIdentifier.Focus();
                return false;
            }

            UIColors.SetNormalBgColor( mtbLastName );
            UIColors.SetNormalBgColor( mtbFirstName );
            UIColors.SetNormalBgColor( mtbStateLicenseNbr );
            UIColors.SetNormalBgColor( cmbStatus );
            UIColors.SetNormalBgColor( mtbUPINnumber );
            UIColors.SetNormalBgColor( mtbNationalProviderIdentifier );

            RuleEngine.GetInstance().EvaluateRule( typeof( PhysicianFirstNameRequired ), NonStaffPhysician );
            RuleEngine.GetInstance().EvaluateRule( typeof( PhysicianLastNameRequired ), NonStaffPhysician );
            RuleEngine.GetInstance().EvaluateRule( typeof( PhysicianStateLicenseNumberPreferred ), NonStaffPhysician );
            RuleEngine.GetInstance().EvaluateRule( typeof( PhysicianUPINStatusRequired ), NonStaffPhysician );
            RuleEngine.GetInstance().EvaluateRule( typeof( PhysicianUPINNumberRequired ), NonStaffPhysician );
            RuleEngine.GetInstance().EvaluateRule<NonStaffPhysicianNPIRequired>( NonStaffPhysician, Model.Activity );
            bool result = RuleEngine.GetInstance().EvaluateRule( typeof( NonStaffNPIIsValid ), NonStaffPhysician );

            return result;
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( RecordNonStaffPhysicianView ) );
            this.gbPhysicianName = new System.Windows.Forms.GroupBox();
            this.mtbLastName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblLastName = new System.Windows.Forms.Label();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.mtbFirstName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblMiddleInitial = new System.Windows.Forms.Label();
            this.mtbMiddleInitial = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.gbUPIN = new System.Windows.Forms.GroupBox();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.mtbUPINnumber = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbStateLicenseNbr = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblNumberVal = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.mtbNationalProviderIdentifier = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.phoneNumberControl = new PatientAccess.UI.CommonControls.PhoneNumberControl();
            this.gbPhysicianName.SuspendLayout();
            this.gbUPIN.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbPhysicianName
            // 
            this.gbPhysicianName.Controls.Add( this.mtbLastName );
            this.gbPhysicianName.Controls.Add( this.lblLastName );
            this.gbPhysicianName.Controls.Add( this.lblFirstName );
            this.gbPhysicianName.Controls.Add( this.mtbFirstName );
            this.gbPhysicianName.Controls.Add( this.lblMiddleInitial );
            this.gbPhysicianName.Controls.Add( this.mtbMiddleInitial );
            this.gbPhysicianName.Location = new System.Drawing.Point( 5, 10 );
            this.gbPhysicianName.Name = "gbPhysicianName";
            this.gbPhysicianName.Size = new System.Drawing.Size( 625, 54 );
            this.gbPhysicianName.TabIndex = 0;
            this.gbPhysicianName.TabStop = false;
            this.gbPhysicianName.Text = "Physician name";
            // 
            // mtbLastName
            // 
            this.mtbLastName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbLastName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbLastName.KeyPressExpression = "^[a-zA-Z][a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbLastName.Location = new System.Drawing.Point( 42, 22 );
            this.mtbLastName.Mask = "";
            this.mtbLastName.MaxLength = 25;
            this.mtbLastName.Name = "mtbLastName";
            this.mtbLastName.Size = new System.Drawing.Size( 287, 20 );
            this.mtbLastName.TabIndex = 1;
            this.mtbLastName.ValidationExpression = "^[a-zA-Z][a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbLastName.Validating += new System.ComponentModel.CancelEventHandler( this.mtbLastName_Validating );
            // 
            // lblLastName
            // 
            this.lblLastName.Location = new System.Drawing.Point( 10, 26 );
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size( 31, 15 );
            this.lblLastName.TabIndex = 0;
            this.lblLastName.Text = "Last:";
            // 
            // lblFirstName
            // 
            this.lblFirstName.Location = new System.Drawing.Point( 340, 26 );
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size( 29, 15 );
            this.lblFirstName.TabIndex = 0;
            this.lblFirstName.Text = "First:";
            // 
            // mtbFirstName
            // 
            this.mtbFirstName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbFirstName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbFirstName.KeyPressExpression = "^[a-zA-Z][a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbFirstName.Location = new System.Drawing.Point( 371, 22 );
            this.mtbFirstName.Mask = "";
            this.mtbFirstName.MaxLength = 13;
            this.mtbFirstName.Name = "mtbFirstName";
            this.mtbFirstName.Size = new System.Drawing.Size( 173, 20 );
            this.mtbFirstName.TabIndex = 2;
            this.mtbFirstName.ValidationExpression = "^[a-zA-Z][a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbFirstName.Validating += new System.ComponentModel.CancelEventHandler( this.mtbFirstName_Validating );
            // 
            // lblMiddleInitial
            // 
            this.lblMiddleInitial.Location = new System.Drawing.Point( 555, 26 );
            this.lblMiddleInitial.Name = "lblMiddleInitial";
            this.lblMiddleInitial.Size = new System.Drawing.Size( 20, 15 );
            this.lblMiddleInitial.TabIndex = 0;
            this.lblMiddleInitial.Text = "MI:";
            // 
            // mtbMiddleInitial
            // 
            this.mtbMiddleInitial.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbMiddleInitial.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbMiddleInitial.KeyPressExpression = "^[a-zA-Z][a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbMiddleInitial.Location = new System.Drawing.Point( 580, 22 );
            this.mtbMiddleInitial.Mask = "";
            this.mtbMiddleInitial.MaxLength = 1;
            this.mtbMiddleInitial.Name = "mtbMiddleInitial";
            this.mtbMiddleInitial.Size = new System.Drawing.Size( 20, 20 );
            this.mtbMiddleInitial.TabIndex = 3;
            this.mtbMiddleInitial.ValidationExpression = "^[a-zA-Z][a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point( 6, 76 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 49, 15 );
            this.label2.TabIndex = 0;
            this.label2.Text = "Number:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point( 6, 127 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 114, 15 );
            this.label3.TabIndex = 0;
            this.label3.Text = "State license number:";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point( 10, 23 );
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size( 40, 15 );
            this.label4.TabIndex = 0;
            this.label4.Text = "Status:";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point( 10, 49 );
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size( 49, 15 );
            this.label5.TabIndex = 0;
            this.label5.Text = "Number:";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point( 6, 246 );
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size( 41, 15 );
            this.label6.TabIndex = 0;
            this.label6.Text = "Phone:";
            // 
            // gbUPIN
            // 
            this.gbUPIN.Controls.Add( this.label4 );
            this.gbUPIN.Controls.Add( this.label5 );
            this.gbUPIN.Controls.Add( this.cmbStatus );
            this.gbUPIN.Controls.Add( this.mtbUPINnumber );
            this.gbUPIN.Location = new System.Drawing.Point( 5, 154 );
            this.gbUPIN.Name = "gbUPIN";
            this.gbUPIN.Size = new System.Drawing.Size( 190, 79 );
            this.gbUPIN.TabIndex = 4;
            this.gbUPIN.TabStop = false;
            this.gbUPIN.Text = "UPIN";
            // 
            // cmbStatus
            // 
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.Items.AddRange( new object[] {
            "KNOWN",
            "UNKNOWN/NONE"} );
            this.cmbStatus.Location = new System.Drawing.Point( 60, 20 );
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size( 119, 21 );
            this.cmbStatus.TabIndex = 1;
            this.cmbStatus.SelectedIndexChanged += new System.EventHandler( this.cmbStatus_SelectedIndexChanged );
            // 
            // mtbUPINnumber
            // 
            this.mtbUPINnumber.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbUPINnumber.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbUPINnumber.KeyPressExpression = "^[a-zA-Z][a-zA-Z0-9]{0,2}[0-9]{0,3}$";
            this.mtbUPINnumber.Location = new System.Drawing.Point( 60, 46 );
            this.mtbUPINnumber.Mask = "";
            this.mtbUPINnumber.MaxLength = 6;
            this.mtbUPINnumber.Name = "mtbUPINnumber";
            this.mtbUPINnumber.Size = new System.Drawing.Size( 62, 20 );
            this.mtbUPINnumber.TabIndex = 2;
            this.mtbUPINnumber.ValidationExpression = "^[a-zA-Z][a-zA-Z0-9]{2}[0-9]{3}$";
            this.mtbUPINnumber.Validating += new System.ComponentModel.CancelEventHandler( this.mtbUPINnumber_Validating );
            // 
            // mtbStateLicenseNbr
            // 
            this.mtbStateLicenseNbr.BackColor = System.Drawing.Color.White;
            this.mtbStateLicenseNbr.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbStateLicenseNbr.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbStateLicenseNbr.KeyPressExpression = "^[ a-zA-Z0-9 ]*$";
            this.mtbStateLicenseNbr.Location = new System.Drawing.Point( 173, 124 );
            this.mtbStateLicenseNbr.Mask = "";
            this.mtbStateLicenseNbr.MaxLength = 14;
            this.mtbStateLicenseNbr.Name = "mtbStateLicenseNbr";
            this.mtbStateLicenseNbr.Size = new System.Drawing.Size( 173, 20 );
            this.mtbStateLicenseNbr.TabIndex = 3;
            this.mtbStateLicenseNbr.Validating += new System.ComponentModel.CancelEventHandler( this.mtbStateLicenseNbr_Validating );
            // 
            // lblNumberVal
            // 
            this.lblNumberVal.Location = new System.Drawing.Point( 170, 76 );
            this.lblNumberVal.Name = "lblNumberVal";
            this.lblNumberVal.Size = new System.Drawing.Size( 41, 15 );
            this.lblNumberVal.TabIndex = 1;
            this.lblNumberVal.Text = "08888";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 6, 101 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 161, 13 );
            this.label1.TabIndex = 5;
            this.label1.Text = "National Provider Identifier (NPI):";
            // 
            // mtbNationalProviderIdentifier
            // 
            this.mtbNationalProviderIdentifier.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbNationalProviderIdentifier.KeyPressExpression = "^[0-9]*$";
            this.mtbNationalProviderIdentifier.Location = new System.Drawing.Point( 173, 98 );
            this.mtbNationalProviderIdentifier.Mask = "";
            this.mtbNationalProviderIdentifier.MaxLength = 10;
            this.mtbNationalProviderIdentifier.Name = "mtbNationalProviderIdentifier";
            this.mtbNationalProviderIdentifier.Size = new System.Drawing.Size( 100, 20 );
            this.mtbNationalProviderIdentifier.TabIndex = 2;
            this.mtbNationalProviderIdentifier.Validating += new System.ComponentModel.CancelEventHandler( this.mtbNationalProviderIdentifier_Validating );
            // 
            // phoneNumberControl
            // 
            this.phoneNumberControl.AreaCode = "";
            this.phoneNumberControl.Location = new System.Drawing.Point( 62, 239 );
            this.phoneNumberControl.Model = ( ( PatientAccess.Domain.Parties.PhoneNumber )( resources.GetObject( "phoneNumberControl.Model" ) ) );
            this.phoneNumberControl.Name = "phoneNumberControl";
            this.phoneNumberControl.PhoneNumber = "";
            this.phoneNumberControl.Size = new System.Drawing.Size( 94, 27 );
            this.phoneNumberControl.TabIndex = 5;
            // 
            // RecordNonStaffPhysicianView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.phoneNumberControl );
            this.Controls.Add( this.mtbNationalProviderIdentifier );
            this.Controls.Add( this.label1 );
            this.Controls.Add( this.lblNumberVal );
            this.Controls.Add( this.mtbStateLicenseNbr );
            this.Controls.Add( this.gbUPIN );
            this.Controls.Add( this.label6 );
            this.Controls.Add( this.label3 );
            this.Controls.Add( this.label2 );
            this.Controls.Add( this.gbPhysicianName );
            this.Name = "RecordNonStaffPhysicianView";
            this.Size = new System.Drawing.Size( 646, 275 );
            this.Enter += new System.EventHandler( this.RecordNonStaffPhysicianView_Enter );
            this.Leave += new System.EventHandler( this.RecordNonStaffPhysicianView_Leave );
            this.gbPhysicianName.ResumeLayout( false );
            this.gbPhysicianName.PerformLayout();
            this.gbUPIN.ResumeLayout( false );
            this.gbUPIN.PerformLayout();
            this.ResumeLayout( false );
            this.PerformLayout();

        }
        #endregion

        #endregion

        #region Private Properties

        private Physician NonStaffPhysician
        {
            get
            {
                return i_NonStaffPhysician;
            }
            set
            {
                i_NonStaffPhysician = value;
            }
        }
        #endregion

        #region Construction and Finalization
        public RecordNonStaffPhysicianView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call

        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            UnRegisterRulesEvents();

            if ( disposing )
            {
                if ( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements

        private Container components = null;
        private GroupBox gbPhysicianName;
        private Label lblLastName;
        private Label lblFirstName;
        private Label lblMiddleInitial;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label1;
        private MaskedEditTextBox mtbLastName;
        private MaskedEditTextBox mtbFirstName;
        private MaskedEditTextBox mtbMiddleInitial;
        private MaskedEditTextBox mtbNationalProviderIdentifier;
        private GroupBox gbUPIN;
        private MaskedEditTextBox mtbUPINnumber;
        private ComboBox cmbStatus;
        private MaskedEditTextBox mtbStateLicenseNbr;
        private Label lblNumberVal;
        private string i_CallingObject;
        private string i_PhysicianRelationshipToView;
        private PhysicianRelationship physicianRelationship;
        private Physician i_NonStaffPhysician = new Physician();
        private bool blnRulesRegistered;
        private bool blnLeaving;
        private bool npiValidated;

        #endregion

        #region Constants

        private const string UNKNOWN_STATUS = "UNKNOWN/NONE";
        private const string FAUX_UPIN = "OTH000";
        private const long NONSTAFFPHYSICIAN_NBR = 8888L;
        private PhoneNumberControl phoneNumberControl;
        private const int NPI_LENGTH = 10;

        #endregion

    }
}
