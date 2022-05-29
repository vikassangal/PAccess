using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for WorkersCompInfoRecvdFromPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class WorkersCompInfoRecvdFromPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler WorkersCompInfoRecvdFromPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            WorkersCompInfoRecvdFromPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            WorkersCompInfoRecvdFromPreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.WorkersCompInfoRecvdFromPreferredEvent = null;
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
            WorkersCompensationCoverage aCoverage = context as WorkersCompensationCoverage;
            if( aCoverage == null )
            {
                return true;
            }

            if( aCoverage.InformationReceivedSource == null 
                || aCoverage.InformationReceivedSource.Description == string.Empty )
            {
                if( this.FireEvents && WorkersCompInfoRecvdFromPreferredEvent != null )
                {
                    WorkersCompInfoRecvdFromPreferredEvent( this, null );
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
        public WorkersCompInfoRecvdFromPreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
