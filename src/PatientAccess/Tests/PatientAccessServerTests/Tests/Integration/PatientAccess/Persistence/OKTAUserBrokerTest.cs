using System;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Security;
using PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for OKTAUserBrokerTest.
    /// </summary>

    //TODO: Create XML summary comment for OKTAUserBrokerTests
    [Ignore]
    [TestFixture()]
    public class OKTAUserBrokerTest : AbstractBrokerTests
    {
        #region Constants
        private new const string
            USER_NAME               = "patientaccess.user04";

        private const string 
            USER_PASSWORD           = "$Mdltrain2",
            FACILITY_CODE_FOR_LOGIN = "PRV",
            PBAR_EMPLOYEE_ID        = "YGRAVES";

        private const string
            ERR_USER_NAME               = "patientaccess.user01",
            ERR_USER_PASSWORD           = "error";
    
        #endregion

        #region SetUp and TearDown UserBrokerTests
        [TestFixtureSetUp()]
        public void SetUpUserBrokerTest()
        {
            i_UserBroker = BrokerFactory.BrokerOfType<IUserBroker>();
            var facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            selectedFacility = facilityBroker.FacilityWith(FACILITY_CODE_FOR_LOGIN);
        }

        [TestFixtureTearDown()]
        public static void TearDownUserBrokerTest()
        {
        }
        #endregion

        #region Test Methods
        [Ignore]
        [Test()]
        
        public void TestAuthenticateUserAndRemoting()
        {
            OKTAUserBroker br = (OKTAUserBroker)BrokerFactory.BrokerOfType<IOKTAUserBroker>();
            SecurityResponse securityResponse = br.AuthenticateUser( USER_NAME, USER_PASSWORD, selectedFacility );

            Assert.IsNotNull( securityResponse, "Null SecurityResponse returned" );
            Assert.IsTrue( securityResponse.IsADAMAuthenticated, "Didn't get ADAMAuthenticated" );
            Assert.IsTrue( securityResponse.IsFacilityAuthorized, "Didn't get FacilityAuthorized" );
            Assert.IsTrue( securityResponse.IsPBARAuthenticated, "Didn't get PBARAuthenticated" );
            Assert.AreEqual( securityResponse.PatientAccessUser.PBAREmployeeID, PBAR_EMPLOYEE_ID, "We got wrong PBAREmployeeID" );
        }
        [Ignore]
        [Test()]
        
        [ExpectedException(typeof(SystemException))]
        public void TestAuthenticateUserWithErrorAndRemoting()
        {
            try
            {
                OKTAUserBroker brErr = (OKTAUserBroker)BrokerFactory.BrokerOfType<IOKTAUserBroker>();
                brErr.AuthenticateUser(ERR_USER_NAME, ERR_USER_PASSWORD, selectedFacility);
                Assert.Fail();
            }
            catch
            {
                Assert.Pass();
            }
        }

          
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static IUserBroker i_UserBroker;
        private Facility selectedFacility;

        #endregion
    }
}