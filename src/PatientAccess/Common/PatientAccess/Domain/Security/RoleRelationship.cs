using System;

using Peradigm.Framework.Domain.Parties;

namespace Peradigm.Framework.Domain.Security
{
	/// <summary>
	/// Summary description for RoleRelationship.
	/// </summary>
	/// 
	//TODO: Create XML summary comment for RoleRelationship
	[Serializable]
	public class RoleRelationship : Relationship
    {
        #region Constants
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public override RelationshipType Type
        {
            get
            {
                return new RelationshipType( this.GetType().Name );
            }
            set
            {
                // can not override RoleRelationship Type
            }
        }
		
		public AbstractOrganizationalUnit OrganizationalUnit
		{
			get
			{
				return this.SecondParty as AbstractOrganizationalUnit;
			}
			set
			{
				this.SecondParty = value;
			}
		}

		public Role Role
		{
			get
			{
				return i_Role;
			}
		    private set
			{
				i_Role = value;
			}
		}

		public User User
		{
			get
			{
				return this.FirstParty as User;
			}
			set
			{
				this.FirstParty = value;
			}
		}
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public RoleRelationship()
            :this( new User(), new Role( String.Empty ), new OrganizationalUnit() )
		{
		}

		private RoleRelationship( User user, Role role, AbstractOrganizationalUnit organizationalUnit )
			: base( new RelationshipType(), user, organizationalUnit )
		{
			this.Role = role;
		}
		#endregion

		#region Data Elements
		private Role i_Role;
		#endregion
	}
}
