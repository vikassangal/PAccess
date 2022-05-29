using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for BenVerPolicyCIN.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class BenVerPolicyCIN : LeafRule
    {
        #region Event Handlers
        public event EventHandler BenVerPolicyCINEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            BenVerPolicyCINEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            BenVerPolicyCINEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.BenVerPolicyCINEvent = null;  
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        public override bool CanBeAppliedTo( object context )
        {
            if( context == null || 
                context.GetType() != typeof( GovernmentMedicaidCoverage ) )
            {                
                return true;
            } 	                        

            if( this.AssociatedControl == null
                || ((InsurancePlanCategory)AssociatedControl).Oid != InsurancePlanCategory.PLANCATEGORY_GOVERNMENT_MEDICAID )
            {
                return true;
            }

            GovernmentMedicaidCoverage aCoverage = context as GovernmentMedicaidCoverage;

            if( aCoverage != null && aCoverage.PolicyCINNumber != String.Empty )
            {
                return true;
            }
            else
            {
                if( this.FireEvents && BenVerPolicyCINEvent != null )
                {
                    BenVerPolicyCINEvent( this, null );
                }
                return false;
            }
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public BenVerPolicyCIN()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
