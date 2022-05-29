using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class CreditCardPaymentTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown CreditCardProviderTests
        [TestFixtureSetUp()]
        public static void SetUpCreditCardProviderTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownCreditCardProviderTests()
        {
            ccp = null;
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestCreditCardPayments()
        {
            ccp = new CreditCardPayment();
            ccp = new CreditCardPayment(CreditCardProvider.MasterCard());
            ccp.CardNumber = string.Empty;
            ccp.ExpirationDate = DateTime.Today;

            Assert.AreEqual(string.Empty, ccp.CardNumber);
            Assert.AreEqual(DateTime.Today, ccp.ExpirationDate);
            Assert.AreEqual(CreditCardProvider.MASTERCARD_OID, ccp.CardType.Oid);
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static CreditCardPayment ccp = null;
        #endregion
    }
}