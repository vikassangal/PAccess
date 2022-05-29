using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain.Locking;
using PatientAccess.Utilities;

namespace PatientAccess.Locking
{
    public class OnlinePreRegistrationSubmissionLocker 
    {
        private IOfflineLockBroker LockBroker { get; set; }

        private string LockHandle { get; set; }

        private string LockOwner { get; set; }

        public OnlinePreRegistrationSubmissionLocker( IOfflineLockBroker lockBroker, string lockHandle, string lockOwner )
        {
            Guard.ThrowIfArgumentIsNullOrEmpty(lockHandle, "lockHandle");
            Guard.ThrowIfArgumentIsNullOrEmpty(lockOwner, "lockOwner");

            LockBroker = lockBroker;
            LockHandle = lockHandle;
            LockOwner = lockOwner;
        }

        public bool AcquireLock()
        {
            var lockIsAcquired = LockBroker.AddLockEntry(LockHandle, LockOwner, ResourceType.OnlinePreregistrationSubmission);

            return lockIsAcquired;
        }

        public void RefreshLock()
        {
            if ( IsLocked() )
            {
                LockBroker.RefreshLock( LockHandle, LockOwner );
            }
        }

        /// <summary>
        /// Releases the lock.
        /// </summary>
        
        public void ReleaseLock()
        {
            if (IsLocked())
            {
                LockBroker.RemoveLockEntry(LockHandle, LockOwner);
            }
        }

        /// <summary>
        /// Checks if the lock exists.
        /// </summary>
        /// <returns><c>true</c> if the lock exists <c>false</c> otherwise.</returns>
        private bool IsLocked()
        {
            return LockBroker.DoesLockEntryExist(LockHandle, LockOwner);
        } 
    }
}
