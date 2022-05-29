using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for InsuranceBrokerTests.
    /// </summary>

    [TestFixture()]
    public class HsvBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown hsvBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpHSVBrokerTests()
        {
            hsvBroker = BrokerFactory.BrokerOfType<IHSVBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownHSVBrokerTests()
        {
            
        }
        #endregion

        #region Test Methods

        [Test()]
        public void TestHospitalServicesFor()
        {
            ArrayList list = (ArrayList)hsvBroker.HospitalServicesFor( ACO_FACILITYID, "2" );
            Assert.IsTrue( list.Count > 0, "List not null" );

            list = (ArrayList)hsvBroker.HospitalServicesFor( ACO_FACILITYID,
                                                             "2", "N" );
            Assert.IsTrue( list.Count > 0, "List or HSVs is empty" );
        }

        [Test()]
        public void TestHospitalServicesWith()
        {
            HospitalService hsv = hsvBroker.HospitalServiceWith( ACO_FACILITYID, "36" );
            Assert.IsNotNull( hsv, "HospitalService object retreived is null" );
        }

        [Test()]
        //        [Ignore("OID Test - SKIP")]
        public void TestHospitalServiceForBlank()
        {
            string blank = String.Empty;
            HospitalService hs = hsvBroker.HospitalServiceWith( ACO_FACILITYID, blank );


            Assert.IsTrue(
                hs.IsValid
                );
        }
        [Test()]
        public void TestSelectHospitalServicesFor()
        {
            IList hsvList = hsvBroker.SelectHospitalServicesFor( ACO_FACILITYID );
            Assert.IsNotNull(
                hsvList, "HospitalServices list retreived is not null" );
            Assert.IsTrue(
                hsvList.Count > 0,
                " There are no results from the HospitalServices search" );
        }


        [Test()]
        public void TestHospitalServicesInvalidCodeWith()
        {
            HospitalService hsv = hsvBroker.HospitalServiceWith( ACO_FACILITYID, "KK" );
            Assert.IsFalse( hsv.IsValid, "HospitalService object retreived is valid" );
        }

 
       
        #endregion

        #region Data Elements
        private static  IHSVBroker hsvBroker;
     
        #endregion
    }
}