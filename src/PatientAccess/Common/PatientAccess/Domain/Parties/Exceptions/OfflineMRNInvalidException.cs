using System;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace PatientAccess.Domain.Parties.Exceptions
{
	/// <summary>
	/// Summary description for OfflineMRNInvalidException.
	/// </summary>
    [Serializable]
    public class OfflineMRNInvalidException : EnterpriseException
    {
        public OfflineMRNInvalidException() 
            : base()
        {
        }
        public OfflineMRNInvalidException(string msg)
            : base(msg)
        {
        }
        public OfflineMRNInvalidException (string msg,Exception ex)
            : base(msg,ex)
        {
        }
        public OfflineMRNInvalidException( string message, 
            Exception innerException,
            Severity severity )
            : base( message, innerException, severity )
        {
        }
        protected OfflineMRNInvalidException( SerializationInfo info, 
            StreamingContext context ) 
            : base( info, context )
        {
        }
    }
}
