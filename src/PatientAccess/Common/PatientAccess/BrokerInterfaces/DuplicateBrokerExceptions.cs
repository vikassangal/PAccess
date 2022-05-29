using System;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace PatientAccess.BrokerInterfaces
{
    [Serializable]
    public class DuplicateRecordInsertException : BrokerException
    {
        // default constructor needed for serialization
        public DuplicateRecordInsertException(  )
        {
        }
        public DuplicateRecordInsertException( SerializationInfo info, StreamingContext context)
            : base( info, context)
        {
        }
        public DuplicateRecordInsertException( string msg, Exception ex )
            : base( msg, ex )
        {
        }
        public DuplicateRecordInsertException( string msg, Exception innerException, Severity severity )
            : base( msg, innerException, severity )
        {
        }
    }
}

