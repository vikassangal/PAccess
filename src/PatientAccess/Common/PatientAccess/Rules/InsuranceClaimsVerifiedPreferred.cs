using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for InsuranceClaimsVerifiedPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InsuranceClaimsVerifiedPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler InsuranceClaimsVerifiedPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            InsuranceClaimsVerifiedPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            InsuranceClaimsVerifiedPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InsuranceClaimsVerifiedPreferredEvent = null;   
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
                context.GetType() != typeof( Coverage ) &&
                context.GetType().BaseType != typeof ( Coverage ) &&
                context.GetType().BaseType.BaseType != typeof ( Coverage ) &&
                context.GetType().BaseType.BaseType.BaseType != typeof ( Coverage ) &&
                context.GetType().BaseType.BaseType.BaseType.BaseType != typeof ( Coverage )
                )
            {
                return true;
            }
            CommercialCoverage aCoverage = context as CommercialCoverage;
            if( aCoverage == null )
            {
                return false;
            }
            if( aCoverage.ClaimsAddressVerified == null 
                || aCoverage.ClaimsAddressVerified.Code == YesNoFlag.CODE_BLANK)
            {
                if( this.FireEvents && InsuranceClaimsVerifiedPreferredEvent != null )
                {
                    InsuranceClaimsVerifiedPreferredEvent( this, null );
                }
                return false;
            }
            return true;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public InsuranceClaimsVerifiedPreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
