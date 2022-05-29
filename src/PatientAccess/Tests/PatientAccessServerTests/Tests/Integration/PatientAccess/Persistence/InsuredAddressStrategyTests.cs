using PatientAccess.Domain.Parties;
using PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class InsuredAddressStrategyTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown InsuredAddressStrategyTests
        [TestFixtureSetUp()]
        public static void SetUpInsuredAddressStrategyTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownInsuredAddressStrategyTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestLoadAddressesForInsured()
        {
            Insured insured = new Insured();
            InsuredAddressStrategy strategy = new InsuredAddressStrategy();
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}