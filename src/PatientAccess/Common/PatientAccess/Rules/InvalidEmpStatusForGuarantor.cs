using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for InvalidEmpStatusForGuarantor Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidEmpStatusForGuarantor : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidEmpStatusForGuarantorEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidEmpStatusForGuarantorEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidEmpStatusForGuarantorEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidEmpStatusForGuarantorEvent = null;   
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

            if( ( account.Guarantor != null ) &&  (account.Guarantor.Employment  != null )
                &&  (account.Guarantor.Employment.Status  != null ))
            {
                if(!(account.Guarantor.Employment.Status.IsValid))
                          
                {
                    if( this.FireEvents && InvalidEmpStatusForGuarantorEvent != null )
                    {
                        this.InvalidEmpStatusForGuarantorEvent(this, null);
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

        public InvalidEmpStatusForGuarantor()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}

