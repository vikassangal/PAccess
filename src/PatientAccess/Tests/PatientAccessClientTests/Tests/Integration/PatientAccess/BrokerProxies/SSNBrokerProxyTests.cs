using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.BrokerProxies
{
    /// <summary>
    /// Summary description for SSNBrokerProxyTests.
    /// </summary>

    //TODO: Create XML summary comment for SSNBrokerProxyTests
    [TestFixture()]
    public class SSNBrokerProxyTests
    {
        #region Constants
        private const string NEWBORN_DESCRIPTION    = "NEWBORN";
        private const string TEST_RESULT_KNOWN      = "Known";
        #endregion

        #region SetUp and TearDown SSNBrokerProxyTests
        [TestFixtureSetUp()]
        public static void SetUpSSNBrokerProxyTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownSSNBrokerProxyTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestSSNStatuses()
        {
            State st = new State(178, "FLORIDA", "FL");
            ArrayList statuses = this.i_ssnBrokerProxy.SSNStatuses(900, st.Code);

            Assert.IsNotNull(statuses, "No SSN Statuses list found");
            Assert.IsTrue(statuses.Count > 0, "No SSN Statuses Found");

            SocialSecurityNumberStatus ssnStat = statuses[ 0 ] as SocialSecurityNumberStatus ;
            if ( null != ssnStat )
            {
                Assert.AreEqual( ssnStat.Description, TEST_RESULT_KNOWN ) ;
            }
        }

        [Test()]
        public void TestSSNStatusesForGuarantor()
        {
            State st = new State( 178, "FLORIDA", "FL" );

            ICollection statuses = this.i_ssnBrokerProxy.SSNStatusesForGuarantor( 900, st.Code);

            Assert.IsNotNull( statuses, "No SSN Statuses list found" );
            Assert.IsTrue( statuses.Count > 0, "No SSN Statuses Found" );

            foreach( SocialSecurityNumberStatus ssnStatus in statuses )
            {
                if( ssnStatus.Description.Trim().ToUpper() == NEWBORN_DESCRIPTION )
                {
                    Assert.Fail( "Fetching NewBorn SSN Status for Guarantor" );
                }
            }
        }
      
        
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private readonly SSNBrokerProxy i_ssnBrokerProxy = new SSNBrokerProxy();
        #endregion
    }
}