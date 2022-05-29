using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for HospitalServicePreferredTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class HospitalServicePreferredTests
    {
        #region Test Methods
        [Test]
        public void TestCanBeAppliedToWithInvalidContextType_ShouldReturnTrue()
        {
            var ruleUnderTest = new HospitalServicePreferred();
            var inValidObjectType = new object();
            var actualResult = ruleUnderTest.CanBeAppliedTo( inValidObjectType );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedToWithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new HospitalServicePreferred();
            var actualResult = ruleUnderTest.CanBeAppliedTo( null );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var ruleUnderTest = new HospitalServicePreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( otherCoverage ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithNullHospitalService_ShouldReturnFalse()
        {
            var account = new Account();
            var ruleUnderTest = new HospitalServicePreferred();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithEmptyHospitalServiceCode_ShouldReturnFalse()
        {
            var account = new Account
                              {
                                  HospitalService = new HospitalService(),
                              };
            var ruleUnderTest = new HospitalServicePreferred();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }


        #endregion

        #region Support Methods

        #endregion

        #region Constants

        #endregion
    }
}
