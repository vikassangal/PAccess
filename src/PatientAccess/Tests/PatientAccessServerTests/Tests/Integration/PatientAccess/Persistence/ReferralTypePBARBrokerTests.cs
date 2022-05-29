using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class ReferralTypePBARBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown ReferralTypePBARBrokerTest
        [TestFixtureSetUp()]
        public static void SetUpReferralTypePBARBrokerTest()
        {
            i_Broker = BrokerFactory.BrokerOfType<IReferralTypeBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownReferralTypePBARBrokerTest()
        {  
        }
        #endregion

        #region Test Methods
    
        [Test()]
        public void TestReferralTypesFor()
        {
            ArrayList referralTypes = (ArrayList)i_Broker.ReferralTypesFor( ACO_FACILITYID );
   
            Assert.IsTrue( referralTypes.Count > 0, "No code found" );
           
        }

        [Test()]
        public void TestReferralTypeWith()
        {            
            ReferralType obj = (ReferralType)i_Broker.ReferralTypeWith( ACO_FACILITYID, "D" );
   
            Assert.AreEqual(
                "DEPT SOCIAL SERVICES",
                obj.Description,
                "DEPT SOCIAL SERVICES" );
        }

        [Test()]
        public void TestReferralTypeForInvalid()
        {
            string invalidCode = "Q";
            ReferralType referralType = (ReferralType)i_Broker.ReferralTypeWith( ACO_FACILITYID, invalidCode );

            Assert.AreEqual(
                invalidCode,
                referralType.Code,
                "Code should be Q" );
            Assert.AreEqual
                ( "Q",
                  referralType.Description,
                  "Description should be 'Q'"
                );
            Assert.IsFalse(
                referralType.IsValid, "Should not be a valid Referral Type code"
                );
        }

        [Test(), ExpectedException( typeof( ArgumentNullException ) )]
        public void TestOccurrenceCodeForNull()
        {
            string invalid = null;
            ReferralType referralType = (ReferralType)i_Broker.ReferralTypeWith( ACO_FACILITYID, invalid );
        }

        [Test()]
        public void TestReferralTypeWithBlank()
        {            
            string blank = String.Empty ;
            ReferralType obj = (ReferralType)i_Broker.ReferralTypeWith( ACO_FACILITYID, blank);
   
            Assert.AreEqual(
                blank,
                obj.Description,
                "Description should be blank");
            Assert.IsTrue(                
                obj.IsValid,
                "ReferralType should be valid");
        
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        
        private static  IReferralTypeBroker i_Broker = null;
        
        #endregion

        #region Constants
        #endregion
    }
}