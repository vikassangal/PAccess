using System;
using System.Configuration;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    [Serializable]
    public class COBReceivedAndIMFMReceivedFeatureManager : ICOBReceivedAndIMFMReceivedFeatureManager
    {
        private DateTime COBReceivedAndIMFMReceivedFeatureStartDate = DateTime.MinValue;
         private DateTime PreRegistrationIMFMReceivedFeatureStartDate = DateTime.MinValue;

        /// <summary>
        /// Determines whether [COB Received Flag and IMFM Received Flag enabled for date] [the specified date].
        /// </summary>
        /// <param name="account">The account.</param>
        /// <returns>
        /// 	<c>true</c> if [COB Received Flag and IMFM Received Flag enabled for date] [the specified date]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsCOBReceivedAndIMFMReceivedEnabledForDate (Account account)
        {
            if ( account != null &&
                 (account.AccountCreatedDate == DateTime.MinValue && DateTime.Today >= FeatureStartDate ||
                  account.AccountCreatedDate >= FeatureStartDate) )
            {
                return true;
            }
            return false;
        }
        private bool IsPreRegistrationIMFMReceivedEnabledForDate(Account account)
        {
            if (account != null &&
                 (account.AccountCreatedDate == DateTime.MinValue && DateTime.Today >= PregRegistration_IMFM_FeatureStartDate ||
                  account.AccountCreatedDate >= PregRegistration_IMFM_FeatureStartDate))
            {
                return true;
            }
            return false;
        }
        public bool IsCOBReceivedEnabledForAccount(Account account)
        {
            if ( account != null && account.KindOfVisit != null && account.FinancialClass != null )
            {
                return IsCOBReceivedAndIMFMReceivedEnabledForDate(account) 
                    && account.KindOfVisit.IsValidForCOBReceived 
                    && account.FinancialClass.IsValidForCOBReceived ;
            }
            return false;
        }

        public bool IsIMFMReceivedEnabledForAccount(Account account)
        {
            if ( account != null && account.KindOfVisit != null && account.FinancialClass != null )
            {
                if (account.KindOfVisit.IsInpatient)
                    return IsIMFMReceivedValidForRegistrationPatient(account);
                else if (account.KindOfVisit.IsPreRegistrationPatient)
                    return IsIMFMReceivedValidForPreRegistrationPatient(account);

            }
            return false;
        }

        private bool IsIMFMReceivedValidForRegistrationPatient(Account account)
        {
            return ( account.KindOfVisit.IsInpatient &&
                    IsCOBReceivedAndIMFMReceivedEnabledForDate(account)
                    && account.FinancialClass.IsValidForIMFMReceived);
        }

        private bool IsIMFMReceivedValidForPreRegistrationPatient(Account account)
        {
           return (account.KindOfVisit.IsPreRegistrationPatient && !account.IsDiagnosticPreregistrationAccount &&
            IsPreRegistrationIMFMReceivedEnabledForDate(account) &&
            account.HospitalService.IsPreAdmitCode && account.FinancialClass.IsValidForIMFMReceived);
        }
        public void IfApplicableResetCOBReceivedOn(Account account)
        {
            if ( account != null && account.COBReceived !=null )
            {
                if ( IsCOBReceivedAndIMFMReceivedEnabledForDate(account) && !IsCOBReceivedEnabledForAccount(account) )
                {
                    account.ResetCOBReceived();
                }
            }
        }

        public void IfApplicableResetIMFMReceivedOn(Account account)
        {
            if ( account != null && account.IMFMReceived != null )
            {
                if ( IsCOBReceivedAndIMFMReceivedEnabledForDate(account) && !IsIMFMReceivedEnabledForAccount(account) )
                {
                    account.ResetIMFMReceived();
                }
            }
        }
        public DateTime FeatureStartDate
        {
            get
            {
                string startDate = ConfigurationManager.AppSettings[COBReceivedAndIMFMReceived_FeatureStartDate];
                if (COBReceivedAndIMFMReceivedFeatureStartDate.Equals(DateTime.MinValue) && startDate !=null)
                {
                    COBReceivedAndIMFMReceivedFeatureStartDate = DateTime.Parse(startDate);
                }
                return COBReceivedAndIMFMReceivedFeatureStartDate;
            }
        }
         public DateTime PregRegistration_IMFM_FeatureStartDate
        {
            get
            {
                string startDate = ConfigurationManager.AppSettings[PREREGISTRATION_IMFM_START_DATE];
                if (PreRegistrationIMFMReceivedFeatureStartDate.Equals(DateTime.MinValue) && startDate !=null)
                {
                    PreRegistrationIMFMReceivedFeatureStartDate = DateTime.Parse(startDate);
                }
                return PreRegistrationIMFMReceivedFeatureStartDate;
            } 
         }

        private const string COBReceivedAndIMFMReceived_FeatureStartDate = "COB_IMFM_START_DATE";
        private const string PREREGISTRATION_IMFM_START_DATE = "PREREGISTRATION_IMFM_START_DATE";
    }
}
