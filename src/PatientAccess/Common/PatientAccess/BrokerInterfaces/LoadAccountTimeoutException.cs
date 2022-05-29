using System;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace PatientAccess.BrokerInterfaces
{
    [Serializable]
    public class LoadAccountTimeoutException : EnterpriseException
    {
        // default constructor needed for serialization
        public LoadAccountTimeoutException()
        {
        }
        public LoadAccountTimeoutException(string msg)
            : base(msg)
        {
        }
        public LoadAccountTimeoutException(string msg,Exception ex)
            : base(msg,ex)
        {
        }
        public LoadAccountTimeoutException( string message, 
            Exception innerException,
            Severity severity )
            : base( message, innerException, severity )
        {
        }
        public LoadAccountTimeoutException(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        {
        }
    }
}

