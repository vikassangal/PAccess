using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.FindBedViews
{
    /// <summary>
    /// Bed Search View
    /// </summary>
    [Serializable]
    public class BedSearchView : ControlView
    {
        
        #region Event Handlers

        /// <summary>
        /// Called When Reset Button is Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButtonClick( object sender, EventArgs e )
        {
            unOccupiedRadioButton.Checked = true;
            bedComboBox.SelectedIndex = 0;
            hideRoomCheckBox.Checked = false;
            if( ResetSearch != null )
            {
                ResetSearch( this, null );
            }
        }

        /// <summary>
        /// Called when Show button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowButtonClick( object sender, EventArgs e )
        {
            this.showButton.Message = "Retrieving beds for find a bed";
            SearchData();
        }

        /// <summary>
        /// Fired on All Radio Butto changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void allRadioButton_CheckedChanged( object sender, EventArgs e )
        {
            hideRoomCheckBox.Checked = !allRadioButton.Checked;
            hideRoomCheckBox.Enabled = !allRadioButton.Checked;
        }

        /// <summary>
        /// Fired on Unoccupied Radio Button changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void unOccupiedRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            hideRoomCheckBox.Checked = !unOccupiedRadioButton.Checked;
            hideRoomCheckBox.Enabled = unOccupiedRadioButton.Checked;
        }

        /// <summary>
        /// This method is called whenever there is a change in Nursing Station
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bedComboBox_SelectedIndexChanged( object sender, EventArgs e )
        {
            PopulateRoomComboBox();
        }

        #endregion

        #region Events

        public event EventHandler LocationsFound; 
        public event EventHandler ResetSearch;
        public event EventHandler LocationsNotFound;
        
        #endregion

        #region Methods

        #endregion

        #region Properties

        public new Account Model
        {
            private get
            {
                return i_Account;
            }
            set
            {
                i_Account = value;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Called when Show button is clicked to fetch locations
        /// </summary>
        private void SearchData()
        {
            bool hideRoom;
            bool unoccupiedBeds;
            string genderId = String.Empty;
            try
            {
                this.Cursor = Cursors.WaitCursor;

                user = User.GetCurrent();
                nursingStationCode = bedComboBox.SelectedItem.ToString();

                if( roomComboBox.Items.Count > 0 )
                {
                    roomCode = roomComboBox.SelectedItem.ToString();
                }
                else
                {
                    roomCode = null;
                }

                unoccupiedBeds = unOccupiedRadioButton.Checked;
                hideRoom = hideRoomCheckBox.Checked;

                if( unoccupiedBeds )
                {
                    if( hideRoom )
                    {
                        genderId = Model.Patient.Sex.Code.ToUpper();
                    }
                    else
                    {
                        genderId = null;
                    }
                }
                else
                {
                    genderId = null;
                }
                LocationSearchCriteria locationSearchCriteria =
                    new LocationSearchCriteria(
                    Convert.ToString( User.GetCurrent().Facility.Code ),
                                      genderId,
                                      nursingStationCode,
                                      roomCode,
                                      unoccupiedBeds );

                LocationBrokerProxy locationBroker = new LocationBrokerProxy();

                ICollection accountProxiesCollection = locationBroker.LocationMatching( locationSearchCriteria );

                if( accountProxiesCollection != null )
                {
                    if( accountProxiesCollection.Count != 0 && LocationsFound != null )
                    {
                        LocationsFound( this, new LooseArgs( accountProxiesCollection ) );
                    }
                    else
                    {
                        if( LocationsNotFound != null )
                        {
                            LocationsNotFound( this, new LooseArgs( this ) );
                        }
                    }
                }
            }
            catch( RemotingTimeoutException )
            {
                MessageBox.Show( UIErrorMessages.FIND_BED_TIMEOUT_MSG );
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
 
        /// <summary>
        /// Populate Room Combo Box called, when Nursing Station is selected
        /// </summary>
        private void PopulateRoomComboBox()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                roomComboBox.Items.Clear();
                nursingStationCode = bedComboBox.SelectedItem.ToString();

                if ( nursingStationCode.Equals( ALL_NURSING_STATIONS ) )
                {
                    roomComboBox.Items.Clear();
                    roomComboBox.Items.Add( ALL_ROOMS );
                    roomComboBox.SelectedItem = ALL_ROOMS;  
                    roomComboBox.Enabled = false;
                    return;
                }
                else
                {
                    roomComboBox.Enabled = true;
                }

                user = User.GetCurrent();
                facility = User.GetCurrent().Facility;
                LocationBrokerProxy locationBroker = new LocationBrokerProxy( );
                    
                rooms = (ArrayList)locationBroker.RoomsFor( facility, nursingStationCode );
                roomComboBox.Items.Add( ALL_ROOMS );
                roomComboBox.SelectedItem = ALL_ROOMS;  

                foreach( Room room in rooms )
                {
                    roomComboBox.Items.Add( room.Code );
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        #endregion

        #region Private Properties

        #endregion

        #region Construction and Finalization

        /// <summary>
        /// Constructor
        /// </summary>
        public BedSearchView()
        {
            this.Cursor = Cursors.WaitCursor;

            InitializeComponent();
            this.showButton.Message = "Retrieving beds for find a bed ";

            EnableThemesOn( this );
            
            user = User.GetCurrent();
            facility = User.GetCurrent().Facility;
            var locationPBARBroker =  new LocationBrokerProxy( );
                
            nursingStations = 
                locationPBARBroker.NursingStationsFor( facility );
            bedComboBox.Items.Add( ALL_NURSING_STATIONS );
            bedComboBox.SelectedItem = ALL_NURSING_STATIONS;  

            roomComboBox.Items.Add( ALL_ROOMS );
            roomComboBox.SelectedItem = ALL_ROOMS;  
            roomComboBox.Enabled = false;
            unOccupiedRadioButton.Checked = true;
            hideRoomCheckBox.Checked = false;

            foreach( NursingStation nursingStation in nursingStations )
            {
                if( !nursingStation.Code.Trim().Equals( ALL_NURSINGSTATION_CODE ) )
                {                        
                    bedComboBox.Items.Add( nursingStation.Code );
                }
            }
        }
        
        #endregion

        #region Windows Form Designer generated code

        /// <summary>
        /// Initialize Method
        /// </summary>
        private void InitializeComponent()
        {
            this.bedSearchpanel = new System.Windows.Forms.Panel();
            this.hideRoomCheckBox = new System.Windows.Forms.CheckBox();
            this.roomComboBox = new System.Windows.Forms.ComboBox();
            this.roomLabel = new System.Windows.Forms.Label();
            this.resetButton = new LoggingButton();
            this.showButton = new LoggingButton();
            this.bedComboBox = new System.Windows.Forms.ComboBox();
            this.bedLabel = new System.Windows.Forms.Label();
            this.unOccupiedRadioButton = new System.Windows.Forms.RadioButton();
            this.bedTypeLabel = new System.Windows.Forms.Label();
            this.allRadioButton = new System.Windows.Forms.RadioButton();
            this.bedTypePanel = new System.Windows.Forms.Panel();
            this.bedSearchpanel.SuspendLayout();
            this.bedTypePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // bedSearchpanel
            // 
            this.bedSearchpanel.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.bedSearchpanel.Controls.Add(this.bedTypePanel);
            this.bedSearchpanel.Controls.Add(this.hideRoomCheckBox);
            this.bedSearchpanel.Controls.Add(this.roomComboBox);
            this.bedSearchpanel.Controls.Add(this.roomLabel);
            this.bedSearchpanel.Controls.Add(this.resetButton);
            this.bedSearchpanel.Controls.Add(this.showButton);
            this.bedSearchpanel.Controls.Add(this.bedComboBox);
            this.bedSearchpanel.Controls.Add(this.bedLabel);
            this.bedSearchpanel.Location = new System.Drawing.Point(0, 0);
            this.bedSearchpanel.Name = "bedSearchpanel";
            this.bedSearchpanel.Size = new System.Drawing.Size(891, 80);
            this.bedSearchpanel.TabIndex = 0;
            // 
            // hideRoomCheckBox
            // 
            this.hideRoomCheckBox.Location = new System.Drawing.Point(170, 40);
            this.hideRoomCheckBox.Name = "hideRoomCheckBox";
            this.hideRoomCheckBox.Size = new System.Drawing.Size(232, 16);
            this.hideRoomCheckBox.TabIndex = 1;
            this.hideRoomCheckBox.Text = "Hide rooms occupied by opposite gender";
            // 
            // roomComboBox
            // 
            this.roomComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.roomComboBox.Location = new System.Drawing.Point(613, 16);
            this.roomComboBox.Name = "roomComboBox";
            this.roomComboBox.Size = new System.Drawing.Size(48, 21);
            this.roomComboBox.TabIndex = 3;
            // 
            // roomLabel
            // 
            this.roomLabel.Location = new System.Drawing.Point(562, 19);
            this.roomLabel.Name = "roomLabel";
            this.roomLabel.Size = new System.Drawing.Size(46, 16);
            this.roomLabel.TabIndex = 3;
            this.roomLabel.Text = "In room:";
            // 
            // resetButton
            // 
            this.resetButton.BackColor = System.Drawing.SystemColors.Control;
            this.resetButton.Location = new System.Drawing.Point(770, 16);
            this.resetButton.Name = "resetButton";
            this.resetButton.TabIndex = 5;
            this.resetButton.Text = "Rese&t";
            this.resetButton.Click += new System.EventHandler(this.ResetButtonClick);
            // 
            // showButton
            // 
            this.showButton.BackColor = System.Drawing.SystemColors.Control;
            this.showButton.Location = new System.Drawing.Point(688, 16);
            this.showButton.Name = "showButton";
            this.showButton.TabIndex = 4;
            this.showButton.Text = "Sh&ow";
            this.showButton.Click += new System.EventHandler(this.ShowButtonClick);
            // 
            // bedComboBox
            // 
            this.bedComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.bedComboBox.Location = new System.Drawing.Point(496, 16);
            this.bedComboBox.Name = "bedComboBox";
            this.bedComboBox.Size = new System.Drawing.Size(48, 21);
            this.bedComboBox.TabIndex = 2;
            this.bedComboBox.SelectedIndexChanged += new System.EventHandler(this.bedComboBox_SelectedIndexChanged);
            // 
            // bedLabel
            // 
            this.bedLabel.Location = new System.Drawing.Point(400, 19);
            this.bedLabel.Name = "bedLabel";
            this.bedLabel.Size = new System.Drawing.Size(93, 16);
            this.bedLabel.TabIndex = 2;
            this.bedLabel.Text = "In nursing station:";
            // 
            // unOccupiedRadioButton
            // 
            this.unOccupiedRadioButton.Location = new System.Drawing.Point(137, 0);
            this.unOccupiedRadioButton.Name = "unOccupiedRadioButton";
            this.unOccupiedRadioButton.Size = new System.Drawing.Size(254, 16);
            this.unOccupiedRadioButton.TabIndex = 0;
            this.unOccupiedRadioButton.Text = "Unoccupied (includes occupied in same room)";
            this.unOccupiedRadioButton.CheckedChanged += new System.EventHandler(this.unOccupiedRadioButton_CheckedChanged);
            // 
            // bedTypeLabel
            // 
            this.bedTypeLabel.Location = new System.Drawing.Point(0, 0);
            this.bedTypeLabel.Name = "bedTypeLabel";
            this.bedTypeLabel.Size = new System.Drawing.Size(82, 16);
            this.bedTypeLabel.TabIndex = 0;
            this.bedTypeLabel.Text = "Show bed type:";
            this.bedTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // allRadioButton
            // 
            this.allRadioButton.Checked = true;
            this.allRadioButton.Location = new System.Drawing.Point(89, 0);
            this.allRadioButton.Name = "allRadioButton";
            this.allRadioButton.Size = new System.Drawing.Size(40, 16);
            this.allRadioButton.TabIndex = 0;
            this.allRadioButton.TabStop = true;
            this.allRadioButton.Text = "All";
            this.allRadioButton.CheckedChanged += new System.EventHandler(this.allRadioButton_CheckedChanged);
            // 
            // bedTypePanel
            // 
            this.bedTypePanel.Controls.Add(this.bedTypeLabel);
            this.bedTypePanel.Controls.Add(this.allRadioButton);
            this.bedTypePanel.Controls.Add(this.unOccupiedRadioButton);
            this.bedTypePanel.Location = new System.Drawing.Point(7, 19);
            this.bedTypePanel.Name = "bedTypePanel";
            this.bedTypePanel.Size = new System.Drawing.Size(393, 20);
            this.bedTypePanel.TabIndex = 0;
            // 
            // BedSearchView
            // 
            this.AcceptButton = this.showButton;
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.Controls.Add(this.bedSearchpanel);
            this.Name = "BedSearchView";
            this.Size = new System.Drawing.Size(891, 80);
            this.bedSearchpanel.ResumeLayout(false);
            this.bedTypePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Data Elements

        private string nursingStationCode = "";
        private string roomCode = "";
        private User user = null;
        private ArrayList rooms = null;
        private Account i_Account = null;
        private Facility facility = null;
        private IList<NursingStation> nursingStations = null;
        private Label bedLabel = null;
        private Label roomLabel = null;
        private LoggingButton showButton = null;
        private LoggingButton resetButton = null;
        private Label bedTypeLabel = null;
        private Panel bedSearchpanel = null;
        private Panel bedTypePanel = null;
        private ComboBox bedComboBox = null;
        private ComboBox roomComboBox = null;
        private CheckBox hideRoomCheckBox = null;
        private RadioButton allRadioButton = null;
        private RadioButton unOccupiedRadioButton = null;
        
        #endregion

        #region Constants
        
        private const string 
            ALL_ROOMS               = "All",
            ALL_NURSING_STATIONS    = "All",
            ALL_NURSINGSTATION_CODE = "$$";

        #endregion
    }
}