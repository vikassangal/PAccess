using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
    public class GuarantorEmailAddressRequiredTests
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
            var ruleUnderTest = new GuarantorEmailAddressRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo(inValidObjectType);
            Assert.IsTrue(actualResult);
        }
        [Test]
        public void TestCanBeAppliedTo_WithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new GuarantorEmailAddressRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo(null);
            Assert.IsTrue(actualResult);
        }

        [Test]
        public void TestCanBeAppliedTo_CreatedDateBeforeReleaseDate_ShouldReturnTrue_ShortRegistrationActivity()
        {
            var account = GetAccount(new ShortRegistrationActivity(), GetTestDateBeforeFeatureStart());
            var ruleUnderTest = new GuarantorEmailAddressRequired();
            account.IsShortRegistered = true;
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_CreatedDateAfterReleaseDate_ShouldReturnFalse_ShortRegistrationActivity()
        {
            var account = GetAccount(new ShortRegistrationActivity(), GetTestDateAfterFeatureStart());
            var ruleUnderTest = new GuarantorEmailAddressRequired();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_CreatedDateAfterReleaseDate_ShortRegistrationActivity_GuarantorEmailIsPopulated_ShouldReturnTrue()
        {
            var account = GetAccount(new ShortRegistrationActivity(), GetTestDateAfterFeatureStart());
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
            var ruleUnderTest = new GuarantorEmailAddressRequired();
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
