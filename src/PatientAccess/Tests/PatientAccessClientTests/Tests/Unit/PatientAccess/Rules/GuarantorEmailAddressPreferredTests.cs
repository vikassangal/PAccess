﻿using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
    public class GuarantorEmailAddressPreferredTests
    {
        [SetUp]
        public void SetUpFeatureManager()
        {
            emailAddressFeatureManager = new EmailAddressFeatureManager();
        }
        [Test]
        public void TestCanBeAppliedTo_WithInvalidContextType_ShouldReturnTrue()
        {
            var inValidObjectType = new object();
            var ruleUnderTest = new GuarantorEmailAddressPreferred();
            var actualResult = ruleUnderTest.CanBeAppliedTo(inValidObjectType);
            Assert.IsTrue(actualResult);
        }
        [Test]
        public void TestCanBeAppliedTo_WithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new GuarantorEmailAddressPreferred();
            var actualResult = ruleUnderTest.CanBeAppliedTo(null);
            Assert.IsTrue(actualResult);
        }

        [Test]
        public void TestCanBeAppliedTo_CreatedDateBeforeReleaseDate_ShouldReturnTrue_ShortPreRegistrationActivity()
        {
            var account = GetAccount(new ShortPreRegistrationActivity(), GetTestDateBeforeFeatureStart());
            var ruleUnderTest = new GuarantorEmailAddressPreferred();
            account.IsShortRegistered = true;
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_CreatedDateAfterReleaseDate_ShouldReturnFalse_ShortPreRegistrationActivity()
        {
            var account = GetAccount(new ShortPreRegistrationActivity(), GetTestDateAfterFeatureStart());
            var ruleUnderTest = new GuarantorEmailAddressPreferred();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_CreatedDateAfterReleaseDate_ShortPreRegistrationActivity_GuarantorEmailIsPopulated_ShouldReturnTrue()
        {
            var account = GetAccount(new ShortPreRegistrationActivity(), GetTestDateAfterFeatureStart());
            account.Guarantor.AddContactPoint(
            new ContactPoint(
                new Address(
                    "1 Microsoft Way (New Address)",
                    "Suite 2",
                    "Redmond",
                    new ZipCode("12345"),
                    new State(),
                    Country.NewUnitedStatesCountry(),
                    new County("2")),
                new PhoneNumber("456", "7894561"),
                new EmailAddress("someone@microsoft.com"),
                TypeOfContactPoint.NewMailingContactPointType()));
            var ruleUnderTest = new GuarantorEmailAddressPreferred();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
        #region Support Methods
        private static Account GetAccount(Activity activity, DateTime accountCreatedDate)
        {
           return  new Account
            {
                Activity = activity,
                AccountCreatedDate = accountCreatedDate,
                Guarantor = new Guarantor()
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
