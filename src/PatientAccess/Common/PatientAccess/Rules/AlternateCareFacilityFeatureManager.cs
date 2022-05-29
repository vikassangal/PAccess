using System;
using System.Configuration;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    public interface IAlternateCareFacilityFeatureManager
    {
        bool IsAlternateCareFacilityEnabledForDate(DateTime date);
        bool IsAlternateCareFacilityEnabledForActivityAndAdmitSource( Activity activity, AdmitSource admitSource );
        bool ShouldWeEnableAlternateCareFacilityFeature(DateTime AccountCreatedDate, DateTime AdmitDate);
    }

    [Serializable]
    public class AlternateCareFacilityFeatureManager : IAlternateCareFacilityFeatureManager
    {
        private DateTime AlternateCareFacilityFeatureStartDate = DateTime.MinValue;
        public AlternateCareFacilityFeatureManager()
        {
        }
        /// <summary>
        /// Determines whether [is alternate care facility enabled for activity and admit source] [the specified activity].
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <param name="admitSource">The admit source.</param>
        /// <returns>
        /// 	<c>true</c> if [is alternate care facility enabled for activity and admit source] [the specified activity]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAlternateCareFacilityEnabledForActivityAndAdmitSource( Activity activity, AdmitSource admitSource )
        {
            if( !activity.IsAdmitNewbornActivity() && admitSource.IsAdmitSourceValidToEnableAlternateCareFacility() )
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether [is alternate care facility enabled for date] [the specified date].
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>
        /// 	<c>true</c> if [is alternate care facility enabled for date] [the specified date]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAlternateCareFacilityEnabledForDate( DateTime  date )
        {
            if ( ( date == DateTime.MinValue && DateTime.Today >= FeatureStartDate ) || 
                 ( date >= FeatureStartDate ) )
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Shoulds the we enable alternate care facility feature.
        /// </summary>
        /// <param name="accountCreatedDate">The account created date.</param>
        /// <param name="admitDate">The admit date.</param>
        /// <returns></returns>
        public bool ShouldWeEnableAlternateCareFacilityFeature( DateTime accountCreatedDate, DateTime admitDate )
        {
            return ( IsAlternateCareFacilityEnabledForDate(accountCreatedDate) &&
                    IsAlternateCareFacilityEnabledForDate(admitDate) );
        }

        /// <summary>
        /// Gets the feature start date.
        /// </summary>
        /// <value>The feature start date.</value>
        private DateTime FeatureStartDate
        {
            get
            {
                if ( AlternateCareFacilityFeatureStartDate.Equals( DateTime.MinValue ))
                {

                    AlternateCareFacilityFeatureStartDate =
                        DateTime.Parse( ConfigurationManager.AppSettings[ALTERNATECAREFACILITY_START_DATE] );
                }
                return AlternateCareFacilityFeatureStartDate;
            }
        }
       
        internal const string ALTERNATECAREFACILITY_START_DATE = "ALTERNATECAREFACILITY_START_DATE";
    } 
}
