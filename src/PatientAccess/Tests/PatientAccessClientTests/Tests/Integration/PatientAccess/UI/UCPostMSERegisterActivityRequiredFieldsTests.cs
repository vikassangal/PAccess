using NUnit.Framework;
using PatientAccess.Actions;
using PatientAccess.Domain;
using PatientAccess.Domain.UCCRegistration;

namespace Tests.Integration.PatientAccess.UI
{
    [TestFixture]
    public class UCPostMSERegisterActivityRequiredFieldsTests
    {
        [Test]
        public void PrimaryCarePhysicianRequiredForUCPostMSERegistrationActivity_ShouldReturnTrue()
        {
            var account = new Account {Activity = new UCCPostMseRegistrationActivity()};
            var PrimaryCarePhysicianRequired =
                account.IsRuleEnforcedForCompositeAction<OnClinicalForm>(RuleIds.PrimaryCarePhysicianRequired);
            Assert.IsTrue(PrimaryCarePhysicianRequired);
        }
       
        [Test]
        public void COBReceivedRequiredForUCPostMSERegistrationActivity_ShouldReturnTrue()
        {
            var account = new Account { Activity = new UCCPostMseRegistrationActivity() };

            account.KindOfVisit = VisitType.Emergency;
            var financialClass = new FinancialClass { Code = "81" };
            account.FinancialClass = financialClass;
            var COBReceivedRequired = account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>(RuleIds.COBReceivedRequired);

            Assert.IsTrue(COBReceivedRequired);
        }

        [Test]
        public void IMFMReceivedNotRequiredForUCPostMSERegistrationActivity_ShouldReturnFalse()
        {
            var account = new Account { Activity = new UCCPostMseRegistrationActivity() };
            account.KindOfVisit = VisitType.Emergency;
            var financialClass = new FinancialClass { Code = "58" };
            account.FinancialClass = financialClass;
            var IMFMReceivedRequired = account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>(RuleIds.IMFMReceivedRequired);

            Assert.IsFalse(IMFMReceivedRequired);
        }
    }
}
