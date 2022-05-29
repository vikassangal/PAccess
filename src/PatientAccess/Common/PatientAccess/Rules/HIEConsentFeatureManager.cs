using System;
using System.Configuration;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    public interface IHIEConsentFeatureManager
    {
        bool IsHIEConsentFeatureManagerEnabled(Account account);
    }

    [Serializable]
    public class HIEConsentFeatureManager : IHIEConsentFeatureManager
    {
        private DateTime HIEConsentStartDate = DateTime.MinValue;
        public bool IsHIEConsentFeatureManagerEnabled(Account account)
        {
            return ((account.AccountCreatedDate == DateTime.MinValue && DateTime.Today >= FeatureStartDate) ||
                    account.AccountCreatedDate >= FeatureStartDate) &&
                   !account.KindOfVisit.IsPreRegistrationPatient &&
                   !account.Activity.IsPreMSEActivities() &&
                   !account.Activity.IsUCCPreMSEActivity();
        }

        public DateTime FeatureStartDate
        {
            get
            {
                string startDate = ConfigurationManager.AppSettings[HIECONSENT_START_DATE];
                if (HIEConsentStartDate.Equals(DateTime.MinValue) && startDate != null)
                {
                    HIEConsentStartDate = DateTime.Parse(startDate);
                }
                return HIEConsentStartDate;
            }
        }

        private const string HIECONSENT_START_DATE = "HIECONSENT_START_DATE";
    }
}
