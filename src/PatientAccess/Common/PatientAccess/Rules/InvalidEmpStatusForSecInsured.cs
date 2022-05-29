using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for InvalidEmpStatusForSecInsured Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidEmpStatusForSecInsured : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidEmpStatusForSecInsuredEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidEmpStatusForSecInsuredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidEmpStatusForSecInsuredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidEmpStatusForSecInsuredEvent = null;   
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

            if( ( account.Insurance != null ))
            {
                Coverage coverage = account.Insurance.CoverageFor(CoverageOrder.SECONDARY_OID);
                if((coverage != null) && (coverage.Insured != null) 
                    && (coverage.Insured.Employment  != null)
                     && (coverage.Insured.Employment.Status  != null))
                {                   
                    if(!(coverage.Insured.Employment.Status.IsValid))
                          
                    {
                        if( this.FireEvents && InvalidEmpStatusForSecInsuredEvent != null )
                        {
                            this.InvalidEmpStatusForSecInsuredEvent(this, null);
                        }
            
                        return false ;
                     ;
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

        public InvalidEmpStatusForSecInsured()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}

