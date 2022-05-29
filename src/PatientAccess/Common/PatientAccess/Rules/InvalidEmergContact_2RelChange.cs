using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for InvalidEmergContact_2RelChange Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidEmergContact_2RelChange : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidEmergContact_2RelChangeEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            if( InvalidEmergContact_2RelChangeEvent != null )
            {
                Delegate[] delegates = this.InvalidEmergContact_2RelChangeEvent.GetInvocationList();

                foreach( Delegate d in delegates )
                {
                    if( d.Target.GetType() == eventHandler.Target.GetType()
                        && d.Method.Name == eventHandler.Method.Name)
                    {
                        return true;
                    }
                }
            }
            this.InvalidEmergContact_2RelChangeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidEmergContact_2RelChangeEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidEmergContact_2RelChangeEvent = null;   
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

            if( ( account != null ) &&  (account.EmergencyContact1  != null )
                &&(account.EmergencyContact2.RelationshipType != null))
            {
               
                if(!(account.EmergencyContact2.RelationshipType.IsValid))
                          
                {
                    if( this.FireEvents && InvalidEmergContact_2RelChangeEvent != null )
                    {
                        this.InvalidEmergContact_2RelChangeEvent(this, null);
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

        public InvalidEmergContact_2RelChange()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}


