using PatientAccess.BrokerInterfaces;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture]
    public class CellPhoneConsentBrokerTests : AbstractBrokerTests
    {
        #region Constants
        private const int TOTAL_NUMBER_OF_CONSENTS = 7;
        #endregion

        #region Test Methods

        [Test]
        public void TestConsentsList()
        {
            var list = BrokerFactory.BrokerOfType<ICellPhoneConsentBroker>().AllCellPhoneConsents();

            Assert.IsNotNull(list, "No list returned from cellPhoneConsentBroker.AllGuarantorConsents");
            Assert.IsTrue( list.Count > 0, "No Consents Returned" );
            Assert.IsTrue(list.Count == TOTAL_NUMBER_OF_CONSENTS, "Incorrect number of Consent Values returned");
        }

       #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        #endregion
    }
}