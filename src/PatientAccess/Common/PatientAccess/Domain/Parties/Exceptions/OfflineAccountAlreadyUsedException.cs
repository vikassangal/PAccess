using System;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace PatientAccess.Domain.Parties.Exceptions
{
	/// <summary>
	/// Summary description for OfflineAccountAlreadyUsedException.
	/// </summary>
	[Serializable]
	public class OfflineAccountAlreadyUsedException : EnterpriseException
	{
        public OfflineAccountAlreadyUsedException() 
            : base()
		{
		}
        public OfflineAccountAlreadyUsedException(string msg)
            : base(msg)
        {
        }
        public OfflineAccountAlreadyUsedException (string msg,Exception ex)
            : base(msg,ex)
        {
        }
        public OfflineAccountAlreadyUsedException( string message, 
            Exception innerException,
            Severity severity )
            : base( message, innerException, severity )
        {
        }
        protected OfflineAccountAlreadyUsedException( SerializationInfo info, 
            StreamingContext context ) 
            : base( info, context )
        {
        }

	}
}
