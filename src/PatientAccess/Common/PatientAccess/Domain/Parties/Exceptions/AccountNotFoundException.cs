using System;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace PatientAccess.Domain.Parties.Exceptions
{
    /// <summary>
    /// Summary description for AccountNotFoundException.
    /// </summary>
    [Serializable]
    public class AccountNotFoundException : EnterpriseException
    {
        public AccountNotFoundException()
        {
        }
        public AccountNotFoundException(string msg)
            : base(msg)
        {
        }
        public AccountNotFoundException (string msg,Exception ex)
            : base(msg,ex)
        {
        }
        public AccountNotFoundException( string message, 
            Exception innerException,
            Severity severity )
            : base( message, innerException, severity )
        {
        }
        public AccountNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        {
        }
    }
}
