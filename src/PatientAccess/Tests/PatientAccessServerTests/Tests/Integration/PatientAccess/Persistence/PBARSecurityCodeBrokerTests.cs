using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture]
    public class PBARSecurityCodeBrokerTests : AbstractBrokerTests
    {
        [Test]
        public void TestGetPrintedSecurityCode()
        {
            const string pbarEmployeeId = "ADT099";
            const string expectedPrintedSecurityCode = "TADT";
            const string facilityCode = "DEL";
            var facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            var facility = facilityBroker.FacilityWith(facilityCode);
            var securityCodeBroker = BrokerFactory.BrokerOfType<ISecurityCodeBroker>();

            var dataPrepConnection = new iDB2Connection { ConnectionString = facility.ConnectionSpec.ConnectionString };

            var dataPrepCommand = dataPrepConnection.CreateCommand();
            dataPrepCommand.CommandText = string.Format("INSERT INTO EI0008P (EIEMP#,EISEC2) VALUES('{0}','{1}')", pbarEmployeeId, expectedPrintedSecurityCode);
            

            try
            {
                // Insert the data into the PBAR table so our test has consistent data
                dataPrepConnection.Open();
                var dataPrepTransaction = dataPrepConnection.BeginTransaction();
                dataPrepCommand.Transaction = dataPrepTransaction;
                dataPrepCommand.ExecuteNonQuery();

                // Run the test
                var printedSecurityCode = securityCodeBroker.GetPrintedSecurityCode(pbarEmployeeId, facility);

                // Get rid of the inserted data
                dataPrepTransaction.Rollback();

                Assert.AreEqual(expectedPrintedSecurityCode, printedSecurityCode);
            }

            finally
            {
                dataPrepCommand.Dispose();
                dataPrepConnection.Close();
                dataPrepConnection.Dispose();
            }
        }
    }
}