using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
    public class HospitalCommunicationFeatureManagerTests
    {
        [SetUp]
        public void SetUpFeatureManager()
        {
            hospitalCommunicationOptInFeatureManager = new HospitalCommunicationOptInFeatureManager();
        }
        #region Defualt Scenarios
        [Test]
        public void TestCanBeAppliedTo_WithNullContext_ShouldReturnFalse()
        {

            var actualResult = hospitalCommunicationOptInFeatureManager.ShouldFeatureBeEnabled(null);
            Assert.IsFalse(actualResult);
        }
        [Test]
        public void TestPatientHospitalCommunicationOptInFeatureVisible_CreatedBeforeReleaseDate_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateBeforeFeatureStart(), VisitType.Inpatient);
            Assert.IsFalse(hospitalCommunicationOptInFeatureManager.ShouldFeatureBeEnabled(account));
        }
        [Test]
        public void TestPatientHospitalCommunicationOptInFeatureVisible_CreatedAfterReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient);
            Assert.IsTrue(hospitalCommunicationOptInFeatureManager.ShouldFeatureBeEnabled(account));
        }
        #endregion Defualt Scenarios
        #region Support Methods

        private static Account GetAccount(Activity activity, DateTime accountCreatedDate, VisitType kindofVisit)
        {
            var facility = new Facility(PersistentModel.NEW_OID,
                                       PersistentModel.NEW_VERSION,
                                       "DOCTORS HOSPITAL DALLAS",
                                       "DHF");
            return new Account
             {
                 Facility = facility,
                 Activity = activity,
                 AccountCreatedDate = accountCreatedDate,
                 KindOfVisit = kindofVisit,
             };
        }
        private DateTime GetTestDateAfterFeatureStart()
        {
            return hospitalCommunicationOptInFeatureManager.FeatureStartDate.AddDays(10);
        }
        private DateTime GetTestDateBeforeFeatureStart()
        {
            return hospitalCommunicationOptInFeatureManager.FeatureStartDate.AddDays(-10);
        }
        #endregion
        #region Constants

        private HospitalCommunicationOptInFeatureManager hospitalCommunicationOptInFeatureManager;

        #endregion
    }

}
