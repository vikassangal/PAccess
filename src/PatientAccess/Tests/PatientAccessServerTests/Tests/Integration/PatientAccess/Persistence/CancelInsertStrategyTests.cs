using System;
using System.Collections;
using Extensions.PersistenceCommon;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.Security;
using PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for CancelInsertStrategyTests.
    /// </summary>

    //TODO: Create XML summary comment for CancelInsertStrategyTests
    [TestFixture()]
    public class CancelInsertStrategyTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown CancelInsertStrategyTests
        [TestFixtureSetUp()]
        public static void SetUpCancelInsertStrategyTests()
        {
            User patientAccessUser = User.GetCurrent();

            if (patientAccessUser.SecurityUser == null)
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility selectedFacility = facilityBroker.FacilityWith(DEL_FACILITY_CODE);

                IUserBroker br = BrokerFactory.BrokerOfType<IUserBroker>();
                SecurityResponse securityResponse = br.AuthenticateUser(USER_NAME, PASSWORD, selectedFacility);

                User.SetCurrentUserTo(securityResponse.PatientAccessUser);
            }
        }

        [TestFixtureTearDown()]
        public static void TearDownCancelInsertStrategyTests()
        {
        }
        #endregion

        #region Test Methods

        [Test()]
        public void TestGenerateSQL()
        {
            Account anAccount = new Account();
            anAccount.AccountNumber = 23456789;
            anAccount.AdmitDate = DateTime.Parse("Dec 17, 2001");
            anAccount.ClergyVisit.SetYes("YES");
            anAccount.DischargeDate = DateTime.Parse("Jan 21, 2002");
            anAccount.DerivedVisitType = "OUTP";

            VisitType visitType = new VisitType(
                PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, VisitType.PREREG_PATIENT_DESC, VisitType.PREREG_PATIENT);
            anAccount.KindOfVisit = visitType;

            Patient patient = new Patient(
                PATIENT_OID,
                Patient.NEW_VERSION,
                this.PATIENT_NAME,
                PATIENT_MRN,
                this.PATIENT_DOB,
                this.PATIENT_SSN,
                this.PATIENT_SEX,
                this.FACILITY
                );

            anAccount.HospitalService = this.hsp;
            patient.PlaceOfWorship = this.placeOfWorship;
            patient.Race = this.race;
            patient.Ethnicity = this.ethnicity;
            patient.MaritalStatus = this.maritalStatus;
            patient.Language = this.language;
            patient.Religion = this.religion;

            anAccount.Patient = patient;
            anAccount.Facility = this.FACILITY;

            anAccount.FinancialClass =
                new FinancialClass(279,
                                    ReferenceValue.NEW_VERSION,
                                    "COMMERCIAL INS",
                                    "20");
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

            anAccount.Location = location;

            //            transactionKeys = new TransactionKeys( 12, 24, 36, 365 );

            Activity currentActivity =
                new CancelPreRegActivity();
            anAccount.Activity = currentActivity;
            this.cancelInsertStrategy = new CancelInsertStrategy();
            this.cancelInsertStrategy.TransactionFileId = "PF";
            this.cancelInsertStrategy.UserSecurityCode = "KEVN";
            ArrayList sqlStrings =
                this.cancelInsertStrategy.BuildSqlFrom(anAccount, this.transactionKeys);
            foreach (string sqlString in sqlStrings)
            {
                int startPositionOfValues =
                    sqlString.IndexOf("(", sqlString.IndexOf(")")) + 1;
                int lengthOfValues =
                    sqlString.Length - sqlString.IndexOf("(", sqlString.IndexOf(")")) - 2;
                string[] ValueArray =
                    sqlString.Substring(startPositionOfValues, lengthOfValues).Split(',');

                Assert.AreEqual(27, ValueArray.Length, "Wrong number of buckets in values Hashtable");

                Assert.AreEqual(" ''", ValueArray[0], "Value of APIDWS should be ''");
                Assert.AreEqual("'PF'", ValueArray[1], "Value of APIDID should be 'PF' ");
                Assert.AreEqual("'$#P@%&'", ValueArray[2], "Value of APRR# should be '$#P@%&' ");
                Assert.AreEqual("'KEVN'", ValueArray[3], "Value of APSEC2 should be 'KEVN' ");
                Assert.AreEqual("0", ValueArray[4], "Value of APHSP# should be 0 ");
                Assert.AreEqual("23456789", ValueArray[5], "Value of APACCT should be 23456789 ");
                Assert.AreEqual("0", ValueArray[6], "Value of APLML should be 0 ");
                Assert.AreEqual("0", ValueArray[7], "Value of APLMD should be 0 ");
                Assert.AreEqual("0", ValueArray[8], "Value of APLUL# should be 0 ");
                Assert.AreEqual("''", ValueArray[9], "Value of APACFL should be '' ");
                Assert.AreEqual("'$#L@%'", ValueArray[11], "Value of APINLG should be '$#L@%' ");
                Assert.AreEqual("''", ValueArray[12], "Value of APBYPS should be '' ");
                Assert.AreEqual("0", ValueArray[13], "Value of APSWPY should be 0 ");
                Assert.AreEqual("'0'", ValueArray[15], "Value of APPTYP should be '0' ");
                Assert.AreEqual("'99'", ValueArray[16], "Value of APMSV should be '99' ");
                Assert.AreEqual("''", ValueArray[17], "Value of APXMIT should be '' ");
                Assert.AreEqual("0", ValueArray[18], "Value of APQNUM should be 0 ");
                Assert.AreEqual("''", ValueArray[19], "Value of APZDTE should be '' ");
                Assert.AreEqual("''", ValueArray[20], "Value of APZTME should be '' ");
                Assert.AreEqual("'01'", ValueArray[21], "Value of APACC should be '01' ");
                Assert.AreEqual("'PACCESS'", ValueArray[22], "Value of APWSIR should be 'PACCESS' ");
                Assert.AreEqual("''", ValueArray[23], "Value of APSECR should be '' ");
                Assert.AreEqual("''", ValueArray[24], "Value of APORR1 should be '' ");
                Assert.AreEqual("''", ValueArray[25], "Value of APORR2 should be '' ");
                Assert.AreEqual("''", ValueArray[26], "Value of APORR3 should be ''");
            }
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private DateTime MinDate = new DateTime(0001, 1, 1, 0, 0, 0);
        private DateTime TestDateTime = new DateTime(1955, 3, 5, 23, 30, 59);

        private SocialSecurityNumber
            PATIENT_SSN = new SocialSecurityNumber("123121234");
        private Name
            PATIENT_NAME = new Name(PATIENT_F_NAME, PATIENT_L_NAME, PATIENT_MI);
        private DateTime
            PATIENT_DOB = new DateTime(1955, 3, 5);
        private Gender
            PATIENT_SEX = new Gender(0, DateTime.Now, "Female", "F");

        private Facility
            FACILITY =
                new Facility(PersistentModel.NEW_OID, PersistentModel.NEW_VERSION,
                             FACILILTY_NAME,
                             FACILITY_CODE,
                             FACILITY_MODTYPE);

        private PlaceOfWorship placeOfWorship = new PlaceOfWorship(
            PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "St. Mary's", "2");
        private Ethnicity ethnicity = new Ethnicity(
            PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "HISPANIC", "3");
        private Race race = new Race(
            PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "WHITE", "1");
        private Religion religion = new Religion(
            PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "JEWISH", string.Empty);
        private Language language = new Language(
            PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "YIDDISH", "1");
        private MaritalStatus maritalStatus = new MaritalStatus(
            PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "MARRIED", "1");


        private HospitalService hsp = new HospitalService(
            PersistentModel.NEW_OID, DateTime.Now, "HSVC 60", "99");

        private FinancialClass fin = new FinancialClass(
            PersistentModel.NEW_OID, DateTime.Now, "FIN CLASS 1", "1");

        private TransactionKeys transactionKeys = new TransactionKeys();

        private CancelInsertStrategy cancelInsertStrategy;

        #endregion

        #region Constants

        private const string
                            PATIENT_F_NAME = "CLAIRE",
                            PATIENT_L_NAME = "FRIED";

        private static readonly string PATIENT_MI = string.Empty;

        private const string
            FACILILTY_NAME = "DELRAY TEST HOSPITAL",
            DEL_FACILITY_CODE = "DEL",
            FACILITY_SERVERID = "123.234.345.456",
            FACILITY_DATABASENAME = "DVLA";

        private const int
            FACILITY_MODTYPE = 11;

        private const long
            PATIENT_OID = 45L,
            PATIENT_MRN = 24004;

        private const string ADD_FLAG = "A";
        private const long FACILITY_ID = 900;
        private const string HSPCODE = "ACO";
        private const string TEST_MRN = "785508";

        #endregion
    }
}