using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using Appearance = Infragistics.Win.Appearance;

namespace PatientAccess.UI.CensusInquiries
{   
    /// <summary>
    /// Census by Religion Summary View
    /// </summary>
    [Serializable]
    public class ReligionCensusSummaryView : ControlView
    {
     
        #region Event
        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override void UpdateView()
        {
            this.religionSummaryPanel.BorderStyle = BorderStyle.FixedSingle;
            this.lblReligionSummary.Visible = false;
            this.CensusReligionSummaryGridCtrl.Visible = true;
            FillDataSource();          
            ReBindGrid();
            CustomizeGridLayout();
            CensusReligionSummaryGridCtrl.Focus(); 
        }

        public void ResetSummaryView()
        {
            this.ReligionCensusSummaryViewPanel.Visible = true;
            this.religionSummaryPanel.BorderStyle = BorderStyle.FixedSingle;;
            this.lblReligionSummary.Visible = false;
            this.CensusReligionSummaryGridCtrl.Visible = true;
            dataSource.Rows.Clear();
        }

        public void NoSummaryAccountsFound()
        {
            this.religionSummaryPanel.BorderStyle = BorderStyle.FixedSingle;
            this.CensusReligionSummaryGridCtrl.Visible = false;
            this.lblReligionSummary.Visible = true;            
        }

        public void SetRowSelectionActiveAppearance()
        {
            CensusReligionSummaryGridCtrl.SetRowSelectionActiveAppearance();
        }
        public void SetRowSelectionInActiveAppearance()
        {
           CensusReligionSummaryGridCtrl.SetRowSelectionDimAppearance();
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods

        private void CreateDataStructure()
        {
            dataSource = new UltraDataSource();
            this.dataSource.Band.Key = "Religion_Summary_Band";
            this.dataSource.Band.Columns.Add( "Religion", typeof( string ) );
            this.dataSource.Band.Columns.Add( "Total", typeof( string ) );
            this.dataSource.Band.Columns.Add( " ", typeof( string ) );
        }


        private void ReBindGrid()
        {
            CensusReligionSummaryGridCtrl.CensusGrid.DataSource = dataSource;
            CensusReligionSummaryGridCtrl.CensusGrid.DataBind();
        }


        private void CustomizeGridLayout()
        {
            //SET_COLUMN_COLOR
            Appearance columnApp = new Appearance(); 

            Appearance appearanceRightAlign = 
                            new Appearance();
            appearanceRightAlign.TextHAlign = HAlign.Right;        

            this.CensusReligionSummaryGridCtrl.CensusGrid.DisplayLayout.Bands[0]
                .Columns[GRIDCOL_RELIGION].CellAppearance = columnApp;

            this.CensusReligionSummaryGridCtrl.CensusGrid.DisplayLayout.Bands[0].
                Columns[GRIDCOL_TOTAL].CellAppearance.TextHAlign  = HAlign.Left ;

            this.CensusReligionSummaryGridCtrl.CensusGrid.DisplayLayout.Bands[0].
                Columns[NO_OF_COLUMNS].CellAppearance = columnApp;
           
            this.CensusReligionSummaryGridCtrl.CensusGrid.DisplayLayout.Bands[0].
                Columns[GRIDCOL_RELIGION].Width = 120;
            this.CensusReligionSummaryGridCtrl.CensusGrid.DisplayLayout.Bands[0].
                Columns[GRIDCOL_TOTAL].Width = 50;
            this.CensusReligionSummaryGridCtrl.CensusGrid.DisplayLayout.Bands[0].
                Columns[NO_OF_COLUMNS].Width = 692;          

            Appearance headerAppearance = new Appearance();
            headerAppearance.BackColor = Color.LightGray;
            headerAppearance.TextHAlign = HAlign.Left;
            this.CensusReligionSummaryGridCtrl.CensusGrid.DisplayLayout
                .Override.HeaderAppearance = headerAppearance;
            // Will not select a default row.
            this.CensusReligionSummaryGridCtrl.CensusGrid.ActiveRow = null;

        }

        private void FillDataSource()
        {
            dataSource.Rows.Clear();
            
            int rowNumber = 0;           
            
            ArrayList allReligionProxies = ( ArrayList ) this.Model;
  
            if( allReligionProxies != null
                && allReligionProxies.Count > 0)
            {
                // Populate Results Grid with UNSPECIFIED religion first
                foreach( Religion religion in allReligionProxies )
                {
                    if( religion.Description.Trim().Length == 0 )
                    {
                        religion.Description = UNSPECIFIED_RELIGION;
                        FillReligionProxyList( religion, rowNumber );
                    }
                }
                // Populate all other religions (Specified)
                foreach( Religion religion in allReligionProxies )
                {
                    if( !( religion.Description.Equals( UNSPECIFIED_RELIGION ) ) )
                    {
                        FillReligionProxyList( religion, rowNumber );
                    }
                }                 
                rowNumber++;
            }            
        }
     
        private void FillReligionProxyList ( Religion religion, int rowNumber )
        {

           object [] religionProxyList = new object[NO_OF_COLUMNS];             

           religionProxyList[0] = religion.Description.Trim();
           religionProxyList[1] = Convert.ToString( religion.TotalCount );
           
           dataSource.Rows.Add( religionProxyList );

        }
        #endregion

        #region Private Properties
        #endregion

        #region Windows Form Designer generated code
        
        private void InitializeComponent()
        {
            this.ReligionCensusSummaryViewPanel = new System.Windows.Forms.Panel();
            this.lineLabel = new PatientAccess.UI.CommonControls.LineLabel();
            this.religionSummaryPanel = new System.Windows.Forms.Panel();
            this.CensusReligionSummaryGridCtrl = new PatientAccess.UI.CommonControls.GridControl();
            this.lblReligionSummary = new System.Windows.Forms.Label();
            this.ReligionCensusSummaryViewPanel.SuspendLayout();
            this.religionSummaryPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ReligionCensusSummaryViewPanel
            // 
            this.ReligionCensusSummaryViewPanel.BackColor = System.Drawing.Color.White;
            this.ReligionCensusSummaryViewPanel.Controls.Add(this.lineLabel);
            this.ReligionCensusSummaryViewPanel.Controls.Add(this.religionSummaryPanel);
            this.ReligionCensusSummaryViewPanel.Location = new System.Drawing.Point(0, 0);
            this.ReligionCensusSummaryViewPanel.Name = "ReligionCensusSummaryViewPanel";
            this.ReligionCensusSummaryViewPanel.Size = new System.Drawing.Size(912, 168);
            this.ReligionCensusSummaryViewPanel.TabIndex = 0;
            // 
            // lineLabel
            // 
            this.lineLabel.BackColor = System.Drawing.Color.White;
            this.lineLabel.Caption = "Statistical Summary by Religion (Inpatients Only)";
            this.lineLabel.Location = new System.Drawing.Point(1, 6);
            this.lineLabel.Name = "lineLabel";
            this.lineLabel.Size = new System.Drawing.Size(892, 16);
            this.lineLabel.TabIndex = 0;
            this.lineLabel.TabStop = false;
            // 
            // religionSummaryPanel
            // 
            this.religionSummaryPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.religionSummaryPanel.Controls.Add(this.CensusReligionSummaryGridCtrl);
            this.religionSummaryPanel.Controls.Add(this.lblReligionSummary);
            this.religionSummaryPanel.Location = new System.Drawing.Point(0, 32);
            this.religionSummaryPanel.Name = "religionSummaryPanel";
            this.religionSummaryPanel.Size = new System.Drawing.Size(899, 132);
            this.religionSummaryPanel.TabIndex = 0;
            // 
            // CensusReligionSummaryGridCtrl
            // 
            this.CensusReligionSummaryGridCtrl.BackColor = System.Drawing.Color.White;
            this.CensusReligionSummaryGridCtrl.Location = new System.Drawing.Point(0, 0);
            this.CensusReligionSummaryGridCtrl.Model = null;
            this.CensusReligionSummaryGridCtrl.Name = "CensusReligionSummaryGridCtrl";
            this.CensusReligionSummaryGridCtrl.Size = new System.Drawing.Size(899, 132);
            this.CensusReligionSummaryGridCtrl.TabIndex = 1;
            // 
            // lblReligionSummary
            // 
            this.lblReligionSummary.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblReligionSummary.Location = new System.Drawing.Point(7, 8);
            this.lblReligionSummary.Name = "lblReligionSummary";
            this.lblReligionSummary.Size = new System.Drawing.Size(319, 23);
            this.lblReligionSummary.TabIndex = 0;
            this.lblReligionSummary.Text = "No statistics to display.";
            // 
            // ReligionCensusSummaryView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.ReligionCensusSummaryViewPanel);
            this.Name = "ReligionCensusSummaryView";
            this.Size = new System.Drawing.Size(912, 184);
            this.ReligionCensusSummaryViewPanel.ResumeLayout(false);
            this.religionSummaryPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        #region Construction and Finalization

        public ReligionCensusSummaryView()
        {
            InitializeComponent();
            CreateDataStructure();
            ReBindGrid();
            CustomizeGridLayout();
            base.EnableThemesOn( this );
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
        private Panel ReligionCensusSummaryViewPanel;
        private GridControl CensusReligionSummaryGridCtrl;
        private Label lblReligionSummary;
        private Panel religionSummaryPanel;
        private LineLabel lineLabel;
        private UltraDataSource dataSource;

        #endregion     

        #region Constants
       
        private const int GRIDCOL_RELIGION          = 0;
        private const int GRIDCOL_TOTAL             = 1;  
        private const int NO_OF_COLUMNS             = 2;
        private const string UNSPECIFIED_RELIGION   = "UNSPECIFIED";

        #endregion
    }
}