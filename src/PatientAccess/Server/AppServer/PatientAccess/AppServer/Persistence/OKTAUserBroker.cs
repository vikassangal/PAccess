using System;
using System.Configuration;
using System.Data.SqlClient;
using Extensions.Persistence;
using Extensions.SecurityService;
using Extensions.SecurityService.Domain;
using PatientAccess.AppServer.Services;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Security;
using PatientAccess.Persistence.Security;
using log4net;
using User = PatientAccess.Domain.User;
using System.Collections;

namespace PatientAccess.Persistence
{
    public class OKTAUserBroker : AbstractBroker, IOKTAUserBroker
    {
        #region Constants

        private const string
            USER_NOT_FOUND_MESSAGE = "User ID and/or password were not recognized",
            OKTA_ERROR_MESSAGE = "We are unable to authorize your access. Please open a ticket in Tenetone for the Patient Access System (PAS) support team.";
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        

        public SecurityResponse AuthenticateUser(string userName, string password, Facility selectedFacility)
        {
            var ADAMuserBroker = BrokerFactory.BrokerOfType<IUserBroker>();
            PatientAccessApplication patientAccessApplication = new PatientAccessApplication();
            SecurityResponse securityResponse = new SecurityResponse();
            Extensions.SecurityService.Domain.User securityUser;
            string PBAREmployeeID = string.Empty;
            string errorInfo = string.Empty;
            try
            {
                OKTASecurityService oktaSecurityService = new OKTASecurityService();
                OKTASecurityService.ApplicationService = patientAccessApplication;
                SecurityService.ApplicationService = patientAccessApplication;
                securityUser = oktaSecurityService.UserWith(userName, password);

                if (securityUser != null && securityUser.Roles().Count>0)
                {
                    securityResponse.IsADAMAuthenticated = true;
                    securityResponse.IsFacilityAuthorized =userBroker.ValidateFacility(securityUser, selectedFacility);
                    if (securityResponse.IsFacilityAuthorized)
                    {
                        Hashtable legacyData = oktaSecurityService.LegacyUserWith(securityUser.EmployeeID);
                        PBAREmployeeID = (string)legacyData[selectedFacility.Code];
                        if (!(PBAREmployeeID == null || PBAREmployeeID.Equals(String.Empty)))
                        {
                            securityResponse.IsPBARAuthenticated = true;
                        }
                        else
                        {
                            securityResponse.LoginFailureMessage = OKTA_ERROR_MESSAGE;
                            errorInfo="OKTA - PBAR Authentication failed, Legacy ID not found in TenetOne for User : "+ userName +" and selected facility : "+selectedFacility.Code;
                            securityResponse = RedirectToAdam(userName, password, selectedFacility,errorInfo, securityResponse);
                            return securityResponse;
                        }
                    }
                    else
                    {
                        securityResponse.LoginFailureMessage = OKTA_ERROR_MESSAGE;
                        errorInfo = "OKTA - User :" + userName + " does not have access to the selected facility :" + selectedFacility.Code;
                        securityResponse = RedirectToAdam(userName, password, selectedFacility, errorInfo, securityResponse);
                        return securityResponse;
                    }
                }
                else
                {
                    if (securityUser!=null)
                    {
                        if (securityUser.Roles().Count == 0)
                        {
                            securityResponse.LoginFailureMessage = OKTA_ERROR_MESSAGE;
                            errorInfo = ("OKTA - Roles not assigned to User: " + userName + " and select facility: " + selectedFacility.Code);
                            securityResponse = RedirectToAdam(userName, password, selectedFacility, errorInfo, securityResponse);
                        }
                       
                    }
                    else
                    {
                        securityResponse.LoginFailureMessage = OKTA_ERROR_MESSAGE;
                        errorInfo = "OKTA - ETENET ID not assigned to User: " + userName + " and select facility: " + selectedFacility.Code;
                        securityResponse = RedirectToAdam(userName, password, selectedFacility, errorInfo, securityResponse);
                    }
                    
                    return securityResponse;
                }

                if (securityResponse.CanLogin())
                {
                    pbarUserBroker.UpdateLastLoginDateForPbarUser(PBAREmployeeID, selectedFacility);
                }

                User patientAccessUser = userBroker.BuildPatientAccessUserFrom(securityUser, selectedFacility, PBAREmployeeID);
                securityResponse.PatientAccessUser = patientAccessUser;

                return securityResponse;
            }
            catch (SecurityException e)
            {
                if (e.InnerException != null && e.InnerException.Message != null &&
                    e.InnerException.Message.IndexOf(USER_NOT_FOUND_MESSAGE) >= 0)
                {
                    securityResponse.LoginFailureMessage = SecurityResponse.OKTA_AUTHENTICATION_FAILURE_MESSAGE;
                    errorInfo = "OKTA UserBroker failed during Authenticate to User: " + userName + " and select facility: " + selectedFacility.Code;
                    c_log.Error(errorInfo);
                    return securityResponse;
                }
                else
                {
                    string msg = string.Format("OKTA UserBroker failed during AuthenticateUser with SecurityException.\n  Facility: selectedFacility {0}\n " +
                        " SecurityUser: {1}", selectedFacility.Code, userName);
                    throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
                }
            }
            catch (Exception e)
            {
                string msg = string.Format("OKTA UserBroker failed during AuthenticateUser.\n  Facility: selectedFacility {0}\n " +
                        " SecurityUser: {1}", selectedFacility.Code, userName);
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
        }

        private SecurityResponse RedirectToAdam(string userName,string password,Facility selectedFacility,string errorInfo, SecurityResponse securityResponse)
        {
            c_log.Error(errorInfo);
            if (IsFallOutToADAM)
            {
                securityResponse = userBroker.AuthenticateUser(userName, password, selectedFacility);
                return securityResponse;
            }
            else
            {
                return securityResponse;
            }
        }

        #endregion
        #region Properties
        #endregion

        #region Construction and Finalization
        public OKTAUserBroker() : base()
        {
        }

        public OKTAUserBroker(string cxnString) : base(cxnString)
        {
        }

        public OKTAUserBroker(SqlTransaction txn) : base(txn)
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger(typeof(OKTAUserBroker));

        private PbarUserBroker pbarUserBroker = new PbarUserBroker();
        private UserBroker userBroker = new UserBroker();
        private bool IsFallOutToADAM =
       bool.Parse(ConfigurationManager.AppSettings["IsFallOutToADAM"]);
        #endregion
    }
}