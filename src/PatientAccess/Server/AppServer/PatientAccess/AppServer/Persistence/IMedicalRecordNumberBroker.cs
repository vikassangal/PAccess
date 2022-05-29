using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
    internal interface IMedicalRecordNumberBroker
    {
        int GetNextMRNFor(Facility facility);
    }
}