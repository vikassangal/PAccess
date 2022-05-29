using System.Collections;
using System.Collections.Generic;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for ISSNBroker.
    /// </summary>
    public interface ISSNBroker
    {
        ArrayList SSNStatuses(long facilityNumber, string stateCode);
        IList<SocialSecurityNumberStatus> SSNStatuses();
        ArrayList SSNStatusesForGuarantor(long facilityNumber, string stateCode);
        SocialSecurityNumberStatus SSNStatusWith( long facilityNumber, string description );
    }
}
