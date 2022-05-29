using System;
using Extensions.PersistenceCommon;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Test case for Domain class CopyAccountPreMSECommand
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class CopyAccountPreMSECommandTests
    {
        #region Constants
        private const string PATIENT_F_NAME = "Sam";
        private const string PATIENT_L_NAME = "Spade";
        private const string PATIENT_MI = "L";
        private const string GUARANTOR_F_NAME = "John";
        private const string GUARANTOR_L_NAME = "Spade";
        private const string GUARANTOR_MI = "D";
        private const string FACILILTY_NAME = "DELRAY TEST HOSPITAL";
        private const string FACILITY_CODE = "DEL";
        private const long PATIENT_OID = 45L;
        private const long PATIENT_MRN = 123456789;
        private const string ADDRESS1 = "335 Nicholson Dr.";
        private const string ADDRESS2 = "#303";
        private const string CITY = "Austin";
        private const string POSTALCODE = "60505";
        #endregion

        #region SetUp and TearDown ConfidentialityStatusTests
        [TestFixtureSetUp()]
        public static void SetUpCopyAccountPreMSECommandTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownCopyAccountPreMSECommandTests()
        {
        }
        #endregion

        #region Test Methods
        /// <summary>
        /// Will test if required information are copied over.
        /// </summary>
        [Test()]
        public void TestExecute()
        {
            Patient patient = new Patient(
                PATIENT_OID,
                Patient.NEW_VERSION,
                this.patientName,
                PATIENT_MRN,
                this.patientDOB,
                this.ssn,
                this.gender,
                this.facility
                );

            PhoneNumber pPhoneNumber = new PhoneNumber( "9725778888" );
            ContactPoint cp = new ContactPoint();
            cp.PhoneNumber = pPhoneNumber;
            patient.AddContactPoint( cp );

            patient.Religion = new Religion(3,ReferenceValue.NEW_VERSION,"ADVENTIST","ADVENTIST");
            patient.PlaceOfWorship = new PlaceOfWorship( 3, ReferenceValue.NEW_VERSION, "Saint EA Seaton Catholic", "Saint" );
            Employment empl = new Employment();
            empl.Employer = new Employer( 1L, DateTime.Now, "PerotSystems", "001", 100 );
            empl.EmployeeID = "234";
            empl.PhoneNumber = new PhoneNumber( "9725770000" );
            empl.Occupation = "Test";
            empl.Status = new EmploymentStatus( 3L, "Active" );
            empl.Employer.PartyContactPoint = new ContactPoint( TypeOfContactPoint.NewBusinessContactPointType() );
            empl.Employer.PartyContactPoint.Address = this.address;

            patient.Employment = empl;

            NoticeOfPrivacyPracticeDocument nppDoc = new NoticeOfPrivacyPracticeDocument();
            nppDoc.SignedOnDate = DateTime.Now;
            nppDoc.NPPVersion = new NPPVersion( 0L, ReferenceValue.NEW_VERSION, "20030101", "20" );

            patient.NoticeOfPrivacyPracticeDocument = nppDoc;

            Account anAccount = new Account();
            anAccount.AccountNumber = 5336040;
            anAccount.Patient = patient;
            anAccount.Facility = this.facility;

            Name guarantorName = new Name( GUARANTOR_F_NAME, GUARANTOR_L_NAME, GUARANTOR_MI );
            Guarantor guarantor = new Guarantor( 1L, ReferenceValue.NEW_VERSION, guarantorName );
            PhoneNumber gPhoneNumber = new PhoneNumber( "9725336666" );
            ContactPoint contactPoint = new ContactPoint();
            contactPoint.PhoneNumber = gPhoneNumber;
            guarantor.AddContactPoint( contactPoint );

            anAccount.GuarantorIs( guarantor, new RelationshipType( 0, DateTime.MinValue, "Parent" ) );

            anAccount.ValuablesAreTaken = new YesNoFlag( "Y" );
            anAccount.Bloodless = new YesNoFlag( "Y" );

            EmergencyContact emergencyContact = new EmergencyContact();
            emergencyContact.Name = "Alan Smith";
            ContactPoint emergencyContactPoint = new ContactPoint();
            emergencyContactPoint.Address = this.address;
            emergencyContactPoint.PhoneNumber = new PhoneNumber( "8005235800" );
            emergencyContactPoint.TypeOfContactPoint = new TypeOfContactPoint( 0L, "work" );
            emergencyContact.AddContactPoint( emergencyContactPoint );
            Relationship relationship = new Relationship( ( new RelationshipType( 0L, DateTime.Now, "Spouse" ) ), patient.GetType(), emergencyContact.GetType() );
            emergencyContact.AddRelationship( relationship );
            anAccount.EmergencyContact1 = emergencyContact;
            anAccount.EmergencyContact2 = emergencyContact;

            CommercialInsurancePlan plan = new CommercialInsurancePlan();
            plan.Oid = 1;
            plan.PlanName = "PRIVATE PAY";

            BillingInformation aBillingInformation = new BillingInformation( this.address,
                                                                             new PhoneNumber( "8005235800" )
                                                                             , new EmailAddress( "Bill.Person@claims.net" ), new TypeOfContactPoint( 0L, "work" ) );

            aBillingInformation.BillingCOName = "Medicare Part A Claims";

            BillingInformation aBillingInformation2 = new BillingInformation( this.address,
                                                                              new PhoneNumber( "8003250058" )
                                                                              , new EmailAddress( "Bill.Person2@claims.net" ), new TypeOfContactPoint( 0L, "Home" ) );


            aBillingInformation2.BillingCOName = "Medicare Part B Claims";

            plan.AddBillingInformation( aBillingInformation );
            plan.AddBillingInformation( aBillingInformation2 );
            Insured insured = new Insured();
            insured.FirstName = "Sam";
            Coverage coverage = Coverage.CoverageFor( plan, insured );
            coverage.BillingInformation = aBillingInformation;
            coverage.CoverageOrder = CoverageOrder.NewPrimaryCoverageOrder();
            anAccount.Insurance.AddCoverage( coverage );

            anAccount.FinancialClass = new FinancialClass( 299, ReferenceValue.NEW_VERSION, "MEDICADE", "40" );
            anAccount.HospitalService = new HospitalService( 0, ReferenceValue.NEW_VERSION, "DIAGNOSTIC OUTOPT", "60" );
            anAccount.KindOfVisit = new VisitType( 0, ReferenceValue.NEW_VERSION, "PRE-MSE", VisitType.EMERGENCY_PATIENT );
            anAccount.BillHasDropped = true;
            anAccount.ScheduleCode = new ScheduleCode( 0, ReferenceValue.NEW_VERSION, "UNKNOWN", "Z" );
            anAccount.ClergyVisit = new YesNoFlag( "Y" );
            anAccount.AddOccurrenceCode( new OccurrenceCode( 1, ReferenceValue.NEW_VERSION, "BIRTH DATE - INSURED A", "A1" ) );
            anAccount.AddOccurrenceCode( new OccurrenceCode( 18, ReferenceValue.NEW_VERSION, "DT OF RETIREMENT PT/BENFY", OccurrenceCode.OCCURRENCECODE_RETIREDATE ) );
            anAccount.AddOccurrenceCode( new OccurrenceCode( 19, ReferenceValue.NEW_VERSION, "DT OF RETIREMENT SPOUSE", OccurrenceCode.OCCURRENCECODE_SPOUSERETIRED ) );

            anAccount.AddConditionCode( new ConditionCode( 0, ReferenceValue.NEW_VERSION, "ConditionCode1", "C1" ) );
            

            anAccount.Activity = new PreMSERegisterActivity();
            IAccountCopyBroker factory  = BrokerFactory.BrokerOfType<IAccountCopyBroker>();
            Account newAccount = factory.CreateAccountCopyFor( anAccount );

            Assert.AreEqual( anAccount.FinancialClass, newAccount.FinancialClass, "Financial Class is copied over" );
            Assert.AreEqual( anAccount.Patient, newAccount.Patient, "Patient is copied over." );
            Assert.AreEqual( anAccount.Patient.Religion, newAccount.Patient.Religion, "Patient's religion is copied over." );
            Assert.AreEqual( anAccount.Patient.PlaceOfWorship, newAccount.Patient.PlaceOfWorship, "Patient's PlaceOfWorship is copied over." );
            Assert.AreNotEqual( anAccount.ValuablesAreTaken, newAccount.ValuablesAreTaken, "Value of ValuablesAreTaken is copied over." );
            Assert.AreEqual( anAccount.Bloodless, newAccount.Bloodless, "Value of Bloodless is copied over." );
            Assert.AreEqual( anAccount.EmergencyContact1, newAccount.EmergencyContact1, "EmergencyContact1 is copied over." );
            Assert.AreEqual( anAccount.EmergencyContact2, newAccount.EmergencyContact2, "EmergencyContact2 is copied over." );
            Assert.AreEqual( newAccount.BillHasDropped, false, "BillHasDropped has been set to false." );
            Assert.AreEqual( newAccount.ScheduleCode, new ScheduleCode(), "ScheduleCode has been Initialized." );
            Assert.AreEqual( anAccount.AccountNumber, newAccount.PreMSECopiedAccountNumber, "Account number is copied over as PreMSECopiedAccountNumber." );
            YesNoFlag blankFlag = new YesNoFlag();
            blankFlag.SetBlank( string.Empty );
            Assert.AreEqual( newAccount.ClergyVisit, YesNoFlag.Blank, "ClergyVisit has been set to Blank." );
            Assert.AreEqual( newAccount.OccurrenceCodes.Count, 2, "Only required occurance codes are copied over." );
            Assert.AreEqual( newAccount.ConditionCodes.Count, 0, "All condition codes are removed." );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        private SocialSecurityNumber
            ssn = new SocialSecurityNumber( "123121234" );
        private Name
            patientName = new Name( PATIENT_F_NAME, PATIENT_L_NAME, PATIENT_MI );
        private DateTime
            patientDOB = new DateTime( 1955, 3, 5 );
        private Gender
            gender = new Gender( 0, DateTime.Now, "Male", "M" );
        private Facility
            facility = new Facility( PersistentModel.NEW_OID,
                                     PersistentModel.NEW_VERSION,
                                     FACILILTY_NAME,
                                     FACILITY_CODE );
        private Address address = new Address( ADDRESS1,
                                               ADDRESS2,
                                               CITY,
                                               new ZipCode( POSTALCODE ),
                                               new State( 0L,
                                                          ReferenceValue.NEW_VERSION,
                                                          "TEXAS",
                                                          "TX" ),
                                               new Country( 0L,
                                                            ReferenceValue.NEW_VERSION,
                                                            "United States",
                                                            "USA" ),
                                               new County( 0L,
                                                           ReferenceValue.NEW_VERSION,
                                                           "ORANGE",
                                                           "122" )
            ); 
        #endregion
    }
}