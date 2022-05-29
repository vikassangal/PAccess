using System;
using System.Configuration;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    public interface IHospitalCommunicationOptInFeatureManager
    {
        bool ShouldFeatureBeEnabled(Account account);
        bool IsValidActivityForEmailAddress(Account anAccount);
    }
    [Serializable]
    public class HospitalCommunicationOptInFeatureManager : IHospitalCommunicationOptInFeatureManager
    {
        private DateTime HospitalCommunicationOptInVisibleStartDate = DateTime.MinValue;


        /// <summary>
        /// Determines whether [is Hospital Communication EmailOpt in View visible for]
        /// Patient type.
        /// </summary>
        /// <param name="account"></param>
        /// 
        /// <returns>
        /// 	<c>true</c> if Hospital Communication  visible for the; 
        /// otherwise, <c>false</c>.
        /// </returns>
        private bool IsHospitalCommunicationOptInVisibleForPatientType(Account account)
        {
            return (account.KindOfVisit.Code != VisitType.PREREG_PATIENT &&
                        !account.IsEDorUrgentCarePremseAccount  );
        }
        /// <summary>
        /// Determines whether [is HospitalCommunicationOptIn visible for date] [the specified date].
        /// </summary>
        /// <param name="accountCreatedDate"> </param>
        /// <returns>
        ///<c>true</c> if [is Hospital Communication  visible for date] [the specified date]; 
        /// otherwise, <c>false</c>.
        /// </returns>
        private bool IsHospitalCommunicationOptInVisibleForAccountCreatedDate(DateTime accountCreatedDate)
        {
            if ((accountCreatedDate == DateTime.MinValue && DateTime.Today >= FeatureStartDate) ||
                 (accountCreatedDate >= FeatureStartDate))
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// Should we make Patient Hospital Communication  Opt In Feature Visible for account created date, 
        /// Patient type and Hospital service.
        /// </summary>
        /// <param name="account">account .</param>

        /// <returns></returns>
        public bool ShouldFeatureBeEnabled(Account account)
        {
            if (account == null)
                return false;
            return (IsHospitalCommunicationOptInVisibleForAccountCreatedDate(account.AccountCreatedDate) &&
                    IsHospitalCommunicationOptInVisibleForPatientType(account));
        }

        public bool IsValidActivityForEmailAddress(Account anAccount)
        {
            return !anAccount.Activity.IsValidTransferActivityForEmailAddress;
        }

        /// <summary>
        /// Gets the feature start date.
        /// </summary>
        /// <value>The feature start date.</value>
        public DateTime FeatureStartDate
        {
            get
            {
                string startDate = ConfigurationManager.AppSettings[HOSPCOMMOPT_START_DATE];
                if (HospitalCommunicationOptInVisibleStartDate.Equals(DateTime.MinValue) && startDate != null)
                {
                    HospitalCommunicationOptInVisibleStartDate = DateTime.Parse(startDate);
                }
                return HospitalCommunicationOptInVisibleStartDate;
            }
        }

        private const string HOSPCOMMOPT_START_DATE = "HOSPCOMMOPT_START_DATE";
    }
}
