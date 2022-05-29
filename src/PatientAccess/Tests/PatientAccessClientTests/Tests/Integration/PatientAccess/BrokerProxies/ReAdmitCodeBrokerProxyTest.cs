using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerProxies;

namespace Tests.Integration.PatientAccess.BrokerProxies
{
    /// <summary>
    /// Summary description for ReAdmitCodeBrokerProxyTest.
    /// </summary>


    [TestFixture()]
    public class ReAdmitCodeBrokerProxyTest
    {
        #region Constants
        #endregion

        #region SetUp and TearDown ReAdmitCodeBrokerProxyTest
        [TestFixtureSetUp()]
        public static void SetUpReAdmitCodeBrokerProxyTest()
        {
            i_Broker = new ReAdmitCodeBrokerProxy();
        }

        [TestFixtureTearDown()]
        public static void TearDownReAdmitCodeBrokerProxyTest()
        {
        }
        #endregion

        #region Test Methods
    
        [Test()]
        public void TestReAdmitCodesFor()
        {            
            ArrayList reAdmitCodes = (ArrayList)i_Broker.ReAdmitCodesFor( FACILITY_NUM );
   
            Assert.IsTrue( reAdmitCodes.Count > 0, "No code found" );
            //            Assert.AreEqual(
            //                "WITHIN 24 HRS",
            //                ((ReAdmitCode)reAdmitCodes[1]).Description,
            //                "WITHIN 24 HRS" );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static ReAdmitCodeBrokerProxy i_Broker = null;
        #endregion

        #region Constants
        private const int
            FACILITY_NUM        = 900;
        #endregion
    }
}