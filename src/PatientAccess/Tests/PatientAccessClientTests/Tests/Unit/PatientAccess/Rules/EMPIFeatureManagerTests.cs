using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for EMPIFeatureManagerTests
    /// </summary>
    [TestFixture]
    [Category("Fast")]
    public class EMPIFeatureManagerTests
    {
        #region Test Methods

        [Test]
        public void IsEMPIFeatureEnabled_WhenActivity_Registration_ShouldReturnTrue()
        {
            var empiFeatureManager = GetEmpiFeatureManager();
            var EMPIEnabled = empiFeatureManager.IsEMPIFeatureEnabled(new RegistrationActivity());
            Assert.IsTrue(EMPIEnabled);
        }

        [Test]
        public void IsEMPIFeatureEnabled_WhenActivity_EditMaintain_ShouldReturnFalse()
        {
            var empiFeatureManager = GetEmpiFeatureManager();
            var EMPIEnabled = empiFeatureManager.IsEMPIFeatureEnabled(new MaintenanceActivity());
            Assert.IsFalse(EMPIEnabled);
        }

        [Test]
        public void IsEMPIFeatureEnabled_WhenActivity_PreRegistration_ShouldReturnTrue()
        {
            var empiFeatureManager = GetEmpiFeatureManager();
            var EMPIEnabled = empiFeatureManager.IsEMPIFeatureEnabled(new PreRegistrationActivity());
            Assert.IsTrue(EMPIEnabled);
        }

        #endregion

        #region Support Methods

        private EMPIFeatureManager GetEmpiFeatureManager()
        {
            var facility = GetICEFacility();
            var empiFeatureManager = new EMPIFeatureManager(facility);
            return empiFeatureManager;
        }

        private Facility GetICEFacility()
        {
            var facility = new Facility(PersistentModel.NEW_OID,
                                        PersistentModel.NEW_VERSION,
                                        "ICE",
                                        "ICE");
            facility["EMPIEnabled"] = true;
            return facility;
        }

        #endregion

    }
}