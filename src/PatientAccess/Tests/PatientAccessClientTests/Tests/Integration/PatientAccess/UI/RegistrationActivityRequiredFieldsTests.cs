using NUnit.Framework;
using PatientAccess.Actions;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.UI
{
    [TestFixture]
    public class RegistrationActivityRequiredFieldsTests
    {
        [Test]
        public void MaritalStatusIsRequireForRegistrationActivity()
        {

            var account = new Account { Activity = new RegistrationActivity() };

            var maritalStatusIsRequiredAndMissing = account.IsRuleEnforcedForCompositeAction<OnPatientDemographics>( RuleIds.MaritalStatusRequired );

            Assert.IsTrue( maritalStatusIsRequiredAndMissing );
        }

        [Test]
        public void EthnicityIsRequireForRegistrationActivity()
        {
            var account = new Account { Activity = new RegistrationActivity() };

            var ethnicityIsRequiredAndMissing = account.IsRuleEnforcedForCompositeAction<OnPatientDemographics>( RuleIds.EthnicityRequired );

            Assert.IsTrue( ethnicityIsRequiredAndMissing );
        }

        [Test]
        public void PrimaryCarePhysicianRequiredForRegistrationActivity_ShouldReturnTrue()
        {
            var account = new Account {Activity = new RegistrationActivity()};
            var PrimaryCarePhysicianRequired =
                account.IsRuleEnforcedForCompositeAction<OnClinicalForm>(RuleIds.PrimaryCarePhysicianRequired);
            Assert.IsTrue(PrimaryCarePhysicianRequired);
        }
        
        [Test]
        public void COBReceivedNotRequiredForRegistrationActivity_ShouldReturnTrue()
        {
            var account = new Account { Activity = new RegistrationActivity() };
            account.KindOfVisit = VisitType.Inpatient;
            var financialClass = new FinancialClass { Code = "81" };
            account.FinancialClass = financialClass;
            var COBReceivedRequired = account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>(RuleIds.COBReceivedRequired);

            Assert.IsTrue(COBReceivedRequired);
        }

        [Test]
        public void IMFMReceivedNotRequiredForRegistrationActivity_ShouldReturnTrue()
        {
            var account = new Account { Activity = new RegistrationActivity() };
            account.KindOfVisit = VisitType.Inpatient;
            var financialClass = new FinancialClass { Code = "42" };
            account.FinancialClass = financialClass;
            var IMFMReceivedRequired = account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>(RuleIds.IMFMReceivedRequired);

            Assert.IsTrue(IMFMReceivedRequired);
        }

    }
}
