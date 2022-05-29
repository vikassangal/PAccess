using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for Clinic_1Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidConditionCode_1Change : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidConditionCode_1ChangeEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidConditionCode_1ChangeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidConditionCode_1ChangeEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidConditionCode_1ChangeEvent = null;   
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

            if( ( account != null ) &&  (account.ConditionCodes  != null )
                &&  (account.ConditionCodes.Count > 0  ))
            {
                ConditionCode cc =  (ConditionCode)account.ConditionCodes[0] ;
                if(!(cc.IsValid))
                          
                {
                    if( this.FireEvents && InvalidConditionCode_1ChangeEvent != null )
                    {
                        this.InvalidConditionCode_1ChangeEvent(this, null);
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

        public InvalidConditionCode_1Change()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}


