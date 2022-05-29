using System;

namespace Peradigm.Framework.Domain.Security
{
	/// <summary>
	/// Generic Credential class contains username and password.
	/// Can be attached to a user (meaning credentials for login) or any othet context.
	/// </summary>
	[Serializable]
	public class Credentials : Model
	{
		#region Event Handlers
		#endregion

		#region Methods
		#endregion

		#region Properties

	    private string UserName
		{
			get
			{
				return i_UserName;
			}
			set
			{
				i_UserName = value;
				if( value == null )
				{
					i_UserName = String.Empty;
				}
			}
		}

	    private string Token
		{
			get
			{
				return i_Token;
			}
			set
			{
				i_Token = value;
				if( value == null )
				{
					i_Token = String.Empty;
				}
			}
		}
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public Credentials():
			this( String.Empty, String.Empty )
		{
		}

		private Credentials( string userName, string token )
		{
			this.UserName = userName;
			this.Token = token;
		}
		#endregion

		#region Data Elements
		private string i_UserName;
		private string i_Token;
		#endregion

		#region Constants
		#endregion
	}
}
