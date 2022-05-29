using System;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class GuarantorBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown guarantorBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpguarantorBrokerTests()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker >();
            facility = facilityBroker.FacilityWith( FACILITY_CODE );
        }
        [TestFixtureTearDown()]
        public static void TearDownguarantorBrokerTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestGuarantorFor()  //get a guarantor for an account#
        {

            Patient patient = new Patient(
                PATIENT_OID,
                Patient.NEW_VERSION,
                this.PATIENT_NAME,
                PATIENT_MRN,
                this.PATIENT_DOB,
                this.PATIENT_SSN,
                this.PATIENT_SEX,
                facility
                );

            IGuarantorBroker guarantorBroker = BrokerFactory.BrokerOfType<IGuarantorBroker>(this.ConnectionString);
            Account anAccount = new Account();

            anAccount.AccountNumber = 30163;
            anAccount.Patient = patient;

            IFacilityBroker fb = BrokerFactory.BrokerOfType<IFacilityBroker>();

            anAccount.Facility = fb.FacilityWith( 900 );
            Guarantor guarantor = guarantorBroker.GuarantorFor( anAccount );

            Assert.IsTrue( guarantor != null, "guarantor should not be null." );

            Assert.AreEqual(
                "ARNIE  RAZORBACK",
                ( (Guarantor)guarantor ).Name.FirstName.TrimEnd() + " " +
                ( (Guarantor)guarantor ).Name.MiddleInitial.TrimEnd() + " " +
                ( (Guarantor)guarantor ).Name.LastName.TrimEnd(),
                "Name should be ARNIE  RAZORBACK"
                );

            Assert.AreEqual(
                "MALE",
                ( (Guarantor)guarantor ).Sex.Description,
                "SEX should be MALE"
                );

            Assert.AreEqual(
                "435-42-3556",
                ( (Guarantor)guarantor ).SocialSecurityNumber.AsFormattedString(),
                "SSN should be 435-42-3556"
                );

            Assert.AreEqual(
                "5945849058",
                ( (Guarantor)guarantor ).DriversLicense.Number,
                "DL# should be 5945849058"
                );

            Assert.AreEqual(
                "GA",
                ( (Guarantor)guarantor ).DriversLicense.State.Code,
                "DL# state Code should be GA"
                );

            Assert.AreEqual(
                "GEORGIA",
                ( (Guarantor)guarantor ).DriversLicense.State.Description,
                "DL# state should be GEORGIA"
                );

            ContactPoint cp = guarantor.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() );

            Assert.AreEqual(
                "MAILING",
                cp.TypeOfContactPoint.Description,
                "ContactPoint type should be MAILING"
                );
            PhoneNumber aPhoneNumber = cp.PhoneNumber;
            Assert.AreEqual(
                "5055553222",
                aPhoneNumber.ToString(),
                "Phone should be 5055553222"
                );

            Assert.AreEqual(
                "505",
                aPhoneNumber.AreaCode,
                "Phone should be 505"
                );

            Assert.AreEqual(
                "5553222",
                aPhoneNumber.Number,
                "Phone should be 5553222"
                );

            Address aAddress = cp.Address;

            Assert.AreEqual(
                "4325 BRYNHURST AVE",
                aAddress.Address1,
                "Address should be 4325 BRYNHURST AVE"
                );

            Assert.AreEqual(
                string.Empty,
                aAddress.Address2,
                "Address should be spaces"
                );

            Assert.AreEqual(
                "CONWAY",
                aAddress.City,
                "City should be CONWAY"
                );

            Assert.AreEqual(
                "ARKANSAS",
                aAddress.State.Description,
                "PostalCode should be ARKANSAS"
                );

            Assert.AreEqual(
                "720320000",
                aAddress.ZipCode.PostalCode,
                "PostalCode should be 720320000"
                );
           
            foreach( Relationship aRelationship in anAccount.Patient.Relationships )
            {

                Assert.AreEqual(
                    "SELF",
                    aRelationship.Type.Description,
                    "Relationship type should be SELF"
                    );               
            }
            Assert.IsNotNull( guarantor.Employment, "Did not find this person's Employment" );
            Employment employment = guarantor.Employment;
            Assert.IsNotNull( employment.Employer, "Did not find this perons's Employer" );
            Employer employer = employment.Employer;
            Assert.AreEqual("BASKIN ROBBINS", employer.Name, "Employer Name should be blank");
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private const string
            PATIENT_F_NAME = "Sam",
            PATIENT_L_NAME = "Spade",
            PATIENT_MI = "L";
        private SocialSecurityNumber
            PATIENT_SSN = new SocialSecurityNumber( "123121234" );
        private const long
            PATIENT_OID = 45L,
            PATIENT_MRN = 123456789;
        private Name
            PATIENT_NAME = new Name( PATIENT_F_NAME, PATIENT_L_NAME, PATIENT_MI );
        private DateTime
            PATIENT_DOB = new DateTime( 1955, 3, 5 );
        private Gender
            PATIENT_SEX = new Gender( 0, DateTime.Now, "Male", "M" );
       
        private new const string FACILITY_CODE = "DEL";
        private static Facility facility = null;
        #endregion
    }
}