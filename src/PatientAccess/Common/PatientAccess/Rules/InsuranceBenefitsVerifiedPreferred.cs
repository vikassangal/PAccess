using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for InsuranceBenefitsVerifiedPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InsuranceBenefitsVerifiedPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler InsuranceBenefitsVerifiedPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            InsuranceBenefitsVerifiedPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            InsuranceBenefitsVerifiedPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InsuranceBenefitsVerifiedPreferredEvent = null;   
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
            bool result = true;

            if( context == null || 
                context.GetType() != typeof( Coverage ) &&
                context.GetType().BaseType != typeof ( Coverage ) &&
                context.GetType().BaseType.BaseType != typeof ( Coverage ) &&
                context.GetType().BaseType.BaseType.BaseType != typeof ( Coverage ) &&
                context.GetType().BaseType.BaseType.BaseType.BaseType != typeof ( Coverage )
                )
            {
                return true;
            }  

            Coverage aCoverage = context as Coverage;

            if( aCoverage.BenefitsVerified == null
                || aCoverage.BenefitsVerified.IsBlank )
            {
                if( this.FireEvents && InsuranceBenefitsVerifiedPreferredEvent != null )
                {
                    InsuranceBenefitsVerifiedPreferredEvent( this, null );
                }
                result = false;
            }

            return result;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public InsuranceBenefitsVerifiedPreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
