using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category( "Fast" )]
    public class MaritalStatusRequiredTests
    {
        [Test]
        public void TestMaritalStatusRequiredRule_WhenMaritalStatusIsNotPresent_ShouldReturnFalse()
        {
            var rule = new MaritalStatusRequired();
            var account = new Account();
            var patient = new Patient();
            account.Patient = patient;
            patient.MaritalStatus = null;
            
            var maritalStatusIsPresent = rule.CanBeAppliedTo( account );

            Assert.IsFalse( maritalStatusIsPresent, "Missing marital status should be reported by the rule" );
        }

        [Test]
        public void TestMaritalStatusRequiredRule_WhenMaritalStatusIsBlank_ShouldReturnFalse()
        {
            var rule = new MaritalStatusRequired();
            var account = new Account();
            var patient = new Patient();
            account.Patient = patient;
            var maritalStatus = new MaritalStatus { Code = string.Empty };
            patient.MaritalStatus = maritalStatus;

            var maritalStatusIsPresent = rule.CanBeAppliedTo( account );

            Assert.IsFalse( maritalStatusIsPresent, "Missing marital status should be reported by the rule" );
        }

        [Test]
        public void TestMaritalStatusRequiredRule_WhenMaritalStatusIsPresent_ShouldReturnTrue()
        {
            var rule = new MaritalStatusRequired();
            var account = new Account();
            var patient = new Patient();
            account.Patient = patient;
            var maritalStatus = new MaritalStatus { Code = "S" };
            patient.MaritalStatus = maritalStatus;

            var maritalStatusIsPresent = rule.CanBeAppliedTo( account );

            Assert.IsTrue( maritalStatusIsPresent, "Rule should not complain if marital status is present" );
        }
    }
}
