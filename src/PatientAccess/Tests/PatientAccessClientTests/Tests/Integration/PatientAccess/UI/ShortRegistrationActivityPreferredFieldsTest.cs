using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;
using OnShortDemographicsForm = PatientAccess.Actions.OnShortDemographicsForm;

namespace Tests.Integration.PatientAccess.UI
{
    [TestFixture]
    public class ShortRegistrationActivityPreferredFieldsTest
    {
        [Test]
        public void PatientMailingPhoneNumberIsPreferred()
        {
            var account = new Account { Activity = new ShortRegistrationActivity() };

            var patientMailingPhoneNumberIsPreferred = account.IsRuleEnforcedForCompositeAction<OnShortDemographicsForm>(RuleIds.MailingAddressPhonePreferred);

            Assert.IsTrue(patientMailingPhoneNumberIsPreferred);
        }

    }
}
