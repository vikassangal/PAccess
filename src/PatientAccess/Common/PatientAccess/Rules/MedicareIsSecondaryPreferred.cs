using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for MedicareIsSecondaryPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class MedicareIsSecondaryPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler MedicareIsSecondaryPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            MedicareIsSecondaryPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            MedicareIsSecondaryPreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.MedicareIsSecondaryPreferredEvent = null;   
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
            if( aCoverage.MedicareIsSecondary == null 
                || aCoverage.MedicareIsSecondary.Code == YesNoFlag.CODE_BLANK )
            {
                if( this.FireEvents && MedicareIsSecondaryPreferredEvent != null )
                {
                    MedicareIsSecondaryPreferredEvent( this, null );
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
        public MedicareIsSecondaryPreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
