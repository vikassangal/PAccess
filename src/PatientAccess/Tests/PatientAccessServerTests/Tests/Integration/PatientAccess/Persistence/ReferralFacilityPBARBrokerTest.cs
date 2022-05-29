using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class ReferralFacilityPBARBrokerTest : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown ReferralFacilityBrokerTest
        [TestFixtureSetUp()]
        public static void SetUpReferralFacilityBrokerTest()
        {
            i_ReferralFacilityBroker = BrokerFactory.BrokerOfType<IReferralFacilityBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownReferralFacilityBrokerTest()
        {
        }
        #endregion

        #region Test Methods

        [Test()]
        public void TestReferralFacilitiesFor()
        {
            ArrayList referralFacilities = (ArrayList)i_ReferralFacilityBroker.ReferralFacilitiesFor( ACO_FACILITYID );

            Assert.IsTrue( referralFacilities.Count > 0, "No code found" );

        }

        [Test()]
        public void TestReferralFacilityWith()
        {
            ReferralFacility obj = (ReferralFacility)i_ReferralFacilityBroker.ReferralFacilityWith( ACO_FACILITYID, "FM" );

            Assert.AreEqual( "FAMILY MEDICAL", obj.Description, "Description is incorrect." );
        }
        [Test()]
        public void TestReferralFacilityWithBlank()
        {
            string blank = string.Empty;
            ReferralFacility obj = (ReferralFacility)i_ReferralFacilityBroker.ReferralFacilityWith( ACO_FACILITYID, blank );

            Assert.AreEqual( blank, obj.Description, "Description should be blank" );
            Assert.IsTrue( obj.IsValid, "ReferralFacility should be valid" );
        }
        [Test()]
        public void TestInValidReferralFacility()
        {
            ReferralFacility obj = (ReferralFacility)i_ReferralFacilityBroker.ReferralFacilityWith( ACO_FACILITYID, "DZ" );
            Assert.IsFalse( obj.IsValid, "ReferralFacility should be invalid" );
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        
        private static IReferralFacilityBroker i_ReferralFacilityBroker = null;
        
        #endregion

    }
}