using System;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class CopyAccountAdmitNewbornCommandTests
    {
		#region Constants 

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
        public const string CODE_BLANK = " ";

		#endregion Constants 

		#region Fields 

        private Address address;        
        private Facility facility;
        private Gender gender;
        private DateTime patientDOB;
        private Name patientName;
        private SocialSecurityNumber ssn;

		#endregion Fields 

		#region Public Methods 

        [SetUp]
        public void SetUpCopyAccountAdmitNewbornCommandTests()
        {
            var facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();

            facility = facilityBroker.FacilityWith( FACILITY_CODE );
            gender = new Gender( 0, DateTime.Now, "Male", "M" );
            patientDOB = new DateTime( 1955, 3, 5 );
            patientName = new Name( PATIENT_F_NAME, PATIENT_L_NAME, PATIENT_MI );
            ssn = new SocialSecurityNumber( "123121234" );
            address = new Address(ADDRESS1, ADDRESS2, CITY, new ZipCode(POSTALCODE),
                                  new State(0L, ReferenceValue.NEW_VERSION, "TEXAS", "TX"),
                                  new Country(0L, ReferenceValue.NEW_VERSION, "United States", "USA"),
                                  new County(0L, ReferenceValue.NEW_VERSION, "ORANGE", "122"));

        }


        [Test()]
        public void TestCopyAccountAdmitNewbornCommandTests()
        {
            Account anAccount = new Account();

            Patient patient = new Patient(
                PATIENT_OID,
                Patient.NEW_VERSION,
                this.patientName,
                PATIENT_MRN,
                this.patientDOB,
                this.ssn,
                this.gender,
                facility
                );

            anAccount.Patient = patient;

            anAccount.Patient.Name = this.patientName; 
            anAccount.FacilityDeterminedFlag = new FacilityDeterminedFlag( 3, ReferenceValue.NEW_VERSION, "FacilityDeterminedFlag", "flag" );
            anAccount.FinancialClass = new FinancialClass( 1, ReferenceValue.NEW_VERSION, "VIP HOSP DEF HMO/PPO", "02" );
            anAccount.Patient.Religion = new Religion( 3, ReferenceValue.NEW_VERSION, "ADVENTIST", "ADVENTIST" );
            anAccount.Patient.PlaceOfWorship = new PlaceOfWorship( 3, ReferenceValue.NEW_VERSION, "Saint EA Seaton Catholic", "Saint" );
            anAccount.ValuablesAreTaken = new YesNoFlag( "Y" );
            anAccount.TenetCare = new YesNoFlag( "N" );
            anAccount.BillHasDropped = true;
            anAccount.ScheduleCode = new ScheduleCode( 1, DateTime.Now, "ADD-ON LESS THAN 24 HOURS", "A" );
            anAccount.CodedDiagnoses = new CodedDiagnoses();
                        
            Employment empl = new Employment();
            empl.Employer = new Employer( 1L, DateTime.Now, "PerotSystems", "001", 100 );
            empl.EmployeeID = "234";
            empl.PhoneNumber = new PhoneNumber( "9725770000" );
            empl.Occupation = "jsdhsdjhdjh";
            empl.Status = new EmploymentStatus( 3L, "Active" );
            empl.Employer.PartyContactPoint = new ContactPoint( TypeOfContactPoint.NewBusinessContactPointType() );
            anAccount.Patient.Employment = empl;

            anAccount.AddConditionCode( new ConditionCode( 0, ReferenceValue.NEW_VERSION, "ConditionCode1", "C1" ) );
            anAccount.AddOccurrenceCode( new OccurrenceCode( 1, ReferenceValue.NEW_VERSION, "BIRTH DATE - INSURED A", "A1" ) );
            anAccount.AddOccurrenceCode( new OccurrenceCode( 18, ReferenceValue.NEW_VERSION, "DT OF RETIREMENT PT/BENFY", OccurrenceCode.OCCURRENCECODE_RETIREDATE ) );
            anAccount.AddOccurrenceCode( new OccurrenceCode( 19, ReferenceValue.NEW_VERSION, "DT OF RETIREMENT SPOUSE", OccurrenceCode.OCCURRENCECODE_SPOUSERETIRED ) );
            
            Physician consultingPhysician = new Physician();
            consultingPhysician.FirstName = "Consulting";
            consultingPhysician.LastName = "Physician";
            consultingPhysician.PhysicianNumber = 6L;
            PhysicianRelationship conRelationship = new PhysicianRelationship(
                PhysicianRole.Consulting(),
                consultingPhysician );

            anAccount.AddPhysicianRelationship( conRelationship );
            
            Name guarantorName = new Name( GUARANTOR_F_NAME, GUARANTOR_L_NAME, GUARANTOR_MI );
            Guarantor guarantor = new Guarantor( 1L, ReferenceValue.NEW_VERSION, guarantorName );
            PhoneNumber gPhoneNumber = new PhoneNumber( "9725336666" );
            ContactPoint contactPoint = new ContactPoint();
            contactPoint.PhoneNumber = gPhoneNumber;
            guarantor.AddContactPoint( contactPoint );

            anAccount.GuarantorIs( guarantor, new RelationshipType( 0, DateTime.MinValue, "Parent" ) );

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

            Insurance ins = new Insurance();
            Coverage c1 = new CommercialCoverage();
            c1.CoverageOrder = new CoverageOrder( 1, "Primary" );

            anAccount.Insurance.AddCoverage( c1 );

            anAccount.Activity = new AdmitNewbornActivity();
            anAccount.Facility = facility;

            anAccount.Activity = new AdmitNewbornActivity();
           
            IAccountCopyBroker factory  = BrokerFactory.BrokerOfType<IAccountCopyBroker>();
            Account CopiedtoAccount = factory.CreateAccountCopyFor( anAccount );

            Assert.AreEqual( anAccount.Patient.LastName, CopiedtoAccount.Patient.LastName, "Patient Name should copy over." );
            Assert.AreNotEqual( anAccount.Patient.Sex, CopiedtoAccount.Patient.Sex, "Gender should not copy forward");
            Assert.AreNotEqual(  anAccount.Patient.MedicalRecordNumber, (float)CopiedtoAccount.Patient.MedicalRecordNumber, 
                                 "MRN should not copy forward" );
            Assert.AreNotEqual( anAccount.FacilityDeterminedFlag, CopiedtoAccount.FacilityDeterminedFlag, "FacilityDetermingFlag is copied over." );
            Assert.AreEqual( anAccount.FinancialClass, CopiedtoAccount.FinancialClass, "Financial Class is copied over." );
            Assert.AreEqual( anAccount.Patient.Religion, CopiedtoAccount.Patient.Religion, "Patient's religion is copied over." );
            Assert.AreEqual( anAccount.Patient.PlaceOfWorship, CopiedtoAccount.Patient.PlaceOfWorship, "Patient's PlaceOfWorship is copied over." );
            Assert.AreNotEqual( anAccount.ValuablesAreTaken, CopiedtoAccount.ValuablesAreTaken, "Value of ValuablesAreTaken is copied over." );
            Assert.AreNotEqual( anAccount.TenetCare, CopiedtoAccount.TenetCare, "Value of TenetCare is copied over." );
            Assert.AreEqual( CopiedtoAccount.BillHasDropped, false, "BillHasDropped has been set to false." );
            Assert.AreEqual( CopiedtoAccount.ScheduleCode, new ScheduleCode(0,DateTime.Now,"WALKIN WITH AN ORDER","W"), "ScheduleCode has been Initialized." );
            Assert.AreEqual(CopiedtoAccount.CodedDiagnoses.CodedDiagnosises.Count, 0 , "CodedDiagnosises has been Initialized." );
            Assert.AreEqual( CopiedtoAccount.CodedDiagnoses.AdmittingCodedDiagnosises.Count, 0, "AdmittingCodedDiagnosises has been Initialized." );
            Assert.AreEqual( CopiedtoAccount.Patient.Employment.EmployeeID, string.Empty, "Employment has been Initialized." );
            Assert.AreEqual( CopiedtoAccount.Patient.Employment.Status, EmploymentStatus.NewNotEmployed() );
            Assert.AreEqual( CopiedtoAccount.ConditionCodes.Count, 0, "All condition codes are removed." );
            Assert.AreEqual( CopiedtoAccount.OccurrenceCodes.Count, 2, "Occurance codes 18 and 19  are copied over." );
            Assert.AreNotEqual( anAccount.Guarantor, CopiedtoAccount.Guarantor, "Guarantor should be modified." );
            Assert.AreNotEqual( anAccount.Insurance, CopiedtoAccount.Insurance, "Insurance should be modified." );
            Assert.AreEqual( anAccount.Activity, CopiedtoAccount.Activity, "Activity is copied over." );
            Assert.AreEqual( anAccount.Facility, CopiedtoAccount.Facility, "Facility is copied over." );

        }

		#endregion Public Methods 
     }
}