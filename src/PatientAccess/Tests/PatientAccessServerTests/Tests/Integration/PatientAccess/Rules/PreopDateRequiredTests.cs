using System;
using PatientAccess.Domain;
using PatientAccess.Rules;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Rules
{
    [TestFixture]
    [Category( "Fast" )]
    public class PreopDateRequiredTests
    {
        #region Test Methods

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION) };
            var ruleUnderTest = new PreopDateRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(otherCoverage));
        }

        
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationPreopDateIsNotMin_ShouldReturnTrue()
        {
            Account account = GetAccount(new RegistrationActivity(), DateTime.Now);
            var ruleUnderTest = new PreopDateRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

          
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndPreopDateIsMin_ShouldReturnFalse()
        {

            var account = GetAccount(new PreRegistrationActivity(), DateTime.MinValue);

            Account.CpasFeatureActivationDate = DateTime.Now - TimeSpan.FromDays(2);

            var ruleUnderTest = new PreopDateRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndPreopDateIsNotMin_ShouldReturnTrue()
        {
            var account = GetAccount(new PreRegistrationActivity(), DateTime.Now);
            var ruleUnderTest = new PreopDateRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

     #endregion
        #region Support Methods
       
        private static Account GetAccount(Activity activity, DateTime preopDate)
        {
            return new Account { Activity = activity, PreopDate = preopDate };
        }

        #endregion
    }
}
