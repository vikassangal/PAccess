
using System;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    public interface ISequesteredPatientFeatureManager
    {
        bool ShouldFeatureBeEnabled(Account account);

    }
    [Serializable]
    public class SequesteredPatientFeatureManager : ISequesteredPatientFeatureManager
    {
        public bool ShouldFeatureBeEnabled(Account account)
        {
            if (account == null || account.Facility == null)
                return false;
            return account.Facility.IsValidForSequesteredPatient();
        }
    }
}
