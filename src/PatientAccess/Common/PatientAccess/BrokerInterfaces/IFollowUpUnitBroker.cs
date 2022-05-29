using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IFollowUpUnitBroker.
    /// </summary>
    public interface IFollowUpUnitBroker
    {
        IList AllFollowUpUnits();
        FollowupUnit FollowUpUnitWith( long oid );
        bool FollowupUnitIsBeingLoaded(string fuuCode);
    }
}
