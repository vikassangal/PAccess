
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    public interface IAdditionalRacesFeatureManager
    {
        bool IsAdditionalRacesFeatureValidForAccount(Account account);
    }
}
