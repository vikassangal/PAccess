using System;
using Extensions.PersistenceCommon;
using NUnit.Framework; 
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MonthlyDueDateRequiredTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class MonthlyDueDateRequiredTests
    {
        
        #region Test Methods
        [Test]
        public void TestCanBeAppliedToWithInvalidContextType_ShouldReturnTrue()
        {
            MonthlyDueDateRequired ruleUnderTest = new MonthlyDueDateRequired();
            object inValidObjectType = new object();
            var actualResult = ruleUnderTest.CanBeAppliedTo( inValidObjectType );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedToWithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new MonthlyDueDateRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo( null );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var ruleUnderTest = new MonthlyDueDateRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( otherCoverage ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndValidFacility_HasOusstandingBalance_NumberOfMonthlyPaymentsIsZero_ShouldReturTrue()
        {
            Account account = GetAccount(new RegistrationActivity(), DHFFacility, TotalPaidLessThanDue, TotalCurrentAmtDue, String.Empty);
            var ruleUnderTest = new MonthlyDueDateRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndValidFacility_HasOusstandingBalance_NumberOfMonthlyPaymentsIsNotZero_ShouldReturFalse()
        {
            Account account = GetAccount(new RegistrationActivity(), DHFFacility, TotalPaidLessThanDue, TotalCurrentAmtDue, String.Empty , 15 , 100 );
            var ruleUnderTest = new MonthlyDueDateRequired();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistration_ValidFacility_ShouldReturnTrue()
        {
            var account = GetAccount(new PreRegistrationActivity(), DHFFacility, TotalPaidLessThanDue, TotalCurrentAmtDue, DayOfMOnthlyPayment );
            var ruleUnderTest = new MonthlyDueDateRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistration_InValidFacility_ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), ICEFacility, TotalPaidLessThanDue, TotalCurrentAmtDue, String.Empty);
            var ruleUnderTest = new MonthlyDueDateRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndValidFacility_BalanceDueIsZero_ShouldReturnTrue()
        {
            Account account = GetAccount(new RegistrationActivity(), DHFFacility, TotalCurrentAmtDue, TotalCurrentAmtDue, String.Empty);
            var ruleUnderTest = new MonthlyDueDateRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndValidFacility_TotalCurrentAmountDueIsZero_ShouldReturnTrue()
        {
            Account account = GetAccount(new RegistrationActivity(), DHFFacility, TotalPaidMoreThanDue, 0, String.Empty);
            var ruleUnderTest = new MonthlyDueDateRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
        public void
            TestCanBeAppliedTo_WhenActivityIsRegistrationAndValidFacility_BalanceDueIsNotZero_DayOfMonthlyDueIsEmpty_ShouldReturnFalse
            ()
        {
            Account account = GetAccount(new RegistrationActivity(), DHFFacility, TotalPaidMoreThanDue, TotalCurrentAmtDue,  String.Empty);
            var ruleUnderTest = new MonthlyDueDateRequired();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        #endregion

        #region Support Methods

        private static Account GetAccount(Activity activity, Facility facility, decimal totalPaid, decimal totalCurrentAmountDue, string monthlyDueDate, int numberOfMonthlyPayments = 0, int monthlyPayment = 0)
        {
            return new Account
            {
                Activity = activity,
                Facility = facility,
                TotalPaid = totalPaid,
                DayOfMonthPaymentDue = monthlyDueDate ,
                TotalCurrentAmtDue = totalCurrentAmountDue,
                NumberOfMonthlyPayments = numberOfMonthlyPayments,
                MonthlyPayment = monthlyPayment
            };
        }
        private Facility ICEFacility
        {
            get
            {
                var facility = new Facility(98,
                    PersistentModel.NEW_VERSION,
                    "ICE",
                    "ICE");
                facility["MonthlyDueDateEnabled"] = null;
                return facility;
            }
        }

        private Facility DHFFacility
        {

            get
            {
                var facility = new Facility(54,
                    PersistentModel.NEW_VERSION,
                    "DHF",
                    "DHF");
                facility["MonthlyDueDateEnabled"] = true;
                return facility;
            }
        }

        private decimal TotalPaidMoreThanDue = 300;
        private decimal TotalPaidLessThanDue = 100;
        private string DayOfMOnthlyPayment = "24";
        private decimal TotalCurrentAmtDue = 200;
        #endregion

    }
}
