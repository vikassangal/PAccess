using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for SecondaryPlanIDRequired.
    /// </summary>
    //TODO: Create XML summary comment for SecondaryPlanIDRequired
    [Serializable]
    [UsedImplicitly]
    public class SecondaryPlanIDRequired : InsuranceVerificationRule
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
            Account anAccount = context as Account;
            Condition condition = anAccount.Diagnosis.Condition;
            if ( condition.GetType() == typeof( Accident ) )
            {
                Accident accident = (Accident)condition;
                if ( accident.Kind.Oid == TypeOfAccident.EMPLOYMENT_RELATED )
                {
                    Coverage coverage = anAccount.Insurance.CoverageFor( CoverageOrder.SECONDARY_OID );
                    if ( coverage.InsurancePlan.GetType() == typeof( WorkersCompensationInsurancePlan ) )
                    {
                        return true;
                    }
                }

            }

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
        public SecondaryPlanIDRequired()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}

