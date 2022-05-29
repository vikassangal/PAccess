using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Rule for address Street
    /// </summary>
	
    [Serializable]
    [UsedImplicitly]
    public class AddressStreetRequired : AddressFieldsRequired
    {
        #region Events
        
        public event EventHandler AddressStreetRequiredEvent;
        
        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.AddressStreetRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.AddressStreetRequiredEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.AddressStreetRequiredEvent = null;  
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || 
                context.GetType() != typeof(Address) )
            {
                return true;
            }

            if( ((Address)context).Address1 == null
                || ((Address)context).Address1.Trim().Length == 0)
            {
                if( this.FireEvents && AddressStreetRequiredEvent != null )
                {
                    this.AddressStreetRequiredEvent(this, new PropertyChangedArgs(this.AssociatedControl));
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
            return true;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AddressStreetRequired()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
