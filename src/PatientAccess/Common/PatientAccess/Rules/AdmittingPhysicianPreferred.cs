using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AdmittingPhysicianPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AdmittingPhysicianPreferred : LeafRule
    {
              #region Events

        public event EventHandler AdmittingPhysicianPreferredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            AdmittingPhysicianPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            AdmittingPhysicianPreferredEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            AdmittingPhysicianPreferredEvent = null;  
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || 
                context.GetType() != typeof( Account ) )
            {                
                return true;
            } 	            
            
            var anAccount = ((Account)context);

            if( anAccount.AdmittingPhysician == null ||
                anAccount.AdmittingPhysician.FirstName.Trim() + anAccount.AdmittingPhysician.LastName.Trim() == string.Empty ) 
            {
                if( FireEvents && AdmittingPhysicianPreferredEvent != null )
                {
                    AdmittingPhysicianPreferredEvent(this, null);
                }
            
                return false;
            }
                return true;
        }

        public override void ApplyTo(object context)
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
