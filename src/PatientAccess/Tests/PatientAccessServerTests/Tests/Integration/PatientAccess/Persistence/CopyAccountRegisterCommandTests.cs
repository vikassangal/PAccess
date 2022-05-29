using System;
using Extensions.PersistenceCommon;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture]
    [Category( "Fast" )]
    public class CopyAccountRegisterCommandTests
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
        #endregion

        #region SetUp and TearDown CopyAccountRegisterCommandTests
        [TestFixtureSetUp()]
        public static void SetUpCopyAccountRegisterCommandTests()
        {
           
        }

        [TestFixtureTearDown()]
        public static void TearDownCopyAccountRegisterCommandTests()
        {
        }
        #endregion

        #region Methods
        [Test()]
        public void TestCopyAccountRegisterCommand()
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
                this.facility
                );

            anAccount.Oid = 123;
            anAccount.AccountNumber = 12345;
            anAccount.AdmitDate = DateTime.Now;
            anAccount.AdmitSource = new AdmitSource( 1L, DateTime.Now, "admitSource", "02" );        
            anAccount.Location = new Location( "NS", "180", "A" );
            anAccount.COSSigned = new ConditionOfService( 3, ReferenceValue.NEW_VERSION, ConditionOfService.UNABLE_DESCRIPTION, ConditionOfService.UNABLE );
            anAccount.HospitalService = new HospitalService( 1, ReferenceValue.NEW_VERSION, "TestHospital Serviece", "58" );
            anAccount.Pregnant = new YesNoFlag( "Y" );
            anAccount.ScheduleCode = new ScheduleCode( 1, DateTime.Now, "ADD-ON LESS THAN 24 HOURS", "A" );
            anAccount.CodedDiagnoses = new CodedDiagnoses();
            anAccount.AddConditionCode( new ConditionCode( 0, ReferenceValue.NEW_VERSION, "ConditionCode1", "C1" ) );
            anAccount.Patient.MostRecentAccountCreationDate = aDateLessThan60DaysInThePast;

            Diagnosis diagnosis = new Diagnosis();
            Illness illness = new Illness();
            illness.Onset = DateTime.Parse( "Jan 21, 2005" );
            diagnosis.ChiefComplaint = "Soar Throat";
            diagnosis.Condition = illness;
            anAccount.Diagnosis = diagnosis;
            Illness patientCond = (Illness)anAccount.Diagnosis.Condition;

            Pregnancy pregnancy = new Pregnancy();
            pregnancy.LastPeriod = DateTime.Parse( "Jan 21, 2005" );
            diagnosis.Condition = pregnancy;
            anAccount.Diagnosis = diagnosis;
            Pregnancy preg = (Pregnancy)anAccount.Diagnosis.Condition;

            UnknownCondition unknown = new UnknownCondition();
            diagnosis.Condition = unknown;
            anAccount.Diagnosis = diagnosis;
            UnknownCondition unk = (UnknownCondition)anAccount.Diagnosis.Condition;

            anAccount.AddOccurrenceCode( new OccurrenceCode( 18, DateTime.Now, "DT OF RETIREMENT PT/BENFY", OccurrenceCode.OCCURRENCECODE_RETIREDATE ) );
            anAccount.AddOccurrenceCode( new OccurrenceCode( 19, DateTime.Now, "DT OF RETIREMENT SPOUSE", OccurrenceCode.OCCURRENCECODE_SPOUSERETIRED ) );
            anAccount.AddOccurrenceCode( new OccurrenceCode( 11, DateTime.Now, "ONSET OF SYMPTOMS/ILLNESS", OccurrenceCode.OCCURRENCECODE_ILLNESS ) );
           
            anAccount.ClinicalComments = "Test";

            Physician consultingPhysician = new Physician();
            consultingPhysician.FirstName = "Consulting";
            consultingPhysician.LastName = "Physician";
            consultingPhysician.PhysicianNumber = 6L;

            PhysicianRelationship conRelationship = new PhysicianRelationship(
                PhysicianRole.Consulting().Role(),
                consultingPhysician );
            
            anAccount.AddPhysicianRelationship( conRelationship );

            Physician admittingPhysician = new Physician();
            admittingPhysician.FirstName = "Admitting";
            admittingPhysician.LastName = "Physician";
            admittingPhysician.PhysicianNumber = 1L;

            PhysicianRelationship admittingRelationship = new PhysicianRelationship(
                PhysicianRole.Admitting().Role(),
                admittingPhysician);

            anAccount.AddPhysicianRelationship(admittingRelationship);

            var primarCarePhysician = new Physician
                {
                    FirstName = "PrimaryCare", 
                    LastName = "Physician", 
                    PhysicianNumber = 1L
                };

            var primaryCareRelationship = new PhysicianRelationship(PhysicianRole.PrimaryCare().Role(), primarCarePhysician);

            anAccount.AddPhysicianRelationship(primaryCareRelationship);

            VisitType visitType = new VisitType();
            visitType.Code = VisitType.OUTPATIENT;
            anAccount.KindOfVisit = visitType;
            
            OccurrenceSpan occSpan1 = new OccurrenceSpan();
            OccurrenceSpan occSpan2 = new OccurrenceSpan();
            occSpan1.SpanCode = new SpanCode( 1, ReferenceValue.NEW_VERSION, "QUALIFYING STAY DATES", "70" );
            occSpan2.SpanCode = new SpanCode( 1, ReferenceValue.NEW_VERSION, "PRIOR STAY DATES", "71" );

            anAccount.OccurrenceSpans.Add( occSpan1 );
            anAccount.OccurrenceSpans.Add( occSpan2 );
                                  
            anAccount.Clinics.Add( new HospitalClinic( 3, ReferenceValue.NEW_VERSION,
                                                       "BLOOD TRANSFUSION", "AL", string.Empty, "01" ) );
                        
            anAccount.BalanceDue = 200.00m;
            anAccount.BillHasDropped = true;
            anAccount.LastChargeDate = DateTime.Now;
            anAccount.MonthlyPayment = 200.00m;
            anAccount.NumberOfMonthlyPayments = 5;
            anAccount.Payment = new Payment();
            anAccount.PreviousTotalCurrentAmtDue = 0;
            anAccount.RequestedPayment = 1000.00m;
            anAccount.TotalCurrentAmtDue = 200.00m;
            anAccount.TotalCharges = 1000.00m;
            anAccount.TotalPaid = 800.00m;
            anAccount.OriginalMonthlyPayment = 200.00m;
            anAccount.ClergyVisit.SetBlank( "N" );

            Insurance ins = new Insurance();
            Coverage c1 = new CommercialCoverage();
            c1.CoverageOrder = new CoverageOrder( 1, "Primary" );
            Coverage c2 = new CommercialCoverage();
            c2.CoverageOrder = new CoverageOrder( 2, "Secondary" );

            anAccount.Insurance.AddCoverage( c1 );
            anAccount.Insurance.AddCoverage( c2 );

            anAccount.Activity = new RegistrationActivity();
            IAccountCopyBroker factory  = BrokerFactory.BrokerOfType<IAccountCopyBroker>();
            Account CopiedtoAccount = factory.CreateAccountCopyFor( anAccount );

            Assert.AreEqual( CopiedtoAccount.Oid,Account.NEW_OID,"Account Oid has been initialized" );
            Assert.AreEqual( CopiedtoAccount.AccountNumber,0L, "Account number has been initialized." );
            Assert.AreEqual( CopiedtoAccount.AdmitDate, DateTime.MinValue, "Admit Date has been initialized." );
            Assert.AreEqual( CopiedtoAccount.AdmitSource, new AdmitSource(), "AdmitSource has been initialized." );
            Assert.AreEqual( CopiedtoAccount.Location, new Location(), "Location has been initialized." );
            Assert.AreEqual( CopiedtoAccount.COSSigned, new ConditionOfService(), "COSSigned has been initialized." );
            Assert.AreEqual( CopiedtoAccount.HospitalService, new HospitalService(), "Hopital Service has been initialized." );
            Assert.AreEqual( CopiedtoAccount.Pregnant, new YesNoFlag(), "Account number has been initialized." );
            Assert.AreEqual( CopiedtoAccount.ScheduleCode, new ScheduleCode(), "ScheduleCode has been initialized." );
            Assert.AreEqual( CopiedtoAccount.CodedDiagnoses.CodedDiagnosises.Count, 0, "CodedDiagnosises has been Initialized." );
            Assert.AreEqual( CopiedtoAccount.CodedDiagnoses.AdmittingCodedDiagnosises.Count, 0, "AdmittingCodedDiagnosises has been Initialized." );
            Assert.AreEqual( CopiedtoAccount.ConditionCodes.Count, 0, "All condition codes are removed." );
            Assert.AreEqual( CopiedtoAccount.Diagnosis.ChiefComplaint,string.Empty, "ChiefComplaint has been initialized." );
            Assert.AreEqual( typeof( UnknownCondition ), CopiedtoAccount.Diagnosis.Condition.GetType() );
            Assert.AreEqual( anAccount.Diagnosis.Condition.GetType(), CopiedtoAccount.Diagnosis.Condition.GetType(), "Condition has been initialized to illness." );
            Assert.AreEqual( CopiedtoAccount.OccurrenceCodes.Count, 2, "All OccurrenceCodes other than 18, 19 are removed." );
            Assert.AreEqual( CopiedtoAccount.ConsultingPhysicians.Count, 0 , "ConsultingPhysicians has been added." );
            Assert.AreEqual( CopiedtoAccount.AdmittingPhysician, null, "AdmittingPhysicians has been removed." );
            Assert.AreEqual(CopiedtoAccount.PrimaryCarePhysician, anAccount.PrimaryCarePhysician, "Primary Care Physician has been copied forward");
            Assert.AreEqual( CopiedtoAccount.OccurrenceSpans.Count, 0, "OccurrenceSpans not removed" );
            Assert.AreEqual( CopiedtoAccount.KindOfVisit, new VisitType(), "Visit Type has been initialized." );
            Assert.AreEqual( CopiedtoAccount.HospitalClinic, new HospitalClinic(), "Hospital Clinic had been initialized." );
            Assert.AreEqual( 0M, CopiedtoAccount.BalanceDue, "Balance Due has been set to false." );
            Assert.AreEqual( CopiedtoAccount.BillHasDropped, false, "BillHasDropped has been initialized." );
            Assert.AreEqual( CopiedtoAccount.LastChargeDate, DateTime.MinValue, "LastChargeDate has been initialized." );
            Assert.AreEqual( CopiedtoAccount.MonthlyPayment, 0M, "Monthly Payment has been initialized." );
            Assert.AreEqual( CopiedtoAccount.NumberOfMonthlyPayments, 0, "NumberOfMonthlyPayments has been initialized." );
            Assert.AreEqual( CopiedtoAccount.Payment.TotalPayment, 0M, "Payment has been initialized." );
            Assert.AreEqual( CopiedtoAccount.PreviousTotalCurrentAmtDue, 0M, "PreviousTotalCurrentAmtDue has been initialized." );
            Assert.AreEqual( CopiedtoAccount.RequestedPayment, 0M, "RequestedPayment has been initialized." );
            Assert.AreEqual( CopiedtoAccount.TotalCurrentAmtDue, 0M, "TotalCurrentAmtDue has been initialized." );
            Assert.AreEqual( CopiedtoAccount.TotalPaid, 0M, "TotalPaid has been initialized." );
            Assert.AreEqual( CopiedtoAccount.TotalCharges, 0M, "TotalCharges has been initialized." );
            Assert.AreEqual( CopiedtoAccount.OriginalMonthlyPayment, 0M, "OriginalMonthlyPayment has been initialized." );
            YesNoFlag blankFlag = new YesNoFlag();
            blankFlag.SetBlank( string.Empty );
            Assert.AreEqual( CopiedtoAccount.ClergyVisit, YesNoFlag.Blank, "ClergyVisit has been set to Blank." );
            
        }

        #endregion

        #region Data Elements
        public const string CODE_BLANK = " ";
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
                                     FACILITY_CODE
                );

        private readonly DateTime aDateLessThan60DaysInThePast = DateTime.Now.Subtract(TimeSpan.FromDays(59));
        
        #endregion
    }
}