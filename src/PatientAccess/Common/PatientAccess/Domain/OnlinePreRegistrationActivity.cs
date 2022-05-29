using System;
using System.Threading;
using PatientAccess.Domain.PreRegistration;
using PatientAccess.Locking;
using PatientAccess.Utilities;

namespace PatientAccess.Domain
{
    [Serializable]
    public class OnlinePreRegistrationActivity : Activity, IDisposable
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool ReadOnlyAccount()
        {
            return true;
        }

        public override bool CanCreateNewPatient()
        {
            return true;
        }

        // Patient Type cannot change for this Activity
        public override bool CanPatientTypeChange()
        {
            return false;
        }

        public void StartRefreshingLock( OnlinePreRegistrationSubmissionLocker locker )
        {
            RefreshTimer = new Timer( delegate { locker.RefreshLock(); }, new object(), TimeSpan.Zero, LockRefreshInterval );
        }

        public bool HasPreRegistrationData()
        {
            return ( PreRegistrationData != null );
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        #endregion

        #region Properties

        public PreRegistrationData PreRegistrationData { get; set; }

        #endregion

        #region Private Methods

        private void Dispose( bool disposing )
        {
            if ( disposing )
            {
                RefreshTimer.Dispose();
            }
        }

        #endregion

        #region Private Properties

        private Timer RefreshTimer { get; set; }

        private TimeSpan LockRefreshInterval { get; set; }

        #endregion

        #region Construction and Finalization

        public OnlinePreRegistrationActivity() : this( TimeSpan.FromMinutes( 5 ) )
        {
            Description = DESCRIPTION;
            ContextDescription = DESCRIPTION;
        }

        public OnlinePreRegistrationActivity( TimeSpan lockRefreshInterval )
        {
            Guard.ThrowIfTimeSpanIsNotPositive( lockRefreshInterval, "lockRefreshInterval" );
            LockRefreshInterval = lockRefreshInterval;
        }

        #endregion

        #region Data Elements

        private const string DESCRIPTION = "View Online Preregistration Submissions";

        #endregion

        #region Constants

        #endregion
    }
}
