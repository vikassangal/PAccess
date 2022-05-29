using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
    class PrimaryCarePhysicianRequiredTests
    {
        [Test]
        public void TestCanBeAppliedTo_WithInvalidContextType_ShouldReturnTrue()
        {
            var inValidObjectType = new object();
            var ruleUnderTest = new PrimaryCarePhysicianRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo(inValidObjectType);
            Assert.IsTrue(actualResult);
        }

        [Test]
        public void TestCanBeAppliedTo_WithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new PrimaryCarePhysicianRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo(null);
            Assert.IsTrue(actualResult);
        }

        [Test]
        public void TestCanBeAppliedTo_RegistrationWhenAccountCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart());
            var ruleUnderTest = new PrimaryCarePhysicianRequired();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_Registration_WhenAccountCreatedBeforeReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateBeforeFeatureStart());
            var ruleUnderTest = new PrimaryCarePhysicianRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
       
        [Test]
        public void TestCanBeAppliedTo_PreMse_WhenAccountCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            var account = GetAccount(new PreMSERegisterActivity(), GetTestDateAfterFeatureStart());
            var ruleUnderTest = new PrimaryCarePhysicianRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_PreMse_WhenAccountCreatedBeforeReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount(new PreMSERegisterActivity(), GetTestDateBeforeFeatureStart());
            var ruleUnderTest = new PrimaryCarePhysicianRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_EDPostMse_WhenAccountCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            var account = GetAccount(new PostMSERegistrationActivity(), GetTestDateAfterFeatureStart());
            var ruleUnderTest = new PrimaryCarePhysicianRequired();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_EDPostMse_WhenAccountCreatedBeforeReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount(new PostMSERegistrationActivity(), GetTestDateBeforeFeatureStart());
            var ruleUnderTest = new PrimaryCarePhysicianRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_UCPostMse_WhenAccountCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            var account = GetAccount(new UCCPostMseRegistrationActivity(), GetTestDateAfterFeatureStart());
            var ruleUnderTest = new PrimaryCarePhysicianRequired();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_UCPostMse_WhenAccountCreatedBeforeReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount(new UCCPostMseRegistrationActivity(), GetTestDateBeforeFeatureStart());
            var ruleUnderTest = new PrimaryCarePhysicianRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
      
        [Test]
        public void TestCanBeAppliedTo_Admit_WhenAccountCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            var account = GetAccount(new AdmitNewbornActivity(), GetTestDateAfterFeatureStart());
            var ruleUnderTest = new PrimaryCarePhysicianRequired();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_Admit_WhenAccountCreatedBeforeReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount(new AdmitNewbornActivity(), GetTestDateBeforeFeatureStart());
            var ruleUnderTest = new PrimaryCarePhysicianRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_ShortRegistration_WhenAccountCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            var account = GetAccount(new ShortRegistrationActivity(), GetTestDateAfterFeatureStart());
            var ruleUnderTest = new PrimaryCarePhysicianRequired();
            account.IsShortRegistered = true;
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_ShortRegistration_WhenAccountCreatedBeforeReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount(new ShortRegistrationActivity(), GetTestDateBeforeFeatureStart());
            var ruleUnderTest = new PrimaryCarePhysicianRequired();
            account.IsShortRegistered = true;
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
        private static DateTime GetTestDateAfterFeatureStart()
        {
            return new PrimaryCarePhysicianRequiredFeatureManager().FeatureStartDate.AddDays(10);
        }
        private static DateTime GetTestDateBeforeFeatureStart()
        {
            return new PrimaryCarePhysicianRequiredFeatureManager().FeatureStartDate.AddDays(-10);
        }


        #endregion
    }
}
