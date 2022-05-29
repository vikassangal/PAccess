using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for MedicareHasHMOCoveragePreferred.
	/// </summary>
	[Serializable]
    [UsedImplicitly]
    public class MedicareHasHMOCoveragePreferred : LeafRule
	{
        #region Event Handlers
        public event EventHandler MedicareHasHMOCoveragePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            MedicareHasHMOCoveragePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            MedicareHasHMOCoveragePreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.MedicareHasHMOCoveragePreferredEvent = null;   
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
            if( aCoverage.PatientHasMedicareHMOCoverage == null 
                || aCoverage.PatientHasMedicareHMOCoverage.Code == YesNoFlag.CODE_BLANK )
            {
                if( this.FireEvents && MedicareHasHMOCoveragePreferredEvent != null )
                {
                    MedicareHasHMOCoveragePreferredEvent( this, null );
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
        public MedicareHasHMOCoveragePreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
