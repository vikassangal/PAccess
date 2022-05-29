using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MedicareHospiceProgramPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class MedicareHospiceProgramPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler MedicareHospiceProgramPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            MedicareHospiceProgramPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            MedicareHospiceProgramPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.MedicareHospiceProgramPreferredEvent = null;   
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
            
            if( aCoverage.PatientIsPartOfHospiceProgram == null
                || aCoverage.PatientIsPartOfHospiceProgram.Code == YesNoFlag.CODE_BLANK )
            {
                if( this.FireEvents && MedicareHospiceProgramPreferredEvent != null )
                {
                    MedicareHospiceProgramPreferredEvent( this, null );
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
        public MedicareHospiceProgramPreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
