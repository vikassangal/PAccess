using System;
using System.Collections;
using System.Text;
using Peradigm.Framework.Domain.Collections;
using Peradigm.Framework.Domain.Parties;

namespace Extensions.SecurityService.Domain
{
	/// <summary>
	/// Summary description for User.
	/// </summary>
	/// 
	//TODO: Create XML summary comment for User
	[Serializable]
	public class User : Person
	{
        #region Constants
        private const string FORMAT_UPN = "{0}.{1}@{2}";
        #endregion

        #region Event Handlers
		#endregion

		#region Methods
        public string DefaultUPNUsing( string domainName )
        {
            return string.Format( FORMAT_UPN, this.FirstName, this.LastName, domainName );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relationship"></param>
		public void AddRelationship( RoleRelationship relationship )
		{
			base.AddRelationship( relationship );
		}

        public ArrayList AllFacilities()
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
        public bool HasPermissionTo( Privilege privilege, Facility facility )
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
        public bool HasPermissionTo( Privilege.Actions action, object context, Facility facility )
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

        public bool IsInRole( string roleName )
        {
            return this.FacilitiesForRole( roleName ).Count > 0;
        }

		public override string ToString()
		{
			StringBuilder res = new StringBuilder( base.ToString() );
			res.Append( "TenetId: " + this.TenetID );
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

        public ArrayList Roles()
		{
			ArrayList roles = new ArrayList();
            RoleRelationship relationship = null;

			foreach( Relationship r in this.Relationships  )
			{
				if( r is RoleRelationship )
				{
                    relationship = (RoleRelationship)r;
					roles.Add( relationship.Role );
				}
			}
			return roles;
		}

        /// <summary>
        /// Answer a collection of all roles for a given facily for which I am a member
        /// </summary>
        /// <param name="facility">Facility of interest</param>
        /// <returns>An ArrayList of roles</returns>
        public ArrayList RolesFor( Facility facility )
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

        public bool HasRelationshipWith( AbstractOrganizationalUnit organizationalUnit, Role role )
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
		public long TenetID
		{
			get
			{
				// Dima Frenkel  1/31/2004: Tenet ID is the same as OID
                // return i_TenetID;
                return this.Oid;
			}
			set
			{
                // Dima Frenkel  1/31/2004: Tenet ID is the same as OID
                // i_TenetID = value;
                this.Oid = value;
			}
		}
		public string UPN
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

        public string EmployeeID
        {
            get
            {
                return i_EmployeeID;
            }
            set
            {
                //i_EmployeeID is the same as Tenet ID 
                i_EmployeeID = value;
            }
        }


        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public User()
		{
			this.UPN = String.Empty;
		}

		public User( long tenetID, string firstName, string lastName )
			: base( tenetID, NEW_VERSION, firstName, lastName )
		{
			this.UPN = String.Empty;
		}

        public User( long tenetID, string fullName )
            : base( tenetID, fullName )
        {
			this.UPN = String.Empty;
        }
		#endregion

		#region Data Elements
		private string i_UPN, i_EmployeeID;
        // Dima Frenkel  1/31/2004: Tenet ID is the same as OID
        // private long i_TenetID;
        #endregion
    }

}
