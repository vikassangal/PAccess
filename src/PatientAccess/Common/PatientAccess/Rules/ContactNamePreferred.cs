using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ContactNamePreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class ContactNamePreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler ContactNamePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            ContactNamePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            ContactNamePreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.ContactNamePreferredEvent = null;   
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( EmergencyContact ) )
            {
                return true;
            }

            EmergencyContact anEC = context as EmergencyContact;

            // only preferred for PreReg and only if the service code is not SP, LB or AB


            if( anEC.Name.Length > 0 )
            {
                return true;
            }
            else
            {
                if( this.FireEvents && ContactNamePreferredEvent != null )
                {
                    ContactNamePreferredEvent( this, new PropertyChangedArgs( this.AssociatedControl ) );
                }
                return false;
            }

        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties


        #endregion

        #region Construction and Finalization
        public ContactNamePreferred()
        {
        }
        #endregion

        #region Data Elements
                

        #endregion

        #region Constants
        #endregion
    }
}
