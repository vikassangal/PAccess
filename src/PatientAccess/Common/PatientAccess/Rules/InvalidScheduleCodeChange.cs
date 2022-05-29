using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for InvalidScheduleCodeChange.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidScheduleCodeChange : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidScheduleCodeChangeEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidScheduleCodeChangeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidScheduleCodeChangeEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidScheduleCodeChangeEvent = null;   
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

            if( ( account != null ) &&  (account.ScheduleCode  != null ))
            {
                if ((account.Activity is AdmitNewbornActivity) ||
                    (account.Activity is PostMSERegistrationActivity))
                {
                    return true ;
                }

                if(!(account.ScheduleCode.IsValid))
                          
                {
                    if( this.FireEvents && InvalidScheduleCodeChangeEvent != null )
                    {
                        this.InvalidScheduleCodeChangeEvent(this, null);
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

        public InvalidScheduleCodeChange()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}


