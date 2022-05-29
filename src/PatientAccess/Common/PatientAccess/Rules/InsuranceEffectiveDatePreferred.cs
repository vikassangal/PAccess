using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for InsuranceEffectiveDatePreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InsuranceEffectiveDatePreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler InsuranceEffectiveDatePreferredPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            InsuranceEffectiveDatePreferredPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            InsuranceEffectiveDatePreferredPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InsuranceEffectiveDatePreferredPreferredEvent = null;   
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
            CoverageForCommercialOther aCoverage = context as CoverageForCommercialOther;
            if( aCoverage == null )
            {
                return false;
            }
            if( aCoverage.EffectiveDateForInsured == DateTime.MinValue )
            {
                if( this.FireEvents && InsuranceEffectiveDatePreferredPreferredEvent != null )
                {
                    InsuranceEffectiveDatePreferredPreferredEvent( this, null );
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
        public InsuranceEffectiveDatePreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
