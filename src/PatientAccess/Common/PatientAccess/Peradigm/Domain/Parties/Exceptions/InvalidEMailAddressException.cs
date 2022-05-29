using System;
using System.Runtime.Serialization;
using Peradigm.Framework.Domain.Exceptions;

namespace Peradigm.Framework.Domain.Parties.Exceptions
{
    [Serializable]
    public class InvalidEMailAddressException : EnterpriseException
    {
        #region Constants

        private const string
            DEFAULT_MESSAGE = "The supplied e-mail address is not valid",
            CONTEXT_KEY     = "EMail";
        #endregion

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
        public InvalidEMailAddressException()
            : this( DEFAULT_MESSAGE )
        {
        }

        public InvalidEMailAddressException( string email )
            : this( email, null )
        {
        }

        private InvalidEMailAddressException( string email, Exception innerException )
            : this( email, innerException, Severity.Low )
        {
        }

        public InvalidEMailAddressException( string email, Severity severity )
            : this( email, null, severity )
        {
        }
		
        private InvalidEMailAddressException( string email, Exception innerException, Severity severity )
            : base( DEFAULT_MESSAGE, innerException, severity )
        {
            if( email != null )
            {
                this.AddContextItem( CONTEXT_KEY, email );
            }
        }
		
        public InvalidEMailAddressException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }
        #endregion

        #region Data Elements
        #endregion
    }
}