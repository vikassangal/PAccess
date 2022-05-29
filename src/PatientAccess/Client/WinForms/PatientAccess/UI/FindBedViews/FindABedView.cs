using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.FindBedViews
{
	/// <summary>
    /// Find A Bed View
	/// </summary>
	[Serializable]
	public class FindABedView : ControlView
	{
        #region Windows Form Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bedSearchViewPanel = new System.Windows.Forms.Panel();
            this.bedSearchView = new PatientAccess.UI.FindBedViews.BedSearchView();
            this.bedResultViewPanel = new System.Windows.Forms.Panel();
            this.bedResultsView = new PatientAccess.UI.FindBedViews.BedResultsView();
            this.accomodationLabel = new System.Windows.Forms.Label();
            this.accomodationComboBox = new System.Windows.Forms.ComboBox();
            this.btnOK = new ClickOnceLoggingButton();
            this.btnCancel = new ClickOnceLoggingButton();
            this.bedSearchViewPanel.SuspendLayout();
            this.bedResultViewPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // bedSearchViewPanel
            // 
            this.bedSearchViewPanel.Controls.Add(this.bedSearchView);
            this.bedSearchViewPanel.Location = new System.Drawing.Point(7, 0);
            this.bedSearchViewPanel.Name = "bedSearchViewPanel";
            this.bedSearchViewPanel.Size = new System.Drawing.Size(891, 80);
            this.bedSearchViewPanel.TabIndex = 0;
            // 
            // bedSearchView
            // 
            this.bedSearchView.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.bedSearchView.Location = new System.Drawing.Point(0, 0);
            this.bedSearchView.Model = null;
            this.bedSearchView.Name = "bedSearchView";
            this.bedSearchView.Size = new System.Drawing.Size(891, 80);
            this.bedSearchView.TabIndex = 0;
            this.bedSearchView.LocationsFound += 
                new System.EventHandler(this.LocationsFound);
            this.bedSearchView.LocationsNotFound += 
                new System.EventHandler(this.LocationsNotFound);
            this.bedSearchView.ResetSearch += 
                new System.EventHandler(this.ResetSearch);
            // 
            // bedResultViewPanel
            // 
            this.bedResultViewPanel.Controls.Add(this.bedResultsView);
            this.bedResultViewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bedResultViewPanel.Location = new System.Drawing.Point(7, 80);
            this.bedResultViewPanel.Name = "bedResultViewPanel";
            this.bedResultViewPanel.Size = new System.Drawing.Size(891, 320);
            this.bedResultViewPanel.TabIndex = 1;
            this.bedResultViewPanel.TabStop = true;
            // 
            // accomodationLabel
            // 
            this.accomodationLabel.Location = new System.Drawing.Point(701, 410);
            this.accomodationLabel.Name = "accomodationLabel";
            this.accomodationLabel.Size = new System.Drawing.Size(88, 14);
            this.accomodationLabel.TabIndex = 0;
            this.accomodationLabel.Text = "Accommodation:";
            // 
            // accomodationComboBox
            // 
            this.accomodationComboBox.Enabled = false;
            this.accomodationComboBox.Location = new System.Drawing.Point(788, 407);
            this.accomodationComboBox.Name = "accomodationComboBox";
            this.accomodationComboBox.Size = new System.Drawing.Size(110, 21);
            this.accomodationComboBox.TabIndex = 2;
            this.accomodationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            // 
            // btnOK
            // 
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(741, 435);
            this.btnOK.Name = "btnOK";
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(823, 435);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            // bedResultsView
            // 
            this.bedResultsView.BackColor = System.Drawing.Color.White;
            this.bedResultsView.Location = new System.Drawing.Point(0, 0);
            this.bedResultsView.Model = null;
            this.bedResultsView.Name = "bedResultsView";
            this.bedResultsView.Size = new System.Drawing.Size(891, 320);
            this.bedResultsView.TabIndex = 1;
            // 
            // 
            // FindABedView
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.accomodationComboBox);
            this.Controls.Add(this.accomodationLabel);
            this.Controls.Add(this.bedResultViewPanel);
            this.Controls.Add(this.bedSearchViewPanel);
            this.Name = "FindABedView";
            this.Size = new System.Drawing.Size(905, 465);
            this.AcceptButton = this.btnOK;
            this.bedSearchViewPanel.ResumeLayout(false);
            this.bedResultViewPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion
        
        #region Events

        public event EventHandler CloseDialog;

        #endregion

        #region Event Handlers

        /// <summary>
        /// Fired on OK Button Clicked Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click( object sender, EventArgs e )
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                LocationBrokerProxy locationBroker = null;
                string bedStr = string.Empty;
                string roomStr = string.Empty;
                string nursingStationStr = string.Empty;
                string[] locationElements = {string.Empty, string.Empty, string.Empty};

                Bed bed = null;
                Room room = null;
                NursingStation nursingStation = null;
                Location newLocation = null;

                locationElements    = location.Split( '-' );
                nursingStationStr   = locationElements[0].Trim();
                roomStr             = locationElements[1];
                bedStr              = locationElements[2];

                if ( nursingStationStr.Length == 1 )
                {
                    nursingStationStr = BLANKSPACE + nursingStationStr;
                }
                nursingStation = new NursingStation( ReferenceValue.NEW_OID, 
                    ReferenceValue.NEW_VERSION,
                    string.Empty, nursingStationStr );
            
                room = new Room( ReferenceValue.NEW_OID, 
                    ReferenceValue.NEW_VERSION, 
                    string.Empty, roomStr );
            
                bed = new Bed( ReferenceValue.NEW_OID, 
                    ReferenceValue.NEW_VERSION,
                    string.Empty, bedStr );
            
                newLocation = new Location( ReferenceValue.NEW_OID, 
                    ReferenceValue.NEW_VERSION,
                    string.Empty, string.Empty, 
                    nursingStation, room, bed );           

                ReservationCriteria reservationCriteria = 
                    new ReservationCriteria();

                reservationCriteria.NewLocation = newLocation; 
                reservationCriteria.OldLocation = Model.Location;
                reservationCriteria.Facility = User.GetCurrent().Facility;
                reservationCriteria.PatientType = Model.KindOfVisit;
                
                locationBroker = new LocationBrokerProxy( );
                ReservationResults reservationResult = 
                    ( ReservationResults )locationBroker.Reserve( reservationCriteria );

                MessageBox.Show( reservationResult.Message,
                                "Information",
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Information,
                                MessageBoxDefaultButton.Button1 );

                if ( reservationResult.ReservationSucceeded )
                {   
                    //Raise bed lock event to patientAccess view
                    ActivityEventAggregator.GetInstance().RaiseBedLockEvent( this, 
                        new LooseArgs( reservationResult.Location ) );

                    if ( Model.KindOfVisit.Code == VisitType.INPATIENT &&
                         accomodationComboBox.SelectedIndex >= 0 )
                    {
                        newLocation.Bed.Accomodation = 
                            (Accomodation)accomodationsCodes[accomodationComboBox.SelectedIndex];
                    }
                    NewLocation = newLocation;

                    //DialogResult doesn't seem to work as expected 
                    //with OK button click and Grid Row Double Click.
                    //So, closing the dialog with CloseDialog Event.
                    //this.btnOK.DialogResult = DialogResult.OK;
                    if( CloseDialog != null )
                    {
                        CloseDialog( sender, null );
                    }
                }
                else
                {
                    NewLocation = null;
                    //this.btnOK.DialogResult = DialogResult.None;
                }
            }
            finally
            {
                this.btnOK.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Fired on Location Found Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LocationsFound( object sender, EventArgs e )
        {
            bedResultsView.Model = ( ArrayList )( ( LooseArgs )e ).Context;
            bedResultsView.UpdateView();
        }

        /// <summary>
        /// Called on Reset Button Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetSearch( object sender, EventArgs e )
        {
            this.ClearAccomodationCodes();
            this.bedResultsView.Model = null;
            this.bedResultsView.ResetView();
            this.btnOK.Enabled = false;
        }

        /// <summary>
        /// Fired on LocationNotFound event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LocationsNotFound( object sender, EventArgs e )
        {
            this.bedResultsView.LocationsNotFound();
            this.ClearAccomodationCodes();
        }

        /// <summary>
        /// Fired on Location Selection Changed Event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LocationSelectionChanged( object sender, EventArgs e )
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                accomodationComboBox.Items.Clear();
                if( e == null )
                {
                    location = null;
                }
                else
                {
                    location = Convert.ToString( ( (LooseArgs)e ).Context );
                }
                if ( location == null )
                {
                    ClearAccomodationCodes();
                    this.btnOK.Enabled = false;
                    return;
                }
                if ( Model.KindOfVisit.Code == VisitType.INPATIENT )
                {
                    FetchAccomodationCodes( location );
                }
                else // for other patient types, no need for Accomodation Codes
                {
                    accomodationComboBox.Enabled = false;
                    this.btnOK.Enabled = true;
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Methods

        #endregion

        #region Properties
       
        public Location NewLocation
        {
            get
            {
                return i_NewLocation;
            }
            set
            {
                i_NewLocation = value;
            }
        }

        public new Account Model
        {
            private get
            {
                return i_Account;
            }
            set
            {
                i_Account = value;
                bedSearchView.Model = value;
            }
        }

        #endregion

        #region Private Methods

        private void ClearAccomodationCodes()
        {
            accomodationComboBox.Items.Clear();
            accomodationComboBox.Items.Add( BLANKSPACE );
            accomodationComboBox.SelectedItem = BLANKSPACE;
            accomodationComboBox.Enabled = false;
        }

        private void FetchAccomodationCodes( string location )
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                string nursingStationStr = string.Empty;
                string[] locationInfo;
                LocationBrokerProxy locationBroker = null;

                locationInfo = location.Split('-');
                nursingStationStr = locationInfo[0];
                if( nursingStationStr.Length == 1 )
                {
                    nursingStationStr = BLANKSPACE + nursingStationStr;
                }
                locationBroker =  new LocationBrokerProxy( );

                accomodationsCodes = 
                    ( ArrayList )locationBroker.AccomodationCodesFor( nursingStationStr, 
                    User.GetCurrent().Facility );

                foreach( Accomodation accomodation in accomodationsCodes )
                {
                    accomodationComboBox.Items.Add( accomodation.Code.Trim() + 
                                                    BLANKSPACE + 
                                                    accomodation.Description.Trim() );
                }
                if ( accomodationsCodes.Count <= 1 )
                {
                    ClearAccomodationCodes();
                    btnOK.Enabled = false;
                    MessageBox.Show( NO_ACCOMODATION_CODES_FOUND_MESSAGE, "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1 );
                }
                else
                {
                    btnOK.Enabled = true;
                    accomodationComboBox.Enabled = true;
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Wire Up Find A Bed View
        /// </summary>
        private void WireupFindABedView()
        {
            bedResultsView.LocationSelectionChanged   += 
                new EventHandler( this.LocationSelectionChanged );
            bedResultsView.LocationSelected +=
                new EventHandler( this.btnOK_Click );
        }

        #endregion

        #region Private Properties

        #endregion

        #region Construction and Finalization
        public FindABedView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            WireupFindABedView();

            base.EnableThemesOn( this );
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #endregion

        #region Data Elements
        
        private Panel bedSearchViewPanel;
        private Panel bedResultViewPanel;
        private BedSearchView bedSearchView;
        private BedResultsView bedResultsView;
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private Label accomodationLabel;
        private ComboBox accomodationComboBox;
        private ClickOnceLoggingButton btnOK;
        private ClickOnceLoggingButton btnCancel;
        private Account i_Account = null;
        private ArrayList accomodationsCodes = null;
        private string location = string.Empty;
        private Location i_NewLocation = null;

        #endregion

        #region Constants
        private const string 
            NO_ACCOMODATION_CODES_FOUND_MESSAGE = "The requested bed cannot be assigned to an inpatient, because it does not have any associated accommodation codes.";
        private const string BLANKSPACE = " ";

        #endregion
    }
}