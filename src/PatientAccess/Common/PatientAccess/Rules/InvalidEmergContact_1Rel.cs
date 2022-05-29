using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for InvalidEmergContact_1Rel Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidEmergContact_1Rel : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidEmergContact_1RelEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidEmergContact_1RelEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidEmergContact_1RelEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidEmergContact_1RelEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }              
            Account  account = (Account)context ;
                                    
            if( account.DischargeDate == DateTime.MinValue )
            {
                return true;
            }

            if( ( account != null ) &&  (account.EmergencyContact1  != null )
                &&(account.EmergencyContact1.RelationshipType != null))
            {
               
                if(!(account.EmergencyContact1.RelationshipType.IsValid))                          
                {
                    if( this.FireEvents && InvalidEmergContact_1RelEvent != null )
                    {
                        this.InvalidEmergContact_1RelEvent(this, null);
                    }

                    return false;
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

        public InvalidEmergContact_1Rel()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}


