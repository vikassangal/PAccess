using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for AccountLock.
    /// </summary>
    [Serializable]
    public class AccountLock
    {
        #region Properties
        public string LockedBy
        {
            get
            {
                return i_LockedBy;
            }
            set
            {
                i_LockedBy = value;
            }
        }

        public DateTime LockedOn
        {
            get
            {
                return i_LockedOn;
            }
            set
            {
                i_LockedOn = value;
            }
        }
        /// <summary>
        /// Get/SET the locked at WorkstationId
        /// </summary>
        public string LockedAt
        {
            get
            {
                return i_LockedAt;
            }
            set
            {
                i_LockedAt = value;
            }
        }
        public bool AcquiredLock
        {
            get
            {
                return i_AcquiredLock;
            }
            set
            {
                i_AcquiredLock = value;
            }
        }

        public bool IsLocked
        {
            get
            {
                return i_IsLocked;
            }
            set
            {
                i_IsLocked = value;
            }
        }

        #endregion

        #region Methods
        public bool IsAccountLocked()
        {
            return this.IsLocked;
        }

        public bool IsLockAcquiredByCurrentUser()
        {
            if( this.IsLocked && this.AcquiredLock )
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        #endregion

        #region Construction and Finalization
        public AccountLock()
        {
        }
        public AccountLock( string lockedBy, DateTime lockedOn, string lockedAt, 
            bool acquiredLock, bool isLocked )
        {
            this.LockedBy = lockedBy;
            this.LockedOn = lockedOn;
            this.LockedAt = lockedAt;
            this.AcquiredLock = acquiredLock;
            this.i_IsLocked = isLocked;
        }
        #endregion

        #region Data Elements
        private string i_LockedBy = string.Empty;
        private DateTime i_LockedOn;
        private string i_LockedAt = string.Empty;
        private bool i_AcquiredLock;
        private bool i_IsLocked;
        #endregion
    }
}
