using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for ModeOfArrivalTests.
    /// </summary>

    //TODO: Create XML summary comment for ModeOfArrivalTests
    [TestFixture()]

    public class ModeOfArrivalPBARBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown ModeOfArrivalTests
        [TestFixtureSetUp()]
        public static void SetUpModeOfArrivalTests()
        {
            i_ModeOfArrivalBroker = BrokerFactory.BrokerOfType<IModeOfArrivalBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownModeOfArrivalTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestModesOfArrival()
        {
            ArrayList modes = i_ModeOfArrivalBroker.ModesOfArrivalFor( ACO_FACILITYID );

            Assert.IsNotNull( modes, "No modes list found" );
            Assert.IsTrue( modes.Count > 0, "No modes found in list" );
        }

        [Test()]
        public void TestSpecificMode()
        {
            ModeOfArrival mode = i_ModeOfArrivalBroker.ModeOfArrivalWith( ACO_FACILITYID, "F" );
            Assert.IsNotNull( mode, "No mode found for 'F'" );
            Assert.AreEqual( "FUNERAL HOME", mode.Description, "Incorrect mode descrition found" );
        }

        [Test()]
        public void TestModesOfArrivalForBlank()
        {
            string blank = String.Empty;

            ModeOfArrival mode1 = i_ModeOfArrivalBroker.ModeOfArrivalWith( ACO_FACILITYID, blank );
            Assert.IsNotNull( mode1, "No Mode of Arrival found for blank code for FacilityOid-900" );
            Assert.AreEqual( blank, mode1.Code, "Mode of Arrival1 Code should be blank" );
            Assert.AreEqual( blank, mode1.Description, "Mode of Arrival1 Description should be blank" );

            ModeOfArrival mode2 = i_ModeOfArrivalBroker.ModeOfArrivalWith( 6, blank );
            Assert.IsNotNull( mode2, "No Mode of Arrival found for blank code for FacilityOid-6" );
            Assert.AreEqual( blank, mode2.Code, "Mode of Arrival2 Code should be blank" );
            Assert.AreEqual( blank, mode2.Description, "Mode of Arrival2 Description should be blank" );

            ModeOfArrival mode3 = i_ModeOfArrivalBroker.ModeOfArrivalWith( 98, blank );
            Assert.IsNotNull( mode3, "No Mode of Arrival found for blank code for FacilityOid-98" );
            Assert.AreEqual( blank, mode3.Code, "Mode of Arrival3 Code should be blank" );
            Assert.AreEqual( blank, mode3.Description, "Mode of Arrival3 Description should be blank" );
        }

        [Test()]
        public void TestIsInValidModeOfArrival()
        {
            string inValidCode = "DZ";
            ModeOfArrival mode1 = i_ModeOfArrivalBroker.ModeOfArrivalWith( ACO_FACILITYID, inValidCode );
            Assert.IsFalse( mode1.IsValid, "Should be invalid." );
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static  IModeOfArrivalBroker i_ModeOfArrivalBroker;
        #endregion
    }
}