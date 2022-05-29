using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain.Parties
{
    [TestFixture]
    [Category( "Fast" )]
    public class AdmittingPhysicianTests
    {
        [Test]
        public void TestIsPhysicianAssignableFor_WhenPhysicianIsActiveVisitTypeIsERPatientAndActivityIsRegistration_ShouldReturnTrue()
        {
            var registrationActivity = new RegistrationActivity();
            
            var physician = new Physician {ActiveInactiveFlag = PhysicianRole.ACTIVE};

            var visitType = new VisitType {Code = VisitType.EMERGENCY_PATIENT};

            var isValid = AdmittingPhysician.IsStatusValidFor(visitType, registrationActivity, physician);

            Assert.IsTrue(isValid);
        }

        [Test]
        public void TestIsPhysicianAssignableFor_WhenPhysicianIsInActiveVisitTypeIsERPatientAndActivityIsRegistration_ShouldReturnTrue()
        {
            var registrationActivity = new RegistrationActivity();
            var physician = new Physician();
            var visitType = new VisitType {Code = VisitType.EMERGENCY_PATIENT};

            var isValid = AdmittingPhysician.IsStatusValidFor(visitType, registrationActivity, physician);

            Assert.IsTrue(isValid);
        }

        [Test]
        public void TestIsPhysicianAssignableFor_WhenPhysicianIsInActiveVisitTypeIsInPatientAndActivityIsRegistration_ShouldReturnFalse()
        {
            var registrationActivity = new RegistrationActivity();
            var physician = new Physician();
            
            var visitType = new VisitType {Code = VisitType.INPATIENT};

            var isValid = AdmittingPhysician.IsStatusValidFor(visitType, registrationActivity, physician);

            Assert.IsFalse(isValid);
        }

        [Test]
        public void TestIsPhysicianAssignableFor_WhenPhysicianIsInActiveVisitTypeIsERPatientAndActivityIsMaintenance_ShouldReturnTrue()
        {
            var maintenanceActivity = new MaintenanceActivity();
            var physician = new Physician();
            var visitType = new VisitType {Code = VisitType.EMERGENCY_PATIENT};

            var isValid = AdmittingPhysician.IsStatusValidFor(visitType, maintenanceActivity, physician);

            Assert.IsTrue(isValid);
        }


        [Test]
        public void TestIsPhysicianAssignableFor_WhenPhysicianIsInActiveVisitTypeIsERPatientAndActivityIsPostMseRegistration_ShouldReturnFalse()
        {
            var postMseRegistrationActivity = new PostMSERegistrationActivity();
            var physician = new Physician();
            var visitType = new VisitType {Code = VisitType.EMERGENCY_PATIENT};
            
            var isValid = AdmittingPhysician.IsStatusValidFor(visitType, postMseRegistrationActivity, physician);

            Assert.IsFalse(isValid);
        }


        [Test]
        public void TestArePrivilegesValidFor_WhenPhysicianDoesNotHaveAdmittingPrivilegesAndVisitTypeIsInPatient_ShouldReturnFalse()
        {
            var activity = new RegistrationActivity();
            var physician = new Physician();

            var account = new Account {Activity = activity};

            var isValid = AdmittingPhysician.ArePrivilegesValidFor(account, physician);
            Assert.IsFalse(isValid);
        }

        [Test]
        public void TestArePrivilegesValidFor_WhenPhysicianDoesNotHaveAdmittingPrivilegesAndVisitTypeIsNonPatient_ShouldReturnTrue()
        {
            var activity = new RegistrationActivity();
            var physician = new Physician();

            var account = new Account { Activity = activity };
            account.KindOfVisit.Code = VisitType.NON_PATIENT;
            account.HospitalService.DayCare = "N";

            var isValid = AdmittingPhysician.ArePrivilegesValidFor(account, physician);
            Assert.IsTrue(isValid);
        }


        [Test]
        public void TestArePrivilegesValidFor_WhenPhysicianHasAdmittingPrivilegesAndVisitTypeIsERPatient_ShouldReturnTrue()
        {
            var registrationActivity = new RegistrationActivity();
            var physician = new Physician {AdmittingPrivileges = "Y"};

            var visitType = new VisitType();
            visitType.Code = VisitType.EMERGENCY_PATIENT;

            var account = new Account
                              {
                                  Activity = registrationActivity, 
                                  KindOfVisit = visitType
                              };

            var isValid = AdmittingPhysician.ArePrivilegesValidFor(account, physician);
            Assert.IsTrue(isValid);
        }

        [Test]
        public void TestArePrivilegesValidFor_WhenPhysicianHasAdmittingPrivilegesVisitTypeIsERPatientAndPhysicianPrivilegeSuspendDateLesserThanAdmitDate_ShouldReturnFalse()
        {
            var activity = new RegistrationActivity();
            var physician = new Physician {AdmittingPrivileges = "Y"};
            var currentDate = DateTime.Now;
            physician.AdmitPrivilegeSuspendDate = currentDate;
            var visitType = new VisitType {Code = VisitType.EMERGENCY_PATIENT};

            var account = new Account
                              {
                                  Activity = activity,
                                  KindOfVisit = visitType,
                                  AdmitDate = currentDate.Add( TimeSpan.FromDays( 1 ) )
                              };

            var isValid = AdmittingPhysician.ArePrivilegesValidFor(account, physician);
            
            Assert.IsFalse(isValid);
        }

        [Test]
        public void TestArePrivilegesValidFor_WhenPhysicianHasAdmittingPrivilegesVisitTypeIsERPatientAndPhysicianPrivilegeSuspendDateGreaterThanAdmitDate_ShouldReturnTrue()
        {
            var registrationActivity = new RegistrationActivity();
            var physician = new Physician {AdmittingPrivileges = "Y"};
            
            var currentDate = DateTime.Now;
            
            physician.AdmitPrivilegeSuspendDate = currentDate.Add(TimeSpan.FromDays(1));
            
            var visitType = new VisitType {Code = VisitType.EMERGENCY_PATIENT};

            var account = new Account
                              {
                                  Activity = registrationActivity, 
                                  KindOfVisit = visitType, 
                                  AdmitDate = currentDate
                              };

            var isValid = AdmittingPhysician.ArePrivilegesValidFor(account, physician);

            Assert.IsTrue(isValid);
        }
    }
}
