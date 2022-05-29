using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class PaymentAmountTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown PaymentAmountTests
        [TestFixtureSetUp]
        public static void SetUpPaymentAmountTests()
        {
        }

        [TestFixtureTearDown]
        public static void TearDownPaymentAmountTests()
        {
        }
        #endregion

        #region Test Methods
        [Test]
        public void TestPaymentAmount()
        {
            PaymentAmount amt = new PaymentAmount 
                                { Money = new Money( 30 ), PaymentMethod = new CashPayment() };

            Assert.AreEqual( amt.Money.Amount, 30 );

            Assert.AreEqual( typeof( CashPayment ), amt.PaymentMethod.GetType() );
        }

        [Test]
        public void TestPaymentAmountConstructor()
        {
            Money money = new Money( 30 );
            PaymentMethod paymentMethod = new CashPayment();

            PaymentAmount amount = new PaymentAmount( money, paymentMethod );

            Assert.IsNotNull( amount );
            Assert.AreEqual( amount.Money.Amount, 30 );
            Assert.AreEqual( typeof( CashPayment ), amount.PaymentMethod.GetType() );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}