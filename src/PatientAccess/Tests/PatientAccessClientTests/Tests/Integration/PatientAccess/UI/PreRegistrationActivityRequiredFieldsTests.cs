using NUnit.Framework;
using PatientAccess.Actions;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.UI
{
    [TestFixture]
    public class PreRegistrationActivityRequiredFieldsTests
    {
        [Test]
        public void MaritalStatusIsRequireForRegistrationActivity()
        {
            var account = new Account { Activity = new PreRegistrationActivity() };

            var maritalStatusRequiredIsEnforced = account.IsRuleEnforcedForCompositeAction<OnPatientDemographics>( RuleIds.MaritalStatusRequired );

            Assert.IsTrue( maritalStatusRequiredIsEnforced );
        }

        [Test]
        public void PrimaryCarePhysicianRequiredForPreRegistrationActivity_ShouldReturnFalse()
        {
            var account = new Account { Activity = new PreRegistrationActivity() };
            var PrimaryCarePhysicianRequired =
                account.IsRuleEnforcedForCompositeAction<OnClinicalForm>(RuleIds.PrimaryCarePhysicianRequired);
            Assert.IsFalse(PrimaryCarePhysicianRequired);
        }

        [Test]
        public void COBReceivedNotRequiredForPreRegistrationActivity_ShouldReturnFalse()
        {
            var account = new Account { Activity = new PreRegistrationActivity() };
            account.KindOfVisit = VisitType.PreRegistration;
            var financialClass = new FinancialClass { Code = "14" };
            account.FinancialClass = financialClass;
            var COBReceivedRequired =
                account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>(RuleIds.COBReceivedRequired);
            Assert.IsFalse(COBReceivedRequired);
        }

        [Test]
        public void TestIsRuleEnforcedForCompositeAction_IMFMReceivedRequired_ForPreRegistrationActivity_WhenHSVIsNot35_ShouldReturnFalse()
        {
            var account = new Account { Activity = new PreRegistrationActivity() };
            account.KindOfVisit = VisitType.PreRegistration;
            var financialClass = new FinancialClass { Code = "26" };
            account.FinancialClass = financialClass;
            account.HospitalService.Code = HospitalService.PRE_REGISTER;
            var IMFMReceivedRequired =
                account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>(RuleIds.IMFMReceivedRequired);
            Assert.IsFalse(IMFMReceivedRequired);
        }

        [Test]
        public void TestIsRuleEnforcedForCompositeAction_IMFMReceivedRequired_ForPreRegistrationActivity_WhenHSVIs35_ShouldReturnTrue()
        {
            var account = new Account { Activity = new PreRegistrationActivity() };
            account.KindOfVisit = VisitType.PreRegistration;
            var financialClass = new FinancialClass { Code = "26" };
            account.FinancialClass = financialClass;
            account.HospitalService.Code = HospitalService.PRE_ADMIT;
            var IMFMReceivedRequired =
                account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>(RuleIds.IMFMReceivedRequired);
            Assert.IsTrue(IMFMReceivedRequired);
        }
    }
}
