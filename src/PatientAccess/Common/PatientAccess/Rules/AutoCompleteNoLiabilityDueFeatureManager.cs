using System;
using System.Configuration;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    public class AutoCompleteNoLiabilityDueFeatureManager : IAutoCompleteNoLiabilityDueFeatureManager
    {
        private DateTime AutoCompleteNoLiablityDueFeatureStartDate = DateTime.MinValue;

        public bool IsAccountCreatedAfterImplementationDate(Account account)
        {
            if (account != null && account.Facility != null )
            {
                if (IsAutoCompleteNoLiabilityDueEnabledForDate(account) && account.Facility.IsAutoCompleteNoLiabilityDueEnabled)
                {
                    return true;
                }
             }

            return false;
        }
        public bool IsAccountCreatedBeforeImplementationDate(Account account)
        {
            if ( account != null && account.Facility != null )
            {
                if ( !IsAutoCompleteNoLiabilityDueEnabledForDate(account) && account.Facility.IsAutoCompleteNoLiabilityDueEnabled )
                {
                    return true;
                }
               
            }

            return false;
        }

        public bool IsFeatureEnabledForToday
        {
            get { return FeatureStartDate <= DateTime.Today; }
        }
        public DateTime FeatureStartDate
        {
            get
            {
                string startDate = ConfigurationManager.AppSettings[AutoCompleteNoLiablityDue_FeatureStartDate];
                if (AutoCompleteNoLiablityDueFeatureStartDate.Equals(DateTime.MinValue) && startDate != null)
                {
                    AutoCompleteNoLiablityDueFeatureStartDate = DateTime.Parse(startDate);
                }
                return AutoCompleteNoLiablityDueFeatureStartDate;
            }
        }

        private bool IsAutoCompleteNoLiabilityDueEnabledForDate(Account account)
        {
            if (account != null &&
                (account.AccountCreatedDate == DateTime.MinValue && DateTime.Today >= FeatureStartDate ||
                 account.AccountCreatedDate >= FeatureStartDate))
            {
                return true;
            }
            return false;
        }

        private const string AutoCompleteNoLiablityDue_FeatureStartDate = "AUTO_COMPLETE_NO_LIABILITY_DUE_START_DATE";
    }
}
