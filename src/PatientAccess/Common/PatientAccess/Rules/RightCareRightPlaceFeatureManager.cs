using System;
using System.Collections.Specialized;
using PatientAccess.Domain;
using PatientAccess.Domain.UCCRegistration;

namespace PatientAccess.Rules
{
    public interface IRightCareRightPlaceFeatureManager
    {
        bool IsFeatureEnabledFor( DateTime accountCreatedDate, DateTime today );
        bool ShouldRCRPFieldsBeVisible( Facility facility, VisitType kindOfVisit ,Activity activity);
    }

    [Serializable]
    public class RightCareRightPlaceFeatureManager : IRightCareRightPlaceFeatureManager
    {
        private NameValueCollection ApplicationSettings { get; set; }

        public RightCareRightPlaceFeatureManager( NameValueCollection appSettings )
        {
            ApplicationSettings = appSettings;

        }

        public bool IsFeatureEnabledFor( DateTime accountCreatedDate , DateTime today )
        {
            if ( ( accountCreatedDate == DateTime.MinValue && today >= FeatureStartDate ) || 
                 ( accountCreatedDate >= FeatureStartDate ) )
            {
                return true;
            }
            return false;
        }

        public bool ShouldRCRPFieldsBeVisible( Facility facility , VisitType kindOfVisit , Activity activity)
        {
            if (facility.IsValidForRCRPFields() && 
                (kindOfVisit.Code.Equals(VisitType.EMERGENCY_PATIENT) || activity.GetType() == typeof(UCCPostMseRegistrationActivity)))
            {
                return true;
            }
            return false;
        }

        private DateTime FeatureStartDate
        {
            get { return DateTime.Parse( ApplicationSettings[RIGHTCARE_RIGHTPLACE_START_DATE] ); }
        }

        private const string RIGHTCARE_RIGHTPLACE_START_DATE = "RIGHTCARE_RIGHTPLACE_START_DATE";
    } 
}
