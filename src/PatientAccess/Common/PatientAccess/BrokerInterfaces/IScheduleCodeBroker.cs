using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IScheduleCodeBroker
    /// </summary>
    public interface IScheduleCodeBroker
    {
        ICollection AllScheduleCodes(long facilityID);
        ScheduleCode ScheduleCodeWith(long facilityID, string code);
    }
}
