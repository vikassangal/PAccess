using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Messaging;
using PatientAccess.Persistence.OnlinePreregistration;
using NUnit.Framework;
using Tests.Utilities.OnlinePreRegistration;

namespace Tests.Unit.PatientAccess.Services.PreRegistration
{
    [TestFixture]
    public class PatientFactoryTests
    {
        private const string PatientAccessResourcesSamplePreRegistrationXml = "Tests.Resources.OnlinePreRegistration.SamplePreRegistration.xml";

        # region Test Methods

        [Test]
        public void TestBuildPatientDemographics_WithValidXml()
        {
            var registrationData = MessageBuilder.GetPreRegistrationData( PatientAccessResourcesSamplePreRegistrationXml );
            var patientFactory = new PatientFactory( registrationData );

            Patient patient = patientFactory.BuildNewPatient();

            Assert.IsTrue( patient.FirstName == registrationData.patient.name.first );
            Assert.IsTrue( patient.LastName == registrationData.patient.name.last );
            Assert.IsTrue( patient.MiddleInitial == registrationData.patient.name.middleInitial );
            Assert.IsTrue( patient.PlaceOfBirth == registrationData.patient.placeOfBirth );
            Assert.IsTrue( patient.DateOfBirth == registrationData.patient.dateOfBirth );
            Assert.IsTrue( patient.SocialSecurityNumber == new SocialSecurityNumber( registrationData.patient.socialSecurityNumber ) );
            Assert.IsTrue( patient.Sex.Code == PatientFactory.EnumToCode( registrationData.patient.gender ) );
            Assert.IsTrue( patient.MaritalStatus.Code == PatientFactory.EnumToCode( registrationData.patient.maritalStatus ) );
            Assert.IsTrue( patient.Race.Code == PatientFactory.EnumToCode( registrationData.patient.race ) );
            Assert.IsTrue( patient.Ethnicity.Code == PatientFactory.EnumToCode( registrationData.patient.ethnicity ) );
            Assert.IsTrue( patient.Religion.Code == PatientFactory.EnumToCode( registrationData.patient.religiousPreference ) );
            Assert.IsTrue( patient.Language.Code == PatientFactory.EnumToCode( (languageType)registrationData.patient.preferredLanguage.Item ) );
        }

        [Test]
        public void TestUpdateExistingPatientWithNewAccount()
        {
            Patient existingPatient = BuildExistingPatient();

            var registrationData = MessageBuilder.GetPreRegistrationData( PatientAccessResourcesSamplePreRegistrationXml );

            var patientFactory = new PatientFactory( registrationData );

            patientFactory.Patient = existingPatient;
            Patient newPatient = patientFactory.UpdateDemographicsForExistingPatient();

            Assert.IsTrue( newPatient.FirstName == registrationData.patient.name.first );
            Assert.IsTrue( newPatient.LastName == registrationData.patient.name.last );
            Assert.IsTrue( newPatient.MiddleInitial == registrationData.patient.name.middleInitial );
            Assert.IsTrue( newPatient.PlaceOfBirth == existingPatient.PlaceOfBirth );
            Assert.IsTrue( newPatient.DateOfBirth == existingPatient.DateOfBirth );
            Assert.IsTrue( newPatient.SocialSecurityNumber == existingPatient.SocialSecurityNumber );
            Assert.IsTrue( newPatient.Sex.Code == existingPatient.Sex.Code );
            Assert.IsTrue( newPatient.MaritalStatus.Code == PatientFactory.EnumToCode( registrationData.patient.maritalStatus ) );
            Assert.IsTrue( newPatient.Race.Code == existingPatient.Race.Code );
            Assert.IsTrue( newPatient.Ethnicity.Code == existingPatient.Ethnicity.Code );
            Assert.IsTrue( newPatient.Religion.Code == PatientFactory.EnumToCode( registrationData.patient.religiousPreference ) );
            Assert.IsTrue( newPatient.Language.Code == existingPatient.Language.Code );

            ContactPoint newMailingContactPoint = newPatient.ContactPointWith(
                TypeOfContactPoint.NewMailingContactPointType() );

            ContactPoint mailingContactPointFromXml = newPatient.ContactPointWith(
                TypeOfContactPoint.NewMailingContactPointType() );

            Assert.IsFalse( ( ( ArrayList )newPatient.ContactPoints ).Contains( existingMailingContactPoint ) );
            Assert.IsFalse( ( ( ArrayList )newPatient.ContactPoints ).Contains( existingMobileContactPoint ) );

            Assert.IsTrue( newMailingContactPoint.Address.OneLineAddressLabel() == mailingContactPointFromXml.Address.OneLineAddressLabel() );
            Assert.IsTrue( newMailingContactPoint.PhoneNumber == mailingContactPointFromXml.PhoneNumber );
            Assert.IsTrue( newMailingContactPoint.CellPhoneNumber == mailingContactPointFromXml.CellPhoneNumber );
            Assert.IsTrue( newMailingContactPoint.EmailAddress == mailingContactPointFromXml.EmailAddress );
        }

        [Test]
        public void TestBuildDefaultPrimaryInsuranceCoverageForUnInsured_WhenNoInsuranceInformationIsSpecified()
        {
            var registrationData = MessageBuilder.GetPreRegistrationData( PatientAccessResourcesSamplePreRegistrationXml );
            registrationData.visit.insuranceInformation = null;
            var patientFactory = new PatientFactory( registrationData );
            var patient = patientFactory.BuildNewPatient();
            var account = patient.SelectedAccount;

            Coverage primaryCoverage = account.Insurance.GetPrimaryCoverage();

            Assert.IsTrue( typeof( SelfPayInsurancePlan ) == primaryCoverage.InsurancePlan.GetType() );
            Assert.IsTrue( primaryCoverage.InsurancePlan.PlanID == InsuranceCoverageFactory.SELF_PAY_PLAN_ID );
            Assert.IsTrue( account.FinancialClass.Code == FinancialClass.UNINSURED_CODE );
        }

        [Test]
        public void TestBuildDefaultPrimaryInsuranceCoverageForUnInsured_WhenPrimaryAndSecondaryInsuranceInformationIsNull()
        {
            var registrationData = MessageBuilder.GetPreRegistrationData( PatientAccessResourcesSamplePreRegistrationXml );
            registrationData.visit.insuranceInformation.primaryInsurance = null;
            registrationData.visit.insuranceInformation.secondaryInsurance = null;

            var patientFactory = new PatientFactory( registrationData );
            var patient = patientFactory.BuildNewPatient();
            var account = patient.SelectedAccount;

            Coverage primaryCoverage = account.Insurance.GetPrimaryCoverage();

            Assert.IsTrue( typeof( SelfPayInsurancePlan ) == primaryCoverage.InsurancePlan.GetType() );
            Assert.IsTrue( primaryCoverage.InsurancePlan.PlanID == InsuranceCoverageFactory.SELF_PAY_PLAN_ID );
            Assert.IsTrue( account.FinancialClass.Code == FinancialClass.UNINSURED_CODE );
        }
        #endregion

        # region Support Methods

        private Patient BuildExistingPatient()
        {
            Patient patient = new Patient
            {
                FirstName = "FIRST NAME P",
                LastName = "LAST NAME P",
                MiddleInitial = "P",
                MaritalStatus = demographicsBroker.MaritalStatusWith( DhfFacility.Oid, "M" ),
                PlaceOfBirth = "PAT",
                DateOfBirth = new DateTime( 1990, 01, 01 ),
                SocialSecurityNumber = new SocialSecurityNumber(),
                Sex = demographicsBroker.GenderWith( DhfFacility.Oid, "Female" ),
                Race = originBroker.RaceWith( DhfFacility.Oid, "ASIAN" ),
                Ethnicity = originBroker.EthnicityWith( DhfFacility.Oid, "HISPANIC" ),
                Religion = religionBroker.ReligionWith( DhfFacility.Oid, "AMISH" )
            };

            patient.AddContactPoint( existingMailingContactPoint );
            patient.AddContactPoint( existingMobileContactPoint );

            return patient;
        }
        #endregion

        #region Properties

        private Facility DhfFacility
        {
            get { return dhfFacility ?? ( dhfFacility = facilityBroker.FacilityWith( "DHF" ) ); }
        }

        #endregion

        #region Data Elements

        private Facility dhfFacility;
        private readonly IDemographicsBroker demographicsBroker = BrokerFactory.BrokerOfType<IDemographicsBroker>();
        private readonly IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
        private readonly IReligionBroker religionBroker = BrokerFactory.BrokerOfType<IReligionBroker>();
        private readonly IOriginBroker originBroker = BrokerFactory.BrokerOfType<IOriginBroker>();
        private readonly ContactPoint existingMailingContactPoint = new ContactPoint
        {
            Address = new Address( "2300 W PLANO PKWY", "", "PLANO", new ZipCode( "75075" ), new State( "TX" ), new Country( "USA" ) ),
            PhoneNumber = new PhoneNumber( "6786786789" ),
            EmailAddress = new EmailAddress( "ABC@ABC.COM" ),
            TypeOfContactPoint = TypeOfContactPoint.NewMailingContactPointType()
        };

        private readonly ContactPoint existingMobileContactPoint = new ContactPoint
        {
            PhoneNumber = new PhoneNumber( "5555556666" ),
            TypeOfContactPoint = TypeOfContactPoint.NewMobileContactPointType()
        };

        #endregion
    }
}
