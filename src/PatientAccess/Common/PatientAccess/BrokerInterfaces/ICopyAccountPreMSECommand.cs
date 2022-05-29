using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    public interface ICopyAccountPreMSECommand
    {
        Account Execute( IAccount fromAccount );
    }
}
