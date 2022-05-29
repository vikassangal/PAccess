using System;
using System.Configuration;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Persistence;
using PatientAccess.Persistence.Utilities;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence.Utilities
{
    [TestFixture()]
    public class IBMUtiltitesTests
    {
        #region Constants
        private const string EMPLOYER_CODE_DATA_AREA_NAME = "EMPLOYER_CODE_DATA_AREA_NAME",
                             EMPLOYER_CODE_LIB_NAME = "EMPLOYER_CODE_LIB_NAME";
        #endregion

        #region SetUp and TearDown IBMUtiltitesTests
        [TestFixtureSetUp()]
        public static void SetUpIBMUtiltitesTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownIBMUtiltitesTests()
        {
        }
        #endregion

        #region Test Methods

        [Test]
        public void TestIsDatabaseAvailableFor()
        {
            FacilityBroker facilityBroker = new FacilityBroker();

            Facility someFacility = facilityBroker.FacilityWith( "ACO" );

            var isDatabaseAvailable = facilityBroker.IsDatabaseAvailableFor( someFacility.ConnectionSpec.ServerIP );
           
            Assert.IsTrue(isDatabaseAvailable);
        }


        /// <summary>
        /// This is actually a negative test. A return of -1 indicates that the call failed
        ///  which is the proper return for all available facilities in dev/test.
        /// </summary>
        [Test()]
        public void TestGetUSCMRN()
        {
            try
            {
                IBMUtilities util = new IBMUtilities();
                long newMRN = util.GetNextUSCMRN(900);

                Assert.AreEqual(IBMUtilities.INVALIDMRN, newMRN, "Method should only return -1 in dev/test environments");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        [Test()]
        public void TestGetNextEmployerNumber()
        {
            string areaName = ConfigurationManager.AppSettings[EMPLOYER_CODE_DATA_AREA_NAME];
            string libName = ConfigurationManager.AppSettings[EMPLOYER_CODE_LIB_NAME];

            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            Facility facility = facilityBroker.FacilityWith(900);
            IBMUtilities util = new IBMUtilities(facility);
            int first = util.GetNextEmployerNumber(areaName, libName);
            int second = util.GetNextEmployerNumber(areaName, libName);

            Assert.IsTrue(second - 1 == first,
                           "Did not get sequencial employerNumbers");
        }

        [Test()]
        public void TestHRAMLock()
        {
            IFacilityBroker fb = BrokerFactory.BrokerOfType<IFacilityBroker>();
            Facility facility = fb.FacilityWith(900);

            IBMUtilities util = new IBMUtilities(facility);
            try
            {
                util.LockAccount(30049, "PACCESS", "SOMEWSID", facility);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            try
            {
                util.UnlockAccount(30049, "PACCESS", "SOMEWSID", facility);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}