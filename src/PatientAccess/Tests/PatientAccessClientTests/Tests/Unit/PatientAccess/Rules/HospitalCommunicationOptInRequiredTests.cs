using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    public class HospitalCommunicationOptInRequiredTests
    {
        #region Defualt Scenarios

        [Test]
        public void TestCanBeAppliedTo_WithInvalidContextType_ShouldReturnTrue()
        {
            var inValidObjectType = new object();
            var ruleUnderTest = new HospitalCommunicationOptInRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo(inValidObjectType);

            Assert.IsTrue(actualResult);
        }

        [Test]
        public void TestCanBeAppliedTo_WithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new HospitalCommunicationOptInRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo(null);

            Assert.IsTrue(actualResult);
        }

        #endregion Defualt Scenarios

        #region PatientType1

        [Test]
        public void
            TestCanBeAppliedTo_WhenActivityIsRegistration_AndPatientType1_AndCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient,
                true);
            var ruleUnderTest = new HospitalCommunicationOptInRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        #endregion PatientType1

        #region PatientType0

        [Test]
        public void
            TestCanBeAppliedTo_WhenActivityIsPreRegistration_AndPatientType0_AndCreatedDateAfterReleaseDate_ShouldReturnTrue
            ()
        {
            var account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterFeatureStart(),
                VisitType.PreRegistration, false);
            var ruleUnderTest = new HospitalCommunicationOptInRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        #endregion PatientType0

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AndCreatedDateBeforeReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateBeforeFeatureStart(), VisitType.Inpatient, true);
            var ruleUnderTest = new HospitalCommunicationOptInRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistration_AndCreatedDateBeforeReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount(new PreRegistrationActivity(), GetTestDateBeforeFeatureStart(), VisitType.PreRegistration, false);
            var ruleUnderTest = new HospitalCommunicationOptInRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        #region Support Methods

        private static Account GetAccount(Activity activity, DateTime accountCreatedDate,
            VisitType kindofVisit, bool featureEnabledForFacility)
        {
            return new Account
            {
                Activity = activity,
                AccountCreatedDate = accountCreatedDate,
                KindOfVisit = kindofVisit,

            };
        }

        #endregion


        #region Constants

        private static DateTime GetTestDateAfterFeatureStart()
        {
            return new HospitalCommunicationOptInFeatureManager().FeatureStartDate.AddDays(10);
        }

        private static DateTime GetTestDateBeforeFeatureStart()
        {
            return new HospitalCommunicationOptInFeatureManager().FeatureStartDate.AddDays(-10);
        }

        #endregion

    }
}
