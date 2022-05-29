using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
    public class GuarantorValidationMustBeViewedTests
    {
        #region Constants

        private readonly FinancialClass FC_14 = new FinancialClass(ReferenceValue.NEW_OID, ReferenceValue.NEW_VERSION, "NonGovernment", "14");
        private readonly FinancialClass FC_80 = new FinancialClass(ReferenceValue.NEW_OID, ReferenceValue.NEW_VERSION, "NonGovernment", "80");
        private readonly FinancialClass FC_42 = new FinancialClass(ReferenceValue.NEW_OID, ReferenceValue.NEW_VERSION, "MEDICARE", "42");

        #endregion

        #region Test Methods

        [Test]
        public void TestCanBeAppliedto_When_FinancialClass14_ShouldReturnFalse()
        {
            var account = GetAccount();
            account.FinancialClass = FC_14;
            var guarantorValidationMustBeViewed = new GuarentorValidationMustBeViewed();
            Assert.IsFalse(guarantorValidationMustBeViewed.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedto_When_FinancialClass80_ShouldReturnFalse()
        {
            var account = GetAccount();
            account.FinancialClass = FC_80;
            var guarantorValidationMustBeViewed = new GuarentorValidationMustBeViewed();
            Assert.IsFalse(guarantorValidationMustBeViewed.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedto_When_FinancialClass42_ShouldReturnTrue()
        {
            var account = GetAccount();
            account.FinancialClass = FC_42;
            var guarantorValidationMustBeViewed = new GuarentorValidationMustBeViewed();
            Assert.IsTrue(guarantorValidationMustBeViewed.CanBeAppliedTo(account));

        }
        #endregion

        #region Support Methods

        private Account GetAccount()
        {
            var account = new Account();
            var aGuarantor = new Guarantor();
            var datavalidationTicket = new DataValidationTicket
            {
                ResultsAvailable = true,
                ResultsReviewed = false
            };

            aGuarantor.DataValidationTicket = datavalidationTicket;
            account.Guarantor = aGuarantor;
            return account;
        }

        #endregion
        #region Data Elements

        #endregion
    }
}