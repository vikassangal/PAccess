using System;
using System.Collections;
using Peradigm.Framework.Domain.Parties;

//using Peradigm.Framework.Persistence;

namespace Extensions.SecurityService.Domain
{
	/// <summary>
	/// Summary description for Application.
	/// </summary>
	/// 
	//TODO: Create XML summary comment for Application
	[Serializable]
	public class Application : Party
	{
		#region Constants
		#endregion

		#region Event Handlers
		#endregion

		#region Methods
		public void AddRole( Role aRole )
		{
			this.PrimRoles.Add( aRole.Name, aRole );
		}

		public Role RoleWith( string roleName )
		{
            Role selectedRole = null;
            if( this.PrimRoles.ContainsKey( roleName ) )
            {
                selectedRole = (Role)this.PrimRoles[roleName];
            }
			return selectedRole;
		}
		#endregion

		#region Properties

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
		
		public Hashtable Roles
		{
			get
			{
				return this.PrimRoles.Clone() as Hashtable;
			}
			
		}
		
        
		override public string Name
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

		
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties

	    private Hashtable PrimRoles
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

		#region Construction and Finalization
		public Application( string accessKey, string applicationGUID, string name )
		{
            this.AccessKey = accessKey;
			this.ApplicationGUID = applicationGUID;
			this.Name = name;
		}
		#endregion

		#region Data Elements
		private string i_AccessKey;
		private string i_ApplicationGUID;
		private string i_Name;
		private Hashtable i_Roles = new Hashtable();
		#endregion
	}
}
