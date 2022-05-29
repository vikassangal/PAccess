using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PlanNameInSecondaryDisplayRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PlanNameInSecondaryDisplayRequired : LeafRule
    {
        #region Events

        public event EventHandler PlanNameInSecondaryDisplayRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.PlanNameInSecondaryDisplayRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.PlanNameInSecondaryDisplayRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PlanNameInSecondaryDisplayRequiredEvent = null;    
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }  
            
            Account anAccount = (Account)context;

            if( ( anAccount.Activity.GetType().Equals( typeof( RegistrationActivity ) ) ||
                   anAccount.Activity is ShortPreRegistrationActivity ||
                   anAccount.Activity is ShortRegistrationActivity ||
                anAccount.Activity.GetType().Equals( typeof( AdmitNewbornActivity ) ) ) &&
                HasNoPlanName( anAccount ) )
            {
                if( this.FireEvents && PlanNameInSecondaryDisplayRequiredEvent != null )
                {
                    this.PlanNameInSecondaryDisplayRequiredEvent(this, null);
                }
            
                return false;
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
        private bool HasNoPlanName( Account anAccount )
        {
            if( anAccount.Insurance.Coverages.Count == 0 )
            {
                return true;
            }

            bool hasSecondary = false;
            foreach( Coverage coverage in anAccount.Insurance.Coverages )
            {
                if( coverage.CoverageOrder.Oid.Equals( CoverageOrder.SECONDARY_OID ) )
                {
                    hasSecondary = true;
                    if( coverage.InsurancePlan.PlanName == string.Empty )
                    {
                        return true;
                    }
                    else
                    {
                        return false;                       
                    }                
                }
            }    
        
            return !hasSecondary? true : false;        
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public PlanNameInSecondaryDisplayRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
