using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AdmitTimePreferredTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class ReferringPhysicianPreferredTests
    {
        #region Test Methods
        [Test]
        public void TestCanBeAppliedToWithInvalidContextType_ShouldReturnTrue()
        {
            var ruleUnderTest = new ReferringPhysicianPreferred();
            var inValidObjectType = new object();
            var actualResult = ruleUnderTest.CanBeAppliedTo( inValidObjectType );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedToWithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new ReferringPhysicianPreferred();
            var actualResult = ruleUnderTest.CanBeAppliedTo( null );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var ruleUnderTest = new ReferringPhysicianPreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( otherCoverage ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithReferringPhysician_ShouldReturnTrue()
        {
            var account = new Account();
            var physicianRelationship = new PhysicianRelationship();
            PhysicianRole physicianRole = new ReferringPhysician();
            physicianRelationship.PhysicianRole = physicianRole;
            var physician = new Physician( 0, DateTime.Now, "Last,First", "First", "Last", "" );
            physicianRelationship.Physician = physician;
            account.AddPhysicianRelationship( physicianRelationship );
            var ruleUnderTest = new ReferringPhysicianPreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithoutReferringPhysician_ShouldReturnFalse()
        {
            var account = new Account();
            var ruleUnderTest = new ReferringPhysicianPreferred();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        #endregion

        #region Support Methods

        #endregion

        #region Constants

        #endregion
    }
}
