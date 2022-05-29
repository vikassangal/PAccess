using System;
using System.Collections;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace Extensions.DB2Persistence
{
	/// <summary>
	/// Summary description for TimeoutException.
	/// </summary>
	[Serializable]
	public class TimeoutException : EnterpriseException
	{
		#region Event Handlers
		#endregion

		#region Methods
		#endregion

		#region Properties
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public TimeoutException( string message )
			: base( message )
		{
		}

		public TimeoutException( string message, Severity severity )
			: base( message, severity )
		{
		}

		public TimeoutException( string message, Severity severity, Hashtable context )
			: base( message, severity, context )
		{
		}

		public TimeoutException( string message, Exception innerException )
			: base( message, innerException )
		{
		}

		public TimeoutException( string message, 
			Exception innerException,
			Severity severity )
			: base( message, innerException, severity )
		{
		}

		public TimeoutException( string message, 
			Exception innerException, 
			Severity severity, 
			Hashtable context )
			: base( message, innerException, severity, context )
		{
		}

		protected TimeoutException( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
		}
		#endregion

		#region Data Elements
		#endregion
	}	
}
