using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category( "Fast" )]
    public class ServicesAuthorizedPreferredTests
    {
        #region Test Methods

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsGovernmentMedicareCoverage_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentMedicareCoverage { CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION) };
            var ruleUnderTest = new ServicesAuthorizedPreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( otherCoverage ) );
        }

        [Test]
        public void TestCanBeAppliedTo_AuthorizationIsEmpty_ShouldReturnTrue()
        {
            var authorization = new Authorization { AuthorizationNumber = string.Empty };
            var wcCoverage = GetWorkersCompensationCoverage( authorization);
            var ruleUnderTest = new ServicesAuthorizedPreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( wcCoverage ) );
        }
        [Test]
        public void TestCanBeAppliedTo_AuthorizationIsNotEmptyServicesAuthorizedIsNotEmpty_ShouldReturnTrue()
        {
            var authorization = new Authorization { AuthorizationNumber = "01234" , ServicesAuthorized =  "Some services"};
            var wcCoverage = GetWorkersCompensationCoverage(authorization);
            var ruleUnderTest = new ServicesAuthorizedPreferred();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo( wcCoverage ));
        }
        [Test]
        public void TestCanBeAppliedTo_AuthorizationIsNotEmptyServicesAuthorizedIsEmpty_ShouldReturnFalse()
        {
            var authorization = new Authorization { AuthorizationNumber = "01234", ServicesAuthorized = string.Empty };
            var wcCoverage = GetWorkersCompensationCoverage(authorization);
            var ruleUnderTest = new ServicesAuthorizedPreferred();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(wcCoverage));
        }
       
        #endregion

        #region Support Methods

        private  Coverage GetWorkersCompensationCoverage( Authorization authorization )
        {
            var workersCompCoverage = new WorkersCompensationCoverage() { CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION) };
            workersCompCoverage.Authorization = authorization;
            return workersCompCoverage;
        }
        #endregion
    }
}
