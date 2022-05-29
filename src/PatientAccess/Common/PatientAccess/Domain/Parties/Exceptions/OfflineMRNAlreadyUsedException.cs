using System;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace PatientAccess.Domain.Parties.Exceptions
{
	/// <summary>
	/// Summary description for OfflineMRNAlreadyUsedException.
	/// </summary>
    [Serializable]
    public class OfflineMRNAlreadyUsedException : EnterpriseException
    {
        public OfflineMRNAlreadyUsedException() 
            : base()
        {
        }
        public OfflineMRNAlreadyUsedException(string msg)
            : base(msg)
        {
        }
        public OfflineMRNAlreadyUsedException (string msg,Exception ex)
            : base(msg,ex)
        {
        }
        public OfflineMRNAlreadyUsedException( string message, 
            Exception innerException,
            Severity severity )
            : base( message, innerException, severity )
        {
        }
        protected OfflineMRNAlreadyUsedException( SerializationInfo info, 
            StreamingContext context ) 
            : base( info, context )
        {
        }
    }
}
