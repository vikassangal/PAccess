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
    public class CopyAccountPreregisterCommandTests
    {
        #region Constants
        private const string PATIENT_F_NAME = "Sam";
        private const string PATIENT_L_NAME = "Spade";
        private const string PATIENT_MI = "L";
        private const string FACILILTY_NAME = "DELRAY TEST HOSPITAL";
        private const string FACILITY_CODE = "DEL";
        private const long PATIENT_OID = 45L;
        private const long PATIENT_MRN = 123456789;
        #endregion

        #region SetUp and TearDown CopyAccountPreregisterCommandTests
        [TestFixtureSetUp()]
        public static void SetUpCopyAccountPreregisterCommandTests()
        {

        }

        [TestFixtureTearDown()]
        public static void TearDownCopyAccountPreregisterCommandTests()
        {
        }
        #endregion

        #region Methods
        [Test()]
        public void TestCopyAccountPreregisterCommand()
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
            //anAccount.BillHasDropped = true; repeated
            anAccount.ScheduleCode = new ScheduleCode( 1, DateTime.Now, "ADD-ON LESS THAN 24 HOURS", "A" );
            anAccount.Pregnant = new YesNoFlag( "Y" );
            anAccount.CodedDiagnoses = new CodedDiagnoses();
            anAccount.ValuablesAreTaken = new YesNoFlag( "N" );

            VisitType visitType = new VisitType();
            visitType.Code = VisitType.OUTPATIENT;
            anAccount.KindOfVisit = visitType;

            anAccount.Clinics.Add( new HospitalClinic( 3, ReferenceValue.NEW_VERSION,
                                                       "BLOOD TRANSFUSION", "AL", string.Empty, "01" ) );

            anAccount.ClinicalComments = "Test";
            anAccount.HospitalService = new HospitalService( 1, ReferenceValue.NEW_VERSION, "TestHospital Serviece", "58" );
            anAccount.Location = new Location( "NS", "180", "A" );
            anAccount.COSSigned = new ConditionOfService( 3, ReferenceValue.NEW_VERSION, ConditionOfService.NOT_AVAILABLE_DESCRIPTION, ConditionOfService.NOT_AVAILABLE );

            Physician consultingPhysician = new Physician();
            consultingPhysician.FirstName = "Consulting";
            consultingPhysician.LastName = "Physician";
            consultingPhysician.PhysicianNumber = 6L;
            PhysicianRelationship conRelationship = new PhysicianRelationship(
                PhysicianRole.Consulting(),
                consultingPhysician );

            anAccount.AddPhysicianRelationship( conRelationship );
          
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

            OccurrenceSpan occSpan1 = new OccurrenceSpan();
            OccurrenceSpan occSpan2 = new OccurrenceSpan();
            occSpan1.SpanCode = new SpanCode( 1, ReferenceValue.NEW_VERSION, "QUALIFYING STAY DATES", "70" );
            occSpan2.SpanCode = new SpanCode( 1, ReferenceValue.NEW_VERSION, "PRIOR STAY DATES", "71" );

            anAccount.OccurrenceSpans.Add( occSpan1 );
            anAccount.OccurrenceSpans.Add( occSpan2 );

            anAccount.AddConditionCode( new ConditionCode( 0, ReferenceValue.NEW_VERSION, "ConditionCode1", "C1" ) );
            anAccount.Bloodless = new YesNoFlag( "N" );

            anAccount.BalanceDue = 200.00m;
            anAccount.BillHasDropped = true;
            anAccount.LastChargeDate = DateTime.Now;
            anAccount.MonthlyPayment = 200.00m;
            anAccount.NumberOfMonthlyPayments = 5;
            anAccount.Payment = new Payment();
            anAccount.PreviousTotalCurrentAmtDue = 0;
            anAccount.RequestedPayment = 1000.00m;
            anAccount.TotalCurrentAmtDue = 200.00m;
            anAccount.TotalPaid = 800.00m;
            anAccount.TotalCharges = 1000.00m;
            anAccount.OriginalMonthlyPayment = 200.00m;
            
            Insurance ins = new Insurance();
            Coverage c1 = new CommercialCoverage();
            c1.CoverageOrder = new CoverageOrder( 1, "Primary" );
            Coverage c2 = new CommercialCoverage();
            c2.CoverageOrder = new CoverageOrder( 2, "Secondary" );

            anAccount.Insurance.AddCoverage( c1 );
            anAccount.Insurance.AddCoverage( c2 );

            anAccount.Activity = new PreRegistrationActivity();
            IAccountCopyBroker factory  = BrokerFactory.BrokerOfType<IAccountCopyBroker>();
            Account CopiedtoAccount = factory.CreateAccountCopyFor( anAccount );

            Assert.AreEqual( CopiedtoAccount.Oid, Account.NEW_OID, "Account Oid has been initialized" );
            Assert.AreEqual( CopiedtoAccount.AccountNumber, 0L, "Account number has been initialized" );
            Assert.AreEqual( CopiedtoAccount.AdmitDate, DateTime.MinValue, "Admit Date has been initialized" );
            Assert.AreEqual( CopiedtoAccount.AdmitSource, new AdmitSource(), "AdmitSource has been initialized" );
            Assert.AreEqual( CopiedtoAccount.ScheduleCode, new ScheduleCode(), "ScheduleCode has been initialized" );
            Assert.AreEqual( CopiedtoAccount.Pregnant, new YesNoFlag(), "Account number has been initialized" );
            Assert.AreEqual( CopiedtoAccount.CodedDiagnoses.CodedDiagnosises.Count, 0, "CodedDiagnosises has been Initialized." );
            Assert.AreEqual( CopiedtoAccount.CodedDiagnoses.AdmittingCodedDiagnosises.Count, 0, "AdmittingCodedDiagnosises has been Initialized." );
            Assert.AreEqual( CopiedtoAccount.ValuablesAreTaken, new YesNoFlag(), "ValuablesAreTaken has been initialized." );
            Assert.AreEqual( CopiedtoAccount.KindOfVisit, new VisitType(), "Visit Type has been initialized." );
            Assert.AreEqual( CopiedtoAccount.HospitalClinic, new HospitalClinic(), "Hospital Clinic has been initialized" );
            Assert.AreEqual( CopiedtoAccount.ClinicalComments, string.Empty, "Clinic Comments has been removed." );
            Assert.AreEqual( CopiedtoAccount.HospitalService, new HospitalService(), "Hopital Service has been initialized" );
            Assert.AreEqual( CopiedtoAccount.Location, new Location(), "Location has been initialized" );
            Assert.AreEqual( CopiedtoAccount.COSSigned, new ConditionOfService(), "COSSigned has been initialized" );
            YesNoFlag blankFlag = new YesNoFlag();
            blankFlag.SetBlank( string.Empty );
            Assert.AreEqual( CopiedtoAccount.ClergyVisit, YesNoFlag.Blank, "ClergyVisit has been set to Blank." );
            Assert.AreEqual( CopiedtoAccount.ConsultingPhysicians.Count, 0, "ConsultingPhysicians has been removed" );
            Assert.AreEqual( CopiedtoAccount.Diagnosis.ChiefComplaint, string.Empty, "Diagnosis has been initialized." );
            Assert.AreEqual( CopiedtoAccount.OccurrenceCodes.Count, 2, "Occurance codes has been removed." );
            Assert.AreEqual( CopiedtoAccount.HospitalClinic, new HospitalClinic(), "Hospital Clinic has been initialized." );
            Assert.AreEqual( CopiedtoAccount.OccurrenceSpans.Count, 0, "OccurrenceSpans not removed" );
            Assert.AreEqual( CopiedtoAccount.ConditionCodes.Count, 0, "All condition codes are removed." );
            Assert.AreEqual( CopiedtoAccount.Bloodless, anAccount.Bloodless, "Value of Bloodless has been initialized." );
            Assert.AreEqual( 0M, CopiedtoAccount.BalanceDue, "Balance Due has been set to false" );
            Assert.AreEqual( CopiedtoAccount.BillHasDropped, false, "BillHasDropped has been initialized" );
            Assert.AreEqual( CopiedtoAccount.LastChargeDate, DateTime.MinValue, "LastChargeDate has been initialized" );
            Assert.AreEqual( CopiedtoAccount.MonthlyPayment, 0M, "Monthly Payment has been initialized" );
            Assert.AreEqual( CopiedtoAccount.NumberOfMonthlyPayments, 0, "NumberOfMonthlyPayments has been initialized" );
            Assert.AreEqual( CopiedtoAccount.Payment.TotalPayment, 0M, "Payment has been initialized" );
            Assert.AreEqual( CopiedtoAccount.PreviousTotalCurrentAmtDue, 0M, "PreviousTotalCurrentAmtDue has been initialized" );
            Assert.AreEqual( CopiedtoAccount.RequestedPayment, 0M, "RequestedPayment has been initialized" );
            Assert.AreEqual( CopiedtoAccount.TotalCurrentAmtDue, 0M, "TotalCurrentAmtDue has been initialized" );
            Assert.AreEqual( CopiedtoAccount.TotalCharges, 0M, "TotalCharges has been initialized" );
            Assert.AreEqual( CopiedtoAccount.TotalPaid, 0M, "TotalPaid has been initialized" );            
            Assert.AreEqual( CopiedtoAccount.OriginalMonthlyPayment, 0M, "OriginalMonthlyPayment has been initialized" );        
            
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
        #endregion
    }
}