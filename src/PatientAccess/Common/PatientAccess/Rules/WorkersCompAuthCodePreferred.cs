using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for WorkersCompAuthCodePreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class WorkersCompAuthCodePreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler WorkersCompAuthCodePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            WorkersCompAuthCodePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            WorkersCompAuthCodePreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.WorkersCompAuthCodePreferredEvent = null;            
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
            if( context == null || context.GetType() != typeof( WorkersCompensationCoverage ) )
            {
                return true;
            }
            WorkersCompensationCoverage aCoverage = context as WorkersCompensationCoverage;
            if( aCoverage == null )
            {
                return false;
            }

            if (aCoverage.Authorization != null)
            {
             if( aCoverage.Authorization.AuthorizationNumber == String.Empty )
                {
                    if( this.FireEvents && WorkersCompAuthCodePreferredEvent != null )
                    {
                        WorkersCompAuthCodePreferredEvent( this, null );
                    }
                    return false;
            }
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
        public WorkersCompAuthCodePreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
