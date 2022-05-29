using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.QuickAccountCreation;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PlanIDInPrimaryDisplayPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PlanIDInPrimaryDisplayPreferred : LeafRule
    {
        #region Events

        public event EventHandler PlanIDInPrimaryDisplayPreferredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.PlanIDInPrimaryDisplayPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.PlanIDInPrimaryDisplayPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PlanIDInPrimaryDisplayPreferredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || !( context is Account ) )
            {
                return true;
            }                 
            
            Account anAccount = (Account)context;

            if (IsValidActivity(anAccount.Activity) &&
                HasNoPlanID( anAccount ) )
            {
                // TLG 10/2/2007 only return true if a handler is registered

                if( this.FireEvents && PlanIDInPrimaryDisplayPreferredEvent != null )
                {
                    this.PlanIDInPrimaryDisplayPreferredEvent( this, null );
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
        private bool IsValidActivity(Activity activity)
        {
            return (activity.GetType().Equals(typeof(PreRegistrationActivity)) ||
                    activity.GetType().Equals(typeof(QuickAccountCreationActivity)) ||
                      activity.GetType().Equals(typeof(QuickAccountMaintenanceActivity))
                      );
        }
        private bool HasNoPlanID( Account anAccount )
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
                    if( coverage.InsurancePlan.PlanID == string.Empty )
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

        public PlanIDInPrimaryDisplayPreferred()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
