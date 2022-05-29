using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class CurrencyTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown CurrencyTests
        [TestFixtureSetUp()]
        public static void SetUpCurrencyTests()
        {

        }

        [TestFixtureTearDown()]
        public static void TearDownCurrencyTests()
        {

        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestConstructors()
        {
            cur1 = new Currency(1.5f);
            Assert.AreEqual(cur1.ExchangeRate, 1.5f);

            cur1 = new Currency(0, string.Empty, string.Empty, 1.5f);
            cur1 = new Currency(0, DateTime.Today, string.Empty, 1.5f);
            cur1 = new Currency(0, DateTime.Today, string.Empty, string.Empty, 1.5f);
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static Currency cur1 = null;
        #endregion
    }
}