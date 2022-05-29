using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MedicaidPatientHasMedicarePreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class MedicaidPatientHasMedicarePreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler MedicaidPatientHasMedicarePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            MedicaidPatientHasMedicarePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            MedicaidPatientHasMedicarePreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.MedicaidPatientHasMedicarePreferredEvent = null;   
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
            if( context == null || context.GetType() != typeof( GovernmentMedicaidCoverage ) )
            {
                return true;
            }
            GovernmentMedicaidCoverage aCoverage = context as GovernmentMedicaidCoverage;
            if( aCoverage == null )
            {
                return false;
            }

            if( aCoverage.PatienthasMedicare == null
                || aCoverage.PatienthasMedicare.Code == YesNoFlag.CODE_BLANK )
            {
                if( this.FireEvents && MedicaidPatientHasMedicarePreferredEvent != null )
                {
                    MedicaidPatientHasMedicarePreferredEvent( this, null );
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
        public MedicaidPatientHasMedicarePreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
