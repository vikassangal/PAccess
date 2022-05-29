using System;
using System.Collections.Generic;
using PatientAccess.AppServer.Persistence.Locking;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain.Locking;
using PatientAccess.Locking;
using PatientAccess.Utilities;
using log4net;

namespace PatientAccess.Persistence.Locking
{

    [Serializable]
    public class SqlOfflineLockBroker : MarshalByRefObject, IOfflineLockBroker
    {
        private static readonly ILog Logger = LogManager.GetLogger( typeof( SqlOfflineLockBroker ) );
        private readonly OffLineLockRepository offLineLockRepository = new OffLineLockRepository();

        #region Methods

        /// <summary>
        /// Adds a new lock entry to locks table if the lock does not exist.
        /// The method returns true if the lock is already owned by  successfully  and the addition is successful.
        /// The  or if the entry already exists the method returns true.
        /// The method returns false if the lock is owned by someone else
        /// </summary>
        /// <param name="lockHandle">The lock handle.</param>
        /// <param name="lockOwner">The lock owner.</param>
        /// <param name="resourceType">Type of lock</param>
        /// <returns><c>true</c> if the addition is successful or if the lock is already owned by the <c>lockOwner</c> <c>false</c> if the lock is owned by someone else.</returns>
        /// <exception cref="ArgumentNullException"><c>lockHandle</c> is null or empty.</exception>
        /// <exception cref="ArgumentNullException"><c>lockOwner</c> is null or empty.</exception>
        /// <exception cref="LockingException"><c>LockingException</c>.</exception>
        public bool AddLockEntry( string lockHandle, string lockOwner, ResourceType resourceType )
        {
            Guard.ThrowIfArgumentIsNullOrEmpty( lockHandle, "lockHandle" );
            Guard.ThrowIfArgumentIsNullOrEmpty( lockOwner, "lockOwner" );
            var newTime = DateTime.Now;
            var newLock = new OfflineLock { Handle = lockHandle, Owner = lockOwner, TimePrint = newTime, ResourceType = resourceType };

            try
            {
                var alreadyExists = offLineLockRepository.LockExists( lockHandle, lockOwner );

                if ( alreadyExists )
                {
                    return true;
                }

                var ownedBySomeoneElse = offLineLockRepository.IsLockOwnedBySomeoneElse( lockHandle, lockOwner );

                if ( ownedBySomeoneElse )
                {
                    return false;
                }

                Logger.Info( string.Format( "Adding lock entry with handle = [{0}], owner = [{1}] and timeprint=[{2}]", newLock.Handle, newLock.Owner, newLock.TimePrint ) );

                return offLineLockRepository.Save( newLock );
            }

            catch ( Exception e )
            {
                throw new LockingException( string.Format( "Could not add lock for lock handle = [{0}], owner = [{1}] and timeprint=[{2}]", lockHandle, lockOwner, newTime ), e );
            }
        }

        /// <summary>
        /// Refreshes the lock by updating the lock's time stamp in the database.
        /// </summary>
        /// <param name="lockHandle">The lock handle.</param>
        /// <param name="lockOwner">The lock owner</param>
        /// <exception cref="LockingException"><c>LockingException</c>if the lock does not exist for the <c>lockOwner</c>.</exception>
        public void RefreshLock( string lockHandle, string lockOwner )
        {
            Guard.ThrowIfArgumentIsNullOrEmpty( lockHandle, "lockHandle" );
            var lockExists = DoesLockEntryExist( lockHandle, lockOwner );

            if ( !lockExists )
            {
                throw new LockingException( string.Format( "lock does not exist for lock handle = [{0}] and lock onwer [{1}]", lockHandle, lockOwner ) );
            }

            var newTime = DateTime.Now;

            try
            {
                Logger.Info( string.Format( "Refreshing lock entry with handle = [{0}], owner = [{1}] and timeprint=[{2}]", lockHandle, lockOwner, newTime ) );
                offLineLockRepository.UpdateLock( lockHandle, lockOwner, newTime );
            }

            catch ( Exception e )
            {
                throw new LockingException( string.Format( "Could not refresh lock for lock handle = [{0}], owner = [{1}] with new time=[{2}]", lockHandle, lockOwner, newTime ), e );
            }
        }

        /// <exception cref="LockingException"><c>LockingException</c>.</exception>
        public IEnumerable<OfflineLock> GetLocksOfType( ResourceType resourceType )
        {
            Logger.Info( string.Format( "Getting locks of type [{0}]", Enum.GetName( typeof( ResourceType ), resourceType ) ) );
            try
            {
                var locks = offLineLockRepository.GetLocksOfType( resourceType );
                return locks;
            }

            catch ( Exception e )
            {
                throw new LockingException( string.Format( "Could not get locks of type [{0}]", Enum.GetName( typeof( ResourceType ), resourceType ) ), e );
            }
        }

        /// <summary>
        /// Removes the lock entry from the lock table.
        /// </summary>
        /// <param name="lockHandle">The lock handle.</param>
        /// <param name="lockOwner">The lock owner</param>
        /// <returns></returns>
        /// <exception cref="LockingException"><c>LockingException</c>.</exception>
        public void RemoveLockEntry( string lockHandle, string lockOwner )
        {
            Guard.ThrowIfArgumentIsNullOrEmpty( lockHandle, "lockHandle" );

            try
            {

                if ( DoesLockEntryExist( lockHandle, lockOwner ) )
                {
                    Logger.Info( string.Format( "Removing lock entry with handle = [{0}]", lockHandle ) );
                    offLineLockRepository.RemoveLock( lockHandle, lockOwner );
                }
            }

            catch ( Exception e )
            {
                throw new LockingException( string.Format( "Could not remove lock for lock handle = [{0}] and lock onwer [{1}]", lockHandle, lockOwner ), e );
            }
        }

        /// <summary>
        /// Checks if the lock entry exists.
        /// </summary>
        /// <param name="lockHandle">The lock handle.</param>
        /// <param name="lockOwner"></param>
        /// <returns></returns>
        /// <exception cref="LockingException"><c>LockingException</c>.</exception>
        public bool DoesLockEntryExist( string lockHandle, string lockOwner )
        {
            Guard.ThrowIfArgumentIsNullOrEmpty( lockHandle, "lockHandle" );

            try
            {
                return offLineLockRepository.LockExists( lockHandle, lockOwner );
            }

            catch ( Exception e )
            {
                throw new LockingException( string.Format( "Could not check lock with handle = [{0}] and owner = [{1}]", lockHandle, lockOwner ), e );
            }
        }

        #endregion Methods
    }
}
