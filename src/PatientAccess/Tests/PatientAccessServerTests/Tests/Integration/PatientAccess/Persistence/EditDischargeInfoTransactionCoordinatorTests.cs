using System;
using Extensions.PersistenceCommon;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class EditDischargeInfoTransactionCoordinatorTests
    {
        #region Constants

        const string HSPCODE = "ACO";
        const long ACO_FACILITY_ID = 900;
        const int MOD_CHECK_DIGIT = 10;
        private const string
            PATIENT_F_NAME = "TESTTRANSFER",
            PATIENT_L_NAME = "TEST";

        private static readonly string PATIENT_MI = string.Empty;

        private const long
            PATIENT_OID = 45L,
            PATIENT_MRN = 785673;
        private SocialSecurityNumber
            PATIENT_SSN = new SocialSecurityNumber("124556789");
        private Name
            PATIENT_NAME = new Name(PATIENT_F_NAME, PATIENT_L_NAME, PATIENT_MI);
        private DateTime
            PATIENT_DOB = new DateTime(1965, 5, 5);
        private Gender
            PATIENT_SEX = new Gender(1, DateTime.Now, "Male", "M");

        #endregion

        #region SetUp and TearDown EditDischargeTransactionCoordinatorTests
        [TestFixtureSetUp()]
        public static void SetUpEditDischargeInfoTransactionCoordinatorTests()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            i_Facility = facilityBroker.FacilityWith(ACO_FACILITY_ID);
        }

        [TestFixtureTearDown()]
        public static void TearDownEditDischargeInfoTransactionCoordinatorTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestTransactionCoordinatorFor()
        {
            Activity currentActivity =
                new EditDischargeDataActivity();
            TransactionCoordinator editDischargeInfoTransactionCoordinator =
                TransactionCoordinator.TransactionCoordinatorFor(currentActivity);
            Assert.IsNotNull(editDischargeInfoTransactionCoordinator);
            Assert.AreEqual(typeof(EditDischargeInfoTransactionCoordinator),
                             editDischargeInfoTransactionCoordinator.GetType());

            TransactionCoordinator nullTransactionCoordinator =
                TransactionCoordinator.TransactionCoordinatorFor(null);
            Assert.IsNull(nullTransactionCoordinator);
        }

        [Test()]
        public void TestGetInsertStrategy()
        {

                Patient patient = new Patient(
                    PATIENT_OID,
                    Patient.NEW_VERSION,
                    this.PATIENT_NAME,
                    PATIENT_MRN,
                    this.PATIENT_DOB,
                    this.PATIENT_SSN,
                    this.PATIENT_SEX,
                    i_Facility
                    );
                PlaceOfWorship placeOfWorship = new PlaceOfWorship(
                    PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "St. Mary's", "2");
                Ethnicity ethnicity = new Ethnicity(
                    PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "HISPANIC", "3");
                Race race = new Race(
                    PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "WHITE", "1");
                Religion religion = new Religion(
                    PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "JEWISH", string.Empty);
                Language language = new Language(
                    PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "YIDDISH", "1");
                MaritalStatus maritalStatus = new MaritalStatus(
                    PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "MARRIED", "1");
                patient.PlaceOfWorship = placeOfWorship;
                patient.Race = race;
                patient.Ethnicity = ethnicity;
                patient.MaritalStatus = maritalStatus;
                patient.Language = language;

                AccountProxy proxy = new AccountProxy(51581,
                                                       patient,
                                                       DateTime.Now,
                                                       DateTime.Now,
                                                       new VisitType(1, ReferenceValue.NEW_VERSION, VisitType.INPATIENT_DESC, VisitType.INPATIENT),
                                                       i_Facility,
                                                       new FinancialClass(20, ReferenceValue.NEW_VERSION, "COMMERCIAL INS", "20"),
                                                       new HospitalService(ReferenceValue.NEW_OID, ReferenceValue.NEW_VERSION, "CORRECTIONAL INPUT", "37"),
                                                       VisitType.INPATIENT_DESC,
                                                       false);

                IAccountBroker accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
                Account account = accountBroker.AccountFor(proxy);
                account.DischargeDate = DateTime.Now;
                account.DischargeDate = DateTime.Now;
                //Location Information  
                Location location = new Location();
                NursingStation nursingStation = new NursingStation(PersistentModel.NEW_OID, DateTime.Now, "6N", "6N");
                location.NursingStation = nursingStation;
                Room room = new Room(PersistentModel.NEW_OID, DateTime.Now, "610", "610");
                location.Room = room;
                Bed bed = new Bed(PersistentModel.NEW_OID, DateTime.Now, "A", "A");
                Accomodation accomodation = new Accomodation(PersistentModel.NEW_OID, DateTime.Now, "PRIVATE", "01");
                location.Bed = bed;
                location.Bed.Accomodation = accomodation;

                account.Location = location;
                account.Activity = new EditDischargeDataActivity();

                DischargeDisposition dischargDisp = new DischargeDisposition(ReferenceValue.NEW_OID, DateTime.Now, "HOME HEALTH", "N");
                account.DischargeDisposition = dischargDisp;

                EditDischargeInfoTransactionCoordinator editDischargeInfoTransactionCoordinator =
                    new EditDischargeInfoTransactionCoordinator();
                editDischargeInfoTransactionCoordinator.NumberOfInsurances = 0;
                editDischargeInfoTransactionCoordinator.NumberOfNonInsurances = 0;
                editDischargeInfoTransactionCoordinator.NumberOfOtherRecs = 0;
                editDischargeInfoTransactionCoordinator.Account = account;
                editDischargeInfoTransactionCoordinator.AppUser = account.Activity.AppUser;

                SqlBuilderStrategy[] strategy = editDischargeInfoTransactionCoordinator.InsertStrategies;


        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        private static Facility i_Facility;

        #endregion

    }
}