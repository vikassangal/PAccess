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
using PatientAccess.UI.CommonControls.Email.Presenters;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.CommonControls.Suffix.Presenters;
using PatientAccess.UI.CommonControls.Suffix.ViewImpl;
using PatientAccess.UI.Factories;
using PatientAccess.UI.GuarantorViews;
using PatientAccess.UI.GuarantorViews.Presenters;
using PatientAccess.UI.GuarantorViews.Views;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.ShortRegistration.GuarantorViews
{
    /// <summary>
    /// Summary description for ShortGuarantorView.
    /// </summary>
    [Serializable]
    public class ShortGuarantorView : ControlView, IGuarantorDateOfBirthView
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

        private void PersonDriversLicenseStateRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbDLState );
            Refresh();
        }
        private void PersonRelationshipPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( relationshipView.ComboBox );
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

        //--------------------- InvalidValues in Comboboxes Event Handlers: ------------------------------------

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
            RuleEngine.GetInstance().EvaluateRule( typeof( OnShortGuarantorForm ), Model_Account );
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

        private void AddressChangedEventHandler( object sender, EventArgs e )
        {
            ContactPoint mailingContactPoint = Model_Account.Guarantor.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() );
            mailingContactPoint.Address = addressView.Model_ContactPoint.Address;

            RuleEngine.GetInstance().EvaluateRule( typeof( GuarantorAddressRequired ), Model_Account.Guarantor );
          
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
                GuarantorDateOfBirthPresenter.HandleGuarantorDateOfBirth();
            }

            RunRules();
        }

        private void RelationshipSelectedEvent( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( relationshipView.ComboBox );
            UIColors.SetNormalBgColor( mtbFirstName );
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
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonPhoneAreaCodeRequired ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonPhoneNumberRequired ), Model_Account.Guarantor );
			RuleEngine.GetInstance().EvaluateRule( typeof( OnShortGuarantorForm ), Model_Account );
        }

        private void addressView_PhoneNumberChanged( object sender, EventArgs e )
        {
            ContactPoint mailingContactPoint = Model_Account.Guarantor.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() );
            mailingContactPoint.PhoneNumber = addressView.Model_ContactPoint.PhoneNumber;

            RuleEngine.GetInstance().EvaluateRule( typeof( PersonPhoneAreaCodePreferred ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonPhoneNumberPreferred ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonPhoneAreaCodeRequired ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonPhoneNumberRequired ), Model_Account.Guarantor );
			RuleEngine.GetInstance().EvaluateRule( typeof( OnShortGuarantorForm ), Model_Account );
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
            EmailAddressPresenter.UpdateGuarantorEmailAddress();
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
            UIColors.SetNormalBgColor( cmbDLState );
            Model_Account.Guarantor.DriversLicense.Number = mtbDLNumber.Text;

            SetDriversLicense();

            RuleEngine.GetInstance().EvaluateRule( typeof( PersonDriversLicensePreferred), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonDriversLicenseStateRequired ), Model_Account.Guarantor );

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
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonDriversLicenseStateRequired ), Model_Account.Guarantor );
        }

        private void cmbDLState_Validating( object sender, CancelEventArgs e )
        {
            if ( cmbDLState.SelectedItem != null )
            {
                Model_Account.Guarantor.DriversLicense.State = (State)cmbDLState.SelectedItem;
            }

            UIColors.SetNormalBgColor( cmbDLState );

            RuleEngine.GetInstance().EvaluateRule( typeof( PersonDriversLicenseStatePreferred ), Model_Account );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonDriversLicenseStateRequired ), Model_Account.Guarantor );
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
            ssnView.SsnFactory = new SsnFactoryCreator(Model_Account).GetSsnFactory();
            EmailAddressPresenter = new EmailAddressPresenter(addressView, Model_Account, RuleEngine.GetInstance() );
            GuarantorDateOfBirthPresenter = new GuarantorDateOfBirthPresenter(this, Model_Account, RuleEngine.GetInstance(), new MessageBoxAdapter());
            suffixPresenter = new SuffixPresenter(suffixView, Model_Account, "Guarantor");
            UpdateView( true );

            if ( loadingModelData )
            {
                // During initial load of account, validate incoming Guarantor Social Security 
                // Number for the Status and update SSN control with the validated SSN and Status.
                ssnView.UpdateView();
            }
            copyPartyView.ComboBox.Focus();
            addressView.ValidateEmailAddress();
            GuarantorDateOfBirthPresenter.HandleGuarantorDateOfBirth();
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
                mtbDob.UnMaskedText = string.Empty;
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
        private bool ValidateDateOfBirth()
        {
            if (mtbDob.UnMaskedText.Length == 0)
            {
                dobDate = DateTime.MinValue;
                return true;
            }

            if (mtbDob.Text.Length != 10)
            {
                mtbDob.Focus();
                UIColors.SetErrorBgColor(mtbDob);
                MessageBox.Show(UIErrorMessages.DOB_INCOMPLETE_ERRMSG, "Error",
                                 MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                 MessageBoxDefaultButton.Button1);
                return false;
            }

            try
            {
                string month = mtbDob.Text.Substring(0, 2);
                string day = mtbDob.Text.Substring(3, 2);
                string year = mtbDob.Text.Substring(6, 4);

                dobDate = new DateTime(Convert.ToInt32(year),
                                        Convert.ToInt32(month),
                                        Convert.ToInt32(day));

                if (dobDate > DateTime.Today)
                {
                    // Remove the Admit Time Leave handler so error isn't generated
                    // when user comes back to the time field to correct the error.
                    mtbDob.Focus();
                    UIColors.SetErrorBgColor(mtbDob);
                    MessageBox.Show(UIErrorMessages.DOB_FUTURE_ERRMSG, "Error",
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button1);
                    return false;
                }

                if (DateValidator.IsValidDate(dobDate) == false)
                {
                    mtbDob.Focus();
                    UIColors.SetErrorBgColor(mtbDob);
                    MessageBox.Show(UIErrorMessages.DOB_NOTVALID_ERRMSG, "Error",
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            catch
            {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                // an invalid year, month, or day.  Simply set field to error color.
                mtbDob.Focus();
                UIColors.SetErrorBgColor(mtbDob);
                MessageBox.Show(UIErrorMessages.DOB_NOTVALID_ERRMSG, "Error",
                                 MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                 MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }
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

                        if ( response != null
                            && response.ResponseCreditReport != null )
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

                if ( Model_Account != null && Model_Account.Guarantor != null )
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
                        if ( Model_Account != null && Model_Account.Guarantor != null )
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
            UnregisterEventHandlers();
            RegisterEventHandlers();
            UIColors.SetNormalBgColor( mtbLastName );
            UIColors.SetNormalBgColor( mtbFirstName );
          
            RunGuarantorValidationRule();

            //---------the next Required Rules are part of OnShortGuarantorForm Rule that is Evaluted below:--------------
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonLastNameRequired ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonFirstNameRequired ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonPhoneAreaCodeRequired ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonPhoneNumberRequired ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( GuarantorAddressRequired ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonRelationshipRequired ), Model_Account.Guarantor );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonRelationshipPreferred ), Model_Account.Guarantor );
            ssnView.RunRules();
            RuleEngine.GetInstance().EvaluateRule( typeof( OnShortGuarantorForm ), Model_Account );
            RuleEngine.GetInstance().EvaluateRule( typeof( GuarantorConsentRequired ), Model_Account);             
            RuleEngine.GetInstance().OneShotRuleEvaluation<GuarantorConsentPreferred>(Model_Account.Guarantor, addressView.GuarantorConsentPreferredEventHandler);
            EmailAddressPresenter.RunGurantorEmailAddressRules();
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
            if ( !i_Registered  && Model_Account != null )
            {
                i_Registered = true;

                RuleEngine.GetInstance().RegisterEvent( typeof( GuarantorValidationFieldsProvided ), Model_Account.Guarantor, new EventHandler( GuarantorValidationFieldsProvidedEventHandler ) );

                RuleEngine.GetInstance().RegisterEvent( typeof( PersonLastNameRequired ), Model_Account.Guarantor, new EventHandler( PersonLastNameRequiredEventHandler ), typeof( OnShortGuarantorForm ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonFirstNameRequired ), Model_Account.Guarantor, new EventHandler( PersonFirstNameRequiredEventHandler ), typeof( OnShortGuarantorForm ) );

                RuleEngine.GetInstance().RegisterEvent( typeof( GuarantorAddressRequired ), Model_Account.Guarantor, new EventHandler( addressView.AddressRequiredEventHandler ), typeof( OnShortGuarantorForm ) );

                RuleEngine.GetInstance().RegisterEvent( typeof( PersonPhoneAreaCodeRequired ), Model_Account.Guarantor, new EventHandler( addressView.AreaCodeRequiredEventHandler ), typeof( OnShortGuarantorForm ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonPhoneNumberRequired ), Model_Account.Guarantor, new EventHandler( addressView.PhoneRequiredEventHandler ), typeof( OnShortGuarantorForm ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonDriversLicenseStateRequired ), Model_Account.Guarantor, new EventHandler( PersonDriversLicenseStateRequiredEventHandler ), typeof( OnShortGuarantorForm ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonRelationshipPreferred ), Model_Account.Guarantor, new EventHandler( PersonRelationshipPreferredEventHandler ), typeof( OnShortGuarantorForm ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonRelationshipRequired ), Model_Account.Guarantor, new EventHandler( PersonRelationshipRequiredEventHandler ), typeof( OnShortGuarantorForm ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( GuarantorConsentRequired ), Model_Account, addressView.GuarantorConsentRequiredEventHandler );
                RuleEngine.GetInstance().RegisterEvent( typeof( GuarantorConsentPreferred), Model_Account, addressView.GuarantorConsentPreferredEventHandler );
                RuleEngine.GetInstance().RegisterEvent( typeof( GuarantorEmailAddressPreferred ), Model_Account.Guarantor, addressView.GuarantorEmailAddressPreferredEventhandler);
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

            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonLastNameRequired ), Model_Account.Guarantor, PersonLastNameRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonFirstNameRequired ), Model_Account.Guarantor, PersonFirstNameRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonRelationshipPreferred ), Model_Account.Guarantor, PersonRelationshipPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonRelationshipRequired ), Model_Account.Guarantor, PersonRelationshipRequiredEventHandler );
            
            RuleEngine.GetInstance().UnregisterEvent( typeof( GuarantorAddressRequired ), Model_Account.Guarantor, addressView.AddressRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonPhoneAreaCodeRequired ), Model_Account.Guarantor, addressView.AreaCodeRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonPhoneNumberRequired ), Model_Account.Guarantor, addressView.PhoneRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonDriversLicenseStateRequired ), Model_Account.Guarantor, PersonDriversLicenseStateRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( GuarantorConsentRequired ), Model_Account, addressView.GuarantorConsentRequiredEventHandler);
            RuleEngine.GetInstance().UnregisterEvent( typeof( GuarantorConsentPreferred ), Model_Account, addressView.GuarantorConsentPreferredEventHandler);
            RuleEngine.GetInstance().UnregisterEvent( typeof( GuarantorEmailAddressPreferred ), Model_Account.Guarantor, addressView.GuarantorEmailAddressPreferredEventhandler );
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
            }

            relationshipView.ResetView();

            mtbLastName.UnMaskedText = String.Empty;

            mtbFirstName.UnMaskedText = String.Empty;

            mtbMI.UnMaskedText = String.Empty;
            mtbDob.UnMaskedText = String.Empty;
            mtbDLNumber.UnMaskedText = String.Empty;
            suffixPresenter.ClearSuffix();
            Model_Account.Guarantor.DateOfBirth = DateTime.MinValue;
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
            FormValidationResults fvr = new FormValidationResults {Model_Guarantor = Model_Account.Guarantor};
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ShortGuarantorView ) );
            this.btnView = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnInitiate = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblStatus = new System.Windows.Forms.Label();
            this.mtbDLNumber = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblState = new System.Windows.Forms.Label();
            this.cmbDLState = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
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
            this.lblStaticCopyFrom = new System.Windows.Forms.Label();
            this.copyPartyView = new PatientAccess.UI.GuarantorViews.CopyPartyView();
            this.addressView = new PatientAccess.UI.AddressViews.AddressView();
            this.suffixView = new SuffixView();
            this.ssnView = new PatientAccess.UI.CommonControls.SSNControl();
            this.btnClearAll = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panelGuarantorView = new System.Windows.Forms.Panel();
            this.mtbDob = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticDateOfBirth = new System.Windows.Forms.Label();
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
            this.mtbDLNumber.Location = new System.Drawing.Point(782, 51);
            this.mtbDLNumber.Mask = "";
            this.mtbDLNumber.MaxLength = 15;
            this.mtbDLNumber.Name = "mtbDLNumber";
            this.mtbDLNumber.Size = new System.Drawing.Size( 96, 20 );
            this.mtbDLNumber.TabIndex = 11;
            this.mtbDLNumber.ValidationExpression = "^[a-zA-Z0-9]*";
            this.mtbDLNumber.Validating += new System.ComponentModel.CancelEventHandler( this.mtbDLNumber_Validating );
            // 
            // lblState
            // 
            this.lblState.Location = new System.Drawing.Point(737, 82);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size( 34, 13 );
            this.lblState.TabIndex = 0;
            this.lblState.Text = "State:";
            // 
            // cmbDLState
            // 
            this.cmbDLState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDLState.Location = new System.Drawing.Point(782, 77);
            this.cmbDLState.Name = "cmbDLState";
            this.cmbDLState.Size = new System.Drawing.Size( 168, 21 );
            this.cmbDLState.TabIndex = 11;
            this.cmbDLState.SelectedIndexChanged += new System.EventHandler( this.DriversLicenseState_SelectedIndexChanged );
            this.cmbDLState.Validating += new System.ComponentModel.CancelEventHandler( this.cmbDLState_Validating );
            // 
            // mtbMI
            // 
            this.mtbMI.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbMI.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbMI.Location = new System.Drawing.Point( 532, 50 );
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
            this.mtbFirstName.Location = new System.Drawing.Point( 341, 50 );
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
            this.mtbLastName.Location = new System.Drawing.Point( 42, 50 );
            this.mtbLastName.Mask = "";
            this.mtbLastName.MaxLength = 25;
            this.mtbLastName.Name = "mtbLastName";
            this.mtbLastName.Size = new System.Drawing.Size( 257, 20 );
            this.mtbLastName.TabIndex = 3;
            this.mtbLastName.Validating += new System.ComponentModel.CancelEventHandler( this.mtbLastName_Validating );
            // 
            // Suffix View
            // 
            this.suffixView.Location = new System.Drawing.Point(575, 50);
            this.suffixView.Name = "SuffixView";
            this.suffixView.Size = new System.Drawing.Size(100, 27);
            this.suffixView.TabIndex = 6;
            this.suffixView.Visible = true;
            // 
            // lblMI
            // 
            this.lblMI.Location = new System.Drawing.Point( 511, 53 );
            this.lblMI.Name = "lblMI";
            this.lblMI.Size = new System.Drawing.Size( 21, 11 );
            this.lblMI.TabIndex = 0;
            this.lblMI.Text = "MI:";
            // 
            // lblFirstName
            // 
            this.lblFirstName.Location = new System.Drawing.Point( 312, 53 );
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size( 32, 12 );
            this.lblFirstName.TabIndex = 0;
            this.lblFirstName.Text = "First:";
            // 
            // lblLastName
            // 
            this.lblLastName.Location = new System.Drawing.Point( 13, 53 );
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size( 29, 16 );
            this.lblLastName.TabIndex = 0;
            this.lblLastName.Text = "Last:";
            // 
            // gbName
            // 
            this.gbName.Location = new System.Drawing.Point( 8, 31 );
            this.gbName.Name = "gbName";
            this.gbName.Size = new System.Drawing.Size( 675, 50 );
            this.gbName.TabIndex = 0;
            this.gbName.TabStop = false;
            this.gbName.Text = "Name";
            // 
            // lblNumber
            // 
            this.lblNumber.Location = new System.Drawing.Point(737, 54);
            this.lblNumber.Name = "lblNumber";
            this.lblNumber.Size = new System.Drawing.Size( 48, 13 );
            this.lblNumber.TabIndex = 0;
            this.lblNumber.Text = "Number:";
            // 
            // gbDriversLicnese
            // 
            this.gbDriversLicnese.Location = new System.Drawing.Point(728, 31);
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
            this.gbIdentity.TabIndex = 10;
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
            this.relationshipView.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( ( byte )( 0 ) ) );
            this.relationshipView.LabelText = "";
            this.relationshipView.Location = new System.Drawing.Point( 315, 8 );
            this.relationshipView.Model = null;
            this.relationshipView.Name = "relationshipView";
            this.relationshipView.PartyForRelationships = null;
            this.relationshipView.PatientIs = null;
            this.relationshipView.RelationshipName = null;
            this.relationshipView.Size = new System.Drawing.Size( 345, 22 );
            this.relationshipView.TabIndex = 2;
            this.relationshipView.RelationshipSelected += new System.EventHandler( this.RelationshipSelectedEvent );
            this.relationshipView.RelationshipValidating += new System.ComponentModel.CancelEventHandler( this.relationshipView_RelationshipValidating );
            // 
            // lblStaticCopyFrom
            // 
            this.lblStaticCopyFrom.Location = new System.Drawing.Point( 10, 9 );
            this.lblStaticCopyFrom.Name = "lblStaticCopyFrom";
            this.lblStaticCopyFrom.Size = new System.Drawing.Size( 127, 21 );
            this.lblStaticCopyFrom.TabIndex = 0;
            this.lblStaticCopyFrom.Text = "Copy to Guarantor from:";
            // 
            // copyPartyView
            // 
            this.copyPartyView.KindOfTargetParty = null;
            this.copyPartyView.Location = new System.Drawing.Point( 132, 8 );
            this.copyPartyView.Model = null;
            this.copyPartyView.Name = "copyPartyView";
            this.copyPartyView.Size = new System.Drawing.Size( 178, 22 );
            this.copyPartyView.TabIndex = 1;
            this.copyPartyView.PartySelected += new System.EventHandler( this.OnPartySelected );
            // 
            // addressView
            // 
            this.addressView.Context = null;
            this.addressView.EditAddressButtonText = "Edit Address...";
            this.addressView.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( ( byte )( 0 ) ) );
            this.addressView.IsAddressWithCounty = false;
            this.addressView.KindOfTargetParty = null;
            this.addressView.Location = new System.Drawing.Point( 8, 83 );
            this.addressView.Mode = PatientAccess.UI.AddressViews.AddressView.AddressMode.PHONECELLCONSENTEMAIL;
            this.addressView.Model = null;
            this.addressView.Model_ContactPoint = null;
            this.addressView.Name = "addressView";
            this.addressView.PatientAccount = null;
            this.addressView.ShowStatus = true;
            this.addressView.Size = new System.Drawing.Size(300, 240);
            this.addressView.TabIndex = 7;
            this.addressView.IsAddressWithStreet2 = true;
            this.addressView.AddressChanged += new System.EventHandler( this.AddressChangedEventHandler );
            this.addressView.AreaCodeChanged += new System.EventHandler( this.addressView_AreaCodeChanged );
            this.addressView.PhoneNumberChanged += new System.EventHandler( this.addressView_PhoneNumberChanged );
            this.addressView.CellPhoneNumberChanged += new System.EventHandler( this.addressView_CellPhoneNumberChanged );
            this.addressView.CellPhoneConsentChanged += new System.EventHandler( this.addressView_CellPhoneConsentChanged );
            this.addressView.EmailChanged += new System.EventHandler( this.addressView_EmailChanged );
            this.addressView.cellPhoneNumberControl.PhoneNumberTextChanged += new System.EventHandler(this.cellPhoneNumberControl_PhoneNumberTextChanged);
            // 
            // mtbDob
            // 
            this.mtbDob.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtStart;
            this.mtbDob.KeyPressExpression = "^\\d*$";
            this.mtbDob.Location = new System.Drawing.Point(72, 353);
            this.mtbDob.Mask = "  /  /    ";
            this.mtbDob.MaxLength = 10;
            this.mtbDob.Name = "mtbDob";
            this.mtbDob.Size = new System.Drawing.Size(70, 18);
            this.mtbDob.TabIndex = 8;
            this.mtbDob.ValidationExpression = "^\\d*$";
            this.mtbDob.Validating += new System.ComponentModel.CancelEventHandler(this.mtbDob_Validating); 
            // 
            // lblStaticDateOfBirth
            // 
            this.lblStaticDateOfBirth.Location = new System.Drawing.Point(19, 356);
            this.lblStaticDateOfBirth.Name = "lblStaticDateOfBirth";
            this.lblStaticDateOfBirth.Size = new System.Drawing.Size(60, 16);
            this.lblStaticDateOfBirth.TabIndex = 0;
            this.lblStaticDateOfBirth.Text = "DOB:";
            // 
            // ssnView
            // 
            this.ssnView.Location = new System.Drawing.Point(325, 103);
            this.ssnView.Model = null;
            this.ssnView.ModelAccount = ( ( PatientAccess.Domain.Account )( resources.GetObject( "ssnView.ModelAccount" ) ) );
            this.ssnView.Name = "ssnView";
            this.ssnView.Size = new System.Drawing.Size( 193, 72 );
            this.ssnView.SsnContext = SsnViewContext.ShortGuarantorView;
            this.ssnView.TabIndex = 9;
            // 
            // btnClearAll
            // 
            this.btnClearAll.Location = new System.Drawing.Point( 881, 345 );
            this.btnClearAll.Message = null;
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size( 72, 23 );
            this.btnClearAll.TabIndex = 12;
            this.btnClearAll.Text = "&Clear All";
            this.btnClearAll.Click += new System.EventHandler( this.btnClearAll_Click );
            // 
            // panelGuarantorView
            // 
            this.panelGuarantorView.Controls.Add( this.btnClearAll );
            this.panelGuarantorView.Controls.Add( this.ssnView );
            this.panelGuarantorView.Controls.Add( this.addressView );
            this.panelGuarantorView.Controls.Add( this.copyPartyView );
            this.panelGuarantorView.Controls.Add( this.lblStaticCopyFrom );
            this.panelGuarantorView.Controls.Add( this.mtbDLNumber );
            this.panelGuarantorView.Controls.Add( this.cmbDLState );
            this.panelGuarantorView.Controls.Add( this.lblStatus );
            this.panelGuarantorView.Controls.Add( this.lblState );
            this.panelGuarantorView.Controls.Add( this.mtbMI );
            this.panelGuarantorView.Controls.Add( this.mtbFirstName );
            this.panelGuarantorView.Controls.Add( this.mtbLastName );
            this.panelGuarantorView.Controls.Add( this.lblMI );
            this.panelGuarantorView.Controls.Add(this.suffixView); 
            this.panelGuarantorView.Controls.Add( this.lblFirstName );
            this.panelGuarantorView.Controls.Add( this.lblLastName );
            this.panelGuarantorView.Controls.Add( this.gbName );
            this.panelGuarantorView.Controls.Add( this.lblNumber );
            this.panelGuarantorView.Controls.Add( this.gbDriversLicnese );
            this.panelGuarantorView.Controls.Add( this.gbIdentity );
            this.panelGuarantorView.Controls.Add( this.relationshipView );
            this.panelGuarantorView.Controls.Add(this.mtbDob);
            this.panelGuarantorView.Controls.Add(this.lblStaticDateOfBirth);
            this.panelGuarantorView.Location = new System.Drawing.Point( 0, 0 );
            this.panelGuarantorView.Name = "panelGuarantorView";
            this.panelGuarantorView.Size = new System.Drawing.Size( 1005, 374 );
            this.panelGuarantorView.TabIndex = 0;
            // 
            // ShortGuarantorView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.panelGuarantorView );
            this.Name = "ShortGuarantorView";
            this.Size = new System.Drawing.Size( 1005, 374 );
            this.Enter += new System.EventHandler( this.GuarantorView_Enter );
            this.Leave += new System.EventHandler( this.GuarantorView_Leave );
            this.Validating += new System.ComponentModel.CancelEventHandler( this.GuarantorView_Validating );
            this.Disposed += new System.EventHandler( this.GuarantorView_Disposed );
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
        public ShortGuarantorView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            ConfigureControls();

            ComponentResourceManager resources = new ComponentResourceManager( typeof( ShortGuarantorView ) );
            addressView.EditAddressButtonText = "Edit &Address...";

            aGuarantor = null;
            selectedRelationshipType = null;
            driversLicenseState = null;
            relationShip = null;
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

        private Label lblStatus;
        private Label lblState;
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
        private RelationshipView relationshipView;
        private CopyPartyView copyPartyView;
        private SSNControl ssnView;
        private ReferenceValueComboBox i_StateComboHelper;

        private Guarantor aGuarantor;
        private State driversLicenseState;
        private Relationship relationShip;
        private RelationshipType selectedRelationshipType;
        private int driversLicenseStateSelectedIndex;
        private bool i_Registered;
        private Relationship i_OriginalRelationship;

        private readonly Timer i_ResponseWaitTimer = new Timer();
        private readonly Timer i_ResponsePollTimer = new Timer();

        private bool blnLeaveRun;
        private string detail = string.Empty;
        private bool loadingModelData = true;
        private EmailAddressPresenter EmailAddressPresenter { get; set; }
        private MaskedEditTextBox mtbDob;
        private Label lblStaticDateOfBirth;
        private DateTime dobDate;
        private SuffixView suffixView;
        private SuffixPresenter suffixPresenter;
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

        // 5 seconds (in milliseconds)
        private const int RESPONSE_POLL_INTERVAL = 5000;

        #endregion
    }
}
