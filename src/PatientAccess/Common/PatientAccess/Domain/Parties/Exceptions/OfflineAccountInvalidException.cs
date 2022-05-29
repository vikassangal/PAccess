using System;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace PatientAccess.Domain.Parties.Exceptions
{
    /// <summary>
    /// Summary description for OfflineAccountInvalidException.
    /// </summary>
    [Serializable]
    public class OfflineAccountInvalidException : EnterpriseException
    {
        public OfflineAccountInvalidException() 
            : base()
        {
        }
        public OfflineAccountInvalidException(string msg)
            : base(msg)
        {
        }
        public OfflineAccountInvalidException (string msg,Exception ex)
            : base(msg,ex)
        {
        }
        public OfflineAccountInvalidException( string message, 
            Exception innerException,
            Severity severity )
            : base( message, innerException, severity )
        {
        }
        protected OfflineAccountInvalidException( SerializationInfo info, 
            StreamingContext context ) 
            : base( info, context )
        {
        }
    }
}
