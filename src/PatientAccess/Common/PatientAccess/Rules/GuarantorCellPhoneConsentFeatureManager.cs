using System;
using System.Configuration;
using PatientAccess.Domain;


namespace PatientAccess.Rules
{
    [Serializable]
    public class GuarantorCellPhoneConsentFeatureManager
    {
        private DateTime CellPhoneConsentRuleForCOSSignedStartDate  = DateTime.MinValue;


        public bool IsCellPhoneConsentRuleForCOSEnabledforaccount(Account account)
        {
            return  IsCellPhoneConsentRuleForCOSEnabledForCreatedAdmitDate(account);
        }

        
        private bool IsCellPhoneConsentRuleForCOSEnabledForCreatedAdmitDate(Account account)
        {
            if (account != null &&
                (account.AdmitDate == DateTime.MinValue && DateTime.Today >= FeatureStartDate ||
                 account.AdmitDate >= FeatureStartDate))
            {
                return true;
            }
            return false;
        }
       
         
        public DateTime FeatureStartDate
        {
            get
            {
                string startDate = ConfigurationManager.AppSettings[cellPhoneConsentRuleForCOSSignedStartDate];

                if (CellPhoneConsentRuleForCOSSignedStartDate.Equals(DateTime.MinValue) && !String.IsNullOrEmpty(startDate))
                {
                    DateTime.TryParse(startDate, out CellPhoneConsentRuleForCOSSignedStartDate);
                }

                return CellPhoneConsentRuleForCOSSignedStartDate;
            }
        }

        private const string cellPhoneConsentRuleForCOSSignedStartDate = "CELLPHONECONSENT_RULE_FOR_COSSIGNED_START_DATE";
    }
     
}
