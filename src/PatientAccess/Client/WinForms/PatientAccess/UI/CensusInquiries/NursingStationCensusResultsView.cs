using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
    /// Census by Nursing Station result view
    /// </summary>
    [Serializable]
    public class NursingStationCensusResultsView : ControlView
    {
        #region Events
        #endregion

        #region Event Handlers

        private void NursingStationCensusResultsView_Load(object sender, EventArgs e)
        {
            this.progressPanel1.Visible = false;
        }

        private void GridControlClick(UltraGridRow ultraGridRow)
        {
            previousSelectedLocation = 
                Convert.ToString( ultraGridRow.Cells[GRIDCOL_LOCATION].Value ).Trim();
        }

        #endregion

        #region Methods

        public void BeforeWork(object sender, EventArgs e)
        {
            this.progressPanel1.Visible = true;
            this.progressPanel1.BringToFront();
            this.nsCensusResultsViewPanel.Visible = false;
        }

        public void AfterWork(object sender, EventArgs e)
        {
            if (this.IsDisposed || this.Disposing)
                return;
            this.progressPanel1.Visible = false;
            this.progressPanel1.SendToBack();
            this.nsCensusResultsViewPanel.Visible = true;
        }

        public override void UpdateView()
        {
            this.nsResultGridControl.Visible = true;

            FillDataSource();

            this.noAccountsFoundLabel.Visible = false;    
            nsResultGridControl.Focus();
        }

        public void NoAccountsFound()
        {
            this.nsCensusResultsViewPanel.Visible = true;
            this.noAccountsFoundLabel.Visible = true;
            this.previousSelectedLocation = String.Empty;
            this.nsResultGridControl.Hide();
            this.nsCensusResultsViewPanel.Visible = true;
        }

        public void ResetView()
        {
            this.noAccountsFoundLabel.Visible = false;
            this.nsResultGridControl.Visible = true;
            this.dataSource.Rows.Clear();
            this.previousSelectedLocation = String.Empty;
        }
        public void SetRowSelectionActiveAppearance()
        {
            nsResultGridControl.SetRowSelectionActiveAppearance();
        }
        public void SetRowSelectionInActiveAppearance()
        {
            nsResultGridControl.SetRowSelectionDimAppearance();
        }
        #endregion

        #region Properties
        public new ArrayList Model
        {
            private get
            {
                return accountProxyList;
            }
            set
            {
                accountProxyList = value;
            }
        }
        #endregion

        #region Private Methods
        private void WireUpNursingStationCensusResultView()
        {
            this.nsResultGridControl = new GridControl();
            // 
            // nsResultGridControl
            // 
            this.nsResultGridControl.BackColor = Color.White;
            this.nsResultGridControl.Location = new Point(0, 0);
            this.nsResultGridControl.Model = null;
            this.nsResultGridControl.Name = "nsResultGridControl";
            this.nsResultGridControl.Size = new Size(896, 210);
            this.nsResultGridControl.TabIndex = 0;
            this.nsResultGridControl.GridControl_Click += 
                new GridControl.UltraGridClickEventHandler
                ( this.GridControlClick );
            
            this.nsCensusResultsViewPanel.Controls.Add(this.nsResultGridControl);
            
            CreateDataStructure();
            nsResultGridControl.CensusGrid.DataSource = dataSource;
            CustomizeGridLayout();       

        }
      
        private void CreateDataStructure()
        {
            dataSource = new UltraDataSource();
            this.dataSource.Band.Key = "Patient_Band";
            this.dataSource.Band.Columns.Add( "Prvcy Opt", typeof(string) );
            this.dataSource.Band.Columns.Add( "Location", typeof(string) );
            this.dataSource.Band.Columns.Add( "Ovfl", typeof(string) );
            this.dataSource.Band.Columns.Add( "RC", typeof(string) );
            this.dataSource.Band.Columns.Add( "Patient Name", typeof(string) );
            this.dataSource.Band.Columns.Add( "Portal Opt In", typeof(string) );
            this.dataSource.Band.Columns.Add( "Account", typeof(string) );
            this.dataSource.Band.Columns.Add( "Attending Physician", typeof(string) );
            this.dataSource.Band.Columns.Add( "Gen", typeof(string) );
            this.dataSource.Band.Columns.Add( "Age", typeof(string) );
            this.dataSource.Band.Columns.Add( "PT", typeof(string) );
            
            this.dataSource.Band.Columns.Add( "Accom", typeof(string) );
            this.dataSource.Band.Columns.Add("HSV Code", typeof(string));
             this.dataSource.Band.Columns.Add( "Isol", typeof(string) );
            
        }

        private void CustomizeGridLayout()
        {
            mainBand = this.nsResultGridControl.CensusGrid.DisplayLayout.Bands[0];

            mainBand.Columns[GRIDCOL_OPT_OUT].Width = 25;
            mainBand.Columns[ GRIDCOL_LOCATION ].Width = 40;
            mainBand.Columns[ GRIDCOL_OVERFLOW ].Width = 12;
            mainBand.Columns[ GRIDCOL_ROOMCONDITION ].Width = 12;
            mainBand.Columns[ GRIDCOL_PATIENTNAME ].Width = 80;
            mainBand.Columns[ GRIDCOL_PORTALOPTIN ].Width = 40;
            mainBand.Columns[ GRIDCOL_ACCOUNT ].Width = 35;
            mainBand.Columns[ GRIDCOL_ATTNPHYSICIAN ].Width = 70;
            mainBand.Columns[ GRIDCOL_GENDER ].Width = 15;
            mainBand.Columns[ GRIDCOL_AGE ].Width = 20;
            mainBand.Columns[ GRIDCOL_PATIENTTYPE ].Width = 15;
            mainBand.Columns[ GRIDCOL_ACCOMMODATION ].Width = 30;
            mainBand.Columns[ GRIDCOL_HSV_CODE].Width = 45;
            mainBand.Columns[ GRIDCOL_ISOLATION ].Width = 15;
            
        }

        private void FillDataSource()
        {
            object [] accountProxyList = new object[ NO_OF_COLUMNS ];  
            string attendingPhysicianName = String.Empty;
            dataSource.Rows.Clear();
           
            ArrayList allAccountProxies = this.Model;

            int rowNumber = 0;

            if( allAccountProxies != null
                && allAccountProxies.Count > 0)
            {
                foreach( AccountProxy accountProxy in allAccountProxies )
                {
                    accountProxyList = new object[ NO_OF_COLUMNS ];
                    attendingPhysicianName = String.Empty;
                    if( accountProxy.Location != null )
                    {
                        accountProxyList[ GRIDCOL_OPT_OUT ] = accountProxy.AddOnlyLegends();
                        accountProxyList[ GRIDCOL_LOCATION ] = accountProxy.Location.ToString();
                        if( accountProxy.Location.Room != null 
                            && accountProxy.Location.Room.RoomCondition != null )
                        {
                            accountProxyList[ GRIDCOL_ROOMCONDITION ] = 
                                accountProxy.Location.Room.RoomCondition.Code;
                        }
                        if( accountProxy.Location.Bed != null
                            && accountProxy.Location.Bed.Accomodation != null )
                        {
                            accountProxyList[ GRIDCOL_ACCOMMODATION ] = 
                                accountProxy.Location.Bed.Accomodation.Code
                                + " "
                                + accountProxy.Location.Bed.Accomodation.Description;
                        }
                    }
                    accountProxyList[ GRIDCOL_OVERFLOW ] = accountProxy.Overflow;
                    if( accountProxy.PendingAdmission == "A" )
                    {
                        accountProxyList[ GRIDCOL_PATIENTNAME ] = "PENDING ADMISSION";
                    }
                    else
                    {
                        var patientNameFormatter = new PatientNameFormatterForCensusReports(accountProxy,false);
                        String patientName = patientNameFormatter.GetFormattedPatientName();

                        accountProxyList[GRIDCOL_PATIENTNAME] = patientName;
                        if( accountProxy.AccountNumber > 0 )
                        {
                            accountProxyList[ GRIDCOL_ACCOUNT ] = accountProxy.AccountNumber;
                        }
                        if (accountProxy.KindOfVisit != null &&
                            (accountProxy.KindOfVisit.Equals(VisitType.Inpatient)||
                             accountProxy.KindOfVisit.Equals(VisitType.Outpatient) ||
                            accountProxy.KindOfVisit.Equals(VisitType.Recurring)) &&
                            accountProxy.PatientPortalOptIn != null &&
                            accountProxy.PatientPortalOptIn.Code != string.Empty)
                        {
                            accountProxyList[ GRIDCOL_PORTALOPTIN ] = accountProxy.PatientPortalOptIn.Code;
                        }
                        if( accountProxy.AttendingPhysician != null )
                        {
                            accountProxyList[ GRIDCOL_ATTNPHYSICIAN ] = 
                                accountProxy.AttendingPhysician.ToString();
                        }
                        if( accountProxy.Patient.Sex != null )
                        {
                            accountProxyList[ GRIDCOL_GENDER ] = accountProxy.Patient.Sex.Code;
                        }
                        if( accountProxy.Patient.DateOfBirth != DateTime.MinValue )
                        {
                            accountProxyList[ GRIDCOL_AGE ] = accountProxy.Patient.AgeAt( 
                                DateTime.Today ).PadLeft( 4, '0').ToUpper();
                        }
                        if( accountProxy.KindOfVisit != null )
                        {
                            accountProxyList[ GRIDCOL_PATIENTTYPE ] = accountProxy.KindOfVisit.Code
                                + " " 
                                + accountProxy.KindOfVisit.Description;
                        }
                        accountProxyList[ GRIDCOL_ISOLATION ] = accountProxy.IsolationCode;
                    }

                    if (accountProxy.HospitalService != null)
                    {
                        accountProxyList[ GRIDCOL_HSV_CODE] = accountProxy.HospitalService;
                    }
                    dataSource.Rows.Add( accountProxyList );
                    if( this.previousSelectedLocation != String.Empty &&
                        accountProxy.Location.ToString().Trim() == 
                        previousSelectedLocation )
                    {
                        nsResultGridControl.CensusGrid.ActiveRow = 
                            (UltraGridRow)nsResultGridControl.CensusGrid.Rows[rowNumber ];
                    }
                    rowNumber++;
                }
            }
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public NursingStationCensusResultsView()
        {
            InitializeComponent();
            WireUpNursingStationCensusResultView();
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

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.progressPanel1 = new ProgressPanel();
            this.nsCensusResultsViewPanel = new System.Windows.Forms.Panel();
            this.noAccountsFoundLabel = new System.Windows.Forms.Label();
            this.nsCensusResultsViewPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = System.Drawing.Color.White;
            this.progressPanel1.Location = new System.Drawing.Point(10,0);
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new System.Drawing.Size(896,210);
            this.progressPanel1.TabIndex = 3;
            // 
            // nsCensusResultsViewPanel
            // 
            this.nsCensusResultsViewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nsCensusResultsViewPanel.Controls.Add(this.noAccountsFoundLabel);
            this.nsCensusResultsViewPanel.Location = new System.Drawing.Point(10, 0);
            this.nsCensusResultsViewPanel.Name = "nsCensusResultsViewPanel";
            this.nsCensusResultsViewPanel.Size = new System.Drawing.Size(896, 210);
            this.nsCensusResultsViewPanel.TabIndex = 0;
            // 
            // noAccountsFoundLabel
            // 
            this.noAccountsFoundLabel.Location = new System.Drawing.Point(10, 5);
            this.noAccountsFoundLabel.Name = "noAccountsFoundLabel";
            this.noAccountsFoundLabel.Size = new System.Drawing.Size(274, 16);
            this.noAccountsFoundLabel.TabIndex = 1;
            this.noAccountsFoundLabel.Text = "No beds were found based on the selected criteria.";
            this.noAccountsFoundLabel.Visible = false;
            // 
            // NursingStationCensusResultsView
            // 
            this.Load +=new EventHandler(NursingStationCensusResultsView_Load);
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.progressPanel1);
            this.Controls.Add(this.nsCensusResultsViewPanel);
            this.Name = "NursingStationCensusResultsView";
            this.Size = new System.Drawing.Size(906, 200);
            this.nsCensusResultsViewPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion

        #region Data Elements
        
        private Container                                     components = null;
        
        private ProgressPanel                                                       progressPanel1;
        private UltraDataSource                 dataSource;
        private GridControl                         nsResultGridControl;
        private UltraGridBand                                                       mainBand;

        private Panel                                          nsCensusResultsViewPanel;
        private Label                                          noAccountsFoundLabel;

        private ArrayList                                                           accountProxyList;

        private string                                                              previousSelectedLocation;
        
        #endregion

        #region Constants
        private const int GRIDCOL_OPT_OUT = 0;
        private const int GRIDCOL_LOCATION = 1;
        private const int GRIDCOL_OVERFLOW = 2;
        private const int GRIDCOL_ROOMCONDITION = 3;
        private const int GRIDCOL_PATIENTNAME = 4;
        private const int GRIDCOL_PORTALOPTIN = 5;
        private const int GRIDCOL_ACCOUNT = 6;
        private const int GRIDCOL_ATTNPHYSICIAN = 7;
        private const int GRIDCOL_GENDER = 8;
        private const int GRIDCOL_AGE = 9;
        private const int GRIDCOL_PATIENTTYPE = 10;
       
        private const int GRIDCOL_ACCOMMODATION = 11;
        private const int GRIDCOL_HSV_CODE = 12;
        private const int GRIDCOL_ISOLATION = 13;
        private const int NO_OF_COLUMNS = 14;
        
        #endregion

    }
}