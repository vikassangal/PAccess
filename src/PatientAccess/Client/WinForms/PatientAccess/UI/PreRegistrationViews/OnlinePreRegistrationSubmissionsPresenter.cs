using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Threading;
using Extensions.SecurityService.Domain;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Locking;
using PatientAccess.UI.Factories;
using PatientAccess.UI.Logging;
using PatientAccess.UI.WorklistViews;
using PatientAccess.Utilities;
using User = PatientAccess.Domain.User;

namespace PatientAccess.UI.PreRegistrationViews
{
    public class OnlinePreRegistrationSubmissionsPresenter : IOnlinePreRegistrationSubmissionsPresenter
    {
        public OnlinePreRegistrationSubmissionsPresenter(
            Facility facility,
            IOnlinePreRegistrationSubmissionsView view,
            Activity activity,
            IPreRegistrationSubmissionsBroker broker )
        {
            Guard.ThrowIfArgumentIsNull( facility, "facility" );
            Guard.ThrowIfArgumentIsNull( view, "view" );
            Guard.ThrowIfArgumentIsNull( activity, "activity" );
            Guard.ThrowIfArgumentIsNull( view, "broker" );

            Facility = facility;
            View = view;
            View.Presenter = this;
            Activity = activity;
            Broker = broker;
            timerDelegate = new TimerCallback( View.RefreshPreRegistrationSubmissionsList );
            refreshTimer = new Timer( timerDelegate, this, Timeout.Infinite, Timeout.Infinite );

            LockBroker = BrokerFactory.BrokerOfType<IOfflineLockBroker>();
            CurrentUser = User.GetCurrent();
        }

        #region Methods

        public void UpdateView()
        {
            View.HideProgressPanel();
            View.HidePatientSearchProgressPanel();
            View.HideNoOnlineSubmissionsMessage();

            var securityUser = patientAccessUser.SecurityUser;
            var securityFrameworkFacility = new Peradigm.Framework.Domain.Parties.Facility( patientAccessUser.Facility.Code, patientAccessUser.Facility.Description );
            var canViewOnlinePreRegistrations = securityUser.HasPermissionTo( Privilege.Actions.View, new PreRegistrationWorklistActivity().Description, securityFrameworkFacility );

            if ( canViewOnlinePreRegistrations )
            {
                View.ShowPreRegPanel();
                View.ShowMatchingPatientsPanel();
                View.SetDefaultButtonState();
                View.HideNoAccessPanel();
                UpdateFilterWorklistView();
                PopulatePreRegistrationSubmissions();
            }
            else
            {
                View.ShowNoAccessView();
            }
        }

        private void UpdateFilterWorklistView()
        {
            FilterWorklistView.Model_WorklistSetting = WorklistBroker.OnlinePreRegWorklistSettings( ( patientAccessUser.SecurityUser.TenetID ) );

            if ( FilterWorklistView.Model_WorklistSetting == null )
            {
                FilterWorklistView.Model_WorklistSetting = new WorklistSettings();
                SetDefaultWorkllistSettings();
            }

            worklistRanges = WorklistBroker.GetOnlinePreRegWorklistRanges();

            if ( worklistRanges != null && worklistRanges.Count > 0 )
            {
                FilterWorklistView.PeriodComboBox.Items.Clear();

                foreach ( WorklistSelectionRange range in worklistRanges )
                {
                    FilterWorklistView.PeriodComboBox.Items.Add( range );
                }
            }

            FilterWorklistView.UpdateView();
            FilterWorklistView.Focus();
        }

        public void CreateAndLoadNewPatient()
        {
            View.ShowWaitCursor();
            var locker = new OnlinePreRegistrationSubmissionLocker( LockBroker, GetLockHandle(), GetLockOwner() );
            var lockAcquired = locker.AcquireLock();
            View.ShowDefaultCursor();

            if ( lockAcquired )
            {
                //Raise activity start event for create new patient
                ActivityEventAggregator.GetInstance().RaiseActivityStartEvent( this, EventArgs.Empty );

                SetupLockRelease( locker );
 
                //pass in the id for the select submission
                var preRegistrationData = preRegistrationSubmissionsBroker.GetDataFor( selectedOnlinePreRegItem.Id );
                var onlinePreRegistrationActivity = ( (OnlinePreRegistrationActivity)Activity.AssociatedActivity );
                onlinePreRegistrationActivity.StartRefreshingLock( locker );

                if ( preRegistrationData != null )
                {
                    onlinePreRegistrationActivity.PreRegistrationData = preRegistrationData;
                    LoadPatient( preRegistrationData.Patient );
                }
                else
                {
                    View.ShowPreRegistrationSubmissionNotAvailableMessage();
                    PopulatePreRegistrationSubmissions();
                }
            }
            else
            {
                View.ShowPreRegistrationSubmissionIsLockedMessage();
            } 
        }

        private static void SetupLockRelease( OnlinePreRegistrationSubmissionLocker locker )
        {
            var worker = new BackgroundWorker();
            worker.DoWork += delegate { locker.ReleaseLock(); };

            ActivityEventAggregator.GetInstance().ActivityCancelled += delegate { worker.RunWorkerAsync(); };
            ActivityEventAggregator.GetInstance().ActivityCompleted += delegate { worker.RunWorkerAsync(); };
        }

        public void CreateAndLoadNewAccountForExistingPatient()
        {
            View.ShowWaitCursor();
            var locker = new OnlinePreRegistrationSubmissionLocker( LockBroker, GetLockHandle(), GetLockOwner() );
            var lockAcquired = locker.AcquireLock();
            View.ShowDefaultCursor();

            if ( lockAcquired )
            {
                //Raise activity start event for create new account
                ActivityEventAggregator.GetInstance().RaiseActivityStartEvent( this, EventArgs.Empty );

                var onlinePreRegistrationActivity = ( (OnlinePreRegistrationActivity)Activity.AssociatedActivity );
                onlinePreRegistrationActivity.StartRefreshingLock(locker);
                SetupLockRelease( locker );

                var existingPatient = GetSelectedPatientFromPatientSearchResult();

                var preRegistrationData = preRegistrationSubmissionsBroker.GetSubmissionInformationForNewAccount( existingPatient, selectedOnlinePreRegItem.Id );
                if ( preRegistrationData != null )
                {
                    onlinePreRegistrationActivity.PreRegistrationData = preRegistrationData;
                    LoadPatient( preRegistrationData.Patient );
                }
                else
                {
                    View.ShowPreRegistrationSubmissionNotAvailableMessage();
                    PopulatePreRegistrationSubmissions();
                }
            }
            else
            {
                View.ShowPreRegistrationSubmissionIsLockedMessage();
            } 
        }

        public void OnViewDispose()
        {
            CancelBackgroundWorker();
            CancelTimer();
        }

        public void PopulatePreRegistrationSubmissions()
        {
            Worklist = WorklistBroker.OnlinePreRegWorklist();

            if ( backgroundWorker == null || !backgroundWorker.IsBusy )
            {
                BeforeWork();

                backgroundWorker = new BackgroundWorker { WorkerSupportsCancellation = true };

                backgroundWorker.DoWork += DoPopulatePreRegistrationSubmissions;
                backgroundWorker.RunWorkerCompleted += AfterWork;

                backgroundWorker.RunWorkerAsync();
            }
        }

        public void ResetButtonClickEvent( object sender, EventArgs e )
        {
            CancelTimer();
            ResetControls();
            SetDefaultWorkllistSettings();

            View.ClearNoOnlineSubmissionsText();
            View.ShowNoOnlineSubmissionsMessage();

            FilterWorklistView.Model_WorklistSetting.SortedColumnDirection = 1;
            FilterWorklistView.UpdateView();

            ResetControls();
        }

        public void PrintReportEvent()
        {
            var worklistSettings = FilterWorklistView.Model_WorklistSetting;
            worklistSettings.WorkListID = worklistOid;
            GetOnlinePreRegistrationItems( worklistSettings );
            var worklistReport = new OnlinePreRegistrationSubmissionsReport
            {
                Model = ArrayList.Adapter( (IList)PreRegistrations ),
                SearchCriteria = SearchCriteria,
                HeaderText = HeaderText
            };
            worklistReport.PrintPreview();
        }

        public void FindMatchingPatients( OnlinePreRegistrationItem preRegistrationItem )
        {
            selectedOnlinePreRegItem = preRegistrationItem;
            View.ShowWaitCursor();

            if ( matchingPatientsBackgroundWorker == null || !matchingPatientsBackgroundWorker.IsBusy )
            {
                BeforeFindMatchingPatientsWork();

                matchingPatientsBackgroundWorker = new BackgroundWorker { WorkerSupportsCancellation = true };

                matchingPatientsBackgroundWorker.DoWork += DoFindMatchingPatients;
                matchingPatientsBackgroundWorker.RunWorkerCompleted += AfterFindMatchingPatientsWork;

                matchingPatientsBackgroundWorker.RunWorkerAsync();
            }

            View.ShowDefaultCursor();
        }

        public void OnViewLeave()
        {
            FilterWorklistView.WorkListView_Leave();
            WorklistBroker.SaveWorklistSettings( User.GetCurrent().SecurityUser.TenetID, FilterWorklistView.Model_WorklistSetting, worklist );
        }

        private Gender GetGender( OnlinePreRegistrationItem onlinePreRegistrationItem )
        {
            Gender gender;
            if ( string.Compare( onlinePreRegistrationItem.Gender, FEMALE_DESCRIPTION, true ) == 0 )
            {
                gender = DemographicsBroker.GenderWith( User.GetCurrent().Facility.Oid, Gender.FEMALE_CODE );
            }
            else if ( string.Compare( onlinePreRegistrationItem.Gender, MALE_DESCRIPTION, true ) == 0 )
            {
                gender = DemographicsBroker.GenderWith( User.GetCurrent().Facility.Oid, Gender.MALE_CODE );
            }
            else
            {
                gender = DemographicsBroker.GenderWith( User.GetCurrent().Facility.Oid, Gender.UNKNOWN_CODE );
            }
            return gender;
        }

        #endregion

        #region Private Methods

        private string GetLockHandle()
        {
            return selectedOnlinePreRegItem.Id.ToString();
        }

        private string GetLockOwner()
        {
            return CurrentUser.SecurityUser.UPN + Environment.MachineName + Process.GetCurrentProcess().Id;
        }

        /// <summary>
        /// Gets the selected patient from patient search result.
        /// </summary>
        private Patient GetSelectedPatientFromPatientSearchResult()
        {
            var patientPbarBroker = BrokerFactory.BrokerOfType<IPatientBroker>();

            View.ShowWaitCursor();
            var patient = patientPbarBroker.PatientFrom( selectedPatientSearchResult );
            View.ShowDefaultCursor();
            BreadCrumbLogger.GetInstance.Log( String.Format( "{0} Patient selected from the search results", patient.Name.AsFormattedName() ) );

            return patient;
        }

        private void LoadPatient( Patient newPatient )
        {
            newPatient.SelectedAccount.IsNew = true;
            newPatient.SelectedAccount.Facility = User.GetCurrent().Facility;
            newPatient.SelectedAccount.Activity = Activity;
            newPatient.SelectedAccount.OnlinePreRegistered = true;
            newPatient.SelectedAccount.ActionsLoader = new ActionLoader( newPatient.SelectedAccount );

            var patientAccessView = ViewFactory.Instance.CreateView<PatientAccessView>();
            patientAccessView.ActivateTab( newPatient.SelectedAccount.ToString(), newPatient.SelectedAccount );
            patientAccessView.EnableSupplementalInformationButton();
        }

        private void SetDefaultWorkllistSettings()
        {
            ProxyFactory.GetTimeBroker();
            FilterWorklistView.Model_WorklistSetting =
                WorklistBroker.DefaultOnlinePreRegWorklistSettings();
        }

        private void CancelTimer()
        {
            if ( refreshTimer != null )
            {
                refreshTimer.Change( Timeout.Infinite, Timeout.Infinite );
                refreshTimer.Dispose();
                refreshTimer = null;
            }
        }

        // Cancels background worker
        private void CancelBackgroundWorker()
        {
            if ( backgroundWorker != null )
            {
                backgroundWorker.CancelAsync();
            }
        }

        private void BeforeWork()
        {
            View.ShowProgressPanel();

            FilterWorklistView.SetPrintButtonState( false );

            // Cancel timer in case it's running
            if ( refreshTimer == null )
            {
                timerDelegate = new TimerCallback( View.RefreshPreRegistrationSubmissionsList );
                refreshTimer = new Timer( timerDelegate, this, Timeout.Infinite, Timeout.Infinite );
            }

            refreshTimer.Change( Timeout.Infinite, Timeout.Infinite );

            View.ShowAppStartingCursor();
        }

        private void DoPopulatePreRegistrationSubmissions( object sender, DoWorkEventArgs e )
        {
            worklistOid = worklist.Oid;

            if ( Facility == null )
            {
                View.ShowNullFacilityMessage();
                return;
            }

            var worklistSetting = FilterWorklistView.Model_WorklistSetting;

            worklistSetting.WorkListID = worklistOid;

            GetOnlinePreRegistrationItems( worklistSetting );

            SearchCriteria = worklistSetting;

            onlinePreRegItems = PreRegistrations;

            // poll CancellationPending and set e.Cancel to true and return 
            if ( backgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            }
        }

        /// <exception cref="Exception"><c>If an exception was encountered in the background processing</c>.</exception>
        private void AfterWork( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( View.ViewDisposed() )
                return;

            View.ShowDefaultCursor();

            if ( e.Cancelled )
            {
                // user cancelled
                // Should be noted that due to a race condition in the DoWork event handler, 
                // the Cancelled flag may not have been set, even though CancelAsync was called.
            }
            else if ( e.Error != null )
            {
                // handle errors/exceptions thrown from within the DoWork method here
                if ( e.Error.GetType() == typeof( RemotingTimeoutException ) )
                {
                    View.ShowRemotingTimeOutMesssage();
                    View.ShowNoOnlineSubmissionsMessage();
                    ResetPreRegistrationSubmissionsView();
                }
                else
                {
                    throw e.Error;
                }
            }
            else
            {
                // success 
                DisplayPreRegistrationSubmissions();
            }

            // post-completion operations...
            View.HideProgressPanel();
        }

        private void DisplayPreRegistrationSubmissions()
        {
            if ( onlinePreRegItems == null || onlinePreRegItems.Count <= 0 )
            {
                View.ShowNoOnlineSubmissionsMessage();
                View.ShowNoMatchingPatientsMessage();
                ResetPreRegistrationSubmissionsView();
                return;
            }

            View.ShowCreateNewPatientButton();
            View.EnableCreateNewPatientButton();

            View.HideNoOnlineSubmissionsMessage();
            FilterWorklistView.SetPrintButtonState( true );

            View.UpdateOnlinePreRegistrationsList( preRegistrations );
            View.SetNewPatientButtonState();

            selectedOnlinePreRegItem = onlinePreRegItems[0];

            FindMatchingPatients( selectedOnlinePreRegItem );

            // Start the timer to refresh the worklist automatically
            if ( refreshTimer != null )
            {
                refreshTimer.Change( TimerIntervalInMilliseconds, TimerIntervalInMilliseconds );
            }
        }

        private void GetOnlinePreRegistrationItems( WorklistSettings ws )
        {
            PreRegistrations.Clear();
            MatchingPatients.Clear();
            preRegistrations = Broker.GetSubmittedMessagesList( Facility.Oid, ws ).ToList();
        }

        private void ResetPreRegistrationSubmissionsView()
        {
            View.HideProgressPanel();
            View.HidePatientSearchProgressPanel();
            ResetControls();
        }

        private void ResetControls()
        {
            selectedOnlinePreRegItem = null;
            View.SetNotEnabledButtonState();
            onlinePreRegItems.Clear();
            MatchingPatients.Clear();
        }

        private void BeforeFindMatchingPatientsWork()
        {
            View.ShowPatientSearchProgressPanel();
            View.ShowWaitCursor();
        }

        private void DoFindMatchingPatients( object sender, DoWorkEventArgs e )
        {
            var gender = GetGender( selectedOnlinePreRegItem );

            var patientSearchCriteria = GetPatientSearchCriteria( selectedOnlinePreRegItem, gender );

            patientSearchCriteria.DayOfBirth = selectedOnlinePreRegItem.DateOfBirth.Day;

            // poll CancellationPending and set e.Cancel to true and return 
            if ( matchingPatientsBackgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            }

            var broker = BrokerFactory.BrokerOfType<IPatientBroker>();

            patientSearchResponse = broker.GetPatientSearchResponseFor( patientSearchCriteria );

            matchingPatients = patientSearchResponse.PatientSearchResults;

            // poll CancellationPending and set e.Cancel to true and return 
            if ( matchingPatientsBackgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            }
        }

        private static PatientSearchCriteria GetPatientSearchCriteria( OnlinePreRegistrationItem onlinePreRegistrationItem, Gender gender )
        {
            return new PatientSearchCriteria(
                User.GetCurrent().Facility.Code,
                GetNameSearchCriteria( onlinePreRegistrationItem.Name.FirstName, FirstnameSerachCiteriaLength ),
                GetNameSearchCriteria( onlinePreRegistrationItem.Name.LastName, LastnameSerachCriteriaLength ),
                new SocialSecurityNumber()  , 
                gender,
                onlinePreRegistrationItem.DateOfBirth.Month,
                onlinePreRegistrationItem.DateOfBirth.Year,
                null,
                null
                );
        }
       
        private static string GetNameSearchCriteria( string name, int length )
        {
            if ( name.Length > length - 1 )
            {
                return name.Substring( 0, length );
            }
            
            return name;
        }

        /// <exception cref="Exception"><c>If an exception was encountered in the background processing</c>.</exception>
        private void AfterFindMatchingPatientsWork( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( View.ViewDisposed() )
                return;

            if ( e.Cancelled )
            {
            }
            else if ( e.Error != null )
            {
                // handle errors/exceptions thrown from within the DoWork method here
                if ( e.Error.GetType() == typeof( RemotingTimeoutException ) )
                {
                    View.ShowPatientSearchTimeOutMessage();
                }
                else
                {
                    throw e.Error;
                }
            }
            else
            {
                // success
                DisplayMatchingPatients();
            }

            View.HidePatientSearchProgressPanel();
            View.ShowDefaultCursor();
        }

        private void DisplayMatchingPatients()
        {
            matchingPatients = patientSearchResponse.GetResultsOfType( TypeOfName.Normal );

            var foundMatchingPatients = matchingPatients.Count > 0;

            if ( foundMatchingPatients )
            {
                View.HideNoMatchingPatientsMessage();
            }
            else
            {
                View.ShowNoMatchingPatientsMessage();
            }

            View.UpdateMatchingPatientsList( MatchingPatients );
            View.SetNewAccountButtonState();

            if ( MatchingPatients.Count > 0 )
                SelectedPatientSearchResult = MatchingPatients[0];
        }

        public OnlinePreRegistrationItem SelectSubmissionItem { get; set; }

        public void ShowPreRegistrationSubmissionsRequestEvent( object sender, EventArgs e )
        {
            onlinePreRegItems.Clear();
            MatchingPatients.Clear();

            View.ShowWaitCursor();

            PopulatePreRegistrationSubmissions();
            View.ShowDefaultCursor();
        }
        #endregion

        #region Private Properties

        private FilterWorklistView FilterWorklistView
        {
            get { return View.FilterWorklistView; }
        }

        private IList<OnlinePreRegistrationItem> PreRegistrations
        {
            get
            {
                return preRegistrations;
            }
        }

        private Worklist Worklist
        {
            set
            {
                worklist = value;
                WorklistType = Convert.ToInt32( worklist.Oid );
            }
        }

        private int WorklistType
        {
            set
            {
                FilterWorklistView.WorklistType = value;
            }
        }

        private Facility Facility
        {
            get;
            set;
        }

        private WorklistSettings SearchCriteria { get; set; }

        private IList<PatientSearchResult> MatchingPatients
        {
            get { return matchingPatients; }
        }

        private IWorklistSettingsBroker WorklistBroker
        {
            get { return worklistBroker ?? ( worklistBroker = BrokerFactory.BrokerOfType<IWorklistSettingsBroker>() ); }
        }

        private IDemographicsBroker DemographicsBroker
        {
            get { return demographicsBroker ?? ( demographicsBroker = BrokerFactory.BrokerOfType<IDemographicsBroker>() ); }
        }

        public PatientSearchResult SelectedPatientSearchResult
        {
            get { return selectedPatientSearchResult; }
            set { selectedPatientSearchResult = value; }
        }

        private User CurrentUser { get; set; }

        private IOfflineLockBroker LockBroker { get; set; }

        private IPreRegistrationSubmissionsBroker Broker { get; set; }

        private Activity Activity { get; set; }

        #endregion

        #region Data Elements
        public IOnlinePreRegistrationSubmissionsView View { get; private set; }
        private readonly User patientAccessUser = User.GetCurrent();

        private ArrayList worklistRanges;
        private BackgroundWorker backgroundWorker;
        private BackgroundWorker matchingPatientsBackgroundWorker;
        private Worklist worklist;
        private long worklistOid;
        private const string HeaderText = "          Online Pregistration Submission List";
        private IWorklistSettingsBroker worklistBroker;
        private IDemographicsBroker demographicsBroker;

        private OnlinePreRegistrationItem selectedOnlinePreRegItem;
        private PatientSearchResult selectedPatientSearchResult;

        private IList<PatientSearchResult> matchingPatients = new List<PatientSearchResult>();
        private IList<OnlinePreRegistrationItem> preRegistrations = new List<OnlinePreRegistrationItem>();
        private IList<OnlinePreRegistrationItem> onlinePreRegItems;

        private PatientSearchResponse patientSearchResponse;

        private Timer refreshTimer;
        private TimerCallback timerDelegate;
        private readonly IPreRegistrationSubmissionsBroker preRegistrationSubmissionsBroker = BrokerFactory.BrokerOfType<IPreRegistrationSubmissionsBroker>();
        private const int TimerIntervalInMilliseconds = 600000; // 10 minutes
        private const int FirstnameSerachCiteriaLength = 1; // Online Pregistration submission that match first letter of the first name
        private const int LastnameSerachCriteriaLength = 4; //Online Pregistration submission that match first four letters of the last name

        #endregion

        #region Constants

        private const string
             FEMALE_DESCRIPTION = "Female",
             MALE_DESCRIPTION = "Male",
             UNKNOWN_DESCRIPTION = "Unknown";

        #endregion
    }
}
