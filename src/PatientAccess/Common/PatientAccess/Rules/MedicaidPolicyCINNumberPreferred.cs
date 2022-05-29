using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MedicaidPolicyCINNumberPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class MedicaidPolicyCINNumberPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler MedicaidPolicyCINNumberPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            MedicaidPolicyCINNumberPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            MedicaidPolicyCINNumberPreferredEvent -= eventHandler;
            return true;
        }
               
        public override void UnregisterHandlers()
        {
            this.MedicaidPolicyCINNumberPreferredEvent = null;            
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
                if( this.FireEvents && MedicaidPolicyCINNumberPreferredEvent != null )
                {
                    MedicaidPolicyCINNumberPreferredEvent( this, null );
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
        public MedicaidPolicyCINNumberPreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
