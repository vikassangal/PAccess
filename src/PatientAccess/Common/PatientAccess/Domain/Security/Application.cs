using System;
using System.Collections;

using Peradigm.Framework.Domain.Parties;

namespace Peradigm.Framework.Domain.Security
{
	/// <summary>
	/// Summary description for Application.
	/// </summary>
	[Serializable]
	public class Application : Party
	{
		#region Constants
		#endregion

		#region Event Handlers
		#endregion

		#region Methods

	    private void AddRole( Role aRole )
		{
			this.PrimRoles.Add( aRole.Name, aRole );
		}

	    private Role RoleWith( string roleName )
		{
            Role selectedRole = null;
            if( this.PrimRoles.ContainsKey( roleName ) )
            {
                selectedRole = (Role)this.PrimRoles[roleName];
            }
			return selectedRole;
		}

		public void AddApplication( Application childApp )
		{
			if( this.PrimApplications.Contains( childApp ) )
			{
				this.RemoveApplication( childApp );
			}
			childApp.AddTo( this );
			this.PrimApplications[childApp.Name] = childApp;
		}

	    private void RemoveApplication( Application childApp )
		{
			if( this.PrimApplications.Contains( childApp.Name ) )
			{
				childApp.RemoveFrom( this );
				this.PrimApplications.Remove( childApp.Name );
			}
		}

	    private void AddTo( Application parentApp )
		{
			IDictionaryEnumerator roles = this.PrimRoles.GetEnumerator();

			while( roles.MoveNext() )
			{
				this.AddRoleTo( parentApp, (Role)roles.Value );
			}
		}

	    private void RemoveFrom( Application parentApp )
		{
			IDictionaryEnumerator roles = this.PrimRoles.GetEnumerator();

			while( roles.MoveNext() )
			{
				this.RemoveRoleFrom( parentApp, (Role)roles.Value );
			}
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

		public Hashtable Applications
		{
			get
			{
				return this.PrimApplications.Clone() as Hashtable;
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

	    private string ServerName
		{
			get
			{
				return i_ServerName;
			}
			set
			{
				i_ServerName = value;
			}
		}

	    private string BuildVersion
		{
			get
			{
				return i_BuildVersion;
			}
			set
			{
				i_BuildVersion = value;
			}
		}

	    private string Environment
		{
			get
			{
				return i_Environment;
			}
			set
			{
				i_Environment = value;
			}
		}
		#endregion

		#region Private Methods
		private void AddRoleTo( Application parentApp, Role roleToAdd )
		{
			Role parentRole = parentApp.RoleWith( roleToAdd.Name );
			
			if( parentRole != null )
			{
				foreach( Privilege privilege in roleToAdd.Privileges() )
				{
					parentRole.AddPrivilege( privilege );
				}
			}
			else
			{
				parentApp.AddRole( roleToAdd );
			}
		}

		private void RemoveRoleFrom( Application parentApp, Role roleToRemove )
		{
			Role parentRole = parentApp.RoleWith( roleToRemove.Name );
			
			if( parentRole != null )
			{
				foreach( Privilege privilege in roleToRemove.Privileges() )
				{
					parentRole.RemovePrivilege( privilege );
				}
			}
		}
		#endregion
		
		#region Private Properties

	    private Hashtable PrimRoles
		{
			get
			{
				return this.i_Roles;
			}
			set
			{
				this.i_Roles = value;
			}
		}

	    private Hashtable PrimApplications
		{
			get
			{
				return this.i_Applications;
			}
			set
			{
				this.i_Applications = value;
			}
		}
		#endregion
		
		#region Construction and Finalization
		public Application( string accessKey,string applicationGUID,string name )
			: this( accessKey,
			        applicationGUID,
			        name,
			        string.Empty,
			        string.Empty,
			        string.Empty )
		{           
		}

		public Application( string accessKey,
			                string applicationGUID,
			                string name,
							string environment,
			                string buildVersion,
		                    string serverName )
		{	
			this.AccessKey = accessKey;
			this.ApplicationGUID = applicationGUID;
			this.Name = name;
			this.Environment = environment;
			this.BuildVersion = buildVersion;
			this.ServerName = serverName;
			this.PrimApplications = new Hashtable();
		}
		#endregion
		
		#region Data Elements
		private string i_AccessKey;
		private string i_ApplicationGUID;
		private string i_Name;
		private string i_BuildVersion;
		private string i_ServerName;
		private string i_Environment;
		private Hashtable i_Roles = new Hashtable();
		private Hashtable i_Applications;
		#endregion
	}
}
