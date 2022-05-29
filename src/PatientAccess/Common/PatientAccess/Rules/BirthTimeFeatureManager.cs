using System;
using System.Configuration;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    public interface IBirthTimeFeatureManager
    {
        bool IsBirthTimeEnabledForDate( DateTime date );
        bool IsBirthTimeEnabledForDate( Activity activity, DateTime date );
    }

    [Serializable]
    public class BirthTimeFeatureManager : IBirthTimeFeatureManager
    {
        private DateTime BirthTimeFeatureStartDate = DateTime.MinValue;

        /// <summary>
        /// Determines whether [is alternate care facility enabled for date] [the specified date].
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>
        /// 	<c>true</c> if [is alternate care facility enabled for date] [the specified date]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsBirthTimeEnabledForDate( DateTime date )
        {
            if ( ( date == DateTime.MinValue && DateTime.Today >= FeatureStartDate ) ||
                 ( date >= FeatureStartDate ) )
            {
                return true;
            }
            return false;
        }

        public bool IsBirthTimeEnabledForDate( Activity activity, DateTime date )
        {
            if(activity==null)
                return IsBirthTimeEnabledForDate( date );

            if ( activity.IsPreMSEActivities() ||
                activity.IsPostMSEActivity() ||
                IsEditPostMseActivity(activity) ||
                activity.IsUCCPreMSEActivity() ||
                activity.IsUccPostMSEActivity() )
            {
                if ( ( date == DateTime.MinValue && DateTime.Today >= MseFeatureStartDate ) ||
                 ( date >= MseFeatureStartDate ) )
                {
                    return true;
                }
                return false;
            }
            return IsBirthTimeEnabledForDate(date);
            
        }

        private static bool IsEditPostMseActivity(Activity activity)
        {
            return activity.IsMaintenanceActivity() && activity.AssociatedActivityType!=null && activity.AssociatedActivityType==typeof(PostMSERegistrationActivity);
        }

        private DateTime FeatureStartDate
        {
            get
            {
                if ( BirthTimeFeatureStartDate.Equals( DateTime.MinValue ) )
                {

                    BirthTimeFeatureStartDate =
                        DateTime.Parse( ConfigurationManager.AppSettings[BIRTHTIME_START_DATE] );
                }
                return BirthTimeFeatureStartDate;
            }
        }

        private DateTime MseFeatureStartDate
        {
            get
            {
                if ( BirthTimeFeatureStartDate.Equals( DateTime.MinValue ) )
                {

                    BirthTimeFeatureStartDate =
                        DateTime.Parse( ConfigurationManager.AppSettings[MSE_BIRTHTIME_START_DATE] );
                }
                return BirthTimeFeatureStartDate;
            }
        }
        private const string BIRTHTIME_START_DATE = "BIRTHTIME_START_DATE";
        private const string MSE_BIRTHTIME_START_DATE = "MSE_BIRTHTIME_START_DATE";
    }
}
