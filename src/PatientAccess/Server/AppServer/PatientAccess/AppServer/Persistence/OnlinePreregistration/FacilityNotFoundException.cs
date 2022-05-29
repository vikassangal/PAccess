using System;
using System.Runtime.Serialization;

namespace PatientAccess.Persistence.OnlinePreregistration
{
    [Serializable]
    public class FacilityNotFoundException : Exception, ISerializable
    {
        public FacilityNotFoundException()
        {
        }
        public FacilityNotFoundException( string message ): base(message)
        {
        }
        public FacilityNotFoundException( string message, Exception inner ): base(message,inner)
        {
        }

        // This constructor is needed for serialization.
        protected FacilityNotFoundException( SerializationInfo info, StreamingContext context ): base(info, context)
        {
        }
    }
}