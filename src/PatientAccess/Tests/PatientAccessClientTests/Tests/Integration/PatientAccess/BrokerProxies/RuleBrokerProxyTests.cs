using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.BrokerProxies
{
    [TestFixture()]
    public class RuleBrokerProxyTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown RuleBrokerProxyTests

        [TestFixtureSetUp()]
        public static void SetUpRuleBrokerProxyTests()
        {
            i_BrokerProxy = new RuleBrokerProxy();
        }

        [TestFixtureTearDown()]
        public static void TearDownRuleBrokerProxyTests()
        {
        }

        #endregion

        #region Test Methods
    
        [Test()]
        public void TestRuleWorklistMapping()
        {
            Hashtable t = i_BrokerProxy.RuleWorklistMapping();

            Assert.IsTrue( t.Count > 0, "No Rule to Worklists Found" );
        }

        [Test()]
        public void TestRuleActionMapping()
        {
            Hashtable t = i_BrokerProxy.RuleActionMapping();

            Assert.IsTrue( t.Count > 0, "No Rule to Actions Found" );
        }

        [Test()]
        public void TestActionWorklistMapping()
        {
            bool found = false;
            Hashtable mappings = new Hashtable();
            string kindOfVisitCode = VisitType.PREREG_PATIENT;
            string financialClassCode = "02";

            mappings = i_BrokerProxy.ActionWorklistMapping( kindOfVisitCode, financialClassCode );
            Assert.IsNotNull( mappings, "We got null collection from ActionWorklistMapping()" );
            Assert.IsTrue( mappings.Count > 0, "We got zero elements collection from ActionWorklistMapping()" );

            ArrayList a;
            string workListName = string.Empty;
            for( int i = 1; i <= mappings.Count; i++ )
            {
                a = (ArrayList)mappings[(long)i];
                if( a != null )
                {
                    workListName = (string)a[1];
                    if( workListName.Equals( "Pre-MSE" ) )
                    {
                        found = true;
                    }
                }
            }
            Assert.IsTrue( found, "Haven't found workList with the name 'Pre-MSE'" );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        private static RuleBrokerProxy i_BrokerProxy = null;

        #endregion

        #region Constants
        #endregion
    }
}