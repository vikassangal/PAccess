using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    public interface ICopyAccountPostMSECommand
    {
        Account Execute( IAccount fromAccount );
    }
}
