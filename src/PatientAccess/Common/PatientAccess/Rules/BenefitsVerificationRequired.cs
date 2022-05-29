using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for BenefitsVerificationRequired.
    /// </summary>
    //TODO: Create XML summary comment for BenefitsVerificationRequired
    [Serializable]
    [UsedImplicitly]
    public class BenefitsVerificationRequired : InsuranceVerificationRule
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
            if( context == null || 
                context.GetType() != typeof( Account ) )
            {                
                return true;
            } 	            

            bool preReqsApply = base.CanBeAppliedTo(context);
            if(!preReqsApply)
            {
                return true;
            }

            Account anAccount = context as Account;
            Coverage coverage = anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID);
            if(coverage != null)
            {
                if(coverage.BenefitsVerified == null
                    || coverage.BenefitsVerified.IsBlank )
                {
                    return false ;
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
        public BenefitsVerificationRequired()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}

