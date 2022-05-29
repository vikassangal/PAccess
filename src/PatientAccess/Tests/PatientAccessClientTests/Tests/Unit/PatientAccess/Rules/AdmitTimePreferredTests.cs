using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AdmitTimePreferredTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class AdmitTimePreferredTests
    {
        #region Test Methods
        [Test]
        public void TestCanBeAppliedToWithInvalidContextType_ShouldReturnTrue()
        {
            var ruleUnderTest = new AdmitTimePreferred();
            var inValidObjectType = new object();
            var actualResult = ruleUnderTest.CanBeAppliedTo( inValidObjectType );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedToWithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new AdmitTimePreferred();
            var actualResult = ruleUnderTest.CanBeAppliedTo( null );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var ruleUnderTest = new AdmitTimePreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( otherCoverage ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithAdmitTime_ShouldReturnTrue()
        {
            var account = new Account
            {
                AdmitDate = new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 2, 30, 5 )
            };
            var ruleUnderTest = new AdmitTimePreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithoutBirthTime_ShouldReturnFalse()
        {
            var account = new Account
            {
                AdmitDate = new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0 )
            };
            var ruleUnderTest = new AdmitTimePreferred();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        #endregion

        #region Support Methods

        #endregion

        #region Constants

        #endregion
    }
}
