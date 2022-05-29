using System;
using System.Runtime.Serialization;
using Peradigm.Framework.Domain.Exceptions;

namespace Peradigm.Framework.Domain.Parties.Exceptions
{
	[Serializable]
	public class ContactURINotFoundException : EnterpriseException
	{
        #region Constants

	    private const string 
			DEFAULT_MESSAGE			= "Requested Contact URI is not found.",
			CONTEXT_URI				= "URI";
		#endregion

		#region Properties

	    private string URI
		{
			get
			{
				return i_URI;
			}
			set
			{
				i_URI = value;
			}
		}
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public ContactURINotFoundException()
			: this( String.Empty )
		{
		}

		public ContactURINotFoundException( string uri )
			: this( uri, null )
		{
		}

		private ContactURINotFoundException( string uri, Exception innerException )
			: this( uri, innerException, Severity.Low )
		{
		}

		public ContactURINotFoundException( string uri, Severity severity )
			: this( uri, null, severity )
		{
		}
		
		private ContactURINotFoundException( string uri, Exception innerException, Severity severity )
			: base( DEFAULT_MESSAGE, innerException, severity )
		{
			this.URI = uri;
			if( uri != null )
			{
				this.AddContextItem( CONTEXT_URI, uri );
			}
		}
		
		public ContactURINotFoundException( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
		}
		#endregion

		#region Data Elements
		private string i_URI;
		#endregion
	}
}
