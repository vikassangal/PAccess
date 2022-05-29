using System;
using System.Collections.Generic;
using PatientAccess.Domain.Locking;
using PatientAccess.Persistence.Nhibernate;
using PatientAccess.Utilities;

namespace PatientAccess.AppServer.Persistence.Locking
{
    public class OffLineLockRepository
    {
        private const int MaxResults = 100;

        public void RemoveLock( string lockHandle, string lockOwner )
        {
            Guard.ThrowIfArgumentIsNullOrEmpty( lockHandle, "lockHandle" );
            Guard.ThrowIfArgumentIsNullOrEmpty( lockOwner, "lockOwner" );

            using ( var session = NHibernateInitializer.OpenSession() )
            using ( var tx = session.BeginTransaction() )
            {
                var lockToDelete = session.QueryOver<OfflineLock>()
                    .Where( x => x.Handle == lockHandle && x.Owner == lockOwner )
                    .SingleOrDefault();

                session.Delete( lockToDelete );
                tx.Commit();
            }
        }

        public bool Save( OfflineLock newLock )
        {
            Guard.ThrowIfArgumentIsNull( newLock, "newLock" );
            Guard.ThrowIfArgumentIsNullOrEmpty( newLock.Handle, "newLock.Handle" );
            Guard.ThrowIfArgumentIsNullOrEmpty( newLock.Owner, "newLock.Owner" );
            using ( var session = NHibernateInitializer.OpenSession() )
            using ( var tx = session.BeginTransaction() )
            {
                var newSavedLock = session.Save( newLock );
                tx.Commit();
                var newLockSaved = newSavedLock != null;

                return newLockSaved;
            }
        }

        public bool IsLockOwnedBySomeoneElse( string lockHandle, string lockOwner )
        {
            Guard.ThrowIfArgumentIsNullOrEmpty( lockHandle, "lockHandle" );
            Guard.ThrowIfArgumentIsNullOrEmpty( lockOwner, "lockOwner" );

            OfflineLock offlineLock;
            using ( var session = NHibernateInitializer.OpenSession() )
            using ( var tx = session.BeginTransaction() )
            {
                offlineLock = session.QueryOver<OfflineLock>()
                    .Where( x => x.Handle == lockHandle && x.Owner != lockOwner )
                    .SingleOrDefault();

                tx.Commit();
            }

            return offlineLock != null;
        }

        public bool LockExists( string lockHandle, string lockOwner )
        {

            Guard.ThrowIfArgumentIsNullOrEmpty( lockHandle, "lockHandle" );
            Guard.ThrowIfArgumentIsNullOrEmpty( lockOwner, "lockOwner" );

            OfflineLock existingLock;
            using ( var session = NHibernateInitializer.OpenSession() )
            using ( var tx = session.BeginTransaction() )
            {
                existingLock = session.QueryOver<OfflineLock>()
                    .Where( x => x.Handle == lockHandle && x.Owner == lockOwner )
                    .SingleOrDefault();

                tx.Commit();
            }

            return existingLock != null;
        }

        public void UpdateLock( string lockHandle, string lockOwner, DateTime newTime )
        {
            Guard.ThrowIfArgumentIsNullOrEmpty( lockHandle, "lockHandle" );
            Guard.ThrowIfArgumentIsNullOrEmpty( lockOwner, "lockOwner" );

            using ( var session = NHibernateInitializer.OpenSession() )
            using ( var tx = session.BeginTransaction() )
            {
                var lockToUpdate = session.QueryOver<OfflineLock>()
                    .Where(x => x.Handle == lockHandle && x.Owner == lockOwner)
                    .SingleOrDefault();
                    

                lockToUpdate.TimePrint = newTime;

                session.Update( lockToUpdate );
                tx.Commit();
            }
        }

        public IEnumerable<OfflineLock> GetLocksOfType( ResourceType resourceType )
        {
            IEnumerable<OfflineLock> offlineLock = new List<OfflineLock>();
            using ( var session = NHibernateInitializer.OpenSession() )
            using ( var tx = session.BeginTransaction() )
            {
                var results = session.QueryOver<OfflineLock>()
                    .Where( x => x.ResourceType == resourceType )
                    .Take( MaxResults )
                    .List<OfflineLock>();

                tx.Commit();

                if ( results != null )
                {
                    offlineLock = results;
                }
            }

            return offlineLock;
        }
    }
}
