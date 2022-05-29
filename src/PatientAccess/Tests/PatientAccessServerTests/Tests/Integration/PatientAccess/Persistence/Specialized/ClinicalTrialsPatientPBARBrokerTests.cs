using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.Specialized;
using PatientAccess.Persistence.Specialized;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence.Specialized
{
    /// <summary>
    ///This is a test class for ClinicalTrialsPatientPBARBrokerTests and is intended
    ///to contain all ClinicalTrialsPatientPBARBrokerTests Unit Tests
    ///</summary>
    [TestFixture]
    public class ClinicalTrialsPatientPBARBrokerTests : PatientPBARBrokerTests
    {

        private ClinicalTrialsPatientPBARBroker UnitUnderTest { get; set; }
        
        [SetUp]
        public void Initialize()
        {
            this.UnitUnderTest = new ClinicalTrialsPatientPBARBroker();
        }

        [Test]
        public void TestSparsePatientWithExtendedField()
        {
            Facility facility = this.FacilityBroker.FacilityWith(900);
            Patient patient = this.UnitUnderTest.SparsePatientWith(785138, facility.Code);
            Assert.IsTrue( patient.HasExtendedProperty( ClinicalTrialsConstants.KEY_IS_ACCOUNT_ELIGIBLE_FOR_CLINICAL_TRIALS ) );
        }

        /// <summary>
        /// Tests the sparse patient with no patient should not throw.
        /// </summary>
        /// <remarks>
        /// Bug-2383 - Offline modes send null patients
        /// </remarks>
        [Test]        
        public void TestSparsePatientWithNoPatientShouldNotThrow()
        {
            Facility facility = this.FacilityBroker.FacilityWith(900);
            Patient patient = this.UnitUnderTest.SparsePatientWith(-99999, facility.Code);
        }

        /// <summary>
        ///A test for PatientFrom
        ///</summary>
        [Test]
        [Ignore]
        public void TestPatientFromWithExtendedField()
        {
            // TODO: Write Me
        }


    }
}