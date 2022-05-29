using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ConditionCode_5 Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidConditionCode_5 : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidConditionCode_5Event;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidConditionCode_5Event += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidConditionCode_5Event -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidConditionCode_5Event = null;   
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
                &&  (account.ConditionCodes.Count > 4 ))
            {
                ConditionCode cc =  (ConditionCode)account.ConditionCodes[4] ;
                if(!(cc.IsValid))
                          
                {
                    if( this.FireEvents && InvalidConditionCode_5Event != null )
                    {
                        this.InvalidConditionCode_5Event(this, null);
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

        public InvalidConditionCode_5()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}


