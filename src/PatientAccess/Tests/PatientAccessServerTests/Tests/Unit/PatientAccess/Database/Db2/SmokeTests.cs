using IBM.Data.DB2.iSeries;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;

namespace Tests.Unit.PatientAccess.Database.Db2
{
    [TestFixture]
    public class SmokeTests
    {
        /// <summary>
        /// Just an environmental smoke test to ensure that we are talking to the
        /// version of DB2 we expect to find.
        /// </summary>
        [Test]
        public void VerifyDatabaseVersion_ShouldBeV6R1()
        {
            const long acoFacilityid = 900;
            const string expectedDatabaseVersion = "07.04.0000";
            var facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            var facility = facilityBroker.FacilityWith(acoFacilityid);
            var connectionString = facility.ConnectionSpec.ConnectionString;

            using (var connection = new iDB2Connection(connectionString))
            {
                connection.Open();
                Assert.AreEqual(expectedDatabaseVersion, connection.ServerVersion);
            }
        }
    }
}