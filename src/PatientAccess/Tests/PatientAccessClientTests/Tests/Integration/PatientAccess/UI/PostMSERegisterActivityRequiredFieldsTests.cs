using NUnit.Framework;
using PatientAccess.Actions;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.UI
{
    [TestFixture]
    public class PostMSERegisterActivityRequiredFieldsTests
    {
        [Test]
        public void MaritalStatusIsRequireForRegistrationActivity()
        {
            var account = new Account { Activity = new PostMSERegistrationActivity() };

            var maritalStatusRequiredIsEnforced = account.IsRuleEnforcedForCompositeAction<OnPatientDemographics>( RuleIds.MaritalStatusRequired );

            Assert.IsTrue( maritalStatusRequiredIsEnforced );
        }

        [Test]
        public void Test_ForPostMSERegistrationActivity_EmployerPhoneSubscriberFieldIsOptional()
        {
            var account = new Account {Activity = new PostMSERegistrationActivity()};
            var employerPhoneSubscriberRequiredIsEnforced =
                account.IsRuleEnforcedForCompositeAction<OnEmploymentForm>(RuleIds.EmployerPhoneSubscriberRequired);
            Assert.IsFalse(employerPhoneSubscriberRequiredIsEnforced);
        }

        [Test]
        public void PrimaryCarePhysicianRequiredForPostMSERegistrationActivity_ShouldReturnTrue()
        {
            var account = new Account {Activity = new PostMSERegistrationActivity()};
            var PrimaryCarePhysicianRequired =
                account.IsRuleEnforcedForCompositeAction<OnClinicalForm>(RuleIds.PrimaryCarePhysicianRequired);
            Assert.IsTrue(PrimaryCarePhysicianRequired);
        }

        
        [Test]
        public void COBReceivedRequiredForPostMSERegistrationActivity_ShouldReturnTrue()
        {
            var account = new Account { Activity = new PostMSERegistrationActivity() };
            account.KindOfVisit = VisitType.Emergency;
            var financialClass = new FinancialClass { Code = "80" };
            account.FinancialClass = financialClass;
            var COBReceivedRequired = account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>(RuleIds.COBReceivedRequired);

            Assert.IsTrue(COBReceivedRequired);
        }

    }
}
