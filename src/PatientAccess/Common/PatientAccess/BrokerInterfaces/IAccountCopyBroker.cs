using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    public interface IAccountCopyBroker
    {
        Account CreateAccountCopyFor(IAccount fromAccount);
    }
}
