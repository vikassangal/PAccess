using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PhysicianUPINNumberRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PhysicianUPINNumberRequired : LeafRule
    {
        #region Events

        public event EventHandler PhysicianUPINNumberRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.PhysicianUPINNumberRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.PhysicianUPINNumberRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PhysicianUPINNumberRequiredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Physician ) )
            {
                return true;
            }     
            
            Physician aPhysician = (Physician)context;

            if( aPhysician.UPIN == null ||
                aPhysician.UPIN == string.Empty )
            {
                if( this.FireEvents && PhysicianUPINNumberRequiredEvent != null )
                {
                    this.PhysicianUPINNumberRequiredEvent(this, null);
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

        public PhysicianUPINNumberRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
