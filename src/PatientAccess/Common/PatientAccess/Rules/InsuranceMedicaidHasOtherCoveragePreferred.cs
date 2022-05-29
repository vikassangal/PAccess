using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for InsuranceMedicaidHasOtherCoveragePreferred.
	/// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InsuranceMedicaidHasOtherCoveragePreferred : LeafRule
	{
        #region Event Handlers
        public event EventHandler InsuranceMedicaidHasOtherCoveragePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            InsuranceMedicaidHasOtherCoveragePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            InsuranceMedicaidHasOtherCoveragePreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InsuranceMedicaidHasOtherCoveragePreferredEvent    = null;   
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
            if( context.GetType() != typeof( GovernmentMedicaidCoverage ) )
            {
                return true;
            }
            GovernmentMedicaidCoverage aCoverage = context as GovernmentMedicaidCoverage;
            if( aCoverage == null )
            {
                return false;
            }
            if( aCoverage.PatienthasOtherInsuranceCoverage == null 
                || aCoverage.PatienthasOtherInsuranceCoverage.Code == YesNoFlag.CODE_BLANK )
            {
                if( this.FireEvents && InsuranceMedicaidHasOtherCoveragePreferredEvent != null )
                {
                    InsuranceMedicaidHasOtherCoveragePreferredEvent( this, null );
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
        public InsuranceMedicaidHasOtherCoveragePreferred()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
