using System;
using System.Text;
using System.Collections;
using System.Security.Principal;

using Peradigm.Framework.Domain.Collections;
using Peradigm.Framework.Domain.Parties;
using Peradigm.Framework.Persistence;

namespace Peradigm.Framework.Domain.Security
{
	/// <summary>
	/// User class represents application user, including his security credentials, 
    /// roles, facilities and privileges.
	/// </summary>
	[Serializable]
	public class User : Person, IIdentity, IPrincipal
	{
        #region Constants
        private const string 
            FORMAT_UPN                  = "{0}.{1}@{2}",
            DEFAULT_AUTHENTICATION_TYPE = "Custom";
        #endregion

        #region Event Handlers
		#endregion

		#region Methods
        /// <summary>
        /// Answer with default, canonical UPN for a given domain name.
        /// </summary>
        /// <param name="domainName">Domain Name for which default UPN is needed.</param>
        /// <returns>Value of default UPN</returns>
        public string DefaultUPNUsing( string domainName )
        {
            return string.Format( FORMAT_UPN, this.FirstName, this.LastName, domainName );
        }

        /// <summary>
        /// Add RoleRelashionship to my roles
        /// </summary>
        /// <param name="relationship">RoleRelationship instance to add</param>
		public void AddRelationship( RoleRelationship relationship )
		{
			base.AddRelationship( relationship );
		}

		/// <summary>
		/// Add context-specific credentials for an third-party/legacy application.
        /// Context is specific to the application. 
        /// For example, for legacy applications where user credentials are different at every facility,
        /// context could be an instance of a Facility.
        /// For legacy applications with single username/password, context could be a hard-coded string.
		/// </summary>
		/// <param name="application">Application name</param>
		/// <param name="credentials">Credentials to add</param>
		/// <param name="context">Context which which credentials are associated. May not be null.</param>
		/// <returns>Credentials added</returns>
        public Credentials AddCredentials( string application, Credentials credentials, object context )
		{
			Hashtable applicationCredentials = this.PrimCredentialsFor( application );
			if( ( applicationCredentials != null ) && ( context != null ) )
			{
				applicationCredentials[context] = credentials;
				return credentials;
			}
			else
			{
				return null;
			}

		}

        /// <summary>
        /// Remove a context-specific credentials for an third-party/legacy application.
        /// </summary>
        /// <param name="application">Application name</param>
        /// <param name="context">Context which which credentials are associated. May not be null.</param>
        /// <returns>Credentials removed or null if credentials were not found</returns>
        public Credentials RemoveCredentials( string application, object context )
		{
			Credentials removedCredentials = null;
			Hashtable applicationCredentials = this.PrimCredentialsFor( application );
			if( ( applicationCredentials != null ) && ( context != null ) )
			{
				removedCredentials = applicationCredentials[context] as Credentials;
				applicationCredentials.Remove( context );
			}
			return removedCredentials;
		}

        /// <summary>
        /// Answer with context-specific credentials for an third-party/legacy application.
        /// Context is specific to the application. 
        /// For example, for legacy applications where user credentials are different at every facility,
        /// context could be an instance of a Facility.
        /// For legacy applications with single username/password, context could be a hard-coded string.
        /// </summary>
        /// <param name="application">Application name</param>
        /// <param name="context">Context which which credentials are associated. May not be null.</param>
        /// <returns>Credentials instance or null if not found</returns>
        public Credentials CredentialsFor( string application, object context )
		{
			Hashtable applicationCredentials = this.PrimCredentialsFor( application );
			Credentials credentials = null;
			if( ( applicationCredentials != null ) && ( context != null ) )
			{
				credentials = applicationCredentials[context] as Credentials;
			}
			
			return credentials;
		}

        /// <summary>
        /// Answer with all (for all contexts) credentials for an third-party/legacy application.
        /// </summary>
        /// <param name="application">Application name</param>
        /// <returns>Hashtable of credentials instances, where keys are Contexts and Values are Credentials</returns>
        public Hashtable CredentialsFor( string application )
		{
			Hashtable applicationCredentials = this.PrimCredentialsFor( application );
			if( applicationCredentials == null )
			{
				applicationCredentials = new Hashtable();
			} 
			else
			{
				applicationCredentials = applicationCredentials.Clone() as Hashtable;
			}

			return applicationCredentials;
		}

		/// <summary>
		/// Clear all legacy/third-party credentials
		/// </summary>
        public void ClearCredentials()
		{
			this.primCredentials.Clear();
		}


        /// <summary>
        /// Answer all Facilities in which this user plays one or more Roles
        /// </summary>
        /// <returns>ArrayList of matching Facility instances</returns>
        private ArrayList AllFacilities()
        {
            Set facilities = new Set();
            foreach( Relationship r in this.Relationships )
            {
                if( r is RoleRelationship )
                {
                    RoleRelationship rr = (RoleRelationship)r;
                    if( rr.OrganizationalUnit is Facility )
                    {
                        facilities.Add( rr.OrganizationalUnit );
                    }
                }
            }
            return facilities.AsArrayList();
        }
		
		/// <summary>
		/// Answer all Facilities in which this user plays a role with a given name
		/// </summary>
		/// <param name="roleName">Name of the application role</param>
        /// <returns>ArrayList of matching Facility instances</returns>
		private ArrayList FacilitiesForRole( string roleName )
		{
			Set facilities = new Set();
			foreach( Relationship r in this.Relationships )
			{
				if( r is RoleRelationship )
				{
					RoleRelationship rr = (RoleRelationship)r;
					if( rr.Role.IsMemberOf( roleName ) && rr.OrganizationalUnit is Facility )
					{
						facilities.Add( rr.OrganizationalUnit );
					}
				}
			}
			return facilities.AsArrayList();
		}

		/// <summary>
        /// Answer all Facilities in which this user has been granted a given permission/privilege.
        /// Note that privileges are granted through role membership.
        /// </summary>
		/// <param name="privilege">Privilege instance</param>
        /// <returns>ArrayList of matching Facility instances</returns>
		private ArrayList FacilitiesForPermission( Privilege privilege )
		{
			Set facilities = new Set();
			foreach( Facility f in this.AllFacilities() )
			{
				if( this.HasPermissionTo( privilege, f ) )
				{
					facilities.Add( f );
				}
			}
			return facilities.AsArrayList();
		}

        /// <summary>
        /// Answer all Facilities in which this user has been granted a 
        /// given permission/privilege, expressed in a form of Action + Context.
        /// Note that privileges are granted through role membership.
        /// </summary>
        /// <param name="action">Action part of the Privilege</param>
        /// <param name="context">Context part of the Privilege</param>
        /// <returns>ArrayList of matching Facility instances</returns>
        public ArrayList FacilitiesForPermission( Privilege.Actions action, object context )
		{
			return this.FacilitiesForPermission( new Privilege( action, context ) );
		}

        /// <summary>
        /// Answer is this user has been granted a given permission/privilege, 
        /// in any facility through membership in any role.
        /// This method should be used by the client application in most cases.
        /// It is preferred to IsInRole method.
        /// </summary>
        /// <param name="privilege">Privilege to check</param>
        /// <returns>True is this user has been granted a given privilege</returns>
        /// <seealso cref="Peradigm.Framework.Domain.Security.User.IsInRole(string)"/>
        public bool HasPermissionTo( Privilege privilege )
		{
			bool hasPermission = false;
			foreach( Privilege p in this.Privileges()  )
			{
				hasPermission = p.Equals( privilege );
                if( hasPermission )
                {
				    break;
                }
			}
			return hasPermission;
		}

        /// <summary>
        /// Answer is this user has been granted a given permission/privilege, 
        /// in any facility through membership in any role.
        /// Privilege is expressed in a form of Action + Context.
        /// This method should be used by the client application in most cases.
        /// It is preferred to IsInRole method.
        /// </summary>
        /// <param name="action">Action part of the Privilege</param>
        /// <param name="context">Context part of the Privilege</param>
        /// <returns>True is this user has been granted a given privilege</returns>
        public bool HasPermissionTo( Privilege.Actions action, object context )
		{
			bool hasPermission = false;
			foreach( Privilege p in this.Privileges()  )
			{
				hasPermission = p.Equals( action, context );
                if( hasPermission )
                {
				    break;
                }
			}
			return hasPermission;
		}

        
        /// <summary>
        /// Answer if I have permission defined by a given privilege for a given facility
        /// </summary>
        /// <param name="privilege">Privilege in question</param>
        /// <param name="facility">Facility of interest</param>
        /// <returns>True if user has permission, false otherwise</returns>
        private bool HasPermissionTo( Privilege privilege, Facility facility )
        {
            return this.HasPermissionTo( privilege.Action, privilege.Context, facility );
        }

        /// <summary>
        /// Answer if I have permission to peform a particular action over a given context object for a given facility
        /// </summary>
        /// <param name="action">Action (Add, Delete, etc.) </param>
        /// <param name="context">Contect object or class</param>
        /// <param name="facility">Facility of interest</param>
        /// <returns>True if user has permission, false otherwise</returns>
        private bool HasPermissionTo( Privilege.Actions action, object context, Facility facility )
        {
            bool hasPermission = false;
            foreach( Privilege p in this.PrivilegesFor( facility ) )
            {
                hasPermission = p.Equals( action, context );
                if( hasPermission )
                {
                    break;
                }
            }

            return hasPermission;
        }

        /// <summary>
        /// Answer is this user plays a given role at any facility
        /// </summary>
        /// <param name="roleName">Name of the Role</param>
        /// <returns>True is user is in the given role, false otherwise</returns>
        public bool IsInRole( string roleName )
        {
            return this.FacilitiesForRole( roleName ).Count > 0;
        }

		public override string ToString()
		{
			StringBuilder res = new StringBuilder( base.ToString() );
			res.Append( "EmployeeId: " + this.EmployeeID );
			return res.ToString();
		}

        /// <summary>
        /// Answer if I am a member of a given role for for a given facility.
        /// </summary>
        /// <param name="roleName">Name of the role</param>
        /// <param name="facility">Facility of interest</param>
        /// <returns>True if user is a member of the role for the facility</returns>
        public bool IsInRole( string roleName, Facility facility )
        {
            bool hasAccess = false;
            if( roleName != null && facility != null )
            {
                ArrayList facilities = this.FacilitiesForRole( roleName );
                foreach( Facility indexFacility in facilities )
                {
                    hasAccess = indexFacility.Equals( facility );
                    if( hasAccess )
                    {
                        break;
                    }
                }
            }
            return hasAccess;
        }

        /// <summary>
        /// Answer with full collection of my privileges
        /// </summary>
        /// <returns>An ArrayList of Permission instances</returns>
        private ArrayList Privileges()
		{
			ArrayList privileges = new ArrayList();

			foreach( Role r in this.Roles() )
			{
				privileges.AddRange( r.Privileges() );
			}

			return privileges;
		}

        /// <summary>
        /// Answer a collection of my privileges for a given facility for any role of which I am a member.
        /// </summary>
        /// <param name="facility">Facility of interest</param>
        /// <returns>An ArrayList of Privileges</returns>
        private ArrayList PrivilegesFor( Facility facility )
        {
            Set privilegesForFacility = new Set();
            foreach( Role r in this.RolesFor( facility ) )
            {
                privilegesForFacility.AddRange( r.Privileges() );
            }
            return privilegesForFacility.AsArrayList();
        }

        /// <summary>
        /// Answer a collection of all roles for all facilies for which I am a member.
        /// </summary>
        /// <returns>An ArrayList of Role instances</returns>
        private ArrayList Roles()
		{
			Hashtable roles = new Hashtable();
            RoleRelationship relationship = null;

			foreach( Relationship r in this.Relationships  )
			{
				if( r is RoleRelationship )
				{
                    relationship = (RoleRelationship)r;
					roles[relationship.Role.Name] = relationship.Role;
				}
			}
			return new ArrayList( roles.Values );
		}

        /// <summary>
        /// Answer a collection of all roles for a given facily for which I am a member
        /// </summary>
        /// <param name="facility">Facility of interest</param>
        /// <returns>An ArrayList of roles</returns>
        private ArrayList RolesFor( Facility facility )
        {
            Set rolesForFacility = new Set();
            Facility roleFacility = null;
            if( facility != null )
            {
                foreach( Relationship relationship in this.primRelationships )
                {
                    if( relationship is RoleRelationship )
                    {
                        RoleRelationship roleRelationship = (RoleRelationship) relationship;
                        if( roleRelationship.OrganizationalUnit is Facility )
                        {
                            roleFacility = roleRelationship.OrganizationalUnit as Facility;
                            if( roleFacility.Equals( facility ) ) 
                            {
                                rolesForFacility.Add( roleRelationship.Role );
                            }
                        }
                    }
                }
            }
            return rolesForFacility.AsArrayList();
        }

	    private bool HasRelationshipWith( AbstractOrganizationalUnit organizationalUnit )
        {
            return this.HasRelationshipWith( organizationalUnit, null ); 
        }

	    private bool HasRelationshipWith( AbstractOrganizationalUnit organizationalUnit, Role role )
        {
            foreach( Relationship relationship in this.primRelationships )
            {
                if( relationship is RoleRelationship )
                {
                    RoleRelationship roleRelationship = (RoleRelationship)relationship;
                    if( roleRelationship.OrganizationalUnit.Equals( organizationalUnit ) &&
                        ( role == null || roleRelationship.Role.IsMemberOf( role.Name ) ) )
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public ArrayList OrganizationalHierarchy()
        {
            AbstractOrganizationalUnit level;
            AbstractOrganizationalUnit parent;
            Set levels = new Set();
            foreach( Relationship relationship in this.primRelationships )
            {
                RoleRelationship roleRelationship = relationship as RoleRelationship;
                if( roleRelationship != null )
                {
                    level = roleRelationship.OrganizationalUnit;
                    parent = level.Parent();
                    while( ( parent!= null ) && this.HasRelationshipWith( parent ) )
                    {
                        level = level.Parent();
                        parent = level.Parent();
                    }
                    levels.Add( level );
                }
            }
            return new ArrayList( levels );
        }

        public ArrayList OrganizationalHierarchyFor( Role role )
        {
            AbstractOrganizationalUnit level;
            AbstractOrganizationalUnit parent;
            Set levels = new Set();
            foreach( Relationship relationship in this.primRelationships )
            {
                RoleRelationship roleRelationship = relationship as RoleRelationship;
                if( roleRelationship != null && roleRelationship.Role.IsMemberOf( role.Name ) )
                {
                    level = roleRelationship.OrganizationalUnit;
                    parent = level.Parent();
                    while( ( parent!= null ) && this.HasRelationshipWith( parent, role ) )
                    {
                        level = level.Parent();
                        parent = level.Parent();
                    }
                    levels.Add( level );
                }
            }
            return new ArrayList( levels );
        }
        #endregion

		#region Properties
        public string AuthenticationType
        {
            get
            {
                return i_AuthenticationType;
            }
            private set
            {
                i_AuthenticationType = value;
            }
        }

	    private long EmployeeID
		{
			get
			{
                return this.Oid;
			}
			set
			{
                this.Oid = value;
			}
		}

        public bool IsAuthenticated
        {
            get
            {
                return true;
            }
        }

        public IIdentity  Identity
        {
            get
            {
                return this;
            }
        }

	    private string GUID
        {
            get
            {
                return i_GUID;
            }
            set
            {
                i_GUID = value;
            }
        }


	    private string UPN
		{
			get
			{
				return i_UPN;
			}
			set
			{
				i_UPN = value;
			}
		}

		#endregion

		#region Private Methods
		private Hashtable PrimCredentialsFor( string application )
		{
			Hashtable applicationCredentials = null;
			if( application != null )
			{
				if( !this.primCredentials.ContainsKey( application ) )
				{
					this.primCredentials.Add( application, new Hashtable() );
				}
				applicationCredentials = this.primCredentials[application] as Hashtable;
			}

			return applicationCredentials;
		}

		#endregion

		#region Private Properties
		private Hashtable primCredentials
		{
			get
			{
				return i_Credentials;
			}
			set
			{
				i_Credentials = value;
			}
		}
		#endregion

		#region Construction and Finalization
		public User()
			: this( PersistentModel.NEW_OID, String.Empty, String.Empty )
		{
		}

		private User( long employeeID, string firstName, string lastName )
			: base( employeeID, PersistentModel.NEW_VERSION, firstName, lastName )
		{
			this.UPN = String.Empty;
			this.primCredentials = new Hashtable();
            this.AuthenticationType = DEFAULT_AUTHENTICATION_TYPE;
            this.GUID = String.Empty;
        }

        public User( long employeeID, string fullName )
            : base( employeeID, fullName )
        {
			this.UPN = String.Empty;
			this.primCredentials = new Hashtable();
            this.AuthenticationType = DEFAULT_AUTHENTICATION_TYPE;
            this.GUID = String.Empty;
        }
		#endregion

		#region Data Elements
		private string i_UPN;
		private Hashtable i_Credentials;
        private string i_AuthenticationType;
        private string i_GUID;
        #endregion
	}
	
}
