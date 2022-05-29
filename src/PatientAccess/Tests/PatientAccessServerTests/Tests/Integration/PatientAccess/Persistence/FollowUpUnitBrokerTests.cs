using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for FollowUpUnitBrokerTests.
    /// </summary>
   
    [TestFixture()]
    public class FollowUpUnitBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown FollowUpUnitBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpFollowUpUnitBrokerTests()
        {
            fuuBroker = BrokerFactory.BrokerOfType<IFollowUpUnitBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownFollowUpUnitBrokerTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestFollowUpUnits()
        {
            IList followUpUnits = fuuBroker.AllFollowUpUnits();
            Assert.IsNotNull( followUpUnits, "Did not find any FollowUpUnits" );

            FollowupUnit fuu = fuuBroker.FollowUpUnitWith(999);
            Assert.IsNotNull( fuu, "Did not find FollowUpUnit with a key of 999" );
        }
        [Test()]
        public void TestLoadingFUUs()
        {
            bool loading000 = fuuBroker.FollowupUnitIsBeingLoaded("000");
            Assert.IsTrue(loading000, "FUU 000 should be loading");

            bool loadingXX = fuuBroker.FollowupUnitIsBeingLoaded("XX");
            Assert.IsFalse(loadingXX, "Followup Unit XX should not be reflected as loading");
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static  IFollowUpUnitBroker fuuBroker = null;
        #endregion

    }
}