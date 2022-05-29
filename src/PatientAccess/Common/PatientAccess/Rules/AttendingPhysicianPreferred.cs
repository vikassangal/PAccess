using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AttendingPhysicianPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AttendingPhysicianPreferred : LeafRule
    {
        #region Events

        public event EventHandler AttendingPhysicianPreferredEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler( EventHandler eventHandler )
        {
            AttendingPhysicianPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            AttendingPhysicianPreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            AttendingPhysicianPreferredEvent = null;
        }

        public override bool CanBeAppliedTo( object context )
        {
            if ( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }

            var anAccount = (Account)context;

            if ( anAccount.Activity == null )
            {
                return true;
            }

            if ( anAccount.KindOfVisit == null )
            {
                return true;
            }

            if ( AccountDoesNotHaveAnAttendingPhysician( anAccount ) )
            {
                if ( FireEvents && AttendingPhysicianPreferredEvent != null )
                {
                    AttendingPhysicianPreferredEvent( this, null );
                }

                return false;
            }
            return true;
        }

        
        private static bool AccountDoesNotHaveAnAttendingPhysician( IAccount anAccount )
        {
            return ( anAccount.AttendingPhysician == null || anAccount.AttendingPhysician.FirstName.Trim() + anAccount.AttendingPhysician.LastName.Trim() == string.Empty );
        }


        public override void ApplyTo( object context )
        {

        }

        public override bool ShouldStopProcessing()
        {
            return false;
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
