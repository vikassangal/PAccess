using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
    public class EmailReasonFeaturemanagerTests
    {
        [SetUp]
        public void SetUpFeatureManager()
        {
            emailReasonFeatureManager = new EmailReasonFeatureManager();
        }

        public void TestCanBeAppliedTo_WithNullContext_ShouldReturnFalse()
        {

            var actualResult = emailReasonFeatureManager.ShouldFeatureBeVisibleForAccountCreatedDate(null);
            Assert.IsFalse(actualResult);
        }
        [Test]
        public void TestRegistrationEmailReasonVisible_AccountCreatedBeforeReleaseDate_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateBeforeFeatureStart());
            Assert.IsFalse(emailReasonFeatureManager.ShouldFeatureBeVisibleForAccountCreatedDate(account));
        }

        [Test]
        public void TestRegistrationEmailReasonVisible_AccountCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart());
            Assert.IsTrue(emailReasonFeatureManager.ShouldFeatureBeVisibleForAccountCreatedDate(account));
        }

        [Test]
        public void TestPreRegistrationEmailReasonVisible_AccountCreatedBeforeReleaseDate_ShouldReturnFalse()
        {
            var account = GetAccount(new PreRegistrationActivity(), GetTestDateBeforeFeatureStart());
            Assert.IsFalse(emailReasonFeatureManager.ShouldFeatureBeVisibleForAccountCreatedDate(account));
        }

        [Test]
        public void TestPreRegistrationEmailReasonVisible_AccountCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterFeatureStart());
            Assert.IsTrue(emailReasonFeatureManager.ShouldFeatureBeVisibleForAccountCreatedDate(account));
        }

        [Test]
        public void TestShortRegistrationEmailReasonVisible_AccountCreatedBeforeReleaseDate_ShortRegistration_ShouldReturnFalse()
        {
            var account = GetAccount(new ShortRegistrationActivity(), GetTestDateBeforeFeatureStart());
            Assert.IsFalse(emailReasonFeatureManager.ShouldFeatureBeVisibleForAccountCreatedDate(account));
        }

        [Test]
        public void TestShortRegistrationEmailReasonVisible_AccountCreatedAfterReleaseDate_ShortRegistration_ShouldReturnTrue()
        {
            var account = GetAccount(new ShortRegistrationActivity(), GetTestDateAfterFeatureStart());
            Assert.IsTrue(emailReasonFeatureManager.ShouldFeatureBeVisibleForAccountCreatedDate(account));
        }

        [Test]
        public void TestShortPreRegistrationEmailReasonVisible_AccountCreatedBeforeReleaseDate_ShortPreregistrtaion_ShouldReturnFalse()
        {
            var account = GetAccount(new ShortPreRegistrationActivity(), GetTestDateBeforeFeatureStart());
            Assert.IsFalse(emailReasonFeatureManager.ShouldFeatureBeVisibleForAccountCreatedDate(account));
        }

        [Test]
        public void TestShortPreRegistrationEmailReasonVisible_AccountCreatedAfterReleaseDate_ShortPreRegistration_ShouldReturnTrue()
        {
            var account = GetAccount(new ShortPreRegistrationActivity(), GetTestDateAfterFeatureStart());
            Assert.IsTrue(emailReasonFeatureManager.ShouldFeatureBeVisibleForAccountCreatedDate(account));
        }

        #region Support Methods
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
            return emailReasonFeatureManager.FeatureStartDate.AddDays(10);
        }
        private DateTime GetTestDateBeforeFeatureStart()
        {
            return emailReasonFeatureManager.FeatureStartDate.AddDays(-10);
        }
        #endregion

        #region constants

        private EmailReasonFeatureManager emailReasonFeatureManager;

        #endregion

    }

}
