using System;

namespace PatientAccess.Domain.Locking
{
    [Serializable]
    public class OfflineLock
    {
        public virtual string Handle { get; set; }
        public virtual string Owner { get; set; }
        public virtual DateTime TimePrint { get; set; }
        public virtual ResourceType ResourceType { get; set; }
    }

    /// <summary>
    /// This enum represents the type of lock to be created. 
    /// To add a new entry to this enum modify the locks table constraint to allow the new value. 
    /// </summary>
    public enum ResourceType
    {
        FacilityForNewEmployerManagementFeature,
        OnlinePreregistrationSubmission
    }
}
