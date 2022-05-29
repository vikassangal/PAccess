using PatientAccess.Domain;

namespace PatientAccess.Persistence.AccountCopy
{
    /// <summary>
    /// This class will create a deep copy of the the old class and then
    /// remove or reset the values that should not actually copy. This is
    /// useful if the majority of the information will copy from the old
    /// account to the new.
    /// </summary>
    class BinaryCloneAccountCopyStrategy : AccountCopyStrategy
    {
        protected override Account CreateInstanceFrom(IAccount oldAccount)
        {
            
            var newAccount = oldAccount.AsAccount().DeepCopy() as Account;
            
            return newAccount;

        }
        protected override Account CreateInstanceFrom(Account oldAccount)
        {
            var newAccount = oldAccount.DeepCopy() as Account;
            return newAccount;

        }
    }
}
