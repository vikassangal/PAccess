using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for BenVerPolicyNumber.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class BenVerPolicyNumber : LeafRule
    {
        #region Event Handlers
        public event EventHandler BenVerPolicyNumberEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            BenVerPolicyNumberEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            BenVerPolicyNumberEvent -= eventHandler;
            return true;
        }
                                
        public override void UnregisterHandlers()
        {
            this.BenVerPolicyNumberEvent = null;  
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
                context.GetType() != typeof( WorkersCompensationCoverage ) )
            {                
                return true;
            } 	                   

            if( this.AssociatedControl == null
                || ((InsurancePlanCategory)AssociatedControl).Oid != InsurancePlanCategory.PLANCATEGORY_WORKERS_COMPENSATION )
            {
                return true;
            }

            WorkersCompensationCoverage aCoverage = context as WorkersCompensationCoverage;

            if( aCoverage != null && aCoverage.PolicyNumber != String.Empty )
            {
                return true;
            }
            else
            {
                if( this.FireEvents && BenVerPolicyNumberEvent != null )
                {
                    BenVerPolicyNumberEvent( this, null );
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
        public BenVerPolicyNumber()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
