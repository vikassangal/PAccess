using System;
using Extensions.PersistenceCommon;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for CancelPreRegTransactionCoordinatorTests.
    /// </summary>

    //TODO: Create XML summary comment for CancelPreRegTransactionCoordinatorTests
    [TestFixture()]
    public class CancelPreRegTransactionCoordinatorTests
    {
        #region Constants

        const long FACILITY_ID = 900;
        private const string
            PATIENT_F_NAME = "TESTTRANSFER",
            PATIENT_L_NAME = "TEST";
            
        private static readonly string PATIENT_MI = string.Empty;
        
        private const long
            PATIENT_OID = 45L,
            PATIENT_MRN = 785673;
        private SocialSecurityNumber
            PATIENT_SSN = new SocialSecurityNumber( "124556789" );
        private Name
            PATIENT_NAME = new Name( PATIENT_F_NAME, PATIENT_L_NAME, PATIENT_MI );
        private DateTime
            PATIENT_DOB = new DateTime( 1965, 5, 5 );
        private Gender
            PATIENT_SEX = new Gender( 1, DateTime.Now, "Male", "M" );

        #endregion

        #region SetUp and TearDown CancelPreRegTransactionCoordinatorTests
        [TestFixtureSetUp()]
        public static void SetUpCancelPreRegTransactionCoordinatorTests()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            i_Facility = facilityBroker.FacilityWith( 900 );
        }

        [TestFixtureTearDown()]
        public static void TearDownCancelPreRegTransactionCoordinatorTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestTransactionCoordinatorFor()
        {
            Activity currentActivity = 
                new CancelPreRegActivity();
            TransactionCoordinator cancelTransactionCoordinator =
                TransactionCoordinator.TransactionCoordinatorFor( currentActivity );

            Assert.IsNotNull( cancelTransactionCoordinator );
            Assert.AreEqual( typeof( CancelPreRegTransactionCoordinator ), 
                             cancelTransactionCoordinator.GetType() );

            TransactionCoordinator nullTransactionCoordinator = 
                TransactionCoordinator.TransactionCoordinatorFor( null );
            Assert.IsNull( nullTransactionCoordinator );
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
                PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "St. Mary's", "2" );
            Ethnicity ethnicity = new Ethnicity(
                PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "HISPANIC", "3" );
            Race race = new Race(
                PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "WHITE", "1" );
            Religion religion = new Religion(
                PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "JEWISH", string.Empty );
            Language language = new Language(
                PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "YIDDISH", "1" );
            MaritalStatus maritalStatus = new MaritalStatus(
                PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "MARRIED", "1" );
            patient.PlaceOfWorship = placeOfWorship;
            patient.Race = race;
            patient.Ethnicity = ethnicity;
            patient.MaritalStatus = maritalStatus;
            patient.Language = language;

            AccountProxy proxy = new AccountProxy( 51581,
                                                   patient,
                                                   DateTime.Now,
                                                   DateTime.Now,
                                                   new VisitType( 1, ReferenceValue.NEW_VERSION, "PREREG_PATIENT", "0" ),
                                                   i_Facility,
                                                   new FinancialClass( 20, ReferenceValue.NEW_VERSION, "COMMERCIAL INS", "20" ),
                                                   new HospitalService( ReferenceValue.NEW_OID, ReferenceValue.NEW_VERSION, "CORRECTIONAL INPUT", "37" ),
                                                   "PREREG_PATIENT",
                                                   false );

            IAccountBroker accountBroker =
                BrokerFactory.BrokerOfType<IAccountBroker>();
            Account account = accountBroker.AccountFor( proxy );
            
            account.Activity = new CancelPreRegActivity();

            CancelPreRegTransactionCoordinator cancelPreRegCoordinator =
                new CancelPreRegTransactionCoordinator();
            cancelPreRegCoordinator.NumberOfInsurances = 0;
            cancelPreRegCoordinator.NumberOfNonInsurances = 0;
            cancelPreRegCoordinator.NumberOfOtherRecs = 0;
            cancelPreRegCoordinator.Account = account;
            cancelPreRegCoordinator.AppUser = account.Activity.AppUser;

            SqlBuilderStrategy[] strategy = cancelPreRegCoordinator.InsertStrategies;

        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static  Facility i_Facility;
        #endregion
    }
}