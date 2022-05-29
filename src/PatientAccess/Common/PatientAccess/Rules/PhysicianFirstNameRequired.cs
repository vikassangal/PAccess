using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PhysicianFirstNameRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PhysicianFirstNameRequired : LeafRule
    {
        #region Events

        public event EventHandler PhysicianFirstNameRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.PhysicianFirstNameRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.PhysicianFirstNameRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PhysicianFirstNameRequiredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Physician ) )
            {
                return true;
            }             
            
            Physician aPhysician = (Physician)context;

            if( aPhysician.Name.FirstName == null ||
                aPhysician.Name.FirstName == string.Empty )
            {
                if( this.FireEvents && PhysicianFirstNameRequiredEvent != null )
                {
                    this.PhysicianFirstNameRequiredEvent(this, null);
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

        public PhysicianFirstNameRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
