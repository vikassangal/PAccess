using System;
using System.Collections;
using System.Configuration;
using System.Data.SqlClient;
using Extensions.Persistence;
using Extensions.SecurityService;
using Extensions.SecurityService.Domain;
using PatientAccess.AppServer.Services;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.Security;
using PatientAccess.Persistence.Security;
using log4net;
using User = PatientAccess.Domain.User;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for UserBroker.
    /// </summary>
    [Serializable]
    public class UserBroker : AbstractBroker, IUserBroker
    {
        #region Constants
        private const string
            USER_NOT_FOUND_MESSAGE = "User not found in Active Directory",
            USERNAME_DEFAULT       = "UserNameDefault",
            PASSWORD_DEFAULT       = "PasswordDefault",
            WORKSTATIONID_DEFAULT  = "WorkstationIDDefault";
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public void Credentials ( out string userName, out string password, out string workstationID )
        {
            userName        = string.Empty;
            password        = string.Empty;
            workstationID   = string.Empty;

            try
            {
                userName      = ConfigurationManager.AppSettings[USERNAME_DEFAULT];
                password      = ConfigurationManager.AppSettings[PASSWORD_DEFAULT];
                workstationID = ConfigurationManager.AppSettings[WORKSTATIONID_DEFAULT];
            }
            catch(Exception ex)
            {
                c_log.Info( "Failed to get credentials from the server", ex ); 
                throw;
            }
        }

        public SecurityResponse AuthenticateUser ( string userName, string password, Facility selectedFacility )
        {
            PatientAccessApplication patientAccessApplication = new PatientAccessApplication();
            SecurityResponse securityResponse = new SecurityResponse();
            Extensions.SecurityService.Domain.User securityUser = new Extensions.SecurityService.Domain.User();
            string PBAREmployeeID = string.Empty;

            try
            {
                SecurityService securityService = new SecurityService( new ADAMService() );
                SecurityService.ApplicationService = patientAccessApplication;
                
                securityUser = securityService.UserWith(userName, password);

                if( securityUser != null )
                {
                    securityResponse.IsADAMAuthenticated = true;
                    securityResponse.IsFacilityAuthorized = ValidateFacility( securityUser, selectedFacility );
                    if( securityResponse.IsFacilityAuthorized )
                    {
                        Hashtable legacyData = securityService.LegacyUserWith( userName, password, patientAccessApplication.LegacyApplicationName );
                        PBAREmployeeID = (string)legacyData [ selectedFacility.Code ];
                        if( !( PBAREmployeeID == null || PBAREmployeeID.Equals(String.Empty) ))
                        {
                            securityResponse.IsPBARAuthenticated = true;
                        }
                        else
                        {
                            securityResponse.LoginFailureMessage = SecurityResponse.PBAR_AUTHENTICATION_FAILURE_MESSAGE;
                            return securityResponse;
                        }
                    }
                    else
                    {
                        securityResponse.LoginFailureMessage = SecurityResponse.FACILITY_AUTHORIZATION_FAILURE_MESSAGE;
                        return securityResponse;
                    }
                }
                else 
                {
                    // A null securityUser will be returned from the SecurityFramework when either the provided
                    // UPN / Password / Application Name / Application GUID / Access Key is invalid or incorrect.
                    securityResponse.LoginFailureMessage = SecurityResponse.ADAM_AUTHENTICATION_FAILURE_MESSAGE;
                    return securityResponse;
                }

                if (securityResponse.CanLogin())
                {
                    pbarUserBroker.UpdateLastLoginDateForPbarUser( PBAREmployeeID, selectedFacility );
                }

                User patientAccessUser = this.BuildPatientAccessUserFrom( securityUser, selectedFacility, PBAREmployeeID );
                securityResponse.PatientAccessUser = patientAccessUser;

                return securityResponse;
            }
            catch( SecurityException e )
            {
                if( e.InnerException != null && e.InnerException.Message != null &&
                    e.InnerException.Message.IndexOf( USER_NOT_FOUND_MESSAGE ) >= 0 )
                {
                    securityResponse.LoginFailureMessage = SecurityResponse.ADAM_AUTHENTICATION_FAILURE_MESSAGE;
                    return securityResponse;
                }
                else
                {
                    string msg = string.Format("UserBroker failed during AuthenticateUser with SecurityException.\n  Facility: selectedFacility {0}\n " +
                        " SecurityUser: {1}\n PBAREmployeeID: {2}", selectedFacility, securityUser, PBAREmployeeID);
                    throw BrokerExceptionFactory.BrokerExceptionFrom( msg, e, c_log );
                }
            }
            catch( Exception e )
            {
                string msg = string.Format( "UserBroker failed during AuthenticateUser.\n  Facility: selectedFacility {0}\n " +
                        " SecurityUser: {1}\n PBAREmployeeID: {2}", selectedFacility, securityUser, PBAREmployeeID );
                throw BrokerExceptionFactory.BrokerExceptionFrom( msg, e, c_log );
            }
        }

        public bool HasPermissionToOverrideMonthlyPayment ( string userName, string password, Facility facility )
        {
            bool hasPermission = false;
            try         
            {
                SecurityService securityService = new SecurityService( new ADAMService() );
                Extensions.SecurityService.Domain.User securityUser = securityService.UserWith(userName, password);
                if( securityUser != null )
                {
                    Peradigm.Framework.Domain.Parties.Facility securityFrameworkFacility = new Peradigm.Framework.Domain.Parties.Facility( facility.Code, facility.Description );
                    hasPermission = securityUser.HasPermissionTo( Privilege.Actions.Edit, typeof( MonthlyPaymentOverride ), securityFrameworkFacility );
                }
            }
            catch( SecurityException e )
            {
                if( e.InnerException.Message.IndexOf( USER_NOT_FOUND_MESSAGE ) >= 0 )
                {
                    return hasPermission;
                }
                else
                {
                    string msg = "UserBroker failed during checking if User HasPermissionToOverrideMonthlyPayment.";
                    throw BrokerExceptionFactory.BrokerExceptionFrom( msg, e, c_log );
                }
            }
            catch( Exception e )
            {
                string msg = "UserBroker failed during checking if User HasPermissionToOverrideMonthlyPayment.";
                throw BrokerExceptionFactory.BrokerExceptionFrom( msg, e, c_log );
            }
            return hasPermission;
        }

         public void Logout ( User patientAccessUser )
        {
             try
             {
                 SecurityService securityService = new SecurityService( new ADAMService() );
                 Extensions.SecurityService.Domain.User securityUser = patientAccessUser.SecurityUser;
                 securityService.Logout( securityUser );
             }
             catch( Exception ex)
             {
                 string msg = ex.Message + " " + ex.StackTrace;
                 throw BrokerExceptionFactory.BrokerExceptionFrom(msg, ex, c_log );
             }
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        public bool ValidateFacility ( Extensions.SecurityService.Domain.User securityUser, Facility facility )
        {
            bool isFacilityAuthorized = false;
            ArrayList frameworkFacilities = securityUser.AllFacilities();
            string facilityCode = facility.Code;

            foreach(  Peradigm.Framework.Domain.Parties.Facility frameworkFacility in frameworkFacilities )
            {
                if( frameworkFacility.Code == facilityCode )
                {
                    isFacilityAuthorized = true;
                    break;
                }
            }
            return isFacilityAuthorized;
        }

        public User BuildPatientAccessUserFrom( Extensions.SecurityService.Domain.User securityUser, Facility facility, string PBAREmployeeID )
        {
            User patientAccessUser = User.NewInstance();
            patientAccessUser.FirstName = securityUser.FirstName;
            patientAccessUser.LastName = securityUser.LastName;
            string middleInitial = string.Empty; //we don't capture this field for securityUser yet
            patientAccessUser.Name = new Name( securityUser.FirstName, securityUser.LastName, middleInitial);
            patientAccessUser.Facility = facility;
            patientAccessUser.SecurityUser = securityUser;
            patientAccessUser.PBAREmployeeID = PBAREmployeeID;

            return patientAccessUser;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public UserBroker() : base()
        {
        }
        
        public UserBroker( string cxnString ) : base( cxnString )
        {
        }

        public UserBroker( SqlTransaction txn ) : base( txn )
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( UserBroker ) );

        private PbarUserBroker pbarUserBroker = new PbarUserBroker();

        #endregion
    }
}
