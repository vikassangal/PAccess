using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.BrokerProxies
{
    [TestFixture()]
    public class ScheduleCodeBrokerProxyTest
    {
        #region Constants
        #endregion

        #region SetUp and TearDown ScheduleCodeBrokerTest
        [TestFixtureSetUp()]
        public static void SetUpScheduleCodeBrokerTest()
        {
            i_BrokerProxy = new ScheduleCodeBrokerProxy( );    
        }

        [TestFixtureTearDown()]
        public static void TearDownScheduleCodeBrokerTest()
        {
     
        }
        #endregion

        #region Test Methods
    
        [Test()]
        public void TestAllScheduleCodes()
        {
            ArrayList scheduleCodes = (ArrayList)i_BrokerProxy.AllScheduleCodes( this.ACO_FACILITYID );
   
            Assert.IsTrue( scheduleCodes.Count > 0, "No schedule code found" );
            Assert.AreEqual(
                "ADD-ON LESS THAN 24 HOURS",
                ((ScheduleCode)scheduleCodes[1]).Description,
                "2nd entry should be SCHEDULED 24 HOURS BEFORE" );
        }
        #endregion
    
        #region Support Methods
        #endregion
        
        #region Data Elements
        private static ScheduleCodeBrokerProxy i_BrokerProxy = null;
        #endregion

        #region Constants
        private long ACO_FACILITYID = 900;
        #endregion
    }
}