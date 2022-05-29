using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for GovtEffectiveDatePreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class GovtEffectiveDatePreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler GovtEffectiveDatePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            GovtEffectiveDatePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            GovtEffectiveDatePreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.GovtEffectiveDatePreferredEvent = null;   
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
            if( context.GetType() != typeof( CoverageForCommercialOther )
                && context.GetType().BaseType != typeof( CoverageForCommercialOther ) 
                && context.GetType().BaseType.BaseType != typeof( CoverageForCommercialOther ) 
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
                if( this.FireEvents && GovtEffectiveDatePreferredEvent != null )
                {
                    GovtEffectiveDatePreferredEvent( this, null );
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
        public GovtEffectiveDatePreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
