using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class CreditValidationResponseTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown CreditReportTests
        [TestFixtureSetUp()]
        public static void SetUpCreditValidationResponseTests()
        {
            cvr1 = new CreditValidationResponse();
            
        }

        [TestFixtureTearDown()]
        public static void TearDownCreditValidationResponseTests()
        {
            cvr1 = null;
            
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestCreditValidationResponse()
        {
            Assert.IsNull(cvr1.ReturnedDataValidationTicket);
            Assert.IsNull(cvr1.ResponseCreditReport);
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static CreditValidationResponse cvr1 = null;
        #endregion
    }
}