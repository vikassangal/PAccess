using PatientAccess.Domain;
using PatientAccess.Rules;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Rules
{
    [TestFixture]
    [Category( "Fast" )]
    public class WorkersCompEmpSupervisorRequiredTests
    {
        #region Test Methods

        [Test]
        public void TestCanBeAppliedTo_WhenCoverageIsNotWorkersCompensationCoverage_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION) };
            var ruleUnderTest = new WorkersCompEmpSupervisorRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(otherCoverage));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenCoverageIsWorkersCompensationAndPatientSupervisorisNotEmpty_ShouldReturnTrue()
        {
            var otherCoverage = new WorkersCompensationCoverage { CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION) };
            otherCoverage.PatientsSupervisor = "Supervisor name";
            var ruleUnderTest = new WorkersCompEmpSupervisorRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(otherCoverage));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenCoverageIsWorkersCompensationAndPatientSupervisorisEmpty_ShouldReturnFalse()
        {
            var otherCoverage = new WorkersCompensationCoverage { CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION) };
            var ruleUnderTest = new WorkersCompEmpSupervisorRequired();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(otherCoverage));
        }
    
        #endregion

        #region Support Methods
        #endregion
    }
}
