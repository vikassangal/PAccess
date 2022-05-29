using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for CodedDiagnosisBrokerTests.
    /// </summary>

    //TODO: Create XML summary comment for CodedDiagnosisBrokerTests
    [TestFixture()]
    public class CodedDiagnosisPBARBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown CodedDiagnosisBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpCodedDiagnosisBrokerTests()
        {
            IFacilityBroker fb = BrokerFactory.BrokerOfType<IFacilityBroker>();
            facility = fb.FacilityWith( ACO_FACILITYID );

            cdBroker = BrokerFactory.BrokerOfType<ICodedDiagnosisBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownCodedDiagnosisBrokerTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestGetDiagnosis()
        {
            CodedDiagnoses cd = cdBroker.CodedDiagnosisFor( ACO_FACILITYID, 30486, 785147, false, facility );
            Assert.IsNotNull(cd, "Failed to load Coded Diagnosis object");
            Assert.IsTrue(cd.CodedDiagnosises.Count == 8, "Wrong number of Coded Diagnosis found");
            Assert.IsTrue(cd.AdmittingCodedDiagnosises.Count == 5, "Wrong number of Admitting Diagnosis found");
            Assert.AreEqual("789.07",(string)cd.CodedDiagnosises[0]);
            Assert.AreEqual("789.07",(string)cd.AdmittingCodedDiagnosises[0]);
        }

        [Test()]
        public void TestPreMSEDiagnosisList()
        {
            CodedDiagnoses cd = cdBroker.CodedDiagnosisFor( ACO_FACILITYID, 31153, 785220, true, facility );
            Assert.IsNotNull(cd, "Failed to load Coded Diagnosis object");
            Assert.IsTrue(cd.CodedDiagnosises.Count == 8, "Wrong number of Coded Diagnosis found");
            Assert.IsTrue(cd.AdmittingCodedDiagnosises.Count == 5, "Wrong number of Admitting Diagnosis found");
            Assert.AreEqual("222.0",(string)cd.CodedDiagnosises[0]);
            Assert.AreEqual("585",(string)cd.AdmittingCodedDiagnosises[0]);
        }

        [Test()]
        public void TestNoData()
        {
            CodedDiagnoses cd = cdBroker.CodedDiagnosisFor( ACO_FACILITYID, 99999, 99999, true, facility );
            Assert.IsNotNull(cd, "Failed to load Coded Diagnosis list");
            Assert.IsTrue(cd.CodedDiagnosises.Count == 8, "Wrong number of Coded Diagnosis found");
            Assert.IsTrue(cd.AdmittingCodedDiagnosises.Count == 5, "Wrong number of Admitting Diagnosis found");
            Assert.AreEqual(string.Empty,(string)cd.AdmittingCodedDiagnosises[0]);
            Assert.AreEqual(string.Empty,(string)cd.CodedDiagnosises[0]);
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static  Facility facility = null;
        private static  ICodedDiagnosisBroker cdBroker = null;
        #endregion
    }
}