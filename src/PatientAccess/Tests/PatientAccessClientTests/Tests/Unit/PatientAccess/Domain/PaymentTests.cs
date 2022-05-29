using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class PaymentTests
    {
        #region Constants
        private const string
            CHECK_NUMBER = "786";

        private const decimal
            NO_MONEY = 0.00m;
        #endregion

        #region SetUp and TearDown PaymentTests
        [TestFixtureSetUp()]
        public static void SetUpPaymentTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownPaymentTests()
        {
        }

        [SetUp()]
        public void SetUpPayments()
        {
            aNewPayment = new Payment();
            aCashPaymentAmount = new PaymentAmount(100.00m, new CashPayment());
            aCheckPaymentAmount = new PaymentAmount(256.00m, new CheckPayment(CHECK_NUMBER));
            aCreditCardPaymentAmount = new PaymentAmount(1234.00m, new CreditCardPayment(CreditCardProvider.Visa()));
            aMoneyOrderPaymentAmount = new PaymentAmount(356.24m, new MoneyOrderPayment());
        }

        #endregion

        #region Test Methods
        [Test()]
        public void TestConstructors()
        {
            Assert.AreEqual(typeof(CashPayment), aCashPaymentAmount.PaymentMethod.GetType(), "Object is NOT a CashPayment");
            Assert.AreEqual(typeof(CheckPayment), aCheckPaymentAmount.PaymentMethod.GetType(), "Object is NOT a CheckPayment");
            Assert.AreEqual(CHECK_NUMBER, ((CheckPayment)aCheckPaymentAmount.PaymentMethod).CheckNumber);
            Assert.AreEqual(typeof(CreditCardPayment), aCreditCardPaymentAmount.PaymentMethod.GetType(), "Object is NOT a CreditCardPayment");
            Assert.AreEqual(CreditCardProvider.Visa(), ((CreditCardPayment)aCreditCardPaymentAmount.PaymentMethod).CardType);
            Assert.AreEqual(typeof(MoneyOrderPayment), aMoneyOrderPaymentAmount.PaymentMethod.GetType(), "Object is NOT a MoneyOrderPayment");
            Assert.AreEqual(100.00m, aCashPaymentAmount.AmountPaid());
            Assert.AreEqual(256.00m, aCheckPaymentAmount.AmountPaid());
            Assert.AreEqual(1234.00m, aCreditCardPaymentAmount.AmountPaid());
            Assert.AreEqual(356.24m, aMoneyOrderPaymentAmount.AmountPaid());

        }

        [Test()]
        public void TestNewPayment()
        {
            Assert.AreEqual(NO_MONEY, aNewPayment.AmountPaidWith(Payment.PaymentType.Cash));
            Assert.AreEqual(NO_MONEY, aNewPayment.AmountPaidWith(Payment.PaymentType.Check));
            Assert.AreEqual(NO_MONEY, aNewPayment.AmountPaidWith(Payment.PaymentType.CreditCard1));
            Assert.AreEqual(NO_MONEY, aNewPayment.AmountPaidWith(Payment.PaymentType.MoneyOrder));
        }

        [Test()]
        public void TestAddPayments()
        {
            aNewPayment.TotalPayment = 125.00m;

            aNewPayment.AddPayment( aCashPaymentAmount,Payment.PaymentType.Cash );
            Assert.AreEqual( 225.00m, aNewPayment.TotalRecordedPayment );
            Assert.AreEqual( 100.00m, aNewPayment.CalculateTotalPayments() );

            aNewPayment.AddPayment(aCheckPaymentAmount, Payment.PaymentType.Check);
            Assert.AreEqual( 481.00m, aNewPayment.TotalRecordedPayment );
            Assert.AreEqual( 356.00m, aNewPayment.CalculateTotalPayments() );
            Assert.AreEqual( CHECK_NUMBER, aNewPayment.CheckNumber() );


            aNewPayment.AddPayment(aCreditCardPaymentAmount, Payment.PaymentType.CreditCard1);
            Assert.AreEqual( 1715.00m, aNewPayment.TotalRecordedPayment );
            Assert.AreEqual( 1590.00m, aNewPayment.CalculateTotalPayments() );
            Assert.AreEqual("Visa", aNewPayment.CardType( Payment.PaymentType.CreditCard1 ));

            aNewPayment.AddPayment( aMoneyOrderPaymentAmount, Payment.PaymentType.MoneyOrder );
            Assert.AreEqual( 2071.24m, aNewPayment.TotalRecordedPayment );
            Assert.AreEqual( 1946.24m, aNewPayment.CalculateTotalPayments() );

            //Add new payments of the same types and check that they overwrite the existing type of payment
            aNewPayment.AddPayment(new PaymentAmount(NO_MONEY, new CashPayment()), Payment.PaymentType.Cash );
            Assert.AreEqual( 1971.24m, aNewPayment.TotalRecordedPayment );
            Assert.AreEqual( 1846.24m, aNewPayment.CalculateTotalPayments() );

            aNewPayment.AddPayment(new PaymentAmount(NO_MONEY, new CheckPayment()), Payment.PaymentType.Check);
            Assert.AreEqual( 1715.24m, aNewPayment.TotalRecordedPayment );
            Assert.AreEqual( 1590.24m, aNewPayment.CalculateTotalPayments() );

            aNewPayment.AddPayment(new PaymentAmount(NO_MONEY, new CreditCardPayment()), Payment.PaymentType.CreditCard1 );
            Assert.AreEqual( 481.24m, aNewPayment.TotalRecordedPayment );
            Assert.AreEqual( 356.24m, aNewPayment.CalculateTotalPayments() );

            aNewPayment.AddPayment(new PaymentAmount(NO_MONEY, new MoneyOrderPayment()), Payment.PaymentType.MoneyOrder);
            Assert.AreEqual( 125.00m, aNewPayment.TotalRecordedPayment );
            Assert.AreEqual( NO_MONEY, aNewPayment.CalculateTotalPayments() );
        }

        [Test()]
        public void TestAmountPaidWith()
        {
            aNewPayment.AddPayment(aCashPaymentAmount, Payment.PaymentType.Cash );
            Assert.AreEqual( aCashPaymentAmount.AmountPaid(), aNewPayment.AmountPaidWith( Payment.PaymentType.Cash ) );

            aNewPayment.AddPayment(aCheckPaymentAmount, Payment.PaymentType.Check );
            Assert.AreEqual( aCheckPaymentAmount.AmountPaid(), aNewPayment.AmountPaidWith( Payment.PaymentType.Check ) );

            aNewPayment.AddPayment(aCreditCardPaymentAmount, Payment.PaymentType.CreditCard1 );
            Assert.AreEqual( aCreditCardPaymentAmount.AmountPaid(), aNewPayment.AmountPaidWith( Payment.PaymentType.CreditCard1 ) );

            aNewPayment.AddPayment(aMoneyOrderPaymentAmount, Payment.PaymentType.MoneyOrder );
            Assert.AreEqual( aMoneyOrderPaymentAmount.AmountPaid(), aNewPayment.AmountPaidWith( Payment.PaymentType.MoneyOrder ) );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static PaymentAmount aCashPaymentAmount = null;
        private static PaymentAmount aCheckPaymentAmount = null;
        private static PaymentAmount aCreditCardPaymentAmount = null;
        private static PaymentAmount aMoneyOrderPaymentAmount = null;
        private static Payment aNewPayment = null;
        #endregion
    }
}