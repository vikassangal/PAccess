using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IAuthorizationStatusBroker.
    /// </summary>
    public interface IAuthorizationStatusBroker
    {
        ICollection AllAuthorizationStatuses();

        AuthorizationStatus AuthorizationStatusWith(string code);   
    }
}
