using System;
using System.Configuration;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    public interface IAuthorizePortalUserFeatureManager
    {
        bool ShouldFeatureBeEnabled(Account account);
        bool IsAuthorizePortalUserRequiredForNewPatient(Account account);
        bool IsAuthorizePortalUserRequiredForExistingPatient(Account account);

    }

    [Serializable]
    public class AuthorizePortalUserFeatureManager : IAuthorizePortalUserFeatureManager
    {
        private const string AUTHORIZEPORTAL_START_DATE = "AUTHORIZEPORTAL_START_DATE";
        private DateTime authorizePortalVisibleStartDate = DateTime.MinValue;
        /// <summary>
        /// Determines whether [is Authorize patient portal user visible for 
        /// Patient type and Hospital service.
        /// </summary>
        /// <param name="account"></param>
        /// 
        /// <returns>
        /// 	<c>true</c> if authorizeportaluser is visible for INPATIENT and EMERGENCY_PATIENT; 
        /// otherwise, <c>false</c>.
        /// </returns>
        private bool IsAuthorizePortalUserVisibleForPatientTypeAndHsv(Account account)
        {
            return account != null && (account.KindOfVisit.Code == VisitType.INPATIENT ||
                                       (account.KindOfVisit.Code == VisitType.OUTPATIENT && !account.IsUrgentCarePreMse) ||
                                       account.KindOfVisit.Code == VisitType.RECURRING_PATIENT ||
                                       IsAuthorizePortalVisibleForEDPatientAndHospitalService(account));
        }

        /// <summary>
        /// Determines whether [is PatientPortalOpt visible for date] [the specified date].
        /// </summary>
        /// <param name="accountCreatedDate"> </param>
        /// <returns>
        ///<c>true</c> if [is PatientPortalOpt visible for date] [the specified date]; 
        /// otherwise, <c>false</c>.
        /// </returns>
        private bool IsAuthorizePortalVisibleForCreateDate(DateTime accountCreatedDate)
        {
            if ((accountCreatedDate == DateTime.MinValue && DateTime.Today >= FeatureStartDate) ||
                (accountCreatedDate >= FeatureStartDate))
            {
                return true;
            }

            return false;
        }
        private bool IsAuthorizePortalUserValidForAccount(Account account)
        {
            if (account != null && account.Activity != null)
            {
                return account.Activity.IsValidForAuthorizeAdditionalPortalUserFeature;
            }
            return false;
        }
        private bool IsAuthorizePortalVisibleForEDPatientAndHospitalService(Account account)
        {
            if (account.KindOfVisit.Code != VisitType.EMERGENCY_PATIENT)
            {
                return false;
            }
            if (account.Activity.IsPostMSEActivity() || account.Activity.IsMaintenanceActivity())
            {
                return true;
            }

            return account.Activity.IsRegistrationActivity() &&
                   (account.HospitalService.Code == HospitalService.HSV58 ||
                    account.HospitalService.Code == HospitalService.HSV59);
        }

        /// <summary>
        /// Should we make Authorize Patient Portal Feature Visible for account created date, 
        /// Patient type and Hospital service.
        /// </summary>
        /// <param name="account">account .</param>

        /// <returns></returns>
        public bool ShouldFeatureBeEnabled(Account account)
        {
            if (account == null || account.Facility == null)
                return false;

            return (account.Facility.IsValidForAuthorizePortalUser() &&

                    (IsAuthorizePortalVisibleForCreateDate(account.AccountCreatedDate) ||
                     (IsAuthorizePortalUserValidForAccount(account) && FeatureStartDate <= DateTime.Today)) &&

                     IsAuthorizePortalUserVisibleForPatientTypeAndHsv(account)
                    );
        }

        public bool IsAuthorizePortalUserRequiredForNewPatient(Account account)
        {
            return ((ShouldFeatureBeEnabled(account) &&
                    (!account.IsCOSSignedNo) &&
                    account.AuthorizeAdditionalPortalUsers.IsBlank));
        }
        public bool IsAuthorizePortalUserRequiredForExistingPatient(Account account)
        {
            return ((ShouldFeatureBeEnabled(account) &&
                    (!account.COSSigned.IsRefused) &&
                    account.AuthorizeAdditionalPortalUsers.IsBlank));
        }

        /// <summary>
        /// Gets the feature start date.
        /// </summary>
        /// <value>The feature start date.</value>
        public DateTime FeatureStartDate
        {
            get
            {
                string startDate = ConfigurationManager.AppSettings[AUTHORIZEPORTAL_START_DATE];
                if (authorizePortalVisibleStartDate.Equals(DateTime.MinValue) && startDate != null)
                {
                    authorizePortalVisibleStartDate = DateTime.Parse(startDate);
                }
                return authorizePortalVisibleStartDate;
            }
        }
    }

}
