using System;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{

    [Serializable]
    public class PatientNameFeatureManager
    {
        public bool IsAutoPopulatePatientName_Enabled(Account account)
        {
            return account != null && IsAutoPopulatePatientName_EnabledForActivity(account.Activity) &&
                   IsAutoPopulatePatientName_EnabledForFacility(account.Facility);
        }

        private bool IsAutoPopulatePatientName_EnabledForFacility(Facility facility)
        {
            return facility != null && !facility.IsBaylorFacility();
        }

        private bool IsAutoPopulatePatientName_EnabledForActivity(Activity activity)
        {
            return activity != null && activity.IsValidForAutopopulatePatientName();
        }
    }
}
