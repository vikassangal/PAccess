using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MedicaidPolicyCINNumberRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class MedicaidPolicyCINNumberRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler MedicaidPolicyCINNumberRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            MedicaidPolicyCINNumberRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            MedicaidPolicyCINNumberRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.MedicaidPolicyCINNumberRequiredEvent = null;   
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
            if( aCoverage.PolicyCINNumber == String.Empty )
            {
                if( this.FireEvents && MedicaidPolicyCINNumberRequiredEvent != null )
                {
                    MedicaidPolicyCINNumberRequiredEvent( this, null );
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
        public MedicaidPolicyCINNumberRequired()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
