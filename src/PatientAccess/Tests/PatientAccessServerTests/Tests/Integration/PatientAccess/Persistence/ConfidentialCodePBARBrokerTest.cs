using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for ConfidentialCodePBARBrokerTest.
    /// </summary>

    //TODO: Create XML summary comment for ConfidentialCodePBARBrokerTests
    [TestFixture()]
    public class ConfidentialCodePBARBrokerTest : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown ConfidentialCodeBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpConfidentialCodeBrokerTest()
        {
            i_ConfidentialCodeBroker = BrokerFactory.BrokerOfType<IConfidentialCodeBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownConfidentialCodeBrokerTest()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestConfidentialCodesFor()
        {
            IList codes = i_ConfidentialCodeBroker.ConfidentialCodesFor( ACO_FACILITYID ) ;
            Assert.IsNotNull(codes, "No ConfidentialCodes returned for hospital # {0}", ACO_FACILITYID ) ;
        }
            
            
        [Test()]
        public void TestConfidentialCodeWith() 
        {
            ConfidentialCode code = i_ConfidentialCodeBroker.ConfidentialCodeWith( ACO_FACILITYID, "C" ) ;
            Assert.IsNotNull( code, "No Confidential Code returned for hospital # 900 and key='C' ");
            Assert.IsTrue( code.Description == "CONFIDENTIAL", "Wrong Confidential Code Description returned for hospital # 900 and key='C' ");
            Assert.IsTrue( code.IsValid);   
        }

        [Test()]
        public void TestConfidentialCodesWithBlank()
        {            
            string blank = String.Empty;
            ConfidentialCode confidentialCode = i_ConfidentialCodeBroker.ConfidentialCodeWith( ACO_FACILITYID, blank ) ;

            Assert.AreEqual(
                blank,
                confidentialCode.Code,
                "Code should be blank");
             
            Assert.IsTrue(
                confidentialCode.IsValid            
                );
        }

        [Test()]
        public void TestConfidentialCodesWithInvalid()
        {            
            string InvalidCode = "A";
            ConfidentialCode confidentialCode = i_ConfidentialCodeBroker.ConfidentialCodeWith( ACO_FACILITYID, InvalidCode ) ;

            Assert.AreEqual(
                InvalidCode,
                confidentialCode.Code,
                "Code should be A ");
            Assert.AreEqual
                (InvalidCode,
                 confidentialCode.Description,
                 "Description should be Invalid"
                );
            Assert.IsFalse(
                confidentialCode.IsValid            
                );
        }

        [Test()]
        [ExpectedException( typeof( ArgumentNullException ) ) ]
        public void TestConfidentialCodesWithNull()
        {
            ConfidentialCode confidentialCode = i_ConfidentialCodeBroker.ConfidentialCodeWith( ACO_FACILITYID, null ) ;
        }

        [Test()]
        [ExpectedException( typeof( BrokerException ) ) ]
        public void TestBadFacility()
        {
            ConfidentialCode code1 = i_ConfidentialCodeBroker.ConfidentialCodeWith( INVALID_FACILITY_ID, "SS" ) ;
            Assert.AreEqual( false, code1.IsValid, "Code SS at facility {0} should not be valid.", INVALID_FACILITY_ID ) ;
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static IConfidentialCodeBroker i_ConfidentialCodeBroker;
        #endregion
    }
}