using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
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
    /// Census by Religion Result View
    /// </summary>
    [Serializable]
    public class ReligionCensusResultsView : ControlView
    {
        #region Events
        #endregion

        #region Event Handlers
             
        public void BeforeWork(object sender, EventArgs e)
        {
            this.progressPanel1.Visible = true;
            this.progressPanel1.BringToFront();
            this.ReligionCensusResultsViewPanel.Visible = false;
        }

        public void AfterWork(object sender, EventArgs e)
        {
            if (this.IsDisposed || this.Disposing)
                return;
            this.progressPanel1.Visible = false;
            this.progressPanel1.SendToBack();
            this.ReligionCensusResultsViewPanel.Visible = true;
        }

        private void GridControlClick(UltraGridRow ultraGridRow)
        {
            PreviousSelectedAccountNo = ultraGridRow.Cells[GRIDCOL_ACCOUNTNO].Value.ToString() ;
        }

        private void BeforeSortOrderChange( object sender, BeforeSortChangeEventArgs e )
        {
            e.Cancel = sortingNotAllowed;
        }

        #endregion

        #region Methods

        public override void UpdateView()
        {
            this.religionResultgridPanel.BorderStyle = BorderStyle.FixedSingle;
            this.lblReligionResult.Visible = false;
            this.CensusReligionResultGridCtrl.Visible = true;     
            CreateDataStructure();
            updateModel();
            FillDataSource();
            CustomizeGridLayout();  
            CensusReligionResultGridCtrl.Focus(); 
            if( !resetMode )
            {
                SortChanged();;
            }
            resetMode  = false;
        }

        public void ResetResultView()
        {
            this.ReligionCensusResultsViewPanel.Visible = true;
            this.religionResultgridPanel.BorderStyle = BorderStyle.FixedSingle;
            this.lblReligionResult.Visible = false;
            this.CensusReligionResultGridCtrl.Visible = true;
            this.PreviousSelectedAccountNo = "";
            resetMode = true;
            dataSource.Rows.Clear();
        }
        
        public void NoAccountsFound()
        {
            this.religionResultgridPanel.BorderStyle = BorderStyle.FixedSingle;
            this.CensusReligionResultGridCtrl.Visible = false;
            this.lblReligionResult.Visible = true;
            this.PreviousSelectedAccountNo = "";
        }
        
        /// <summary>
        /// Sorts the Religion Census Results View data, 
        /// based on sort parameter.
        /// </summary>
        public void SortChanged()
        {
            if( !resetMode )
            {
                sortingNotAllowed = false;
                UltraGridBand accountBand = this.CensusReligionResultGridCtrl.CensusGrid.DisplayLayout.Bands[0];
                if( i_SortByColumn.Equals( SORT_BY_RELIGION ) )
                {
                    accountBand.SortedColumns.Clear();
                    CensusReligionResultGridCtrl.CensusGrid.DataSource = dataSource;
                }
                else
                {
                    accountBand.Columns[GRIDCOL_LOCATIONFORSORT].SortIndicator 
                        = SortIndicator.Ascending;
                    accountBand.Columns[GRIDCOL_RELIGIOFORSORT].SortIndicator
                        = SortIndicator.Ascending; 
                    accountBand.Columns[GRIDCOL_PATIENTFORSORT].SortIndicator 
                        = SortIndicator.Ascending; 
                }

                CensusReligionResultGridCtrl.CensusGrid.DataBind();

                if( PreviousSelectedAccountNo.Equals( string.Empty ))
                {
                    CensusReligionResultGridCtrl.CensusGrid.ActiveRow = 
                        (UltraGridRow)CensusReligionResultGridCtrl.CensusGrid.Rows[0];
                }
                else
                {
                    foreach( UltraGridRow row in CensusReligionResultGridCtrl.CensusGrid.Rows )
                    {
                        if( row.Cells[GRIDCOL_ACCOUNTNO].Value.ToString().Equals( PreviousSelectedAccountNo ) )
                        {
                            CensusReligionResultGridCtrl.CensusGrid.ActiveRow = row;
                            break;
                        }
                    }
                }
                sortingNotAllowed = true;
            }
        }
        public void SetRowSelectionActiveAppearance()
        {
            CensusReligionResultGridCtrl.SetRowSelectionActiveAppearance();
        }
        public void SetRowSelectionInActiveAppearance()
        {
            CensusReligionResultGridCtrl.SetRowSelectionDimAppearance();
        }

        #endregion

        #region Properties

        public string ReligionType
        {
            get
            {
                return i_ReligionType;
            }
            set
            {
                i_ReligionType = value;
            }
        }


        public string SortByColumn
        {
            get
            {
                return i_SortByColumn;
            }
            set
            {
                i_SortByColumn = value;
            }
        }
        #endregion

        #region Private Methods

        private void CreateDataStructure()
        {
            dataSource = new UltraDataSource();
            this.dataSource.Band.Key = "Religion_Band";
            this.dataSource.Band.Columns.Add( "Prvcy Opt", typeof( string ) );
            this.dataSource.Band.Columns.Add( "Religion", typeof( string ) );
            this.dataSource.Band.Columns.Add( "Location", typeof( string ) );
            this.dataSource.Band.Columns.Add( "Patient Name", typeof( string ) );
            this.dataSource.Band.Columns.Add( "Gen", typeof( string ) );
            this.dataSource.Band.Columns.Add( "Age", typeof( string ) );
            this.dataSource.Band.Columns.Add( "Isol", typeof( string ) );
            this.dataSource.Band.Columns.Add( "PT", typeof( string ) );            
            this.dataSource.Band.Columns.Add( "Place of Worship", typeof( string ) );
            this.dataSource.Band.Columns.Add( "CLV", typeof( string ) );
            this.dataSource.Band.Columns.Add( "Admit Date", typeof( string ) );
            this.dataSource.Band.Columns.Add( "Disch Status", typeof( string ) );
            this.dataSource.Band.Columns.Add( "MRN", typeof( string ) );
            this.dataSource.Band.Columns.Add( "ReligionForSort", typeof( string ) );
            this.dataSource.Band.Columns.Add( "LocationForSort", typeof( string ) );
            this.dataSource.Band.Columns.Add( "PatientNameForSort", typeof( string ) );
            
        }

        private void updateModel()
        {            
            CensusReligionResultGridCtrl.CensusGrid.DataSource = dataSource;            
        }

        private void CustomizeGridLayout()
        {
            UltraGridBand mainBand = 
                this.CensusReligionResultGridCtrl.CensusGrid.DisplayLayout.Bands[0];

            mainBand.Columns[GRIDCOL_OPT_OUT].Width = 11;
            mainBand.Columns[GRIDCOL_RELIGION].Width = 22;
            mainBand.Columns[GRIDCOL_LOCATION].Width = 13;
            mainBand.Columns[GRIDCOL_PATIENTNAME].Width = 30;
            mainBand.Columns[GRIDCOL_GENDER].Width = 5;
            mainBand.Columns[GRIDCOL_AGE].Width = 10;
            mainBand.Columns[GRIDCOL_ISOLATION].Width = 8;
            mainBand.Columns[GRIDCOL_PATIENTTYPE].Width = 4;
            mainBand.Columns[GRIDCOL_PLACEOFWORSHIP].Width = 18;
            mainBand.Columns[GRIDCOL_CLV].Width = 5;
            mainBand.Columns[GRIDCOL_ADMITDATE].Width = 12;
            mainBand.Columns[GRIDCOL_DISCHSTATUS].Width = 14;
            mainBand.Columns[GRIDCOL_ACCOUNTNO].Hidden = true;
            mainBand.Columns[GRIDCOL_RELIGIOFORSORT].Hidden = true;
            mainBand.Columns[GRIDCOL_LOCATIONFORSORT].Hidden = true;
            mainBand.Columns[GRIDCOL_PATIENTFORSORT].Hidden = true;
        }

        private void FillDataSource()
        {
            dataSource.Rows.Clear();

            ArrayList allAccountProxies = ( ArrayList ) this.Model;
  
            if( allAccountProxies != null
                && allAccountProxies.Count > 0)
            {
                 // Populate Results Grid with UNSPECIFIED religion first
                foreach( AccountProxy accountProxy in allAccountProxies )
                {
                    if( accountProxy.Patient.Religion != null &&
                        accountProxy.Patient.Religion.Code.Trim() == UNSPECIFIED_RELIGION )
                    {
                        accountProxy.Patient.Religion.Description = UNSPECIFIED_RELIGION;
                    
                        FillAccountProxyList( accountProxy );
                    }
                }
                // Populate all other religions (Specified)
                foreach( AccountProxy accountProxy in allAccountProxies )
                {
                    if( accountProxy.Patient.Religion != null &&
                        accountProxy.Patient.Religion.Code.Trim().Length > 0 && 
                        accountProxy.Patient.Religion.Code.Trim() != UNSPECIFIED_RELIGION )
                    {
                        FillAccountProxyList( accountProxy );
                    }
                }                 
            }           
        }
       
        private void FillAccountProxyList ( AccountProxy accountProxy )
        {
            object [] accountProxyList = new object[NO_OF_COLUMNS];             

            accountProxyList[GRIDCOL_OPT_OUT] = accountProxy.AddOnlyLegends();
            accountProxyList[GRIDCOL_RELIGION] 
                = accountProxy.Patient.Religion.Description;
            accountProxyList[GRIDCOL_RELIGIOFORSORT] 
                = accountProxy.Patient.Religion.Description;

            if( accountProxy.Location != null )
            {
                accountProxyList[GRIDCOL_LOCATION] 
                    = accountProxy.Location.ToString();
                accountProxyList[GRIDCOL_LOCATIONFORSORT] 
                    = accountProxy.Location.ToString();
            }

            var patientNameFormatter = new PatientNameFormatterForCensusReports(accountProxy,false);
            String patientName = patientNameFormatter.GetFormattedPatientName();

            accountProxyList[GRIDCOL_PATIENTNAME] = patientName;

            accountProxyList[GRIDCOL_PATIENTFORSORT] 
                = accountProxy.Patient.LastName + ", " + 
                accountProxy.Patient.FirstName + " " + accountProxy.Patient.MiddleInitial;

            accountProxyList[GRIDCOL_RELIGION] = 
                accountProxyList[GRIDCOL_RELIGION].ToString();

            accountProxyList[GRIDCOL_LOCATION] = 
                accountProxyList[GRIDCOL_LOCATION].ToString();

            accountProxyList[GRIDCOL_PATIENTNAME] 
                = accountProxyList[GRIDCOL_PATIENTNAME].ToString();

            accountProxyList[GRIDCOL_PATIENTNAME] = 
                accountProxyList[GRIDCOL_PATIENTNAME].ToString();

            if( accountProxy.Patient.Sex != null )
            {
                accountProxyList[GRIDCOL_GENDER] 
                    = accountProxy.Patient.Sex.Code;
            }
            accountProxyList[GRIDCOL_AGE] 
                = accountProxy.Patient.AgeAt( DateTime.Today ).
                PadLeft( 4, '0' ).ToUpper();
            accountProxyList[GRIDCOL_ISOLATION]
                = accountProxy.IsolationCode;
            accountProxyList[GRIDCOL_PATIENTTYPE] 
                = accountProxy.KindOfVisit.Code+" "+ 
                accountProxy.KindOfVisit.Description;
            
            if( accountProxy.Patient.PlaceOfWorship != null )
            {
                accountProxyList[GRIDCOL_PLACEOFWORSHIP] 
                    = accountProxy.Patient.PlaceOfWorship.Description;
            }
           
            accountProxyList[GRIDCOL_CLV] = accountProxy.ClergyVisit.Code;
            accountProxyList[GRIDCOL_ADMITDATE] = accountProxy.AdmitDate.
                ToString( "MM/dd/yyyy", DateTimeFormatInfo.InvariantInfo );

            if( accountProxy.PendingDischarge != null && 
                accountProxy.PendingDischarge.Equals( "Y" ) )
            {
                accountProxyList[GRIDCOL_DISCHSTATUS] = PENDINGDISCHARGE;
            }
            else
            {
                if( accountProxy.DischargeDate.Equals( DateTime.MinValue )  )
                {
                    accountProxyList[GRIDCOL_DISCHSTATUS] = String.Empty;
                }
                else
                {
                    accountProxyList[GRIDCOL_DISCHSTATUS] 
                        = accountProxy.DischargeDate.
                        ToString( "MM/dd/yyyy", DateTimeFormatInfo.InvariantInfo );
                }
            }
            accountProxyList[GRIDCOL_ACCOUNTNO] 
                = accountProxy.AccountNumber;          
            
            dataSource.Rows.Add( accountProxyList );

        }

        #endregion

        #region Private Properties
        #endregion

        #region Windows Form Designer generated code
        
        private void InitializeComponent()
        {
            this.ReligionCensusResultsViewPanel = new System.Windows.Forms.Panel();
            this.religionResultgridPanel = new System.Windows.Forms.Panel();
            this.lblReligionResult = new System.Windows.Forms.Label();
            this.CensusReligionResultGridCtrl = new PatientAccess.UI.CommonControls.GridControl();
            this.progressPanel1 = new PatientAccess.UI.CommonControls.ProgressPanel();
            this.ReligionCensusResultsViewPanel.SuspendLayout();
            this.religionResultgridPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ReligionCensusResultsViewPanel
            // 
            this.ReligionCensusResultsViewPanel.BackColor = System.Drawing.Color.White;
            this.ReligionCensusResultsViewPanel.Controls.Add(this.religionResultgridPanel);
            this.ReligionCensusResultsViewPanel.Location = new System.Drawing.Point(0, 0);
            this.ReligionCensusResultsViewPanel.Name = "ReligionCensusResultsViewPanel";
            this.ReligionCensusResultsViewPanel.Size = new System.Drawing.Size(912, 232);
            this.ReligionCensusResultsViewPanel.TabIndex = 0;
            // 
            // religionResultgridPanel
            // 
            this.religionResultgridPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.religionResultgridPanel.Controls.Add(this.CensusReligionResultGridCtrl);
            this.religionResultgridPanel.Controls.Add(this.lblReligionResult);
            this.religionResultgridPanel.Location = new System.Drawing.Point(0, 2);
            this.religionResultgridPanel.Name = "religionResultgridPanel";
            this.religionResultgridPanel.Size = new System.Drawing.Size(899, 213);
            this.religionResultgridPanel.TabIndex = 1;
            // 
            // CensusReligionResultGridCtrl
            // 
            this.CensusReligionResultGridCtrl.BackColor = System.Drawing.Color.White;
            this.CensusReligionResultGridCtrl.Location = new System.Drawing.Point(0, 0);
            this.CensusReligionResultGridCtrl.Model = null;
            this.CensusReligionResultGridCtrl.Name = "CensusReligionResultGridCtrl";
            this.CensusReligionResultGridCtrl.Size = new System.Drawing.Size(899, 208);
            this.CensusReligionResultGridCtrl.TabIndex = 6;
            this.CensusReligionResultGridCtrl.GridControl_Click += 
                new PatientAccess.UI.CommonControls.GridControl.UltraGridClickEventHandler
                ( this.GridControlClick );
            this.CensusReligionResultGridCtrl.GridControl_BeforeSortOrderChange += 
                new PatientAccess.UI.CommonControls.GridControl.BeforeSortChangeEventHandler 
                ( this.BeforeSortOrderChange );
            // 
            // lblReligionResult
            // 
            this.lblReligionResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblReligionResult.Location = new System.Drawing.Point(7, 8);
            this.lblReligionResult.Name = "lblReligionResult";
            this.lblReligionResult.Size = new System.Drawing.Size(296, 23);
            this.lblReligionResult.TabIndex = 0;
            this.lblReligionResult.Text = "No patients were found based on the selected criteria.";
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = System.Drawing.Color.White;
            this.progressPanel1.Location = new System.Drawing.Point(8, 8);
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new System.Drawing.Size(896, 216);
            this.progressPanel1.TabIndex = 1;
            // 
            // ReligionCensusResultsView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.ReligionCensusResultsViewPanel);
            this.Controls.Add(this.progressPanel1);
            this.Name = "ReligionCensusResultsView";
            this.Size = new System.Drawing.Size(912, 240);
            this.ReligionCensusResultsViewPanel.ResumeLayout(false);
            this.religionResultgridPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        #region Construction and Finalization

        public ReligionCensusResultsView()
        {
            InitializeComponent();
            base.EnableThemesOn( this );
            CreateDataStructure();
            updateModel();
            CustomizeGridLayout();           
        }

        /// <summary> 
        /// Cleanup any resources being used.
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
        private Panel ReligionCensusResultsViewPanel;
        private GridControl CensusReligionResultGridCtrl;
        private UltraDataSource dataSource;
        private Label lblReligionResult;
        private Panel religionResultgridPanel;
        private string PreviousSelectedAccountNo = "";
        private string i_SortByColumn;
        private string i_ReligionType = "R";  
        private bool resetMode = false;
        private bool sortingNotAllowed = true;

        #endregion        

        #region Constants

        private const string PENDINGDISCHARGE       =   "Pending";
        private const int GRIDCOL_OPT_OUT           = 0;
        private const int GRIDCOL_RELIGION          = 1;
        private const int GRIDCOL_LOCATION          = 2;  
        private const int GRIDCOL_PATIENTNAME       = 3;
        private const int GRIDCOL_GENDER            = 4;
        private const int GRIDCOL_AGE               = 5;
        private const int GRIDCOL_ISOLATION         = 6;
        private const int GRIDCOL_PATIENTTYPE       = 7;
        private const int GRIDCOL_PLACEOFWORSHIP    = 8;
        private const int GRIDCOL_CLV               = 9;
        private const int GRIDCOL_ADMITDATE         = 10;     
        private const int GRIDCOL_DISCHSTATUS       = 11;
        private const int GRIDCOL_ACCOUNTNO         = 12;
        private const int GRIDCOL_RELIGIOFORSORT    = 13;
        private const int GRIDCOL_LOCATIONFORSORT   = 14;
        private const int GRIDCOL_PATIENTFORSORT    = 15;

        private const int NO_OF_COLUMNS             = 16;
           
        private const string SORT_BY_RELIGION = "R";
        private ProgressPanel progressPanel1;
        private const string UNSPECIFIED_RELIGION   = "UNSPECIFIED";
       

        #endregion
    }
}
