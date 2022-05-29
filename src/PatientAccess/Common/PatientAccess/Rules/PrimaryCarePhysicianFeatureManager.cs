using System;
using System.Configuration;

namespace PatientAccess.Rules
{
    public interface IPrimaryCarePhysicianFeatureManager
    {
        bool IsPrimaryCarePhysicianEnabledForDate(DateTime date);
    }

    [Serializable]
    public class PrimaryCarePhysicianFeatureManager : IPrimaryCarePhysicianFeatureManager
    {
        private DateTime PrimaryCarePhysicianFeatureStartDate = DateTime.MinValue;

        /// <summary>
        /// Determines whether [is alternate care facility enabled for date] [the specified date].
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>
        /// 	<c>true</c> if [is alternate care facility enabled for date] [the specified date]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPrimaryCarePhysicianEnabledForDate( DateTime  date )
        {
            if ( ( date == DateTime.MinValue && DateTime.Today >= FeatureStartDate ) || 
                 ( date >= FeatureStartDate ) )
            {
                return true;
            }
            return false;
        }

       
        private DateTime FeatureStartDate
        {
            get
            {
                if ( PrimaryCarePhysicianFeatureStartDate.Equals( DateTime.MinValue ))
                {

                    PrimaryCarePhysicianFeatureStartDate =
                        DateTime.Parse( ConfigurationManager.AppSettings[PRIMARYCAREPHYSICIAN_START_DATE] );
                }
                return PrimaryCarePhysicianFeatureStartDate;
            }
        }

        private const string PRIMARYCAREPHYSICIAN_START_DATE = "PRIMARYCAREPHYSICIAN_START_DATE";
    } 
}
