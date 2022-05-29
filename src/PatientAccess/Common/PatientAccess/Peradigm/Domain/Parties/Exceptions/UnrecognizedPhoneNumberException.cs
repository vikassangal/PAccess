using System;
using System.Runtime.Serialization;
using Peradigm.Framework.Domain.Exceptions;

namespace Peradigm.Framework.Domain.Parties.Exceptions
{
	[Serializable]
	public class UnrecognizedPhoneNumberException : EnterpriseException
	{
		#region Constants

	    private const string 
			DEFAULT_MESSAGE			= "The supplied phone number could not be parsed into the proper components.",
			CONTEXT_PHONE_NUMBER	= "PhoneNumber";
		#endregion

		#region Event Handlers
		#endregion

		#region Methods
		#endregion

		#region Properties

	    private string PhoneNumber
		{
			get
			{
				return i_PhoneNumber;
			}
			set
			{
				i_PhoneNumber = value;
			}
		}
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public UnrecognizedPhoneNumberException()
			: this( String.Empty )
		{
		}

		public UnrecognizedPhoneNumberException( string phoneNumber )
			: this( phoneNumber, null )
		{
		}

		private UnrecognizedPhoneNumberException( string phoneNumber, Exception innerException )
			: this( phoneNumber, innerException, Severity.Low )
		{
		}

		public UnrecognizedPhoneNumberException( string phoneNumber, Severity severity )
			: this( phoneNumber, null, severity )
		{
		}
		
		private UnrecognizedPhoneNumberException( string phoneNumber, Exception innerException, Severity severity )
			: base( DEFAULT_MESSAGE, innerException, severity )
		{
			this.PhoneNumber = phoneNumber;
			if( phoneNumber != null )
			{
				this.AddContextItem( CONTEXT_PHONE_NUMBER, phoneNumber );
			}
		}
		
		public UnrecognizedPhoneNumberException( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
		}
		#endregion

		#region Data Elements
		private string i_PhoneNumber;
		#endregion
	}
}
