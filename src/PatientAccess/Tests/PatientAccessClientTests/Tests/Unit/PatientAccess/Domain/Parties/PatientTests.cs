using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain.Parties
{
    /// <summary>
    /// Summary description for PatientTests.
    /// </summary>

    [TestFixture()]
    public class PatientTests
    {
        #region Constants
        private const string
            PATIENT_F_NAME = "Sam",
            PATIENT_L_NAME = "Spade",
            PATIENT_MI = "L",
            PATIENT_SUFFIX = "SR";
        private static readonly SocialSecurityNumber
            PATIENT_SSN = new SocialSecurityNumber( "123121234" );
        private const long
            PATIENT_OID = 45L,
            PATIENT_MRN = 123456789;
        private static readonly Name
            PATIENT_NAME_SUFFIX = new Name( PATIENT_F_NAME, PATIENT_L_NAME, PATIENT_MI, PATIENT_SUFFIX );

        private static readonly Name
            PATIENT_NAME = new Name(PATIENT_F_NAME, PATIENT_L_NAME, PATIENT_MI),
            PATIENT_ALIAS_NAME = new Name("Santa", "Claus", "N");

        private static readonly DateTime
            PATIENT_DOB = new DateTime( 1955, 3, 5 );
        private static readonly Gender
            PATIENT_SEX = new Gender( 0, DateTime.Now, "Male", "M" );
        private const string
            FACILILTY_NAME = "DELRAY TEST HOSPITAL",
            FACILITY_CODE = "DEL";
        private static readonly Facility
            FACILITY = new Facility( PersistentModel.NEW_OID,
                                           PersistentModel.NEW_VERSION,
                                           FACILILTY_NAME,
                                           FACILITY_CODE );
        #endregion

        #region SetUp and TearDown PatientTests
        [TestFixtureSetUp()]
        public static void SetUpPatientTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownPatientTests()
        {
        }

        [SetUp()]
        public void SetUpPatients()
        {
            i_Patient = new Patient(
                PATIENT_OID,
                Patient.NEW_VERSION,
                PATIENT_NAME,
                PATIENT_MRN,
                PATIENT_DOB,
                PATIENT_SSN,
                PATIENT_SEX,
                FACILITY
                );
        }
        #endregion

        #region Test Methods
        [Test()]
        [Category( "Fast" )]
        public void TestConstructors()
        {
            Patient = new Patient(
                PATIENT_OID,
                Patient.NEW_VERSION,
                PATIENT_NAME,
                PATIENT_MRN,
                PATIENT_DOB,
                PATIENT_SSN,
                PATIENT_SEX,
                FACILITY
                );

            Patient patient = Patient;

            Assert.AreEqual(
                PATIENT_OID,
                patient.Oid,
                "Patient's Oid is " + PATIENT_OID
                );

            Assert.AreEqual(
                Patient.NEW_VERSION,
                patient.Timestamp,
                "Patient's Version is " + Patient.NEW_VERSION
                );

            Assert.AreEqual(
                PATIENT_F_NAME,
                patient.FirstName,
                "Patient's first name should be " + PATIENT_F_NAME
                );

            Assert.AreEqual(
                PATIENT_L_NAME,
                patient.LastName,
                "Patient's last name should be " + PATIENT_L_NAME
                );

            Assert.AreEqual(
                PATIENT_MI,
                patient.MiddleInitial,
                "Patient's middle initial should be " + PATIENT_MI
                );

            Assert.AreEqual(
                PATIENT_MRN,
                patient.MedicalRecordNumber,
                "Patient's medical record number should be " + PATIENT_MRN
                );

            Assert.AreEqual(
                PATIENT_DOB,
                patient.DateOfBirth,
                "Patient's date of birth should be " + PATIENT_DOB
                );

            Assert.AreEqual(
                PATIENT_SSN,
                patient.SocialSecurityNumber,
                "Patient's social security number should be " + PATIENT_SSN
                );

            Assert.AreEqual(
                PATIENT_SEX,
                patient.Sex,
                "Patient's sex should be " + PATIENT_SEX
                );


            ArrayList accounts = new ArrayList();
            accounts.Add( new Account() );
            accounts.Add( new Account() );

            Patient = new Patient( Patient );
            Patient.AddAccounts( accounts );

            Assert.AreEqual(
                2,
                Patient.Accounts.Count,
                "Patient has two accounts"
                );
        }

        [Test()]
        [Category( "Fast" )]
        public void TestAliases()
        {
            Assert.IsNotNull(
                Patient.Aliases,
                "Collection of Aliases should not be null"
                );

            Patient.AddAlias( new Name( "Joe", "Blow", "T" ) );
            Patient.AddAlias( new Name( "Santa", "Claus", "N" ) );

            Assert.IsTrue(
                Patient.Aliases.Count == 2,
                "There should be 2 aliases for this patient"
                );

            Assert.AreEqual(
                "Joe",
                ( (Name)Patient.Aliases[0] ).FirstName,
                "The first name of the first alias should be Joe"
                );

            Assert.AreEqual(
                "Claus",
                ( (Name)Patient.Aliases[1] ).LastName,
                "The last name of the second alias should be Claus"
                );

            Patient.Aliases.Add( new Name( "James", "Severance", "P" ) );

            Assert.IsTrue(
                Patient.Aliases.Count == 2,
                "There should still only be two aliases, the Aliases property is returns a clone, so adding to it does not add to the collection held onto by the Patient"
                );

            Patient.Aliases.RemoveAt( 0 );

            Assert.IsTrue(
                Patient.Aliases.Count == 2,
                "There should still be two aliases, the Aliases property should return a clone and not the real arraylist"
                );
        }

        [Test()]
        [Category( "Fast" )]
        public void TestPatientNameSuffix()
        {
            Patient patient = new Patient();
            patient.Name = PATIENT_NAME_SUFFIX;
            Assert.AreEqual(
                PATIENT_F_NAME,
                patient.FirstName,
                "Patient's first name should be " + PATIENT_F_NAME
                );

            Assert.AreEqual(
                PATIENT_L_NAME,
                patient.LastName,
                "Patient's last name should be " + PATIENT_L_NAME
                );

            Assert.AreEqual(
                PATIENT_MI,
                patient.MiddleInitial,
                "Patient's middle initial should be " + PATIENT_MI
                );

            Assert.AreEqual(
                PATIENT_SUFFIX,
                patient.Suffix,
                "Patient's Suffix should be " + PATIENT_SUFFIX
                );
        }

        [Ignore()] //"Causes Stack Overflow"
        public void TestAsPatient()
        {
            Assert.AreEqual(
                Patient,
                Patient.AsPatient(),
                "AsPatient should return 'this'"
                );
        }

        [Test()]
        [Category( "Fast" )]
        public void TestPatientsAccounts()
        {
            ArrayList accounts = new ArrayList();
            accounts.Add( new Account() );
            accounts.Add( new Account() );
            accounts.Add( new Account() );

            Assert.AreEqual(
                0,
                Patient.Accounts.Count,
                "Patient should initially have 0 accounts"
                );

            foreach ( IAccount account in accounts )
            {
                Patient.AddAccount( account );
            }

            Assert.AreEqual(
                3,
                Patient.Accounts.Count,
                "Patient should have three accounts"
                );
        }

        [Test()]
        [Category( "Fast" )]
        public void TestCopyAsPatient()
        {
            Guarantor g = new Guarantor();
            Patient p = g.CopyAsPatient();
        }

        [Test()]
        public void TestAddAutoGeneratedSpanCodes70And71With_WhenDataValidForSpanCodes70And71()
        {
            Account inpatientAccount1 = new Account();
            inpatientAccount1.AccountNumber = 12345;
            var currentDate = DateTime.Now;
            inpatientAccount1.AdmitDate = currentDate.Subtract( TimeSpan.FromDays( 5 ) );
            inpatientAccount1.DischargeDate = currentDate.Subtract( TimeSpan.FromDays( 2 ) );
            inpatientAccount1.KindOfVisit.Code = VisitType.INPATIENT;
            Facility facility = new Facility();
            facility.Code = "DEL";
            facility.Oid = 6;
            inpatientAccount1.Facility = facility;

            Account inpatientAccount2 = new Account();
            inpatientAccount2.AccountNumber = 67890;
            inpatientAccount2.AdmitDate = currentDate.Subtract( TimeSpan.FromDays( 10 ) );
            inpatientAccount2.DischargeDate = currentDate.Subtract( TimeSpan.FromDays( 7 ) );
            inpatientAccount2.KindOfVisit.Code = VisitType.INPATIENT;
            inpatientAccount2.Facility = facility;

            ArrayList accounts = new ArrayList();
            accounts.Add( inpatientAccount1 );
            accounts.Add( inpatientAccount2 );

            ISpanCodeBroker scBroker = new SpanCodeBrokerProxy();
            SpanCode spanCode70 = scBroker.SpanCodeWith( facility.Oid, SpanCode.QUALIFYING_STAY_DATES );
            SpanCode spanCode71 = scBroker.SpanCodeWith( facility.Oid, SpanCode.PRIOR_STAY_DATES );

            Account newAccount = new Account();
            newAccount.KindOfVisit.Code = VisitType.INPATIENT;
            newAccount.AdmitDate = currentDate.Subtract( TimeSpan.FromDays( 2 ) );
            newAccount.HospitalService.Code = "95";

            Insurance insurance = new Insurance();
            CommercialInsurancePlan plan = new CommercialInsurancePlan();
            plan.PlanSuffix = "304";

            Payor payor = new Payor();
            payor.Code = "VE";
            payor.Name = "PRIVATE PAY";

            plan.Payor = payor;

            Coverage primaryCoverage = new CommercialCoverage();
            primaryCoverage.CoverageOrder = new CoverageOrder( 1L, "Primary" );
            primaryCoverage.InsurancePlan = plan;

            insurance.AddCoverage( primaryCoverage );

            Patient.SelectedAccount = newAccount;
            Patient.AddAccounts( accounts );
            Patient.SelectedAccount.Insurance = insurance;

            inpatientAccount1.Insurance = insurance;
            inpatientAccount2.Insurance = insurance;

            Patient.AddAutoGeneratedSpanCodes70And71With( spanCode70, spanCode71 );

            Account selectedAccount = Patient.SelectedAccount;

            IList<OccurrenceSpan> occurrenceSpans = selectedAccount.OccurrenceSpans.Cast<OccurrenceSpan>().ToList();

            Assert.IsTrue( occurrenceSpans.Count == 2 );
            Assert.IsTrue( occurrenceSpans.Where( x => x != null && x.SpanCode.IsPriorStayDates ).Count() == 1 );
            Assert.IsTrue( occurrenceSpans.Where( x => x != null && x.SpanCode.IsQualifyingStayDate ).Count() == 1 );
        }

        [Test()]
        public void TestAddAutoGeneratedSpanCodes70And71With_WhenDatainValidForSpanCodes70And71()
        {
            Account inpatientAccount1 = new Account();
            inpatientAccount1.AccountNumber = 12345;
            var currentDate = DateTime.Now;
            inpatientAccount1.AdmitDate = currentDate.Subtract(TimeSpan.FromDays(5));
            inpatientAccount1.DischargeDate = currentDate.Subtract(TimeSpan.FromDays(2));
            inpatientAccount1.KindOfVisit.Code = VisitType.INPATIENT;
            Facility facility = new Facility();
            facility.Code = "DEL";
            facility.Oid = 6;
            inpatientAccount1.Facility = facility;

            Account inpatientAccount2 = new Account();
            inpatientAccount2.AccountNumber = 67890;
            inpatientAccount2.AdmitDate = currentDate.Subtract(TimeSpan.FromDays(10));
            inpatientAccount2.DischargeDate = currentDate.Subtract(TimeSpan.FromDays(7));
            inpatientAccount2.KindOfVisit.Code = VisitType.INPATIENT;
            inpatientAccount2.Facility = facility;

            ArrayList accounts = new ArrayList();
            accounts.Add(inpatientAccount1);
            accounts.Add(inpatientAccount2);

            ISpanCodeBroker scBroker = new SpanCodeBrokerProxy();
            SpanCode spanCode70 = scBroker.SpanCodeWith(facility.Oid, SpanCode.QUALIFYING_STAY_DATES);
            SpanCode spanCode71 = scBroker.SpanCodeWith(facility.Oid, SpanCode.PRIOR_STAY_DATES);

            Account newAccount = new Account();
            newAccount.KindOfVisit.Code = VisitType.INPATIENT;
            newAccount.AdmitDate = currentDate.Subtract(TimeSpan.FromDays(2));
            newAccount.HospitalService.Code = "95";

            Insurance insurance = new Insurance();
            CommercialInsurancePlan plan = new CommercialInsurancePlan();
            plan.PlanSuffix = "1LB";

            Payor payor = new Payor();
            payor.Code = "VE";
            payor.Name = "PRIVATE PAY";

            plan.Payor = payor;

            Coverage primaryCoverage = new CommercialCoverage();
            primaryCoverage.CoverageOrder = new CoverageOrder(1L, "Primary");
            primaryCoverage.InsurancePlan = plan;

            insurance.AddCoverage(primaryCoverage);

            Patient.SelectedAccount = newAccount;
            Patient.AddAccounts(accounts);
            Patient.SelectedAccount.Insurance = insurance;

            inpatientAccount1.Insurance = insurance;
            inpatientAccount2.Insurance = insurance;

            Patient.AddAutoGeneratedSpanCodes70And71With(spanCode70, spanCode71);

            Account selectedAccount = Patient.SelectedAccount;

            IList<OccurrenceSpan> occurrenceSpans = selectedAccount.OccurrenceSpans.Cast<OccurrenceSpan>().ToList();

            Assert.IsTrue(occurrenceSpans.Count == 2);
            Assert.IsFalse(occurrenceSpans.Where(x => x != null && x.SpanCode.IsPriorStayDates).Count() == 1);
            Assert.IsFalse(occurrenceSpans.Where(x => x != null && x.SpanCode.IsQualifyingStayDate).Count() == 1);
        }
        [Test()]
        public void TestAddAutoGeneratedSpanCodes70And71With_WhenDataInvalidForSpanCodes70And71()
        {
            Account patientAccount1 = new Account();
            patientAccount1.AccountNumber = 12345;
            var currentDate = DateTime.Now;
            patientAccount1.AdmitDate = currentDate.Subtract( TimeSpan.FromDays( 5 ) );
            patientAccount1.DischargeDate = currentDate.Subtract( TimeSpan.FromDays( 2 ) );
            patientAccount1.KindOfVisit.Code = VisitType.OUTPATIENT;
            Facility facility = new Facility();
            facility.Code = "DEL";
            facility.Oid = 6;
            patientAccount1.Facility = facility;

            Account patientAccount2 = new Account();
            patientAccount2.AccountNumber = 67890;
            patientAccount2.AdmitDate = currentDate.Subtract( TimeSpan.FromDays( 10 ) );
            patientAccount2.DischargeDate = currentDate.Subtract( TimeSpan.FromDays( 7 ) );
            patientAccount2.KindOfVisit.Code = VisitType.OUTPATIENT;
            patientAccount2.Facility = facility;

            ArrayList accounts = new ArrayList();
            accounts.Add( patientAccount1 );
            accounts.Add( patientAccount2 );

            ISpanCodeBroker scBroker = new SpanCodeBrokerProxy();
            SpanCode spanCode70 = scBroker.SpanCodeWith( facility.Oid, SpanCode.QUALIFYING_STAY_DATES );
            SpanCode spanCode71 = scBroker.SpanCodeWith( facility.Oid, SpanCode.PRIOR_STAY_DATES );

            Account newAccount = new Account();
            newAccount.KindOfVisit.Code = VisitType.INPATIENT;
            newAccount.AdmitDate = currentDate.Subtract( TimeSpan.FromDays( 2 ) );

            Patient.SelectedAccount = newAccount;
            Patient.AddAccounts( accounts );

            Patient.AddAutoGeneratedSpanCodes70And71With( spanCode70, spanCode71 );

            Account selectedAccount = Patient.SelectedAccount;

            IList<OccurrenceSpan> occurrenceSpans = selectedAccount.OccurrenceSpans.Cast<OccurrenceSpan>().ToList();

            Assert.IsTrue( occurrenceSpans.Count == 2 );
            Assert.IsTrue( occurrenceSpans.Where( x => x != null && x.SpanCode.IsPriorStayDates ).Count() == 0 );
            Assert.IsTrue( occurrenceSpans.Where( x => x != null && x.SpanCode.IsQualifyingStayDate ).Count() == 0 );
        }

        [Test()]
        public void TestAddAutoGeneratedSpanCodes70And71With_WhenDataValidForSpanCode71AndInvalidFor70()
        {
            Account inpatientAccount1 = new Account();
            inpatientAccount1.AccountNumber = 12345;
            var currentDate = DateTime.Now;
            inpatientAccount1.AdmitDate = currentDate.Subtract( TimeSpan.FromDays( 5 ) );
            inpatientAccount1.DischargeDate = currentDate.Subtract( TimeSpan.FromDays( 2 ) );
            inpatientAccount1.KindOfVisit.Code = VisitType.INPATIENT;
            Facility facility = new Facility();
            facility.Code = "DEL";
            facility.Oid = 6;
            inpatientAccount1.Facility = facility;

            Account inpatientAccount2 = new Account();
            inpatientAccount2.AccountNumber = 67890;
            inpatientAccount2.AdmitDate = currentDate.Subtract( TimeSpan.FromDays( 10 ) );
            inpatientAccount2.DischargeDate = currentDate.Subtract( TimeSpan.FromDays( 7 ) );
            inpatientAccount2.KindOfVisit.Code = VisitType.OUTPATIENT;
            inpatientAccount2.Facility = facility;

            ArrayList accounts = new ArrayList();
            accounts.Add( inpatientAccount1 );
            accounts.Add( inpatientAccount2 );

            ISpanCodeBroker scBroker = new SpanCodeBrokerProxy();
            SpanCode spanCode70 = scBroker.SpanCodeWith( facility.Oid, SpanCode.QUALIFYING_STAY_DATES );
            SpanCode spanCode71 = scBroker.SpanCodeWith( facility.Oid, SpanCode.PRIOR_STAY_DATES );

            Account newAccount = new Account();
            newAccount.KindOfVisit.Code = VisitType.INPATIENT;
            newAccount.AdmitDate = currentDate.Subtract( TimeSpan.FromDays( 2 ) );
            newAccount.HospitalService.Code = "95";

            Patient.SelectedAccount = newAccount;
            Patient.AddAccounts( accounts );

            Patient.AddAutoGeneratedSpanCodes70And71With( spanCode70, spanCode71 );

            Account selectedAccount = Patient.SelectedAccount;

            IList<OccurrenceSpan> occurrenceSpans = selectedAccount.OccurrenceSpans.Cast<OccurrenceSpan>().ToList();

            Assert.IsTrue( occurrenceSpans.Count == 2 );
            Assert.IsTrue( occurrenceSpans.Where( x => x != null && x.SpanCode.IsPriorStayDates ).Count() == 1 );
            Assert.IsTrue( occurrenceSpans.Where( x => x != null && x.SpanCode.IsQualifyingStayDate ).Count() == 0 );
        }

        [Test()]
        [Category( "Fast" )]
        public void TestClearPriorSystemGeneratedOccurrenceSpans_WhenVisitIsOutpatientAndSpans71And74_ShouldClear71()
        {
            AddOccurrenceSpans71And74ToPatient();
            Patient.SelectedAccount.KindOfVisit.Code = VisitType.OUTPATIENT;

            Patient.ClearPriorSystemGeneratedOccurrenceSpans();

            Account selectedAccount = Patient.SelectedAccount;

            IList<OccurrenceSpan> occurrenceSpans = selectedAccount.OccurrenceSpans.Cast<OccurrenceSpan>().ToList();

            Assert.IsTrue( occurrenceSpans.Count == 2 );
            Assert.IsTrue( occurrenceSpans.Where( x => x != null && x.SpanCode.IsPriorStayDates ).Count() == 0 );
            Assert.IsTrue( occurrenceSpans.Where( x => x != null && x.SpanCode.IsNoncoveredLevelOfCare ).Count() == 1 );
        }


        [Test()]
        [Category( "Fast" )]
        public void TestClearPriorSystemGeneratedOccurrenceSpans_WhenVisitIsInpatientAndSpans74And71IsSystemGenerated_ShouldClear71()
        {
            AddOccurrenceSpans71And74ToPatient();
            Patient.SelectedAccount.KindOfVisit.Code = VisitType.INPATIENT;

            Patient.ClearPriorSystemGeneratedOccurrenceSpans();

            Account selectedAccount = Patient.SelectedAccount;

            IList<OccurrenceSpan> occurrenceSpans = selectedAccount.OccurrenceSpans.Cast<OccurrenceSpan>().ToList();

            Assert.IsTrue( occurrenceSpans.Count == 2 );
            Assert.IsTrue( occurrenceSpans.Where( x => x != null && x.SpanCode.IsPriorStayDates ).Count() == 0 );
            Assert.IsTrue( occurrenceSpans.Where( x => x != null && x.SpanCode.IsNoncoveredLevelOfCare ).Count() == 1 );
        }

        [Test()]
        [Category( "Fast" )]
        public void TestClearPriorSystemGeneratedOccurrenceSpans_WhenVisitIsInpatientAndSpans74And71IsNotSystemGenerated_ShouldNotClear71()
        {
            AddOccurrenceSpans71And74ToPatient();
            Patient.SelectedAccount.KindOfVisit.Code = VisitType.INPATIENT;
            ( (OccurrenceSpan)Patient.SelectedAccount.OccurrenceSpans[0] ).IsSystemGenerated = false;

            Patient.ClearPriorSystemGeneratedOccurrenceSpans();

            Account selectedAccount = Patient.SelectedAccount;

            IList<OccurrenceSpan> occurrenceSpans = selectedAccount.OccurrenceSpans.Cast<OccurrenceSpan>().ToList();

            Assert.IsTrue( occurrenceSpans.Count == 2 );
            Assert.IsTrue( occurrenceSpans.Where( x => x != null && x.SpanCode.IsPriorStayDates ).Count() == 1 );
            Assert.IsTrue( occurrenceSpans.Where( x => x != null && x.SpanCode.IsNoncoveredLevelOfCare ).Count() == 1 );
        }

        [Test()]
        [Category( "Fast" )]
        public void TestClearPriorSystemGeneratedOccurrenceSpans_WhenActivityIsActivatePreRegistrationAndSpans71And74_ShouldClear71()
        {
            AddOccurrenceSpans71And74ToPatient();
            Patient.SelectedAccount.KindOfVisit.Code = VisitType.PREREG_PATIENT;
            Patient.SelectedAccount.Activity = new RegistrationActivity();
            Patient.SelectedAccount.Activity.AssociatedActivityType = typeof( ActivatePreRegistrationActivity );

            Patient.ClearPriorSystemGeneratedOccurrenceSpans();

            Account selectedAccount = Patient.SelectedAccount;
            IList<OccurrenceSpan> occurrenceSpans = selectedAccount.OccurrenceSpans.Cast<OccurrenceSpan>().ToList();

            Assert.IsTrue( occurrenceSpans.Count == 2 );
            Assert.IsTrue( occurrenceSpans.Where( x => x != null && x.SpanCode.IsPriorStayDates ).Count() == 0 );
            Assert.IsTrue( occurrenceSpans.Where( x => x != null && x.SpanCode.IsNoncoveredLevelOfCare ).Count() == 1 );
        }

        [Test()]
        [Category("Fast")]
        public void TestSetDisplayPatient_SetName()
        {
            var patient = GetPatient();
            patient.SetPatientContextHeaderData();
            Assert.AreEqual(patient.Name,patient.PatientContextHeaderData.PatientName,"Patient name not set");
        }

        [Test()]
        [Category("Fast")]
        public void TestSetDisplayPatient_SetDateOfBirth()
        {
            var patient = GetPatient();
            patient.SetPatientContextHeaderData();
            Assert.AreEqual(patient.DateOfBirth, patient.PatientContextHeaderData.DOB, "Patient DOB not set");
        }

        [Test()]
        [Category("Fast")]
        public void TestSetDisplayPatient_SetMedicalRecordNumber()
        {
            var patient = GetPatient();
            patient.SetPatientContextHeaderData();
            Assert.AreEqual(patient.MedicalRecordNumber, patient.PatientContextHeaderData.MRN, "Patient MRN not set");
        }

        [Test()]
        [Category("Fast")]
        public void TestSetDisplayPatient_SetSocialSecurityNumber()
        {
            var patient = GetPatient();
            patient.SetPatientContextHeaderData();
            Assert.AreEqual(patient.SocialSecurityNumber, patient.PatientContextHeaderData.SSN, "Patient SSN not set");
        }

        [Test()]
        [Category("Fast")]
        public void TestSetDisplayPatient_SetGender()
        {
            var patient = GetPatient();
            patient.SetPatientContextHeaderData();
            Assert.AreEqual(patient.Sex, patient.PatientContextHeaderData.Sex, "Patient Gender not set");
        }

        [Test()]
        [Category("Fast")]
        public void TestSetDisplayPatient_SetAlias()
        {
            var patient = GetPatient();
            patient.SetPatientContextHeaderData();
            Assert.AreEqual(((Name)patient.Aliases[0]).AsFormattedName(), patient.PatientContextHeaderData.AKA, "Patient Alias name not set");
        }

        #endregion

        #region Support Methods
        private void AddOccurrenceSpans71And74ToPatient()
        {
            SpanCode spanCode1 = new SpanCode {Code = SpanCode.PRIOR_STAY_DATES};

            OccurrenceSpan occSpan1 = new OccurrenceSpan {SpanCode = spanCode1, IsSystemGenerated = true};

            SpanCode spanCode2 = new SpanCode {Code = SpanCode.NONCOVERED_LEVEL_OF_CARE};

            OccurrenceSpan occSpan2 = new OccurrenceSpan {SpanCode = spanCode2, IsSystemGenerated = false};

            Account newAccount = new Account();
            Patient.SelectedAccount = newAccount;
            Patient.SelectedAccount.OccurrenceSpans.Add( occSpan1 );
            Patient.SelectedAccount.OccurrenceSpans.Add( occSpan2 );
        }

        private Patient Patient
        {
            get
            {
                return i_Patient;
            }
            set
            {
                i_Patient = value;
            }
        }

        private Patient GetPatient()
        {
            var patient = new Patient
            {
                Name = PATIENT_NAME,
                DateOfBirth = PATIENT_DOB,
                MedicalRecordNumber = PATIENT_MRN,
                Sex = PATIENT_SEX,
                SocialSecurityNumber = PATIENT_SSN
            };
            patient.AddAlias(PATIENT_ALIAS_NAME);
            return patient;
        }
        #endregion

        #region Data Elements
        private static Patient i_Patient;
        #endregion
    }
}