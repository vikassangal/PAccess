using System;
using System.Configuration;
using PatientAccess.Domain;
using PatientAccess.Utilities;

namespace PatientAccess.Rules
{
    public interface ICptCodesFeatureManager
    {
        bool ShouldFeatureBeVisible( Account account );
    }

    [Serializable]
    public class CptCodesFeatureManager : ICptCodesFeatureManager
    {
        private DateTime cptCodesVisibleStartDate = DateTime.MinValue;

        /// <summary>
        /// Determines whether [is CPT Codes visible for date] [the specified date].
        /// </summary>
        /// <param name="accountCreatedDate"> </param>
        /// <returns>
        ///<c>true</c> if [is CPT Codes visible for date] [the specified date]; 
        /// otherwise, <c>false</c>.
        /// </returns>
        public bool ShouldFeatureBeVisible( Account account )
        {
            Guard.ThrowIfArgumentIsNull( account, "account" );

            var accountCreatedDate = account.AccountCreatedDate;

            if ( ( accountCreatedDate == DateTime.MinValue && DateTime.Today >= FeatureStartDate ) ||
                 ( accountCreatedDate >= FeatureStartDate ) )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the feature start date.
        /// </summary>
        /// <value>The feature start date.</value>
        private DateTime FeatureStartDate
        {
            get
            {
                string startDate = ConfigurationManager.AppSettings[CPTCODES_START_DATE];
                if ( cptCodesVisibleStartDate.Equals( DateTime.MinValue ) && startDate != null )
                {
                    cptCodesVisibleStartDate = DateTime.Parse( startDate );
                }
                return cptCodesVisibleStartDate;
            }
        }

        private const string CPTCODES_START_DATE = "CPTCODES_START_DATE";
    }
}