using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class dischargeBrokerTest : AbstractBrokerTests
    {
        #region Constants
        #endregion        

        #region SetUp and TearDown DischargeBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpDischargeBrokerTests()
        {
            dischargeBroker = BrokerFactory.BrokerOfType<IDischargeBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownDischargeBrokerTests()
        {      
        }
        #endregion

        #region Test Methods

        [Test()]
        public void AllDispositions()
        {
            ArrayList dispositions = (ArrayList)dischargeBroker.AllDischargeDispositions( ACO_FACILITYID );
            Assert.IsNotNull( dispositions, "No dispositions returned");
            Assert.IsTrue( dispositions.Count > 1, "Incorrect dispositions returned");
        }

        [Test()]
        [ExpectedException( typeof( BrokerException ) ) ]
        public void AllDispositionsInvalidFacilityID()
        {
            ArrayList dispositions = (ArrayList)dischargeBroker.AllDischargeDispositions( INVALID_FACILITY_ID ) ;            
        }

        [Test()]
        public void TestDispositionCodeWith()
        {
            DischargeDisposition disp = dischargeBroker.DischargeDispositionWith( ACO_FACILITYID, "M" );
            Assert.IsNotNull(disp,"Did not find discharge disposition with code of 'M'");
            Assert.AreEqual("M",disp.Code,"Incorrect code for EXPIRED Discharge Disposition");
        }

        [Test()]
        [ExpectedException( typeof( ArgumentNullException ) ) ]
        public void TestDispositionCodeWithNull()
        {
            dischargeBroker.DischargeDispositionWith( ACO_FACILITYID, null  ) ;
        }

        [Test()]
        [ExpectedException( typeof( BrokerException ) ) ] 
        public void TestDispositionCodeWithInvalidFacilityID()
        {
            dischargeBroker.DischargeDispositionWith( INVALID_FACILITY_ID, "M" ) ;
        }
        
        [Test()]
        public void TestDispositionCodeForBlank()
        {            
            string blank = String.Empty;

            DischargeDisposition disp = dischargeBroker.DischargeDispositionWith( ACO_FACILITYID,blank );

            Assert.AreEqual(
                blank,
                disp.Code,
                "Code  should be blank");

            Assert.AreEqual
                (blank,
                 disp.Description,
                 "Description should be blank"
                );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static  IDischargeBroker dischargeBroker;
        #endregion
    }
}