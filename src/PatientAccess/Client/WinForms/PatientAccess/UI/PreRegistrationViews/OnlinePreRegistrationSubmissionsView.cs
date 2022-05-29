using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.WorklistViews;

namespace PatientAccess.UI.PreRegistrationViews
{
    public partial class OnlinePreRegistrationSubmissionsView : ControlView, IOnlinePreRegistrationSubmissionsView
    {

        public OnlinePreRegistrationSubmissionsView()
        {
            InitializeComponent();

            //There is a defect in the FilterWorklistView type such that if there are no subscribers for these events 
            //then it does not update the letter selections. We need the updated letter selections for this view.
            //To work around that defect for now we are adding no op event handlers.
            i_FilterWorklistView.FirstFilterLetterSelected += delegate { };
            i_FilterWorklistView.LastFilterLetterSelected += delegate { };

            EnableThemesOn( this );

            SetBackgroundColor();
            CreateOnlinePreRegDataTableColumns();
            CreateMatchingPatientsDataTableColumns();
        }

        private void SetBackgroundColor()
        {
            BackColor = UIColors.FormBackColor;
        }

        private void CreateOnlinePreRegDataTableColumns()
        {
            onlinePreRegDataTable.Columns.Add( isLockedDataColumn );
            onlinePreRegDataTable.Columns.Add( patientNameDataColumn );
            onlinePreRegDataTable.Columns.Add( visitedBeforeDataColumn );
            onlinePreRegDataTable.Columns.Add( genderDataColumn );
            onlinePreRegDataTable.Columns.Add( dobDataColumn );
            onlinePreRegDataTable.Columns.Add( ssnDataColumn );
            onlinePreRegDataTable.Columns.Add( addressDataColumn );
            onlinePreRegDataTable.Columns.Add( admitDateDataColumn );
            onlinePreRegDataTable.Columns.Add( dummyDataColumn );
        }

        private void GenerateOnlinePreRegistrationsDataTable( IEnumerable<OnlinePreRegistrationItem> preRegistrations )
        {
            onlinePreRegDataTable.Rows.Clear();

            foreach ( OnlinePreRegistrationItem onlinePreRegWorklistItem in preRegistrations )
            {
                DataRow row = onlinePreRegDataTable.NewRow();

                // Replace the string value with the image value.
                if ( onlinePreRegWorklistItem.IsLocked )
                {
                    row[IS_LOCKED] = lockImage.Images[0];
                }
                else
                {
                    row[IS_LOCKED] = new Bitmap( 1, 1 );
                }

                row[PATIENT_NAME] = onlinePreRegWorklistItem.PatientName;
                row[VISITED_BEFORE] = onlinePreRegWorklistItem.VisitedBefore.HasValue
                                          ? ( ( bool )onlinePreRegWorklistItem.VisitedBefore
                                                 ? YesNoFlag.DESCRIPTION_YES
                                                 : YesNoFlag.DESCRIPTION_NO )
                                          : string.Empty;

                row[GENDER] = onlinePreRegWorklistItem.Gender;
                row[DATE_OF_BIRTH] = onlinePreRegWorklistItem.DateOfBirth;
                row[SSN] = onlinePreRegWorklistItem.Ssn;
                row[ADDRESS] = onlinePreRegWorklistItem.Address;
                row[ADMIT_DATE] = onlinePreRegWorklistItem.AdmitDate;
                row[DUMMY_COLUMN] = onlinePreRegWorklistItem;

                onlinePreRegDataTable.Rows.Add( row );
            }
        }

        public void UpdateOnlinePreRegistrationsList( IEnumerable<OnlinePreRegistrationItem> preRegistrations )
        {
            OnlinePreRegDataGrid.AutoGenerateColumns = false;
            GenerateOnlinePreRegistrationsDataTable( preRegistrations );

            onlinePreRegDataGrid.SelectionChanged -= OnlineEntriesDataGrid_SelectionChanged;
            OnlinePreRegDataGrid.DataSource = onlinePreRegDataTable;
            onlinePreRegDataGrid.SelectionChanged += OnlineEntriesDataGrid_SelectionChanged;
        }

        private void CreateMatchingPatientsDataTableColumns()
        {
            matchingPatientsDataTable.Columns.Add( matchingNameDataColumn );
            matchingPatientsDataTable.Columns.Add( matchingGenderDataColumn );
            matchingPatientsDataTable.Columns.Add( matchingDobDataColumn );
            matchingPatientsDataTable.Columns.Add( matchingSsnDataColumn );
            matchingPatientsDataTable.Columns.Add( matchingAddressDataColumn );
            matchingPatientsDataTable.Columns.Add( matchingDummyDataColumn );
        }

        private void GenerateMatchingPatientsDataTable( IEnumerable<PatientSearchResult> matchingPatients )
        {
            matchingPatientsDataTable.Rows.Clear();

            foreach ( PatientSearchResult patient in matchingPatients )
            {
                DataRow row = matchingPatientsDataTable.NewRow();

                row[MATCHING_PATIENT_NAME] = patient.FormattedName;
                row[MATCHING_GENDER] = patient.Gender.Description;
                row[MATCHING_DATE_OF_BIRTH] = patient.DateOfBirth;
                row[MATCHING_SSN] = new SocialSecurityNumber( patient.SocialSecurityNumber ).AsFormattedString();
                row[MATCHING_ADDRESS] = patient.FormattedAddress;
                row[MATCHING_DUMMY_COLUMN] = patient;

                matchingPatientsDataTable.Rows.Add( row );
            }
        }

        public void UpdateMatchingPatientsList( IEnumerable<PatientSearchResult> matchingPatients )
        {
            MatchingPatientsDataGrid.AutoGenerateColumns = false;
            GenerateMatchingPatientsDataTable( matchingPatients );
            MatchingPatientsDataGrid.DataSource = matchingPatientsDataTable;
        }

        #region Event Handlers

        public void ShowNoAccessView()
        {
            SetVisibilityFor( onlinePreRegistrationsPanel.Controls, false );
            SetVisibilityFor( matchingPatientsPanel.Controls, false );
            HideNewPatientAndAccountButtons();
            ShowNoAccessPanel();
        }

        public void ShowMatchingPatientsPanel()
        {
            SetVisibilityFor( matchingPatientsPanel.Controls, true );
        }

        public void ShowPreRegPanel()
        {
            SetVisibilityFor( onlinePreRegistrationsPanel.Controls, true );
        }

        private void OnlinePreRegWorklistView_Disposed( object sender, EventArgs e )
        {
            Presenter.OnViewDispose();
        }

        private void btnCreateNewPatient_Click( object sender, EventArgs e )
        {
            Presenter.CreateAndLoadNewPatient();
        }

        private void btnCreateNewAccount_Click( object sender, EventArgs e )
        {
            Presenter.CreateAndLoadNewAccountForExistingPatient();
        }

        private void OnlinePreRegWorklistView_Leave( object sender, EventArgs e )
        {
            Presenter.OnViewLeave();
        }

        private void OnlinePreRegDataGrid_DataBindingComplete( object sender, DataGridViewBindingCompleteEventArgs e )
        {
            UpdateSelectedOnlinePreRegItem();
        }

        private void OnlineEntriesDataGrid_SelectionChanged( object sender, EventArgs e )
        {
            UpdateSelectedOnlinePreRegItem();
        }

        private void MatchingPatientsDataGrid_SelectionChanged( object sender, EventArgs e )
        {
            if ( matchingPatientsDataGrid.SelectedRows.Count > 0 )
            {
                DataRowView row = ( DataRowView )matchingPatientsDataGrid.SelectedRows[0].DataBoundItem;
                Presenter.SelectedPatientSearchResult = ( PatientSearchResult )row.Row[MATCHING_DUMMY_COLUMN];
            }
        }

        public void ShowPreRegistrationSubmissionIsLockedMessage()
        {
            MessageBox.Show( UIErrorMessages.ONLINE_PREREGISTRATION_ENTRY_LOCKED, "Error",
                             MessageBoxButtons.OK, MessageBoxIcon.Error,
                             MessageBoxDefaultButton.Button1 );
        }

        public void ShowPreRegistrationSubmissionNotAvailableMessage()
        {
            MessageBox.Show( UIErrorMessages.ONLINE_PREREGISTRATION_ENTRY_UNAVAILABLE, "Error",
                             MessageBoxButtons.OK, MessageBoxIcon.Error,
                             MessageBoxDefaultButton.Button1 );
        }

        private void ResetButtonClickEvent( object sender, EventArgs e )
        {
            Presenter.ResetButtonClickEvent( sender, e );
        }

        private void ShowPreRegistrationSubmissionsRequestEvent( object sender, EventArgs e )
        {
            Presenter.ShowPreRegistrationSubmissionsRequestEvent( sender, e );
        }

        private void PrintReportEvent()
        {
            Presenter.PrintReportEvent();
        }

        public void ClearNoOnlineSubmissionsText()
        {
            labelNoOnlineSubmissions.Text = string.Empty;
        }

        private static void OnCellToolTipTextNeeded( object sender, DataGridViewCellToolTipTextNeededEventArgs e )
        {
            var grid = (DataGridView)sender;
            if ( e.RowIndex > -1 && e.ColumnIndex > -1 )
            {
                var row = grid.Rows[e.RowIndex];

                var cell = row.Cells[e.ColumnIndex];
                //we do not want to show anything in the tool tip when the cell type is something other than text e.g. image
                if(cell is DataGridViewTextBoxCell && cell.FormattedValue != null)
                {
                    var cellValue = cell.FormattedValue.ToString();
                    e.ToolTipText = cellValue;
                }
            }
        }

        #endregion

        private static void SetVisibilityFor( ControlCollection controls, bool isVisible )
        {
            foreach ( Control c in controls )
            {
                c.Visible = isVisible;
            }
        }

        public void SetDefaultButtonState()
        {
            ShowNewPatientAndAccountButtons();
            SetNotEnabledButtonState();
        }

        public void ShowNewPatientAndAccountButtons()
        {
            ShowCreateNewAccountButton();
            ShowCreateNewPatientButton();
        }

        public void HideNewPatientAndAccountButtons()
        {
            HideCreateNewAccountButton();
            HideCreateNewPatientButton();

            SetNotEnabledButtonState();
        }

        public void ShowCreateNewPatientButton()
        {
            btnCreateNewPatient.Visible = true;
        }

        public void HideCreateNewPatientButton()
        {
            btnCreateNewPatient.Visible = false;
        }

        public void ShowCreateNewAccountButton()
        {
            btnCreateNewAccount.Visible = true;
        }

        public void HideCreateNewAccountButton()
        {
            btnCreateNewAccount.Visible = false;
        }

        public void SetNotEnabledButtonState()
        {
            DisableCreateNewAccountButton();
            DisableCreateNewPatientButton();
        }

        public void EnableCreateNewPatientButton()
        {
            btnCreateNewPatient.Enabled = true;
        }

        public void EnableCreateNewAccountButton()
        {
            btnCreateNewAccount.Enabled = true;
        }

        public void DisableCreateNewPatientButton()
        {
            btnCreateNewPatient.Enabled = false;
        }

        public void DisableCreateNewAccountButton()
        {
            btnCreateNewAccount.Enabled = false;
        }

        private void UpdateSelectedOnlinePreRegItem()
        {
            if ( onlinePreRegDataGrid.SelectedRows.Count > 0 )
            {
                DataRowView row = ( DataRowView )onlinePreRegDataGrid.SelectedRows[0].DataBoundItem;
                selectedOnlinePreRegItem = ( OnlinePreRegistrationItem )row.Row[DUMMY_COLUMN];
                Presenter.SelectSubmissionItem = selectedOnlinePreRegItem;
                Presenter.FindMatchingPatients( selectedOnlinePreRegItem );
            }
        }

        #region Methods

        public void ShowProgressPanel()
        {
            progressPanel.Visible = true;
            progressPanel.BringToFront();
        }

        public void HideProgressPanel()
        {
            progressPanel.Visible = false;
            progressPanel.SendToBack();
        }

        public void ShowPatientSearchProgressPanel()
        {
            progressPanelPatientSearch.Visible = true;
            progressPanelPatientSearch.BringToFront();
        }

        public void HidePatientSearchProgressPanel()
        {
            progressPanelPatientSearch.Visible = false;
            progressPanelPatientSearch.SendToBack();
        }

        private void ShowNoAccessPanel()
        {
            noAccessPanel.Visible = true;
            noAccessPanel.BringToFront();
        }

        public void HideNoAccessPanel()
        {
            noAccessPanel.Visible = false;
            noAccessPanel.SendToBack();
        }

        public void ShowNullFacilityMessage()
        {
            MessageBox.Show( "FacilityBroker returned null Facility object", "Error",
                             MessageBoxButtons.OK, MessageBoxIcon.Error,
                             MessageBoxDefaultButton.Button1 );
        }

        public bool ViewDisposed()
        {
            return IsDisposed || Disposing;
        }

        public void ShowRemotingTimeOutMesssage()
        {
            MessageBox.Show( UIErrorMessages.TIMEOUT_WORKLIST_DISPLAY );
        }

        public void ShowNoOnlineSubmissionsMessage()
        {
            labelNoOnlineSubmissions.Text = PreRegistrationSubmissionsMessage;
            labelNoOnlineSubmissions.Visible = true;
            labelNoOnlineSubmissions.BorderStyle = BorderStyle.FixedSingle;
            labelNoOnlineSubmissions.BringToFront();

            OnlinePreRegDataGrid.DataSource = null;
            MatchingPatientsDataGrid.DataSource = null;
        }

        public void HideNoOnlineSubmissionsMessage()
        {
            labelNoOnlineSubmissions.Text = string.Empty;
            labelNoOnlineSubmissions.Visible = false;
            labelNoOnlineSubmissions.SendToBack();
        }

        public void ShowNoMatchingPatientsMessage()
        {
            lblNoMatchingPatients.Text = NoMatchingPatientsMessage;
            lblNoMatchingPatients.Visible = true;
            lblNoMatchingPatients.BorderStyle = BorderStyle.FixedSingle;
            lblNoMatchingPatients.BringToFront();
        }

        public void HideNoMatchingPatientsMessage()
        {
            lblNoMatchingPatients.Text = string.Empty;
            lblNoMatchingPatients.Visible = false;
            lblNoMatchingPatients.SendToBack();
        }

        public void ShowNoAccessMessage()
        {
            labelNoAccess.Text = NoAccessMessage;
            labelNoAccess.Visible = true;
            labelNoAccess.BringToFront();
        }

        public void ShowDefaultCursor()
        {
            Cursor = Cursors.Default;
        }

        public void ShowWaitCursor()
        {
            Cursor = Cursors.WaitCursor;
        }

        public void ShowAppStartingCursor()
        {
            Cursor = Cursors.AppStarting;
        }

        public void ShowPatientSearchTimeOutMessage()
        {
            MessageBox.Show( UIErrorMessages.PATIENT_SEARCH_TIMEOUT_MSG );
        }

        public void RefreshPreRegistrationSubmissionsList( Object state )
        {
            OnlinePreRegistrationSubmissionsPresenter preRegPresenter = ( OnlinePreRegistrationSubmissionsPresenter )state;
            OnlinePreRegistrationSubmissionsView view = ( OnlinePreRegistrationSubmissionsView )preRegPresenter.View;
            if ( view != null && view.Visible )
            {
                view.Invoke( new PopulatePreRegistrationSubmissionsDelegate( Presenter.PopulatePreRegistrationSubmissions ) );
            }
        }

        public void SetNewPatientButtonState()
        {
            btnCreateNewPatient.Enabled = OnlinePreRegDataGrid.Rows.Count > 0;
        }

        public void SetNewAccountButtonState()
        {
            btnCreateNewAccount.Enabled = MatchingPatientsDataGrid.Rows.Count > 0;
        }

        #endregion

        #region Properties

        private Facility Facility
        {
            get
            {
                return User.GetCurrent().Facility;
            }
        }

        private FilterEnterDataGridView OnlinePreRegDataGrid
        {
            get
            {
                return onlinePreRegDataGrid;
            }
        }

        private FilterEnterDataGridView MatchingPatientsDataGrid
        {
            get
            {
                return matchingPatientsDataGrid;
            }
        }

        public FilterWorklistView FilterWorklistView
        {
            get { return i_FilterWorklistView ?? ( i_FilterWorklistView = new FilterWorklistView() ); }
            set
            {
                i_FilterWorklistView = value;
            }
        }

        private string PreRegistrationSubmissionsMessage
        {
            get
            {
                return i_PreRegistrationSubmissionsMessage;
            }
        }

        private string NoMatchingPatientsMessage
        {
            get
            {
                return i_noMatchingPatientsMessage;
            }
        }

        private string NoAccessMessage
        {
            get
            {
                return i_NoAccessMessage;
            }
        }

        private string PBARErrorMessage
        {
            get
            {
                return i_pbarErrorMessage;
            }
        }

        public IOnlinePreRegistrationSubmissionsPresenter Presenter
        {
            get
            {
                return presenter;
            }
            set
            {
                presenter = value;
            }
        }

        #endregion

        #region Private Properties
        #endregion

        #region Data Elements

        private delegate void PopulatePreRegistrationSubmissionsDelegate();

        private OnlinePreRegistrationItem selectedOnlinePreRegItem;
        private IOnlinePreRegistrationSubmissionsPresenter presenter;

        readonly DataTable onlinePreRegDataTable = new DataTable();

        readonly DataColumn isLockedDataColumn = new DataColumn( IS_LOCKED, typeof( Bitmap ) );

        readonly DataColumn patientNameDataColumn = new DataColumn( PATIENT_NAME, typeof( string ) );
        readonly DataColumn visitedBeforeDataColumn = new DataColumn( VISITED_BEFORE, typeof( string ) );
        readonly DataColumn genderDataColumn = new DataColumn( GENDER, typeof( string ) );
        readonly DataColumn dobDataColumn = new DataColumn( DATE_OF_BIRTH, typeof( DateTime ) );
        readonly DataColumn ssnDataColumn = new DataColumn( SSN, typeof( string ) );
        readonly DataColumn addressDataColumn = new DataColumn( ADDRESS, typeof( string ) );
        readonly DataColumn admitDateDataColumn = new DataColumn( ADMIT_DATE, typeof( DateTime ) );
        readonly DataColumn dummyDataColumn = new DataColumn( DUMMY_COLUMN, typeof( OnlinePreRegistrationItem ) );

        readonly DataTable matchingPatientsDataTable = new DataTable();

        readonly DataColumn matchingNameDataColumn = new DataColumn( MATCHING_PATIENT_NAME, typeof( string ) );
        readonly DataColumn matchingGenderDataColumn = new DataColumn( MATCHING_GENDER, typeof( string ) );
        readonly DataColumn matchingDobDataColumn = new DataColumn( MATCHING_DATE_OF_BIRTH, typeof( DateTime ) );
        readonly DataColumn matchingSsnDataColumn = new DataColumn( MATCHING_SSN, typeof( string ) );
        readonly DataColumn matchingAddressDataColumn = new DataColumn( MATCHING_ADDRESS, typeof( string ) );
        readonly DataColumn matchingDummyDataColumn = new DataColumn( MATCHING_DUMMY_COLUMN, typeof( PatientSearchResult ) );

        #endregion

        #region Constants

        private const string i_pbarErrorMessage = "This activity cannot proceed because the system is unavailable.  Please try again later.";
        private const string i_PreRegistrationSubmissionsMessage = "No submissions were found based on the applied filter settings.";
        private const string i_noMatchingPatientsMessage = "No matches to display.";
        private const string i_NoAccessMessage = "This screen is unavailable because you do not have permission to view this information.";

        private const string
            IS_LOCKED = "IsLocked",
            PATIENT_NAME = "PatientName",
            VISITED_BEFORE = "VisitedBefore",
            GENDER = "Gender",
            DATE_OF_BIRTH = "DateOfBirth",
            SSN = "Ssn",
            ADDRESS = "Address",
            ADMIT_DATE = "AdmitDate",
            DUMMY_COLUMN = "DummyColumn";

        private const string
            MATCHING_PATIENT_NAME = "MatchingPatientName",
            MATCHING_GENDER = "MatchingGender",
            MATCHING_DATE_OF_BIRTH = "MatchingDateOfBirth",
            MATCHING_SSN = "MatchingSsn",
            MATCHING_ADDRESS = "MatchingAddress",
            MATCHING_DUMMY_COLUMN = "MatchingDummyColumn";


        #endregion
    }
}
