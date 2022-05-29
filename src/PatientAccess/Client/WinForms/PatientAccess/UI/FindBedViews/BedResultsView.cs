using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using PatientAccess.BrokerInterfaces;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.FindBedViews
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class BedResultsView : ControlView
    {

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.bedResultsViewPanel = new System.Windows.Forms.Panel();
            this.noAccountsFoundLabel = new System.Windows.Forms.Label();
            this.bedResultsViewPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // bedResultsViewPanel
            // 
            this.bedResultsViewPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.bedResultsViewPanel.Controls.Add(this.noAccountsFoundLabel);
            this.bedResultsViewPanel.Location = new System.Drawing.Point(0, 0);
            this.bedResultsViewPanel.Name = "bedResultsViewPanel";
            this.bedResultsViewPanel.Size = new System.Drawing.Size(891, 320);
            this.bedResultsViewPanel.TabIndex = 0;
            // 
            // noAccountsFoundLabel
            // 
            this.noAccountsFoundLabel.Location = new System.Drawing.Point(5, 5);
            this.noAccountsFoundLabel.Name = "noAccountsFoundLabel";
            this.noAccountsFoundLabel.Size = new System.Drawing.Size(274, 16);
            this.noAccountsFoundLabel.TabIndex = 0;
            this.noAccountsFoundLabel.Text = "No beds were found based on the selected criteria.";
            this.noAccountsFoundLabel.Visible = false;
            // 
            // BedResultsView
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.Controls.Add(this.bedResultsViewPanel);
            this.Name = "BedResultsView";
            this.Size = new System.Drawing.Size(891, 320);
            this.bedResultsViewPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        #region Event Handlers

        private void GridControl_Click( UltraGridRow ultraGridRow )
        {
            previousSelectedLocation = 
                ultraGridRow.Cells[GRIDCOL_LOCATION].Value.ToString().Trim();
            // Occupied, hence return
            if( LocationSelectionChanged != null )
            {
                if ( ultraGridRow.Cells[GRIDCOL_PATIENTNAME].Value.ToString() != String.Empty &&
                     ultraGridRow.Cells[GRIDCOL_PATIENTNAME].Value.ToString().Length > 0 )
                {
                    LocationSelectionChanged( this, null );
                }
                else
                {
                    LocationSelectionChanged( this, 
                        new LooseArgs( previousSelectedLocation ) );
                }
            }
        }

        private void GridControl_DoubleClick( UltraGridRow ultraGridRow )
        {
            if ( ultraGridRow.Cells[GRIDCOL_PATIENTNAME].Value.ToString() == String.Empty )
            {
                if( LocationSelected != null )
                {
                    LocationSelected( this, null );
                }
            }
        }
        
        private void bedResultGridControl_GridControl_BeforeSortOrderChange(object sender, BeforeSortChangeEventArgs e)
        {
            //e.Cancel = true;
        }

        #endregion

        #region Events 

        public event EventHandler LocationSelectionChanged;
        public event EventHandler LocationSelected;

        #endregion

        #region Methods

        public override void UpdateView()
        {
            this.bedResultGridControl.Visible = true;
            this.noAccountsFoundLabel.Visible = false;
            FillDataSource();
           
            bedResultGridControl.CensusGrid.DataBind();
            bedResultGridControl.Focus(); 
            this.bedResultGridControl.CensusGrid.DisplayLayout.Bands[0].SortedColumns.Add(
                "Location", false, false );

        }

        public void LocationsNotFound()
        {
            this.bedResultsViewPanel.Visible = true;
            this.noAccountsFoundLabel.Visible = true;
            this.previousSelectedLocation = String.Empty;
            this.bedResultGridControl.Visible = false;
        }

        public void ResetView()
        {
            this.noAccountsFoundLabel.Visible = false;
            this.bedResultGridControl.Visible = true;
            this.dataSource.Rows.Clear();
            this.previousSelectedLocation = String.Empty;
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

        private void WireUpBedResultView()
        {
            this.bedResultGridControl = new GridControl();
            this.bedResultGridControl.BackColor = Color.White;
            this.bedResultGridControl.Location = new Point(0, 0);
            this.bedResultGridControl.Model = null;
            this.bedResultGridControl.Name = "bedResultGridControl";
            this.bedResultGridControl.Size = new Size(891, 320);
            this.bedResultGridControl.TabIndex = 0;
            this.bedResultGridControl.GridControl_Click += 
                new GridControl.UltraGridClickEventHandler( 
                this.GridControl_Click );
            this.bedResultGridControl.GridControl_DoubleClick +=
                new GridControl.UltraGridDoubleClickEventHandler(
                this.GridControl_DoubleClick );
            this.bedResultGridControl.GridControl_BeforeSortOrderChange += 
                new GridControl.BeforeSortChangeEventHandler( 
                bedResultGridControl_GridControl_BeforeSortOrderChange );
            this.bedResultsViewPanel.Controls.Add( this.bedResultGridControl );
            this.bedResultGridControl.CensusGrid.DisplayLayout.BorderStyle =
                                UIElementBorderStyle.None;

            CreateDataStructure();
            bedResultGridControl.CensusGrid.DataSource = dataSource;
            CustomizeGridLayout();       
        }

        private void CreateDataStructure()
        {
            dataSource = new UltraDataSource();
            this.dataSource.Band.Key = "Patient_Band";
            this.dataSource.Band.Columns.Add( "Location", typeof(string) );
            this.dataSource.Band.Columns.Add( "Ovfl", typeof(string) );
            this.dataSource.Band.Columns.Add( "Patient Name", typeof(string) );
            this.dataSource.Band.Columns.Add( "Account", typeof(string) );
            this.dataSource.Band.Columns.Add( "Gender", typeof(string) );
            this.dataSource.Band.Columns.Add( "Age", typeof(string) );
            this.dataSource.Band.Columns.Add( "Patient Type", typeof(string) );            
            this.dataSource.Band.Columns.Add( "Attending Physician", typeof(string) );
            this.dataSource.Band.Columns.Add( "Isol", typeof(string) );
        }

        private void CustomizeGridLayout()
        {
            mainBand = this.bedResultGridControl.CensusGrid.DisplayLayout.Bands[0];
            mainBand.Columns[GRIDCOL_LOCATION].Width = 25;
            mainBand.Columns[GRIDCOL_OVERFLOW].Width = 10;
            mainBand.Columns[GRIDCOL_PATIENTNAME].Width = 80;
            mainBand.Columns[GRIDCOL_ACCOUNT].Width = 35;
            mainBand.Columns[GRIDCOL_GENDER].Width = 20;
            mainBand.Columns[GRIDCOL_AGE].Width = 20;
            mainBand.Columns[GRIDCOL_PATIENTTYPE].Width = 40;
            mainBand.Columns[GRIDCOL_PHYSICIAN].Width = 70;
            mainBand.Columns[GRIDCOL_ISOLATION].Width = 15;
            
        }

        private void FillDataSource()
        {
            object [] accountProxyList = new object[NO_OF_COLUMNS];  
            string attendingPhysicianName = String.Empty;
            dataSource.Rows.Clear();
            
            ArrayList allAccountProxies = this.Model;

            int rowNumber = 0;

            foreach( AccountProxy accountProxy in allAccountProxies )
            {
                accountProxyList = new object[NO_OF_COLUMNS];
                attendingPhysicianName = String.Empty;
                if( accountProxy.Location != null )
                {
                    accountProxyList[GRIDCOL_LOCATION] = 
                        accountProxy.Location.ToString();
                }
                accountProxyList[GRIDCOL_OVERFLOW] = accountProxy.Overflow;
                if( accountProxy.PendingAdmission == "A" )
                {
                    accountProxyList[GRIDCOL_PATIENTNAME] = ADMISSION_STATUS;
                }
                else
                {
                    accountProxyList[GRIDCOL_PATIENTNAME] = 
                        accountProxy.Patient.FormattedName;
                }
                if( accountProxy.AccountNumber > 0 )
                {
                    accountProxyList[GRIDCOL_ACCOUNT] = 
                        accountProxy.AccountNumber;
                }
                if( accountProxy.Patient.Sex != null )
                {
                    accountProxyList[GRIDCOL_GENDER] = 
                        accountProxy.Patient.Sex.Code;
                }
                if( accountProxy.Patient.DateOfBirth != DateTime.MinValue )
                {
                    accountProxyList[GRIDCOL_AGE] = 
                        accountProxy.Patient.AgeAt( 
                        DateTime.Today ).PadLeft( 4, '0').ToUpper();
                }
                if( accountProxy.KindOfVisit != null )
                {
                    accountProxyList[GRIDCOL_PATIENTTYPE] = 
                        accountProxy.KindOfVisit.Code
                        + " " 
                        + accountProxy.KindOfVisit.Description;
                }
                if( accountProxy.AttendingPhysician != null )
                {
                    accountProxyList[GRIDCOL_PHYSICIAN] = 
                        accountProxy.AttendingPhysician.ToString();
                }
                accountProxyList[GRIDCOL_ISOLATION] = 
                    accountProxy.IsolationCode;

                dataSource.Rows.Add( accountProxyList );
                UltraGridRow gridRow = (UltraGridRow)bedResultGridControl.CensusGrid.Rows[rowNumber];
                if( gridRow.Cells[GRIDCOL_PATIENTNAME].Value.ToString() != String.Empty )
                {
                    gridRow.Activation = Activation.Disabled ;
                }
                 
                if( this.previousSelectedLocation != String.Empty &&
                    accountProxy.Location.ToString().Trim() == 
                    previousSelectedLocation )
                {
                    bedResultGridControl.CensusGrid.ActiveRow = 
                        (UltraGridRow)bedResultGridControl.CensusGrid.Rows[rowNumber];
                }
                rowNumber++;
            }
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public BedResultsView()
        {
            InitializeComponent();
            WireUpBedResultView();
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

        private Container components = null;
        private UltraDataSource dataSource;
        private GridControl bedResultGridControl;
        private Panel bedResultsViewPanel;
        private Label noAccountsFoundLabel;
        private ArrayList accountProxyList;
        private string previousSelectedLocation;
        private UltraGridBand mainBand;
        
        #endregion

        #region Constants
        
        private const int
            NO_OF_COLUMNS       = 9,
            GRIDCOL_LOCATION    = 0,
            GRIDCOL_OVERFLOW    = 1,
            GRIDCOL_PATIENTNAME = 2,
            GRIDCOL_ACCOUNT     = 3,
            GRIDCOL_GENDER      = 4,
            GRIDCOL_AGE         = 5,
            GRIDCOL_PATIENTTYPE = 6,
            GRIDCOL_PHYSICIAN   = 7,
            GRIDCOL_ISOLATION   = 8;

        private const string 
            ADMISSION_STATUS = "PENDING ADMISSION";

        #endregion
    }
}