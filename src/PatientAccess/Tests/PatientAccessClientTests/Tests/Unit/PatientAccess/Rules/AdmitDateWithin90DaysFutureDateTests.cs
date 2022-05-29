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
    public class AdmitDateWithin90DaysFutureDateTests
    {
        #region Test Methods
        [Test]
        public void TestCanBeAppliedToWithInvalidContextType_ShouldReturnTrue()
        {
            var ruleUnderTest = new AdmitDateWithin90DaysFutureDate();
            var inValidObjectType = new object();
            var actualResult = ruleUnderTest.CanBeAppliedTo( inValidObjectType );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedToWithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new AdmitDateWithin90DaysFutureDate();
            var actualResult = ruleUnderTest.CanBeAppliedTo( null );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var ruleUnderTest = new AdmitDateWithin90DaysFutureDate();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( otherCoverage ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithAdmitTime9DaysInFuture_ShouldReturnTrue()
        {
            var account = new Account
            {
                AdmitDate = DateTime.Now.AddDays(90)
            };
            var ruleUnderTest = new AdmitDateWithin90DaysFutureDate();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithAdmitTime91DaysInFuture_ShouldReturnFalse()
        {
            var account = new Account
            {
                AdmitDate = DateTime.Now.AddDays( 91 )
            };
            var ruleUnderTest = new AdmitDateWithin90DaysFutureDate();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        #endregion

        #region Support Methods

        #endregion

        #region Constants

        #endregion
    }
}
