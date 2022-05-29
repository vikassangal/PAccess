using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using OnContactAndDiagnosisForm = PatientAccess.Actions.OnContactAndDiagnosisForm;
using OnPreMSEDemographicsForm = PatientAccess.Actions.OnPreMSEDemographicsForm;

namespace Tests.Integration.PatientAccess.UI
{
    [TestFixture]
    public class EditPreMseActivityRequiredFieldsTests
    {
        [Test]
        public void MaritalStatusIsRequireForRegistrationActivity()
        {
            var account = new Account { Activity = new EditPreMseActivity() };

            var maritalStatusRequiredIsEnforced = account.IsRuleEnforcedForCompositeAction<OnPreMSEDemographicsForm>( RuleIds.MaritalStatusRequired );

            Assert.IsTrue( maritalStatusRequiredIsEnforced, "Marital status is should be required" );
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

            var account = new Account { Activity = new EditPreMseActivity(), EmergencyContact1 = emergencyContactWithPhoneAreaCodeOnly };

            var phoneNumberRequiredIsEnforced = account.IsRuleEnforcedForCompositeAction<OnContactAndDiagnosisForm>( RuleIds.AreaCodeRequiresPhoneNumber );

            Assert.IsTrue( phoneNumberRequiredIsEnforced, "Emergency contact phone number should be required if phone area code is present" );
        }

        [Test]
        public void MailingPhoneNumberShouldbeRequiredIfAreaCodeIsEntered()
        {
            var account = new Account { Activity = new EditPreMseActivity(), Patient = new Patient() };

            account.Patient.AddContactPoint(

                new ContactPoint( TypeOfContactPoint.NewMailingContactPointType() )
                {
                    PhoneNumber = new PhoneNumber { AreaCode = "123" }
                } );


            var phoneNumberRequiredIsEnforced = account.IsRuleEnforcedForCompositeAction<OnPreMSEDemographicsForm>( RuleIds.AreaCodeRequiresPhoneNumber );

            Assert.IsTrue( phoneNumberRequiredIsEnforced, "Mailing contact phone number should be required if phone area code is present" );
        }

        [Test]
        public void PhysicalAddressPhoneNumberShouldbeRequiredIfAreaCodeIsEntered()
        {
            var account = new Account { Activity = new EditPreMseActivity(), Patient = new Patient() };

            account.Patient.AddContactPoint(

                new ContactPoint( TypeOfContactPoint.NewPhysicalContactPointType() )
                {
                    PhoneNumber = new PhoneNumber { AreaCode = "123" }
                } );


            var phoneNumberRequiredIsEnforced = account.IsRuleEnforcedForCompositeAction<OnPreMSEDemographicsForm>( RuleIds.AreaCodeRequiresPhoneNumber );

            Assert.IsTrue( phoneNumberRequiredIsEnforced, "Physical address phone number should be required if phone area code is present" );
        }

        [Test]
        public void PrimaryCarePhysicianRequiredForEditPreMseActivity_ShouldReturnTrue()
        {
            var account = new Account {Activity = new EditPreMseActivity(), Patient = new Patient()};
            var PrimaryCarePhysicianRequired =
                account.IsRuleEnforcedForCompositeAction<OnContactAndDiagnosisForm>(RuleIds.PrimaryCarePhysicianRequired);
            Assert.IsTrue(PrimaryCarePhysicianRequired);
        }
    }
}
