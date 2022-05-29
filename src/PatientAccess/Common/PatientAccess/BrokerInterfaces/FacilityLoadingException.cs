using System;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for FacilityLoadingException.
	/// </summary>
	[Serializable]
    public class FacilityLoadingException : EnterpriseException
    {
        public FacilityLoadingException()
        {
        }
        public FacilityLoadingException(string msg)
            : base(msg)
        {
        }
        public FacilityLoadingException (string msg,Exception ex)
            : base(msg,ex)
        {
        }
        public FacilityLoadingException( string message, 
            Exception innerException,
            Severity severity )
            : base( message, innerException, severity )
        {
        }
        public FacilityLoadingException(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        {
        }

    }
}
