using PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture]
    [Category( "Fast" )]
    public class CacheKeysTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown CacheKeysTests
        [TestFixtureSetUp()]
        public static void SetUpCacheKeysTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownCacheKeysTests()
        {
        }

        #endregion

        #region Test Methods
        [Test()]
        public void TestCacheKeyForHspCode()
        {
            Assert.AreEqual( "CACHE_KEY_FOR_RELIGIOUSPLACESOFWORSHIP_ACO", CacheKeys.KeyFor( CacheKeys.CACHE_KEY_FOR_RELIGIOUSPLACESOFWORSHIP, "ACO" ) );
        }

        [Test()]
        public void TestComplexKeyFor()
        {
            Assert.AreEqual( "CACHE_KEY_FOR_HSVS_32",CacheKeys.KeyFor(CacheKeys.CACHE_KEY_FOR_HSVS, "32"));
        }

        [Test()]
        public void TestCacheExpiratinInterval()
        {
            Assert.AreEqual("CACHE_KEY_FOR_HSVS_INTERVAL", CacheKeys.CacheExpirationIntervalKeyFor(CacheKeys.CACHE_KEY_FOR_HSVS));
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion

    }
}