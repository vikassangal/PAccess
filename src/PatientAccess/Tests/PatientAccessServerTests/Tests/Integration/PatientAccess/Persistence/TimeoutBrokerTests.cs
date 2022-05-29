using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for FacilityBrokerTests.
    /// </summary>
    [TestFixture()]
    public class TimeoutBrokerTests : AbstractBrokerTests
    {

        #region SetUp and TearDown TimeBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpTimeBrokerTests()
        {
            timeoutBroker = BrokerFactory.BrokerOfType<ITimeoutBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownTimeBrokerTests()
        {
            timeoutBroker = null;
        }

        #endregion

        #region Test Methods
        [Test()]
        public void TestTimeoutFor()
        {
            ActivityTimeout activityTimeout = timeoutBroker.TimeoutFor();
            Assert.IsNotNull(activityTimeout, "fail to get time out config");
            Assert.AreEqual(activityTimeout.FirstAlertTime, 1620000, "activity time out first alert should be 1620000 milliseconds" );
            Assert.AreEqual(activityTimeout.SecondAlertTime, 300000, "activity time out second alert should be 300000 milliseconds");
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static  ITimeoutBroker timeoutBroker = null;
        #endregion

        #region Constants
        #endregion
    }
}