using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.PAIWalkinOutpatientCreation;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Domain.UCCRegistration;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PlanIDInPrimaryDisplayRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PlanIDInPrimaryDisplayRequired : LeafRule
    {
        #region Events

        public event EventHandler PlanIDInPrimaryDisplayRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.PlanIDInPrimaryDisplayRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.PlanIDInPrimaryDisplayRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PlanIDInPrimaryDisplayRequiredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || ! ( context is Account ) )
            {
                return true;
            }              
            
            Account anAccount = (Account)context;

            if( ( anAccount.Activity is MaintenanceActivity         ||
                  anAccount.Activity is RegistrationActivity        ||
                  anAccount.Activity is PreRegistrationActivity     ||
                  anAccount.Activity is ShortPreRegistrationActivity ||
                  anAccount.Activity is ShortRegistrationActivity ||
                   anAccount.Activity is ShortMaintenanceActivity ||
                  anAccount.Activity is PostMSERegistrationActivity ||
                  anAccount.Activity is UCCPostMseRegistrationActivity ||
                  anAccount.Activity is AdmitNewbornActivity        ||
                  anAccount.Activity is PreAdmitNewbornActivity ||
                  anAccount.Activity is TransferOutToInActivity ||
                   anAccount.Activity is TransferERToOutpatientActivity ||
                    anAccount.Activity is TransferOutpatientToERActivity ||
                    anAccount.Activity is PAIWalkinOutpatientCreationActivity) &&
                 HasNoPlanID( anAccount ) )
            {
                // TLG 10/2/2007 only return true if a handler is registered

                if( this.FireEvents && PlanIDInPrimaryDisplayRequiredEvent != null )
                {
                    this.PlanIDInPrimaryDisplayRequiredEvent( this, null );
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

        public PlanIDInPrimaryDisplayRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
