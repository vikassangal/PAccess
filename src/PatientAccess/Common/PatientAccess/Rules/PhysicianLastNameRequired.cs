using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PhysicianLastNameRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PhysicianLastNameRequired : LeafRule
    {
        #region Events

        public event EventHandler PhysicianLastNameRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.PhysicianLastNameRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.PhysicianLastNameRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PhysicianLastNameRequiredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Physician ) )
            {
                return true;
            }     

            Physician aPhysician = (Physician)context;

            if( aPhysician.Name.LastName == null ||
                aPhysician.Name.LastName == string.Empty )
            {
                if( this.FireEvents && PhysicianLastNameRequiredEvent != null )
                {
                    this.PhysicianLastNameRequiredEvent(this, null);
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

        public PhysicianLastNameRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
