using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class HospitalClinicTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown HospitalClinicTests
        [TestFixtureSetUp()]
        public static void SetUpHospitalClinicTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownHospitalClinicTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testHospitalClinic()
        {
           
            HospitalClinic clinic = new HospitalClinic(3,ReferenceValue.NEW_VERSION,
                                                       "BLOOD TRANSFUSION","AL", string.Empty, "01");
            clinic.Name =  "BANG KARNATAKA" ;
            Assert.AreEqual(
                typeof(HospitalClinic),
                clinic.GetType()
                );
                 
            Assert.AreEqual(
                "BLOOD TRANSFUSION",
                clinic.Description
                );
            Assert.AreEqual(
                "AL",
                clinic.Code
                );
            Assert.AreEqual(
                3,
                clinic.Oid
                );
            Assert.AreEqual(
                "BANG KARNATAKA",
                clinic.Name
                );
            Assert.AreEqual(
                "01",
                clinic.SiteCode);
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}