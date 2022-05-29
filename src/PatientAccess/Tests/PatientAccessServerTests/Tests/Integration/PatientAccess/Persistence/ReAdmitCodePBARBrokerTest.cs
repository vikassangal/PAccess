using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// ReAdmitCodeBrokerTest
    /// </summary>
        
    [TestFixture()]
    public class ReAdmitCodePBARBrokerTest : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown ReAdmitCodeBrokerTest
        [TestFixtureSetUp()]
        public static void SetUpReAdmitCodeBrokerTest()
        {
            i_ReAdmitCodeBroker = BrokerFactory.BrokerOfType<IReAdmitCodeBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownReAdmitCodeBrokerTest()
        {
           
        }
        #endregion

        #region Test Methods
    
        [Test()]
        public void TestReAdmitCodesFor()
        {
            ArrayList reAdmitCodes = (ArrayList)i_ReAdmitCodeBroker.ReAdmitCodesFor( ACO_FACILITYID );
   
            Assert.IsTrue( reAdmitCodes.Count > 0, "No code found" );
        }

        [Test()]
        public void TestReAdmitCodeWith()
        {
            ReAdmitCode obj = (ReAdmitCode)i_ReAdmitCodeBroker.ReAdmitCodeWith( ACO_FACILITYID, "2" );
   
            Assert.AreEqual(
                "WITHIN 24 HRS",
                obj.Description,
                "WITHIN 24 HRS" );
        }

        [Test()]
        public void TestReAdmitCodesForBlank()
        {            
            string blank = string.Empty;

            ReAdmitCode rac1 = i_ReAdmitCodeBroker.ReAdmitCodeWith( ACO_FACILITYID, blank );
            Assert.AreEqual( blank, rac1.Code, "ReAdmitCode1 should be blank" );
            Assert.AreEqual( blank, rac1.Description, "ReAdmitCode1 Description should be blank" );
            Assert.IsTrue( rac1.IsValid );
        }

        [Test()]
        public void TestIsInvalidReAdmitCode()
        {
            string invalidCode = "DZ";

            ReAdmitCode rac1 = i_ReAdmitCodeBroker.ReAdmitCodeWith( ACO_FACILITYID, invalidCode );
            Assert.IsFalse( rac1.IsValid, "Should be invalid." );
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        
        private static IReAdmitCodeBroker i_ReAdmitCodeBroker = null;
        
        #endregion

        #region Constants
        #endregion
    }
}