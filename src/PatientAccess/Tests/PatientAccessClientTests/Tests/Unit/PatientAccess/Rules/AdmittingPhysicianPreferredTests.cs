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
    public class AdmittingPhysicianPreferredTests
    {
        #region Test Methods
        [Test]
        public void TestCanBeAppliedToWithInvalidContextType_ShouldReturnTrue()
        {
            var ruleUnderTest = new AdmittingPhysicianPreferred();
            var inValidObjectType = new object();
            var actualResult = ruleUnderTest.CanBeAppliedTo( inValidObjectType );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedToWithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new AdmittingPhysicianPreferred();
            var actualResult = ruleUnderTest.CanBeAppliedTo( null );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var ruleUnderTest = new AdmittingPhysicianPreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( otherCoverage ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithAdmittingPhysician_ShouldReturnTrue()
        {
            var account = new Account();
            var physicianRelationship = new PhysicianRelationship();
            PhysicianRole physicianRole = new AdmittingPhysician();
            physicianRelationship.PhysicianRole = physicianRole;
            var physician = new Physician( 0, DateTime.Now, "Last,First","First","Last","" );
            physicianRelationship.Physician = physician;
            account.AddPhysicianRelationship(physicianRelationship);
            var ruleUnderTest = new AdmittingPhysicianPreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithoutAdmittingPhysician_ShouldReturnFalse()
        {
            var account = new Account();
            var ruleUnderTest = new AdmittingPhysicianPreferred();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        #endregion

        #region Support Methods

        #endregion

        #region Constants

        #endregion
    }
}
