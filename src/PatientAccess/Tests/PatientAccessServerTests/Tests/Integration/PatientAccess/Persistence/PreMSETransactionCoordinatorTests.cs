using System;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for PreMSETransactionCoordinatorTests.
    /// </summary>

    //TODO: Create XML summary comment for PreMSETransactionCoordinatorTests
    [TestFixture()]
    public class PreMSETransactionCoordinatorTests
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

        #region SetUp and TearDown MaintenanceTransactionCoordinatorTests
        [TestFixtureSetUp()]
        public static void SetUpMaintenanceTransactionCoordinatorTests()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            i_Facility = facilityBroker.FacilityWith( 900 );
        }

        [TestFixtureTearDown()]
        public static void TearDownMaintenanceTransactionCoordinatorTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestTransactionCoordinatorFor()
        {
            Activity currentActivity =
                new PreMSERegisterActivity();
            TransactionCoordinator preMSETransactionCoordinator =
                TransactionCoordinator.TransactionCoordinatorFor( currentActivity );
            Assert.IsNotNull( preMSETransactionCoordinator );
            Assert.AreEqual( typeof( PreMSETransactionCoordinator ),
                             preMSETransactionCoordinator.GetType() );
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
                
                account.Activity = new PreMSERegisterActivity();

                PreMSETransactionCoordinator preMSECoordinator =
                    new PreMSETransactionCoordinator();
                preMSECoordinator.NumberOfInsurances = 0;
                preMSECoordinator.NumberOfNonInsurances = 0;
                preMSECoordinator.NumberOfOtherRecs = 0;
                preMSECoordinator.Account = account;
                preMSECoordinator.AppUser = account.Activity.AppUser;

                SqlBuilderStrategy[] strategy = preMSECoordinator.InsertStrategies;

        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        private static  Facility i_Facility;
        #endregion
    }
}