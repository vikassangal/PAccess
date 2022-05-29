using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    public interface ICopyAccountPreregisterCommand
    {
        Account Execute( IAccount fromAccount );
    }
}
