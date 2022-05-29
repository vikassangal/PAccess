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
    public class AttendingPhysicianPreferredTests
    {
        #region Test Methods
        [Test]
        public void TestCanBeAppliedToWithInvalidContextType_ShouldReturnTrue()
        {
            var ruleUnderTest = new AttendingPhysicianPreferred();
            var inValidObjectType = new object();
            var actualResult = ruleUnderTest.CanBeAppliedTo( inValidObjectType );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedToWithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new AttendingPhysicianPreferred();
            var actualResult = ruleUnderTest.CanBeAppliedTo( null );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var ruleUnderTest = new AttendingPhysicianPreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( otherCoverage ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithAttendingPhysician_ShouldReturnTrue()
        {
            var account = new Account
                              {
                                    KindOfVisit = new VisitType(),
                                    Activity= new AdmitNewbornActivity() 
                              };
            var physicianRelationship = new PhysicianRelationship();
            PhysicianRole physicianRole = new AttendingPhysician();
            physicianRelationship.PhysicianRole = physicianRole;
            var physician = new Physician( 0, DateTime.Now, "Last,First", "First", "Last", "" );
            physicianRelationship.Physician = physician;
            account.AddPhysicianRelationship( physicianRelationship );
            var ruleUnderTest = new AttendingPhysicianPreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithoutAttendingPhysician_ShouldReturnFalse()
        {
            var account = new Account
            {
                KindOfVisit = new VisitType(),
                Activity = new AdmitNewbornActivity()
            };
            var ruleUnderTest = new AttendingPhysicianPreferred();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        #endregion

        #region Support Methods

        #endregion

        #region Constants

        #endregion
    }
}
