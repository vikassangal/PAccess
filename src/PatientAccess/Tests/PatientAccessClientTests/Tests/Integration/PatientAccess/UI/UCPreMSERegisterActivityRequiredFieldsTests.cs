using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.UCCRegistration;
using OnContactAndDiagnosisForm = PatientAccess.Actions.OnContactAndDiagnosisForm;

namespace Tests.Integration.PatientAccess.UI
{
    [TestFixture]
    public class UCPreMSERegisterActivityRequiredFieldsTests
    {
       
        [Test]
        public void PrimaryCarePhysicianRequiredForUCPreMSERegisterActivity_ShouldReturnTrue()
        {
            var account = new Account { Activity = new UCCPreMSERegistrationActivity() };
            var PrimaryCarePhysicianRequired =
                account.IsRuleEnforcedForCompositeAction<OnContactAndDiagnosisForm>(RuleIds.PrimaryCarePhysicianRequired);
            Assert.IsTrue(PrimaryCarePhysicianRequired);
        }
    }
}
