using System;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for PatientBrokerException.
    /// </summary>
    [Serializable]
    public class BrokerException : EnterpriseException
    {
        public BrokerException()
        {
        }
        public BrokerException(string msg)
            : base(msg)
        {
        }
        public BrokerException (string msg,Exception ex)
            : base(msg,ex)
        {
        }
        public BrokerException( string message, 
            Exception innerException,
            Severity severity )
            : base( message, innerException, severity )
        {
        }
        public BrokerException(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        {
        }

    }
}
