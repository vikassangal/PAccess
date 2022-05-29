using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IEMPIBroker.
    /// </summary>
    public interface IEMPIBroker
    {
        PatientSearchResponse SearchEMPI(PatientSearchCriteria searchCriteria, Facility facility); 
        EMPIPatient GetEMPIPatientFor(Patient pbarPatient, Facility currentFacility);
        string GetPBARFacilityCode(string EMPIFacilityCode);
    }
}
