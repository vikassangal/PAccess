using System;
using System.Runtime.Serialization;
using Extensions.Exceptions;

namespace PatientAccess.Domain.Parties.Exceptions
{
    /// <summary>
    /// Summary description for VersionNotMatchedException.
    /// </summary>
    [Serializable]
    public class VersionNotMatchedException : EnterpriseException
    {
        public VersionNotMatchedException( Severity severity )
            : base( VERSION_NOT_MATCHED, severity )
        {
        }
        public VersionNotMatchedException() : base( VERSION_NOT_MATCHED )
        {
        }
        public VersionNotMatchedException( string msg )
            : base( msg )
        {
        }
        public VersionNotMatchedException( string msg, Exception ex )
            : base( msg, ex )
        {
        }
        public VersionNotMatchedException( string message,
            Exception innerException,
            Severity severity )
            : base( message, innerException, severity )
        {
        }
        public VersionNotMatchedException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }

        private const string VERSION_NOT_MATCHED = "Client version does not match server version.";
    }
}
