using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Persistence;
using NUnit.Framework;
using PatientAccess.Domain.UCCRegistration;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for UrgentCareTransactionCoordinatorTests.
    /// </summary>

    //TODO: Create XML summary comment for UrgentCareTransactionCoordinatorTests
    [TestFixture()]
    public class UrgentCareTransactionCoordinatorTests
    {
        #region Constants

        private const string
            PATIENT_F_NAME = "TESTTRANSFER",
            PATIENT_L_NAME = "TEST";

        private static readonly string PATIENT_MI = string.Empty;

        #endregion

        #region SetUp and TearDown MaintenanceTransactionCoordinatorTests
        [TestFixtureSetUp()]
        public static void SetUpMaintenanceTransactionCoordinatorTests()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            i_Facility = facilityBroker.FacilityWith( 900 );
        }
 
        #endregion

        #region Test Methods
        [Test()]
        public void TestUrgentCarePreMseTransactionCoordinatorFor()
        {
            Activity currentActivity =
                new UCCPreMSERegistrationActivity();
            TransactionCoordinator i_UCPreMseTransactionCoordinator =
                TransactionCoordinator.TransactionCoordinatorFor( currentActivity );
            Assert.IsNotNull(i_UCPreMseTransactionCoordinator);
            Assert.AreEqual( typeof( PreMSETransactionCoordinator ),
                             i_UCPreMseTransactionCoordinator.GetType());
        }
        [Test()]
        public void TestUrgentCarePostMseTransactionCoordinatorFor()
        {
            Activity currentActivity =
                new UCCPostMseRegistrationActivity();
            TransactionCoordinator i_UCPostMseTransactionCoordinator =
                TransactionCoordinator.TransactionCoordinatorFor(currentActivity);
            Assert.IsNotNull(i_UCPostMseTransactionCoordinator);
            Assert.AreEqual(typeof(PostMSETransactionCoordinator),
                             i_UCPostMseTransactionCoordinator.GetType());
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        private static  Facility i_Facility;
        #endregion
    }
}