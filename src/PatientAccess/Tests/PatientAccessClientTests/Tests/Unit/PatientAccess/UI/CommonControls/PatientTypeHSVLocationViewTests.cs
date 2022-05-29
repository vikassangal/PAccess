using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace Tests.Unit.PatientAccess.UI.CommonControls
{
    /// <summary>
    /// Summary description for PatientTypeHSVLocationViewTests
    /// </summary>
    [TestFixture]
    public class PatientTypeHSVLocationViewTests
    {
        private const long ACO_FACILITY_ID = 900;

        [Test]
        public void TestPopulateHsvCodes_WhenActivityIsRegistrationAndVisitTypeIsER()
        {
            //TODO-AC not really a unit test as it touches the database move/mark it appropriately
            PatientTypeHSVLocationView patientTypeHsvLocationView = new PatientTypeHSVLocationView();
            HSVBrokerProxy brokerProxy = new HSVBrokerProxy();

            VisitType erVisit = new VisitType { Code = VisitType.EMERGENCY_PATIENT };
            Activity registrationActivity = new RegistrationActivity();
            FinancialClass financialClass = new FinancialClass();
            HospitalService hospitalService = new HospitalService();
            ArrayList expectedHsvCodes = (ArrayList)brokerProxy.HospitalServicesFor(ACO_FACILITY_ID, VisitType.EMERGENCY_PATIENT);
            ArrayList actualHsvCodes = patientTypeHsvLocationView.GetHospitalServiceCodes(ACO_FACILITY_ID, erVisit, registrationActivity, hospitalService, financialClass);

            CollectionAssert.AreEquivalent(expectedHsvCodes, actualHsvCodes);
        }

        [Test]
        [Category( "Fast" )]
        public void TestIsPatientTypeValidForLocation_WhenActivityIsRegistrationAndVisitTypeIsER_ShouldReturnFalse()
        {
            PatientTypeHSVLocationView patientTypeHsvLocationView = new PatientTypeHSVLocationView();
            HospitalService hospitalService = new HospitalService { Code = HospitalService.AMBULANCE };
            VisitType visitKind = new VisitType { Code = VisitType.EMERGENCY_PATIENT };
            Activity activity = new RegistrationActivity();

            bool isValid = patientTypeHsvLocationView.IsPatientTypeValidForLocation(hospitalService, visitKind, hospitalService, activity);

            Assert.IsFalse(isValid);
        }

        [Test]
        [Category( "Fast" )]
        public void TestIsPatientTypeValidForLocation_WhenActivityIsRegistrationAndVisitTypeIsInPatient_ShouldReturnTrue()
        {
            PatientTypeHSVLocationView patientTypeHsvLocationView = new PatientTypeHSVLocationView();
            HospitalService hospitalService = new HospitalService { Code = HospitalService.AMBULANCE };
            VisitType visitKind = new VisitType { Code = VisitType.INPATIENT };
            Activity activity = new RegistrationActivity();

            bool isValid = patientTypeHsvLocationView.IsPatientTypeValidForLocation(hospitalService, visitKind, hospitalService, activity);

            Assert.IsTrue(isValid);
        }

        [Test]
        [Category( "Fast" )]
        public void TestIsPatientTypeValidForLocation_WhenActivityIsMaintenanceAndVisitTypeIsER_ShouldReturnFalse()
        {
            PatientTypeHSVLocationView patientTypeHsvLocationView = new PatientTypeHSVLocationView();
            HospitalService hospitalService = new HospitalService { Code = HospitalService.AMBULANCE };
            VisitType visitKind = new VisitType { Code = VisitType.EMERGENCY_PATIENT };
            Activity activity = new MaintenanceActivity();

            bool isValid = patientTypeHsvLocationView.IsPatientTypeValidForLocation(hospitalService, visitKind, hospitalService, activity);

            Assert.IsFalse(isValid);
        }

        [Test]
        [Category( "Fast" )]
        public void TestEnableOrDisableEDLogFields_ActivityIsMaintenanceAndVisitTypeIsER_ShouldFireEnableEDLogFieldsEvent()
        {
            PatientTypeHSVLocationView patientTypeHsvLocationView = new PatientTypeHSVLocationView();
            bool eventWasFired = false;
            patientTypeHsvLocationView.PatientTypeChanged += delegate { eventWasFired = true; };

            VisitType kindOfVisit = new VisitType { Code = VisitType.EMERGENCY_PATIENT };

            patientTypeHsvLocationView.FirePatientTypeChanged(kindOfVisit);

            Assert.IsTrue(eventWasFired);
        }
    }
}