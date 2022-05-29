using System.Collections.Generic;
using PatientAccess.Domain.Locking;

namespace PatientAccess.BrokerInterfaces
{
    ///<summary>
    /// 
    ///</summary>
    public interface IOfflineLockBroker
    {
        /// <summary>
        /// Adds the lock entry.
        /// </summary>
        /// <param name="lockHandle">The lock handle.</param>
        /// <param name="lockOwner">The lock owner.</param>
        /// <param name="resourceType">The type of lock</param>
        /// <returns></returns>
        bool AddLockEntry(string lockHandle, string lockOwner, ResourceType resourceType);


        /// <summary>
        /// Removes the lock entry.
        /// </summary>
        /// <param name="lockHandle">The lock handle.</param>
        /// <param name="lockOwner">The lock owner</param>
        void RemoveLockEntry( string lockHandle, string lockOwner );

        /// <summary>
        /// Checks if the lock entry exists.
        /// </summary>
        /// <param name="lockHandle">The lock handle.</param>
        /// <param name="lockOwner">The lock owner</param>
        /// <returns></returns>
        bool DoesLockEntryExist(string lockHandle, string lockOwner);

        /// <summary>
        /// Refreshes the lock.
        /// </summary>
        /// <param name="lockHandle">The lock handle.</param>
        /// <param name="lockOwner">The lock owner</param>
        /// <returns></returns>
        void RefreshLock( string lockHandle, string lockOwner );

        /// <summary>
        /// Gets locks with the resource type specified
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <returns></returns>
        IEnumerable<OfflineLock> GetLocksOfType( ResourceType resourceType );
    }
}
