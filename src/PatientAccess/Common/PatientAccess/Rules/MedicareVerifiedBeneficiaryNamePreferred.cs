using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MedicareVerifiedBeneficiaryNamePreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class MedicareVerifiedBeneficiaryNamePreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler MedicareVerifiedBeneficiaryNamePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            MedicareVerifiedBeneficiaryNamePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            MedicareVerifiedBeneficiaryNamePreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.MedicareVerifiedBeneficiaryNamePreferredEvent = null;   
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

            if( aCoverage.VerifiedBeneficiaryName == null
                || aCoverage.VerifiedBeneficiaryName.Code == YesNoFlag.CODE_BLANK )
            {
                if( this.FireEvents && MedicareVerifiedBeneficiaryNamePreferredEvent != null )
                {
                    MedicareVerifiedBeneficiaryNamePreferredEvent( this, null );
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
        public MedicareVerifiedBeneficiaryNamePreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
