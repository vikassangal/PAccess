using System;  
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for HIEConsentFeatureManagerTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class HIEConsentFeatureManagerTests
    {
        
        [Test]
        public void ShouldWeEnableHIEConsentFlags_WhenActivity_Registrattion_AccountCreatedBeforeFeature_ShouldReturnFalse()
        {
            var hieConsentFeatureManager = new HIEConsentFeatureManager();
            var Account = GetAccount(new RegistrationActivity(), GetTestDateBeforeFeatureStart, VisitType.Outpatient  );
            bool hieConsentEnabled = hieConsentFeatureManager.IsHIEConsentFeatureManagerEnabled(Account );
            Assert.IsFalse( hieConsentEnabled );
        }

        [Test]
        public void ShouldWeEnableHIEConsentFlags_WhenActivityIsPreRegistration_WhenAccountCreatedDateIsBeforeTheFeatuteStartDate_ShouldReturnFalse()
        {
            var hieConsentFeatureManager = new HIEConsentFeatureManager();
            var Account = GetAccount(new PreRegistrationActivity(), GetTestDateBeforeFeatureStart , VisitType.PreRegistration );
            bool hieConsentEnabled = hieConsentFeatureManager.IsHIEConsentFeatureManagerEnabled(Account);
            Assert.IsFalse(hieConsentEnabled);
            
        }
        [Test]
        public void ShouldWeEnableHIEConsentFlags_WhenActivityIsPreMSE_WhenAccountCreatedDateIsBeforeTheFeatuteStartDate_ShouldReturnFalse()
        {
            var hieConsentFeatureManager = new HIEConsentFeatureManager();
            var Account = GetAccount(new PreMSERegisterActivity(), GetTestDateBeforeFeatureStart , VisitType.Emergency );
            bool hieConsentEnabled = hieConsentFeatureManager.IsHIEConsentFeatureManagerEnabled(Account);
            Assert.IsFalse(hieConsentEnabled);

        }
        [Test]
        public void ShouldWeEnableHIEConsentFlags_WhenActivity_Registrattion_AccountCreatedAfterFeature_ShouldReturnTrue()
        {
            var hieConsentFeatureManager = new HIEConsentFeatureManager();
            var Account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart, VisitType.Outpatient);
            bool hieConsentEnabled = hieConsentFeatureManager.IsHIEConsentFeatureManagerEnabled(Account);
            Assert.IsTrue(hieConsentEnabled);
        }

        [Test]
        public void ShouldWeEnableHIEConsentFlags_WhenActivityIsPreRegistration_WhenAccountCreatedDateIsAfterTheFeatuteStartDate_ShouldReturnFalse()
        {
            var hieConsentFeatureManager = new HIEConsentFeatureManager();
            var Account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterFeatureStart , VisitType.PreRegistration );
            bool hieConsentEnabled = hieConsentFeatureManager.IsHIEConsentFeatureManagerEnabled(Account);
            Assert.IsFalse(hieConsentEnabled);

        }
        [Test]
        public void ShouldWeEnableHIEConsentFlags_WhenActivityIsPreMSE_WhenAccountCreatedDateIsAfterTheFeatuteStartDate_ShouldReturnFalse()
        {
            var hieConsentFeatureManager = new HIEConsentFeatureManager();
            var Account = GetAccount(new PreMSERegisterActivity(), GetTestDateBeforeFeatureStart , VisitType.Emergency );
            bool hieConsentEnabled = hieConsentFeatureManager.IsHIEConsentFeatureManagerEnabled(Account);
            Assert.IsFalse(hieConsentEnabled);

        }
       
       private static Account GetAccount( Activity activity, DateTime accountCreatedDate, VisitType visitType )
        {
            return new Account
                {
                    Activity = activity,
                    AccountCreatedDate = accountCreatedDate,
                    KindOfVisit = visitType
                };
        }
        private static DateTime GetTestDateAfterFeatureStart
        {
            get { return new HIEConsentFeatureManager().FeatureStartDate.AddDays(10); }
        }
        private static DateTime GetTestDateBeforeFeatureStart
        {
            get { return new HIEConsentFeatureManager().FeatureStartDate.AddDays(-10); }
        }
        #region Constants
        #endregion
    }
}