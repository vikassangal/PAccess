using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Locking;

namespace PatientAccess.UI.NewEmployersManagement
{
    internal class NewEmployerManagmentFacade
    {
        private const string ACCESSING_SCREEN_MESSAGE = "Accessing new employer management screen ...";
        private const string OFFLINELOCK_REFRESH_INTERVAL = "OFFLINELOCK_REFRESH_INTERVAL";
        private Panel _panel;
        private IEmployerBroker _employerBroker;
        private IOfflineLockBroker _lockBroker;
        private User _currentUser;
        private NewEmployerManagementLocker _employerManagementLocker;
        private NewEmployersManagementView _employersManagementView;
        private NewEmployersManagementPresenter _employersManagementPresenter;
        private ActivityEventAggregator _eventAggregator;
        private BackgroundWorker _acquireLockWorker;
        private BackgroundWorker _releaseLockWorker;
        private bool _isLockAcquired;
        private Label _accessingScreenMessage;
        private PatientAccessView _patientAccessView;


        public NewEmployerManagmentFacade(PatientAccessView patientAccessView, ActivityEventAggregator aggregator)
        {
            this.Panel = patientAccessView.Panel;
            this.PatientAccessView = patientAccessView;
            this.EmployerBroker = BrokerFactory.BrokerOfType<IEmployerBroker>();

            this.LockBroker = BrokerFactory.BrokerOfType<IOfflineLockBroker>();

            this.CurrentUser = User.GetCurrent();

            string lockOwner = this.GetLockOwner();
            string lockHandle = this.GetLockHandle();
            this.EventAggregator = aggregator;

            TimeSpan lockrefreshInterval = this.GetRefreshIntervalFromConfiguration();
            this.EmployerManagementLocker = new NewEmployerManagementLocker(this.LockBroker, lockHandle, lockOwner, lockrefreshInterval);
            this.EmployersManagementView = new NewEmployersManagementView();
            this.EmployersManagementPresenter = new NewEmployersManagementPresenter(this.CurrentUser, this.EmployersManagementView, this.EmployersManagementView, this.EmployerBroker);
            this.AcquireLockWorker = new BackgroundWorker();
            this.AcquireLockWorker.DoWork += this.TryToAcquireLock;
            this.AcquireLockWorker.RunWorkerCompleted += this.ShowScreenIfLockIsAcquired;

            this.ReleaseLockWorker = new BackgroundWorker();
            this.ReleaseLockWorker.DoWork += this.ReleaseLockWorkerOnDoWork;
            this.ReleaseLockWorker.RunWorkerCompleted += this.ReleaseLockWorkerOnRunWorkerCompleted;


            this.AccessingScreenMessage = new Label();
            this.AccessingScreenMessage.AutoSize = true;
            this.AccessingScreenMessage.Text = ACCESSING_SCREEN_MESSAGE;
        }


        private TimeSpan GetRefreshIntervalFromConfiguration()
        {
            string refreshInterval = ConfigurationManager.AppSettings[OFFLINELOCK_REFRESH_INTERVAL];
            return TimeSpan.Parse(refreshInterval);
        }


        private PatientAccessView PatientAccessView
        {
            get
            {
                return this._patientAccessView;
            }
            set
            {
                this._patientAccessView = value;
            }
        }


        private void ReleaseLockWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw e.Error;
            }
        }

        private void ReleaseLockWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            if (this.EmployerManagementLocker.IsLocked())
            {
                this.EmployerManagementLocker.ReleaseLock();
            }
        }


        void TryToAcquireLock(object sender, DoWorkEventArgs e)
        {
            this.IsLockAcquired = this.EmployerManagementLocker.AcquireLock();
        }

        void ShowScreenIfLockIsAcquired(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw e.Error;
            }

            this.ClearAccessingScreenMessage();

#if (!NOLOCKING)
            if (this.IsLockAcquired)
            {
#endif
                this.StartActivity();
                this.DisplayNewEmployerManagementView();
#if (!NOLOCKING)
            }
            else
            {
                this.DisplayShowFeatureIsLockedMessage();
            }
#endif
        }

        private Panel Panel
        {
            get
            {
                return this._panel;
            }
            set
            {
                this._panel = value;
            }
        }

        private bool IsLockAcquired
        {
            get
            {
                return this._isLockAcquired;
            }
            set
            {
                this._isLockAcquired = value;
            }
        }

        private BackgroundWorker AcquireLockWorker
        {
            get
            {
                return this._acquireLockWorker;
            }
            set
            {
                this._acquireLockWorker = value;
            }
        }

        private ActivityEventAggregator EventAggregator
        {
            get
            {
                return this._eventAggregator;
            }
            set
            {
                this._eventAggregator = value;
            }
        }

        private NewEmployersManagementPresenter EmployersManagementPresenter
        {
            get
            {
                return this._employersManagementPresenter;
            }
            set
            {
                this._employersManagementPresenter = value;
            }
        }

        private NewEmployersManagementView EmployersManagementView
        {
            get
            {
                return this._employersManagementView;
            }
            set
            {
                this._employersManagementView = value;
            }
        }

        private NewEmployerManagementLocker EmployerManagementLocker
        {
            get
            {
                return this._employerManagementLocker;
            }
            set
            {
                this._employerManagementLocker = value;
            }
        }

        private User CurrentUser
        {
            get
            {
                return this._currentUser;
            }
            set
            {
                this._currentUser = value;
            }
        }

        private IOfflineLockBroker LockBroker
        {
            get
            {
                return this._lockBroker;
            }
            set
            {
                this._lockBroker = value;
            }
        }

        private IEmployerBroker EmployerBroker
        {
            get
            {
                return this._employerBroker;
            }
            set
            {
                this._employerBroker = value;
            }
        }

        private Label AccessingScreenMessage
        {
            get
            {
                return this._accessingScreenMessage;
            }
            set
            {
                this._accessingScreenMessage = value;
            }
        }

        private BackgroundWorker ReleaseLockWorker
        {
            get
            {
                return this._releaseLockWorker;
            }
            set
            {
                this._releaseLockWorker = value;
            }
        }


        public void DisplayViewIfNotLocked()
        {
            if (!this.AcquireLockWorker.IsBusy && !this.ReleaseLockWorker.IsBusy)
            {
                this.DisAbleAdminMenu();
                this.ShowAccessingScreenMessage();
                this.AcquireLockWorker.RunWorkerAsync();
            }
        }


        private void DisAbleAdminMenu()
        {
            this.PatientAccessView.mnuAdmin.Enabled = false;
        }


        private void DisplayShowFeatureIsLockedMessage()
        {
            this.EmployersManagementPresenter.ShowFeatureIsLockedMessage();
            this.Panel.Controls.Add(this.EmployersManagementView);
        }

        private void ShowAccessingScreenMessage()
        {
            this.Panel.Controls.Add(this.AccessingScreenMessage);
        }

        private void ClearAccessingScreenMessage()
        {
            if (this.Panel.Controls.Contains(AccessingScreenMessage))
            {
                this.Panel.Controls.Remove(AccessingScreenMessage);
            }
            this.EnableAdminMenu();
        }


        private void EnableAdminMenu()
        {
            this.PatientAccessView.mnuAdmin.Enabled = true;
        }


        private void DisplayNewEmployerManagementView()
        {
            this.Panel.Controls.Add(this.EmployersManagementView);
            this.EmployersManagementPresenter.Initialize();
        }

        private void StartActivity()
        {
            this.EventAggregator.RaiseActivityStartEvent(this.EmployersManagementView, EventArgs.Empty);
            this.EventAggregator.ActivityCancelled += this.OnActivityCancelled;
            this.EventAggregator.ActivityCompleted += this.OnActivityCompleted;
        }

        private void OnActivityCompleted(object sender, EventArgs e)
        {
            this.RemoveNewEmployerManagementView();
            this.ReleaseLock();
        }

        private void ReleaseLock()
        {

            if (!ReleaseLockWorker.IsBusy)
            {
                this.ReleaseLockWorker.RunWorkerAsync();
            }
        }


        private void RemoveNewEmployerManagementView()
        {
            if (this.Panel.Controls.Contains(this.EmployersManagementView))
            {
                this.Panel.Controls.Remove(this.EmployersManagementView);
            }
        }

        private void OnActivityCancelled(object sender, EventArgs e)
        {
            this.RemoveNewEmployerManagementView();
            this.ReleaseLock();
        }

        private string GetLockHandle()
        {
            return this.CurrentUser.Facility.Code;
        }

        private string GetLockOwner()
        {
            return this.CurrentUser.SecurityUser.UPN + Environment.MachineName + Process.GetCurrentProcess().Id;
        }
    }
}
