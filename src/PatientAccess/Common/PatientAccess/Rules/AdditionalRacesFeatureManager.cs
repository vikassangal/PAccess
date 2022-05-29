using System;
using PatientAccess.Domain;


namespace PatientAccess.Rules
{
    [Serializable]
    public class AdditionalRacesFeatureManager : IAdditionalRacesFeatureManager
    {

        public bool IsFacilityValidForAdditionalRacesFeature(Facility facility)
        {
            return (facility != null &&
                    facility.FacilityState != null &&
                    facility.FacilityState.IsCalifornia);

        }

        public bool IsAdditionalRacesFeatureValidForActivity(Account account)
        {
            if (account != null &&
                account.Activity != null && account.KindOfVisit != null)
            {
                return (account.Activity.IsValidForAdditionalRaceCodes() &&
                        !account.KindOfVisit.IsPreRegistrationPatient);
            }

            return false;
        }

        public bool IsAdditionalRacesFeatureValidForAccount(Account account)
        {
            return (account != null && IsFacilityValidForAdditionalRacesFeature(account.Facility) &&
                    IsAdditionalRacesFeatureValidForActivity(account));
        }

    }
}
