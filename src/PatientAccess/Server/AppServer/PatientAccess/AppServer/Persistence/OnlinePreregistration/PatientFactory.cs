using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Messaging;

namespace PatientAccess.Persistence.OnlinePreregistration
{
    public class PatientFactory
    {

        #region Constants

        private const string CODE = "CODE_";
        private const string UNDER_SCORE = "_";

        #endregion

        #region Methods

        public Patient BuildNewPatient()
        {
            BuildDemographicsForNewPatient();
            patient.IsNew = true;
            BuildAccount();

            return patient;
        }

        public Patient UpdateExistingPatientWithNewAccount( Patient existingPatient )
        {
            patient = existingPatient;
            patient.IsNew = false;

            BuildAccount();
            
            // Get certain Demographics information from the existing patient
            accountBroker.AddDemographicDataTo( patient.SelectedAccount );

            // Update certain Demographics information from the Online form data and leave the rest as is.
            UpdateDemographicsForExistingPatient();

            return patient;
        }

        private void BuildDemographicsForNewPatient()
        {
            patient.FirstName = PatientType.name.first;
            patient.LastName = PatientType.name.last;
            patient.MiddleInitial = PatientType.name.middleInitial ?? string.Empty;

            patient.PlaceOfBirth = PatientType.placeOfBirth ?? string.Empty;
            patient.DateOfBirth = PatientType.dateOfBirth;

            patient.SocialSecurityNumber = ( PatientType.socialSecurityNumber != null ) ? 
                new SocialSecurityNumber( PatientType.socialSecurityNumber ) : new SocialSecurityNumber();

            patient.Sex = demographicsBroker.GenderWith( Facility.Oid, EnumToCode( PatientType.gender ) );

            patient.MaritalStatus = demographicsBroker.MaritalStatusWith( Facility.Oid,
                                                                          EnumToCode( PatientType.maritalStatus ) );

            patient.Race = originBroker.RaceWith( Facility.Oid, EnumToCode( PatientType.race ) );

            patient.Ethnicity = PatientType.ethnicitySpecified ? 
                originBroker.EthnicityWith( Facility.Oid, EnumToCode( PatientType.ethnicity ) ) : new Ethnicity();

            patient.Religion = PatientType.religiousPreferenceSpecified ?
                religionBroker.ReligionWith( Facility.Oid, EnumToCode( PatientType.religiousPreference ) ) : new Religion();

            BuildLanguage();

            ContactPoint mailingContactPoint = 
                ContactPointFactory.BuildContactPoint( PatientType, TypeOfContactPoint.NewMailingContactPointType() );

            ContactPoint mobileContactPoint = ContactPointFactory.BuildMobileContactPoint( PatientType );

            patient.AddContactPoint( mailingContactPoint );
            patient.AddContactPoint( mobileContactPoint ); 
        }

        /// <summary>
        /// Update certain Demographics information from the received
        /// eTenet Online form data and leave the rest as is.
        /// </summary>
        /// <returns></returns> 
        internal Patient UpdateDemographicsForExistingPatient()
        {
            patient.FirstName = PatientType.name.first;
            patient.LastName = PatientType.name.last;
            patient.MiddleInitial = PatientType.name.middleInitial ?? string.Empty;

            patient.MaritalStatus = demographicsBroker.MaritalStatusWith( Facility.Oid,
                                                                          EnumToCode( PatientType.maritalStatus ) );

            patient.Religion = PatientType.religiousPreferenceSpecified ?
                religionBroker.ReligionWith( Facility.Oid, EnumToCode( PatientType.religiousPreference ) ) : new Religion();

            // Remove existing patient contact points like Mailing Address, Physical Address and Phone Numbers
            patient.RemoveContactPoint( TypeOfContactPoint.NewMailingContactPointType() );
            patient.RemoveContactPoint( TypeOfContactPoint.NewPhysicalContactPointType() );
            patient.RemoveContactPoint( TypeOfContactPoint.NewMobileContactPointType() );

            ContactPoint mailingContactPoint =
                ContactPointFactory.BuildContactPoint( PatientType, TypeOfContactPoint.NewMailingContactPointType() );

            ContactPoint mobileContactPoint = ContactPointFactory.BuildMobileContactPoint( PatientType );

            patient.AddContactPoint( mailingContactPoint );
            patient.AddContactPoint( mobileContactPoint );

            return patient;
        }

        private void BuildAccount()
        {
            Account account = new Account
            {
                AdmitDate = GetAdmitDateAndTimeFrom( VisitType.expectedAdmissionDateTime ),
                Diagnosis = { Procedure = VisitType.procedureTreatmentOrTest }
            };

            BuildGuarantor( account );
            account.EmergencyContact1 = BuildEmergencyContact( account, PatientType.emergencyContact1 );
            account.EmergencyContact2 = BuildEmergencyContact( account, PatientType.emergencyContact2 );

            // Default Insurance for UnInsured
            if ( ( VisitType.insuranceInformation == null ) ||
                 ( VisitType.insuranceInformation.primaryInsurance == null &&
                   VisitType.insuranceInformation.secondaryInsurance == null ) )
            {
                Coverage coverage = InsuranceCoverageFactory.BuildDefaultPrimaryInsuranceCoverageForUnInsured( 
                    account.AdmitDate, account.Patient.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() ).Address );

                account.Insurance.AddCoverage( coverage );
                account.FinancialClass = financialClassBroker.FinancialClassWith( Facility.Oid, FinancialClass.UNINSURED_CODE );
            }

            account.IsNew = true;
            account.Facility = Facility;
            account.OnlinePreRegistered = true;
            patient.SelectedAccount = account;
            account.Patient = patient; 
        }

        private void BuildLanguage()
        {
            Language language = new Language();

            if ( PatientType.preferredLanguage != null )
            {
                if ( PatientType.preferredLanguage.Item is languageType )
                {
                    language = demographicsBroker.LanguageWith( Facility.Oid,
                                    EnumToCode( ( languageType )PatientType.preferredLanguage.Item ) );
                }
                else if ( PatientType.preferredLanguage.Item is string )
                {
                    language = demographicsBroker.LanguageWith( Facility.Oid, Language.OTHER_LANGUAGE_CODE );
                    patient.OtherLanguage = ( string )PatientType.preferredLanguage.Item;
                }
            }

            patient.Language = language;
        }

        private void BuildGuarantor( Account account )
        {
            guarantorType guarantorType = PatientType.guarantor;

            Guarantor guarantor = new Guarantor
            {
                FirstName = guarantorType.name.first,
                LastName = guarantorType.name.last,
                SocialSecurityNumber = ( guarantorType.socialSecurityNumber != null ) ? 
                    new SocialSecurityNumber( guarantorType.socialSecurityNumber ) : new SocialSecurityNumber()
            };

            ContactPoint contactPoint = 
                ContactPointFactory.BuildContactPoint( guarantorType, TypeOfContactPoint.NewMailingContactPointType() );
            guarantor.AddContactPoint( contactPoint );

            string relationshipCode = EnumToCode( PatientType.guarantor.relationship );
            RelationshipType aRelationshipType = relationshipTypeBroker.RelationshipTypeWith( Facility.Oid, relationshipCode );
            account.GuarantorIs( guarantor, aRelationshipType );
        }

        private EmergencyContact BuildEmergencyContact( Account account, emergencyContactType emergencyContactType )
        {
            EmergencyContact emergencyContact = EmergencyContactFactory.BuildEmergencyContact( emergencyContactType, account.Patient.GetType() );

            ArrayList emergencyRelationships = emergencyContact.FindRelationships( emergencyContact.RelationshipType );
            account.Patient.RemoveRelationship( (Relationship)emergencyRelationships[0] );
            account.Patient.AddRelationship( ( Relationship )emergencyRelationships[0] );  //add to patient directly

            return emergencyContact;
        }

        public static string SplitEnumValueAsCode( string input )
        {
            input = input.Replace( CODE, string.Empty );
            int endPoint = input.IndexOf( UNDER_SCORE );

            string code = input.Substring( 0, endPoint );
            return code;
        }

        public static string EnumToCode( genderType enumValue )
        {
            return SplitEnumValueAsCode( enumValue.ToString() );
        }

        public static string EnumToCode( maritalStatusType enumValue )
        {
            return SplitEnumValueAsCode( enumValue.ToString() );
        }

        public static string EnumToCode( raceType enumValue )
        {
            return SplitEnumValueAsCode( enumValue.ToString() );
        }

        public static string EnumToCode( ethnicityType enumValue )
        {
            return SplitEnumValueAsCode( enumValue.ToString() );
        }

        public static string EnumToCode( languageType enumValue )
        {
            return SplitEnumValueAsCode( enumValue.ToString() );
        }

        public static string EnumToCode( religionType enumValue )
        {
            return SplitEnumValueAsCode( enumValue.ToString() );
        }

        public static string EnumToCode( relationshipType enumValue )
        {
            return SplitEnumValueAsCode( enumValue.ToString() );
        }

        public static string EnumToCode( fullTimePartTimeMilitaryOrOtherEmploymentStatusType enumValue )
        {
            return SplitEnumValueAsCode( enumValue.ToString() );
        }

        public static string EnumToCode( retiredNotEmployedOrSelfEmployedStatusType enumValue )
        {
            return SplitEnumValueAsCode( enumValue.ToString() );
        }

        private static DateTime GetAdmitDateAndTimeFrom( DateTime date )
        {
            string dateTimeString = date.ToString( "MMddyyyy hhmm" );

            string dateText = dateTimeString.Substring( 0, 8 );
            string timeText = dateTimeString.Substring( 9 );

            DateTime theDate;
            int month = Convert.ToInt32( dateText.Substring( 0, 2 ) );
            int day = Convert.ToInt32( dateText.Substring( 2, 2 ) );
            int year = Convert.ToInt32( dateText.Substring( 4, 4 ) );

            int hour = 0;
            int minute = 0;

            if ( timeText.Length == 4 )
            {
                hour = Convert.ToInt32( timeText.Substring( 0, 2 ) );
                minute = Convert.ToInt32( timeText.Substring( 2, 2 ) );
            }

            if ( ( hour >= 0 && hour <= 23 ) && ( minute >= 0 && minute <= 59 ) )
            {
                theDate = new DateTime( year, month, day, hour, minute, 0 );
            }
            else
            {
                theDate = new DateTime( year, month, day );
            }
            return theDate;
        }

        #endregion

        #region Properties

        private preRegistration PreRegistrationData { get; set; }

        internal Patient Patient
        {
            get { return patient; }
            set { patient = value; }
        }

        private patientType PatientType
        {
            get { return PreRegistrationData.patient; }
        }

        private visitType VisitType
        {
            get { return PreRegistrationData.visit; }
        }

        private Facility Facility
        {
            get { return i_Facility ?? ( i_Facility = facilityBroker.FacilityWith( PreRegistrationData.facility.code ) ); }
        }

        private ContactPointFactory ContactPointFactory
        {
            get { return contactPointFactory; }
        }

        private EmergencyContactFactory EmergencyContactFactory
        {
            get { return emergencyContactFactory; }
        }

        private InsuranceCoverageFactory InsuranceCoverageFactory
        {
            get { return insuranceCoverageFactory; }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public PatientFactory( preRegistration preRegistration )
        {
            PreRegistrationData = preRegistration;
            contactPointFactory = new ContactPointFactory( Facility );
            emergencyContactFactory = new EmergencyContactFactory( Facility );
            insuranceCoverageFactory = new InsuranceCoverageFactory( Facility );
        }

        #endregion

        #region Data Elements
        private Facility i_Facility;
        private Patient patient = new Patient();
        private readonly ContactPointFactory contactPointFactory;
        private readonly EmergencyContactFactory emergencyContactFactory;
        private readonly InsuranceCoverageFactory insuranceCoverageFactory;

        private readonly IAccountBroker accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
        private readonly IDemographicsBroker demographicsBroker = BrokerFactory.BrokerOfType<IDemographicsBroker>();
        private readonly IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
        private readonly IRelationshipTypeBroker relationshipTypeBroker = BrokerFactory.BrokerOfType<IRelationshipTypeBroker>();
        private readonly IReligionBroker religionBroker = BrokerFactory.BrokerOfType<IReligionBroker>();
        private readonly IOriginBroker originBroker = BrokerFactory.BrokerOfType<IOriginBroker>();
        private readonly IFinancialClassesBroker financialClassBroker = BrokerFactory.BrokerOfType<IFinancialClassesBroker>();

        #endregion


    }
}
