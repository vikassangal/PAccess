using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ConditionCode_7 Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidConditionCode_7 : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidConditionCode_7Event;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidConditionCode_7Event += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidConditionCode_7Event -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidConditionCode_7Event = null;   
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

            if( ( account != null ) &&  (account.ConditionCodes  != null )
                &&  (account.ConditionCodes.Count > 6))
            {
                ConditionCode cc =  (ConditionCode)account.ConditionCodes[6] ;
                if(!(cc.IsValid))
                          
                {
                    if( this.FireEvents && InvalidConditionCode_7Event != null )
                    {
                        this.InvalidConditionCode_7Event(this, null);
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

        public InvalidConditionCode_7()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}

