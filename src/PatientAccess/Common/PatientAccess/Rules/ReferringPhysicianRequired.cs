using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ReferringPhysicianRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class ReferringPhysicianRequired : LeafRule
    {
        #region Events

        public event EventHandler ReferringPhysicianRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.ReferringPhysicianRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.ReferringPhysicianRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.ReferringPhysicianRequiredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }  
            
            Account anAccount = ((Account)context);

            if(  anAccount.ReferringPhysician == null ||
                 anAccount.ReferringPhysician.LastName.Trim() + anAccount.ReferringPhysician.LastName.Trim() == string.Empty ) 
            {
                if( this.FireEvents && ReferringPhysicianRequiredEvent != null )
                {
                    this.ReferringPhysicianRequiredEvent(this, null);
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

        public ReferringPhysicianRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
