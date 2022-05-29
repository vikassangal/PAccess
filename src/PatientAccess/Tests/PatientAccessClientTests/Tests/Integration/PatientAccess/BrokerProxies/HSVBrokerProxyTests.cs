using System.Collections;
using System.Linq;
using NUnit.Framework;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;

namespace Tests.Integration.PatientAccess.BrokerProxies
{
    /// <summary>
    /// 
    /// IList SelectHospitalServicesFor( long facilityNumber );
    /// ICollection HospitalServicesFor( long facilityNumber, string patientTypeCode );
    /// ICollection HospitalServicesFor( long facilityNumber, string patientTypeCode, string dayCare );
    /// 
    /// </summary>

    [TestFixture()]
    public class HSVBrokerProxyTests
    {
        #region SetUp and TearDown

        [TestFixtureSetUp()]
        public static void SetUpHSVBrokerProxyTests()
        {
            i_BrokerProxy = new HSVBrokerProxy();
        }

        [TestFixtureTearDown()]
        public static void TearDownAdmitSourceBrokerProxyTests()
        {
        }

        #region Data Elements

        private static HSVBrokerProxy i_BrokerProxy = null;

        #endregion


        #endregion
		
        #region Tests

        [Test()]
        public void TestHospitalServicesFor()
        {
            ArrayList list = ( ArrayList ) i_BrokerProxy.HospitalServicesFor( 900, "2" );
            Assert.IsTrue( list.Count > 0, "List not null" );

            list = (ArrayList) i_BrokerProxy.HospitalServicesFor(900,"2","N");
            Assert.IsTrue( list.Count > 0, "List or HSVs is empty" );
        }

        [Test()]
        public void TestSelectHospitalServicesFor()
        {
            IList hsvList = i_BrokerProxy.SelectHospitalServicesFor( 900 );
            Assert.IsNotNull( 
                hsvList, "HospitalServices list retreived is not null" );
            Assert.IsTrue( 
                hsvList.Count > 0,
                " There are no results from the HospitalServices search" );
        }

        [Test()]
        public void TestHospitalServicesFor_WhenPT2ShortEditMaintain_ShouldNotIncludeHSV87()
        {
             var hospitalService = new HospitalService(1, ReferenceValue.NEW_VERSION, "ACUTE CARE CLINIC", "06")
                 {
                     DayCare = "N",
                     OPFlag = "Y"
                 };

            var financialClass = new FinancialClass(1, ReferenceValue.NEW_VERSION,"COMM IST,MNGD 2ND","21");
            var visitType = new VisitType { Code = VisitType.OUTPATIENT };

            var hsvList = i_BrokerProxy.HospitalServicesFor( DHF_FACILITY_ID,
                                                            visitType,
                                                            new ShortMaintenanceActivity(),
                                                            hospitalService,
                                                            financialClass).Cast<HospitalService>().ToList();

            bool containsHsv87 = hsvList.Any( x => x.Code == HSVCode87 );

            Assert.IsTrue( hsvList.Any(), "List of HSVs is empty" );

            Assert.IsFalse( containsHsv87, "Hospital service list should not include HSV 87" );
        }
        [Test()]
        public void TestHospitalServicesFor_WhenPT2ShortRegistration_ShouldNotIncludeHSVOH()
        {
            var hospitalService = new HospitalService(1, ReferenceValue.NEW_VERSION, "Organ Harvesting", "OH")
            {
                DayCare = "Y",
                OPFlag = "Y"
            };

            var financialClass = new FinancialClass(1, ReferenceValue.NEW_VERSION, "COMM IST,MNGD 2ND", "21");
            var visitType = new VisitType { Code = VisitType.OUTPATIENT };

            var hsvList = i_BrokerProxy.HospitalServicesFor(DHF_FACILITY_ID,
                                                            visitType,
                                                            new ShortRegistrationActivity(), 
                                                            hospitalService,
                                                            financialClass).Cast<HospitalService>().ToList();

            bool containsHsvOH = hsvList.Any(x => x.Code == HospitalService.ORGAN_HARVESTING);

            Assert.IsTrue(hsvList.Any(), "List of HSVs is empty");

            Assert.IsFalse(containsHsvOH, "Hospital service list should not include HSV OH");
        }
        
        #endregion	
    
        #region Constants

        public const long DHF_FACILITY_ID = 54;
        public const string PATIENT_TYPE_2 = "2";
        public const string DAY_CARE = "N";
        public const string HSVCode87 = "87";

        #endregion
    }
}