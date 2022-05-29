using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IWorklistBroker.
    /// </summary>
    public interface IWorklistBroker
    {        
        void DeleteAccountWorklistItemsWith(long facilityId,long accountNumber);
        void ProcessAllWorklistsFor(Facility facility);
        Hashtable RemainingActionsFor( long accountNumber, long medicalRecordNumber, long facilityOid );
        void SaveRemainingActions(Account anAccount);

    }
}
