
using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
    public class PullPriorVisitInsuranceToPreMseFeatureManagerTests
    {
        #region Defaut Scenario

        [Test]
        public void TestCanBeAppliedTo_WithNullContext_IsAccountCreatedBeforeImplimentationDate_ShouldReturnFalse()
        {
            var pullPriorVisitInsuranceToPreMseFeatureManager = new PullPriorVisitInsuranceToPreMseFeatureManager();
            var actualResult = pullPriorVisitInsuranceToPreMseFeatureManager.IsPullPriorVisitInsuranceToPreMseEnabledForDate(null);

            Assert.IsFalse(actualResult);
        }

        [Test]
        public void TestCanBeAppliedTo_WithNullContext_IsAccountCreatedAfterImplimentationDate_ShouldReturnFalse()
        {
            var pullPriorVisitInsuranceToPreMseFeatureManager = new PullPriorVisitInsuranceToPreMseFeatureManager();
            var actualResult = pullPriorVisitInsuranceToPreMseFeatureManager.IsPullPriorVisitInsuranceToPreMseEnabledForDate(null);

            Assert.IsFalse(actualResult);
        }
   
        #endregion

        [Test]
        public void TestPullPriorVisitInsuranceToPreMseFeatureEnabled_AccountCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            var pullPriorVisitInsuranceToPreMseFeatureManager = new PullPriorVisitInsuranceToPreMseFeatureManager();

            var account = GetAccount(GetTestDateAfterFeatureStart()); 
            var actualResult = pullPriorVisitInsuranceToPreMseFeatureManager.IsPullPriorVisitInsuranceToPreMseEnabledForDate(account);

            Assert.IsTrue(actualResult);
        }
       
        [Test]
        public void TestPullPriorVisitInsuranceToPreMseFeatureEnabled_AccountCreatedBeforeReleaseDate_ShouldReturnFalse()
        {
            var pullPriorVisitInsuranceToPreMseFeatureManager = new PullPriorVisitInsuranceToPreMseFeatureManager();

            var account = GetAccount(GetTestDateBeforeFeatureStart()); 
            var actualResult = pullPriorVisitInsuranceToPreMseFeatureManager.IsPullPriorVisitInsuranceToPreMseEnabledForDate(account);

            Assert.IsFalse(actualResult);
        }
   
        
        private static Account GetAccount( DateTime accountCreatedDate)
        {
            return new Account
            {
                AccountCreatedDate = accountCreatedDate
            };
        }
        
        #region Constants
        private static DateTime GetTestDateAfterFeatureStart()
        {
            return new PullPriorVisitInsuranceToPreMseFeatureManager().FeatureStartDate.AddDays(10);
        }
        private static DateTime GetTestDateBeforeFeatureStart()
        {
            return new PullPriorVisitInsuranceToPreMseFeatureManager().FeatureStartDate.AddDays(-10);
        }
        #endregion
    }
}
