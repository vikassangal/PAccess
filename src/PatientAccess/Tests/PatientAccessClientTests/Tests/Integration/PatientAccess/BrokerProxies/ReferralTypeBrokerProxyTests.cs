using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerProxies;

namespace Tests.Integration.PatientAccess.BrokerProxies
{
    /// <summary>
    /// Summary description for ReferralTypeBrokerProxyTests.
    /// </summary>

    //TODO: Create XML summary comment for ReferralTypeBrokerProxyTests
    [TestFixture()]
    public class ReferralTypeBrokerProxyTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown ReferralTypeBrokerProxyTests
        [TestFixtureSetUp()]
        public static void SetUpReferralTypeBrokerProxyTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownReferralTypeBrokerProxyTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestReferralTypesFor()
        {
            ArrayList referralTypes = (ArrayList)this.i_BrokerProxy.ReferralTypesFor( FACILITY_NUM );
   
            Assert.IsTrue( referralTypes.Count > 0, "No code found" ); 
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private readonly ReferralTypeBrokerProxy i_BrokerProxy = new ReferralTypeBrokerProxy();
        private const int FACILITY_NUM        = 900;
        #endregion
    }
}