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
    public class TransferOutPatientToInPatientTransactionCoordinatorTests
    {
        #region Constants

        const long TEST_MRN = 785673;
        const long FACILITY_ID = 900;

        private const string PATIENT_F_NAME = "CLAIRE", PATIENT_L_NAME = "FRIED";

        private static readonly string PATIENT_MI = string.Empty;
        
        private const long
            PATIENT_OID     = 45L;
        private SocialSecurityNumber
            PATIENT_SSN     = new SocialSecurityNumber( "123121234" );
        private Name
            PATIENT_NAME    = new Name( PATIENT_F_NAME, PATIENT_L_NAME, PATIENT_MI );
        private DateTime
            PATIENT_DOB     = new DateTime( 1955, 3, 5 );
        private Gender
            PATIENT_SEX     = new Gender( 0, DateTime.Now, "Female", "F" );


        #endregion

        #region SetUp and TearDown TransferOutPatientToInPatientTransactionCoordinatorTests
        [TestFixtureSetUp()]
        public static void SetUpTransferOutPatientToInPatientTransactionCoordinatorTests()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            i_Facility = facilityBroker.FacilityWith( FACILITY_ID );
        }

        [TestFixtureTearDown()]
        public static void TearDownTransferOutPatientToInPatientTransactionCoordinatorTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestTransactionCoordinatorFor()
        {
            Activity currentActivity = 
                new TransferOutToInActivity();
            TransactionCoordinator coordinator =
                TransactionCoordinator.TransactionCoordinatorFor( currentActivity );
            Assert.IsNotNull( coordinator );
            Assert.AreEqual( typeof( TransferOutPatientToInPatientTransactionCoordinator ), 
                             coordinator.GetType() );
        }


        [Test()]
        public void TestGetInsertStrategy()
        {

            Patient patient = new Patient(
                    PATIENT_OID,
                    Patient.NEW_VERSION,
                    this.PATIENT_NAME,
                    TEST_MRN,
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
                                                       new VisitType( 1, ReferenceValue.NEW_VERSION, VisitType.INPATIENT_DESC, VisitType.INPATIENT ),
                                                       i_Facility,
                                                       new FinancialClass( 20, ReferenceValue.NEW_VERSION, "COMMERCIAL INS", "20" ),
                                                       new HospitalService( ReferenceValue.NEW_OID, ReferenceValue.NEW_VERSION, "CORRECTIONAL INPUT", "37" ),
                                                       VisitType.INPATIENT_DESC,
                                                       false );

                IAccountBroker accountBroker =
                    BrokerFactory.BrokerOfType<IAccountBroker>();
                Account account = accountBroker.AccountFor( proxy );
                account.DischargeDate = DateTime.Now;
                //Location Information  
                Location location = new Location();
                NursingStation nursingStation = new NursingStation( PersistentModel.NEW_OID, DateTime.Now, "6N", "6N" );
                location.NursingStation = nursingStation;
                Room room = new Room( PersistentModel.NEW_OID, DateTime.Now, "610", "610" );
                location.Room = room;
                Bed bed = new Bed( PersistentModel.NEW_OID, DateTime.Now, "A", "A" );
                Accomodation accomodation = new Accomodation( PersistentModel.NEW_OID, DateTime.Now, "PRIVATE", "01" );
                location.Bed = bed;
                location.Bed.Accomodation = accomodation;

                account.Location = location;
                account.Activity = new TransferOutToInActivity();

                TransferOutPatientToInPatientTransactionCoordinator transferOutToIn =
                    new TransferOutPatientToInPatientTransactionCoordinator();
                transferOutToIn.NumberOfInsurances = 0;
                transferOutToIn.NumberOfNonInsurances = 0;
                transferOutToIn.NumberOfOtherRecs = 0;
                transferOutToIn.Account = account;
                transferOutToIn.AppUser = account.Activity.AppUser;

                SqlBuilderStrategy[] strategy = transferOutToIn.InsertStrategies;

        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        private static  Facility i_Facility;

        #endregion
    }
}