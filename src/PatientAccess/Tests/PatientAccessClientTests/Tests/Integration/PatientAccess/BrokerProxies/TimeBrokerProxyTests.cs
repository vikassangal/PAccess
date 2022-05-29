using System;
using NUnit.Framework;
using PatientAccess;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Http;
using Rhino.Mocks;

namespace Tests.Integration.PatientAccess.BrokerProxies
{
    [TestFixture]
    [Category( "Fast" )]
    public class TimeBrokerProxyTests
    {
        private const int toleranceAllowedInSeconds = 10;

        [Test]
        public void TestTimeAtFirstCall_ShouldCallTheUnderlyingBroker()
        {

            const int dstOffset = 6;
            const int offset = 1;
            ICache cache = new HttpRuntimeCache();

            cache.Remove(
                string.Format(TimeBrokerProxy.CACHE_KEY_FORMAT_STRING, offset, dstOffset));

            ITimeBroker mockTimeBroker = MockRepository.GenerateMock<ITimeBroker>();

            DateTime expectedServerTime = DateTime.Now;
            mockTimeBroker.Expect( ( x => x.TimeAt( offset, dstOffset ) ) ).Return( expectedServerTime );

            var timeBrokerProxy = new TimeBrokerProxy( mockTimeBroker, cache );

            DateTime actualServerTime = timeBrokerProxy.TimeAt( offset, dstOffset );

            AssertAreSameUptillTheSecond( expectedServerTime, actualServerTime, toleranceAllowedInSeconds );
            mockTimeBroker.VerifyAllExpectations();
        }


        [Test]
        public void TestTimeAtSecondCall_ForSecondCallShouldNotCallTheUnderlyingBrokerAndShouldUseTheCache()
        {

            DateTime expectedHubTime = DateTime.Now;
            const int dstOffset = -5;
            const int offset = 1;
            ICache cache = new HttpRuntimeCache();

            cache.Remove(
                string.Format(TimeBrokerProxy.CACHE_KEY_FORMAT_STRING, offset, dstOffset));

            ITimeBroker mockTimeBroker = MockRepository.GenerateMock<ITimeBroker>();

            var timeBrokerProxy = new TimeBrokerProxy( mockTimeBroker, cache);
            mockTimeBroker.Expect( ( x => x.TimeAt( offset, dstOffset ) ) ).Return( expectedHubTime ).Repeat.Once();
            timeBrokerProxy.TimeAt( offset, dstOffset );

            mockTimeBroker.Expect( ( x => x.TimeAt( offset, dstOffset ) ) ).Repeat.Never();
            DateTime proxyHubTime = timeBrokerProxy.TimeAt( offset, dstOffset );

            AssertAreSameUptillTheSecond( expectedHubTime, proxyHubTime, toleranceAllowedInSeconds );
            mockTimeBroker.VerifyAllExpectations();
        }


        /// <summary>
        /// Custom assertion to compare <see cref="TimeSpan"/> objects with a tolerance for
        /// seconds.
        /// </summary>
        /// <param name="expected">The expected.</param>
        /// <param name="actual">The actual.</param>
        /// <param name="secondsDelta">The seconds delta.</param>
        private static void AssertAreSameUptillTheSecond( DateTime expected, DateTime actual, int secondsDelta )
        {
            Assert.AreEqual( expected.Date, actual.Date,"The dates should be the same" );

            Assert.AreEqual( expected.Hour, actual.Hour,"The hours should be the same" );

            Assert.AreEqual( expected.Minute, actual.Minute, "The minutes should be the same" );

            Assert.AreEqual( expected.Second, actual.Second, secondsDelta, "The seconds delta should not be more than: " + secondsDelta );
        }
    }
}
