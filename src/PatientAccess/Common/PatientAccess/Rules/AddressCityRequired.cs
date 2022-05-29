using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Rule for address City
    /// </summary>
	
    [Serializable]
    [UsedImplicitly]
    public class AddressCityRequired : AddressFieldsRequired
    {
        #region Events
        
        public event EventHandler AddressCityRequiredEvent;
        
        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.AddressCityRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.AddressCityRequiredEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.AddressCityRequiredEvent = null;  
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || 
                context.GetType() != typeof(Address) )
            {                
                return true;
            } 	                       

            if( ((Address)context).City == null
                || ((Address)context).City.Trim().Length == 0)
            {
                if( this.FireEvents && AddressCityRequiredEvent != null )
                {
                    this.AddressCityRequiredEvent(this, new PropertyChangedArgs(this.AssociatedControl));
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
        public AddressCityRequired()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
