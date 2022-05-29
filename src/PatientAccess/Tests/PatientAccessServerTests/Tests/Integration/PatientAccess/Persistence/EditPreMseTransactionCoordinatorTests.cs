using System;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for EditPreMseTransactionCoordinatorTests.
    /// </summary>

    //TODO: Create XML summary comment for EditPreMseTransactionCoordinatorTests
    [TestFixture()]
    public class EditPreMseTransactionCoordinatorTests
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

        #region SetUp and TearDown EditPreMseTransactionCoordinatorTests
        [TestFixtureSetUp()]
        public static void SetUpEditPreMseTransactionCoordinatorTests()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            i_Facility = facilityBroker.FacilityWith( FACILITY_ID );
        }

        [TestFixtureTearDown()]
        public static void TearDownEditPreMseTransactionCoordinatorTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestTransactionCoordinatorFor()
        {
            Activity currentActivity = 
                new EditPreMseActivity();
            TransactionCoordinator EditPreMseTransactionCoordinator =
                TransactionCoordinator.TransactionCoordinatorFor( currentActivity );
            Assert.IsNotNull( EditPreMseTransactionCoordinator );
            Assert.AreEqual( typeof( EditPreMseTransactionCoordinator ), 
                             EditPreMseTransactionCoordinator.GetType() );
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
                                                       new VisitType( 1, ReferenceValue.NEW_VERSION, "EMERGENCY_PATIENT", "3" ),
                                                       i_Facility,
                                                       new FinancialClass( 20, ReferenceValue.NEW_VERSION, "COMMERCIAL INS", "20" ),
                                                       new HospitalService( ReferenceValue.NEW_OID, ReferenceValue.NEW_VERSION, "CORRECTIONAL INPUT", "37" ),
                                                       "EMERGENCY_PATIENT",
                                                       false );

                IAccountBroker accountBroker =
                    BrokerFactory.BrokerOfType<IAccountBroker>();
                Account account = accountBroker.AccountFor( proxy );
                
                account.Activity = new EditPreMseActivity();

                EditPreMseTransactionCoordinator EditPreMSECoordinator =
                    new EditPreMseTransactionCoordinator();
                EditPreMSECoordinator.NumberOfInsurances = 0;
                EditPreMSECoordinator.NumberOfNonInsurances = 0;
                EditPreMSECoordinator.NumberOfOtherRecs = 0;
                EditPreMSECoordinator.Account = account;
                EditPreMSECoordinator.AppUser = account.Activity.AppUser;

                SqlBuilderStrategy[] strategy = EditPreMSECoordinator.InsertStrategies;


        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        private static  Facility i_Facility;

        #endregion
    }
}