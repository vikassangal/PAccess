using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for WorkersCompClaimNumberPreferred.
	/// </summary>
    [Serializable]
    [UsedImplicitly]
    public class WorkersCompClaimNumberPreferred : LeafRule
	{
        #region Event Handlers
        public event EventHandler WorkersCompClaimNumberPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            WorkersCompClaimNumberPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            WorkersCompClaimNumberPreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.WorkersCompClaimNumberPreferredEvent = null;            
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
            if( context == null
                || context.GetType() != typeof(WorkersCompensationCoverage) )
            {
                return true;
            }

            WorkersCompensationCoverage aCoverage = context as WorkersCompensationCoverage;

            if( aCoverage != null
                && aCoverage.ClaimNumberForIncident != null
                && aCoverage.ClaimNumberForIncident != String.Empty )
            {
                return true;
            }
            else
            {
                if( this.FireEvents && WorkersCompClaimNumberPreferredEvent != null )
                {
                    WorkersCompClaimNumberPreferredEvent( this, null );
                }
                
                return false;
            }
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public WorkersCompClaimNumberPreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
