using System;
using System.Configuration;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// /// Applies Required rule to the Account.IsPatientInClinicalResearchStudy property; 
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PatientInClinicalstudyRequired : LeafRule
    {
        [NonSerialized]
        private ClinicalTrialsFeatureManager _clinicalTrialsFeatureManager;

        internal ClinicalTrialsFeatureManager ClinicalTrialsFeatureManager
        {
            get { return _clinicalTrialsFeatureManager; }
            set { _clinicalTrialsFeatureManager = value; }
        }

        #region Events

        public event EventHandler IspatientInClinicalResearchStudyRequiredEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler( EventHandler eventHandler )
        {
            IspatientInClinicalResearchStudyRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            IspatientInClinicalResearchStudyRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            IspatientInClinicalResearchStudyRequiredEvent = null;
        }

        public override bool CanBeAppliedTo( object context )
        {
            if ( context == null || !( context is IAccount ) )
            {
                return true;
            }

            IAccount model = ( IAccount )context;

            if ( model.Facility != null &&
                !model.Facility.IsValidForClinicalResearchFields() )
            {
                return true;
            }

            ClinicalTrialsFeatureManager = new ClinicalTrialsFeatureManager( ConfigurationManager.AppSettings );

            if ( ClinicalTrialsFeatureManager.ShouldWeEnableClinicalResearchFields( model.AdmitDate, DateTime.Today ) && 
                 model.IsPatientInClinicalResearchStudy.Code.Trim() == string.Empty )
            {
                if ( FireEvents && IspatientInClinicalResearchStudyRequiredEvent != null )
                {
                    IspatientInClinicalResearchStudyRequiredEvent( this, null );
                }

                return false;
            }

            return true;
        }

        public override void ApplyTo( object context )
        {

        }

        public override bool ShouldStopProcessing()
        {
            return false;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
