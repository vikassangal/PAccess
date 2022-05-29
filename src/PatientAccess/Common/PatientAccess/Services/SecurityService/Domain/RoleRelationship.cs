using System;
using Peradigm.Framework.Domain.Parties;

namespace Extensions.SecurityService.Domain
{
	/// <summary>
	/// Summary description for RoleRelationship.
	/// </summary>
	/// 
	//TODO: Create XML summary comment for RoleRelationship
	[Serializable]
	public class RoleRelationship : Relationship
	{
		#region Event Handlers
		#endregion

		#region Methods
		#endregion

		#region Properties
		
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
			set
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
		{
		}

		public RoleRelationship( User user, Role role, AbstractOrganizationalUnit organizationalUnit )
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
