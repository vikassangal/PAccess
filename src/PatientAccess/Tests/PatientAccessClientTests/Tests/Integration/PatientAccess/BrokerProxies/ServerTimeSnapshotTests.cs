using System;
using NUnit.Framework;
using PatientAccess.BrokerProxies;

namespace Tests.Integration.PatientAccess.BrokerProxies
{
    /// <summary>
    /// Summary description for ServerTimeSnapshotTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class ServerTimeSnapshotTests
    {

        /// <summary>
        /// Tests the calculate local time.
        /// </summary>
        [Test]
        public void TestCalculateLocalTime()
        {
            try
            {

                var snapshot = new ServerTimeSnapshot();
                var currentDateTime = DateTime.Now;

                // This should be a local time
                var serverDateTime = currentDateTime.Subtract(TimeSpan.FromMinutes(5));

                // These two should be UTC to avoid any timezone issues
                var creationDateTime = serverDateTime.ToUniversalTime().AddSeconds(10);
                var currentTimeOverride = creationDateTime.AddMinutes(10);

                // Dummy up the data for the calculation
                snapshot.ServerDateTime = serverDateTime;
                snapshot.TimeSnapshotCreated = creationDateTime;
                ServerTimeSnapshot.DateTimeOverride = currentTimeOverride;

                // This should mirror the calculation done by the method
                var expectedResult =
                    serverDateTime + (currentTimeOverride - creationDateTime);

                Assert.AreEqual(expectedResult, snapshot.CalculateLocalTime());

            }
            finally
            {
                ServerTimeSnapshot.DateTimeOverride = DateTime.MinValue;                
            }

        }
    }
}
