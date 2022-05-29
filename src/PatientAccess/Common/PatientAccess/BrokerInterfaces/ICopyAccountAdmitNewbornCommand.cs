using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    public interface ICopyAccountAdmitNewbornCommand
    {
        Account Execute( IAccount fromAccount );
    }
}
