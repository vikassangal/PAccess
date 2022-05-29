using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class CreditCardProviderTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown CreditCardProviderTests
        [TestFixtureSetUp()]
        public static void SetUpCreditCardProviderTests()
        {
            noProvider = new CreditCardProvider();
            amexProvider = CreditCardProvider.AmericanExpress();
            discoverProvider = CreditCardProvider.Discover();
            mastercardProvider = CreditCardProvider.MasterCard();
            visaProvider = CreditCardProvider.Visa();
        }

        [TestFixtureTearDown()]
        public static void TearDownCreditCardProviderTests()
        {
            noProvider = null;
            amexProvider = null;
            discoverProvider = null;
            mastercardProvider = null;
            visaProvider = null;
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestConstructors()
        {
            Assert.AreEqual( string.Empty, noProvider.Description );
            Assert.AreEqual( "AMEX", amexProvider.Description );
            Assert.AreEqual( "Discover", discoverProvider.Description );
            Assert.AreEqual( "MasterCard", mastercardProvider.Description );
            Assert.AreEqual( "Visa", visaProvider.Description );

            new CreditCardProvider();
            new CreditCardProvider(0L, string.Empty, string.Empty);
            new CreditCardProvider(0L, DateTime.Today, string.Empty);
            new CreditCardProvider(0L, DateTime.Today, string.Empty, string.Empty);
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static CreditCardProvider noProvider = null;
        private static CreditCardProvider amexProvider = null;
        private static CreditCardProvider discoverProvider = null;
        private static CreditCardProvider mastercardProvider = null;
        private static CreditCardProvider visaProvider = null;
        
        #endregion
    }
}