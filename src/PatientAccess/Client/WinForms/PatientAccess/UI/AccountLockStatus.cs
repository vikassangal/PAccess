using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.UI
{
    /// <summary>
    /// Retrieves the current Account Lock Status.
    /// </summary>
    class AccountLockStatus
    {
        #region Event Handlers
        #endregion

        #region Methods

        private static AccountLock AccountLockStatusFor( IAccount anAccount, User user )
        {
            IAccountBroker broker = BrokerFactory.BrokerOfType<IAccountBroker>();
            AccountLock al = broker.AccountLockStatusFor( anAccount.AccountNumber, user.Facility.Code, user.PBAREmployeeID, user.WorkstationID );
            return al;
        }
        public static bool IsAccountLocked( IAccount anAccount, User user )
        {
            AccountLock al = AccountLockStatusFor( anAccount, user );
            return al.IsAccountLocked();
        }
        public static bool IsLockAcquiredByCurrentUser( IAccount anAccount, User user )
        {
            AccountLock al = AccountLockStatusFor( anAccount, user );
            return al.IsLockAcquiredByCurrentUser();
        }

        #endregion

        #region Data Elements
        #endregion
    }
}
