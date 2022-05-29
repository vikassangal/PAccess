using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Rule for address State
    /// </summary>
	
    [Serializable]
    [UsedImplicitly]
    public class AddressStateRequired : AddressFieldsRequired
    {
        #region Events
        
        public event EventHandler AddressStateRequiredEvent;
        
        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.AddressStateRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.AddressStateRequiredEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.AddressStateRequiredEvent = null;  
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || 
                context.GetType() != typeof(Address) )
            {
                return true;
            }

            if( (((Address)context).State == null
                || ((Address)context).State.Description == null
                || ((Address)context).State.Description.Trim().Length == 0 )
                    && 
                (((Address)context).Country != null
                && ((Address)context).Country.Code != null
                && ((Address)context).Country.Code == Country.USA_CODE ))
            {
                if( this.FireEvents && AddressStateRequiredEvent != null )
                {
                    this.AddressStateRequiredEvent(this, null);
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
        public AddressStateRequired()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
