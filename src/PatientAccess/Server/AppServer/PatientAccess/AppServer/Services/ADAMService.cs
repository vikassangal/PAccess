using System;
using System.Configuration;
using System.Net;
using System.Xml;
using Extensions.SecurityService.Domain;
using PatientAccess.Annotations;
using Peradigm.Framework.Domain.Exceptions;
using Peradigm.Framework.Domain.Parties;
using Peradigm.Framework.Domain.Xml;

namespace PatientAccess.AppServer.Services
{
    /// <summary>
    /// Security service provider that connects with the Tenet ADAM
    /// infrastructure.  This provider requires platform dependent 
    /// configuration keys that describe the location of the ADAM
    /// web services used by the provider.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class ADAMService : ISecurityProvider
    {
        #region Constants
        private const int   
            MILLISECONDS_IN_ONE_SECOND				= 1000;
            
        private const string 
			ADAM_ACCESS_KEY							= "ADAMAccessKey",
			ADAM_APPLICATION_GUID					= "ADAMApplicationGUID",
			CONFIG_PROXY_URL						= "ADAM.ProxyServer",
			CONFIG_PROXY_EXCEPTION					= "ADAM.ProxyException",
			CONFIG_ADAM_SECURITY_SERVICE_URL		= "ADAM.SecurityService.URL",
			CONFIG_ADAM_SECURITY_SERVICE_TIMEOUT	= "ADAM.SecurityService.Timeout",
			EXCEPTION_MISSING_CONFIG_SETTING		= "Configuration Setting with Key '{0}' is required has not been defined.",
			EXPRESSION_IS_AUTHENTICATED				= "User/@IsAuthenticated";

		private const string
            ERR_AUTHENTICATE_USER			 = "Security Service Error - Application failed during user authentication: {0}.",
            ERR_AUTHENTICATE_USER_FOR_LEGACY = "Security Service Error - Application failed during legacy user authentication: {0}.",
			ERR_LOGOUT_USER					 = "Security Service Error - Application failed during user logout: {0}.",
			ERR_READ_APPLICATION_ROLES		 = "Security Service Error - Error retrieving roles for application: {0}.",
			ERR_READ_USER_PRIVILEGES		 = "Security Service Error - Error retrieving user privileges for application: {0}.",
			ERR_READ_APPROLE_FACILITY_USERS	 = "Security Service Error - Error retrieving application role users for facility: {0}.";
		#endregion

        #region Event Handlers
        #endregion

        #region Methods
        public bool HasUserWith( string upn, string password )
        {
            bool hasUser = false;
			try
			{
				this.SetUpProxyServer();
				string userAuthenticationXml = this.AdamSecurityService.AuthenticateUser( upn, password, this.ApplicationGUID, this.AccessKey );

				XmlDocument document = new XmlDocument();
				document.LoadXml( userAuthenticationXml );
				XPathParser parser = new XPathParser( document );
				string hasAccess = parser.ValueAt( EXPRESSION_IS_AUTHENTICATED );
			    
				hasUser = Convert.ToBoolean( hasAccess );
			}
            catch( Exception e )
            {
                throw new SecurityException( String.Format( ERR_AUTHENTICATE_USER, e.Message ), e, Severity.Catastrophic );
            }
			finally
			{
				this.ClearProxyServer();
			}
			return hasUser;
        }

        public string LegacyUserWith( string upn, string password, string legacyApplicationName )
        {
            string legacyUser = String.Empty;
            try
            {
                this.SetUpProxyServer();
                legacyUser = this.AdamSecurityService.UserPrivilegeForLegacy( upn, password, legacyApplicationName, this.ApplicationGUID, this.AccessKey );
            }
            catch( Exception e )
            {
                throw new SecurityException( String.Format( ERR_AUTHENTICATE_USER_FOR_LEGACY, e.Message ), e, Severity.Catastrophic );
            }
            finally
            {
                this.ClearProxyServer();
            }
            return legacyUser;
        }

		public void Logout(Extensions.SecurityService.Domain.User user )
        {
			try
			{
				this.SetUpProxyServer();
				this.AdamSecurityService.LastLogoutByUserLegacy( null, 
					null, 
					DateTime.Now.ToString(), 
					user.UPN, 
					null, 
					null, 
					this.ApplicationGUID, 
					this.AccessKey );
			}
			catch( Exception e )
			{
				throw new SecurityException( String.Format( ERR_LOGOUT_USER, e.Message ), e, Severity.Catastrophic );
			}
			finally
			{
				this.ClearProxyServer();
			}
		}
        
        public string RolesFor( Application application )
        {
            string roles = String.Empty;
            try
            {
			    this.SetUpProxyServer();
				roles = this.AdamSecurityService.AppRoles( application.Name, this.ApplicationGUID, this.AccessKey );
            }
            catch( Exception e )
            {
                throw new SecurityException( String.Format( ERR_READ_APPLICATION_ROLES, e.Message ), e, Severity.Catastrophic );
            }
			finally
			{
				this.ClearProxyServer();
			}
            return roles;
        }

        public string RolesFor( Application application, string upn, string password )
        {
            string roles = String.Empty;
			try
			{
				this.SetUpProxyServer();
				roles = this.AdamSecurityService.UserPrivilegesForCache( upn, password, application.Name, this.ApplicationGUID, this.AccessKey );
			}
			catch( Exception e )
			{
                throw new SecurityException( String.Format( ERR_READ_USER_PRIVILEGES, e.Message ), e, Severity.Catastrophic );
			}
			finally
			{
				this.ClearProxyServer();
			}
			return roles;
        }

        public string UsersIn( Application application, Role role, Facility facility )
        {
            string users = String.Empty;
            try
            {
			    this.SetUpProxyServer();
				users = this.AdamSecurityService.UsersForAppRoleFacility( application.Name, 
					application.Name + "-" + role.Name, 
					facility.Code,
					this.ApplicationGUID,
					this.AccessKey );
            }
            catch( Exception e )
            {
                throw new SecurityException( String.Format( ERR_READ_APPROLE_FACILITY_USERS, e.Message ), e, Severity.Catastrophic );
            }
			finally
			{
				this.ClearProxyServer();
			}
			return users;
        }

        private Extensions.AdamSecurityService.SecurityService ConstructADAMSecurityService()
		{
            Extensions.AdamSecurityService.SecurityService securityService = new Extensions.AdamSecurityService.SecurityService();

			securityService.Url = this.ConfigurationSetting( CONFIG_ADAM_SECURITY_SERVICE_URL );
			securityService.Timeout = int.Parse( this.ConfigurationSetting( CONFIG_ADAM_SECURITY_SERVICE_TIMEOUT ) ) * MILLISECONDS_IN_ONE_SECOND;
			return securityService;
		}

		private string ConfigurationSetting( string Key )
		{
            string settingValue = ConfigurationManager.AppSettings[Key];
			if( settingValue == null )
			{
				throw new ConfigurationErrorsException( string.Format( EXCEPTION_MISSING_CONFIG_SETTING, Key ) );
			}
			return settingValue;
		}
		#endregion

        #region Properties
        #endregion

        #region Private Methods
		private void SetUpProxyServer()
		{
			this.ClearProxyServer();
			if( this.UseProxy )
			{
				WebProxy proxyServer = new WebProxy( new Uri( this.ProxyServer ), true );
				if( this.ProxyException != String.Empty )
				{
					proxyServer.BypassArrayList.Add( this.ProxyException );
				}
				WebRequest.DefaultWebProxy = proxyServer;
			}
		}

		private void ClearProxyServer()
		{
            WebRequest.DefaultWebProxy = null;
		}
		#endregion

        #region Private Properties
		private string ProxyServer
		{
			get
			{
                string url = ConfigurationManager.AppSettings[CONFIG_PROXY_URL];
				if( url == null )
				{
					url = String.Empty;
				}
				return url;
			}
		}

		private bool UseProxy
		{
			get
			{
				return !this.ProxyServer.Equals( String.Empty );
			}
		}

		private string ProxyException
		{
			get
			{
                string url = ConfigurationManager.AppSettings[CONFIG_PROXY_EXCEPTION];
				if( url == null )
				{
					url = String.Empty;
				}
				return url;
			}
		}

		private string AccessKey
		{
			get
			{
				return i_AccessKey;
			}
			set
			{
				i_AccessKey = value;
			}
		}

        private Extensions.AdamSecurityService.SecurityService AdamSecurityService
		{
			get
			{
                Extensions.AdamSecurityService.SecurityService securityService = this.ConstructADAMSecurityService();
				return securityService;
			}
		}

		private string ApplicationGUID
		{
			get
			{
				return i_ApplicationGUID;
			}
			set
			{
				i_ApplicationGUID = value;
			}
		}
        #endregion

        #region Construction and Finalization
        public ADAMService()
        {
            this.AccessKey = ConfigurationManager.AppSettings[ADAM_ACCESS_KEY];
            this.ApplicationGUID = ConfigurationManager.AppSettings[ADAM_APPLICATION_GUID];
        }
        #endregion

        #region Data Elements
		private string i_AccessKey;
		private string i_ApplicationGUID;
        #endregion
    }
}
