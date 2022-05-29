using System;
using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.BrokerProxies
{
    [TestFixture()]
    public class ReferralSourceBrokerProxyTests
    {
        #region Constants

        private const string c_14 = "14";
        private const string c_14_Desc = "HEALTH LINK COMMERCIAL";

        #endregion

        #region SetUp and TearDown ReferralSourceBrokerProxyTests
        [TestFixtureSetUp()]
        public static void SetUpReferralSourceBrokerProxyTests()
        {
            i_BrokerProxy = new ReferralSourceBrokerProxy();
        }

        [TestFixtureTearDown()]
        public static void TearDownReferralSourceBrokerProxyTests()
        {
        }
        #endregion

        #region Test Methods
    
        [Test()]
        public void TestAllReferralSourcesForACO()
        {
            ArrayList referralSources = (ArrayList)i_BrokerProxy.AllReferralSources( ACO_FACILITYID );
   
            Assert.IsTrue( referralSources.Count > 0, "No referral source found" );
            Assert.AreEqual( "A1", ( (ReferralSource)referralSources[1] ).Code,
                             "First entry should be A1" );
            ReferralSource rs = (ReferralSource)referralSources[1];
            Assert.IsTrue( rs.IsValid, "Should be valid code" );
        }
    
        [Test()]
        public void TestInvalidReferralSource()
        {
            ReferralSource referralSource = i_BrokerProxy.ReferralSourceWith( ACO_FACILITYID, "00" );
            Assert.IsFalse( referralSource.IsValid, "Should be invalid code" );
        }

        [Test()]
        public void TestReferralSourceWith()
        {
            ReferralSource referralSource = i_BrokerProxy.ReferralSourceWith( ACO_FACILITYID, c_14 );

            Assert.IsTrue( referralSource.IsValid, "Should be valid code" );
            Assert.AreEqual( c_14_Desc, referralSource.Description );
        }

        [Test()]
        public void TestReferralSourceCodeForBlank()
        {
            string blank = String.Empty;
            ReferralSource referralSource = i_BrokerProxy.ReferralSourceWith( ACO_FACILITYID, blank );

            Assert.AreEqual( blank, referralSource.Code, "Code should be blank" );
            Assert.AreEqual( blank, referralSource.Description, "Description should be blank" );
            Assert.IsTrue( referralSource.IsValid, "Should be valid code" );
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void TestReferralSourceCodeForNull()
        {
            ReferralSource referralSource = i_BrokerProxy.ReferralSourceWith( ACO_FACILITYID, null );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        private static ReferralSourceBrokerProxy i_BrokerProxy = null;

        #endregion

        #region Constants
        const long ACO_FACILITYID = 900;
        const long DHF_FACILITYID = 54;

        #endregion
    }
}