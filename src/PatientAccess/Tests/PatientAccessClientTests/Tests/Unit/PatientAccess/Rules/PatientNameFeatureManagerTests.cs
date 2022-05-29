using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
    public class PatientNameFeatureManagerTests
    {
        [Test]
        public void TestCanBeAppliedTo_WithNullContext_ShouldReturnFalse()
        {
            var patientNameFeatureManager = new PatientNameFeatureManager();
            var actualResult = patientNameFeatureManager.IsAutoPopulatePatientName_Enabled(null);
            Assert.IsFalse(actualResult);
        }

        [Test]
        public void TestCanBeAppliedTo_WithNullActivity_ShouldReturnFalse()
        {
            var account = GetAccount(null,  true);
            var patientNameFeatureManager = new PatientNameFeatureManager();
            var actualResult = patientNameFeatureManager.IsAutoPopulatePatientName_Enabled(account);
            Assert.IsFalse(actualResult);
        }

        [Test]
        public void TestIsAutoPopulateNameFeatureEnabled_AdmitNewBornActivity_ShouldReturnTrue()
        {
            var account = GetAccount(new AdmitNewbornActivity(), true);
            var patientNameFeatureManager = new PatientNameFeatureManager();
            Assert.IsTrue(patientNameFeatureManager.IsAutoPopulatePatientName_Enabled(account));
        }
        [Test]
        public void TestIsAutoPopulateNameFeatureEnabled_AdmitNewBornActivity_FacilityyNotEnabled_ShouldReturnFalse()
        {
            var account = GetAccount(new AdmitNewbornActivity(), false);
            var patientNameFeatureManager = new PatientNameFeatureManager();
            Assert.IsFalse(patientNameFeatureManager.IsAutoPopulatePatientName_Enabled(account));
        }
        
        [Test]
        public void TestIsAutoPopulateNameFeatureEnabled_WhenActivityIsPreRegistration_FacilityEnabled_ShouldReturnFalse()
        {
            var account = GetAccount(new PreRegistrationActivity(), true);
            var patientNameFeatureManager = new PatientNameFeatureManager();
            Assert.IsFalse(patientNameFeatureManager.IsAutoPopulatePatientName_Enabled(account));
        }
        
        [Test]
        public void TestIsAutoPopulateNameFeatureEnabled_WhenActivityIsRegistration_FacilityEnabled_ShouldReturFalse()
        {
            Account account = GetAccount(new RegistrationActivity(), true);
            var patientNameFeatureManager = new PatientNameFeatureManager();
            Assert.IsFalse(patientNameFeatureManager.IsAutoPopulatePatientName_Enabled(account));
        }
      
        [Test]
        public void TestIsAutoPopulateNameFeatureEnabled_WhenActivityIsShortRegistration_FacilityEnabled_ShouldReturnFalse()
        {
            Account account = GetAccount(new ShortRegistrationActivity(), true);
            var patientNameFeatureManager = new PatientNameFeatureManager();
            Assert.IsFalse(patientNameFeatureManager.IsAutoPopulatePatientName_Enabled(account));
        }

        [Test]
        public void
            TestIsAutoPopulateNameFeatureEnabled_WhenActivityIsPreAdmiNewborn_FacilityEnabled_ShouldReturnTrue()
        {
            Account account = GetAccount(new PreAdmitNewbornActivity(), true);
            var patientNameFeatureManager = new PatientNameFeatureManager();
            Assert.IsTrue(patientNameFeatureManager.IsAutoPopulatePatientName_Enabled(account));
        }
        [Test]
        public void
            TestIsAutoPopulateNameFeatureEnabled_WhenActivityIsPreAdmiNewborn_FacilityNotEnabled_ShouldReturnTrue()
        {
            Account account = GetAccount(new PreAdmitNewbornActivity(), false);
            var patientNameFeatureManager = new PatientNameFeatureManager();
            Assert.IsFalse(patientNameFeatureManager.IsAutoPopulatePatientName_Enabled(account));
        }

        [Test]
        public void TestIsAutoPopulateNameFeatureEnabled_EditPreAdmit_FacilityEnabled_ShouldReturnTrue()
        {
            var EditPreAdmitNewBornActivity =
                new MaintenanceActivity {AssociatedActivityType = typeof (PreAdmitNewbornActivity)};
            var account = GetAccount(EditPreAdmitNewBornActivity, true);
            var patientNameFeatureManager = new PatientNameFeatureManager();
            Assert.IsTrue(patientNameFeatureManager.IsAutoPopulatePatientName_Enabled(account));
        }

        [Test]
        public void TestIsAutoPopulateNameFeatureEnabled_EditPreAdmit_FacilityNotEnabled_ShouldReturnFasle()
        {
            var EditPreAdmitNewBornActivity =
                new MaintenanceActivity {AssociatedActivityType = typeof(PreAdmitNewbornActivity)};
            var account = GetAccount(EditPreAdmitNewBornActivity, false);
            var patientNameFeatureManager = new PatientNameFeatureManager();
            Assert.IsFalse(patientNameFeatureManager.IsAutoPopulatePatientName_Enabled(account));
        }

        #region Support Methods

        private static Account GetAccount(Activity activity, bool featureEnabledForFacility)
        {
            var facility = new Facility(PersistentModel.NEW_OID,
                PersistentModel.NEW_VERSION,
                "DOCTORS HOSPITAL DALLAS",
                "DHF");
            if (featureEnabledForFacility)
            {
                facility["IsBaylorFacility"] = null;
            }
            else
            {
                facility["IsBaylorFacility"] = true;
            }

            return new Account
            {
                Facility = facility,
                Activity = activity
            };
        }

        #endregion

        #region Constants

        #endregion
    }
}
