using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IConditionCodeBroker.
    /// </summary>
    public interface IConditionCodeBroker
    {
        IList AllSelectableConditionCodes(long faciltiyID);
        ConditionCode ConditionCodeWith( long facilityID, long oid );
        ConditionCode ConditionCodeWith( long facilityID, string ConditionCode );
    }
}
