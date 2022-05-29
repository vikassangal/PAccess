using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain.Parties
{
    [TestFixture]
    [Category( "Fast" )]
    public class AttendingPhysicianTests
    {
        [Test]
        public void TestIsPhysicianAssignableFor_WhenPhysicianIsActiveVisitTypeIsERPatientAndActivityIsRegistration_ShouldReturnTrue()
        {
            var registrationActivity = new RegistrationActivity();
            var physician = new Physician {ActiveInactiveFlag = PhysicianRole.ACTIVE};
            var visitType = new VisitType {Code = VisitType.EMERGENCY_PATIENT};
            
            var isValid = AttendingPhysician.IsStatusValidFor( visitType, registrationActivity, physician );

            Assert.IsTrue( isValid );
        }

        [Test]
        public void TestIsPhysicianAssignableFor_WhenPhysicianIsInActiveVisitTypeIsERPatientAndActivityIsRegistration_ShouldReturnTrue()
        {
            var activity = new RegistrationActivity();
            var physician = new Physician();
            var visitType = new VisitType {Code = VisitType.EMERGENCY_PATIENT};
            
            var isValid = AttendingPhysician.IsStatusValidFor( visitType, activity, physician );

            Assert.IsTrue( isValid );
        }

        [Test]
        public void TestIsPhysicianAssignableFor_WhenPhysicianIsInActiveVisitTypeIsInPatientAndActivityIsRegistration_ShouldReturnFalse()
        {
            var activity = new RegistrationActivity();
            var physician = new Physician();
            var visitType = new VisitType {Code = VisitType.INPATIENT};
            
            var isValid = AttendingPhysician.IsStatusValidFor( visitType, activity, physician );

            Assert.IsFalse( isValid );
        }

        [Test]
        public void TestIsPhysicianAssignableFor_WhenPhysicianIsInActiveVisitTypeIsERPatientAndActivityIsMaintenance_ShouldReturnTrue()
        {
            var activity = new MaintenanceActivity();
            var physician = new Physician();
            var visitType = new VisitType {Code = VisitType.EMERGENCY_PATIENT};
            
            bool isValid = AttendingPhysician.IsStatusValidFor( visitType, activity, physician );

            Assert.IsTrue( isValid );
        }

        [Test]
        public void TestIsPhysicianAssignableFor_WhenPhysicianIsInActiveVisitTypeIsERPatientAndActivityIsPostMseRegistration_ShouldReturnFalse()
        {
            var activity = new PostMSERegistrationActivity();
            var physician = new Physician();
            var visitType = new VisitType {Code = VisitType.EMERGENCY_PATIENT};
            
            bool isValid = AttendingPhysician.IsStatusValidFor( visitType, activity, physician );

            Assert.IsFalse( isValid );
        }
    }
}