using System;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace PatientAccess.Domain.Parties.Exceptions
{
    /// <summary>
    /// Summary description for ServiceUnavailableException.
    /// </summary>
    //TODO: Create XML summary comment for ServiceUnavailableException
    [Serializable]
    public class ServiceUnavailableException : EnterpriseException
    {

        #region Construction and Finalization
        public ServiceUnavailableException()
        {
        }
        public ServiceUnavailableException( string msg )
            : base(msg)
        {
        }
        public ServiceUnavailableException( string msg, Exception ex )
            : base(msg, ex)
        {
        }
        public ServiceUnavailableException( string message,
            Exception innerException,
            Severity severity)
            : base( message, innerException, severity )
        {
        }
        protected ServiceUnavailableException( SerializationInfo info, StreamingContext context )
            : base(info, context)
        {
        }
        #endregion

    }
}
