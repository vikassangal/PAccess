using System;
using System.Configuration;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    [Serializable]
    public class PrimaryCarePhysicianRequiredFeatureManager : IPrimaryCarePhysicianRequiredFeatureManager
    {
        #region Methods
        private bool IsAccountCreatedAfterFeatureStartDate(Account account)
        {
            if (account != null && (account.AccountCreatedDate == DateTime.MinValue && DateTime.Today >= FeatureStartDate ||
                account.AccountCreatedDate >= FeatureStartDate))
            {
                return true;
            }

            return false;
        }

        public bool IsPrimarycarephysicianRequiredfor(Account account)
        {
            return account != null &&
                   IsPrimaryCarePhysicianRequiredValidForDate(account) &&
                   IsPrimaryCarePhysicianRequiredValidForPatientType(account);
        }

        private bool IsPrimaryCarePhysicianRequiredValidForDate(Account account)
        {
            return IsAccountCreatedAfterFeatureStartDate(account) || IsActivatePreRegistrationActivity(account);
        }

        private static bool IsPrimaryCarePhysicianRequiredValidForPatientType(Account account)
        {
            return
                !account.KindOfVisit.IsNonPatient;
        }

        #endregion Methods

        #region Private Methods

        private bool IsActivatePreRegistrationActivity(Account account)
        {
            if ( account.Activity.IsActivatePreAdmitNewbornActivity() ||
                account.Activity.IsActivatePreRegisterActivity()  )
            {
                return true;
            }
            return false;
        }

        #endregion Private Methods

        #region Properties
        public DateTime FeatureStartDate
        {
            get
            {
                string startDate = ConfigurationManager.AppSettings[PRIMARYCAREPHYSICIAN_REQUIRED_START_DATE_SET];
                if (primaryCarePhysicianRequiredStartDate.Equals(DateTime.MinValue) && startDate != null)
                {
                    primaryCarePhysicianRequiredStartDate = DateTime.Parse(startDate);
                }
                return primaryCarePhysicianRequiredStartDate;
            }
        }

        #endregion Properties

        #region Data Elements

        private DateTime primaryCarePhysicianRequiredStartDate = DateTime.MinValue;

        #endregion Data Elements

        #region Constants

        private const string PRIMARYCAREPHYSICIAN_REQUIRED_START_DATE_SET = "PRIMARYCAREPHYSICIAN_REQUIRED_START_DATE";

        #endregion Constants
    }
}
