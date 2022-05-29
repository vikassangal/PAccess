using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class ReferralSourcePBARBrokerTest : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown ReferralSourceBrokerTest
        [TestFixtureSetUp()]
        public static void SetUpReferralSourceBrokerTest()
        {
            i_ReferralSourceBroker = BrokerFactory.BrokerOfType<IReferralSourceBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownReferralSourceBrokerTest()
        {
        }
        #endregion

        #region Test Methods

        [Test()]
        public void TestAllReferralSourcesForACO()
        {
            ArrayList referralSources = (ArrayList)i_ReferralSourceBroker.AllReferralSources( ACO_FACILITYID );

            Assert.IsTrue( referralSources.Count > 0, "No referral source found" );
            Assert.AreEqual("A1", ((ReferralSource)referralSources[1]).Code,
                             "First entry should be A1.");
            ReferralSource rs = (ReferralSource)referralSources[1];
            Assert.IsTrue( rs.IsValid, "Should be valid code" );
        }
        [Test()]
        public void TestAllReferralSourcesForDHF()
        {
            ArrayList referralSources = (ArrayList)i_ReferralSourceBroker.AllReferralSources(DHF_FACILITYID);

            Assert.IsTrue(referralSources.Count > 0, "No referral source found");
            Assert.AreEqual("D1", ((ReferralSource)referralSources[1]).Code,
                             "First entry should be D1.");
            ReferralSource rs = (ReferralSource)referralSources[1];
            Assert.IsTrue(rs.IsValid, "Should be valid code");
        }
        [Test()]
        public void TestInvalidReferralSource()
        {
            ReferralSource referralSource = i_ReferralSourceBroker.ReferralSourceWith( ACO_FACILITYID, "00" );
            Assert.IsFalse( referralSource.IsValid, "Should be invalid code" );
        }

        [Test()]
        public void TestReferralSourceCodeForBlank()
        {
            string blank = String.Empty;
            ReferralSource referralSource = i_ReferralSourceBroker.ReferralSourceWith( ACO_FACILITYID, blank );

            Assert.AreEqual( blank, referralSource.Code, "Code should be blank" );
            Assert.AreEqual( blank, referralSource.Description, "Description should be blank" );
            Assert.IsTrue( referralSource.IsValid, "Should be valid code");
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static IReferralSourceBroker i_ReferralSourceBroker = null;
        #endregion

        #region Constants
        #endregion
    }
}