using PatientAccess.Domain;

namespace PatientAccess.Persistence.AccountCopy
{
    public interface IAccountCopyStrategy
    {
        Account CopyAccount(IAccount fromAccount);
    }
}
