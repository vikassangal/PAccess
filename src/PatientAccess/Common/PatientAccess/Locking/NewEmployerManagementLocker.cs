using System;
using System.Threading;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain.Locking;
using PatientAccess.Utilities;

namespace PatientAccess.Locking
{
    public class NewEmployerManagementLocker : IDisposable
    {
        private IOfflineLockBroker _lockBroker;

        private string _lockOwner;

        private string _lockHandle;

        private Timer _refreshTimer;

        private TimeSpan _refreshTimerTimePeriod;

        private IOfflineLockBroker LockBroker
        {
            get
            {
                return this._lockBroker;
            }
            set
            {
                this._lockBroker = value;
            }
        }

        private string LockHandle
        {
            get
            {
                return this._lockHandle;
            }
            set
            {
                this._lockHandle = value;
            }
        }

        private string LockOwner
        {
            get
            {
                return this._lockOwner;
            }
            set
            {
                this._lockOwner = value;
            }
        }

        private Timer RefreshTimer
        {
            get
            {
                return this._refreshTimer;
            }
            set
            {
                this._refreshTimer = value;
            }
        }

        private TimeSpan RefreshTimerTimePeriod
        {
            get
            {
                return this._refreshTimerTimePeriod;
            }
            set
            {
                this._refreshTimerTimePeriod = value;
            }
        }

        private readonly TimeSpan DueTimeToStopTimer;

        public NewEmployerManagementLocker(IOfflineLockBroker lockBroker, string lockHandle, string lockOwner) :
            this(lockBroker, lockHandle, lockOwner, TimeSpan.FromSeconds(60)) { }


        public NewEmployerManagementLocker(IOfflineLockBroker lockBroker, string lockHandle, string lockOwner, TimeSpan refreshTimePeriod)
        {
            Guard.ThrowIfArgumentIsNullOrEmpty(lockHandle, "lockHandle");
            Guard.ThrowIfArgumentIsNullOrEmpty(lockOwner, "lockOwner");
            Guard.ThrowIfTimeSpanIsNotPositive(refreshTimePeriod, "refreshTimePeriod");

            this.LockBroker = lockBroker;
            this.LockHandle = lockHandle;
            this.LockOwner = lockOwner;
            this.RefreshTimerTimePeriod = refreshTimePeriod;
            this.DueTimeToStopTimer = TimeSpan.FromMilliseconds(-1);
            this.RefreshTimer = new Timer(this.DoRefresh, new object(), this.DueTimeToStopTimer, this.DueTimeToStopTimer);
        }

        public bool AcquireLock()
        {
            bool lockIsAcquired = this.LockBroker.AddLockEntry(this.LockHandle, this.LockOwner, ResourceType.FacilityForNewEmployerManagementFeature);
            if (lockIsAcquired)
            {
                this.StartLockRefreshTimer();
            }

            return lockIsAcquired;
        }

        /// <summary>
        /// Releases the lock.
        /// </summary>
        
        public void ReleaseLock()
        {
            if (this.IsLocked())
            {
                this.LockBroker.RemoveLockEntry(this.LockHandle, this.LockOwner);
                this.StopRefreshTimer();
            }
        }

        /// <summary>
        /// Checks if the lock exists.
        /// </summary>
        /// <returns><c>true</c> if the lock exists <c>false</c> otherwise.</returns>
        public bool IsLocked()
        {
            return this.LockBroker.DoesLockEntryExist(this.LockHandle, this.LockOwner);
        }


        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }


        private void StartLockRefreshTimer()
        {

            this.RefreshTimer.Change(TimeSpan.Zero, this.RefreshTimerTimePeriod);
        }

        private void DoRefresh(object state)
        {
            this.LockBroker.RefreshLock(this.LockHandle, this.LockOwner);
        }

        private void StopRefreshTimer()
        {
            this.RefreshTimer.Change(this.DueTimeToStopTimer, this.RefreshTimerTimePeriod);
        }


        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.RefreshTimer.Dispose();
            }
        }
    }
}