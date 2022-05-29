using System;
using System.Runtime.Remoting.Messaging;
using PatientAccess.Utilities;

namespace PatientAccess.RemotingServices
{
    /// <summary>
    /// Contains the call context data that may be need on the server side.
    /// Add more fields to this type if more data is needed.
    /// </summary>
    [Serializable]
    public class CallContextData : ILogicalThreadAffinative
    {
        /// <exception cref="ArgumentNullException"><c>userId</c> is null or empty.</exception>
        public CallContextData( string userId )
        {
            Guard.ThrowIfArgumentIsNullOrEmpty( userId, "userId" );

            UserId = userId;
        }

        public string UserId { get; private set; }
    }
}