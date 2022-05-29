using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for FacilityBrokerTests.
    /// </summary>
    [TestFixture()]
    public class AuthorizationStatusPBARBrokerTests : AbstractBrokerTests
    {
        #region Constants
        private const long      TEST_AUTHORIZATION_STATUS_ID            = 1,
                                EXPECTED_AUTHORIZATION_STATUS_ID_BLANK  = 4, 
                                EXPECTED_CARDINALITY                    = 4 ;

        private const string    TEST_AUTHORIZATION_STATUS_CODE          = "A",
                                TEST_AUTHORIZATION_BLANK_CODE           = " ",
                                EXPECTED_AUTHORIZATIONSTATUS_NAME       = "APPROVED",
                                EXPECTED_AUTHORIZATION_STATUS_BLANK     = " " ;
        #endregion

        #region SetUp and TearDown AuthorizationStatusBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpAuthorizationStatusBrokerTests()
        {
            asb = BrokerFactory.BrokerOfType<IAuthorizationStatusBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownAuthorizationStatusBrokerTests()
        {           
        }

        #endregion

        #region Test Methods
        [Test()]
        public void TestGetAuthorizationStatuses()
        {
            ArrayList list = asb.AllAuthorizationStatuses() as ArrayList;
            Assert.IsNotNull(
                list, "No AuthorizationStatuses were found");

            foreach (AuthorizationStatus entry in list)
            {
                Assert.IsTrue(entry.GetType() == typeof(AuthorizationStatus));
                Assert.IsTrue(entry.Oid != 0);
            }
        }

        [Test()]
        public void TestGetAuthorizationStatusesCardinality()
        {
            ArrayList list = asb.AllAuthorizationStatuses() as ArrayList;
            Assert.IsNotNull( list, "No AuthorizationStatuses were found" ) ;
            Assert.AreEqual( EXPECTED_CARDINALITY, list.Count,
                             "Cardinality test failed - expected a collection size count of {0}.", EXPECTED_CARDINALITY ) ;
        }

        [Test()]
        public void TestSpecificAuthorizationStatusByCode()
        {
            AuthorizationStatus authStatus = asb.AuthorizationStatusWith( TEST_AUTHORIZATION_STATUS_CODE ) ;
            Assert.IsNotNull(authStatus, "Can not find AuthorizationStatus by Code") ;
            Assert.AreEqual( TEST_AUTHORIZATION_STATUS_ID, authStatus.Oid, "AuthorizationStatus ID is not as expected" ) ;
            Assert.AreEqual( EXPECTED_AUTHORIZATIONSTATUS_NAME, authStatus.Description,
                             "AuthorizationStatus Name should be: " + EXPECTED_AUTHORIZATIONSTATUS_NAME ) ;
        }

        [Test()]
        public void TestSpecificAuthorizationStatusByBlankCode()
        {
            AuthorizationStatus authStatus = asb.AuthorizationStatusWith( TEST_AUTHORIZATION_BLANK_CODE ) ;
            Assert.IsNotNull( authStatus, "Can not find AuthorizationStatus by Code" ) ;
            Assert.AreEqual( EXPECTED_AUTHORIZATION_STATUS_ID_BLANK, authStatus.Oid, "AuthorizationStatus ID is not as expected" ) ;
            Assert.AreEqual( EXPECTED_AUTHORIZATION_STATUS_BLANK, 
                             authStatus.Description,
                             "AuthorizationStatus Name should be: " + 
                             EXPECTED_AUTHORIZATION_STATUS_BLANK ) ;
        }

        [Test()]
        [ExpectedException( typeof( ArgumentNullException ) ) ] 
        public void TestSpecificAuthorizationStatusByNullCode()
        {
            AuthorizationStatus authStatus = asb.AuthorizationStatusWith( null ) ;
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        private static  IAuthorizationStatusBroker asb = null;

        #endregion

    }
}