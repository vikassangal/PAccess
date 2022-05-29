using System;
using System.Collections;
using System.Configuration;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using Peradigm.Framework.Domain.Collections;
using Extensions.SecurityService.Domain;
using Peradigm.Framework.Domain.Exceptions;
using Peradigm.Framework.Domain.Parties;
using Peradigm.Framework.Domain.Xml;

namespace Extensions.SecurityService
{
	/// <summary>
	/// Security Service is responsible for supplying an application with relevant security information
	/// about users based on the configured and installed providers.
	/// </summary>
    [Serializable]
    public class SecurityService
    {
        #region Constants
        private const string 
            USER_WITH_EXPRESSION_LAST_NAME   = "Result/User/@LastName",
            USER_WITH_EXPRESSION_FIRST_NAME  = "Result/User/@FirstName",
            USER_WITH_EXPRESSION_HSP_CODE    = @"Result/User/Applications/Application/Roles/Role[@Name=""{0}""]/Facilities/Facility/@HspCode",
            USER_WITH_EXPRESSION_TENET_ID    = "Result/User/@EmployeeId",
            USER_WITH_EXPRESSION_ROLE_NAME   = "Result/User/Applications/Application/Roles/Role/@Name",
            USER_WITH_EXPRESSION_GLOBAL_ROLE = @"Result/User/Applications/Application/Roles/Role[@Name=""{0}""]/Globals/Global/@FullName",
            USER_AUTHENTICATED               = "Result/User/@IsAuthenticated",
            TRUE = "TRUE",
            RESULT_VALUE = "Result/@Value"; 

        private const string 
            LEGACY_USER_WITH_EXPRESSION_LEGACY_MAP    = "Result/User/Application/LegacyMaps/LegacyMap",
            LEGACY_USER_WITH_EXPRESSION_LEGACY_USERID = "LegacyUserID",
            LEGACY_USER_WITH_EXPRESSION_HSP_CODE      = "HspCD";

        // TLG 11/11/2007
        // adding Result to the following path is an assumption! Could not be tested because we are not authorized to this method!
        private const string 
            ROLES_FROM_EXPRESSION_NAME        = "Result/Application/Roles/Role/@Name",
            ROLES_FROM_EXPRESSION_APP_ROLE_ID = "Result/Application/Roles/Role/@AppRoleID";

        private const string 
            GLOBAL_ROLE                     = "Global Role";

	    private const string
	        ERR_NO_PROVIDER_LOADED = "Security Service Error - Security Provider is not loaded",
	        ERR_LOADING_PROVIDER = "Security Service Error - Could not load Provider: {0}",
	        ERROR_INVALID_PROVIDER_PATH = "Security Service Error - Path to Security Service Provider is missing or invalid",
	        ERROR_INVALID_PROVIDER_CLASS = "Security Service Error - Classname of Security Service Provider is missing or invalid",
	        ERR_AUTHENTICATE_USER = "Security Service Error - Application failed during user authentication: {0}.",
	        ERR_AUTHENTICATE_USER_FOR_LEGACY = "Security Service Error - Application failed during user authentication for legacy: {0}.",
	        ERR_LOGOUT_USER = "Security Service Error - Application failed during user logout: {0}.",
	        ERR_READ_APPLICATION_ROLES = "Security Service Error - Error retrieving roles for application: {0}.",
	        ERR_READ_USER_PRIVILEGES = "Security Service Error - Error retrieving user privileges for application: {0}.";


        private const string
            CONFIG_PROVIDER_PATH            = "SecurityService.Provider.Path",
            CONFIG_PROVIDER_CLASS           = "SecurityService.Provider.Class";
        
		#endregion

		#region Event Handlers
		#endregion

		#region Methods
		/// <summary>
		/// Authenticate User in AD.
		/// </summary>
		/// <param name="upn"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public bool AuthenticateUser( string upn, string password )
		{
			bool userHasAccess = false;
			try
			{
				userHasAccess = this.Provider.HasUserWith( upn, password );
			}
			catch( SecurityException )
			{
				throw;
			}
			catch( Exception e )
			{
				throw new SecurityException( String.Format( ERR_AUTHENTICATE_USER, e.Message ), e, Severity.Catastrophic );
			}
			return userHasAccess;
		}

	    /// <summary>
	    /// Authenticate User in PBAR
	    /// </summary>
	    /// <param name="upn"></param>
	    /// <param name="password"></param>
	    /// <param name="legacyApplicationName"></param>
	    /// <returns></returns>
	    public Hashtable LegacyUserWith( string upn, string password, string legacyApplicationName )
		{
			string legacyUserXml = string.Empty;
            Hashtable legacyUser = new Hashtable();
			try
			{
				legacyUserXml = this.Provider.LegacyUserWith( upn, password, legacyApplicationName );

                XmlDocument document = new XmlDocument();
                document.LoadXml( legacyUserXml );
                XPathNavigator navigator = document.CreateNavigator();

                XPathNodeIterator nodeLegacyMapIterator;

                nodeLegacyMapIterator = navigator.Select( LEGACY_USER_WITH_EXPRESSION_LEGACY_MAP );
                
                string hospCode, legacyUserID; 
                while( nodeLegacyMapIterator.MoveNext() )
                {
                    hospCode = nodeLegacyMapIterator.Current.GetAttribute( LEGACY_USER_WITH_EXPRESSION_HSP_CODE, navigator.NamespaceURI);
                    legacyUserID = nodeLegacyMapIterator.Current.GetAttribute( LEGACY_USER_WITH_EXPRESSION_LEGACY_USERID, navigator.NamespaceURI);
                    if( !(legacyUser.ContainsKey( hospCode )) )
                    {
                        legacyUser.Add( hospCode, legacyUserID );
                    }
                }
			}
			catch( SecurityException )
			{
				throw;
			}
			catch( Exception e )
			{
				throw new SecurityException( String.Format( ERR_AUTHENTICATE_USER_FOR_LEGACY, e.Message ), e, Severity.Catastrophic );
			}
			return legacyUser;
		}

		public void Logout( User user )
		{
			try
			{
				this.Provider.Logout( user );
			}
			catch( SecurityException )
			{
				throw;
			}
			catch( Exception e )
			{
				throw new SecurityException( String.Format( ERR_LOGOUT_USER, e.Message ), e, Severity.Catastrophic );
			}
		}

		/// <summary>
		/// Obsolete: For XML Serialization
		/// </summary>
		/// <returns></returns>
		public ArrayList RolesFor()
		{
			ArrayList roles = new ArrayList();
			try
			{
				string rolesInXml = this.Provider.RolesFor( this.Application );
				Hashtable roleNames = this.RolesFrom( rolesInXml );
			
				foreach( DictionaryEntry de in roleNames )
				{
					Role aRole = new Role( Convert.ToInt64( de.Key ), de.Value.ToString() );
					roles.Add( aRole );
				}
			}
			catch( SecurityException )
			{
				throw;
			}
			catch( Exception e )
			{
				throw new SecurityException( String.Format( ERR_READ_APPLICATION_ROLES, e.Message ), e, Severity.Catastrophic );
			}
			return roles;
		}

		public User UserWith( string loginName, string password )
		{
			return this.UserWith( loginName, loginName, password );
		}

		/// <summary>
		/// Create a user with rolerelationship by facility
		/// </summary>
		/// <param name="upn"></param>
		/// <param name="loginName"></param>
		/// <param name="password"></param> 
		/// <returns></returns>
		private User UserWith( string upn, string loginName, string password )
		{
			//PortalUser portalUser = null;
			User domainUser = null;
			try
			{
				string userRolesXml = this.Provider.RolesFor( this.Application, loginName, password );
            
				XmlDocument document = new XmlDocument();
				document.LoadXml( userRolesXml );
				XPathParser parser = new XPathParser( document );
			
				XPathNavigator navigator = document.CreateNavigator();

                string resultValue = parser.ValueAt( RESULT_VALUE );

                if( resultValue != null && resultValue.ToUpper() == TRUE )
                {

                    string isAuth = parser.ValueAt( USER_AUTHENTICATED );
                    bool attributeIsAuthenticated = Convert.ToBoolean( isAuth );
                    string attributeLastName = parser.ValueAt( USER_WITH_EXPRESSION_LAST_NAME );
                    string attributeFirstName = parser.ValueAt( USER_WITH_EXPRESSION_FIRST_NAME );
                    string attributeTenetID = parser.ValueAt( USER_WITH_EXPRESSION_TENET_ID );

                    string attributeRoleName = String.Empty;

                    XPathNodeIterator nodeHSPCodeIterator;
                    XPathNodeIterator nodeRoleNameIterator;

                    nodeRoleNameIterator = navigator.Select( USER_WITH_EXPRESSION_ROLE_NAME );

                    if( attributeIsAuthenticated )
                    {
                        domainUser = new User( Convert.ToInt64( attributeTenetID ), attributeFirstName, attributeLastName );
                        domainUser.UPN = upn;

                        OrganizationalUnit hierarchy = OrganizationalService.OrganizationalService.Service.GetOrganizationalHierarchy();
                        Role aRole;
                        while( nodeRoleNameIterator.MoveNext() )
                        {
                            attributeRoleName = nodeRoleNameIterator.Current.Value;
                            string roleName = attributeRoleName.Substring( attributeRoleName.LastIndexOf( "-" ) + 1 );
                            aRole = this.Application.RoleWith( roleName );
                            if( aRole != null )
                            {
                                ArrayList facilityCodes = new ArrayList();
                                string attributeGlobalRole = parser.ValueAt( String.Format( USER_WITH_EXPRESSION_GLOBAL_ROLE, attributeRoleName ) );

                                if( attributeGlobalRole != null && attributeGlobalRole.Equals( GLOBAL_ROLE ) )
                                {
                                    this.IsGlobalRole = true;
                                }
                                else
                                {
                                    this.IsGlobalRole = false;
                                    nodeHSPCodeIterator = navigator.Select( String.Format( USER_WITH_EXPRESSION_HSP_CODE, attributeRoleName ) );
                                    while( nodeHSPCodeIterator.MoveNext() )
                                    {
                                        facilityCodes.Add( nodeHSPCodeIterator.Current.Value );
                                    }
                                }
                                this.BuildOrganizationalRelationshipsFor( domainUser, aRole, hierarchy, facilityCodes );
                            }
                        }
                        // this condition occurs when the user has been authenticated by ADAM but does not have
                        // any roles for the selected application
                        if( domainUser.Roles().Count == 0 )
                        {
                            domainUser = null;
                        }
                    }
                }
			}
			catch( SecurityException )
			{
				throw;
			}
			catch( Exception e )
			{
				throw new SecurityException( String.Format( ERR_READ_USER_PRIVILEGES, e.Message ), e, Severity.Catastrophic );
			}

			return domainUser;
		}

		#endregion

		#region Properties
        private Application Application
        {
            get
            {
                return ApplicationService;
            }
        }

		public static Application ApplicationService
		{
		    private get
			{
				return c_ApplicationService;
			}
			set
			{
				c_ApplicationService = value;
			}
		}
		#endregion

		#region Private Methods
        private Hashtable AllFacilitiesFrom( OrganizationalUnit hierarchy )
        {
            Hashtable facilities = new Hashtable();
            foreach( Facility facility in hierarchy.AllFacilities() )
            {     
                facilities.Add( facility.Code, facility );
            }        
            return facilities;
        }
        
        private void BuildOrganizationalRelationshipsFor( User user, 
                                                          Role role, 
                                                          OrganizationalUnit hierarchy, 
                                                          ArrayList facilityCodes )
        {
            ArrayList userFacilities = new ArrayList();

            if( this.IsGlobalRole )
            {   
                userFacilities = hierarchy.AllFacilities();                    //get all facilities from hierarchy
            }
            else if( facilityCodes.Count > 0 )
            {
                Hashtable allFacilities = this.AllFacilitiesFrom( hierarchy );
                foreach( string facilityCode in facilityCodes )
                {
                    Facility facility = (Facility)allFacilities[facilityCode]; //only facilities in hierarchy are accepted
                    if( facility != null )
                    {
                        userFacilities.Add( facility );
                    }
                }
            }
            foreach( Facility facility in userFacilities )
            {
                user.AddRelationship( new RoleRelationship( user, role, facility ) );

                ArrayList userLevels = this.OrganizationalHierarchyFor( facility, userFacilities );
                foreach( OrganizationalUnit organizationalUnit in userLevels )
                {
                    if( !user.HasRelationshipWith( organizationalUnit, role ) )
                    {
                        user.AddRelationship( new RoleRelationship( user, role, organizationalUnit ) );
                    }
                }
            }
        }
     

        

        private void EnsureProviderIsLoaded()
        {
            if( this.Provider == null )
            {
                throw new SecurityException( ERR_NO_PROVIDER_LOADED, Severity.Catastrophic );
            }
        }

        private ArrayList OrganizationalHierarchyFor( Facility facility, ICollection allFacilities )
        {
            Set hierarchy = new Set();
            OrganizationalUnit parent = facility.Parent() as OrganizationalUnit;
            while( parent != null )
            {
                if( parent.IsComplete( allFacilities ) )
                {
                    hierarchy.Add( parent );
                    parent = parent.Parent() as OrganizationalUnit;
                }
                else
                {
                    parent = null;
                }
            }

            return new ArrayList( hierarchy );
        }

        private void LoadProvider()
        {
            string providerPath = ConfigurationManager.AppSettings[CONFIG_PROVIDER_PATH];
            string providerClass = ConfigurationManager.AppSettings[CONFIG_PROVIDER_CLASS];

            this.ValidateProviderConfiguration( providerPath, providerClass );

            this.LoadPrivderFrom( providerPath, providerClass );
        }
        
        private void LoadPrivderFrom( string providerPath, string providerClass )
        {
            try
            {
                Assembly providerAssembly = Assembly.LoadFrom( providerPath );
                object instance = providerAssembly.CreateInstance( providerClass );
                this.Provider = (ISecurityProvider) instance;
                this.EnsureProviderIsLoaded();
            }
            catch( Exception loadingError )
            {
                string msg = String.Format( ERR_LOADING_PROVIDER, loadingError.Message );
                throw new SecurityException( msg, loadingError, Severity.Catastrophic );
            }
        }

		/// <summary>
		/// This method is called by RolesFor() to build 
		/// roles fromt he XML from ADAM.
		/// </summary>
		/// <param name="rolesInXml"></param>
		/// <returns></returns>
		private Hashtable RolesFrom( string rolesInXml )
		{
			//build roles from XML provided from ADAM.
			XmlDocument document = new XmlDocument();
			document.LoadXml( rolesInXml );
			XPathNavigator navigator = document.CreateNavigator();
			
			Hashtable roles = new Hashtable();
			
			string attributeName = String.Empty;
			string attributeAppRoleID = String.Empty;
			

			XPathNodeIterator nodeNameIterator;
			XPathNodeIterator nodeAppRoleIDIterator;
			nodeNameIterator =  navigator.Select( ROLES_FROM_EXPRESSION_NAME );
			nodeAppRoleIDIterator = navigator.Select( ROLES_FROM_EXPRESSION_APP_ROLE_ID );
			while( nodeNameIterator.MoveNext() && nodeAppRoleIDIterator.MoveNext() )
			{
				attributeName = nodeNameIterator.Current.Value;
				attributeAppRoleID = nodeAppRoleIDIterator.Current.Value;
				roles.Add( attributeAppRoleID, attributeName );
			}
			return roles;
		}   

        private void ValidateProviderConfiguration( string providerPath, string providerClass )
        {
            if( ( providerPath == null  ||  providerPath.Trim().Equals( String.Empty ) ) )
            {
                string msg = ERROR_INVALID_PROVIDER_PATH;
                throw new SecurityException( msg, Severity.Catastrophic );
            }

            if( ( providerClass == null  ||  providerClass.Trim().Equals( String.Empty ) ) )
            {
                string msg = ERROR_INVALID_PROVIDER_CLASS;
                throw new SecurityException( msg, Severity.Catastrophic );
            }
        }

        
		#endregion

		#region Private Properties

        private bool IsGlobalRole
        {
            get
            {
                return i_IsGlobalRole;
            }
            set
            {
                i_IsGlobalRole = value;
            }
        }

            private ISecurityProvider Provider
        {
            get
            {
                return i_Provider;
            }
            set
            {
                i_Provider = value;
            }
        }

		#endregion

		#region Construction and Finalization

		public SecurityService( ISecurityProvider provider )
		{
			this.Provider = provider;
			if( this.Provider == null )
			{
				this.LoadProvider();
			}
		}

		#endregion

		#region Data Elements

        private ISecurityProvider       i_Provider;
        private bool                    i_IsGlobalRole = false;
		private static Application      c_ApplicationService;
        
        #endregion

    }
}
