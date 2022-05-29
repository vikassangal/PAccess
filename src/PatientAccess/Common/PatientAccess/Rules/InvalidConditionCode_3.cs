using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ConditionCode_3 Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidConditionCode_3 : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidConditionCode_3Event;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidConditionCode_3Event += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidConditionCode_3Event -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidConditionCode_3Event = null;   
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
                &&  (account.ConditionCodes.Count > 2   ))
            {
                ConditionCode cc =  (ConditionCode)account.ConditionCodes[2] ;
                if(!(cc.IsValid))
                          
                {
                    if( this.FireEvents && InvalidConditionCode_3Event != null )
                    {
                        this.InvalidConditionCode_3Event(this, null);
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

        public InvalidConditionCode_3()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}


