using System.Configuration;
using System.Data.SqlClient;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture]
    public class PriorVisitBrokerTests : AbstractBrokerTests
    {
        #region Constants

        private const string CONNECTION_STRING_NAME = "ConnectionString";

        #endregion

        #region SetUp and TearDown PriorVisitBrokerTests

        [SetUp]
        public void SetUpPriorVisitBrokerTests()
        {
            i_FacilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            priorVisitBroker = BrokerFactory.BrokerOfType<IPriorVisitBroker>();
            i_ICEFacility = i_FacilityBroker.FacilityWith(ICE_FACILITYID);
            dbConnection = new SqlConnection
            {
                ConnectionString = ConfigurationManager.ConnectionStrings[CONNECTION_STRING_NAME]
                    .ConnectionString
            };
            dbConnection.Open();
        }

        [TearDown]
        public void TearDownPriorVisitBrokerTests()
        {
            if (dbConnection != null)
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        #endregion

        #region Test Methods

        [Test]
        public void TestGetPriorVisitResponse_WhenPatientHasMatchingCertificateNumber_ShouldReturnNonZeroResults()
        {
            var priorVisitRequest = new PriorVisitInformationRequest
            {
                Facility = i_ICEFacility,
                AccountNumber = ACCOUNT_NUMBER,
                First1OfFirstName = FIRST1_FIRSTNAME,
                First4OfLastName = FIRST4_LASTNAME,
                DateOfBirth = DOB,
                Gender = INPUT_GENDER,
                MedicareHic = string.Empty,
                MBINumber = string.Empty,
                Certificate = CERTIFICATE_NUMBER
            };
            var priorVisitResults = priorVisitBroker.GetPriorVisitResponse(priorVisitRequest);
            Assert.AreNotEqual(PRIOR_ACCOUNTNUMBER_WITH_ZEROES, priorVisitResults.PriorAccountNumber.Trim(),
                "There is no prior visit found for this patient");
        }

        [Test]
        [Ignore] //"This test is dependent on PBAR procedure"
        public void TestGetPriorVisitResponse_WhenPatientHasMatchingMedicareHICNumber_ShouldReturnNonZeroResults()
        {
            var priorVisitRequest = new PriorVisitInformationRequest
            {
                Facility = i_ICEFacility,
                AccountNumber = INPUT_ACCOUNT_NUMBER,
                First1OfFirstName = INPUT_FIRST1_FIRSTNAME,
                First4OfLastName = INPUT_FIRST4_LASTNAME,
                DateOfBirth = INPUT_DOB,
                Gender = INPUT_GENDER,
                MedicareHic = INPUT_MEDICARE_HIC,
                Certificate = string.Empty,
            };
            var priorVisitResults = priorVisitBroker.GetPriorVisitResponse(priorVisitRequest);
            Assert.AreNotEqual(PRIOR_ACCOUNTNUMBER_WITH_ZEROES, priorVisitResults.PriorAccountNumber.Trim(),
                "There is no prior visit found for this patient");
        }

        #endregion

        #region Support Methods

        #endregion

        #region Properties

        #endregion

        #region Data Elements

        private static IPriorVisitBroker priorVisitBroker;
        private static SqlConnection dbConnection;
        private Facility i_ICEFacility;
        private IFacilityBroker i_FacilityBroker;

        #endregion

        #region Constants

        private const string ACCOUNT_NUMBER = "000924092",
            FIRST1_FIRSTNAME = "B",
            FIRST4_LASTNAME = "HILL",
            DOB = "19350425",
            CERTIFICATE_NUMBER = "248487476A",

            INPUT_ACCOUNT_NUMBER = "000924126",
            INPUT_FIRST1_FIRSTNAME = "C",
            INPUT_FIRST4_LASTNAME = "NOVO",
            INPUT_DOB = "19521208",
            INPUT_GENDER = "F",
            INPUT_MEDICARE_HIC = "156263988A";

        private const string PRIOR_ACCOUNTNUMBER_WITH_ZEROES = "000000000";

        private const long ICE_FACILITYID = 98;

        #endregion
    }
}