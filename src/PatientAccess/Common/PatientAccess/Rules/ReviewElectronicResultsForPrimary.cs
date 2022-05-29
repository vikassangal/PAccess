using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ReviewElectronicResultsForPrimary.
    /// </summary>
    //TODO: Create XML summary comment for ReviewElectronicResultsForPrimary
    [Serializable]
    [UsedImplicitly]
    public class ReviewElectronicResultsForPrimary : InsuranceVerificationRule
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
            Account anAccount = context as Account;
            Coverage coverage = anAccount.Insurance.CoverageFor( CoverageOrder.PRIMARY_OID );
            if( coverage != null && coverage.DataValidationTicket != null )
            {
                if( ( coverage.DataValidationTicket.BenefitsResponse != null &&
                        coverage.DataValidationTicket.BenefitsResponse.ResponseStatus != null &&
                        coverage.DataValidationTicket.BenefitsResponse.ResponseStatus.Oid != BenefitResponseStatus.ACCEPTED_OID &&
                        coverage.DataValidationTicket.BenefitsResponse.ResponseStatus.Oid != BenefitResponseStatus.AUTO_ACCEPTED_OID &&
                        coverage.DataValidationTicket.BenefitsResponse.ResponseStatus.Oid != BenefitResponseStatus.REJECTED_OID )
                     && ( coverage.DataValidationTicket != null )
                    && ( coverage.DataValidationTicket.ResultsAvailable )
                    )
                {
                    return false;
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
        public ReviewElectronicResultsForPrimary()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}

