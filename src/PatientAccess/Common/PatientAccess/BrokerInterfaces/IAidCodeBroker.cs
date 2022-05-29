using System.Collections;

namespace PatientAccess.BrokerInterfaces
{
    public interface IAidCodeBroker
    {
        ArrayList GetAidCode(int FacilityId);
        ArrayList GetCalOptimaPlanIds(int FacilityId);
    }
}
