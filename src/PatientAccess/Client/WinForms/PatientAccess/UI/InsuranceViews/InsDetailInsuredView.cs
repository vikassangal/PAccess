using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.AddressViews;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.CommonControls.Suffix.Presenters;
using PatientAccess.UI.CommonControls.Suffix.ViewImpl;
using PatientAccess.UI.Factories;
using PatientAccess.UI.GuarantorViews;
using PatientAccess.UI.HelperClasses;
using EmploymentView = PatientAccess.UI.CommonControls.EmploymentView;

namespace PatientAccess.UI.InsuranceViews
{
    /// <summary>
    /// Event handler for CopyPartyView.
    /// </summary>
    [Serializable]
    public class InsDetailInsuredView : ControlView
    {
        #region Events
        public event EventHandler ResetButtonClicked;
        #endregion

        #region Rule Event Handlers
        private void PersonRelationshipRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( relationshipView.ComboBox );
        }

        private void PersonLastNameRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbLastName );
        }

        private void PersonFirstNameRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbFirstName );
        }

     
        private void PersonDateOfBirthRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( mtbDob );
        }

        private void PersonEmploymentStatusRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( employmentView.EmploymentStatusComboBox );
        }
       
        private void PersonEmployerAddressRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( (Control)employmentView.EmployerField );
        }

        private void InsuredEmployerRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( ( Control )employmentView.EmployerField ); 
            Refresh();
        }

        private void PersonRelationshipPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( relationshipView.ComboBox );
        }

        private void PersonLastNamePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( mtbLastName );
        }

        private void PersonFirstNamePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( mtbFirstName );
        }

        private void PersonDateOfBirthPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( mtbDob );
        }


        private void PersonEmploymentStatusPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( employmentView.EmploymentStatusComboBox );
        }

        private void PersonEmployerAddressPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( (Control)employmentView.EmployerField );
        }
        private void EmploymentAreaCodePreferredEventHandler( object sender, EventArgs e )
        {
            PropertyChangedArgs args = (PropertyChangedArgs)e;
            Control aControl = args.Context as Control;

            if ( aControl == employmentView.PhoneNumberControl &&
                employmentView.PhoneNumberControl != null )
            {
                employmentView.PhoneNumberControl.SetAreaCodePreferredColor();
            }
        }

        private void EmploymentPhoneNumberPreferredEventHandler( object sender, EventArgs e )
        {
            PropertyChangedArgs args = (PropertyChangedArgs)e;
            Control aControl = args.Context as Control;

            if ( aControl == employmentView.PhoneNumberControl &&
                employmentView.PhoneNumberControl != null )
            {
                employmentView.PhoneNumberControl.SetPhoneNumberPreferredColor();
            }
        }

        //---------------------Evaluate ComboBoxes -------------------------------------------------------------
        private void relationshipView_RelationshipValidating( object sender, CancelEventArgs e )
        {
            if( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( relationshipView.ComboBox );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidRelForPriInsured ), Model_Account );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidRelForPriInsuredChange ), Model_Account );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidRelForSecInsured ), Model_Account );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidRelForSecInsuredChange ), Model_Account );
                RuleEngine.GetInstance().EvaluateRule( typeof( PersonRelationshipRequired ), Model_Insured);
                RuleEngine.GetInstance().EvaluateRule( typeof( PersonRelationshipPreferred ), Model_Insured);
            }
        }

        private void genderControl_GenderControlValidating( object sender, CancelEventArgs e )
        {
            if( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( genderControl.ComboBox );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidGenderForPriInsured ), Model_Account );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidGenderForPriInsuredChange ), Model_Account );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidGenderForSecInsured ), Model_Account );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidGenderForSecInsuredChange ), Model_Account );
                RuleEngine.GetInstance().EvaluateRule( typeof( PersonGenderRequired ), Model_Insured );
                RuleEngine.GetInstance().EvaluateRule( typeof( PersonGenderPreferred ), Model_Insured );
               
            }
        }

        private void employmentView_EmploymentStatusValidating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( employmentView.EmploymentStatusComboBox );
            UIColors.SetNormalBgColor( (Control)employmentView.EmployerField );
            UIColors.SetNormalBgColor( employmentView.PhoneNumberControl );
            UIColors.SetNormalBgColor( employmentView.EmploymentStatusComboBox );

            Refresh();

            IsEmployerChanged();
                
            Model_Insured.Employment = employmentView.Model_Employment;

            SetPreviousEmployer();

            if( !blnLeaveRun )
            {
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidEmpStatusForPriInsured ), Model_Account );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidEmpStatusForPriInsuredChange ), Model_Account );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidEmpStatusForSecInsured ), Model_Account );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidEmpStatusForSecInsuredChange ), Model_Account );
            }

            RuleEngine.GetInstance().EvaluateRule( typeof( PersonEmploymentStatusRequired ), Model_Insured );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonEmploymentStatusPreferred ), Model_Insured );
            
            RuleEngine.GetInstance().EvaluateRule(typeof(InsuredEmployerRequired), Model_Insured, employmentView.EmployerField);
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonEmployerPreferred), Model_Insured, employmentView.EmployerField );  

            // it is imperative that these two rules are run in this order
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonEmployerAddressPreferred ), Model_Insured );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonEmployerAddressRequired), Model_Insured );
        }
        //--------------------- InvalidValues in Comboboxes Event Handlers: ------------------------------------

        private static void ProcessInvalidCodeEvent( PatientAccessComboBox comboBox )
        {
            UIColors.SetDeactivatedBgColor( comboBox );

            MessageBox.Show( UIErrorMessages.INVALID_VALUE_ERRMSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1 );

            if( !comboBox.Focused )
            {
                comboBox.Focus();
            }
        }

        private void InvalidRelForPriInsuredChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( relationshipView.ComboBox );
        }
        private void InvalidRelForSecInsuredChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( relationshipView.ComboBox );
        }
        private void InvalidGenderForPriInsuredChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( genderControl.ComboBox );
        }
        private void InvalidGenderForSecInsuredChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( genderControl.ComboBox );
        }
        private void InvalidEmpStatusForPriInsuredChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( employmentView.EmploymentStatusComboBox );
        }
        private void InvalidEmpStatusForSecInsuredChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( employmentView.EmploymentStatusComboBox );
        }
        private void PersonGenderRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( genderControl.ComboBox );
        }
        private void PersonGenderPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( genderControl.ComboBox );
        }

        //-----------------------------------------------------------------

        private void InvalidRelForPriInsuredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( relationshipView.ComboBox );
        }
        private void InvalidRelForSecInsuredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( relationshipView.ComboBox );
        }
        private void InvalidGenderForPriInsuredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( genderControl.ComboBox );
        }
        private void InvalidGenderForSecInsuredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( genderControl.ComboBox );
        }
        private void InvalidEmpStatusForPriInsuredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( employmentView.EmploymentStatusComboBox );
        }
        private void InvalidEmpStatusForSecInsuredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( employmentView.EmploymentStatusComboBox );
        }

        private void SetPreviousEmployer()
        {
            if( Model_Insured != null &&
                   Model_Insured.Employment != null &&
                   Model_Insured.Employment.Employer != null &&
                   Model_Insured.Employment.Status != null )
            {
                prevEmployerName = Model_Insured.Employment.Employer.Name;
                prevEmploymentStatusCode = Model_Insured.Employment.Status.Code;
            }
        }

        #endregion

        #region Event Handlers

        private void InsDetailInsuredView_Leave( object sender, EventArgs e )
        {
            blnLeaveRun = true;
            //run Invalid Values Rules:
            RuleEngine.GetInstance().EvaluateRule( typeof( OnInsuredFormForPrimaryInsurance ), Model_Account );
            RuleEngine.GetInstance().EvaluateRule( typeof( OnInsuredFormForSecondaryInsurance ), Model_Account );
            blnLeaveRun = false;

            UpdateModel();
        }

        private void InsDetailInsuredView_Disposed( object sender, EventArgs e )
        {
            UnregisterEventHandlers();
        }

        private void mtbLastName_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbLastName );
            Refresh();
            Model_Insured.LastName = mtbLastName.UnMaskedText;
            EvaluateInsuredLastNameRequired();
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonLastNamePreferred ), Model_Insured );
        }

        private void mtbFirstName_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbFirstName );
            Refresh();
            Model_Insured.FirstName = mtbFirstName.UnMaskedText;
            EvaluateInsuredFirstNameRequired();
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonFirstNamePreferred ), Model_Insured );
        }
        private void EvaluateInsuredFirstNameRequired()
        {
            if ( !IsShortPreRegistrationAccount() )
            {

                RuleEngine.GetInstance().EvaluateRule(typeof (PersonFirstNameRequired), Model_Insured);
            }
        }
        private void EvaluateInsuredLastNameRequired()
        {
            if ( !IsShortPreRegistrationAccount() )
            {

                RuleEngine.GetInstance().EvaluateRule( typeof( PersonLastNameRequired ), Model_Insured );
            }
        }
        private void RegisterInsuredLastNameRequired( Type compRuleType )
        {
            if ( !IsShortPreRegistrationAccount() )
            {

                RuleEngine.GetInstance().RegisterEvent( typeof( PersonLastNameRequired ), Model_Insured, mtbLastName, PersonLastNameRequiredEventHandler, compRuleType );
            }
        }
        private void RegisterInsuredFirstNameRequired( Type compRuleType )
        {
            if ( !IsShortPreRegistrationAccount())
            {
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonFirstNameRequired ), Model_Insured, mtbLastName, PersonFirstNameRequiredEventHandler, compRuleType );
            }
        }
          private bool IsShortPreRegistrationAccount()
          {
              if ( Model_Account.Activity.IsShortPreRegistrationActivity() ||
                  ( Model_Account.Activity.IsShortMaintenanceActivity() && 
                     Model_Account.KindOfVisit.Code == VisitType.PREREG_PATIENT))
              {
                  return true;
              }
              return false;
          }
        private void mtbMiddleInitial_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbMiddleInitial );
            Refresh();
            Model_Insured.Name.MiddleInitial = mtbMiddleInitial.UnMaskedText;
        }

        private void mtbNationalID_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbNationalID );
            Refresh();
            Model_Insured.NationalID = mtbNationalID.UnMaskedText;
        }

        private void employmentView_EmploymentViewChangedEvent( object sender, EventArgs e )
        {
            if (Model_Insured == null)
            {
                return;
            }

            UIColors.SetNormalBgColor((Control) employmentView.EmployerField);
            UIColors.SetNormalBgColor(employmentView.EmploymentStatusComboBox);
            Refresh();

            Model_Insured.Employment = employmentView.Model_Employment;
            IsEmployerChanged();
            SetPreviousEmployer();

            RuleEngine.GetInstance().EvaluateRule(typeof (PersonEmployerAddressRequired), Model_Insured);
            RuleEngine.GetInstance().EvaluateRule(typeof (PersonEmployerAddressPreferred), Model_Insured);

            RuleEngine.GetInstance().EvaluateRule(typeof (PersonEmploymentStatusRequired), Model_Insured);
            RuleEngine.GetInstance().EvaluateRule(typeof (PersonEmploymentStatusPreferred), Model_Insured);
            
            RuleEngine.GetInstance().EvaluateRule(typeof (InsuredEmployerRequired), Model_Insured,
                                                  employmentView.EmployerField);
            RuleEngine.GetInstance().EvaluateRule(typeof (PersonEmployerPreferred), Model_Insured,
                                                  employmentView.EmployerField);
            RuleEngine.GetInstance().EvaluateRule( typeof( EmploymentPhoneNumberPreferred ), Model_Insured.Employment, employmentView.PhoneNumberControl );
            RuleEngine.GetInstance().EvaluateRule( typeof( EmploymentPhoneAreaCodePreferred ),
                                                  Model_Insured.Employment, employmentView.PhoneNumberControl );

        }

        private void AddressChangedEventHandler( object sender, EventArgs e )
        {
            ContactPoint physicalContactPoint = Model_Insured.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );
            physicalContactPoint.Address = addressView.Model_ContactPoint.Address;
            RuleEngine.GetInstance().EvaluateRule( typeof( InsuredAddressPreferred ), Model_Insured );
            RuleEngine.GetInstance().EvaluateRule( typeof( InsuredAddressRequired ), Model_Insured );

        }

        private void PartySelectedEvent( object sender, EventArgs e )
        {
            LooseArgs args = (LooseArgs)e;

            if( args.Context != null )
            {
                string partyType = copyPartyView.ComboBox.Text;

                Party partySelected = (Party)args.Context;

                if( partySelected != null )
                {
                    Model_Insured = partySelected.CopyAsInsured();
                    Model_Coverage.Insured = Model_Insured;
                    UpdateView( false );
                }
                else
                {
                    ResetView( false );
                }

                string relType = string.Empty;

                switch( partyType )
                {
                    case "Patient":
                        relType = SELF;
                        break;

                    case "Patient's Employer":
                        relType = EMPLOYEE;
                        break;

                    case "Guarantor":
                        foreach( Relationship r in partySelected.Relationships )
                        {
                            relType = r.Type.Description;
                            break;
                        }
                        break;
                    case "Insured - Primary":
                    case "Insured - Secondary":
                        foreach( Relationship r in partySelected.Relationships )
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
        }

        private void relationshipView_RelationshipSelected( object sender, EventArgs e )
        {
            LooseArgs args = (LooseArgs) e;
            if( args == null )
            {
                return;
            }
            
            if( Model_Insured == null )
            {
                Model_Insured = new Insured();
            }

            if( i_OriginalRelationship != null && i_OriginalRelationship.Type != null )
            {
                Model_Insured.RemoveRelationship( i_OriginalRelationship );
                Model_Account.Patient.RemoveRelationship( i_OriginalRelationship );
            }

            RelationshipType selectedRelationshipType = args.Context as RelationshipType;

            if( selectedRelationshipType != null )
            {
                Relationship insRelationship = new Relationship( selectedRelationshipType,
                        Model_Account.Patient.GetType(), Model_Insured.GetType() );

                i_OriginalRelationship = insRelationship;

                Model_Insured.AddRelationship( insRelationship );

                Relationship patRelationship = new Relationship( selectedRelationshipType,
                                                                 Model_Account.Patient.GetType(), Model_Insured.GetType() );

                Model_Account.Patient.AddRelationship( patRelationship );
            }

            UIColors.SetNormalBgColor( mtbDob );
            UIColors.SetNormalBgColor( mtbFirstName );
            UIColors.SetNormalBgColor( genderControl.ComboBox );
            UIColors.SetNormalBgColor( employmentView.EmploymentStatusComboBox );
            UIColors.SetNormalBgColor( relationshipView.ComboBox );

            RuleEngine.GetInstance().EvaluateRule( typeof( PersonEmploymentStatusRequired ), Model_Insured );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonEmploymentStatusPreferred ), Model_Insured );

            RuleEngine.GetInstance().EvaluateRule( typeof( PersonRelationshipRequired ), Model_Insured );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonRelationshipPreferred ), Model_Insured );

            EvaluateInsuredLastNameRequired();
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonLastNamePreferred ), Model_Insured );

            EvaluateInsuredFirstNameRequired();
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonFirstNamePreferred ), Model_Insured );
       
            RuleEngine.GetInstance().EvaluateRule( typeof( PrimaryInsuredDateOfBirthRequired ), Model_Account );
            RuleEngine.GetInstance().EvaluateRule( typeof( PrimaryInsuredDateOfBirthPreferred ), Model_Account );

            RuleEngine.GetInstance().EvaluateRule( typeof( SecondaryInsuredDateOfBirthRequired ), Model_Account );
            RuleEngine.GetInstance().EvaluateRule( typeof( SecondaryInsuredDateOfBirthPreferred ), Model_Account );

            RuleEngine.GetInstance().EvaluateRule( typeof( InsuredEmployerRequired ), Model_Insured, employmentView.EmployerField );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonEmployerPreferred), Model_Insured, employmentView.EmployerField );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonGenderPreferred ), Model_Insured );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonGenderRequired), Model_Insured );
            RuleEngine.GetInstance().EvaluateRule( typeof( InsuredAddressPreferred ), Model_Insured );
            RuleEngine.GetInstance().EvaluateRule( typeof( InsuredAddressRequired ), Model_Insured );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonPhoneAreaCodePreferred ), Model_Insured );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonPhoneNumberPreferred ), Model_Insured );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonEmployerAddressPreferred ), Model_Insured );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonEmployerAddressRequired ), Model_Insured );
            RuleEngine.GetInstance().EvaluateRule( typeof( EmploymentPhoneNumberPreferred ), Model_Insured.Employment, employmentView.PhoneNumberControl );
            RuleEngine.GetInstance().EvaluateRule( typeof( EmploymentPhoneAreaCodePreferred ), Model_Insured.Employment, employmentView.PhoneNumberControl );
       
        }

        private void GenderSelectedEvent( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( genderControl.ComboBox );

            LooseArgs args = (LooseArgs)e;

            if( Model_Insured == null )
            {
                return;
            }
            if( args != null )
            {
                Gender gender = args.Context as Gender;
                Model_Insured.Sex = gender;
            }
            else
            {
                Model_Insured.Sex = new Gender();
            }
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonGenderRequired ), Model_Insured );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonGenderPreferred ), Model_Insured );
       
        }

        private void AddressView_AreaCodeChanged( object sender, EventArgs e )
        {
            ContactPoint physicalContactPoint = Model_Insured.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );
            physicalContactPoint.PhoneNumber = addressView.Model_ContactPoint.PhoneNumber;
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonPhoneAreaCodePreferred ), Model_Insured );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonPhoneNumberPreferred ), Model_Insured );
             
        }

        private void AddressView_PhoneNumberChanged( object sender, EventArgs e )
        {
            ContactPoint physicalContactPoint = Model_Insured.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType() );
            physicalContactPoint.PhoneNumber = addressView.Model_ContactPoint.PhoneNumber;
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonPhoneAreaCodePreferred ), Model_Insured );
            RuleEngine.GetInstance().EvaluateRule( typeof( PersonPhoneNumberPreferred ), Model_Insured );
             
        }

        private void AddressView_CellPhoneNumberChanged( object sender, EventArgs e )
        {
            ContactPoint mobileContactPoint = Model_Insured.ContactPointWith( TypeOfContactPoint.NewMobileContactPointType() );
            mobileContactPoint.PhoneNumber = addressView.Model_ContactPoint.CellPhoneNumber;
        }

        private void mtbDob_Leave( object sender, EventArgs e )
        {
            //this.mtbDob_Validating(this, new CancelEventArgs(false));
        }

        private void mtbDob_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbDob );

            if( !ValidateDateOfBirth() )
            {
                UIColors.SetErrorBgColor( mtbDob );
                mtbDob.Focus();
                //e.Cancel = true;
                //return;
            }

            Model_Insured.DateOfBirth = dobDate;

            if( Model_Coverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID )
            {
                RuleEngine.GetInstance().EvaluateRule( typeof( PrimaryInsuredDateOfBirthRequired ), Model_Account );
                RuleEngine.GetInstance().EvaluateRule( typeof( PrimaryInsuredDateOfBirthPreferred ), Model_Account );
            }
            else
            {
                RuleEngine.GetInstance().EvaluateRule( typeof( SecondaryInsuredDateOfBirthRequired ), Model_Account );
                RuleEngine.GetInstance().EvaluateRule( typeof( SecondaryInsuredDateOfBirthPreferred ), Model_Account );
            }
        }

        private void ResetButtonClick( object sender, EventArgs e )
        {
            if( ResetButtonClicked != null )
            {
                ResetButtonClicked( sender, e );
            }
            ResetView( true );
        }

        private void IsEmployerChanged()
        {
            bool isMessageDisplayed = false;
            if( Model_Insured != null && 
                Model_Insured.Employment != null)
            {
                if( Model_Insured.Employment.Status != null )
                {
                    string empStatusCode = Model_Insured.Employment.Status.Code;
                    if( prevEmploymentStatusCode.Trim().Length > 0 && empStatusCode != Model_Insured.PreviousEmploymentStatusCode &&
                        empStatusCode != prevEmploymentStatusCode )
                    {
                        isMessageDisplayed = true;
                        MessageBox.Show( UIErrorMessages.INSURED_EMPLOYMENT_STATUS_CHANGED, "Warning",
                                         MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 );
                    }
                }
                if( !isMessageDisplayed && Model_Insured.Employment.Employer != null )
                {
                    string employerName = Model_Insured.Employment.Employer.Name;

                    if( prevEmployerName.Trim().Length > 0 && employerName != Model_Insured.PreviousEmployerName 
                      && employerName != prevEmployerName )
                    {
                        MessageBox.Show( UIErrorMessages.INSURED_EMPLOYER_CHANGED, "Warning",
                                         MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 );
                    }
                }
            }
        }

        #endregion

        #region Methods

        public bool CheckValidations()
        {
            return ( ValidateDateOfBirth() );
        }

        public override void UpdateView()
        {
            if( ParentForm != null )
            {
                copyPartyView.CoverageOrder = ( (InsuranceDetails)ParentForm ).insuranceDetailsView.Model_Coverage.CoverageOrder;
            }

            UpdateView( true );
        }

        private void UpdateView( bool updateCopyParty )
        {
            if( updateCopyParty )
            {
                copyPartyView.Model = Model_Account;
                copyPartyView.KindOfTargetParty = Model_Insured.GetType();
                copyPartyView.CoverageOrder = Model_Coverage.CoverageOrder;
                copyPartyView.UpdateView();
            }

            if( Model_Insured != null )
            {
                foreach( Relationship r in Model_Insured.Relationships )
                {
                    i_OriginalRelationship = r;
                    break;
                }

                relationshipView.Model = Model_Insured.Relationships;
                relationshipView.PartyForRelationships = Model_Insured;
                relationshipView.UpdateView();

                // Set ComboBox selectedIndex to last selected relationship
                if( i_OriginalRelationship != null )
                {
                    relationshipView.ComboBox.SelectedItem = i_OriginalRelationship.Type.AsDictionaryEntry();
                }

                if( Model_Insured.DateOfBirth != DateTime.MinValue )
                {
                    mtbDob.UnMaskedText = Model_Insured.DateOfBirth.ToString( "MMddyyyy" );
                    dobDate = Model_Insured.DateOfBirth;
                }
                else
                {
                    mtbDob.UnMaskedText = String.Empty;
                    dobDate = DateTime.MinValue;
                }
                var suffixContext  = Model_Coverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID ? "PrimaryInsured" : "SecondaryInsured";
                suffixPresenter = new SuffixPresenter(suffixView, Model_Account, suffixContext );
                mtbLastName.UnMaskedText = Model_Insured.LastName;
                mtbFirstName.UnMaskedText = Model_Insured.FirstName;
                mtbMiddleInitial.UnMaskedText = Model_Insured.Name.MiddleInitial;
                suffixPresenter.UpdateView();
               
                mtbNationalID.UnMaskedText = Model_Insured.NationalID;

                genderControl.InitializeGendersComboBox();

                if( Model_Insured.Sex != null )
                {
                    genderControl.ComboBox.SelectedItem = Model_Insured.Sex.AsDictionaryEntry();
                }

                SetPreviousEmployer();

                employmentView.Model = Model_Insured.Employment;
                employmentView.Model_Account = Model_Account;
                employmentView.CoverageOrderID = Model_Coverage.CoverageOrder.Oid;
              
                addressView.KindOfTargetParty = Model_Insured.GetType();
                addressView.Context = Model_Coverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID ? "PrimaryInsured" : "SecondaryInsured";
                addressView.PatientAccount = Model_Account;
                ContactPoint generalContactPoint = new ContactPoint
                    {
                        Address = Model_Insured.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType()). Address,
                        PhoneNumber = Model_Insured.ContactPointWith( TypeOfContactPoint.NewPhysicalContactPointType()). PhoneNumber,
                        CellPhoneNumber = Model_Insured.ContactPointWith( TypeOfContactPoint.NewMobileContactPointType()). PhoneNumber
                    };
                addressView.Model_ContactPoint = generalContactPoint;
            }
            else
            {
                employmentView.Model = null;
                addressView.Model = null;
            }

            employmentView.UpdateView();
            addressView.UpdateView();

            RegisterEventHandlers();
            RunRules();
        }

        public override void UpdateModel()
        {
            if( Model_Insured != null )
            {
                if( employmentView.Model_Employment != null )
                {
                    Model_Insured.Employment = (Employment)employmentView.Model_Employment.DeepCopy();
                }
            }

            Model_Coverage.Insured = Model_Insured;

        }
        #endregion

        #region Properties
        public Coverage Model_Coverage
        {
            set
            {
                i_Model_Coverage = value;
            }
            private get
            {
                return i_Model_Coverage;
            }
        }

        public Insured Model_Insured
        {
            set
            {
                Model = value;
            }
            private get
            {
                return (Insured)Model;
            }
        }

        public Account Model_Account
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

        private bool IsDOBBefore1800
        {
            get { return (dobDate < earliestDateOfBirth); }
        }

        #endregion

        #region Private Methods

        private void RunRules()
        {
            UnregisterEventHandlers();
            RegisterEventHandlers();
            if ( Model_Account != null )
            {
                UIColors.SetNormalBgColor( relationshipView.ComboBox );
                UIColors.SetNormalBgColor( mtbLastName );
                UIColors.SetNormalBgColor( mtbFirstName );
                UIColors.SetNormalBgColor( genderControl.ComboBox );
                UIColors.SetNormalBgColor( mtbDob );
                UIColors.SetNormalBgColor( employmentView.EmploymentStatusComboBox );
                UIColors.SetNormalBgColor( (Control)employmentView.EmployerField );
                UIColors.SetNormalBgColor( employmentView.PhoneNumberControl );

                if ( Model_Insured == null )
                {
                    Model_Insured = new Insured();
                }

                RuleEngine.GetInstance().EvaluateRule( typeof( PersonRelationshipRequired ), Model_Insured );
                
                RuleEngine.GetInstance().EvaluateRule( typeof( PersonGenderRequired ), Model_Insured );
                RuleEngine.GetInstance().EvaluateRule( typeof( PersonDateOfBirthRequired ), Model_Insured );
                RuleEngine.GetInstance().EvaluateRule( typeof( InsuredAddressRequired ), Model_Insured );

                RuleEngine.GetInstance().EvaluateRule( typeof( PersonEmploymentStatusRequired ), Model_Insured );
                RuleEngine.GetInstance().EvaluateRule( typeof( PersonEmployerAddressRequired ), Model_Insured );



                RuleEngine.GetInstance().EvaluateRule( typeof( PersonRelationshipPreferred ), Model_Insured );
                EvaluateInsuredFirstNameRequired();
                EvaluateInsuredLastNameRequired();

                RuleEngine.GetInstance().EvaluateRule( typeof( PrimaryInsuredDateOfBirthPreferred ), Model_Account );
                RuleEngine.GetInstance().EvaluateRule( typeof( PrimaryInsuredDateOfBirthRequired ), Model_Account );
                RuleEngine.GetInstance().EvaluateRule( typeof( SecondaryInsuredDateOfBirthPreferred ), Model_Account );
                RuleEngine.GetInstance().EvaluateRule( typeof( SecondaryInsuredDateOfBirthRequired ), Model_Account );

                RuleEngine.GetInstance().EvaluateRule( typeof( PersonGenderPreferred ), Model_Insured );
                RuleEngine.GetInstance().EvaluateRule( typeof( InsuredAddressPreferred ), Model_Insured );
                RuleEngine.GetInstance().EvaluateRule( typeof( PersonPhoneNumberPreferred ), Model_Insured );
                RuleEngine.GetInstance().EvaluateRule( typeof( PersonPhoneAreaCodePreferred ), Model_Insured );

                RuleEngine.GetInstance().EvaluateRule( typeof( PersonEmploymentStatusPreferred ), Model_Insured );
                RuleEngine.GetInstance().EvaluateRule( typeof( InsuredEmployerRequired ), Model_Insured, employmentView.EmployerField );
                RuleEngine.GetInstance().EvaluateRule( typeof( PersonEmployerAddressPreferred ), Model_Insured );

                RuleEngine.GetInstance().EvaluateRule( typeof( EmploymentPhoneNumberPreferred ), Model_Insured.Employment );
                RuleEngine.GetInstance().EvaluateRule( typeof( EmploymentPhoneAreaCodePreferred ), Model_Insured.Employment );
                //run required/preferred rules (Insured as context):
                RuleEngine.GetInstance().EvaluateRule( typeof( OnInsuredFormForPrimaryInsurance ), Model_Insured );
                RuleEngine.GetInstance().EvaluateRule( typeof( OnInsuredFormForSecondaryInsurance ), Model_Insured );

                //run invalid value rules (Account as context):
                RuleEngine.GetInstance().EvaluateRule( typeof( OnInsuredFormForPrimaryInsurance ), Model_Account );
                RuleEngine.GetInstance().EvaluateRule( typeof( OnInsuredFormForSecondaryInsurance ), Model_Account );
            }
        }

        private void RegisterEventHandlers()
        {
         
            if ( !i_Registered )
            {
                i_Registered = true;

                Type compRuleType;

                if ( Model_Coverage != null
                    && Model_Coverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID )
                {
                    compRuleType = typeof( OnInsuredFormForPrimaryInsurance );
                }
                else
                {
                    compRuleType = typeof( OnInsuredFormForSecondaryInsurance );
                }

                RuleEngine.GetInstance().RegisterEvent( typeof( PersonRelationshipRequired ), Model_Insured, new EventHandler( PersonRelationshipRequiredEventHandler ), compRuleType );
                RegisterInsuredFirstNameRequired( compRuleType );
                RegisterInsuredLastNameRequired( compRuleType );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonGenderRequired ), Model_Insured, new EventHandler( PersonGenderRequiredEventHandler ), compRuleType );
                RuleEngine.GetInstance().RegisterEvent( typeof( InsuredAddressRequired ), Model_Insured, new EventHandler( addressView.AddressRequiredEventHandler ), compRuleType );





                RuleEngine.GetInstance().RegisterEvent( typeof( PersonEmploymentStatusRequired ), Model_Insured, new EventHandler( PersonEmploymentStatusRequiredEventHandler ), compRuleType );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonEmployerAddressRequired ), Model_Insured, new EventHandler( PersonEmployerAddressRequiredEventHandler ), compRuleType );

                if ( Model_Coverage != null )
                {
                    if ( Model_Coverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID )
                    {
                        RuleEngine.GetInstance().RegisterEvent( typeof( PrimaryInsuredDateOfBirthRequired ), Model_Account,
                            new EventHandler( PersonDateOfBirthRequiredEventHandler ), compRuleType );
                        RuleEngine.GetInstance().RegisterEvent( typeof( PrimaryInsuredDateOfBirthPreferred ),
                            Model_Account, new EventHandler( PersonDateOfBirthPreferredEventHandler ), compRuleType );
                    }
                    else
                    {
                        RuleEngine.GetInstance().RegisterEvent( typeof( SecondaryInsuredDateOfBirthRequired ),
                            Model_Account, new EventHandler( PersonDateOfBirthRequiredEventHandler ), compRuleType );
                        RuleEngine.GetInstance().RegisterEvent( typeof( SecondaryInsuredDateOfBirthPreferred ),
                            Model_Account, new EventHandler( PersonDateOfBirthPreferredEventHandler ), compRuleType );
                    }
                }

                RuleEngine.GetInstance().RegisterEvent( typeof( PersonRelationshipPreferred ), Model_Insured, new EventHandler( PersonRelationshipPreferredEventHandler ), compRuleType );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonLastNamePreferred ), Model_Insured, new EventHandler( PersonLastNamePreferredEventHandler ), compRuleType );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonFirstNamePreferred ), Model_Insured, new EventHandler( PersonFirstNamePreferredEventHandler ), compRuleType );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonGenderPreferred ), Model_Insured, new EventHandler( PersonGenderPreferredEventHandler ), compRuleType );
                RuleEngine.GetInstance().RegisterEvent( typeof( InsuredAddressPreferred ), Model_Insured, new EventHandler( addressView.AddressPreferredEventHandler ), compRuleType );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonPhoneAreaCodePreferred ), Model_Insured, new EventHandler( addressView.AreaCodePreferredEventHandler ), compRuleType );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonPhoneNumberPreferred ), Model_Insured, new EventHandler( addressView.PhonePreferredEventHandler ), compRuleType );

                RuleEngine.GetInstance().RegisterEvent( typeof( PersonEmploymentStatusPreferred ), Model_Insured, new EventHandler( PersonEmploymentStatusPreferredEventHandler ), compRuleType );
                RuleEngine.GetInstance().RegisterEvent( typeof( InsuredEmployerRequired ), Model_Insured, new EventHandler( InsuredEmployerRequiredEventHandler ), compRuleType );
                RuleEngine.GetInstance().RegisterEvent( typeof( PersonEmployerAddressPreferred ), Model_Insured, new EventHandler( PersonEmployerAddressPreferredEventHandler ), compRuleType );

                RuleEngine.GetInstance().RegisterEvent( typeof( EmploymentPhoneAreaCodePreferred ), Model_Insured.Employment, employmentView.PhoneNumberControl, EmploymentAreaCodePreferredEventHandler, compRuleType );
                RuleEngine.GetInstance().RegisterEvent( typeof( EmploymentPhoneNumberPreferred ), Model_Insured.Employment, employmentView.PhoneNumberControl, EmploymentPhoneNumberPreferredEventHandler, compRuleType );

                RuleEngine.GetInstance().RegisterEvent( typeof( InvalidRelForPriInsured ), Model_Account, new EventHandler( InvalidRelForPriInsuredEventHandler ), compRuleType );
                RuleEngine.GetInstance().RegisterEvent( typeof( InvalidRelForPriInsuredChange ), Model_Account, new EventHandler( InvalidRelForPriInsuredChangeEventHandler ), compRuleType );
                RuleEngine.GetInstance().RegisterEvent( typeof( InvalidGenderForPriInsured ), Model_Account, new EventHandler( InvalidGenderForPriInsuredEventHandler ), compRuleType );
                RuleEngine.GetInstance().RegisterEvent( typeof( InvalidGenderForPriInsuredChange ), Model_Account, new EventHandler( InvalidGenderForPriInsuredChangeEventHandler ), compRuleType );
                RuleEngine.GetInstance().RegisterEvent( typeof( InvalidEmpStatusForPriInsured ), Model_Account, new EventHandler( InvalidEmpStatusForPriInsuredEventHandler ), compRuleType );
                RuleEngine.GetInstance().RegisterEvent( typeof( InvalidEmpStatusForPriInsuredChange ), Model_Account, new EventHandler( InvalidEmpStatusForPriInsuredChangeEventHandler ), compRuleType );

                RuleEngine.GetInstance().RegisterEvent( typeof( InvalidRelForSecInsured ), Model_Account, new EventHandler( InvalidRelForSecInsuredEventHandler ), compRuleType );
                RuleEngine.GetInstance().RegisterEvent( typeof( InvalidRelForSecInsuredChange ), Model_Account, new EventHandler( InvalidRelForSecInsuredChangeEventHandler ), compRuleType );
                RuleEngine.GetInstance().RegisterEvent( typeof( InvalidGenderForSecInsured ), Model_Account, new EventHandler( InvalidGenderForSecInsuredEventHandler ), compRuleType );
                RuleEngine.GetInstance().RegisterEvent( typeof( InvalidGenderForSecInsuredChange ), Model_Account, new EventHandler( InvalidGenderForSecInsuredChangeEventHandler ), compRuleType );
                RuleEngine.GetInstance().RegisterEvent( typeof( InvalidEmpStatusForSecInsured ), Model_Account, new EventHandler( InvalidEmpStatusForSecInsuredEventHandler ), compRuleType );
                RuleEngine.GetInstance().RegisterEvent( typeof( InvalidEmpStatusForSecInsuredChange ), Model_Account, new EventHandler( InvalidEmpStatusForSecInsuredChangeEventHandler ), compRuleType );
            }
        }

        private void UnregisterEventHandlers()
        {
            i_Registered = false;

            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonRelationshipRequired ), Model_Insured, PersonRelationshipRequiredEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonLastNameRequired ), Model_Insured, PersonLastNameRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonFirstNameRequired ), Model_Insured, PersonFirstNameRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonGenderRequired ), Model_Insured, PersonGenderRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InsuredAddressRequired ), Model_Insured, addressView.AddressRequiredEventHandler );



            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonEmploymentStatusRequired ), Model_Insured, PersonEmploymentStatusRequiredEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonEmployerAddressRequired ), Model_Insured, PersonEmployerAddressRequiredEventHandler );

            if ( Model_Coverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID )
            {
                RuleEngine.GetInstance().UnregisterEvent( typeof( PrimaryInsuredDateOfBirthRequired ), Model_Account, PersonDateOfBirthRequiredEventHandler );
                RuleEngine.GetInstance().UnregisterEvent( typeof( PrimaryInsuredDateOfBirthPreferred ), Model_Account, PersonDateOfBirthPreferredEventHandler );
            }
            else
            {
                RuleEngine.GetInstance().UnregisterEvent( typeof( SecondaryInsuredDateOfBirthRequired ), Model_Account, PersonDateOfBirthRequiredEventHandler );
                RuleEngine.GetInstance().UnregisterEvent( typeof( SecondaryInsuredDateOfBirthPreferred ), Model_Account, PersonDateOfBirthPreferredEventHandler );
            }

            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonRelationshipPreferred ), Model_Insured, PersonRelationshipPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonLastNamePreferred ), Model_Insured, PersonLastNamePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonFirstNamePreferred ), Model_Insured, PersonFirstNamePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonGenderPreferred ), Model_Insured, PersonGenderPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InsuredAddressPreferred ), Model_Insured, addressView.AddressPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonPhoneNumberPreferred ), Model_Insured, addressView.PhonePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonPhoneAreaCodePreferred ), Model_Insured, addressView.AreaCodePreferredEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonEmploymentStatusPreferred ), Model_Insured, PersonEmploymentStatusPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InsuredEmployerRequired ), Model_Insured, InsuredEmployerRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PersonEmployerAddressPreferred ), Model_Insured, PersonEmployerAddressPreferredEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( EmploymentPhoneAreaCodePreferred ), Model_Insured.Employment, EmploymentAreaCodePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( EmploymentPhoneNumberPreferred ), Model_Insured.Employment, EmploymentPhoneNumberPreferredEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidRelForPriInsured ), Model_Account, InvalidRelForPriInsuredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidRelForPriInsuredChange ), Model_Account, InvalidRelForPriInsuredChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidGenderForPriInsured ), Model_Account, InvalidGenderForPriInsuredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidGenderForPriInsuredChange ), Model_Account, InvalidGenderForPriInsuredChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidEmpStatusForPriInsured ), Model_Account, InvalidEmpStatusForPriInsuredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidEmpStatusForPriInsuredChange ), Model_Account, InvalidEmpStatusForPriInsuredChangeEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidRelForSecInsured ), Model_Account, InvalidRelForSecInsuredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidRelForSecInsuredChange ), Model_Account, InvalidRelForSecInsuredChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidGenderForSecInsured ), Model_Account, InvalidGenderForSecInsuredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidGenderForSecInsuredChange ), Model_Account, InvalidGenderForSecInsuredChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidEmpStatusForSecInsured ), Model_Account, InvalidEmpStatusForSecInsuredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidEmpStatusForSecInsuredChange ), Model_Account, InvalidEmpStatusForSecInsuredChangeEventHandler );
        }
        private bool ValidateDateOfBirth()
        {
            if( mtbDob.UnMaskedText.Length == 0 )
            {
                dobDate = DateTime.MinValue;
                return true;
            }
            
            if( mtbDob.Text.Length != 10 )
            {
                mtbDob.Focus();
                UIColors.SetErrorBgColor( mtbDob );
                MessageBox.Show( UIErrorMessages.DOB_INCOMPLETE_ERRMSG, "Error",
                                 MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                 MessageBoxDefaultButton.Button1 );
                return false;
            }
            
            try
            {
                string month = mtbDob.Text.Substring( 0, 2 );
                string day = mtbDob.Text.Substring( 3, 2 );
                string year = mtbDob.Text.Substring( 6, 4 );

                dobDate = new DateTime( Convert.ToInt32( year ),
                                        Convert.ToInt32( month ),
                                        Convert.ToInt32( day ) );

                if ( IsDOBBefore1800 )
                {
                    mtbDob.Focus();
                    UIColors.SetErrorBgColor(mtbDob);
                    MessageBox.Show(UIErrorMessages.DOB_OUT_OF_RANGE, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);
                    return false;
                }

                if( dobDate > DateTime.Today )
                {
                    // Remove the Admit Time Leave handler so error isn't generated
                    // when user comes back to the time field to correct the error.
                    mtbDob.Focus();
                    UIColors.SetErrorBgColor( mtbDob );
                    MessageBox.Show( UIErrorMessages.DOB_FUTURE_ERRMSG, "Error",
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button1 );
                    return false;
                }
                
                if( DateValidator.IsValidDate( dobDate ) == false )
                {
                    mtbDob.Focus();
                    UIColors.SetErrorBgColor( mtbDob );
                    MessageBox.Show( UIErrorMessages.DOB_NOTVALID_ERRMSG, "Error",
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button1 );
                    return false;
                }
            }
            catch
            {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                // an invalid year, month, or day.  Simply set field to error color.
                mtbDob.Focus();
                UIColors.SetErrorBgColor( mtbDob );
                MessageBox.Show( UIErrorMessages.DOB_NOTVALID_ERRMSG, "Error",
                                 MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                 MessageBoxDefaultButton.Button1 );
                return false;
            }
            return true;
        }

        private void ResetView( bool resetCopyParty )
        {
            Model_Insured = new Insured();
            Model_Coverage.Insured = Model_Insured;
            addressView.ResetView();

            if( resetCopyParty )
            {
                copyPartyView.ResetView();
            }

            employmentView.ResetView( resetCopyParty );
            relationshipView.ResetView();
            i_OriginalRelationship = null;
            genderControl.ResetView();
            suffixPresenter.ClearSuffix();
            mtbNationalID.UnMaskedText = String.Empty;
            mtbLastName.UnMaskedText = String.Empty;
            mtbFirstName.UnMaskedText = String.Empty;
            mtbMiddleInitial.UnMaskedText = String.Empty;
            mtbDob.UnMaskedText = String.Empty;
            dobDate = DateTime.MinValue;

            RegisterEventHandlers();
            RunRules();
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureFirstNameAndLastName( mtbFirstName );
            MaskedEditTextBoxBuilder.ConfigureFirstNameAndLastName( mtbLastName );
            MaskedEditTextBoxBuilder.ConfigureMIAndSuffix( mtbMiddleInitial );
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpName = new System.Windows.Forms.GroupBox();
            this.lblStaticMI = new System.Windows.Forms.Label();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.lblStaticLastName = new System.Windows.Forms.Label();
            this.mtbLastName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbFirstName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbMiddleInitial = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.suffixView = new SuffixView();
            this.lblStaticGender = new System.Windows.Forms.Label();
            this.lblStaticNatlID = new System.Windows.Forms.Label();
            this.relationshipView = new PatientAccess.UI.CommonControls.RelationshipView();
            this.addressView = new PatientAccess.UI.AddressViews.AddressView();
            this.employmentView = new PatientAccess.UI.CommonControls.EmploymentView();
            this.btnReset = new LoggingButton();
            this.genderControl = new PatientAccess.UI.CommonControls.GenderControl();
            this.lblStaticDateOfBirth = new System.Windows.Forms.Label();
            this.lblStaticCopyFrom = new System.Windows.Forms.Label();
            this.mtbNationalID = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.copyPartyView = new PatientAccess.UI.GuarantorViews.CopyPartyView();
            this.mtbDob = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.grpName.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpName
            // 
            this.grpName.Controls.Add( this.lblStaticMI );
            this.grpName.Controls.Add( this.lblFirstName );
            this.grpName.Controls.Add( this.lblStaticLastName );
            this.grpName.Controls.Add( this.mtbLastName );
            this.grpName.Controls.Add( this.mtbFirstName );
            this.grpName.Controls.Add( this.mtbMiddleInitial );
            this.grpName.Controls.Add(this.suffixView);
            this.grpName.Location = new System.Drawing.Point( 15, 52 );
            this.grpName.Name = "grpName";
            this.grpName.Size = new System.Drawing.Size( 640, 55 );
            this.grpName.TabIndex = 3;
            this.grpName.TabStop = false;
            this.grpName.Text = "Name";
            // 
            // lblStaticMI
            // 
            this.lblStaticMI.Location = new System.Drawing.Point( 509, 24 );
            this.lblStaticMI.Name = "lblStaticMI";
            this.lblStaticMI.Size = new System.Drawing.Size( 21, 23 );
            this.lblStaticMI.TabIndex = 4;
            this.lblStaticMI.Text = "MI:";
            // 
            // lblFirstName
            // 
            this.lblFirstName.Location = new System.Drawing.Point( 304, 24 );
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size( 30, 23 );
            this.lblFirstName.TabIndex = 2;
            this.lblFirstName.Text = "First:";
            // 
            // lblStaticLastName
            // 
            this.lblStaticLastName.Location = new System.Drawing.Point( 9, 24 );
            this.lblStaticLastName.Name = "lblStaticLastName";
            this.lblStaticLastName.Size = new System.Drawing.Size( 29, 23 );
            this.lblStaticLastName.TabIndex = 0;
            this.lblStaticLastName.Text = "Last:";
            // 
            // mtbLastName
            // 
            this.mtbLastName.AcceptsTab = true;
            this.mtbLastName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbLastName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbLastName.Location = new System.Drawing.Point( 38, 21 );
            this.mtbLastName.Mask = "";
            this.mtbLastName.MaxLength = 25;
            this.mtbLastName.Name = "mtbLastName";
            this.mtbLastName.Size = new System.Drawing.Size( 257, 20 );
            this.mtbLastName.TabIndex = 1;
            this.mtbLastName.Validating += new System.ComponentModel.CancelEventHandler( this.mtbLastName_Validating );
            // 
            // mtbFirstName
            // 
            this.mtbFirstName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbFirstName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbFirstName.Location = new System.Drawing.Point( 334, 21 );
            this.mtbFirstName.Mask = "";
            this.mtbFirstName.MaxLength = 13;
            this.mtbFirstName.Name = "mtbFirstName";
            this.mtbFirstName.Size = new System.Drawing.Size( 162, 20 );
            this.mtbFirstName.TabIndex = 2;
            this.mtbFirstName.Validating += new System.ComponentModel.CancelEventHandler( this.mtbFirstName_Validating );
            // 
            // mtbMiddleInitial
            // 
            this.mtbMiddleInitial.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbMiddleInitial.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbMiddleInitial.Location = new System.Drawing.Point( 530, 21 );
            this.mtbMiddleInitial.Mask = "";
            this.mtbMiddleInitial.MaxLength = 1;
            this.mtbMiddleInitial.Name = "mtbMiddleInitial";
            this.mtbMiddleInitial.Size = new System.Drawing.Size( 18, 20 );
            this.mtbMiddleInitial.TabIndex = 3;
            this.mtbMiddleInitial.Validating += new System.ComponentModel.CancelEventHandler( this.mtbMiddleInitial_Validating );

            this.suffixView.Location = new System.Drawing.Point(551, 19);
            this.suffixView.Name = "Suffix View";
            this.suffixView.Size = new System.Drawing.Size(86, 27);
            this.suffixView.TabIndex = 6;
            this.suffixView.Visible = true;
            // 
            // lblStaticGender
            // 
            this.lblStaticGender.Location = new System.Drawing.Point( 662, 19 );
            this.lblStaticGender.Name = "lblStaticGender";
            this.lblStaticGender.Size = new System.Drawing.Size( 44, 23 );
            this.lblStaticGender.TabIndex = 0;
            this.lblStaticGender.Text = "Gender:";
            // 
            // lblStaticNatlID
            // 
            this.lblStaticNatlID.Location = new System.Drawing.Point( 662, 83 );
            this.lblStaticNatlID.Name = "lblStaticNatlID";
            this.lblStaticNatlID.Size = new System.Drawing.Size( 65, 23 );
            this.lblStaticNatlID.TabIndex = 0;
            this.lblStaticNatlID.Text = "National ID:";
            // 
            // relationshipView
            // 
            this.relationshipView.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (System.Byte)( 0 ) ) );
            this.relationshipView.LabelText = "The Patient is the Insured's:";
            this.relationshipView.Location = new System.Drawing.Point( 313, 15 );
            this.relationshipView.Model = null;
            this.relationshipView.Name = "relationshipView";
            this.relationshipView.PartyForRelationships = null;
            this.relationshipView.PatientIs = null;
            this.relationshipView.RelationshipName = null;
            this.relationshipView.Size = new System.Drawing.Size( 336, 24 );
            this.relationshipView.TabIndex = 2;
            this.relationshipView.RelationshipSelected += new System.EventHandler( this.relationshipView_RelationshipSelected );
            this.relationshipView.RelationshipValidating += new System.ComponentModel.CancelEventHandler( this.relationshipView_RelationshipValidating );
            // 
            // addressView
            // 
            this.addressView.Context = null;
            this.addressView.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (System.Byte)( 0 ) ) );
            this.addressView.KindOfTargetParty = null;
            this.addressView.Location = new System.Drawing.Point( 15, 115 );
            this.addressView.Model = null;
            this.addressView.Mode = AddressViews.AddressView.AddressMode.PHONECELL;
            this.addressView.ShowStatus = false;
            this.addressView.Name = "addressView";
            this.addressView.PatientAccount = null;
            this.addressView.TabIndex = 7;
            this.addressView.IsAddressWithStreet2 = true;
            this.addressView.AddressChanged += new System.EventHandler( this.AddressChangedEventHandler );
            this.addressView.AreaCodeChanged += new EventHandler( this.AddressView_AreaCodeChanged );
            this.addressView.PhoneNumberChanged += new EventHandler( this.AddressView_PhoneNumberChanged );
            this.addressView.CellPhoneNumberChanged += new EventHandler( this.AddressView_CellPhoneNumberChanged );
            // 
            // employmentView
            // 
            this.employmentView.Location = new System.Drawing.Point( 305, 115 );
            this.employmentView.Model = null;
            this.employmentView.Model_Account = null;
            this.employmentView.Model_Employment = null;
            this.employmentView.Name = "employmentView";
            this.employmentView.Size = new System.Drawing.Size( 360, 225 );
            this.employmentView.TabIndex = 9;
            this.employmentView.EmploymentViewChangedEvent += new System.EventHandler( this.employmentView_EmploymentViewChangedEvent );
            this.employmentView.EmploymentStatusValidating += new System.ComponentModel.CancelEventHandler( this.employmentView_EmploymentStatusValidating );
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point( 708, 354 );
            this.btnReset.Name = "btnReset";
            this.btnReset.TabIndex = 10;
            this.btnReset.Text = "Clear All";
            this.btnReset.Click += new System.EventHandler( this.ResetButtonClick );
            // 
            // genderControl
            // 
            this.genderControl.Location = new System.Drawing.Point( 732, 16 );
            this.genderControl.Model = null;
            this.genderControl.Name = "genderControl";
            this.genderControl.Size = new System.Drawing.Size( 76, 21 );
            this.genderControl.TabIndex = 4;
            this.genderControl.GenderSelectedEvent += new System.EventHandler( this.GenderSelectedEvent );
            this.genderControl.GenderControlValidating += new System.ComponentModel.CancelEventHandler( this.genderControl_GenderControlValidating );
            // 
            // lblStaticDateOfBirth
            // 
            this.lblStaticDateOfBirth.Location = new System.Drawing.Point( 662, 51 );
            this.lblStaticDateOfBirth.Name = "lblStaticDateOfBirth";
            this.lblStaticDateOfBirth.Size = new System.Drawing.Size( 71, 16 );
            this.lblStaticDateOfBirth.TabIndex = 0;
            this.lblStaticDateOfBirth.Text = "DOB:";
            // 
            // lblStaticCopyFrom
            // 
            this.lblStaticCopyFrom.Location = new System.Drawing.Point( 18, 20 );
            this.lblStaticCopyFrom.Name = "lblStaticCopyFrom";
            this.lblStaticCopyFrom.Size = new System.Drawing.Size( 121, 23 );
            this.lblStaticCopyFrom.TabIndex = 0;
            this.lblStaticCopyFrom.Text = "Copy to Insured from:";
            // 
            // mtbNationalID
            // 
            this.mtbNationalID.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbNationalID.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbNationalID.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbNationalID.Location = new System.Drawing.Point( 732, 81 );
            this.mtbNationalID.Mask = "";
            this.mtbNationalID.MaxLength = 12;
            this.mtbNationalID.Name = "mtbNationalID";
            this.mtbNationalID.Size = new System.Drawing.Size( 88, 20 );
            this.mtbNationalID.TabIndex = 6;
            this.mtbNationalID.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbNationalID.Validating += new System.ComponentModel.CancelEventHandler( this.mtbNationalID_Validating );
            // 
            // copyPartyView
            //             
            this.copyPartyView.KindOfTargetParty = null;
            this.copyPartyView.Location = new System.Drawing.Point( 128, 15 );
            this.copyPartyView.Model = null;
            this.copyPartyView.Name = "copyPartyView";
            this.copyPartyView.Size = new System.Drawing.Size( 178, 24 );
            this.copyPartyView.TabIndex = 1;
            this.copyPartyView.PartySelected += new System.EventHandler( this.PartySelectedEvent );
            // 
            // mtbDob
            // 
            this.mtbDob.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtStart;
            this.mtbDob.KeyPressExpression = "^\\d*$";
            this.mtbDob.Location = new System.Drawing.Point( 732, 48 );
            this.mtbDob.Mask = "  /  /    ";
            this.mtbDob.MaxLength = 10;
            this.mtbDob.Name = "mtbDob";
            this.mtbDob.Size = new System.Drawing.Size( 70, 20 );
            this.mtbDob.TabIndex = 5;
            this.mtbDob.ValidationExpression = "^\\d*$";
            this.mtbDob.Validating += new System.ComponentModel.CancelEventHandler( this.mtbDob_Validating );
            this.mtbDob.Leave += new EventHandler( mtbDob_Leave );
            // 
            // InsDetailInsuredView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.mtbDob );
            this.Controls.Add( this.copyPartyView );
            this.Controls.Add( this.mtbNationalID );
            this.Controls.Add( this.lblStaticCopyFrom );
            this.Controls.Add( this.lblStaticDateOfBirth );
            this.Controls.Add( this.genderControl );
            this.Controls.Add( this.btnReset );
            this.Controls.Add( this.employmentView );
            this.Controls.Add( this.addressView );
            this.Controls.Add( this.relationshipView );
            this.Controls.Add( this.lblStaticNatlID );
            this.Controls.Add( this.lblStaticGender );
            this.Controls.Add( this.grpName );
            this.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (System.Byte)( 0 ) ) );
            this.Name = "InsDetailInsuredView";
            this.Size = new System.Drawing.Size( 880, 450 );
            this.Disposed += new System.EventHandler( this.InsDetailInsuredView_Disposed );
            this.Leave += new System.EventHandler( this.InsDetailInsuredView_Leave );
            this.grpName.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion

        #endregion

        #region Construction and Finalization
        public InsDetailInsuredView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            ConfigureControls();

            EnableThemesOn( this );
            //            savedInsuredData = new Insured();
            addressView.EditAddressButtonText = "Edit &Address....";
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
        private Container components = null;

        private LoggingButton btnReset;

        private Label lblFirstName;
        private Label lblStaticMI;
        private Label lblStaticCopyFrom;
        private Label lblStaticLastName;
        private Label lblStaticGender;
        private Label lblStaticNatlID;
        private Label lblStaticDateOfBirth;

        private GroupBox grpName;

        private MaskedEditTextBox mtbDob;
        private MaskedEditTextBox mtbFirstName;
        private MaskedEditTextBox mtbLastName;
        private MaskedEditTextBox mtbMiddleInitial;
        private SuffixView suffixView;
        private MaskedEditTextBox mtbNationalID;

        private AddressView addressView;
        private CopyPartyView copyPartyView;
        private EmploymentView employmentView;
        private GenderControl genderControl;
        private RelationshipView relationshipView;

        private Account i_Account;
        private Coverage i_Model_Coverage;
        private DateTime dobDate;
        private bool i_Registered;
        private Relationship i_OriginalRelationship;
        
        private bool blnLeaveRun;

        #endregion

        #region Constants
        private const string
            SELF = "Self",
            EMPLOYEE = "Employee";
        private string prevEmployerName = string.Empty;
        private string prevEmploymentStatusCode = string.Empty;
        private SuffixPresenter suffixPresenter;
        private static readonly DateTime earliestDateOfBirth = new DateTime(1800, 01, 01);
        #endregion
    }
}
