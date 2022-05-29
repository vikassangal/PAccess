using NUnit.Framework;
using PatientAccess.Actions;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;
using OnShortDemographicsForm = PatientAccess.Actions.OnShortDemographicsForm;
using OnShortGuarantorForm = PatientAccess.Actions.OnShortGuarantorForm;

namespace Tests.Integration.PatientAccess.UI
{
    [TestFixture]
    public class ShortRegistrationActivityRequiredFieldsTests
    {
        [Test]
        public void MaritalStatusIsRequireForRegistrationActivity()
        {
            var account = new Account { Activity = new ShortRegistrationActivity() };

            var maritalStatusIsRequiredAndMissing = account.IsRuleEnforcedForCompositeAction<OnShortDemographicsForm>( RuleIds.MaritalStatusRequired );

            Assert.IsTrue( maritalStatusIsRequiredAndMissing );
        }

        [Test]
        public void PatientMailingPhoneNumberIsNotRequired()
        {
            var account = new Account { Activity = new ShortRegistrationActivity() };

            var patientMailingPhoneNumberIsRequired = account.IsRuleEnforcedForCompositeAction<OnShortDemographicsForm>(RuleIds.MailingAddressPhoneRequired);

            Assert.IsFalse(patientMailingPhoneNumberIsRequired);
        }

        [Test]
        public void PatientMailingAreaCodeIsNotRequired()
        {
            var account = new Account { Activity = new ShortRegistrationActivity() };

            var patientMailingAreaCodeIsRequired = account.IsRuleEnforcedForCompositeAction<OnShortDemographicsForm>(RuleIds.MailingAddressAreaCodeRequired);

            Assert.IsFalse(patientMailingAreaCodeIsRequired);
        }

        [Test]
        public void TestEmailAddressRequired_ForShortRegistrationActivity_OnDemographicsForm_ShouldReturnTrue()
        {
            var account = new Account {Activity = new ShortRegistrationActivity()};
            var EmailAddressRequired = account.IsRuleEnforcedForCompositeAction<OnShortDemographicsForm>(RuleIds.EmailAddressRequired);
            Assert.IsFalse(EmailAddressRequired);
        }

        [Test]
        public void TestEmailAddressRequired_ForShortRegistrationActivity_OnGuarantorsForm_ShouldReturnFalse()
        {
            var account = new Account { Activity = new ShortRegistrationActivity() };
            var GuarantorEmailAddressRequired = account.IsRuleEnforcedForCompositeAction<OnShortGuarantorForm>(RuleIds.GuarantorEmailAddressRequired);
            Assert.IsFalse(GuarantorEmailAddressRequired);
        }
        [Test]
        public void PrimaryCarePhysicianRequiredForShortRegistrationActivity_ShouldReturnTrue()
        {
            var account = new Account {Activity = new ShortRegistrationActivity()};
            var PrimaryCarePhysicianRequired =
                account.IsRuleEnforcedForCompositeAction<OnShortDiagnosisForm>(RuleIds.PrimaryCarePhysicianRequired);
            Assert.IsTrue(PrimaryCarePhysicianRequired);
        }
        
        [Test]
        public void COBReceivedRequiredForShortRegistrationActivity_ShouldReturnTrue()
        {
            var account = new Account { Activity = new ShortRegistrationActivity() };
            account.KindOfVisit = VisitType.Outpatient;
            var financialClass = new FinancialClass { Code = "80" };
            account.FinancialClass = financialClass;
            var COBReceivedRequired = account.IsRuleEnforcedForCompositeAction<OnShortRegulatoryForm>(RuleIds.COBReceivedRequired);
        
            Assert.IsTrue(COBReceivedRequired);
        }

        [Test]
        public void IMFMReceivedNotRequiredForShortRegistrationActivity_ShouldReturnFalse()
        {
            var account = new Account {Activity = new ShortRegistrationActivity()};
            account.KindOfVisit = VisitType.Recurring;
            var financialClass = new FinancialClass { Code = "84" };
            account.FinancialClass = financialClass;

            var IMFMReceivedRequired = account.IsRuleEnforcedForCompositeAction<OnShortRegulatoryForm>(RuleIds.IMFMReceivedRequired);

            Assert.IsFalse(IMFMReceivedRequired);
        }
    }
}
