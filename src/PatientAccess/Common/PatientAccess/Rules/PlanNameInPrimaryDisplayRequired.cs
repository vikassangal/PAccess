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
    /// Summary description for PlanNameInPrimaryDisplayRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PlanNameInPrimaryDisplayRequired : LeafRule
    {
        #region Events

        public event EventHandler PlanNameInPrimaryDisplayRequiredEvent;

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.PlanNameInPrimaryDisplayRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.PlanNameInPrimaryDisplayRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.PlanNameInPrimaryDisplayRequiredEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }  
            
            Account anAccount = (Account)context;

            if( ( anAccount.Activity.GetType() == typeof( MaintenanceActivity )  ||
                  anAccount.Activity.GetType() == typeof( RegistrationActivity )  ||
                  anAccount.Activity.GetType() == typeof( PreRegistrationActivity) ||
                  anAccount.Activity.GetType() == typeof(ShortPreRegistrationActivity) ||
                  anAccount.Activity.GetType() == typeof( ShortRegistrationActivity) ||
                  anAccount.Activity.GetType() == typeof( PostMSERegistrationActivity ) ||
                  anAccount.Activity.GetType() == typeof(UCCPostMseRegistrationActivity) ||
                  anAccount.Activity.GetType() == typeof( AdmitNewbornActivity )  ||
                  anAccount.Activity.GetType() == typeof( PreAdmitNewbornActivity ) ||
                  anAccount.Activity.GetType() == typeof( TransferOutToInActivity ) ||
                  anAccount.Activity.GetType() == typeof(TransferOutpatientToERActivity) ||
                  anAccount.Activity.GetType() == typeof(TransferERToOutpatientActivity) ||
                  anAccount.Activity.GetType() == typeof( PAIWalkinOutpatientCreationActivity )) &&
                    HasNoPlanName( anAccount ) )
            {
                if( this.FireEvents && PlanNameInPrimaryDisplayRequiredEvent != null )
                {
                    this.PlanNameInPrimaryDisplayRequiredEvent(this, null);
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

        public PlanNameInPrimaryDisplayRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
