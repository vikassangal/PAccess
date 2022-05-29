using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ReviewGuarantorCreditReport.
    /// </summary>
    //TODO: Create XML summary comment for ReviewGuarantorCreditReport
    [Serializable]
    [UsedImplicitly]
    public class ReviewGuarantorCreditReport : PatientLiabilityRule
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
                Guarantor guarantor = anAccount.Guarantor;
                if(guarantor != null)
                {
                    if( (guarantor.DataValidationTicket != null) 
                        && (guarantor.DataValidationTicket.ResultsAvailable) 
                        && !(guarantor.DataValidationTicket.ResultsReviewed)
                        )
                    {
                        return false ;
                    }
                }
            }
            return true ;
        }
        #endregion

        #region Properties
      
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ReviewGuarantorCreditReport()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}

