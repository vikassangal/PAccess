using System;
using System.Configuration;

namespace PatientAccess.Rules
{
    public interface IRightToRestrictFeatureManager
    {
        bool IsRightToRestrictEnabledForAccountCreatedDate( DateTime admitDate );
    }

    [Serializable]
    public class RightToRestrictFeatureManager : IRightToRestrictFeatureManager
    {
        private DateTime rightToRestrictStartDate = DateTime.MinValue;
        public bool IsRightToRestrictEnabledForAccountCreatedDate( DateTime accountCreatedDate )
        {
            if ((accountCreatedDate == DateTime.MinValue && DateTime.Today >= FeatureStartDate) || 
                accountCreatedDate >= FeatureStartDate )
            {
                return true;
            }

            return false;
        }

        public DateTime FeatureStartDate
        {
            get
            {
                string startDate = ConfigurationManager.AppSettings[RIGHTTORESTRICT_START_DATE];
                if (rightToRestrictStartDate.Equals(DateTime.MinValue) && startDate != null)
                {
                    rightToRestrictStartDate = DateTime.Parse(startDate);
                }
                return rightToRestrictStartDate;
            }
        }

        private const string RIGHTTORESTRICT_START_DATE = "RIGHTTORESTRICT_START_DATE";
    } 
}
