using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.AddressViews;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.CommonControls.Email.Presenters;
using PatientAccess.UI.CommonControls.Suffix.Presenters;
using PatientAccess.UI.CommonControls.Suffix.ViewImpl;
using PatientAccess.UI.Factories;
using PatientAccess.UI.GuarantorViews.Presenters;
using PatientAccess.UI.GuarantorViews.Views;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.GuarantorViews
{
    /// <summary>
    /// Summary description for GuarantorView.
    /// </summary>
    [Serializable]
    public class GuarantorView : ControlView  , IGuarantorDateOfBirthView
    {
        #region Events
        public event EventHandler EnableInsuranceTab;
        #endregion

        #region Event Handlers

        #region Rule Event Handlers

        private void PersonRelationshipRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( relationshipView.ComboBox );
            Refresh();
        }

        private void PersonLastNameRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbLastName );
            Refresh();
        }

        private void PersonFirstNameRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbFirstName );
            Refresh();
        }

        private void PersonDriversLicenseStatePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cmbDLState );
            Refresh();
        }

        private void PersonEmployerRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( (Control)employmentView.EmployerField );
            Refresh();
        }

        private void PersonRelationshipPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( relationshipView.ComboBox );
            Refresh();
        }

        private void PersonLastNamePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( mtbLastName );
            Refresh();
        }

        private void PersonFirstNamePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( mtbFirstName );
            Refresh();
        }

        private void PersonGenderPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( genderControl.ComboBox );
            Refresh();
        }

        private void PersonDriversLicensePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( mtbDLNumber );
            Refresh();
        }

        private void GuarantorEmploymentPhoneNumberPreferredEvent(object sender, EventArgs e)
        {
            PropertyChangedArgs args = (PropertyChangedArgs)e;
            Control aControl = args.Context as Control;

            if ( aControl == employmentView.PhoneNumberControl &&
                 employmentView.PhoneNumberControl != null )
            {
                employmentView.PhoneNumberControl.SetPhoneNumberPreferredColor();
                Refresh();
            }

        }

        private void GuarantorEmploymentPhoneAreaCodePreferredEvent(object sender, EventArgs e)
        {
            PropertyChangedArgs args = (PropertyChangedArgs)e;
            Control aControl = args.Context as Control;

            if ( aControl == employmentView.PhoneNumberControl &&
                 employmentView.PhoneNumberControl != null )
            {
                employmentView.PhoneNumberControl.SetAreaCodePreferredColor();
                Refresh();
            }

        }

        private void GuarantorEmploymentStatusPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( employmentView.ComboBox );
            Refresh();
        }

        private void GuarantorEmployerAddressPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( (Control)employmentView.EmployerField );
            Refresh();
        }

        //---------------------Evaluate ComboBoxes -------------------------------------------------------------
        private void relationshipView_RelationshipValidating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( relationshipView.ComboBox );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidRelForGuarantor ), Model_Account );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidRelForGuarantorChange ), Model_Account );
            }
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonRelationshipRequired ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonRelationshipPreferred ), Model_Account.Guarantor );
        }
        private void genderControl_GenderControlValidating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( genderControl.ComboBox );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidGenderForGuarantor ), Model_Account );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidGenderForGuarantorChange ), Model_Account );
            }

            RuleEngine.GetInstance().EvaluateRule( typeof( PersonGenderPreferred ), Model_Account.Guarantor );
        }
        private void employmentView_EmploymentStatusForGuarantorValidating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                if ( employmentView.ComboBox.BackColor != UIColors.TextFieldBackgroundPreferred
                    && employmentView.ComboBox.BackColor != UIColors.TextFieldBackgroundRequired )
                {
                    UIColors.SetNormalBgColor( employmentView.ComboBox );
                }

                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidEmpStatusForGuarantor ), Model_Account );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidEmpStatusForGuarantorChange ), Model_Account );
            }
        }
        //--------------------- InvalidValues in Comboboxes Event Handlers: ------------------------------------

        private static void ProcessInvalidCodeEvent( PatientAccessComboBox comboBox )
        {
            UIColors.SetDeactivatedBgColor( comboBox );

            MessageBox.Show( UIErrorMessages.INVALID_VALUE_ERRMSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1 );

            if ( !comboBox.Focused )
            {
                comboBox.Focus();
            }
        }

        private void InvalidRelForGuarantorChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( relationshipView.ComboBox );
        }
        private void InvalidGenderForGuarantorChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( genderControl.ComboBox );
        }
        private void InvalidEmpStatusForGuarantorChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( employmentView.ComboBox );
        }


        //-----------------------------------------------------------------

        private void InvalidRelForGuarantorEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( relationshipView.ComboBox );
        }
        private void InvalidGenderForGuarantorEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( genderControl.ComboBox );
        }
        private void InvalidEmpStatusForGuarantorEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( employmentView.ComboBox );
        }

        #endregion

        private void GuarantorView_Enter( object sender, EventArgs e )
        {
            RegisterEventHandlers();
            IAccountView accountView = AccountView.GetInstance();

            // Display message where the patient is over 65 and if the user selects a 
            // non-Medicare Primary payor and the secondary payor is not entered or null.
            if ( accountView.IsMedicareAdvisedForPatient() )
            {
                accountView.MedicareOver65Checked = true;

                DialogResult warningResult = MessageBox.Show( UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_QUESTION,
                    UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_TITLE,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning );

                if ( warningResult == DialogResult.Yes )
                {
                    if ( EnableInsuranceTab != null )
                    {
                        EnableInsuranceTab( this, new LooseArgs( Model_Account ) );
                    }
                }
            }
        }

        private void GuarantorView_Leave( object sender, EventArgs e )
        {
            blnLeaveRun = true;
            RuleEngine.GetInstance().EvaluateRule( typeof( OnGuarantorForm ), Model_Account );
            blnLeaveRun = false;

            stopResponsePollTimer();
            stopResponseWaitTimer();
            UnregisterEventHandlers();
        }

        private void GuarantorView_Disposed( object sender, EventArgs e )
        {
            UnregisterEventHandlers();
        }

        private void GuarantorValidationFieldsProvidedEventHandler( object sender, EventArgs e )
        {
            lblValidationStatus.Text = MISSING_FIELDS;
            btnInitiate.Enabled = false;
        }

        private void EmploymentView_PhoneNumberChanged(object sender, EventArgs e)
        {

            RuleEngine.GetInstance()
                .EvaluateRule(typeof (GuarantorEmploymentPhoneNumberPreferred), Model_Account,
                    employmentView.PhoneNumberControl);
            RuleEngine.GetInstance()
                .EvaluateRule(typeof (GuarantorEmploymentPhoneAreaCodePreferred), Model_Account,
                    employmentView.PhoneNumberControl);
        }

        private void EmploymentView_AreaCodeChanged(object sender, EventArgs e)
        {
            RuleEngine.GetInstance()
                .EvaluateRule(typeof (GuarantorEmploymentPhoneNumberPreferred), Model_Account,
                    employmentView.PhoneNumberControl);
            RuleEngine.GetInstance()
                .EvaluateRule(typeof (GuarantorEmploymentPhoneAreaCodePreferred), Model_Account,
                    employmentView.PhoneNumberControl);
        }

        private void employmentView_EmploymentViewChangedEvent( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( (Control)employmentView.EmployerField );
            UIColors.SetNormalBgColor( employmentView.ComboBox );

            Model_Account.Guarantor.Employment = employmentView.Model_Employment;

            RuleEngine.GetInstance().EvaluateRule( typeof( GuarantorEmploymentStatusPreferred ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule(typeof(GuarantorEmploymentPhoneNumberPreferred), Model_Account, employmentView.PhoneNumberControl);
            RuleEngine.GetInstance().EvaluateRule(typeof(GuarantorEmploymentPhoneAreaCodePreferred), Model_Account, employmentView.PhoneNumberControl);

            // it is imperative that these two rules are run in this order
            RuleEngine.GetInstance().EvaluateRule( typeof( GuarantorEmployerAddressPreferred ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonEmployerRequired ), Model_Account.Guarantor, employmentView.EmployerField );

        }

        private void AddressChangedEventHandler( object sender, EventArgs e )
        {
            ContactPoint mailingContactPoint = Model_Account.Guarantor.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() );
            mailingContactPoint.Address = addressView.Model_ContactPoint.Address;

            RuleEngine.GetInstance().EvaluateRule( typeof( GuarantorAddressRequired ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( GuarantorAddressPreferred ), Model_Account.Guarantor );

            RunGuarantorValidationRule();
        }

        private void OnPartySelected( object sender, EventArgs e )
        {
            LooseArgs args = (LooseArgs)e;
            Party partySelected = (Party)args.Context;

            if ( partySelected != null )
            {
                string partyType = copyPartyView.ComboBox.Text;
                ResetView( false );

                // load the fields from the selected party

                aGuarantor = partySelected.CopyAsGuarantor();

                RelationshipType aRelationshipType = new RelationshipType();

                Model_Account.GuarantorIs( aGuarantor, aRelationshipType );

                UpdateView( false );

                // set the relationship type

                string relType = string.Empty;

                switch ( partyType )
                {
                    case "Patient":
                        relType = SELF;
                        break;
                    case "Patient's Employer":
                        relType = EMPLOYEE;
                        break;

                    case "Insured - Primary":
                    case "Insured - Secondary":
                        foreach ( Relationship r in partySelected.Relationships )
                        {
                            relType = r.Type.Description;
                            break;
                        }
                        break;

                    case "Primary Insured's Employer":
                        break;

                    case "Secondary Insured's Employer":
                        break;

                    default:
                        break;
                }

                relationshipView.ComboBox.SelectedIndex = relationshipView.ComboBox.FindString( relType );
            }
            GuarantorDateOfBirthPresenter.HandleGuarantorDateOfBirth();
            RunRules();
        }
      
        /// <summary>
        /// GenderSelectedEvent - sent from the GenderControl common control
        /// </summary>
        private void GenderSelectedEvent( object sender, EventArgs e )
        {
            LooseArgs args = (LooseArgs)e;
            if ( args != null )
            {
                gender = args.Context as Gender;
            }
            else
            {
                gender = new Gender();
            }

            Model_Account.Guarantor.Sex = gender;

            UIColors.SetNormalBgColor( genderControl.ComboBox );

            RuleEngine.GetInstance().EvaluateRule( typeof( PersonGenderPreferred ), Model_Account.Guarantor );
        }

        private void RelationshipSelectedEvent( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( employmentView.ComboBox );
            UIColors.SetNormalBgColor( (Control)employmentView.EmployerField );
            UIColors.SetNormalBgColor( relationshipView.ComboBox );
            UIColors.SetNormalBgColor( mtbFirstName );
            UIColors.SetNormalBgColor( genderControl.ComboBox );
            ssnView.SetSSNNumberNormalColor();

            LooseArgs args = (LooseArgs)e;
            if ( args == null )
            {
                return;
            }
            selectedRelationshipType = args.Context as RelationshipType;

            if ( i_OriginalRelationship != null
                && i_OriginalRelationship.Type != null )
            {
                Model_Account.Guarantor.RemoveRelationship( i_OriginalRelationship );
                Model_Account.Patient.RemoveRelationship( i_OriginalRelationship );
            }

            if ( selectedRelationshipType != null )
            {
                Relationship newRelationship = new Relationship( selectedRelationshipType,
                    Model_Account.Patient.GetType(), Model_Account.Guarantor.GetType() );

                i_OriginalRelationship = newRelationship;

                Model_Account.Guarantor.AddRelationship( newRelationship );
                Model_Account.Patient.AddRelationship( newRelationship );
            }
            GuarantorDateOfBirthPresenter.HandleGuarantorDateOfBirth();
            UIColors.SetNormalBgColor( relationshipView.ComboBox );

            RuleEngine.GetInstance().EvaluateRule( typeof( PersonRelationshipRequired ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonRelationshipPreferred ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonLastNameRequired ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonLastNamePreferred ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonFirstNameRequired ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonFirstNamePreferred ), Model_Account.Guarantor );
            ssnView.RunRules();
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonGenderPreferred ), Model_Account.Guarantor );

            RuleEngine.GetInstance().EvaluateRule( typeof( GuarantorEmploymentStatusPreferred ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( GuarantorEmploymentPhoneNumberPreferred ), Model_Account , employmentView.PhoneNumberControl );
            RuleEngine.GetInstance().EvaluateRule( typeof( GuarantorEmploymentPhoneAreaCodePreferred ), Model_Account , employmentView.PhoneNumberControl );

            // it is imperative that these two rules are run in this order
            RuleEngine.GetInstance().EvaluateRule( typeof( GuarantorEmployerAddressPreferred ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonEmployerRequired ), Model_Account.Guarantor, employmentView.EmployerField );
        }

        private void GuarantorView_Validating( object sender, CancelEventArgs e )
        {
            // Required fields will be validated here once they are defined.
            UpdateModel();
        }

        private void addressView_AreaCodeChanged( object sender, EventArgs e )
        {
            ContactPoint mailingContactPoint = Model_Account.Guarantor.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() );
            mailingContactPoint.PhoneNumber = addressView.Model_ContactPoint.PhoneNumber;

            RuleEngine.GetInstance().EvaluateRule( typeof( PersonPhoneAreaCodePreferred ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonPhoneNumberPreferred ), Model_Account.Guarantor );
        }

        private void addressView_PhoneNumberChanged( object sender, EventArgs e )
        {
            ContactPoint mailingContactPoint = Model_Account.Guarantor.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() );
            mailingContactPoint.PhoneNumber = addressView.Model_ContactPoint.PhoneNumber;

            RuleEngine.GetInstance().EvaluateRule( typeof( PersonPhoneAreaCodePreferred ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonPhoneNumberPreferred ), Model_Account.Guarantor );
        }

        private void addressView_CellPhoneNumberChanged( object sender, EventArgs e )
        {
            ContactPoint mobileContactPoint = Model_Account.Guarantor.ContactPointWith( TypeOfContactPoint.NewMobileContactPointType() );
            mobileContactPoint.PhoneNumber = addressView.Model_ContactPoint.CellPhoneNumber;
            addressView.ValidateCellPhoneConsent();
        }

        private void addressView_CellPhoneConsentChanged(object sender, EventArgs e)
        {
            ContactPoint mobileContactPoint = Model_Account.Guarantor.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType());
            mobileContactPoint.CellPhoneConsent = addressView.Model_ContactPoint.CellPhoneConsent;
        } 

        private void addressView_EmailChanged( object sender, EventArgs e )
        {
            ContactPoint mailingContactPoint = Model_Account.Guarantor.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() );
            mailingContactPoint.EmailAddress = addressView.Model_ContactPoint.EmailAddress;
            EmailAddressPresenter.RunGurantorEmailAddressRules();
        }

        private void cellPhoneNumberControl_PhoneNumberTextChanged(object sender, EventArgs e)
        {
            var guarantorCellPhoneConsentFeatureManager = new GuarantorCellPhoneConsentFeatureManager();
            var featureIsEnabledToDefaultForCOSSigned =
                guarantorCellPhoneConsentFeatureManager.IsCellPhoneConsentRuleForCOSEnabledforaccount(Model_Account);
            if (!featureIsEnabledToDefaultForCOSSigned)
            {
                addressView.ResetCellPhoneConsentValue();
                Model_Account.Guarantor.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType())
                    .CellPhoneConsent = new CellPhoneConsent();
            }
        }

        private void mtbDLNumber_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbDLNumber );
            Model_Account.Guarantor.DriversLicense.Number = mtbDLNumber.Text;

            SetDriversLicense();

            RuleEngine.GetInstance().EvaluateRule( typeof( PersonDriversLicensePreferred ), Model_Account.Guarantor );
        }

        private void DriversLicenseState_SelectedIndexChanged( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( cmbDLState );
            if ( cmbDLState.SelectedIndex >= 0 )
            {
                driversLicenseState = (State)cmbDLState.SelectedItem;
            }

            Model_Account.Guarantor.DriversLicense.State = driversLicenseState;
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonDriversLicenseStatePreferred ), Model_Account );
        }

        private void cmbDLState_Validating( object sender, CancelEventArgs e )
        {
            if ( cmbDLState.SelectedItem != null )
            {
                Model_Account.Guarantor.DriversLicense.State = (State)cmbDLState.SelectedItem;
            }

            UIColors.SetNormalBgColor( cmbDLState );

            RuleEngine.GetInstance().EvaluateRule( typeof( PersonDriversLicenseStatePreferred ), Model_Account );
        }

        private void btnClearAll_Click( object sender, EventArgs e )
        {
            ResetView( true );
            RunRules();
        }

        private void mtbLastName_Validating( object sender, CancelEventArgs e )
        {
            Model_Account.Guarantor.LastName = mtbLastName.Text;

            UIColors.SetNormalBgColor( mtbLastName );

            RuleEngine.GetInstance().EvaluateRule( typeof( PersonLastNameRequired ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonLastNamePreferred ), Model_Account.Guarantor );

            RunGuarantorValidationRule();
        }

        private void mtbFirstName_Validating( object sender, CancelEventArgs e )
        {
            Model_Account.Guarantor.FirstName = mtbFirstName.Text;

            UIColors.SetNormalBgColor( mtbFirstName );

            RuleEngine.GetInstance().EvaluateRule( typeof( PersonFirstNameRequired ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonFirstNamePreferred ), Model_Account.Guarantor );

            RunGuarantorValidationRule();
        }

        private void mtbMI_Validating( object sender, CancelEventArgs e )
        {
            Model_Account.Guarantor.Name.MiddleInitial = mtbMI.Text;
        }

        #endregion

        #region Methods

        public override void UpdateView()
        {
            EmailAddressPresenter = new EmailAddressPresenter(addressView, Model_Account, RuleEngine.GetInstance());
            GuarantorDateOfBirthPresenter = new GuarantorDateOfBirthPresenter(this, Model_Account, RuleEngine.GetInstance(), new MessageBoxAdapter());
            suffixPresenter = new SuffixPresenter(suffixView, Model_Account , "Guarantor" );
            ssnView.SsnFactory = new SsnFactoryCreator(Model_Account).GetSsnFactory();
            UpdateView( true );
            

            if ( loadingModelData )
            {
                // During initial load of account, validate incoming Guarantor Social Security 
                // Number for the Status and update SSN control with the validated SSN and Status.
                ssnView.UpdateView();
            }
            copyPartyView.ComboBox.Focus();
            addressView.ValidateEmailAddress();
            loadingModelData = false;
        }

        /// <summary>
        /// UpdateView method.
        /// </summary>
        private void UpdateView( bool updateCopyParty )
        {
            
            StateComboHelper = new ReferenceValueComboBox( cmbDLState );

            RuleEngine.GetInstance().LoadRules( Model_Account );
            RegisterEventHandlers();

            InitializeDLStatesComboBox();
            genderControl.InitializeGendersComboBox();

            if ( Model_Account == null )
            {
                return;
            }

            foreach ( Relationship r in Model_Account.Guarantor.Relationships )
            {
                i_OriginalRelationship = r;
                break;
            }

            mtbLastName.UnMaskedText = Model_Account.Guarantor.Name.LastName;
            mtbFirstName.UnMaskedText = Model_Account.Guarantor.Name.FirstName;
            mtbMI.UnMaskedText = Model_Account.Guarantor.Name.MiddleInitial;
            suffixPresenter.UpdateView();

            addressView.PatientAccount = Model_Account;
            addressView.KindOfTargetParty = Model_Account.Guarantor.GetType();
            addressView.Context = "Guarantor";

            ContactPoint mailingContactPoint = Model_Account.Guarantor.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() );
            ContactPoint mobileContactPoint = Model_Account.Guarantor.ContactPointWith( TypeOfContactPoint.NewMobileContactPointType() );
            addressView.Model = new ContactPoint( mailingContactPoint.Address, mailingContactPoint.PhoneNumber, mobileContactPoint.PhoneNumber,
                mailingContactPoint.EmailAddress, TypeOfContactPoint.NewMailingContactPointType() );
            addressView.Model_ContactPoint.CellPhoneConsent = mobileContactPoint.CellPhoneConsent;
            addressView.UpdateView();

            ssnView.Model = Model_Account.Guarantor;
            ssnView.ModelAccount = Model_Account;

            if ( updateCopyParty )
            {
                copyPartyView.Model = Model_Account;
                copyPartyView.KindOfTargetParty = Model_Account.Guarantor.GetType();
                copyPartyView.UpdateView();
            }
            else
            {
                // When Guarantor relationship is changed (Example: Selected to be the patient), update SSN 
                // control with the already known SSN and SSN Status values based on Admit Date and State criteria.
                ssnView.UpdateDefaultSocialSecurityNumberForAdmitDate();
            }

            GuarantorDateOfBirthPresenter.HandleGuarantorDateOfBirth();

            employmentView.Model = Model_Account.Guarantor.Employment;
            employmentView.Activity = Model_Account.Activity;
            employmentView.UpdateView();

            if ( Model_Account.Guarantor.Sex != null )
            {
                genderControl.ComboBox.SelectedItem = Model_Account.Guarantor.Sex.AsDictionaryEntry();
            }
            genderControlSelectedIndex = genderControl.ComboBox.SelectedIndex;

            relationshipView.LabelText = "The Patient is the Guarantor's:";
            relationshipView.Model = Model_Account.Guarantor.Relationships;
            relationshipView.PartyForRelationships = Model_Account.Guarantor;
            relationshipView.UpdateView();

            // if admitting newborn, default the relationship to 'Natural Child'

            if ( Model_Account.Activity.GetType() == typeof( AdmitNewbornActivity ) )
            {
                relationshipView.ComboBox.SelectedIndex = -1;
                relationshipView.ComboBox.SelectedIndex = relationshipView.ComboBox.FindString( NATURAL_CHILD );
            }

            mtbDLNumber.UnMaskedText = Model_Account.Guarantor.DriversLicense.Number.TrimEnd();
            SetDriversLicense();

            CheckValidation();

            //Set default state for DL.
            SetDefaultDLState();
            RunRules();
        }

        void IGuarantorDateOfBirthView.ShowMe()
        {
            mtbDob.Visible = true;
            lblStaticDateOfBirth.Visible = true;
        }

        void IGuarantorDateOfBirthView.HideMe()
        {
            mtbDob.Visible = false;
            lblStaticDateOfBirth.Visible = false;
        }

        void IGuarantorDateOfBirthView.Populate(DateTime dateOfBirth)
        {
            if (dateOfBirth != DateTime.MinValue)
            {
                mtbDob.UnMaskedText = dateOfBirth.ToString("MMddyyyy");
                dobDate = dateOfBirth;
            }
            else
            {
                mtbDob.UnMaskedText = String.Empty;
                dobDate = DateTime.MinValue;
            }
        }


        void IGuarantorDateOfBirthView.FocusMe()
        {
            mtbDob.Focus();
        }

        void IGuarantorDateOfBirthView.SetErrorColor()
        {
           UIColors.SetErrorBgColor(mtbDob);
        }

        void IGuarantorDateOfBirthView.SetNormalColor()
        {
            UIColors.SetNormalBgColor(mtbDob);
        }

        void IGuarantorDateOfBirthView.SetPreferredColor()
        {
            UIColors.SetPreferredBgColor(mtbDob);
        }

        void IGuarantorDateOfBirthView.SetRequireedColor()
        {
            UIColors.SetRequiredBgColor(mtbDob);
        }

        string IGuarantorDateOfBirthView.UnmaskedText
        {
            get { return mtbDob.UnMaskedText; }
            set { mtbDob.UnMaskedText = value; }
        }

        /// <summary>
        /// UpdateModel method.
        /// </summary>
        public override void UpdateModel()
        {
            if ( selectedRelationshipType != null )
            {
                relationShip = new Relationship( selectedRelationshipType,
                                                    Model_Account.Patient.GetType(),
                                                    Model_Account.Guarantor.GetType() );

                Model_Account.Guarantor.RemoveRelationship( selectedRelationshipType );
                Model_Account.Guarantor.AddRelationship( relationShip );
            }

            if ( Model_Account.Guarantor.Name != null )
            {
                Model_Account.Guarantor.Name.FirstName = mtbFirstName.Text;
                Model_Account.Guarantor.Name.LastName = mtbLastName.Text;
                Model_Account.Guarantor.Name.MiddleInitial = mtbMI.Text;
            }
            
            Model_Account.Guarantor.DriversLicense.Number = mtbDLNumber.Text;

            if ( driversLicenseState != null )
            {
                int index = cmbDLState.FindString( driversLicenseState.ToString() );
                if ( index != -1 && driversLicenseStateSelectedIndex != index )
                {
                    Model_Account.Guarantor.DriversLicense.State = driversLicenseState;
                }
            }

            if ( aGuarantor != null )
            {
                Model_Account.GuarantorIs( aGuarantor, selectedRelationshipType );
            }
            if ( employmentView.Model_Employment != null &&
                employmentView.Model_Employment != Model_Account.Guarantor.Employment )
            {
                Model_Account.Guarantor.Employment = (Employment)employmentView.Model_Employment.DeepCopy();
            }
            if ( gender != null )
            {
                int index = genderControl.ComboBox.FindString( gender.ToString() );
                if ( index != -1 && index != genderControlSelectedIndex )
                {
                    Model_Account.Guarantor.Sex = gender;
                }
            }
            if ( aGuarantor != null && selectedRelationshipType != null )
            {
                Model_Account.GuarantorIs( aGuarantor, selectedRelationshipType );
            }

            ContactPoint generalContactPoint = Model_Account.Guarantor.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() );
            generalContactPoint.Address = addressView.Model_ContactPoint.Address;
            generalContactPoint.PhoneNumber = addressView.Model_ContactPoint.PhoneNumber;
            generalContactPoint.EmailAddress = addressView.Model_ContactPoint.EmailAddress;

            generalContactPoint = Model_Account.Guarantor.ContactPointWith( TypeOfContactPoint.NewMobileContactPointType() );
            generalContactPoint.PhoneNumber = addressView.Model_ContactPoint.CellPhoneNumber;
        }
        #endregion

        #region Properties
        public Account Model_Account
        {
            private get
            {
                return (Account)Model;
            }
            set
            {
                Model = value;
            }
        }

        public GuarantorDateOfBirthPresenter GuarantorDateOfBirthPresenter { get; set; }

        #endregion

        #region Private Methods

        #region Guarantor validation methods

        private void SetDefaultDLState()
        {
            State searchState = new State();

            if ( Model_Account.Guarantor.DriversLicense != null &&
                Model_Account.Guarantor.DriversLicense.State != null &&
                Model_Account.Guarantor.DriversLicense.State.Code != String.Empty )
            {
                searchState = Model_Account.Guarantor.DriversLicense.State;
            }

            cmbDLState.SelectedItem = searchState;
        }

        private bool FinancialClassWarrantsValidation()
        {
            FinancialClass fc = Model_Account.FinancialClass;

            if ( fc == null )
            {
                return false;
            }

            return fc.WarrantsValidation();
        }

        private void CheckValidation()
        {
            btnInitiate.Enabled = false;

            if ( !FinancialClassWarrantsValidation() )
            {
                lblValidationStatus.Text = NOT_NEEDED;
                btnView.Enabled = false;       // OTD 34909
                btnInitiate.Enabled = false;      // OTD 34909
                ToggleEnabled( true );
                return;
            }


            DataValidationTicket aTicket = Model_Account.Guarantor.DataValidationTicket;

            // if we have a guarantor validation, set up appropriately based on status
            if ( aTicket != null
                && aTicket.TicketId.Trim().Length > 0
                && aTicket.AccountNumber == Model_Account.AccountNumber )
            {
                if ( aTicket.ResultsAvailable )
                {
                    if ( !aTicket.ResultsReviewed )
                    {
                        // the results were never reviewed, so
                        // retrieve the results
                        CreditValidationResponse response = GetValidationResponse();

                        if ( response != null &&
                            response.ResponseCreditReport != null && 
                           HasValuesToCopy() )
                        {
                            Model_Account.Guarantor.CreditReport = response.ResponseCreditReport;
                            btnView.Enabled = true;
                            lblValidationStatus.Text = RESULTS_AVAILABLE;
                            ToggleEnabled( false );
                        }
                        else
                        {
                            // error
                            ToggleEnabled( true );
                            btnView.Enabled = false;
                            lblValidationStatus.Text = NO_RESPONSE;
                        }
                    }
                    else
                    {
                        ToggleEnabled( true );
                        lblValidationStatus.Text = RESULTS_REVIEWED;
                    }
                }
                else
                {
                    // no results available
                    // check initiated time to determine if we still need to poll
                    ToggleEnabled( false );
                    lblValidationStatus.Text = IN_PROGRESS;
                    StartResponseWaitTimer();
                }
            }
            else // no ticket exists
            {
                // enable the Initiate Validation button
                lblValidationStatus.Text = NEEDED;
                btnInitiate.Enabled = false;
                btnView.Enabled = false;
            }
        }
        private bool HasValuesToCopy()
        {
            return !(String.IsNullOrEmpty(Model_Account.Guarantor.LastName.Trim()) ||
                     String.IsNullOrEmpty(Model_Account.Guarantor.FirstName.Trim()));
        }

        private CreditValidationResponse GetValidationResponse()
        {
            if ( Model_Account == null
                || Model_Account.Guarantor == null
                || Model_Account.Guarantor.DataValidationTicket == null )
            {
                return null;
            }

            IDataValidationBroker gBroker =
                BrokerFactory.BrokerOfType<IDataValidationBroker>();

            CreditValidationResponse response = gBroker.GetCreditValidationResponse( Model_Account.Guarantor.DataValidationTicket.TicketId,
                User.GetCurrent().SecurityUser.UPN, User.GetCurrent().Facility.Code );

            return response;
        }

        private bool InitiateCreditValidation()
        {
            bool rc = false;

            try
            {
                IDataValidationBroker dvBroker =
                    BrokerFactory.BrokerOfType<IDataValidationBroker>();

                DataValidationTicket dvt = dvBroker.InitiateGuarantorValidation( Model_Account.Guarantor,
                    User.GetCurrent().SecurityUser.UPN, User.GetCurrent().Facility.Code,
                    Model_Account.AccountNumber, Model_Account.Patient.MedicalRecordNumber );

                if ( dvt != null )
                {
                    rc = true;
                    Model_Account.Guarantor.DataValidationTicket = dvt;
                }

                StartResponseWaitTimer();
            }
            catch ( Exception ex )
            {
                btnInitiate.Enabled = true;
                ToggleEnabled( true );

                if ( ex.ToString().Contains( UIErrorMessages.EDV_UNAVAILABLE_ERROR ) )
                {
                    detail = UIErrorMessages.EDV_UNAVAILABLE_MESSAGE;
                    return rc;
                }

                string error = ex.Message;
                detail = error;
                if ( ex.InnerException != null && ex.InnerException.Message.IndexOf( "com.psc.edv" ) != -1 )
                {
                    detail = ex.InnerException.Message.Substring( 0, ex.InnerException.Message.IndexOf( "com.psc.edv" ) );
                }
                else if ( ex.Message.IndexOf( "com.psc.edv" ) != -1 )
                {
                    detail = ex.Message.Substring( 0, ex.Message.IndexOf( "com.psc.edv" ) );
                }
            }

            return rc;
        }

        private static DateTime GetCurrentFacilityDateTime( int gmtOffset, int dstOffset )
        {
            ITimeBroker timeBroker = ProxyFactory.GetTimeBroker();
            return timeBroker.TimeAt( gmtOffset, dstOffset );
        }

        /// <summary>
        /// StartResponseWaitTimer - start the timer to wait for the response
        /// </summary>
        private void StartResponseWaitTimer()
        {
            lblValidationStatus.Text = IN_PROGRESS;

            DateTime currentFacilityDateTime = GetCurrentFacilityDateTime( User.GetCurrent().Facility.GMTOffset,
                                                                     User.GetCurrent().Facility.DSTOffset );

            if ( Model_Account.Guarantor.DataValidationTicket.InitiatedOn == DateTime.MinValue )
            {
                Model_Account.Guarantor.DataValidationTicket.InitiatedOn = currentFacilityDateTime;
            }

            int duration;

            if ( Model_Account.Guarantor.DataValidationTicket != null
                && Model_Account.Guarantor.DataValidationTicket.InitiatedOn != DateTime.MinValue
                && Model_Account.Guarantor.DataValidationTicket.InitiatedOn.AddMinutes( 5 ) > currentFacilityDateTime )
            {

                TimeSpan ts = currentFacilityDateTime.Subtract( Model_Account.Guarantor.DataValidationTicket.InitiatedOn );
                duration = RESPONSE_WAIT_DURATION - Convert.ToInt32( ts.TotalMilliseconds );
            }
            else
            {
                duration = 0;
            }

            if ( duration > 0 )
            {
                i_ResponseWaitTimer.Start();
                i_ResponseWaitTimer.Enabled = true;
                i_ResponseWaitTimer.Interval = duration;
                i_ResponseWaitTimer.Tick += responseWaitTimer_Tick;

                StartResponsePollTimer();
            }
            else
            {
                lblValidationStatus.Text = NO_RESPONSE;
                ToggleEnabled( true );
            }
        }

        /// <summary>
        /// StartResponsePollTimer - start the timer to poll every few (currently 10) seconds
        /// </summary>
        private void StartResponsePollTimer()
        {
            lblValidationStatus.Text = IN_PROGRESS;

            i_ResponsePollTimer.Start();
            i_ResponsePollTimer.Enabled = true;
            i_ResponsePollTimer.Interval = RESPONSE_POLL_INTERVAL;
            i_ResponsePollTimer.Tick += responsePollTimer_Tick;
        }

        /// <summary>
        /// responsePollTimer_Tick - handler for ResponsePollTimer expired event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void responsePollTimer_Tick( object sender, EventArgs e )
        {
            // see if we've got a response
            try
            {
                CreditValidationResponse response = null;

                if ( Model_Account != null
                    && Model_Account.Guarantor != null )
                {
                    response = GetValidationResponse();
                }

                if ( response != null
                    && response.ReturnedDataValidationTicket.ResultsAvailable )
                {
                    stopResponsePollTimer();
                    stopResponseWaitTimer();

                    if ( response.ResponseCreditReport == null )
                    {
                        btnInitiate.Enabled = true;
                        ToggleEnabled( true );

                        lblValidationStatus.Text = NO_RESULTS;
                    }
                    else
                    {
                        if ( Model_Account != null
                            && Model_Account.Guarantor != null )
                        {
                            Model_Account.Guarantor.CreditReport = response.ResponseCreditReport;

                            if ( !Model_Account.Guarantor.DataValidationTicket.ResultsReviewed )
                            {
                                lblValidationStatus.Text = RESULTS_AVAILABLE;
                                btnView.Enabled = true;
                                btnView.Focus();
                                Model_Account.Guarantor.DataValidationTicket.ResultsAvailable = true;
                            }
                        }
                    }
                }
            }
            catch ( Exception )
            {
                stopResponsePollTimer();
                stopResponseWaitTimer();

                btnInitiate.Enabled = true;
                ToggleEnabled( true );
                lblValidationStatus.Text = SERVICE_UNAVAILABLE;
            }
        }

        /// <summary>
        /// responseWaitTimer_Tick - handler for ResponseWaitTimer expired event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void responseWaitTimer_Tick( object sender, EventArgs e )
        {
            // no response for the wait interval (up to 5 minutes)

            stopResponsePollTimer();
            stopResponseWaitTimer();

            RunGuarantorValidationRule();

            lblValidationStatus.Text = NO_RESPONSE;
            ToggleEnabled( true );
            btnInitiate.Enabled = true;
            btnInitiate.Focus();
        }

        /// <summary>
        /// stopResponseWaitTimer - stop the wait timer
        /// </summary>
        private void stopResponseWaitTimer()
        {
            if ( i_ResponseWaitTimer != null )
            {
                i_ResponseWaitTimer.Stop();
                i_ResponseWaitTimer.Enabled = false;
            }
        }

        /// <summary>
        /// stopResponsePollTimer - stop the polling timer
        /// </summary>
        private void stopResponsePollTimer()
        {
            if ( i_ResponsePollTimer != null )
            {
                i_ResponsePollTimer.Stop();
                i_ResponsePollTimer.Enabled = false;
            }
        }

        #endregion

        /// <summary>
        /// PopulateSsnStatusControl - build out the SSN Status dropdown
        /// </summary>
        private void RunRules()
        {

            UIColors.SetNormalBgColor( employmentView.ComboBox );
            UIColors.SetNormalBgColor( (Control)employmentView.EmployerField );

            UIColors.SetNormalBgColor( mtbLastName );
            UIColors.SetNormalBgColor( mtbFirstName );
            UIColors.SetNormalBgColor( genderControl.ComboBox );
            UIColors.SetNormalBgColor( mtbDLNumber );
            UIColors.SetNormalBgColor( cmbDLState );
            UIColors.SetNormalBgColor( relationshipView.ComboBox );

            RunGuarantorValidationRule();

            //---------the next Required Rules are part of OnGuarantorForm Rule that is Evaluted below:--------------

            RuleEngine.GetInstance().EvaluateRule( typeof( PersonPhoneNumberPreferred ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonPhoneAreaCodePreferred ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( GuarantorEmploymentPhoneNumberPreferred ), Model_Account , employmentView.PhoneNumberControl );
            RuleEngine.GetInstance().EvaluateRule( typeof( GuarantorEmploymentPhoneAreaCodePreferred ), Model_Account , employmentView.PhoneNumberControl );
            RuleEngine.GetInstance().EvaluateRule( typeof( GuarantorConsentRequired ), Model_Account);           
            RuleEngine.GetInstance().OneShotRuleEvaluation<GuarantorConsentPreferred>(Model_Account.Guarantor, addressView.GuarantorConsentPreferredEventHandler);

            // it is imperative that these two rules are run in this order
            RuleEngine.GetInstance().EvaluateRule( typeof( GuarantorEmployerAddressPreferred ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonEmployerRequired ), Model_Account.Guarantor, employmentView.EmployerField );
            EmailAddressPresenter.RunGurantorEmailAddressRules();

            //run Required Rules
            RuleEngine.GetInstance().EvaluateRule( typeof( OnGuarantorForm ), Model_Account.Guarantor );

            //run Invalid Rules
            RuleEngine.GetInstance().EvaluateRule( typeof( OnGuarantorForm ), Model_Account );

        }

        private void RunGuarantorValidationRule()
        {
            if ( lblValidationStatus.Text.Trim() == string.Empty
                || lblValidationStatus.Text == NEEDED
                || lblValidationStatus.Text == NO_RESPONSE
                || lblValidationStatus.Text == RESULTS_REVIEWED
                || lblValidationStatus.Text == VALIDATION_PAUSED
                || lblValidationStatus.Text == SERVICE_UNAVAILABLE
                || lblValidationStatus.Text == MISSING_FIELDS )
            {
                if ( FinancialClassWarrantsValidation() )
                {
                    if ( lblValidationStatus.Text == string.Empty )
                    {
                        lblValidationStatus.Text = NEEDED;
                    }

                    btnInitiate.Enabled = true;
                    if ( RuleEngine.GetInstance().EvaluateRule( typeof( GuarantorValidationFieldsProvided ), Model_Account.Guarantor ) )
                    {
                        if ( lblValidationStatus.Text == MISSING_FIELDS )
                        {
                            lblValidationStatus.Text = NEEDED;
                        }
                    }
                }
                else
                {
                    btnInitiate.Enabled = false;
                }
            }
        }

        private void RegisterEventHandlers()
        {
            if ( !i_Registered && Model_Account !=null  )
            {
                i_Registered = true;

                RuleEngine.GetInstance().RegisterEvent( typeof( GuarantorValidationFieldsProvided ), Model_Account.Guarantor, new EventHandler( GuarantorValidationFieldsProvidedEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonRelationshipRequired ), Model_Account.Guarantor, new EventHandler( PersonRelationshipRequiredEventHandler ), typeof( OnGuarantorForm ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonLastNameRequired ), Model_Account.Guarantor, new EventHandler( PersonLastNameRequiredEventHandler ), typeof( OnGuarantorForm ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonFirstNameRequired ), Model_Account.Guarantor, new EventHandler( PersonFirstNameRequiredEventHandler ), typeof( OnGuarantorForm ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonDriversLicenseStatePreferred ), Model_Account.Guarantor, new EventHandler( PersonDriversLicenseStatePreferredEventHandler ), typeof( OnGuarantorForm ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( GuarantorAddressRequired ), Model_Account.Guarantor, new EventHandler( addressView.AddressRequiredEventHandler ), typeof( OnGuarantorForm ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( GuarantorEmployerAddressPreferred ), Model_Account, new EventHandler( GuarantorEmployerAddressPreferredEventHandler ) );

                RuleEngine.GetInstance().RegisterEvent( typeof( PersonRelationshipPreferred ), Model_Account.Guarantor, new EventHandler( PersonRelationshipPreferredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonLastNamePreferred ), Model_Account.Guarantor, new EventHandler( PersonLastNamePreferredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonFirstNamePreferred ), Model_Account.Guarantor, new EventHandler( PersonFirstNamePreferredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonGenderPreferred ), Model_Account.Guarantor, new EventHandler( PersonGenderPreferredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonDriversLicensePreferred ), Model_Account.Guarantor, new EventHandler( PersonDriversLicensePreferredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( GuarantorAddressPreferred ), Model_Account.Guarantor, new EventHandler( addressView.AddressPreferredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonPhoneAreaCodePreferred ), Model_Account.Guarantor, new EventHandler( addressView.AreaCodePreferredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonPhoneNumberPreferred ), Model_Account.Guarantor, new EventHandler( addressView.PhonePreferredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( GuarantorEmploymentStatusPreferred ), Model_Account.Guarantor, new EventHandler( GuarantorEmploymentStatusPreferredEventHandler ), typeof( OnGuarantorForm ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonEmployerRequired ), Model_Account.Guarantor, new EventHandler( PersonEmployerRequiredEventHandler ), typeof( OnGuarantorForm ) );
                RuleEngine.GetInstance().RegisterEvent(typeof(GuarantorEmploymentPhoneAreaCodePreferred), Model_Account, employmentView.PhoneNumberControl, GuarantorEmploymentPhoneAreaCodePreferredEvent, typeof(OnGuarantorForm));
                RuleEngine.GetInstance().RegisterEvent(typeof(GuarantorEmploymentPhoneNumberPreferred), Model_Account, employmentView.PhoneNumberControl, GuarantorEmploymentPhoneNumberPreferredEvent, typeof(OnGuarantorForm));
                RuleEngine.GetInstance().RegisterEvent(typeof(GuarantorConsentRequired), Model_Account, addressView.GuarantorConsentRequiredEventHandler);
                RuleEngine.GetInstance().RegisterEvent(typeof(GuarantorConsentPreferred), Model_Account, addressView.GuarantorConsentPreferredEventHandler);

                RuleEngine.GetInstance().RegisterEvent( typeof( InvalidRelForGuarantor ), Model_Account, new EventHandler( InvalidRelForGuarantorEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( InvalidRelForGuarantorChange ), Model_Account, new EventHandler( InvalidRelForGuarantorChangeEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( InvalidGenderForGuarantor ), Model_Account, new EventHandler( InvalidGenderForGuarantorEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( InvalidGenderForGuarantorChange ), Model_Account, new EventHandler( InvalidGenderForGuarantorChangeEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( InvalidEmpStatusForGuarantor ), Model_Account, new EventHandler( InvalidEmpStatusForGuarantorEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( InvalidEmpStatusForGuarantorChange ), Model_Account, new EventHandler( InvalidEmpStatusForGuarantorChangeEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( GuarantorEmailAddressPreferred), Model_Account, addressView.GuarantorEmailAddressPreferredEventhandler);
             }
        }

        private void UnregisterEventHandlers()
        {
            i_Registered = false;

            if ( Model_Account == null )
            {
                Model_Account = new Account();
            }

            RuleEngine.GetInstance().UnregisterEvent( typeof( GuarantorValidationFieldsProvided ), Model_Account.Guarantor, GuarantorValidationFieldsProvidedEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonRelationshipRequired ), Model_Account.Guarantor, PersonRelationshipRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonLastNameRequired ), Model_Account.Guarantor, PersonLastNameRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonFirstNameRequired ), Model_Account.Guarantor, PersonFirstNameRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonDriversLicenseStatePreferred ), Model_Account.Guarantor, PersonDriversLicenseStatePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( GuarantorAddressRequired ), Model_Account.Guarantor, addressView.AddressRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonEmployerRequired ), Model_Account.Guarantor, PersonEmployerRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( GuarantorEmployerAddressPreferred ), Model_Account, GuarantorEmployerAddressPreferredEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonRelationshipPreferred ), Model_Account.Guarantor, PersonRelationshipPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonLastNamePreferred ), Model_Account.Guarantor, PersonLastNamePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonFirstNamePreferred ), Model_Account.Guarantor, PersonFirstNamePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonGenderPreferred ), Model_Account.Guarantor, PersonGenderPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonDriversLicensePreferred ), Model_Account.Guarantor, PersonDriversLicensePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( GuarantorAddressPreferred ), Model_Account.Guarantor, addressView.AddressPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonPhoneNumberPreferred ), Model_Account.Guarantor, addressView.PhonePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonPhoneAreaCodePreferred ), Model_Account.Guarantor, addressView.PhonePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( GuarantorEmploymentStatusPreferred ), Model_Account.Guarantor, GuarantorEmploymentStatusPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent(typeof(GuarantorEmploymentPhoneNumberPreferred), Model_Account.Guarantor.Employment.Employer, GuarantorEmploymentPhoneNumberPreferredEvent);
            RuleEngine.GetInstance().UnregisterEvent(typeof(GuarantorEmploymentPhoneAreaCodePreferred), Model_Account.Guarantor.Employment.Employer, GuarantorEmploymentPhoneAreaCodePreferredEvent);
            RuleEngine.GetInstance().UnregisterEvent(typeof(GuarantorConsentRequired),Model_Account, addressView.GuarantorConsentRequiredEventHandler);
            RuleEngine.GetInstance().UnregisterEvent( typeof( GuarantorConsentPreferred), Model_Account, addressView.GuarantorConsentPreferredEventHandler);

            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidRelForGuarantor ), Model_Account, InvalidRelForGuarantorEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidRelForGuarantorChange ), Model_Account, InvalidRelForGuarantorChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidGenderForGuarantor ), Model_Account, InvalidGenderForGuarantorEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidGenderForGuarantorChange ), Model_Account, InvalidGenderForGuarantorChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidEmpStatusForGuarantor ), Model_Account, InvalidEmpStatusForGuarantorEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidEmpStatusForGuarantorChange ), Model_Account, InvalidEmpStatusForGuarantorChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( GuarantorEmailAddressPreferred), Model_Account, addressView.GuarantorEmailAddressPreferredEventhandler);

        }


        private void InitializeDLStatesComboBox()
        {
            if ( IsInRuntimeMode )
            {
                if ( cmbDLState.Items.Count == 0 )
                {
                    IAddressBroker broker = BrokerFactory.BrokerOfType<IAddressBroker>();
                    if ( cmbDLState.Items.Count == 0 )
                    {
                        StateComboHelper.PopulateWithCollection(broker.AllStates(User.GetCurrent().Facility.Oid));
                    }
                    //CR0351 - State combo box is always enabled.

                }
            }
        }

        private void ResetView( bool resetCopyParty )
        {
            addressView.ResetView();

            if ( resetCopyParty )
            {
                copyPartyView.ResetView();
                employmentView.ResetView();
            }

            genderControl.ResetView();

            relationshipView.ResetView();

            mtbLastName.UnMaskedText = String.Empty;

            mtbFirstName.UnMaskedText = String.Empty;
            mtbDob.UnMaskedText = String.Empty;
            mtbMI.UnMaskedText = String.Empty;
            suffixPresenter.ClearSuffix();
            Model_Account.Guarantor.DateOfBirth = DateTime.MinValue;

            mtbDLNumber.UnMaskedText = String.Empty;
            cmbDLState.SelectedItem = new State();
            Model_Account.Guarantor.DriversLicense.State = new State();

            SetDriversLicense();

            Model_Account.Guarantor.SocialSecurityNumber = new SocialSecurityNumber();
            ssnView.UpdateDefaultSocialSecurityNumberForAdmitDate();

            UpdateModel();
        }

        private void SetDriversLicense()
        {
            // Update the Driver's License 
            driversLicenseState = Model_Account.Guarantor.DriversLicense.State;

            if ( driversLicenseState != null
                && cmbDLState.SelectedIndex <= 0 )
            {
                driversLicenseStateSelectedIndex = cmbDLState.FindString( driversLicenseState.ToString() );
                if ( driversLicenseStateSelectedIndex != -1 )
                {
                    cmbDLState.SelectedIndex = driversLicenseStateSelectedIndex;
                }
            }
        }

        private void btnInitiate_Click( object sender, EventArgs e )
        {
            btnInitiate.Enabled = false;
            ToggleEnabled( false );

            bool rc = InitiateCreditValidation();

            if ( !rc )
            {
                lblValidationStatus.Text = detail;
            }

            btnView.Focus();
        }

        private void btnView_Click( object sender, EventArgs e )
        {
            FormValidationResults fvr = new FormValidationResults();
            fvr.Model_Guarantor = Model_Account.Guarantor;
            fvr.UpdateView();

            try
            {
                fvr.ShowDialog( this );

                if ( fvr.DialogResult == DialogResult.OK )
                {
                    Model_Account.Guarantor.Name = fvr.Model_Guarantor.Name;
                    Model_Account.Guarantor.SocialSecurityNumber = fvr.Model_Guarantor.SocialSecurityNumber;
                    UpdateView( false );
                    ToggleEnabled( true );
                    btnInitiate.Enabled = true;
                    btnView.Enabled = false;

                    lblValidationStatus.Text = RESULTS_REVIEWED;
                    Model_Account.Guarantor.DataValidationTicket.ResultsReviewed = true;
                }
            }
            finally
            {
                fvr.Dispose();
            }
        }

        private void ToggleEnabled( bool inBool )
        {
            mtbLastName.Enabled = inBool;
            mtbFirstName.Enabled = inBool;
            mtbMI.Enabled = inBool;
            addressView.Enabled = inBool;
            ssnView.Enabled = inBool;
            btnClearAll.Enabled = inBool;
            copyPartyView.Enabled = inBool;
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureFirstNameAndLastName( mtbFirstName );
            MaskedEditTextBoxBuilder.ConfigureFirstNameAndLastName( mtbLastName );
            MaskedEditTextBoxBuilder.ConfigureMIAndSuffix( mtbMI );
            
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnView = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnInitiate = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblStatus = new System.Windows.Forms.Label();
            this.mtbDLNumber = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblState = new System.Windows.Forms.Label();
            this.cmbDLState = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblGender = new System.Windows.Forms.Label();
            this.mtbMI = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbFirstName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbLastName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblMI = new System.Windows.Forms.Label();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.lblLastName = new System.Windows.Forms.Label(); 
            this.gbName = new System.Windows.Forms.GroupBox();
            this.lblNumber = new System.Windows.Forms.Label();
            this.gbDriversLicnese = new System.Windows.Forms.GroupBox();
            this.gbIdentity = new System.Windows.Forms.GroupBox();
            this.lblValidationStatus = new System.Windows.Forms.Label();
            this.relationshipView = new PatientAccess.UI.CommonControls.RelationshipView();
            this.genderControl = new PatientAccess.UI.CommonControls.GenderControl();
            this.lblStaticCopyFrom = new System.Windows.Forms.Label();
            this.copyPartyView = new PatientAccess.UI.GuarantorViews.CopyPartyView();
            this.employmentView = new PatientAccess.UI.GuarantorViews.EmploymentView();
            this.addressView = new PatientAccess.UI.AddressViews.AddressView();
            this.ssnView = new PatientAccess.UI.CommonControls.SSNControl();
            this.btnClearAll = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblStaticDateOfBirth = new System.Windows.Forms.Label();
            this.mtbDob = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.suffixView = new SuffixView();
            this.panelGuarantorView = new System.Windows.Forms.Panel();
            this.gbIdentity.SuspendLayout();
            this.panelGuarantorView.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnView
            // 
            this.btnView.Enabled = false;
            this.btnView.Location = new System.Drawing.Point( 133, 150 );
            this.btnView.Message = null;
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size( 72, 23 );
            this.btnView.TabIndex = 18;
            this.btnView.Text = "Re&view";
            this.btnView.Click += new System.EventHandler( this.btnView_Click );
            // 
            // btnInitiate
            // 
            this.btnInitiate.Enabled = false;
            this.btnInitiate.Location = new System.Drawing.Point( 13, 150 );
            this.btnInitiate.Message = null;
            this.btnInitiate.Name = "btnInitiate";
            this.btnInitiate.Size = new System.Drawing.Size( 102, 23 );
            this.btnInitiate.TabIndex = 17;
            this.btnInitiate.Text = "Ini&tiate Validation";
            this.btnInitiate.Click += new System.EventHandler( this.btnInitiate_Click );
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point( 26, 305 );
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size( 40, 13 );
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Status:";
            // 
            // mtbDLNumber
            // 
            this.mtbDLNumber.BackColor = System.Drawing.Color.White;
            this.mtbDLNumber.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbDLNumber.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbDLNumber.KeyPressExpression = "^[a-zA-Z0-9]*";
            this.mtbDLNumber.Location = new System.Drawing.Point( 775, 62 );
            this.mtbDLNumber.Mask = "";
            this.mtbDLNumber.MaxLength = 15;
            this.mtbDLNumber.Name = "mtbDLNumber";
            this.mtbDLNumber.Size = new System.Drawing.Size( 96, 20 );
            this.mtbDLNumber.TabIndex = 9;
            this.mtbDLNumber.ValidationExpression = "^[a-zA-Z0-9]*";
            this.mtbDLNumber.Validating += new System.ComponentModel.CancelEventHandler( this.mtbDLNumber_Validating );
            // 
            // lblState
            // 
            this.lblState.Location = new System.Drawing.Point( 730, 92 );
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size( 34, 13 );
            this.lblState.TabIndex = 0;
            this.lblState.Text = "State:";
            // 
            // cmbDLState
            // 
            this.cmbDLState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDLState.Location = new System.Drawing.Point( 775, 88 );
            this.cmbDLState.Name = "cmbDLState";
            this.cmbDLState.Size = new System.Drawing.Size( 168, 21 );
            this.cmbDLState.TabIndex = 9;
            this.cmbDLState.Validating += new System.ComponentModel.CancelEventHandler( this.cmbDLState_Validating );
            this.cmbDLState.SelectedIndexChanged += new System.EventHandler( this.DriversLicenseState_SelectedIndexChanged );
            // 
            // lblGender
            // 
            this.lblGender.Location = new System.Drawing.Point( 680, 15 );
            this.lblGender.Name = "lblGender";
            this.lblGender.Size = new System.Drawing.Size( 45, 14 );
            this.lblGender.TabIndex = 0;
            this.lblGender.Text = "Gender:";
            // 
            // mtbMI
            // 
            this.mtbMI.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbMI.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbMI.Location = new System.Drawing.Point( 532, 62 );
            this.mtbMI.Mask = "";
            this.mtbMI.MaxLength = 1;
            this.mtbMI.Name = "mtbMI";
            this.mtbMI.Size = new System.Drawing.Size( 18, 20 );
            this.mtbMI.TabIndex = 5;
            this.mtbMI.Validating += new System.ComponentModel.CancelEventHandler( this.mtbMI_Validating );
            // 
            // mtbFirstName
            // 
            this.mtbFirstName.BackColor = System.Drawing.Color.White;
            this.mtbFirstName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbFirstName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbFirstName.ForeColor = System.Drawing.SystemColors.WindowText;

            this.mtbFirstName.Location = new System.Drawing.Point( 341, 62 );
            this.mtbFirstName.Mask = "";
            this.mtbFirstName.MaxLength = 13;
            this.mtbFirstName.Name = "mtbFirstName";
            this.mtbFirstName.Size = new System.Drawing.Size( 162, 20 );
            this.mtbFirstName.TabIndex = 4;
            this.mtbFirstName.Validating += new System.ComponentModel.CancelEventHandler( this.mtbFirstName_Validating );
            // 
            // mtbLastName
            // 
            this.mtbLastName.BackColor = System.Drawing.Color.White;
            this.mtbLastName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbLastName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbLastName.ForeColor = System.Drawing.SystemColors.WindowText;

            this.mtbLastName.Location = new System.Drawing.Point( 42, 62 );
            this.mtbLastName.Mask = "";
            this.mtbLastName.MaxLength = 25;
            this.mtbLastName.Name = "mtbLastName";
            this.mtbLastName.Size = new System.Drawing.Size( 257, 20 );
            this.mtbLastName.TabIndex = 3;

            this.mtbLastName.Validating += new System.ComponentModel.CancelEventHandler( this.mtbLastName_Validating );
            // 
            // lblMI
            // 
            this.lblMI.Location = new System.Drawing.Point( 511, 65 );
            this.lblMI.Name = "lblMI";
            this.lblMI.Size = new System.Drawing.Size( 21, 11 );
            this.lblMI.TabIndex = 0;
            this.lblMI.Text = "MI:";
            // 
            // lblFirstName
            // 
            this.lblFirstName.Location = new System.Drawing.Point( 312, 65 );
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size( 32, 12 );
            this.lblFirstName.TabIndex = 0;
            this.lblFirstName.Text = "First:";
            // 
            // lblLastName
            // 
            this.lblLastName.Location = new System.Drawing.Point( 13, 65 );
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size( 29, 16 );
            this.lblLastName.TabIndex = 0;
            this.lblLastName.Text = "Last:";
            // 
            // Suffix View
            // 
            this.suffixView.Location = new System.Drawing.Point(575, 59);
            this.suffixView.Name = "Suffix View";
            this.suffixView.Size = new System.Drawing.Size(100, 27);
            this.suffixView.TabIndex = 6;
            this.suffixView.Visible = true;
            // 
            // gbName
            // 
            this.gbName.Location = new System.Drawing.Point( 8, 40 );
            this.gbName.Name = "gbName";
            this.gbName.Size = new System.Drawing.Size( 670, 55 );
            this.gbName.TabIndex = 0;
            this.gbName.TabStop = false;
            this.gbName.Text = "Name";
            // 
            // lblNumber
            // 
            this.lblNumber.Location = new System.Drawing.Point( 730, 65 );
            this.lblNumber.Name = "lblNumber";
            this.lblNumber.Size = new System.Drawing.Size( 48, 13 );
            this.lblNumber.TabIndex = 0;
            this.lblNumber.Text = "Number:";
            // 
            // gbDriversLicnese
            // 
            this.gbDriversLicnese.Location = new System.Drawing.Point( 722, 44 );
            this.gbDriversLicnese.Name = "gbDriversLicnese";
            this.gbDriversLicnese.Size = new System.Drawing.Size( 229, 74 );
            this.gbDriversLicnese.TabIndex = 0;
            this.gbDriversLicnese.TabStop = false;
            this.gbDriversLicnese.Text = "U.S. driver\'s license";
            // 
            // gbIdentity
            // 
            this.gbIdentity.Controls.Add( this.lblValidationStatus );
            this.gbIdentity.Controls.Add( this.btnInitiate );
            this.gbIdentity.Controls.Add( this.btnView );
            this.gbIdentity.Location = new System.Drawing.Point( 325, 185 );
            this.gbIdentity.Name = "gbIdentity";
            this.gbIdentity.Size = new System.Drawing.Size( 279, 187 );
            this.gbIdentity.TabIndex = 17;
            this.gbIdentity.TabStop = false;
            this.gbIdentity.Text = "Identity and credit validation";
            // 
            // lblValidationStatus
            // 
            this.lblValidationStatus.Location = new System.Drawing.Point( 12, 29 );
            this.lblValidationStatus.Name = "lblValidationStatus";
            this.lblValidationStatus.Size = new System.Drawing.Size( 253, 100 );
            this.lblValidationStatus.TabIndex = 0;
            // 
            // relationshipView
            // 
            this.relationshipView.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.relationshipView.LabelText = "";
            this.relationshipView.Location = new System.Drawing.Point( 315, 10 );
            this.relationshipView.Model = null;
            this.relationshipView.Name = "relationshipView";
            this.relationshipView.PartyForRelationships = null;
            this.relationshipView.PatientIs = null;
            this.relationshipView.RelationshipName = null;
            this.relationshipView.Size = new System.Drawing.Size( 345, 24 );
            this.relationshipView.TabIndex = 2;
            this.relationshipView.RelationshipSelected += new System.EventHandler( this.RelationshipSelectedEvent );
            this.relationshipView.RelationshipValidating += new System.ComponentModel.CancelEventHandler( this.relationshipView_RelationshipValidating );
            // 
            // genderControl
            // 
            this.genderControl.BackColor = System.Drawing.Color.White;
            this.genderControl.Location = new System.Drawing.Point( 725, 11 );
            this.genderControl.Model = null;
            this.genderControl.Name = "genderControl";
            this.genderControl.Size = new System.Drawing.Size( 92, 23 );
            this.genderControl.TabIndex = 7;
            this.genderControl.GenderControlValidating += new System.ComponentModel.CancelEventHandler( this.genderControl_GenderControlValidating );
            this.genderControl.GenderSelectedEvent += new System.EventHandler( this.GenderSelectedEvent );
            // 
            // lblStaticCopyFrom
            // 
            this.lblStaticCopyFrom.Location = new System.Drawing.Point( 10, 14 );
            this.lblStaticCopyFrom.Name = "lblStaticCopyFrom";
            this.lblStaticCopyFrom.Size = new System.Drawing.Size( 127, 23 );
            this.lblStaticCopyFrom.TabIndex = 0;
            this.lblStaticCopyFrom.Text = "Copy to Guarantor from:";
            // 
            // copyPartyView
            // 
            this.copyPartyView.KindOfTargetParty = null;
            this.copyPartyView.Location = new System.Drawing.Point( 132, 13 );
            this.copyPartyView.Model = null;
            this.copyPartyView.Name = "copyPartyView";
            this.copyPartyView.Size = new System.Drawing.Size( 178, 24 );
            this.copyPartyView.TabIndex = 1;
            this.copyPartyView.PartySelected += new System.EventHandler( this.OnPartySelected );
            // 
            // employmentView
            // 
            this.employmentView.Activity = null;
            this.employmentView.BackColor = System.Drawing.Color.White;
            this.employmentView.Location = new System.Drawing.Point( 615, 123 );
            this.employmentView.Model = null;
            this.employmentView.Model_Employment = null;
            this.employmentView.Name = "employmentView";
            this.employmentView.Size = new System.Drawing.Size( 377, 214 );
            this.employmentView.TabIndex = 19;
            this.employmentView.EmploymentViewChangedEvent += new System.EventHandler( this.employmentView_EmploymentViewChangedEvent );
            this.employmentView.EmploymentStatusForGuarantorValidating += new System.ComponentModel.CancelEventHandler( this.employmentView_EmploymentStatusForGuarantorValidating );
            // 
            // addressView
            // 
            this.addressView.Context = null;
            this.addressView.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.addressView.KindOfTargetParty = null;
            this.addressView.Location = new System.Drawing.Point( 8, 103 );
            this.addressView.Mode = PatientAccess.UI.AddressViews.AddressView.AddressMode.PHONECELLCONSENTEMAIL;
            this.addressView.Model = null;
            this.addressView.Name = "addressView";
            this.addressView.PatientAccount = null;
            this.addressView.Size = new System.Drawing.Size( 300, 270 );
            this.addressView.TabIndex = 10;
            this.addressView.IsAddressWithStreet2 = true;
            this.addressView.AddressChanged += new System.EventHandler( this.AddressChangedEventHandler );
            this.addressView.PhoneNumberChanged += new System.EventHandler( this.addressView_PhoneNumberChanged );
            this.employmentView.PhoneNumberControl.PhoneNumberChanged += new System.EventHandler(EmploymentView_PhoneNumberChanged);
            this.employmentView.PhoneNumberControl.AreaCodeChanged += new System.EventHandler(EmploymentView_AreaCodeChanged);
            this.addressView.AreaCodeChanged += new EventHandler( addressView_AreaCodeChanged );
            this.addressView.CellPhoneNumberChanged += new System.EventHandler( this.addressView_CellPhoneNumberChanged );
            this.addressView.CellPhoneConsentChanged += new System.EventHandler( this.addressView_CellPhoneConsentChanged );
            this.addressView.EmailChanged += new System.EventHandler( this.addressView_EmailChanged );
            this.addressView.cellPhoneNumberControl.PhoneNumberTextChanged += new System.EventHandler(this.cellPhoneNumberControl_PhoneNumberTextChanged);
            // 
            // lblStaticDateOfBirth
            // 
            this.lblStaticDateOfBirth.Location = new System.Drawing.Point(840, 15);
            this.lblStaticDateOfBirth.Name = "lblStaticDateOfBirth";
            this.lblStaticDateOfBirth.Size = new System.Drawing.Size(35, 16);
            this.lblStaticDateOfBirth.TabIndex = 0;
            this.lblStaticDateOfBirth.Text = "DOB:";
            // 
            // mtbDob
            // 
            this.mtbDob.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtStart;
            this.mtbDob.KeyPressExpression = "^\\d*$";
            this.mtbDob.Location = new System.Drawing.Point(880, 11);
            this.mtbDob.Mask = "  /  /    ";
            this.mtbDob.MaxLength = 10;
            this.mtbDob.Name = "mtbDob";
            this.mtbDob.Size = new System.Drawing.Size(70, 20);
            this.mtbDob.TabIndex = 8;
            this.mtbDob.ValidationExpression = "^\\d*$";
            this.mtbDob.Validating += new System.ComponentModel.CancelEventHandler(this.mtbDob_Validating); 
            // 
            // ssnView
            // 
            this.ssnView.Location = new System.Drawing.Point( 325, 103 );
            this.ssnView.Name = "ssnView";
            this.ssnView.SsnContext = SsnViewContext.GuarantorView;
            this.ssnView.TabIndex = 15;
            // 
            // btnClearAll
            // 
            this.btnClearAll.Location = new System.Drawing.Point( 881, 345 );
            this.btnClearAll.Message = null;
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size( 72, 23 );
            this.btnClearAll.TabIndex = 23;
            this.btnClearAll.Text = "&Clear All";
            this.btnClearAll.Click += new System.EventHandler( this.btnClearAll_Click );
            // 
            // panelGuarantorView
            // 
            this.panelGuarantorView.Controls.Add( this.btnClearAll );
            this.panelGuarantorView.Controls.Add( this.ssnView );
            this.panelGuarantorView.Controls.Add( this.addressView );
            this.panelGuarantorView.Controls.Add( this.employmentView );
            this.panelGuarantorView.Controls.Add( this.copyPartyView );
            this.panelGuarantorView.Controls.Add( this.genderControl );
            this.panelGuarantorView.Controls.Add( this.lblStaticCopyFrom );
            this.panelGuarantorView.Controls.Add( this.mtbDLNumber );
            this.panelGuarantorView.Controls.Add( this.cmbDLState );
            this.panelGuarantorView.Controls.Add( this.lblStatus );
            this.panelGuarantorView.Controls.Add( this.lblState );
            this.panelGuarantorView.Controls.Add( this.mtbMI );
            this.panelGuarantorView.Controls.Add( this.mtbFirstName );
            this.panelGuarantorView.Controls.Add( this.mtbLastName );
            this.panelGuarantorView.Controls.Add( this.lblGender );
            this.panelGuarantorView.Controls.Add( this.lblMI );
            this.panelGuarantorView.Controls.Add( this.lblFirstName );
            this.panelGuarantorView.Controls.Add(this.suffixView); 
            this.panelGuarantorView.Controls.Add( this.lblLastName );
            this.panelGuarantorView.Controls.Add( this.gbName );
            this.panelGuarantorView.Controls.Add( this.lblNumber );
            this.panelGuarantorView.Controls.Add( this.gbDriversLicnese );
            this.panelGuarantorView.Controls.Add( this.gbIdentity );
            this.panelGuarantorView.Controls.Add( this.relationshipView );
            this.panelGuarantorView.Controls.Add(this.lblStaticDateOfBirth);
            this.panelGuarantorView.Controls.Add(this.mtbDob);
            this.panelGuarantorView.Location = new System.Drawing.Point( 0, 0 );
            this.panelGuarantorView.Name = "panelGuarantorView";
            this.panelGuarantorView.Size = new System.Drawing.Size( 1005, 374 );
            this.panelGuarantorView.TabIndex = 0;
            // 
            // GuarantorView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.panelGuarantorView );
            this.Name = "GuarantorView";
            this.Size = new System.Drawing.Size( 1005, 374 );
            this.Enter += new System.EventHandler( this.GuarantorView_Enter );
            this.Disposed += new System.EventHandler( this.GuarantorView_Disposed );
            this.Leave += new System.EventHandler( this.GuarantorView_Leave );
            this.Validating += new System.ComponentModel.CancelEventHandler( this.GuarantorView_Validating );
            this.gbIdentity.ResumeLayout( false );
            this.panelGuarantorView.ResumeLayout( false );
            this.panelGuarantorView.PerformLayout();
            this.ResumeLayout( false );

        }
 
        private void mtbDob_Validating(object sender, CancelEventArgs e)
        {

            UIColors.SetNormalBgColor(mtbDob);

            if (!GuarantorDateOfBirthPresenter.ValidateDateOfBirth())
            {
                UIColors.SetErrorBgColor(mtbDob);
                mtbDob.Focus();
            }
            
        }


        #endregion

        #endregion

        #region Private Properties
        private ReferenceValueComboBox StateComboHelper
        {
            get
            {
                return i_StateComboHelper;
            }
            set
            {
                i_StateComboHelper = value;
            }
        }
        #endregion

        #region Construction and Finalization
        public GuarantorView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            ConfigureControls();

            ComponentResourceManager resources = new ComponentResourceManager( typeof( GuarantorView ) );
            copyPartyView.CoverageOrder = ( (CoverageOrder)( resources.GetObject( "copyPartyView.CoverageOrder" ) ) );
            addressView.EditAddressButtonText = "Edit &Address...";

            gender = null;
            aGuarantor = null;
            selectedRelationshipType = null;
            driversLicenseState = null;
            relationShip = null;
            genderControlSelectedIndex = -1;
            driversLicenseStateSelectedIndex = -1;
        }

        protected override void Dispose( bool disposing )
        {
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

        private LoggingButton btnClearAll;
        private LoggingButton btnView;
        private LoggingButton btnInitiate;

        private PatientAccessComboBox cmbDLState;
        private SuffixView suffixView;

        private Label lblStatus;
        private Label lblState;
        private Label lblGender;
        private Label lblMI;
        private Label lblFirstName;
        private Label lblLastName;
        private Label lblNumber;
        private Label lblStaticCopyFrom;
        private Label lblValidationStatus;

        private MaskedEditTextBox mtbDLNumber;
        private MaskedEditTextBox mtbMI;
        private MaskedEditTextBox mtbFirstName;
        private MaskedEditTextBox mtbLastName;

        private GroupBox gbName;
        private GroupBox gbDriversLicnese;
        private GroupBox gbIdentity;

        private AddressView addressView;
        private EmploymentView employmentView;
        private RelationshipView relationshipView;
        private GenderControl genderControl;
        private CopyPartyView copyPartyView;
        private SSNControl ssnView;
        private ReferenceValueComboBox i_StateComboHelper;
        private Label lblStaticDateOfBirth;
        private MaskedEditTextBox mtbDob;

        private Gender gender;
        private Guarantor aGuarantor;
        private State driversLicenseState;
        private Relationship relationShip;
        private RelationshipType selectedRelationshipType;
        private int genderControlSelectedIndex;
        private int driversLicenseStateSelectedIndex;
        private bool i_Registered;
        private Relationship i_OriginalRelationship;

        private readonly Timer i_ResponseWaitTimer = new Timer();
        private readonly Timer i_ResponsePollTimer = new Timer();
        private DateTime dobDate;

        private bool blnLeaveRun;
        private string detail = string.Empty;
        private bool loadingModelData = true;
        private EmailAddressPresenter EmailAddressPresenter { get; set; }

        #endregion

        #region Constants

        private const string
            IN_PROGRESS = "Initiated, in progress.",
            NO_RESPONSE = "Initiated, no response.",
            RESULTS_AVAILABLE = "Results awaiting review.",
            RESULTS_REVIEWED = "Results reviewed.",
            NOT_NEEDED = "No validation needed.",
            NEEDED = "Validation needed.",
            NO_RESULTS = "No results available.",
            MISSING_FIELDS = "Missing data for validation. Last name, First name, and Address fields are required.",
            VALIDATION_PAUSED = "Validation paused due to missing or invalid information. Either make changes to the information you provided or include additional information such as SSN, driver's license, or contact information",
            SERVICE_UNAVAILABLE = "System electronic verification is unavailable. Please try again later.";

        private const string
            SELF = "Self",
            EMPLOYEE = "Employee",
            NATURAL_CHILD = "Natural Child";

        // 5 minutes (in milliseconds)
        private const int RESPONSE_WAIT_DURATION = 300000;
        private Panel panelGuarantorView;
        private SuffixPresenter suffixPresenter;

        // 5 seconds (in milliseconds)
        private const int RESPONSE_POLL_INTERVAL = 5000;

        #endregion
    }
}
