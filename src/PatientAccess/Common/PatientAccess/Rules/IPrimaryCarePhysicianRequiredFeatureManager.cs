using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    public interface IPrimaryCarePhysicianRequiredFeatureManager
    {
        bool IsPrimarycarephysicianRequiredfor(Account account);
    }

}
