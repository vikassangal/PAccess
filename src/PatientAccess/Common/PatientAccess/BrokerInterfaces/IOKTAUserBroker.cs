using PatientAccess.Domain.Security;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    public interface IOKTAUserBroker
    {
        SecurityResponse AuthenticateUser(string userName, string password, Facility selectedFacility);
    }
}
