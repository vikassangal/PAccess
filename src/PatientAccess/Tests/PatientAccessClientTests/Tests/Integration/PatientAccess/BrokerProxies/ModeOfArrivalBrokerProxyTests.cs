using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.BrokerProxies
{
    /// <summary>
    /// Summary description for ModeOfArrivalBrokerProxyTests.
    /// </summary>

    //TODO: Create XML summary comment for ModeOfArrivalBrokerProxyTests
    [TestFixture()]
    public class ModeOfArrivalBrokerProxyTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown ModeOfArrivalBrokerProxyTests
        [TestFixtureSetUp()]
        public static void SetUpModeOfArrivalBrokerProxyTests()
        {
            i_Facility = new Facility(900,ReferenceValue.NEW_VERSION,"SRE","SRE");
        }

        [TestFixtureTearDown()]
        public static void TearDownModeOfArrivalBrokerProxyTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestModesOfArrival()
        {
            ArrayList modes = i_modesOfArrivalBrokerProxy.ModesOfArrivalFor( i_Facility.Oid );

            Assert.IsNotNull(  modes, "No modes list found" );
            Assert.IsTrue( modes.Count > 0, "No modes found in list" );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static readonly IModeOfArrivalBroker i_modesOfArrivalBrokerProxy = new ModeOfArrivalBrokerProxy();
        private static Facility i_Facility = null;
        #endregion
    }
}