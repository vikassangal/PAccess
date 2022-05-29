using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PlanNameInSecondaryDisplayPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PlanNameInSecondaryDisplayPreferred : LeafRule
    {
        #region Events

        public event EventHandler PlanNameInSecondaryDisplayPreferredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.PlanNameInSecondaryDisplayPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.PlanNameInSecondaryDisplayPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PlanNameInSecondaryDisplayPreferredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }              
            Account anAccount = (Account)context;

            if( anAccount.Activity.GetType().Equals( typeof( PreRegistrationActivity ) ) &&
                HasNoPlanName( anAccount ) )
            {
                if( this.FireEvents && PlanNameInSecondaryDisplayPreferredEvent != null )
                {
                    this.PlanNameInSecondaryDisplayPreferredEvent(this, null);
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

        public PlanNameInSecondaryDisplayPreferred()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
