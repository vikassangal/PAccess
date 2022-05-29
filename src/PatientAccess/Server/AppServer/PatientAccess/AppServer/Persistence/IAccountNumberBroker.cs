using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
    internal interface IAccountNumberBroker
    {
        long GetNextValidAccountNumberFor( Facility facility );
    }
}
