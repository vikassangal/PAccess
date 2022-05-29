using System;
using System.Configuration;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    public interface IPatientPortalOptInFeatureManager
    {
        bool ShouldFeatureBeEnabled( Account account );
    }

    [Serializable]
    public class PatientPortalOptInFeatureManager : IPatientPortalOptInFeatureManager
    {
        private DateTime patientPortalOptInVisibleStartDate = DateTime.MinValue;
        
        /// <summary>
        /// Determines whether [is Patient portalOpt in View visible for 
        /// Patient type and Hospital service.
        /// </summary>
        /// <param name="account"></param>
        /// 
        /// <returns>
        /// 	<c>true</c> if PatientPortalOpt visible for INPATIENT and EMERGENCY_PATIENT; 
        /// otherwise, <c>false</c>.
        /// </returns>
        private bool IsPatientPortalOptInVisibleForPatientTypeAndHsv(Account account)
        {
            return account != null && (account.KindOfVisit.Code == VisitType.INPATIENT ||
                                       ( account.KindOfVisit.Code == VisitType.OUTPATIENT && !account.IsUrgentCarePreMse )  ||
                                       account.KindOfVisit.Code == VisitType.RECURRING_PATIENT ||
                                       IsPatientPortalOptInVisibleForEDPatientAndHospitalService(account));
        }

        /// <summary>
        /// Determines whether [is PatientPortalOpt visible for date] [the specified date].
        /// </summary>
        /// <param name="accountCreatedDate"> </param>
        /// <returns>
        ///<c>true</c> if [is PatientPortalOpt visible for date] [the specified date]; 
        /// otherwise, <c>false</c>.
        /// </returns>
        private bool IsPatientPortalOptInVisibleForAdmitDate( DateTime accountCreatedDate )
        {
            if ( ( accountCreatedDate == DateTime.MinValue && DateTime.Today >= FeatureStartDate ) ||
                 ( accountCreatedDate >= FeatureStartDate ) )
            {
                return true;
            }

            return false;
        }

        private bool IsPatientPortalOptInVisibleForEDPatientAndHospitalService(Account account)
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
        /// Should we make Patient Portal Opt In Feature Visible for account created date, 
        /// Patient type and Hospital service.
        /// </summary>
        /// <param name="account">account .</param>

        /// <returns></returns>
        public bool ShouldFeatureBeEnabled( Account account )
        {
            if ( account == null || account.Facility == null )
                return false;

            return (account.Facility.IsValidForPatientPortalOptIn() &&
                    IsPatientPortalOptInVisibleForAdmitDate(account.AccountCreatedDate) &&
                    IsPatientPortalOptInVisibleForPatientTypeAndHsv(account));
        }

        /// <summary>
        /// Gets the feature start date.
        /// </summary>
        /// <value>The feature start date.</value>
        public DateTime FeatureStartDate
        {
            get
            {
                string startDate = ConfigurationManager.AppSettings[PATIENTPORTALOPT_START_DATE];
                if (patientPortalOptInVisibleStartDate.Equals(DateTime.MinValue) && startDate != null)
                {
                    patientPortalOptInVisibleStartDate = DateTime.Parse(startDate);
                }
                return patientPortalOptInVisibleStartDate;
            }
        }

        private const string PATIENTPORTALOPT_START_DATE = "PATIENTPORTALOPT_START_DATE";
    }
}