using System;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    public interface IEMPIFeatureManager
    {
        bool IsEMPIFeatureEnabled(Activity activity);
        bool IsEMPIFeatureEnabledForSearch(Activity activity);
        bool IsEMPIFeatureEnabledForFacility();
    }

    [Serializable]
    public class EMPIFeatureManager : IEMPIFeatureManager
    {
        private readonly Facility CurrentUserFacility;

        public EMPIFeatureManager()
        {
            CurrentUserFacility = User.GetCurrent().Facility;
        }

        public EMPIFeatureManager(Facility facility)
        {
            CurrentUserFacility = facility;
        }

        public bool IsEMPIFeatureEnabled(Activity activity)
        {
            return ((CurrentUserFacility != null) && CurrentUserFacility.IsEMPIEnabled && (activity != null) && activity.IsValidForUpdateFromEMPI);
        }
        public bool IsEMPIFeatureEnabledForSearch(Activity activity)
        {
            return ((CurrentUserFacility != null) && CurrentUserFacility.IsEMPIEnabled && (activity != null) && activity.IsValidForEMPISearch);
        }
        public bool IsEMPIFeatureEnabledForFacility()
        {
            return ((CurrentUserFacility != null) && CurrentUserFacility.IsEMPIEnabled);
        }
    }
}
