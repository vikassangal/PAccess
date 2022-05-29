using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Security;

namespace Tests.Integration.PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for AbstractBrokerInterfaceTests.
    /// </summary>
    public abstract class AbstractBrokerInterfaceTests
    {
        #region Constants

        private const string USER_NAME = "patientaccess.user03";
        private const string PASSWORD = "Password1";
        public const string FACILITY_CODE = "ACO";
        private const long ACO_FACILITYID = 900;
        #endregion

        #region SetUp and TearDown GenericBrokerTests
        [TestFixtureSetUp()]
        public void CreateUser()
        {
            User patientAccessUser = User.GetCurrent();
            if( patientAccessUser.SecurityUser == null )
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility selectedFacility = facilityBroker.FacilityWith( ACO_FACILITYID );


                IUserBroker br = BrokerFactory.BrokerOfType<IUserBroker>();
                SecurityResponse securityResponse = br.AuthenticateUser( USER_NAME, PASSWORD, selectedFacility );

                User.SetCurrentUserTo( securityResponse.PatientAccessUser ); 
            }
        }
        #endregion
    }
}