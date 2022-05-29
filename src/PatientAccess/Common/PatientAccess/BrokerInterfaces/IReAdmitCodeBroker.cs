using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Interface lists the ReAdmitCodeBroker Broker related methods.
    /// </summary>
    public interface IReAdmitCodeBroker
    {
        // Get ReAdmit Codes for a facility
        ICollection ReAdmitCodesFor( long facilityNumber );
        ReAdmitCode ReAdmitCodeWith(long facilityNumber, string code);

    }
}
