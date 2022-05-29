using System.Collections;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class HospitalServiceTests
    {
        #region	Constants

        HospitalService psychHSV = new HospitalService
        {
            Code = HospitalService.PSYCH_NON_LOCKED,
            Description = "PSYCH_NON_LOCKED"
        };

        HospitalService nonPsychHSV = new HospitalService
        {
            Code = HospitalService.HSV57,
            Description = "OUTPT_IN_BED_NON_OBS"
        };
        #endregion

        #region	SetUp and TearDown HospitalServiceTests
        [TestFixtureSetUp()]
        public static void SetUpHospitalServiceTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownHospitalServiceTests()
        {
        }
        #endregion

        #region	Test Methods
        [Test()]
        public void testHospitalService()
        {
            HospitalService service1 = new HospitalService( 1, ReferenceValue.NEW_VERSION, "TestHospital Serviece", "58" );

            Assert.AreEqual(
                typeof( HospitalService ),
                service1.GetType()
                );

            Assert.AreEqual(
                "TestHospital Serviece",
                service1.Description
                );
            Assert.AreEqual(
                "58",
                service1.Code
                );
            Assert.AreEqual(
                1,
                service1.Oid
                );
            service1.FacilityOid = 900;
            service1.OPFlag = "Y";
            service1.IPTransferRestriction = "Y";
            service1.DayCare = "M";

            HospitalService service2 = new HospitalService( 2, ReferenceValue.NEW_VERSION, "BLOOD TRANSFUSION", "43" );
            HospitalService service3 = new HospitalService( 3, ReferenceValue.NEW_VERSION, "BLOOD TRANSFUSION", "54" );
            HospitalService service4 = new HospitalService( 4, ReferenceValue.NEW_VERSION, "BLOOD TRANSFUSION", "63" );

            service2.FacilityOid = 6;
            service2.OPFlag = "Y";
            service2.IPTransferRestriction = "Y";

            service3.FacilityOid = 900;
            service3.OPFlag = "M";
            service3.IPTransferRestriction = "Y";

            service4.FacilityOid = 6;
            service4.OPFlag = "Y";
            service4.IPTransferRestriction = "Y";

            ArrayList allHospitalServices = new ArrayList();
            allHospitalServices.Add( service1 );
            allHospitalServices.Add( service2 );
            allHospitalServices.Add( service3 );
            allHospitalServices.Add( service4 );
            Assert.AreEqual( 4,
                             allHospitalServices.Count
                );
            Assert.IsTrue( allHospitalServices.Contains( service2 ) );

            Assert.IsTrue( HospitalService.CheckForBedAssignment( "58" ) );
            ICollection hsvs = service1.HospitalServicesFor( allHospitalServices, VisitType.INPATIENT );
            Assert.AreEqual( 4,
                             allHospitalServices.Count
                );
            Assert.IsTrue( hsvs.Count > 0 );
            hsvs = service1.HospitalServicesFor( allHospitalServices, VisitType.EMERGENCY_PATIENT, "M" );

            Assert.IsTrue( hsvs.Count == 1 );
        }

        public void testHospitalService_CheckBedAssignment_ForOH_()
        {
            HospitalService service1 = new HospitalService(1, ReferenceValue.NEW_VERSION, "Organ Harvesting", "OH");
            Assert.IsTrue(HospitalService.CheckForBedAssignment(service1.Code));
        }

        [Test()]
        public void testHospitalServiceForClient1()
        {
            HospitalService service1 = new HospitalService( 1, ReferenceValue.NEW_VERSION, "BLOOD TRANSFUSION", "43" );
            HospitalService client1 = new HospitalService( 5, ReferenceValue.NEW_VERSION, "CLIENT1", "C1" );
            HospitalService client2 = new HospitalService( 6, ReferenceValue.NEW_VERSION, "CLIENT2", "C2" );

            client1.FacilityOid = 900;
            client1.OPFlag = "Y";
            client1.IPTransferRestriction = "Y";

            client2.FacilityOid = 900;
            client2.OPFlag = "Y";
            client2.IPTransferRestriction = "Y";

            ArrayList allHospitalServices = new ArrayList();

            allHospitalServices.Add( client1 );
            allHospitalServices.Add( client2 );

            ArrayList hsvs1 = (ArrayList)service1.HospitalServicesFor( allHospitalServices, VisitType.INPATIENT );
            Assert.IsFalse( hsvs1.Contains( client1 ) );
            Assert.IsFalse( hsvs1.Contains( client2 ) );

            ArrayList hsvs2 = (ArrayList)service1.HospitalServicesFor( allHospitalServices, VisitType.OUTPATIENT );
            Assert.IsTrue( hsvs2.Count > 0 );
            Assert.IsTrue( hsvs2.Contains( client1 ) );
            Assert.IsTrue( hsvs2.Contains( client2 ) );

            ArrayList hsvs3 = (ArrayList)service1.HospitalServicesFor( allHospitalServices, VisitType.EMERGENCY_PATIENT );
            Assert.IsTrue( hsvs3.Count > 0 );
            Assert.IsTrue( hsvs3.Contains( client1 ) );
            Assert.IsTrue( hsvs3.Contains( client2 ) );

            ArrayList hsvs4 = (ArrayList)service1.HospitalServicesFor( allHospitalServices, VisitType.RECURRING_PATIENT );
            Assert.IsTrue( hsvs4.Count > 0 );
            Assert.IsTrue( hsvs4.Contains( client1 ) );
            Assert.IsTrue( hsvs4.Contains( client2 ) );

            ArrayList hsvs5 = (ArrayList)service1.HospitalServicesFor( allHospitalServices, VisitType.NON_PATIENT );
            Assert.IsTrue( hsvs5.Count > 0 );
            Assert.IsTrue( hsvs5.Contains( client1 ) );
            Assert.IsTrue( hsvs5.Contains( client2 ) );
        }

        [Test]
        public void TestHsv57IsDayCare()
        {
            HospitalService hsv57 = new HospitalService
            {
                Code = HospitalService.HSV57,
                Description = "OUTPT IN BED NON OBS"
            };

            Assert.AreEqual( typeof( HospitalService ), hsv57.GetType() );
            Assert.AreEqual( HospitalService.HSV57, hsv57.Code );
            Assert.IsTrue( hsv57.IsDayCare() );
        }

        [Test]
        public void TestHSV_OH_IsDayCare()
        {
            var hsvOH = new HospitalService
            {
                Code = HospitalService.ORGAN_HARVESTING,
                Description = "Organ Harvesting"
            };

            Assert.IsTrue( hsvOH.IsDayCare() );
        }

        [Test]
        public void TestHSVisPsychCode()
        {
            Assert.AreEqual(HospitalService.PSYCH_NON_LOCKED, psychHSV.Code);
            Assert.IsTrue(psychHSV.IsPsychCode());
        }

        [Test]
        public void TestHSVisNonPsychCode()
        {
            Assert.AreEqual(HospitalService.HSV57, nonPsychHSV.Code);
            Assert.IsFalse(nonPsychHSV.IsPsychCode());
        }

       #endregion

        #region	Support	Methods
        #endregion

        #region	Data Elements
        #endregion
    }
}