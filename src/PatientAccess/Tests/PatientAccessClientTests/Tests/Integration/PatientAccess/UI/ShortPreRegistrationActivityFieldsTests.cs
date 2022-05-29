using NUnit.Framework;
using PatientAccess.Actions;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;
using OnShortDemographicsForm = PatientAccess.Actions.OnShortDemographicsForm;

namespace Tests.Integration.PatientAccess.UI
{
    [TestFixture]
    public class ShortPreRegistrationActivityFieldsTests
    {
        [Test]
        public void MaritalStatusIsRequireForRegistrationActivity()
        {
            var account = new Account { Activity = new ShortPreRegistrationActivity() };

            var maritalStatusIsRequiredAndMissing = account.IsRuleEnforcedForCompositeAction<OnShortDemographicsForm>( RuleIds.MaritalStatusRequired );

            Assert.IsTrue( maritalStatusIsRequiredAndMissing );
        }

        [Test]
        public void AdmitTimeIsNotRequired()
        {
            var account = new Account { Activity = new ShortPreRegistrationActivity() };

            var admitTimeIsRequired = account.IsRuleEnforcedForCompositeAction<OnShortDemographicsForm>( RuleIds.AdmitTimeRequired );

            Assert.IsFalse( admitTimeIsRequired );
        }

        [Test]
        public void AdmitTimeIsPreferred()
        {
            var account = new Account { Activity = new ShortPreRegistrationActivity() };

            var admitTimeIsPreferred = account.IsRuleEnforcedForCompositeAction<OnShortDemographicsForm>( RuleIds.AdmitTimePreferred );

            Assert.IsTrue( admitTimeIsPreferred );
        }

        [Test]
        public void PrimaryCarePhysicianRequiredForShortRegistrationActivity_ShouldReturnTrue()
        {
            var account = new Account { Activity = new ShortPreRegistrationActivity() };
            var PrimaryCarePhysicianRequired =
                account.IsRuleEnforcedForCompositeAction<OnShortDiagnosisForm>(RuleIds.PrimaryCarePhysicianRequired);
            Assert.IsTrue(PrimaryCarePhysicianRequired);
        }
    }
}
