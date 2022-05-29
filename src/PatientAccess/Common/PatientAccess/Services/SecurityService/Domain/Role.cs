using System;
using System.Collections;
using Peradigm.Framework.Persistence;

namespace Extensions.SecurityService.Domain
{
	/// <summary>
	/// Summary description for Role.
	/// </summary>
	/// 
	//TODO: Create XML summary comment for Role
	[Serializable]
	public class Role : PersistentModel
	{
		#region Constants
		#endregion

		#region Event Handlers
		#endregion

		#region Methods
        /// <summary>
        /// Add a role to my collection of members and answer the role I added.
        /// If the role has already been added, do not add the role again.
        /// </summary>
        /// <param name="role">
        /// A role to add to my collection.
        /// </param>
        /// <returns>
        /// The Role that was provided.
        /// </returns>
        public Role AddMember( Role role )
        {
            if( !this.PrimMembers.Contains( role ) )
            {
                this.PrimMembers.Add( role );
                role.PrimParents.Add( this );
            }
            return role;
        }

        public Privilege AddPrivilege( Privilege privilege )
        {
            if( !this.PrimPrivileges.Contains( privilege ) )
            {
                this.PrimPrivileges.Add( privilege );
            }
            return privilege;
        }

        public bool IsMemberOf( string roleName )
        {
            bool isRoleMember = this.Name == roleName;
            IEnumerator parents = this.PrimParents.GetEnumerator();
            Role parentRole = null;

            while( !isRoleMember && parents.MoveNext() )
            {
                parentRole = parents.Current as Role;
                isRoleMember = parentRole.IsMemberOf( roleName );
            }
            return isRoleMember;

        }

        /// <summary>
        /// Answer a collection of my privileges which include privileges for 
        /// any role of which I am a member.
        /// </summary>
        /// <returns>
        /// An ArrayList of privileges.
        /// </returns>
        public ArrayList Privileges()
        {
            ArrayList basePrivileges = (ArrayList)this.PrimPrivileges.Clone();
            foreach( Role parentRole in this.PrimParents )
            {
                basePrivileges.AddRange( parentRole.Privileges() );
            }
            return basePrivileges;
        }

        /// <summary>
        /// Remove a role from my collection of members
        /// </summary>
        /// <param name="role">
        /// A role to remove from my collection of members.
        /// </param>
        /// <returns>
        /// The Role that was provided.
        /// </returns>
        public Role RemoveMember( Role role )
        {
            if( this.PrimMembers.Contains( role ) )
            {
                this.PrimMembers.Remove( role );
                role.PrimParents.Remove( this );
            }
            return role;
        }

        public Privilege RemovePrivilege( Privilege privilege )
        {
            if( this.PrimPrivileges.Contains( privilege ) )
            {
                this.PrimPrivileges.Remove( privilege );
            }
            return privilege;
        }
		#endregion

		#region Properties
        public ArrayList Members
        {
            get
            {
                return (ArrayList)this.PrimMembers.Clone();
            }
        }

		public string Name
		{
			get
			{
				return i_Name;
			}
			set
			{
				i_Name = value;
			}
		}

        public string Description
        {
            get
            {
                return i_Description;
            }
            set
            {
                i_Description = value;
            }
        }                

        public ArrayList Parents
        {
            get
            {
                return (ArrayList)this.PrimParents.Clone();
            }
        }

		public Hashtable Roles
		{
			get
			{
				return i_Roles;
			}
			set
			{
				i_Roles = value;
			}
		}

		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
        private ArrayList PrimMembers
        {
            get
            {
                return i_Members;
            }
            set
            {
                i_Members = value;
            }
        }

        private ArrayList PrimParents
        {
            get
            {
                return i_Parents;
            }
            set
            {
                i_Parents = value;
            }
        }

        private ArrayList PrimPrivileges
        {
            get
            {
                return i_Privileges;
            }
            set
            {
                i_Privileges = value;
            }
        }
        #endregion

		#region Construction and Finalization
        public Role()
        {
        }
        public Role( long oid, string name, string description )
            : this( oid, name )
        {
            this.Description = description;
        }

		public Role( string name )
			: this( NEW_OID, name )
		{
		}

		public Role( long oid, string name )
			: this( oid, NEW_VERSION, name )
		{
		}


		private Role( long oid, byte[] version, string name)
			: base( oid, version )
		{
			this.Name = name;

            this.PrimMembers    = new ArrayList();
            this.PrimParents    = new ArrayList();
            this.PrimPrivileges = new ArrayList();
		}
		#endregion

		#region Data Elements
        private ArrayList   i_Members;
		private string      i_Name;
        private string      i_Description;
        private ArrayList   i_Parents;
        private ArrayList   i_Privileges;
		private Hashtable    i_Roles = new Hashtable ();
		#endregion
	}
}
