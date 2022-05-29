using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for InvalidGenderForPriInsured Required.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidGenderForPriInsured : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidGenderForPriInsuredEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidGenderForPriInsuredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidGenderForPriInsuredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidGenderForPriInsuredEvent = null;   
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
                Coverage coverage = account.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID);
                if((coverage != null) && (coverage.Insured != null) && (coverage.Insured.Sex != null) )
                {                   
                    if(!(coverage.Insured.Sex.IsValid))
                          
                    {
                        if( this.FireEvents && InvalidGenderForPriInsuredEvent != null )
                        {
                            this.InvalidGenderForPriInsuredEvent(this, null);
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

        public InvalidGenderForPriInsured()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}

