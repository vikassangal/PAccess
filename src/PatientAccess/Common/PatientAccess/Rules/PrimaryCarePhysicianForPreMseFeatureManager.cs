using System;
using System.Collections.Specialized;

namespace PatientAccess.Rules
{
    [Serializable]
    public class PrimaryCarePhysicianForPreMseFeatureManager
    {
        public PrimaryCarePhysicianForPreMseFeatureManager( NameValueCollection appSettings )
        {
            ApplicationSettings = appSettings;
        }

        /// <summary>
        /// Determines whether the feature is enabled for the specified date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>
        /// 	<c>true</c> if feature is enabled for the specified date; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEnabledFor( DateTime date )
        {
            if ( ( date == DateTime.MinValue && DateTime.Today >= FeatureStartDate ) ||
                 ( date >= FeatureStartDate ) )
            {
                return true;
            }
            return false;
        }

        internal const string PRIMARYCAREPHYSICIAN_FOR_PREMSE_START_DATE = "PRIMARYCAREPHYSICIAN_FOR_PREMSE_START_DATE";
        private NameValueCollection ApplicationSettings { get; set; }

        private DateTime FeatureStartDate
        {
            get
            {
                return DateTime.Parse( ApplicationSettings[PRIMARYCAREPHYSICIAN_FOR_PREMSE_START_DATE] );
            }
        }
    }
}
