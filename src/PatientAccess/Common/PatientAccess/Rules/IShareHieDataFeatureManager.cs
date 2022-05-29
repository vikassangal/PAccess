using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    public interface IShareHIEDataFeatureManager
    {
        bool IsShareHieDataEnabledforaccount(Account account);
        YesNoFlag DefaultShareHieDataForFacility(Facility facility);
    }
}
