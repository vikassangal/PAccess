using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class ScheduleCodePBARBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown ScheduleCodeBrokerTest
        [TestFixtureSetUp()]
        public static void SetUpScheduleCodeBrokerTest()
        {
            i_ScheduleCodeBroker = BrokerFactory.BrokerOfType<IScheduleCodeBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownScheduleCodeBrokerTest()
        {
        }
        #endregion

        #region Test Methods
    
        [Test()]
        public void TestAllScheduleCodes()
        {
            ArrayList scheduleCodes = (ArrayList)i_ScheduleCodeBroker.AllScheduleCodes( ACO_FACILITYID );
   
            Assert.IsTrue( scheduleCodes.Count > 0, "No schedule code found" );
            Assert.AreEqual(
                "ADD-ON LESS THAN 24 HOURS",
                ((ScheduleCode)scheduleCodes[1]).Description,
                "should be ADD-ON LESS THAN 24 HOURS" );
        }

        [Test()]
        public void TestScheduleCodeWith()
        {
            ScheduleCode scheduleCode = i_ScheduleCodeBroker.ScheduleCodeWith( ACO_FACILITYID, "S" );
   
            Assert.AreEqual(
                "SCHEDULED 24 HOURS BEFORE",
                scheduleCode.Description,
                "the description should be SCHEDULED 24 HOURS BEFORE" );
        }

        [Test()]
        public void TestScheduleCodeWithBlank()
        {            
            string blank = string.Empty ;
            ScheduleCode scheduleCode = i_ScheduleCodeBroker.ScheduleCodeWith( ACO_FACILITYID, blank );
   
            Assert.AreEqual(
                blank,
                scheduleCode.Description.Trim(),
                "the description should be blank" );
            Assert.IsTrue(scheduleCode.IsValid,
                          "scheduleCode should be valid" );
        }

        [Test()]
        public void TestIsInValidScheduleCode()
        {
            string invalidCode = "Z";
            ScheduleCode scheduleCode = i_ScheduleCodeBroker.ScheduleCodeWith( ACO_FACILITYID, invalidCode );
            Assert.IsFalse( scheduleCode.IsValid, "scheduleCode should be invalid" );
        }

        [Test(), ExpectedException( typeof( ArgumentNullException ) )]
        public void TestScheduleCodeForNULL()
        {
            string invalid = null;
            ScheduleCode scheduleCode = i_ScheduleCodeBroker.ScheduleCodeWith( ACO_FACILITYID, invalid );
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        
        private static IScheduleCodeBroker i_ScheduleCodeBroker = null;  
        
        #endregion

        #region Constants
      
        #endregion
    }
}