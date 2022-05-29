using System;
using System.Collections;
using Extensions.PersistenceCommon;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence;
using PatientAccess.Persistence.Factories;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class TransactionBrokerTests : AbstractBrokerTests
    {
        #region SetUp and TearDown PreregistrationTransactionCoordinatorTests

        [TestFixtureSetUp()]
        public static void SetUpTransactionBrokerTests()
        {
            var facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();

            facility = facilityBroker.FacilityWith(FACILITY_CODE);
            i_Facility = facilityBroker.FacilityWith(900);
        }

        [TestFixtureTearDown()]
        public static void TearDownTransactionBrokerTests()
        {
        }

        #endregion

        #region Test Methods

        [Test()]
        [Ignore()] //"Leave this here for white box testing. Data changes daily so an automated test is not practical"
        public void TestIsTXNAllowed()
        {
            var account = new Account();
            account.AccountNumber = 100082;
            var facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            Facility l_Facility = facilityBroker.FacilityWith(900);
            account.Facility = l_Facility;
            Activity activity = new TransferInToOutActivity();
            var accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();

            bool allowed = accountBroker.IsTxnAllowed(account.Facility.Code, account.AccountNumber, activity);

            Console.WriteLine(allowed.ToString());
        }

        [Test()]
        public void TestIsNewTransaction()
        {
            ITransactionBroker transactionBroker = new TransactionBroker();
            var NewTransSqlStatements = new ArrayList();
            string HPADAPMPNewTransStr =
                "2323,&*%P@#$,2323,sds2323,&*%S@#$, $#L@%,9S5',&*@&Q%,,0,'1',0,2323,$#G%&*,9463388,$#P@%&,0,";
            NewTransSqlStatements.Add(HPADAPMPNewTransStr);
            string HPADAPGMStrNewTransStr =
                "fgf4545,&*%P@#$,2323, $#L@%12, we,&*@&Q%, 34we,3434,$#G%&*, drer3434, err,$#P@%&,34534,";
            NewTransSqlStatements.Add(HPADAPGMStrNewTransStr);
            string HPADAPIDStrNewTransStr =
                "cvkcv767,&*%P@#$,4545,$#L@%1212,sd7656,&*%S@#$, desd,&*@&Q%, sd34,ererr,$#G%&*,sdd3434,sdsd,$#P@%&";
            NewTransSqlStatements.Add(HPADAPIDStrNewTransStr);
            string HPADAPNMStrNewTransStr =
                "xcjh8768,&*%P@#$,344,$#L@%23233,&*@&Q%,23sdsdsd,sd76sd,&*%S@#$,34344,dfdfdff3,$#P@%&, 3434,dfdf,$#G%&*,34dfdf,3434,ddrr,3434,";
            NewTransSqlStatements.Add(HPADAPNMStrNewTransStr);

            Assert.IsTrue(transactionBroker.IsNewTransaction(NewTransSqlStatements));

            var OldTransSqlStatements = new ArrayList();
            string HPADAPMPOldTransStr = "','GA',''45fgfg,'','',14564984,55450,'',0,0,0,'','','BHW SHEET METAL COM','";
            OldTransSqlStatements.Add(HPADAPMPOldTransStr);
            string HPADAPGMStrOldTransStr = "','454554GA'4,'','','',1467984,0,'',0,0,0,'','','BHW SHEET METAL COM','";
            OldTransSqlStatements.Add(HPADAPGMStrOldTransStr);
            string HPADAPIDStrOldTransStr = "','GA','','','',14454567984,0,'',0,0,0,'','','BHW SHEET METAL COM','";
            OldTransSqlStatements.Add(HPADAPIDStrOldTransStr);
            string HPADAPNMStrOldTransStr = "','GA','',er3454545'','',1467984,0,'',0,0,0,'','','BHW SHEET METAL COM','";
            OldTransSqlStatements.Add(HPADAPNMStrOldTransStr);

            Assert.IsFalse(transactionBroker.IsNewTransaction(OldTransSqlStatements));
        }

        [Test()]
        public void TestInsertAliasNames()
        {
            try
            {
                Account anAccount = GetAccount();

                var patEmpr = new Employer();
                employment = new Employment(anAccount.Patient);
                employment.Employer = patEmpr;
                patEmpr.Name = "AMERICAN EXPRESS";
                var alias1 = new Name("Cl", "Fr", "MI", "SU", "Y");
                var alias2 = new Name("Cl2", "Fr2", "m", "S", "N");
                anAccount.Patient.AddAlias(alias1);
                anAccount.Patient.AddAlias(alias2);

                anAccount.Patient.Employment = new Employment(anAccount.Patient);
                anAccount.Patient.DriversLicense = new DriversLicense(
                    "72368223828",
                    new State(PersistentModel.NEW_OID, DateTime.Now, "Texas"));
                ArrayList SqlStrings = BuildSql(anAccount);
                Execute(anAccount, SqlStrings);
            }
            catch (Exception ex)
            {
                Assert.Fail("Save Failed: " + ex.Message);
            }
        }

        [Test()]
        public void TestInsertPregnancyIndicator()
        {
            try
            {
                Account anAccount = GetAccount();
                anAccount.Pregnant = YesNoFlag.Yes;
                ArrayList SqlStrings  = BuildSql(anAccount);
                Execute(anAccount, SqlStrings);

            }
            catch (Exception ex)
            {
                Assert.Fail("Save Failed: " + ex.Message);
            }
        }
      
        private ArrayList  BuildSql(Account anAccount)
        {
            
            i_PatientInsertStrategy = SqlBuilderStrategyFactory.CreatePatientInsertStrategy();
            i_PatientInsertStrategy.TransactionFileId = "PG";
            i_PatientInsertStrategy.PreDischargeFlag = "D";
            return i_PatientInsertStrategy.BuildSqlFrom(anAccount, transactionKeys);

        }
        private void Execute( Account anAccount, ArrayList sqlStrings  )
        {
            var preRegTransCoord =
                 new PreregistrationTransactionCoordinator(anAccount);

            preRegTransCoord.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTION;
            preRegTransCoord.NumberOfInsurances = anAccount.Insurance.Coverages.Count;

            preRegTransCoord.AppUser = anAccount.Activity.AppUser;
            ITransactionBroker transactionBroker = new TransactionBroker();
            transactionBroker.TransactionCoordinator = preRegTransCoord;
            transactionBroker.Execute(sqlStrings, i_Facility, anAccount, true);
        }
        [Test()]
        public void TestSaveOfBadTxn()
        {
            var anAccount = new Account();

            var preRegTransCoord =
                new PreregistrationTransactionCoordinator(anAccount);
            preRegTransCoord.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTION;

            ITransactionBroker transactionBroker = new TransactionBroker();
            try
            {
                transactionBroker.ExecuteTransaction(preRegTransCoord);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " " + ex.StackTrace);
                Assert.IsTrue(true, "Saving of blank account save did not fail as expected");
            }

            anAccount.Patient = new Patient();
            try
            {
                transactionBroker.ExecuteTransaction(preRegTransCoord);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " " + ex.StackTrace);
                Assert.IsTrue(true, "Saving of an account with Empty Patient save did not fail as expected");
            }

            anAccount.Activity = new PreRegistrationActivity();
            try
            {
                transactionBroker.ExecuteTransaction(preRegTransCoord);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " " + ex.StackTrace);
                Assert.IsTrue(true, "Saving of an account with Empty Patient save did not fail as expected");
            }
        }

        [Test()]
        [Ignore]
        public void TestSave()
        {
            try
            {
                var criteria = new PatientSearchCriteria(
                    HSPCODE,
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    null,
                    PatientSearchCriteria.NO_MONTH,
                    PatientSearchCriteria.NO_YEAR,
                    TEST_MRN,
                    String.Empty
                    );

                var patientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();

                PatientSearchResponse patientSearchResponse = patientBroker.GetPatientSearchResponseFor(criteria);

                Patient patient = patientBroker.PatientFrom(patientSearchResponse.PatientSearchResults[0]);
                var proxy = new AccountProxy(64238,  
                                             patient,
                                             DateTime.Now,
                                             DateTime.Now,
                                             new VisitType(0, ReferenceValue.NEW_VERSION, VisitType.PREREG_PATIENT_DESC,
                                                           VisitType.PREREG_PATIENT),
                                             i_Facility,
                                             new FinancialClass(279, ReferenceValue.NEW_VERSION, "COMMERCIAL INS", "20"),
                                             new HospitalService(61, ReferenceValue.NEW_VERSION, "OTHER ICU", "48"),
                                             "PRE-REGISTER",
                                             false);
                var accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
                Account account = accountBroker.AccountFor(proxy);
                account.Activity = new PreRegistrationActivity();

                User appUser = User.GetCurrent();
                appUser.PBAREmployeeID = "PAUSRT03";
                appUser.WorkstationID = string.Empty;
                appUser.UserID = "PAUSRT03";

                //these are normally set by the Save Method
                account.Activity.AppUser = appUser;

                // reset the accountnumber by getting a new one.

                var facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility facility = facilityBroker.FacilityWith(900);

                account.AccountNumber = accountBroker.GetNewAccountNumberFor(facility);
                Console.WriteLine("New Account Created = " + account.AccountNumber);
                account.AdmitDate = DateTime.Now;
                account.IsNew = true;

                // reset this to pre-reg
                account.KindOfVisit = new VisitType(0, ReferenceValue.NEW_VERSION,
                                                    "PREREG_PATIENT", VisitType.PREREG_PATIENT);

                // reset the insurance records to be new
                foreach (Coverage coverage in account.Insurance.Coverages)
                {
                    coverage.IsNew = true;
                }

                var preRegTransCoord =
                    new PreregistrationTransactionCoordinator(account);
                preRegTransCoord.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTION;
                preRegTransCoord.NumberOfInsurances = account.Insurance.Coverages.Count;

                preRegTransCoord.AppUser = account.Activity.AppUser;

                ITransactionBroker transactionBroker = new TransactionBroker();
                transactionBroker.ExecuteTransaction(preRegTransCoord);
                Assert.IsTrue(true, "Saving of account Succeeded");
            }
            catch (Exception ex)
            {
                Assert.Fail("Saving of account failed." + ex.Message + " " + ex);
            }
        }

        [Test()]
        [ExpectedException(typeof (TransactionUpdateException))]
        public void TestTransactionUpdateException()
        {
            try
            {
                var facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility facility = facilityBroker.FacilityWith(TEST_FACILITY_ID);

                var anAccount = new Account();
                anAccount.AccountNumber = 7051667;
                anAccount.AdmitDate = DateTime.Parse("Dec 17, 2001");
                anAccount.ClergyVisit.SetYes("YES");
                anAccount.DischargeDate = DateTime.Parse("Jan 21, 2002");
                anAccount.DerivedVisitType = "INPATENT";

                var visitType = new VisitType(
                    PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, VisitType.INPATIENT_DESC, VisitType.INPATIENT);
                anAccount.KindOfVisit = visitType;

                var patient = new Patient(
                    TEST_PATIENT_OID,
                    Patient.NEW_VERSION,
                    TEST_PATIENT_NAME,
                    MEDICAL_RECORD_NUMBER,
                    TEST_PATIENT_DOB,
                    TEST_PATIENT_SSN,
                    TEST_PATIENT_SEX,
                    facility
                    );
                patient.FirstName = "SOME REDICULOUSLY LONG NAME THAT COULD NEVER EVER BE VALID";
                anAccount.HospitalService = hsp;

                employment = new Employment(patient);
                var patEmpr = new Employer();
                employment.Employer = patEmpr;
                patEmpr.Name = "AMERICAN EXPRESS";
                var alias1 = new Name("Cl", "Fr", "MI", "SU", "Y");
                var alias2 = new Name("Cl2", "Fr2", "m", "S", "N");
                patient.AddAlias(alias1);
                patient.AddAlias(alias2);

                anAccount.Patient = patient;
                anAccount.Facility = facility;

                anAccount.Patient.Employment = new Employment(patient);
                anAccount.Patient.DriversLicense = new DriversLicense(
                    "72368223828",
                    new State(PersistentModel.NEW_OID, DateTime.Now, "Texas"));

                transactionKeys = new TransactionKeys(12, 24, 36, 0, 365);
                Activity currentActivity =
                    new PreDischargeActivity();
                anAccount.Activity = currentActivity;
                i_PatientInsertStrategy = SqlBuilderStrategyFactory.CreatePatientInsertStrategy();
                i_PatientInsertStrategy.TransactionFileId = "PG";
                i_PatientInsertStrategy.PreDischargeFlag = "D";
                i_PatientInsertStrategy.UserSecurityCode = "KEVN";
                var preRegTransCoord =
                    new PreregistrationTransactionCoordinator(anAccount);
                preRegTransCoord.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTION;
                preRegTransCoord.NumberOfInsurances = anAccount.Insurance.Coverages.Count;

                preRegTransCoord.AppUser = anAccount.Activity.AppUser;
                ArrayList sqlStrings =
                    i_PatientInsertStrategy.BuildSqlFrom(anAccount, transactionKeys);

                ITransactionBroker transactionBroker = new TransactionBroker();
                transactionBroker.TransactionCoordinator = preRegTransCoord;
                transactionBroker.ExecuteTransaction(preRegTransCoord);
            }
            catch (TransactionUpdateException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        [Test()]
        public void TestLogTransactionHeaderNumbersFor()
        {
            int numberOfNonInsurances = 2;
            int numberOfInsurances = 4;
            int numberOfOthers = 3;

            int patientRecordNumber = 12;
            int logNumber = 25;
            int insuranceRecordNumber = 36;
            int otherRecordNumber = 5;
            int daysSinceAdmission = 365;

            transactionKeys = new TransactionKeys(patientRecordNumber - numberOfNonInsurances,
                                                  logNumber - 1,
                                                  insuranceRecordNumber - numberOfInsurances,
                                                  otherRecordNumber - numberOfOthers,
                                                  daysSinceAdmission);
            
            ITransactionBroker transactionBroker = new TransactionBroker();
            transactionBroker.TransactionKeys = transactionKeys;

            string logString1 = transactionBroker.LogTransactionHeaderNumbersFor
                ("DEL", numberOfNonInsurances,
                 numberOfInsurances, numberOfOthers,
                 transactionBroker);
            Console.WriteLine(logString1);

            numberOfNonInsurances = 0;
            numberOfInsurances = 0;
            numberOfOthers = 0;

            string logString2 = transactionBroker.LogTransactionHeaderNumbersFor
                ("DEL", numberOfNonInsurances,
                 numberOfInsurances, numberOfOthers,
                 transactionBroker);
            Console.WriteLine(logString2);

            numberOfNonInsurances = 3;

            string logString3 = transactionBroker.LogTransactionHeaderNumbersFor
                ("DEL", numberOfNonInsurances,
                 numberOfInsurances, numberOfOthers,
                 transactionBroker);
            Console.WriteLine(logString3);
        }

        #endregion

        #region Support Methods

        #endregion

        private Account GetAccount()
        {
            var anAccount = new Account();
            anAccount.AccountNumber = 23456789;
            anAccount.AdmitDate = DateTime.Parse("Dec 17, 2001");
            anAccount.ClergyVisit.SetYes("YES");
            anAccount.DischargeDate = DateTime.Parse("Jan 21, 2002");
            anAccount.DerivedVisitType = "INPATENT";

            var visitType = new VisitType(
                PersistentModel.NEW_OID, PersistentModel.NEW_VERSION,
                VisitType.INPATIENT_DESC, VisitType.INPATIENT);
            anAccount.KindOfVisit = visitType;

            var patient = new Patient(
                PATIENT_OID,
                Patient.NEW_VERSION,
                PATIENT_NAME,
                PATIENT_MRN,
                PATIENT_DOB,
                PATIENT_SSN,
                PATIENT_SEX,
                facility
                );

            patient.IsNew = false;
            anAccount.Facility = facility;
            anAccount.HospitalService = hsp;
            anAccount.Patient = patient;
            Activity currentActivity =
                new PreDischargeActivity();
            anAccount.Activity = currentActivity;

            return anAccount;
        }

        #region Data Elements

        private static Facility i_Facility;
        private Employment employment;

        private HospitalService hsp = new HospitalService(
            PersistentModel.NEW_OID, DateTime.Now, "HSVC 60", "99");

        private PatientInsertStrategy i_PatientInsertStrategy;

        private DateTime
            PATIENT_DOB = new DateTime(1955, 3, 5);

        private Name
            PATIENT_NAME = new Name(PATIENT_F_NAME, PATIENT_L_NAME, PATIENT_MI);

        private Gender
            PATIENT_SEX = new Gender(0, DateTime.Now, "Female", "F");

        private SocialSecurityNumber
            PATIENT_SSN = new SocialSecurityNumber("123121234");

        private TransactionKeys transactionKeys;

        #endregion

        #region Constants

        private new const string
            FACILITY_CODE = "DEL";

        private const string HSPCODE = "ACO";

        private const long
            MEDICAL_RECORD_NUMBER = 31463;

        private const int NUMBER_OF_NON_INSURANCE_TRANSACTION = 2;

        private const string
            PATIENT_F_NAME = "CLAIRE",
            PATIENT_L_NAME = "FRIED";

        private const long
            PATIENT_MRN = 24004;

        private const long
            PATIENT_OID = 45L;

        private const long
            TEST_FACILITY_ID = 98;

        private const string TEST_MRN = "785433";

        private const string
            TEST_PATIENT_F_NAME = "HUGH L",
            TEST_PATIENT_L_NAME = "ABBOTT";

        private const long
            TEST_PATIENT_OID = 77L;

        private static readonly string PATIENT_MI = string.Empty;

        private static readonly string TEST_PATIENT_MI = string.Empty;
        private static Facility facility = null;

        private DateTime
            TEST_PATIENT_DOB = new DateTime(1975, 1, 1);

        private Name
            TEST_PATIENT_NAME = new Name(TEST_PATIENT_F_NAME, TEST_PATIENT_L_NAME, TEST_PATIENT_MI);

        private Gender
            TEST_PATIENT_SEX = new Gender(0, DateTime.Now, "Female", "F");

        private SocialSecurityNumber
            TEST_PATIENT_SSN = new SocialSecurityNumber("12345678");

        #endregion
    }
}