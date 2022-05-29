using System.Collections;
using PatientAccess.BrokerInterfaces;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for FacilityBrokerTests.
    /// </summary>
    [TestFixture()]
    public class ClientConfigurationBrokerTests : AbstractBrokerTests
    {
        #region SetUp and TearDown ClientConfigurationBrokerTests
        
        [TestFixtureSetUp()]
        public static void SetUpClientConfigurationBrokerTests()
        {
            clientConfigurationBroker = BrokerFactory.BrokerOfType<IClientConfigurationBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownClientConfigurationBrokerTests()
        {

        }

        #endregion

        #region Test Methods

        [Test()]
        public void TestConfigurationValues()
        {
            Hashtable configValues = clientConfigurationBroker.ConfigurationValues();
            Assert.IsTrue(configValues.Count > 0, "fail to get client config");
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        private static  IClientConfigurationBroker clientConfigurationBroker = null;

        #endregion

        #region Constants
        #endregion
    }
}