using System;
using System.Configuration;
using PatientAccess.Domain;


namespace PatientAccess.Rules
{
    [Serializable]
    public class ShareHIEDataFeatureManager : IShareHIEDataFeatureManager
    {
        private DateTime shareHIEDataFeatureStartDate = DateTime.MinValue;

        /// <summary>
        /// Determines whether [share HIE Data enabled for date] [the specified date].
        /// </summary>
        /// <param name="account">The account.</param>
        /// <returns>
        /// 	<c>true</c> if [Share HIE Data enabled for date] [the specified date]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsShareHieDataEnabledForDate(Account account)
        {
            return IsShareHieDataEnabledForCreatedDate(account) ||
                   IsShareHieDataEnabledForAdmitDate(account);
        }

        private bool IsShareHieDataEnabledForCreatedDate(Account account)
        {
            if (account != null &&
                (account.AccountCreatedDate == DateTime.MinValue && DateTime.Today >= FeatureStartDate ||
                 account.AccountCreatedDate >= FeatureStartDate))
            {
                return true;
            }
            return false;
        }
        private bool IsShareHieDataEnabledForAdmitDate(Account account)
        {
            if (account != null &&
                (account.AdmitDate == DateTime.MinValue && DateTime.Today >= FeatureStartDate ||
                 account.AdmitDate >= FeatureStartDate))
            {
                return true;
            }
            return false;
        }
        public bool IsShareHieDataEnabledforaccount(Account account)
        {
            if ( account != null && account.Activity!=null )
            {
                return (!IsPreAdmitNewbornActivity(account) && 
                        IsShareHieDataEnabledForDate(account));
            }
            return false;
        }

        public YesNoFlag DefaultShareHieDataForFacility(Facility facility)
        {
            if (facility != null && facility != null)
            {
                return facility.DefaultShareHIEData();
            }
            return YesNoFlag.Blank;
        }

        private bool IsPreAdmitNewbornActivity(Account account)
        {
            return (account.Activity.IsPreAdmitNewbornActivity() ||
                    account.Activity.IsEditPreAdmitNewbornActivity());
        }

        public DateTime FeatureStartDate
        {
            get
            {
                string startDate = ConfigurationManager.AppSettings[share_HIE_Data_FeatureStartDate];

                if (shareHIEDataFeatureStartDate.Equals(DateTime.MinValue) && !String.IsNullOrEmpty(startDate))
                {
                    DateTime.TryParse(startDate, out shareHIEDataFeatureStartDate);
                }

                return shareHIEDataFeatureStartDate;
            }
        }

        private const string share_HIE_Data_FeatureStartDate = "SHARE_HIE_DATA_START_DATE";
    } 
}
