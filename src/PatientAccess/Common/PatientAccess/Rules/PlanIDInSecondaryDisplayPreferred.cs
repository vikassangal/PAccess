using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PlanIDInSecondaryDisplayPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PlanIDInSecondaryDisplayPreferred : LeafRule
    {
        #region Events

        public event EventHandler PlanIDInSecondaryDisplayPreferredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.PlanIDInSecondaryDisplayPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.PlanIDInSecondaryDisplayPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PlanIDInSecondaryDisplayPreferredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || ! ( context is Account ) )
            {
                return true;
            }              
            
            Account anAccount = (Account)context;

            if( anAccount.Activity is PreRegistrationActivity &&
                HasNoPlanID( anAccount ) )
            {
                // TLG 10/2/2007 only return true if a handler is registered

                if( this.FireEvents && PlanIDInSecondaryDisplayPreferredEvent != null )
                {
                    this.PlanIDInSecondaryDisplayPreferredEvent( this, null );
                    return false;
                }
                else
                {
                    return true;
                }                            
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

        private bool HasNoPlanID( Account anAccount )
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
        
            return !hasSecondary? true : false;        
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public PlanIDInSecondaryDisplayPreferred()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
