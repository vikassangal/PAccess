using NUnit.Framework;
using PatientAccess.Actions;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using OnContactAndDiagnosisForm = PatientAccess.Actions.OnContactAndDiagnosisForm;
using OnPreMSEDemographicsForm = PatientAccess.Actions.OnPreMSEDemographicsForm;

namespace Tests.Integration.PatientAccess.UI
{
    [TestFixture]
    public class PreMSERegisterActivityRequiredFieldsTests
    {
        [Test]
        public void MaritalStatusIsRequireForRegistrationActivity()
        {
            var account = new Account { Activity = new PreMSERegisterActivity() };

            var isMaritalStatusIsRequiredEnforced = account.IsRuleEnforcedForCompositeAction<OnPreMSEDemographicsForm>( RuleIds.MaritalStatusRequired );

            Assert.IsTrue( isMaritalStatusIsRequiredEnforced );
        }

        [Test]
        public void EmergencyContactPhoneNumberShouldbeRequiredIfAreaCodeIsEntered()
        {
            var emergencyContactWithPhoneAreaCodeOnly = new EmergencyContact { Name = "Alan Smith" };

            emergencyContactWithPhoneAreaCodeOnly.AddContactPoint(

                new ContactPoint( TypeOfContactPoint.NewPhysicalContactPointType() )
                {
                    PhoneNumber = new PhoneNumber { AreaCode = "123" }
                } );

            var account = new Account { Activity = new PreMSERegisterActivity(), EmergencyContact1 = emergencyContactWithPhoneAreaCodeOnly };

            var isAreaCodeRequiresPhoneNumberEnforced = account.IsRuleEnforcedForCompositeAction<OnContactAndDiagnosisForm>( RuleIds.AreaCodeRequiresPhoneNumber );

            Assert.IsTrue( isAreaCodeRequiresPhoneNumberEnforced, "Phone number should be required if area code is present" ); 
        }

        [Test]
        public void PrimaryCarePhysicianRequiredForPreMSERegisterActivity_ShouldReturnTrue()
        {
            var account = new Account { Activity = new PreMSERegisterActivity() };
            var PrimaryCarePhysicianRequired =
                account.IsRuleEnforcedForCompositeAction<OnContactAndDiagnosisForm>(RuleIds.PrimaryCarePhysicianRequired);
            Assert.IsTrue(PrimaryCarePhysicianRequired);
        }

        [Test]
        public void COBReceivedNotRequiredForPreMSERegisterActivity_ShouldReturnFalse()
        {
            var account = new Account { Activity = new PreMSERegisterActivity() };
            account.KindOfVisit = VisitType.PreRegistration;
            var financialClass = new FinancialClass { Code = "20" };
            account.FinancialClass = financialClass;
            var COBReceivedRequired = account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>(RuleIds.COBReceivedRequired);
        
            Assert.IsFalse(COBReceivedRequired);
        }

        [Test]
        public void IMFMReceivedNotRequiredForPreMSERegisterActivity_ShouldReturnFalse()
        {
            var account = new Account {Activity = new PreMSERegisterActivity()};
            account.KindOfVisit = VisitType.PreRegistration;
            var financialClass = new FinancialClass { Code = "23" };
            account.FinancialClass = financialClass;
            var IMFMReceivedRequired = account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>(RuleIds.IMFMReceivedRequired);

            Assert.IsFalse(IMFMReceivedRequired);
        }
    }
}
