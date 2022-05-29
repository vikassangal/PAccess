using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
    public class EmailReasonRequiredTests
    {

        [Test]
        public void TestCanBeAppliedTo_WithInvalidContextType_ShouldReturnTrue()
        {
            var inValidObjectType = new object();
            var ruleUnderTest = new EmailReasonRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo(inValidObjectType);
            Assert.IsTrue(actualResult);
        }

        [Test]
        public void TestCanBeAppliedTo_WithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new EmailReasonRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo(null);
            Assert.IsTrue(actualResult);
        }

        [Test]
        public void TestCanBeAppliedTo_RegistrationActivity_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), true);
            var ruleUnderTest = new EmailReasonRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenConditionOfServiceIsRefused_ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), true);
            var ruleUnderTest = new EmailReasonRequired();
            var condtionOfService = new ConditionOfService()
            {
                Code = ConditionOfService.REFUSED,
                Description = ConditionOfService.REFUSED_DESCRIPTION
            };
            account.COSSigned = condtionOfService;
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_WhenConditionOfSServiceIsMedicallyUnableToSign_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), true);
            var ruleUnderTest = new EmailReasonRequired();
            var condtionOfService = new ConditionOfService()
            {
                Code = ConditionOfService.UNABLE,
                Description = ConditionOfService.UNABLE_DESCRIPTION
            };
            account.COSSigned = condtionOfService;
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_PreRegistrationActivity_ShouldReturnFalse()
        {
            var account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterFeatureStart(), true);
            var ruleUnderTest = new EmailReasonRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_DuringPOSTMSEActivityAfterFeatureStartDate_ShouldReturnTrue()
        {
            var account = GetAccount(new PostMSERegistrationActivity(), GetTestDateAfterFeatureStart(), true);
            var ruleUnderTest = new EmailReasonRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_DuringShortRegistrationActivityAfterFeatureStartDate_ShouldReturnTrue()
        {
            var account = GetAccount(new ShortRegistrationActivity(), GetTestDateAfterFeatureStart(), true);
            var ruleUnderTest = new EmailReasonRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        
        #region Support Methods

        private static Account GetAccount(Activity activity, DateTime accountCreatedDate, bool featureEnabledForFacility)
        {
            var facility = new Facility(PersistentModel.NEW_OID,
                                       PersistentModel.NEW_VERSION,
                                       "DOCTORS HOSPITAL DALLAS",
                                       "DHF");
            if (featureEnabledForFacility)
            {
                facility["IsPatientPortalOptInEnabled"] = true;
            }
            return new Account
            {
                Facility = facility,
                Activity = activity,
                AccountCreatedDate = accountCreatedDate,
            };

        }

        #endregion

        private static DateTime GetTestDateAfterFeatureStart()
        {
            return new EmailReasonFeatureManager().FeatureStartDate.AddDays(10);
        }


    }
}
