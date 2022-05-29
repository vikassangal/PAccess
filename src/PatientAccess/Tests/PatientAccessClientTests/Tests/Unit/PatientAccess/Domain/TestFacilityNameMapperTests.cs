using NUnit.Framework;
using PatientAccess.Services.EMPIService;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class TestFacilityNameMapperTests 
    {

        #region Constants

        private const string DHF = "DHF";
        private const string DXX = "DHF";
        private const string PRV = "PRV";
        private const string PYY = "PRV";
        private const string SIE = "SIE";
        private const string SES = "SES";
       #endregion Constants

        #region Test Methods
        [Test()]
        public void TestGetEMPIFacilityCode_PBARFacilityIsDHF_EMPIFacilityShouldbeDXX()
        {
            var mapper = new TestFacilityNameMapper();
            var empifacility = mapper.GetEMPIFacilityCode(DHF);
            Assert.AreEqual(empifacility, DXX);
        }
        [Test()]
        public void TestGetEMPIFacilityCode_PBARFacilityIsPRV_EMPIFacilityShouldbePYY()
        {
            var mapper = new TestFacilityNameMapper();
            var empifacility = mapper.GetEMPIFacilityCode(PRV);
            Assert.AreEqual(empifacility, PYY);
        }
        [Test()]
        public void TestGetEMPIFacilityCode_PBARFacilityIsSIE_EMPIFacilityShouldbeSIE()
        {
            var mapper = new TestFacilityNameMapper();
            var empifacility = mapper.GetEMPIFacilityCode(SIE);
            Assert.AreEqual(empifacility, SIE);
        }
        [Test()]
        public void TestGetEMPIFacilityCode_PBARFacilityIsSES_EMPIFacilityShouldbeSES()
        {
            var mapper = new TestFacilityNameMapper();
            var empifacility = mapper.GetEMPIFacilityCode(SES);
            Assert.AreEqual(empifacility, SES);
        }
        #endregion
    }
}