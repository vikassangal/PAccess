using System;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PlanIDRequired.
    /// </summary>
    //TODO: Create XML summary comment for PlanIDRequired
    [Serializable]
    [UsedImplicitly]
    public class PlanIDRequired : InsuranceVerificationRule
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
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }              
            
            bool preReqsApply = base.CanBeAppliedTo(context);
            if(!preReqsApply)
            {
                return true ;
            }
            Account anAccount = context as Account;
            Condition condition = anAccount.Diagnosis.Condition;
            if(condition.GetType() == typeof(Accident))
            {
                Accident accident = (Accident)condition;

                if(accident.Kind != null
                    && accident.Kind.Oid == TypeOfAccident.EMPLOYMENT_RELATED)
                {
                    Coverage coverage = anAccount.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID);
                    if( coverage != null)
                    {
                        if(coverage.InsurancePlan != null
                            && coverage.InsurancePlan.GetType()!= typeof(WorkersCompensationInsurancePlan))
                        {
                            return false ;
                        }
                    }
                }

            }
                
            //            }
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
        public PlanIDRequired()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}

