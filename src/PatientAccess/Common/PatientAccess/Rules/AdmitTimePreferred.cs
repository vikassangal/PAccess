using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AdmitTimePreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AdmitTimePreferred : LeafRule
    {
        #region Events
        public event EventHandler AdmitTimePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            AdmitTimePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            AdmitTimePreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            AdmitTimePreferredEvent = null;
        }

        public override void ApplyTo( object context )
        {
        }

        public override bool ShouldStopProcessing()
        {
            return false;
        }

        public override bool CanBeAppliedTo( object context )
        {
            if ( context== null || context.GetType() != typeof( Account ) )
            {
                return true;
            }
            var anAccount = context as Account;
            if ( anAccount == null )
            {
                return false;
            }
            if ( anAccount.AdmitDate.TimeOfDay <= TimeSpan.Zero || anAccount.AdmitDate.TimeOfDay == TimeSpan.MinValue )
            {
                if ( FireEvents && AdmitTimePreferredEvent != null )
                {
                    AdmitTimePreferredEvent( this, null );
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
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
