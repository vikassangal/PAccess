using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ShareHIEDataFeatureManagerTests
    /// </summary>
    [TestFixture]
    [Category("Fast")]
    public class ShareHIEDataFeatureManagerTests
    {
        [Test]
        public void ShouldWeEnableShareHIEDataFlags_WhenActivity_Registration_AccountCreatedBeforeFeatureStartDate_ShouldReturnTrue()
        {
            var shareHIEDataFeatureManager = new ShareHIEDataFeatureManager();
            var account = GetAccount(new RegistrationActivity(), GetTestDateBeforeFeatureStart, VisitType.Inpatient, GetAdmitDateBeforeFeatureStart());
            bool shareHIEDataEnabled = shareHIEDataFeatureManager.IsShareHieDataEnabledforaccount(account);
            Assert.IsFalse(shareHIEDataEnabled);
        }
        [Test]
        public void ShouldWeEnableShareHIEDataFlags_WhenActivity_Registration_AccountCreatedAfterFeatureStartDate_ShouldReturnTrue()
        {
            var shareHIEDataFeatureManager = new ShareHIEDataFeatureManager();
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart, VisitType.Inpatient, GetAdmitDateAfterFeatureStart());
            bool shareHIEDataEnabled = shareHIEDataFeatureManager.IsShareHieDataEnabledforaccount(account);
            Assert.IsTrue(shareHIEDataEnabled);
        }
        [Test]
        public void ShouldWeEnableShareHIEDataFlags_ActivatePreRegistration_AccountActivatedAfterFeatureStartDate_ShouldReturnTrue()
        {
            var shareHIEDataFeatureManager = new ShareHIEDataFeatureManager();
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart, VisitType.Inpatient, GetAdmitDateAfterFeatureStart());
            account.Activity.AssociatedActivityType = typeof(ActivatePreRegistrationActivity);
            bool shareHIEDataEnabled = shareHIEDataFeatureManager.IsShareHieDataEnabledforaccount(account);
            Assert.IsTrue(shareHIEDataEnabled);
        }
        [Test]
        public void ShouldWeEnableShareHIEDataFlags_WhenActivityIsPreMSE_WhenAccountCreatedDateIsBeforeTheFeatuteStartDate_ShouldReturnFalse()
        {
            var shareHIEDataFeatureManager = new ShareHIEDataFeatureManager();
            var account = GetAccount(new PreMSERegisterActivity(), GetTestDateBeforeFeatureStart, VisitType.Emergency, GetAdmitDateBeforeFeatureStart());
            bool shareHIEDataEnabled = shareHIEDataFeatureManager.IsShareHieDataEnabledforaccount(account);
            Assert.IsFalse(shareHIEDataEnabled);
         }
        [Test]
        public void ShouldWeEnableShareHIEDataFlags_ActivatePreAdmitNewBorn_AccountActivatedAfterFeatureStartDate_ShouldReturnFalse()
        {
            var activity = new PreAdmitNewbornActivity { AssociatedActivityType = typeof(ActivatePreRegistrationActivity) };
            if (activity.IsPreAdmitNewbornActivity())
            {
                var shareHIEDataFeatureManager = new ShareHIEDataFeatureManager();
                var account = GetAccount(activity, GetTestDateAfterFeatureStart, VisitType.Inpatient, GetAdmitDateAfterFeatureStart());
                bool shareHIEDataEnabled = shareHIEDataFeatureManager.IsShareHieDataEnabledforaccount(account);
                Assert.IsFalse(shareHIEDataEnabled);
            }
        }
        [Test]
        public void ShouldWeEnableShareHIEDataFlags_WhenActivityIsPostMSE_WhenAccountCreatedDateIsBeforeTheFeatuteStartDate_ShouldReturnFalse()
        {
            var shareHIEDataFeatureManager = new ShareHIEDataFeatureManager();
            var account = GetAccount(new PostMSERegistrationActivity(), GetTestDateBeforeFeatureStart, VisitType.Emergency, GetAdmitDateBeforeFeatureStart());
            bool shareHIEDataEnabled = shareHIEDataFeatureManager.IsShareHieDataEnabledforaccount(account);
            Assert.IsFalse(shareHIEDataEnabled);
        }
        [Test]
        public void ShouldWeEnableShareHIEDataFlags_Post_MSE_AccountActivatedBeforeFeatureStartDate_ShouldReturnFalse()
        {
            var shareHIEDataFeatureManager = new ShareHIEDataFeatureManager();
            var account = GetAccount(new PostMSERegistrationActivity(), GetTestDateBeforeFeatureStart, VisitType.Inpatient, GetAdmitDateBeforeFeatureStart());
            bool shareHIEDataEnabled = shareHIEDataFeatureManager.IsShareHieDataEnabledforaccount(account);
            Assert.IsFalse(shareHIEDataEnabled);
        }
        [Test]
        public void ShouldWeEnableShareHIEDataFlags_UC_Post_MSE_AccountActivatedBeforeFeatureStartDate_ShouldReturnFalse()
        {
            var shareHIEDataFeatureManager = new ShareHIEDataFeatureManager();
            var account = GetAccount(new UCCPostMseRegistrationActivity(), GetTestDateBeforeFeatureStart, VisitType.Inpatient, GetAdmitDateBeforeFeatureStart());
            bool shareHIEDataEnabled = shareHIEDataFeatureManager.IsShareHieDataEnabledforaccount(account);
            Assert.IsFalse(shareHIEDataEnabled);
        }
        [Test]
        public void ShouldWeEnableShareHIEDataFlags_UC_Post_MSE_AccountActivatedAfterFeatureStartDate_ShouldReturnTrue()
        {
            var shareHIEDataFeatureManager = new ShareHIEDataFeatureManager();
            var account = GetAccount(new UCCPostMseRegistrationActivity(), GetTestDateAfterFeatureStart, VisitType.Inpatient, GetAdmitDateAfterFeatureStart());
            bool shareHIEDataEnabled = shareHIEDataFeatureManager.IsShareHieDataEnabledforaccount(account);
            Assert.IsTrue(shareHIEDataEnabled);
        }
        private static Account GetAccount(Activity activity, DateTime accountCreatedDate, VisitType visitType, DateTime admitDate)
        {
            return new Account
            {
                Activity = activity,
                AccountCreatedDate = accountCreatedDate,
                KindOfVisit = visitType,
                AdmitDate = admitDate
                
                
            };
        }
        private static DateTime GetTestDateAfterFeatureStart
        {
            get { return new ShareHIEDataFeatureManager().FeatureStartDate.AddDays(10); }
        }
        private static DateTime GetTestDateBeforeFeatureStart
        {
            get { return new ShareHIEDataFeatureManager().FeatureStartDate.AddDays(-10); }
        }
        private static DateTime GetAdmitDateAfterFeatureStart()
        {
            return new ShareHIEDataFeatureManager().FeatureStartDate.AddDays(10);
        }

        private static DateTime GetAdmitDateBeforeFeatureStart()
        {
            return new ShareHIEDataFeatureManager().FeatureStartDate.AddDays(-10);
        }
    }
}
