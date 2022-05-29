using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Security;
using PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for UserBrokerTest.
    /// </summary>

    //TODO: Create XML summary comment for UserBrokerTests
    [TestFixture()]
    public class UserBrokerTest : AbstractBrokerTests
    {
        #region Constants
        private new const string
            USER_NAME               = "patientaccess.user03";

        private const string 
            USER_PASSWORD           = "Password1",
            FACILITY_CODE_FOR_LOGIN = "DEL",
            PBAR_EMPLOYEE_ID        = "PAUSRT03";

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
        [Test()]
        public void TestAuthenticateUserAndRemoting()
        {
            UserBroker br = (UserBroker)BrokerFactory.BrokerOfType<IUserBroker>();
            SecurityResponse securityResponse = br.AuthenticateUser( USER_NAME, USER_PASSWORD, selectedFacility );

            Assert.IsNotNull( securityResponse, "Null SecurityResponse returned" );
            Assert.IsTrue( securityResponse.IsADAMAuthenticated, "Didn't get ADAMAuthenticated" );
            Assert.IsTrue( securityResponse.IsFacilityAuthorized, "Didn't get FacilityAuthorized" );
            Assert.IsTrue( securityResponse.IsPBARAuthenticated, "Didn't get PBARAuthenticated" );
            Assert.AreEqual( securityResponse.PatientAccessUser.PBAREmployeeID, PBAR_EMPLOYEE_ID, "We got wrong PBAREmployeeID" );
        }

        [Test()]
        public void TestAuthenticateUserWithErrorAndRemoting()
        {
            UserBroker brErr = (UserBroker)BrokerFactory.BrokerOfType<IUserBroker>();
            SecurityResponse errSecurityResponse = brErr.AuthenticateUser( ERR_USER_NAME, ERR_USER_PASSWORD, selectedFacility );
            Assert.IsNull( errSecurityResponse.PatientAccessUser, "We got NOT Null PatientAccessUser when a Null one expected" );

            //sign in with correct userid/pswd to prevent account locking:
            SecurityResponse securityResponse = brErr.AuthenticateUser( USER_NAME, USER_PASSWORD, selectedFacility );
        }

            
        [Test()]
        public void TestLogout() 
        {
            User patientAccessUser = User.GetCurrent();
            Extensions.SecurityService.Domain.User securityUser = new Extensions.SecurityService.Domain.User();
            securityUser.UPN = USER_NAME;
            patientAccessUser.SecurityUser = securityUser;

            i_UserBroker.Logout( patientAccessUser );
            //TODO: now call ADAM to check if User has been logged out :)
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