using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.AddressViews.Presenters;
using PatientAccess.UI.AddressViews.Views;
using Rhino.Mocks;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
    public class CellPhoneConsentFeatureManagerTests
    {
        [Test]
        public void Test_CellPhoneConsentFeatureManager_Registration_AdmitDateBeforeFeatureStartDate_ShouldReturnFalse()
        {
            var account = new Account();
            account.AdmitDate = GetAccountCreatedDateBeforeFeatureStart();
            var feature = new GuarantorCellPhoneConsentFeatureManager();
            var result = feature.IsCellPhoneConsentRuleForCOSEnabledforaccount(account);
            Assert.IsFalse(result);
        }
        [Test]
        public void Test_CellPhoneConsentFeatureManager_Registration_AdmitDateAfterFeatureStartDate_ShouldReturnTrue()
        {
            var account = new Account();
            account.AdmitDate = GetAccountCreatedDateAfterFeatureStart();
            var feature = new GuarantorCellPhoneConsentFeatureManager();
            var result = feature.IsCellPhoneConsentRuleForCOSEnabledforaccount(account);
            Assert.IsTrue(result);
        }

        [Test]
        public void Test_CellPhoneConsentFeatureManager_Registration_AdmitDateMinValueFeatureStartDate_ShouldReturnTrue()
        {
            var account = new Account();
            account.AdmitDate = DateTime.MinValue;
            var feature = new GuarantorCellPhoneConsentFeatureManager();
            var result = feature.IsCellPhoneConsentRuleForCOSEnabledforaccount(account);
            Assert.IsTrue(result);
        }
        #region Support Methods


        private DateTime GetAccountCreatedDateAfterFeatureStart()
        {
            return new GuarantorCellPhoneConsentFeatureManager().FeatureStartDate.AddDays(10);
        }

        private DateTime GetAccountCreatedDateBeforeFeatureStart()
        {
            return new GuarantorCellPhoneConsentFeatureManager().FeatureStartDate.AddDays(-10);
        }

        #endregion
    }


}
