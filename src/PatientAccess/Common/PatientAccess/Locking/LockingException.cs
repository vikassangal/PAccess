using System;
using System.Runtime.Serialization;

namespace PatientAccess.Locking
{
    [Serializable]
    public class LockingException : Exception
    {
        public LockingException() { }
        public LockingException(string message) : base(message) { }
        public LockingException(string message, Exception innerException) : base(message, innerException) { }
        protected LockingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}