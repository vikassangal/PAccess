using System;
using System.Configuration;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    public class PullPriorVisitInsuranceToPreMseFeatureManager
    {
        private DateTime PullPriorVisitInsuranceToPreMseFeatureStartDate = DateTime.MinValue;

        public DateTime FeatureStartDate
        {
            get
            {
                string startDate = ConfigurationManager.AppSettings[PULL_PRIOR_VISIT_INSURANCE_PREMSE_START_DATE];
                if (PullPriorVisitInsuranceToPreMseFeatureStartDate.Equals(DateTime.MinValue) && startDate != null)
                {
                    PullPriorVisitInsuranceToPreMseFeatureStartDate = DateTime.Parse(startDate);
                }

                return PullPriorVisitInsuranceToPreMseFeatureStartDate;
            }
        }

        public bool IsPullPriorVisitInsuranceToPreMseEnabledForDate(Account account)
        {
            if (account != null &&
                (account.AccountCreatedDate == DateTime.MinValue && DateTime.Today >= FeatureStartDate ||
                 account.AccountCreatedDate >= FeatureStartDate))
            {
                return true;
            }

            return false;
        }

        private const string PULL_PRIOR_VISIT_INSURANCE_PREMSE_START_DATE =
            "PULL_PRIOR_VISIT_INSURANCE_PREMSE_START_DATE";
    }
}
