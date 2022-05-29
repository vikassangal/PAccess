using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MedicareDaysRemainingCoInsPreferred.
    /// </summary>
    
    [Serializable]
    [UsedImplicitly]
    public class MedicareDaysRemainingCoInsPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler MedicareDaysRemainingCoInsPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            MedicareDaysRemainingCoInsPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            MedicareDaysRemainingCoInsPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.MedicareDaysRemainingCoInsPreferredEvent = null;   
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
            if( aCoverage.RemainingCoInsurance <= 0 )
            {
                if( this.FireEvents && MedicareDaysRemainingCoInsPreferredEvent != null )
                {
                    MedicareDaysRemainingCoInsPreferredEvent( this, null );
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
        public MedicareDaysRemainingCoInsPreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
