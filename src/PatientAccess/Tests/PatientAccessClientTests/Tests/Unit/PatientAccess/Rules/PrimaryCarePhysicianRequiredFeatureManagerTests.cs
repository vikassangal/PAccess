using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PrimaryCarePhysicianRequiredFeatureManagerTests
    /// </summary>
    [TestFixture]
    [Category("Fast")]
    public class PrimaryCarePhysicianRequiredFeatureManagerTests
    {
        [SetUp]
        public void SetUpFeatureManager()
        {
            primaryCarePhysicianRequiredFeatureManager = new PrimaryCarePhysicianRequiredFeatureManager();
        }
        [Test]
        public void TestCanBeAppliedTo_WithNullContext_ShouldReturFalse()
        {

            var actualResult = primaryCarePhysicianRequiredFeatureManager.IsPrimarycarephysicianRequiredfor(null);
            Assert.IsFalse(actualResult);
        }
        [Test]
        public void TestRegistrationPCPRequiredVisible_AccountCreatedBeforeReleaseDate_Inpatient_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateBeforeFeatureStart(), VisitType.Inpatient );
            Assert.IsFalse(primaryCarePhysicianRequiredFeatureManager.IsPrimarycarephysicianRequiredfor(account));
        }

        [Test]
        public void TestRegistrationPCPRequiredVisible_AccountCreatedAfterReleaseDate_Inpatient_ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart() , VisitType.Inpatient);
            Assert.IsTrue(primaryCarePhysicianRequiredFeatureManager.IsPrimarycarephysicianRequiredfor(account));
        }
        [Test]
        public void TestRegistrationPCPRequiredVisible_AccountCreatedAfterReleaseDate_NonPatient_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart() , VisitType.NonPatient) ;
            Assert.IsFalse(primaryCarePhysicianRequiredFeatureManager.IsPrimarycarephysicianRequiredfor(account));
        }
       
        [Test]
        public void TestAdmitNewBornPCPRequiredVisible_AccountCreatedBeforeReleaseDate_Inpatient_ShouldReturnFalse()
        {
            var account = GetAccount(new AdmitNewbornActivity(), GetTestDateBeforeFeatureStart(), VisitType.Inpatient);
            Assert.IsFalse(primaryCarePhysicianRequiredFeatureManager.IsPrimarycarephysicianRequiredfor(account));
        }

        [Test]
        public void TestAdmitNewBornPCPRequiredVisible_AccountCreatedAfterReleaseDate_Inpatient_ShouldReturnTrue()
        {
            var account = GetAccount(new AdmitNewbornActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient );
            Assert.IsTrue(primaryCarePhysicianRequiredFeatureManager.IsPrimarycarephysicianRequiredfor(account));
        }
        [Test]
        public void TestEDPostMsePCPRequiredVisible_AccountCreatedBeforeReleaseDate_EmergencyPatient_ShouldReturnFalse()
        {
            var account = GetAccount(new PostMSERegistrationActivity(), GetTestDateBeforeFeatureStart(), VisitType.Emergency);
            Assert.IsFalse(primaryCarePhysicianRequiredFeatureManager.IsPrimarycarephysicianRequiredfor(account));
        }
        [Test]
        public void TestEDPostMsePCPRequiredVisible_AccountCreatedAfterReleaseDate_EmergencyPatient_ShouldReturnTrue()
        {
            var account = GetAccount(new PostMSERegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Emergency );
            Assert.IsTrue(primaryCarePhysicianRequiredFeatureManager.IsPrimarycarephysicianRequiredfor(account));
        }
        [Test]
        public void TestUCPostMsePCPRequiredVisible_AccountCreatedAfterReleaseDate_OutPatient_ShouldReturnTrue()
        {
            var account = GetAccount(new UCCPostMseRegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Outpatient);
            Assert.IsTrue(primaryCarePhysicianRequiredFeatureManager.IsPrimarycarephysicianRequiredfor(account));
        }
        [Test]
        public void TestUCPostMsePCPRequiredVisible_AccountCreatedBeforeReleaseDate__OutPatient_ShouldReturnFalse()
        {
            var account = GetAccount(new UCCPostMseRegistrationActivity(), GetTestDateBeforeFeatureStart(), VisitType.Outpatient) ;
            Assert.IsFalse(primaryCarePhysicianRequiredFeatureManager.IsPrimarycarephysicianRequiredfor(account));
        }
        [Test]
        public void TestUCPreMsePCPRequiredVisible_AccountCreatedBeforeReleaseDate_OutPatient_ShouldReturnFalse()
        {
            var account = GetAccount(new UCCPreMSERegistrationActivity(), GetTestDateBeforeFeatureStart(), VisitType.Outpatient);
            Assert.IsFalse(primaryCarePhysicianRequiredFeatureManager.IsPrimarycarephysicianRequiredfor(account));
        }
        [Test]
        public void TestUCPreMsePCPRequiredVisible_AccountCreatedAfterReleaseDate_OutPatient_ShouldReturnTrue()
        {
            var account = GetAccount(new UCCPreMSERegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Outpatient );
            Assert.IsTrue(primaryCarePhysicianRequiredFeatureManager.IsPrimarycarephysicianRequiredfor(account));
        }
        [Test]
        public void TestPreMsePCPRequiredVisible_AccountCreatedBeforeReleaseDate_EmergencyPatient_ShouldReturnFalse()
        {
            var account = GetAccount(new PreMSERegisterActivity(), GetTestDateBeforeFeatureStart(), VisitType.Emergency);
            Assert.IsFalse(primaryCarePhysicianRequiredFeatureManager.IsPrimarycarephysicianRequiredfor(account));
        }
        [Test]
        public void TestPreMsePCPRequiredVisible_AccountCreatedAfterReleaseDate_EmergencyPatient_ShouldReturnTrue()
        {
            var account = GetAccount(new PreMSERegisterActivity(), GetTestDateAfterFeatureStart(), VisitType.Emergency);
            Assert.IsTrue(primaryCarePhysicianRequiredFeatureManager.IsPrimarycarephysicianRequiredfor(account));
        }
        #region Support Methods
        private static Account GetAccount(Activity activity, DateTime accountCreatedDate, VisitType patientType)
        {
            return new Account
            {
                Activity = activity,
                AccountCreatedDate = accountCreatedDate,
                KindOfVisit = patientType
            };
        }
        private DateTime GetTestDateAfterFeatureStart()
        {
            return primaryCarePhysicianRequiredFeatureManager.FeatureStartDate.AddDays(10);
        }
        private DateTime GetTestDateBeforeFeatureStart()
        {
            return primaryCarePhysicianRequiredFeatureManager.FeatureStartDate.AddDays(-10);
        }
        #endregion

        #region constants

        private PrimaryCarePhysicianRequiredFeatureManager primaryCarePhysicianRequiredFeatureManager;

        #endregion

    }
}
