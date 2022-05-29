using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    public interface INotifyPCPFeatureManager
    {
        bool IsNotifyPCPEnabledforaccount(Account account);
        YesNoFlag DefaultNotifyPCPForFacility(Facility facility);
    }
}
