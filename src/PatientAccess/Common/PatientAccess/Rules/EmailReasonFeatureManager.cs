using System;
using System.Configuration;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    public interface IEmailReasonFeatureManager
    {
        bool ShouldFeatureBeVisibleForAccountCreatedDate(Account account);
    }

    [Serializable]
    public class EmailReasonFeatureManager : IEmailReasonFeatureManager
    {
        private DateTime emailReasonStartDate = DateTime.MinValue;
        public bool ShouldFeatureBeVisibleForAccountCreatedDate(Account account)
        {
            if ((account.AccountCreatedDate == DateTime.MinValue && DateTime.Today >= FeatureStartDate) ||
                account.AccountCreatedDate >= FeatureStartDate)
            {
                return true;
            }

            return false;
        }

        public DateTime FeatureStartDate
        {
            get
            {
                string startDate = ConfigurationManager.AppSettings[EMAILREASON_START_DATE];
                if (emailReasonStartDate.Equals(DateTime.MinValue) && startDate != null)
                {
                    emailReasonStartDate = DateTime.Parse(startDate);
                }
                return emailReasonStartDate;
            }
        }

        private const string EMAILREASON_START_DATE = "EMAILREASON_START_DATE";
    }
}
