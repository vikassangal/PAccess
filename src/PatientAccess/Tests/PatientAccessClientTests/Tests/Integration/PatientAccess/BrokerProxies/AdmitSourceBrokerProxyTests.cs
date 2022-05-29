using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.BrokerProxies
{
    [TestFixture()]
    public class AdmitSourceBrokerProxyTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown AdmitSourceBrokerProxyTests
        [TestFixtureSetUp()]
        public static void SetUpAdmitSourceBrokerProxyTests()
        {
            i_BrokerProxy = new AdmitSourceBrokerProxy();
        }

        [TestFixtureTearDown()]
        public static void TearDownAdmitSourceBrokerProxyTests()
        {
        }
        #endregion

        #region Test Methods

        [Test()]
        public void TestAllTypesOfAdmitSources()
        {
            ArrayList admitSources = (ArrayList)i_BrokerProxy.AllTypesOfAdmitSources( 900 );

            AdmitSource selectedSource = null;

            foreach (AdmitSource admitSource in admitSources)
            {
                if (admitSource.Code == "A")
                    selectedSource = admitSource;
            }

            Assert.IsNotNull( selectedSource, "Did not find Physician AdmitSource" );
        }

        [Test()]
        public void TestAdmitSourcesForNotNewBorn()
        {
            ArrayList admitSources = (ArrayList)i_BrokerProxy.AdmitSourcesForNotNewBorn( 900 );

            foreach (AdmitSource admitSource in admitSources)
            {
                Assert.IsTrue( admitSource.Code != "L", "Code should not be L" );
            }
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        private static AdmitSourceBrokerProxy i_BrokerProxy = null;

        #endregion

        #region Constants
        #endregion
    }
}