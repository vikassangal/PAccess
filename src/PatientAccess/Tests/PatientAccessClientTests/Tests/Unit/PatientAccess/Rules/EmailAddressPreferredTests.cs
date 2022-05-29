using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;

namespace  Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
  public  class EmailAddressPreferredTests
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
            var ruleUnderTest = new EmailAddressPreferred();
            var actualResult = ruleUnderTest.CanBeAppliedTo(inValidObjectType);
            Assert.IsTrue(actualResult);
        }
        [Test]
        public void TestCanBeAppliedTo_WithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new EmailAddressPreferred();
            var actualResult = ruleUnderTest.CanBeAppliedTo(null);
            Assert.IsTrue(actualResult);
        }

        [Test]
        public void TestCanBeAppliedTo_CreatedDateBeforeReleaseDate_ShouldReturnTrue_ShortPreRegistrationActivity()
        {
            var account = GetAccount(new ShortPreRegistrationActivity(), GetTestDateBeforeFeatureStart());
            var ruleUnderTest = new EmailAddressPreferred();
            account.IsShortRegistered = true;
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_CreatedDateAfterReleaseDate_ShouldReturnFalse_ShortPreRegistrationActivity()
        {
            var account = GetAccount(new ShortPreRegistrationActivity(), GetTestDateAfterFeatureStart());
            var ruleUnderTest = new EmailAddressPreferred();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_CreatedDateBeforeReleaseDate_ShouldReturnTrue_PreRegistrationActivity()
        {
            var account = GetAccount(new PreRegistrationActivity(), GetTestDateBeforeFeatureStart());
            var ruleUnderTest = new EmailAddressPreferred();
            account.IsShortRegistered = false;
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_CreatedDateBeforeReleaseDate_ShouldReturnFalse_RegistrationActivity()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateBeforeFeatureStart());
            var ruleUnderTest = new EmailAddressPreferred();
            account.IsShortRegistered = false;
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_EmailReasonAsEmailProvided_And_HospitalCommunicationIsYes_And_PatientPortalIsYes_ShouldReturnFalse_ShortPreRegistrationActivity()
        {
            var account = GetAccount(new ShortPreRegistrationActivity(), GetTestDateAfterFeatureStart());
            var ruleUnderTest = new EmailAddressPreferred();
            account.IsShortRegistered = true;
            account.Patient.EmailReason.Code = EmailReason.PROVIDED;
            account.Patient.HospitalCommunicationOptIn.Code = YesNoFlag.CODE_YES; 
            account.PatientPortalOptIn.Code = YesNoFlag.CODE_YES;
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));    
        }
       
        [Test]
        public void TestCanBeAppliedTo_EmailReasonAsBlank_ShouldReturnFalse_ShortPreRegistrationActivity()
        {
            var account = GetAccount(new ShortPreRegistrationActivity(), GetTestDateAfterFeatureStart());
            var ruleUnderTest = new EmailAddressPreferred();
            account.IsShortRegistered = true;
            account.Patient.EmailReason.Code = EmailReason.BLANK;
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_EmailReasonAsBlank_And_HospitalCommunicationIsYes_And_PatientPortalIsNo_ShouldReturnFalse_PreRegistrationActivity()
        {
            var account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterFeatureStart());
            account.KindOfVisit = VisitType.PreRegistration;
            var ruleUnderTest = new EmailAddressPreferred();
            account.IsShortRegistered = false;
            account.Patient.EmailReason.Code = EmailReason.BLANK;
            account.Patient.HospitalCommunicationOptIn.Code = YesNoFlag.CODE_YES;
            account.PatientPortalOptIn.Code = YesNoFlag.CODE_NO;
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_EmailReasonAsPatientDeclinedEmail_And_HospitalCommunicationIsYes_And_PatientPortalIsNo_ShouldReturnTrue_PreRegistrationActivity()
        {
            var account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterFeatureStart());
            account.KindOfVisit = VisitType.PreRegistration;
            var ruleUnderTest = new EmailAddressPreferred();
            account.IsShortRegistered = false;
            account.Patient.EmailReason.Code = EmailReason.DECLINED;
            account.Patient.AddContactPoint(
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
            account.Patient.HospitalCommunicationOptIn.Code = YesNoFlag.CODE_YES;
            account.PatientPortalOptIn.Code = YesNoFlag.CODE_NO;
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_EmailReasonAsRequestToRemoveEmail_ShouldReturnFalse_PreAdmitNewbornActivity()
        {
            var account = GetAccount(new PreAdmitNewbornActivity(), GetTestDateAfterFeatureStart());
            account.KindOfVisit = VisitType.PreRegistration;
            var ruleUnderTest = new EmailAddressPreferred();
            account.IsShortRegistered = false;
            account.Patient.EmailReason.Code = EmailReason.REMOVE;
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_CreatedDateAfterReleaseDate_ShortPreRegistrationActivity_PatientEmailIsPopulated_ShouldReturnTrue()
        {
            var account = GetAccount(new ShortPreRegistrationActivity(), GetTestDateAfterFeatureStart());
            account.Patient.AddContactPoint(
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
            var ruleUnderTest = new EmailAddressPreferred();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
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
