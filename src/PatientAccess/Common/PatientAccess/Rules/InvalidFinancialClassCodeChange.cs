using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for FinancialClass Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidFinancialClassCodeChange : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidFinancialClassCodeChangeEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidFinancialClassCodeChangeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidFinancialClassCodeChangeEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidFinancialClassCodeChangeEvent = null;   
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

            if( ( account  != null ) &&  (account.FinancialClass  != null ) )
            {
                if(!(account.FinancialClass.IsValid))                          
                {
                    if( this.FireEvents && InvalidFinancialClassCodeChangeEvent != null )
                    {
                        this.InvalidFinancialClassCodeChangeEvent(this, null);
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

        public InvalidFinancialClassCodeChange()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}





