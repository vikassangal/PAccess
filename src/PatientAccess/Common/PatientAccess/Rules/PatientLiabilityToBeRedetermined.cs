using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PatientLiabilityToBeRedetermined.
    /// </summary>
    //TODO: Create XML summary comment for PatientLiabilityToBeRedetermined
    [Serializable]
    [UsedImplicitly]
    public class PatientLiabilityToBeRedetermined : PatientLiabilityRule
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
        public override bool ShouldStopProcessing()
        {
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            
        }

        public override void ApplyTo( object context )
        {
        }

        public override bool CanBeAppliedTo( object context )
        {
            bool preReqsApply = base.CanBeAppliedTo(context);
            if(preReqsApply)
            {
                Account anAccount = context as Account;

                Coverage coverage = null;

                if( anAccount != null
                    && anAccount.Insurance != null)
                {
                    coverage =  anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID);
                }                
                bool isInsured = true;
                bool isUnInsured = true;    
           
                if( anAccount.FinancialClass != null)
                {
                    isInsured = IsInsured(anAccount.FinancialClass);
                    isUnInsured = IsUninsured(anAccount.FinancialClass);
                }
                          
                if( isInsured )
                {
                    if( coverage != null
                        && !coverage.NoLiability
                        && ( coverage.Deductible + coverage.CoPay == 0 )
                        && ( anAccount.TotalPaid > 0 ||
                           anAccount.NumberOfMonthlyPayments > 0 )
                        )
                    {
                        return false;
                    }
                }
                else if( isUnInsured )
                {
                    if( coverage != null
                        && !anAccount.Insurance.HasNoLiability
                        && ( anAccount.TotalCurrentAmtDue == 0)
                        && ( anAccount.TotalPaid > 0 ||
                        anAccount.NumberOfMonthlyPayments > 0 )
                        )
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
        public PatientLiabilityToBeRedetermined()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}

