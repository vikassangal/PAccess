using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AdmittingPhysicianRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AdmittingPhysicianRequired : LeafRule
    {
        #region Events

        public event EventHandler AdmittingPhysicianRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.AdmittingPhysicianRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.AdmittingPhysicianRequiredEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.AdmittingPhysicianRequiredEvent = null;  
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || 
                context.GetType() != typeof( Account ) )
            {                
                return true;
            } 	            
            
            Account anAccount = ((Account)context);

            if( anAccount.AdmittingPhysician == null ||
                anAccount.AdmittingPhysician.FirstName.Trim() + anAccount.AdmittingPhysician.LastName.Trim() == string.Empty ) 
            {
                if( this.FireEvents && AdmittingPhysicianRequiredEvent != null )
                {
                    this.AdmittingPhysicianRequiredEvent(this, null);
                }
            
                return false;
            }
            else
            {
                return true;
            }           
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

        public AdmittingPhysicianRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
