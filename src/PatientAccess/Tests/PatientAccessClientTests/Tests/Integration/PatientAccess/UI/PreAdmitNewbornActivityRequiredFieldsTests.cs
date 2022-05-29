using NUnit.Framework;
using PatientAccess.Actions;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.UI
{
    [TestFixture]
    public class PreAdmitNewbornActivityRequiredFieldsTests
    {
        [Test]
        public void MaritalStatusIsRequired()
        {
            var account = new Account { Activity = new PreAdmitNewbornActivity() };

            var maritalStatusRequiredIsEnforced = account.IsRuleEnforcedForCompositeAction<OnPatientDemographics>( RuleIds.MaritalStatusRequired );

            Assert.IsTrue( maritalStatusRequiredIsEnforced );
        }

        [Test]
        public void PrimaryCarePhysicianRequiredForPreAdmitNewbornActivity_ShouldReturnFalse()
        {
            var account = new Account { Activity = new PreAdmitNewbornActivity() };
            var PrimaryCarePhysicianRequired =
                account.IsRuleEnforcedForCompositeAction<OnClinicalForm>(RuleIds.PrimaryCarePhysicianRequired);
            Assert.IsFalse(PrimaryCarePhysicianRequired);
        }

        [Test]
        public void COBReceivedNotRequiredForPreAdmitNewbornActivity_ShouldReturnFalse()
        {
            var account = new Account { Activity = new PreAdmitNewbornActivity() };
            account.KindOfVisit = VisitType.PreRegistration;
            var financialClass = new FinancialClass { Code = "80" };
            account.FinancialClass = financialClass;

            var COBReceivedRequired = account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>(RuleIds.COBReceivedRequired);

            Assert.IsFalse(COBReceivedRequired);
        }

        [Test]
        public void TestIsRuleEnforcedForCompositeAction_IMFMReceivedRequired_ForPreAdmitNewbornActivity_WhenHSVIsNot35_ShouldReturnFalse()
        {
            var account = new Account { Activity = new PreAdmitNewbornActivity() };
            account.KindOfVisit = VisitType.PreRegistration;
            var financialClass = new FinancialClass { Code = "40" };
            account.FinancialClass = financialClass;
            account.HospitalService.Code = HospitalService.PRE_REGISTER;
            var IMFMReceivedRequired = account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>(RuleIds.IMFMReceivedRequired);

            Assert.IsFalse(IMFMReceivedRequired);
        }

        [Test]
        public void TestIsRuleEnforcedForCompositeAction_IMFMReceivedRequired_ForPreAdmitNewbornActivity_WhenHSVIs35_ShouldReturnTrue()
        {
            var account = new Account { Activity = new PreAdmitNewbornActivity() };
            account.KindOfVisit = VisitType.PreRegistration;
            var financialClass = new FinancialClass { Code = "40" };
            account.FinancialClass = financialClass;
            account.HospitalService.Code = HospitalService.PRE_ADMIT;
            var IMFMReceivedRequired = account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>(RuleIds.IMFMReceivedRequired);

            Assert.IsTrue(IMFMReceivedRequired);
        }
    }
}
