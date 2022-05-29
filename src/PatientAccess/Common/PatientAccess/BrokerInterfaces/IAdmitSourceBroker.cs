using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IAdmitSourceBroker.
    /// </summary>
    public interface IAdmitSourceBroker
    {
        ICollection AllTypesOfAdmitSources(long facilityID);

        ICollection AdmitSourcesForNotNewBorn(long facilityID);
        AdmitSource AdmitSourceForNewBorn(long facilityID);
        AdmitSource AdmitSourceWith(long facilityID, string code);

     }
}
