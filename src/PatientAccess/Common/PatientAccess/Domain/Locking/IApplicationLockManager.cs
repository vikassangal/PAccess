namespace PatientAccess.Domain.Locking
{
    public interface IApplicationLockManager
    {
        bool AcquireLock(string lockHandle, string lockOwner);
        void ReleaseLock(string lockHandle, string lockOwner);
        bool LockExists(string lockHandle, string lockOwner);
    }
}
