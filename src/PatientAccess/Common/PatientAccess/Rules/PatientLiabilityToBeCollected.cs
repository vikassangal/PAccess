using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PatientLiabilityToBeCollected.
    /// </summary>
    //TODO: Create XML summary comment for PatientLiabilityToBeCollected
    [Serializable]
    [UsedImplicitly]
    public class PatientLiabilityToBeCollected : PatientLiabilityRule
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }             
            
            bool preReqsApply = base.CanBeAppliedTo(context);
            if(preReqsApply)
            {
                Account anAccount = context as Account;
                Coverage coverage = null;
                bool   noLiability   =  false;
                decimal amtDue = 0;
                if( anAccount != null
                    && anAccount.Insurance != null)
                {
                    coverage = anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID);
                }
                
                if( coverage != null )
                {
                    noLiability = coverage.NoLiability;
                    amtDue =  coverage.Deductible + coverage.CoPay ;
                }
                if( anAccount.FinancialClass != null
                    && IsInsured(anAccount.FinancialClass) )
                {
                    if( anAccount.TotalCurrentAmtDue > 0 
                        && anAccount.TotalPaid < anAccount.TotalCurrentAmtDue
                        && anAccount.NumberOfMonthlyPayments == 0 ) 
                    {
                        return false ;
                    }
                   
                }
                else
                    if( anAccount.FinancialClass != null
                    && IsUninsured(anAccount.FinancialClass) )
                {
                    if( anAccount.TotalCurrentAmtDue > 0
                        && anAccount.TotalPaid < anAccount.TotalCurrentAmtDue 
                        && anAccount.NumberOfMonthlyPayments == 0 )
                    {
                        return false ;
                    }
                   
                }
             
                         
            }
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
        public PatientLiabilityToBeCollected()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}

