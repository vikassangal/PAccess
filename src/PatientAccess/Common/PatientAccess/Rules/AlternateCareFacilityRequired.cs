using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AlternateCareFacilityRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AlternateCareFacilityRequired : LeafRule
    {

        [NonSerialized]
        private AlternateCareFacilityFeatureManager _alternateCareFacilityFeatureManager;

        internal AlternateCareFacilityFeatureManager AlternateCareFacilityFeatureManager
        {
            get { return _alternateCareFacilityFeatureManager; }
            set { _alternateCareFacilityFeatureManager = value; }
        }
        #region Events
        
        public event EventHandler AlternateCareFacilityRequiredEvent;
        
        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler( EventHandler eventHandler )
        {
            AlternateCareFacilityRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            AlternateCareFacilityRequiredEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            AlternateCareFacilityRequiredEvent = null;  
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || 
                context.GetType() != typeof( Account ) )
            {                
                return true;
            }
            AlternateCareFacilityFeatureManager = new AlternateCareFacilityFeatureManager();
            Account anAccount = (Account) context;
            if( anAccount.AdmitSource != null && 
                AlternateCareFacilityFeatureManager.IsAlternateCareFacilityEnabledForActivityAndAdmitSource( anAccount.Activity, anAccount.AdmitSource )&&
                AlternateCareFacilityFeatureManager.ShouldWeEnableAlternateCareFacilityFeature(anAccount.AccountCreatedDate , anAccount.AdmitDate)  &&
                String.IsNullOrEmpty( anAccount.AlternateCareFacility ) )
            {
                if( FireEvents && AlternateCareFacilityRequiredEvent != null )
                {
                    AlternateCareFacilityRequiredEvent( this, null );
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
        public AlternateCareFacilityRequired()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
