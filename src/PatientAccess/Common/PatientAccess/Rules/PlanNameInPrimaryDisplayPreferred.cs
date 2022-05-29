using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.QuickAccountCreation;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PlanNameInPrimaryDisplayPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PlanNameInPrimaryDisplayPreferred : LeafRule
    {
        #region Events

        public event EventHandler PlanNameInPrimaryDisplayPreferredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.PlanNameInPrimaryDisplayPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.PlanNameInPrimaryDisplayPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PlanNameInPrimaryDisplayPreferredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }  
           
            Account anAccount = (Account)context;

            if (IsValidActivity( anAccount.Activity ) &&
                HasNoPlanName( anAccount ) )
            {
                if( this.FireEvents && PlanNameInPrimaryDisplayPreferredEvent != null )
                {
                    this.PlanNameInPrimaryDisplayPreferredEvent(this, null);
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
        private bool IsValidActivity( Activity activity )
        {
            return (activity.GetType().Equals(typeof (PreRegistrationActivity)) ||
                    activity.GetType().Equals(typeof (QuickAccountCreationActivity)) ||
                      activity.GetType().Equals(typeof(QuickAccountMaintenanceActivity)) 
                      );
        }
        private bool HasNoPlanName( Account anAccount )
        {
            if( anAccount.Insurance.Coverages.Count == 0 )
            {
                return true;
            }

            bool hasPrimary = false;
            foreach( Coverage coverage in anAccount.Insurance.Coverages )
            {
                if( coverage.CoverageOrder.Oid.Equals( CoverageOrder.PRIMARY_OID ) )
                {
                    hasPrimary = true;
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
        
            return !hasPrimary? true : false;        
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public PlanNameInPrimaryDisplayPreferred()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
