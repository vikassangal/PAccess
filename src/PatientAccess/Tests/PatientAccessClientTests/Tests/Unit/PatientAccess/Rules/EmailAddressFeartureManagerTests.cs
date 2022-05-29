using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;

namespace  Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
    class EmailAddressFeartureManagerTests
    {
        [SetUp]

        public void SetUpFeatureManager()
        {
            emailAddressFeatureManager = new EmailAddressFeatureManager();
        }
        [Test]
        public void TestCanBeAppliedTo_WithNullContext_ShouldReturnFalse()
        {

            var actualResult = emailAddressFeatureManager.ShouldFeatureBeEnabled(null);
            Assert.IsFalse(actualResult);
        }
        [Test]
        public void TestShortRegistrationEmailAddressVisible_AccountCreatedBeforeReleaseDate_ShortRegistration_ShouldReturnFalse()
        {
            var account = GetAccount(new ShortRegistrationActivity(), GetTestDateBeforeFeatureStart());
            account.IsShortRegistered = true;
            Assert.IsFalse(emailAddressFeatureManager.ShouldFeatureBeEnabled(account));
        }

        [Test]
        public void TestShortRegistrationEmailAddressVisible_AccountCreatedAfterReleaseDate_ShortRegistration_ShouldReturnTrue()
        {
            var account = GetAccount(new ShortRegistrationActivity(), GetTestDateAfterFeatureStart());
            Assert.IsTrue(emailAddressFeatureManager.ShouldFeatureBeEnabled(account));
        }

        [Test]
        public void TestShortPreRegistrationEmailAddressVisible_AccountCreatedBeforeReleaseDate_ShortPreregistrtaion_ShouldReturnFalse()
        {
            var account = GetAccount(new ShortPreRegistrationActivity(), GetTestDateBeforeFeatureStart());
            account.IsShortRegistered = true;
            Assert.IsFalse(emailAddressFeatureManager.ShouldFeatureBeEnabled(account));
        }

        [Test]
        public void TestShortPreRegistrationEmailAddressVisible_AccountCreatedAfterReleaseDate_ShortPreregistration_ShouldReturnTrue()
        {
            var account = GetAccount(new ShortPreRegistrationActivity(), GetTestDateAfterFeatureStart());
            Assert.IsTrue(emailAddressFeatureManager.ShouldFeatureBeEnabled(account));
        }
        #region support methods
        private static Account GetAccount(Activity activity, DateTime accountCreatedDate)
        {
            return new Account
            {
                Activity = activity,
                AccountCreatedDate = accountCreatedDate,

            };
        }
        private DateTime GetTestDateAfterFeatureStart()
        {
            return emailAddressFeatureManager.FeatureStartDate.AddDays(10);
        }
        private DateTime GetTestDateBeforeFeatureStart()
        {
            return emailAddressFeatureManager.FeatureStartDate.AddDays(-10);
        }
        #endregion

        #region constants

        private EmailAddressFeatureManager emailAddressFeatureManager;

        #endregion
    }

}
