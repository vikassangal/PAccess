using PatientAccess.BrokerInterfaces;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture]
    public class EmailReasonBrokerTests : AbstractBrokerTests
    {
        #region Constants
        private const int TOTAL_NUMBER_OF_EMAILREASONS = 4;
        #endregion

        #region Test Methods

        [Test]
        public void TestEmailReasonList()
        {
            var list = BrokerFactory.BrokerOfType<IEmailReasonBroker>().AllEmailReasons();
            Assert.IsTrue(list.Count == TOTAL_NUMBER_OF_EMAILREASONS, "Incorrect number of EmailReason Values returned");
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        #endregion
    }
}