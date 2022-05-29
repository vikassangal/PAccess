using System;
using System.Configuration;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    public interface IEmailAddressFeatureManager
    {
        bool ShouldFeatureBeEnabled(Account account);
    }

    [Serializable]
    public class EmailAddressFeatureManager : IEmailAddressFeatureManager
    {
        private DateTime patientEmailAddressVisibleStartDate = DateTime.MinValue;

        public bool ShouldFeatureBeEnabled(Account account)
        {
            if (account == null || account.Facility == null)
                return false;

            return (!account.IsShortRegistered||(account.AccountCreatedDate == DateTime.MinValue && DateTime.Today >= FeatureStartDate) ||
                   account.AccountCreatedDate >= FeatureStartDate);
        }

        /// <summary>
        /// Gets the feature start date.
        /// </summary>
        /// <value>The feature start date.</value>
        public DateTime FeatureStartDate
        {
            get
            {
                string startDate = ConfigurationManager.AppSettings[EMAIL_ADDRESS_REQUIRED_START_DATE];
                if (patientEmailAddressVisibleStartDate.Equals(DateTime.MinValue) && startDate != null)
                {
                    patientEmailAddressVisibleStartDate = DateTime.Parse(startDate);
                }
                return patientEmailAddressVisibleStartDate;
            }
        }

        private const string EMAIL_ADDRESS_REQUIRED_START_DATE = "EMAIL_ADDRESS_REQUIRED_START_DATE";
    }
}