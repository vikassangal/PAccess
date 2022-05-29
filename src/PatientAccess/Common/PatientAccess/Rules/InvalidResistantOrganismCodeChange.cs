using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ResistantOrganism Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidResistantOrganismCodeChange : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidResistantOrganismCodeChangeEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidResistantOrganismCodeChangeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidResistantOrganismCodeChangeEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidResistantOrganismCodeChangeEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }              
            Account  account = (Account)context ;
                                    
            if( account.DischargeDate != DateTime.MinValue )
            {
                return true;
            }

            if( ( account  != null ) &&  (account.ResistantOrganism  != null ) )
            {
                if(!(account.ResistantOrganism.IsValid))                          
                {
                    if( this.FireEvents && InvalidResistantOrganismCodeChangeEvent != null )
                    {
                        this.InvalidResistantOrganismCodeChangeEvent(this, null);
                    }
                
                    return false ;
                }
                else
                {
                    return true;
                }           
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

        public InvalidResistantOrganismCodeChange()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}





