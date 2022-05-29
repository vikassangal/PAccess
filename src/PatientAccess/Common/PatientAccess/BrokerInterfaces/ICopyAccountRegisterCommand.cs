using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    public interface ICopyAccountRegisterCommand
    {
        Account Execute( IAccount fromAccount );
    }
}
