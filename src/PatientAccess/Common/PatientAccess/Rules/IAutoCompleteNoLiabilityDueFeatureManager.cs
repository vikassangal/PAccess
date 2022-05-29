using PatientAccess.Domain;
namespace PatientAccess.Rules
{
    public interface IAutoCompleteNoLiabilityDueFeatureManager
    {
        bool IsAccountCreatedAfterImplementationDate(Account account);

        bool IsAccountCreatedBeforeImplementationDate(Account account);

        bool IsFeatureEnabledForToday { get;}
    }
}
