using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PhysicianStateLicenseNumberPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PhysicianStateLicenseNumberPreferred : LeafRule
    {
        #region Events

        public event EventHandler PhysicianStateLicenseNumberPreferredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.PhysicianStateLicenseNumberPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.PhysicianStateLicenseNumberPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PhysicianStateLicenseNumberPreferredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Physician ) )
            {
                return true;
            }     

            Physician aPhysician = (Physician)context;

            if( aPhysician.StateLicense == null ||
                aPhysician.StateLicense == string.Empty )
            {
                if( this.FireEvents && PhysicianStateLicenseNumberPreferredEvent != null )
                {
                    this.PhysicianStateLicenseNumberPreferredEvent(this, null);
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

        public PhysicianStateLicenseNumberPreferred()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
