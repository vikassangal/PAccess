using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for WorkersCompEmpSupervisorRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class WorkersCompEmpSupervisorRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler WorkersCompEmpSupervisorRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            WorkersCompEmpSupervisorRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            WorkersCompEmpSupervisorRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.WorkersCompEmpSupervisorRequiredEvent = null;            
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
            if( context.GetType() != typeof( WorkersCompensationCoverage ) )
            {
                return true;
            }

            WorkersCompensationCoverage aCoverage = context as WorkersCompensationCoverage;

            if( aCoverage != null && aCoverage.PatientsSupervisor.Trim() != String.Empty )
            {
                return true;
            }
            else
            {
                if( this.FireEvents && WorkersCompEmpSupervisorRequiredEvent != null )
                {
                    WorkersCompEmpSupervisorRequiredEvent( this, null );
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
        public WorkersCompEmpSupervisorRequired()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
