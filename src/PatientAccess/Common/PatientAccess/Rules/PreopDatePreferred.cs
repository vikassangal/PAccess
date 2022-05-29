using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    [Serializable]
    [UsedImplicitly]
    public class PreopDatePreferred : LeafRule
    {
        #region Events
        public event EventHandler PreopDatePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            PreopDatePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            PreopDatePreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            PreopDatePreferredEvent = null;
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
            if ( context.GetType() != typeof( Account ) )
            {
                return true;
            }

            var anAccount = context as Account;

            if ( anAccount == null )
            {
                return false;
            }

            if ( anAccount.ShouldWeEnablePreopDate() && anAccount.PreopDate.Date.Equals( DateTime.MinValue ) )
            {
                if ( FireEvents && PreopDatePreferredEvent != null )
                {
                    PreopDatePreferredEvent( this, null );
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

        public PreopDatePreferred()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
