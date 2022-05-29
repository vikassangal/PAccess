using System;

namespace PatientAccess.Domain.Security
{
	/// <summary>
	/// Summary description for SecurityResponse.
	/// </summary>
//TODO: Create XML summary comment for SecurityResponse
    [Serializable]
    public class SecurityResponse
    {
        #region Event Handlers
        #endregion

        #region Methods

        public bool CanLogin()
        {
            return( IsADAMAuthenticated && IsFacilityAuthorized && IsPBARAuthenticated ); 
        }


        #endregion

        #region Properties

        public bool IsADAMAuthenticated
        {
            get
            {
                return i_IsADAMAuthenticated;
            }
            set
            {
                i_IsADAMAuthenticated = value;
            }
        }
        public bool IsPBARAuthenticated
        {
            get
            {
                return i_IsPBARAuthenticated;
            }
            set
            {
                i_IsPBARAuthenticated = value;
            }
        }
        public bool IsFacilityAuthorized
        {
            get
            {
                return i_IsFacilityAuthorized;
            }
            set
            {
                i_IsFacilityAuthorized = value;
            }
        }
        public string LoginFailureMessage
        {
            get
            {
                return i_LoginFailureMessage;
            }
            set
            {
                i_LoginFailureMessage = value;
            }
        }
        public User PatientAccessUser
        {
            get
            {
                return i_PatientAccessUser;
            }
            set
            {
                i_PatientAccessUser = value;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public SecurityResponse()
        {
        }
        #endregion

        #region Data Elements

        private bool i_IsADAMAuthenticated = false;
        private bool i_IsPBARAuthenticated = false;
        private bool i_IsFacilityAuthorized = false;

        private string i_LoginFailureMessage;

        private User i_PatientAccessUser;

        #endregion

        #region Constants

        public const string ADAM_AUTHENTICATION_FAILURE_MESSAGE = 
            "The User ID and/or password were not recognized. Please try again.\nOR\nIf you have repeatedly attempted to log in using this User ID and password without success,\nplease call the Tenet Help Desk for assistance.";
        
        public const string FACILITY_AUTHORIZATION_FAILURE_MESSAGE = 
            "Authorization to this facility is required. \nPlease select another facility, or call the Tenet Help Desk for assistance.";

        public const string PBAR_AUTHENTICATION_FAILURE_MESSAGE =
            "We are unable to validate your access. Please open a ticket in Tenetone for the Patient Access System (PAS) support team.";
            

        public const string OKTA_AUTHENTICATION_FAILURE_MESSAGE =
            "The User ID and/or password were not recognized . Please try again.\nOR\nIf you have repeatedly attempted to log in using this User ID and password without success,\nplease call the Tenet Help Desk for assistance.";

        public const string OKTA_FACILITY_AUTHORIZATION_FAILURE_MESSAGE =
            "Authorization to this facility is required. \nPlease select another facility, or call the Tenet Help Desk for assistance.";

        public const string OKTA_PBAR_AUTHENTICATION_FAILURE_MESSAGE =
            "Authentication is incomplete. Please call the Tenet Help Desk for assistance.";
        public const string OKTA_ROLE_FAILURE_MESSAGE =
            "Role not assigned for selected facility. Please call the Tenet Help Desk for assistance.";
        public const string OKTA_ETENETID_FAILURE_MESSAGE =
            "ETENET ID not assigned for User. Please call the Tenet Help Desk for assistance.";

        #endregion
    }
}
