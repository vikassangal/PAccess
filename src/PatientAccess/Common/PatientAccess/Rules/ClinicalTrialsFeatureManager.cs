using System;
using System.Collections.Specialized;

namespace PatientAccess.Rules
{
    public interface IClinicalTrialsFeatureManager
    {
        bool ShouldWeEnableClinicalResearchFields( DateTime admitDate, DateTime today);
    }

    [Serializable]
    public class ClinicalTrialsFeatureManager : IClinicalTrialsFeatureManager
    {
        private NameValueCollection ApplicationSettings { get; set; }

        public ClinicalTrialsFeatureManager( NameValueCollection appSettings )
        {
            ApplicationSettings = appSettings;

        }

        public bool ShouldWeEnableClinicalResearchFields( DateTime admitDate, DateTime today)
        {
            if ( ( admitDate == DateTime.MinValue && today >= FeatureStartDate ) || admitDate >= FeatureStartDate )
            {
                return true;
            }

            return false;
        }

        private DateTime FeatureStartDate
        {
            get { return DateTime.Parse( ApplicationSettings[CLINICALRESEARCHFIELDS_START_DATE] ); }
        }

        internal const string CLINICALRESEARCHFIELDS_START_DATE = "CLINICALRESEARCHFIELDS_START_DATE";
    } 
}
