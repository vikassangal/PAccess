using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MedicarePartBCoveragePreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class MedicarePartBCoveragePreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler MedicarePartBCoveragePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            MedicarePartBCoveragePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            MedicarePartBCoveragePreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.MedicarePartBCoveragePreferredEvent = null;   
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
            if( context == null || context.GetType() != typeof( GovernmentMedicareCoverage ) )
            {
                return true;
            }
            GovernmentMedicareCoverage aCoverage = context as GovernmentMedicareCoverage;
            if( aCoverage == null )
            {
                return false;
            }
            if( aCoverage.PartBCoverage == null
                || aCoverage.PartBCoverage.Code == YesNoFlag.CODE_BLANK )
            {
                if( this.FireEvents && MedicarePartBCoveragePreferredEvent != null )
                {
                    MedicarePartBCoveragePreferredEvent( this, null );
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
        public MedicarePartBCoveragePreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
