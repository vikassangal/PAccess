using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for ConditionOfServiceBrokerTests.
    /// </summary>
   
    [TestFixture()]
    public class ConditionOfServiceBrokerTests : AbstractBrokerTests
    {
        #region Constants
        private const int TOTAL_NUMBER_OF_COS = 5;
        #endregion

        #region SetUp and TearDown ConditionOfServiceBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpConditionOfServiceBrokerTests()
        {
            i_cosbroker = BrokerFactory.BrokerOfType<IConditionOfServiceBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownConditionOfServiceBrokerTests()
        {
        }
        #endregion

        #region Test Methods

        [Test()]
        public void TestCOSList()
        {
            ICollection list = i_cosbroker.AllConditionsOfService();

            Assert.IsNotNull( list, "No list returned from ConditionOfServiceBroker.AllConditionsOfService" );
            Assert.IsTrue( list.Count > 0, "No Conditions of Service Returned" );
            Assert.IsTrue( list.Count == TOTAL_NUMBER_OF_COS, "Incorrect number of COS Values returned" );
            ConditionOfService cos = i_cosbroker.ConditionOfServiceWith( "Y" );
            Assert.IsNotNull( cos, "No COS of type 'Y' found" );
            Assert.AreEqual( cos.Description, "Yes" );
            Assert.AreEqual( cos.Code, "Y" );
        }
        [Test()]
        [ExpectedException( typeof( ArgumentNullException ) )]
        public void TestConditionOfServiceForNull()
        {
            ConditionOfService cos = i_cosbroker.ConditionOfServiceWith( null );

            Assert.IsTrue(
                cos.IsValid
                );
        }
    
        [Test()] 
        public void TestConditionOfServiceForInValid()
        {
            string COSInvalidCode = "I";
            ConditionOfService cos = i_cosbroker.ConditionOfServiceWith( COSInvalidCode );

            Assert.IsFalse(
                cos.IsValid
                );
        }

        [Test()]
        public void TestConditionOfServiceForBlank()
        {
           ConditionOfService cos = i_cosbroker.ConditionOfServiceWith( ConditionOfService.BLANK );

            Assert.AreEqual(
                String.Empty,
                cos.Code,
                "COS should be blank" );

            Assert.AreEqual
                ( String.Empty,
                  cos.Description,
                  "Description should be blank"
                );

            Assert.IsTrue(
                cos.IsValid
                );
        }

        [Test()]
        public void TestConditionOfServiceForUNABLE()
        {
            ConditionOfService cos = i_cosbroker.ConditionOfServiceWith( ConditionOfService.UNABLE );
            Assert.AreEqual( "U", cos.Code, "Code  should be U " );
            Assert.AreEqual( "No, Patient Medically Unable to Sign" , cos.Description,
                             "Description should be - No, Patient Medically Unable to Sign" );
            Assert.IsTrue( cos.IsValid );

        }
        [Test()]
        public void TestConditionOfServiceForREFUSED()
        { 
            ConditionOfService cos = i_cosbroker.ConditionOfServiceWith( ConditionOfService.REFUSED );

            Assert.AreEqual( "R", cos.Code, "Code  should be R " );
            Assert.AreEqual( "No, Patient Refused to Sign", cos.Description,
                             "Description should be -  No, Patient Refused to Sign" );
            Assert.IsTrue( cos.IsValid );

        }
        [Test()]
        public void TestConditionOfServiceForNOTAVAILABLE()
        {
            ConditionOfService cos = i_cosbroker.ConditionOfServiceWith( ConditionOfService.NOT_AVAILABLE );

            Assert.AreEqual( "N", cos.Code, "Code  should be N" );
            Assert.AreEqual( "No, Patient Not Available to Sign" , cos.Description,
                             "Description should be -  No, Patient Not Available to Sign" );
            Assert.IsTrue( cos.IsValid );

        }
        [Test()]
        public void TestConditionOfServiceForYes()
        {
            ConditionOfService cos = i_cosbroker.ConditionOfServiceWith( ConditionOfService.YES );

            Assert.AreEqual( "Y", cos.Code, "Code  should be N" );
            Assert.AreEqual( "Yes", cos.Description,
                             "Description should be -  No, Patient Not Available to Sign" );
            Assert.IsTrue( cos.IsValid );

        }


        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static IConditionOfServiceBroker i_cosbroker; 
        #endregion
    }
}