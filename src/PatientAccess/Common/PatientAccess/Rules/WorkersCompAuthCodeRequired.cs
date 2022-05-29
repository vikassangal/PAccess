using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for WorkersCompAuthCodeRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class WorkersCompAuthCodeRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler WorkersCompAuthCodeRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            WorkersCompAuthCodeRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            WorkersCompAuthCodeRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.WorkersCompAuthCodeRequiredEvent = null;            
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
            if( aCoverage == null )
            {
                return false;
            }
            if( aCoverage.Authorization.AuthorizationNumber == String.Empty )
            {
                if( this.FireEvents && WorkersCompAuthCodeRequiredEvent != null )
                {
                    WorkersCompAuthCodeRequiredEvent( this, null );
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
        public WorkersCompAuthCodeRequired()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
