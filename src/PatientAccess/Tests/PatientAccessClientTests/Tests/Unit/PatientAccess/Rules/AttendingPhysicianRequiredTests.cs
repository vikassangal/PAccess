using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AdmitTimeRequiredTests
    /// </summary>
    [TestFixture]
    [Category("Fast")]
    public class AttendingPhysicianRequiredTests
    {
        #region Test Methods
        [Test]
        public void TestCanBeAppliedTo_WithoutAttendingPhysician_ForEditPreMseActivity_ShouldReturnFalse()
        {
            var account = new Account
            {
                KindOfVisit = new VisitType(),
                Activity = new EditPreMseActivity()
            };
            var ruleUnderTest = new AttendingPhysicianRequired();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_WithAttendingPhysician_ForEditPreMseActivity_ShouldReturnTrue()
        {
            var account = new Account
                              {
                                  KindOfVisit = new VisitType(),
                                  Activity = new EditPreMseActivity()
                              };

            var physicianRelationship = new PhysicianRelationship();
            PhysicianRole physicianRole = new AttendingPhysician();
            physicianRelationship.PhysicianRole = physicianRole;
            var physician = new Physician(0, DateTime.Now, "Last,First", "First", "Last", "");
            physicianRelationship.Physician = physician;
            account.AddPhysicianRelationship(physicianRelationship);

            var ruleUnderTest = new AttendingPhysicianRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        #endregion

        #region Support Methods

        #endregion

        #region Constants

        #endregion
    }
}
