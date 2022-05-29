using System;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace PatientAccess.Domain
{
    [Serializable]
    public class InvalidEmailAddressException : EnterpriseException
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
        public InvalidEmailAddressException( string uri )
            : base( String.Format( MESSAGE, uri ) )
        {
        }

        public InvalidEmailAddressException( string message, Exception innerException )
            : base( message, innerException )
        {
        }

        public InvalidEmailAddressException( string message, Severity severity )
            : base( message, severity )
        {
        }


        public InvalidEmailAddressException( string message, 
            Exception innerException,
            Severity severity )
            : base( message, innerException, severity )
        {
        }

        public InvalidEmailAddressException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        private const string MESSAGE = "{0} is an invalid email address.";
        #endregion
    }
}
