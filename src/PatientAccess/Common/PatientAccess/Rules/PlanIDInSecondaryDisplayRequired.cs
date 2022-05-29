using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PlanIDInSecondaryDisplayRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PlanIDInSecondaryDisplayRequired : LeafRule
    {
        #region Events

        public event EventHandler PlanIDInSecondaryDisplayRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.PlanIDInSecondaryDisplayRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.PlanIDInSecondaryDisplayRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PlanIDInSecondaryDisplayRequiredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || ! ( context is Account ) )
            {
                return true;
            }  
            
            Account anAccount = (Account)context;

            if( ( anAccount.Activity is RegistrationActivity ||         
                   anAccount.Activity is ShortPreRegistrationActivity ||
                   anAccount.Activity is ShortRegistrationActivity ||
                  anAccount.Activity is AdmitNewbornActivity ) &&
                HasNoPlanID( anAccount ) )
            {
                // TLG 10/2/2007 only return true if a handler is registered

                if( this.FireEvents && PlanIDInSecondaryDisplayRequiredEvent != null )
                {
                    this.PlanIDInSecondaryDisplayRequiredEvent( this, null );
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

        public PlanIDInSecondaryDisplayRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
