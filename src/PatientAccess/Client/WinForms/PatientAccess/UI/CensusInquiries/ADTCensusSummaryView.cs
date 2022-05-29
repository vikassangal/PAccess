using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using Appearance = Infragistics.Win.Appearance;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
    /// Admissions, Discharges and Transfers Census Summary View
    /// </summary>
    [Serializable]
    public class ADTCensusSummaryView : ControlView
    {
        
        #region Events
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public override void UpdateView()
        {
            FillDataSource();
            adtSummaryGridControl.CensusGrid.DataSource = dataSource;
            adtSummaryGridControl.CensusGrid.DataBind();
            CustomizeGridLayout();
        }

        private void WireUpADTCensusSummaryView()
        {
            CreateDataStructure();
            adtSummaryGridControl.CensusGrid.DataSource = dataSource;
            CustomizeGridLayout();
        }

        public void ResetView()
        {
            dataSource.Rows.Clear();
            this.adtSummaryGridControl.Visible = true;
            this.adtSummaryGridControl.Show();
        }
        public void SetRowSelectionActiveAppearance()
        {
            adtSummaryGridControl.SetRowSelectionActiveAppearance();
        }
        public void SetRowSelectionInActiveAppearance()
        {
            adtSummaryGridControl.SetRowSelectionDimAppearance();
        }
        #endregion

        #region Properties
        public new ArrayList Model
        {
            private get
            {
                return base.Model as ArrayList;	
            }	
            set	
            {		
                base.Model = value;	
            }
        }
        #endregion

        #region Private Methods
        private void CreateDataStructure()
        {
            dataSource = new UltraDataSource();
            this.dataSource.Band.Key = "Summary_Band";
            this.dataSource.Band.Columns.Add("NS", typeof(string));
            this.dataSource.Band.Columns.Add("Prev Census", typeof(int));
            this.dataSource.Band.Columns.Add("Admit Today", typeof(int));
            this.dataSource.Band.Columns.Add("Disch Today", typeof(int));
            this.dataSource.Band.Columns.Add("Deaths Today", typeof(int));
            this.dataSource.Band.Columns.Add("Trfr Today", typeof(int));            
            this.dataSource.Band.Columns.Add("Avl Beds", typeof(int));
            this.dataSource.Band.Columns.Add("Curr Census", typeof(int));
            this.dataSource.Band.Columns.Add("Total Beds", typeof(int));
            this.dataSource.Band.Columns.Add("% Occ", typeof(string));
            this.dataSource.Band.Columns.Add("Mo Pt Days", typeof(int));
            this.dataSource.Band.Columns.Add("Mo Beds Avail", typeof(int));
            this.dataSource.Band.Columns.Add("Mo % Occ", typeof(string));
        }

        private void CustomizeGridLayout()
        {
            Appearance appearanceRightAlign = 
                new Appearance();
            appearanceRightAlign.TextHAlign = HAlign.Right;
            Appearance appearanceLeftAlign = 
                new Appearance();
            appearanceLeftAlign.TextHAlign = HAlign.Left;
            mainBand = 
                this.adtSummaryGridControl.CensusGrid.DisplayLayout.Bands[ 
                                                    GRIDCOL_NURSING_STATION ];

            mainBand.Columns[ GRIDCOL_NURSING_STATION ].Width = 25;
            mainBand.Columns[ GRIDCOL_PREVIOUS_CENSUS ].Width = 55;
            mainBand.Columns[ GRIDCOL_PREVIOUS_CENSUS ].CellAppearance = 
                                                       appearanceLeftAlign;
            mainBand.Columns[ GRIDCOL_ADMIT_TODAY ].Width = 55;
            mainBand.Columns[ GRIDCOL_ADMIT_TODAY ].CellAppearance = 
                                                        appearanceLeftAlign;
            mainBand.Columns[ GRIDCOL_DISCHARGE_TODAY ].Width = 50;
            mainBand.Columns[ GRIDCOL_DISCHARGE_TODAY ].CellAppearance = 
                                                        appearanceLeftAlign;
            mainBand.Columns[ GRIDCOL_DEATHS_TODAY ].Width = 55;
            mainBand.Columns[ GRIDCOL_DEATHS_TODAY ].CellAppearance = 
                                                        appearanceLeftAlign;
            mainBand.Columns[ GRIDCOL_TRANSFER_TODAY ].Width = 50;
            mainBand.Columns[ GRIDCOL_TRANSFER_TODAY ].CellAppearance = 
                                                        appearanceLeftAlign;
            mainBand.Columns[ GRIDCOL_AVAILABLE_BEDS ].Width = 40;
            mainBand.Columns[ GRIDCOL_AVAILABLE_BEDS ].CellAppearance = 
                                                        appearanceLeftAlign;
            mainBand.Columns[ GRIDCOL_CURRENT_CENSUS ].Width = 50;
            mainBand.Columns[ GRIDCOL_CURRENT_CENSUS ].CellAppearance = 
                                                        appearanceLeftAlign;
            mainBand.Columns[ GRIDCOL_TOTAL_BEDS ].Width = 50;
            mainBand.Columns[ GRIDCOL_TOTAL_BEDS ].CellAppearance = 
                                                        appearanceLeftAlign;
            mainBand.Columns[ GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS ].Width = 30;
            mainBand.Columns[ GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS ].CellAppearance = 
                                                        appearanceRightAlign;
            mainBand.Columns[ GRIDCOL_TOTAL_OCCUPIED_BEDS_MONTH ].Width = 50;
            mainBand.Columns[ GRIDCOL_TOTAL_OCCUPIED_BEDS_MONTH ].CellAppearance = 
                                                        appearanceLeftAlign;
            mainBand.Columns[ GRIDCOL_TOTAL_BEDS_MONTH ].Width = 60;
            mainBand.Columns[ GRIDCOL_TOTAL_BEDS_MONTH ].CellAppearance = 
                                                        appearanceLeftAlign;
            mainBand.Columns[ GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS_MONTH ].Width = 50;
            mainBand.Columns[ GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS_MONTH ].CellAppearance = 
                                                        appearanceRightAlign;
            Appearance headerAppearance = 
                                        new Appearance();
            headerAppearance.BackColor = Color.LightGray;
            headerAppearance.TextHAlign = HAlign.Center;
            adtSummaryGridControl.CensusGrid.DisplayLayout.Override.HeaderAppearance = 
                                                                        headerAppearance;

            adtSummaryGridControl.CensusGrid.ActiveRow = null;
        }

        private void FillDataSource()
        {
            object [] summaryColumns;
            string attendingPhysicianName = String.Empty;
            dataSource.Rows.Clear();
            ArrayList nursingStationList = this.Model;
            int percentOccupiedBeds;
            int percentOccupiedBedsForMonth;

            if( nursingStationList != null
                && nursingStationList.Count > 0)
            {
                foreach( NursingStation nursingStation in nursingStationList )
                {
                    summaryColumns = new object[ NO_OF_COLUMNS ];
                    percentOccupiedBeds = 0;
                    percentOccupiedBedsForMonth = 0;
                    summaryColumns[ GRIDCOL_NURSING_STATION ] = nursingStation.Code;
                    summaryColumns[ GRIDCOL_PREVIOUS_CENSUS ] = 
                        nursingStation.PreviousCensus;
                    summaryColumns[ GRIDCOL_ADMIT_TODAY ] = 
                        nursingStation.AdmitToday;
                    summaryColumns[ GRIDCOL_DISCHARGE_TODAY ] = 
                        nursingStation.DischargeToday;
                    summaryColumns[ GRIDCOL_DEATHS_TODAY ] = 
                        nursingStation.DeathsToday;
                    summaryColumns[ GRIDCOL_TRANSFER_TODAY ] = 
                        nursingStation.TransferredFromToday;
                    summaryColumns[ GRIDCOL_AVAILABLE_BEDS ] = 
                        nursingStation.AvailableBeds;
                    summaryColumns[ GRIDCOL_CURRENT_CENSUS ] = 
                        nursingStation.CurrentCensus;
                    summaryColumns[ GRIDCOL_TOTAL_BEDS ] = nursingStation.TotalBeds;
                    if( nursingStation.TotalBeds > 0 )
                    {
                        percentOccupiedBeds = (int) Math.Round( ( (decimal) 
                            nursingStation.CurrentCensus * 100 / 
                            (decimal) nursingStation.TotalBeds ) );
                        summaryColumns[ GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS ] = 
                            percentOccupiedBeds + "%";
                    }
                    else
                    {
                        summaryColumns[ GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS ] = "0%";
                    }
                    summaryColumns[ GRIDCOL_TOTAL_OCCUPIED_BEDS_MONTH ] = 
                        nursingStation.TotalOccupiedBedsForMonth;
                    summaryColumns[ GRIDCOL_TOTAL_BEDS_MONTH ] = 
                        nursingStation.TotalBedsForMonth;
                    if( nursingStation.TotalBedsForMonth > 0 )
                    {
                        percentOccupiedBedsForMonth = (int)Math.Round( ( (decimal) 
                            nursingStation.TotalOccupiedBedsForMonth * 100
                            / (decimal) nursingStation.TotalBedsForMonth ) );
                        summaryColumns[ GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS_MONTH ] = 
                            percentOccupiedBedsForMonth + "%";
                    }
                    else
                    {
                        summaryColumns[ GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS_MONTH ] = "0%";
                    }

                    dataSource.Rows.Add( summaryColumns );
                }

            }
        }

        #endregion

        #region Private Properties
        #endregion

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.adtCensusSummaryViewpanel = new System.Windows.Forms.Panel();
            this.adtSummaryGridControl = new PatientAccess.UI.CommonControls.GridControl();
            this.adtCensusSummaryViewpanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // adtCensusSummaryViewpanel
            // 
            this.adtCensusSummaryViewpanel.Controls.Add(this.adtSummaryGridControl);
            this.adtCensusSummaryViewpanel.BackColor = System.Drawing.Color.White;
            this.adtCensusSummaryViewpanel.Location = new System.Drawing.Point(10, 0);
            this.adtCensusSummaryViewpanel.Name = "adtCensusSummaryViewpanel";
            this.adtCensusSummaryViewpanel.Size = new System.Drawing.Size(896, 99);
            this.adtCensusSummaryViewpanel.TabIndex = 0;
            this.adtCensusSummaryViewpanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            //
            // adtSummaryGridControl
            // 
            this.adtSummaryGridControl.BackColor = System.Drawing.Color.White;
            this.adtSummaryGridControl.Location = new System.Drawing.Point(0, 0);
            this.adtSummaryGridControl.Model = null;
            this.adtSummaryGridControl.Name = "adtSummaryGridControl";
            this.adtSummaryGridControl.Size = new System.Drawing.Size(896, 99);
            this.adtSummaryGridControl.TabIndex = 1;
            this.adtSummaryGridControl.TabStop = true;
            // 
            // ADTCensusSummaryView
            // 
            this.Controls.Add(this.adtCensusSummaryViewpanel);
            this.Name = "ADTCensusSummaryView";
            this.Size = new System.Drawing.Size(912, 99);
            this.ResumeLayout(false);

        }
        #endregion

        #region Construction and Finalization
        public ADTCensusSummaryView()
        {
            InitializeComponent();
            base.EnableThemesOn( this );
            WireUpADTCensusSummaryView();
        }
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        private Panel adtCensusSummaryViewpanel;
        private Container components = null;
        private UltraDataSource dataSource; 
        private GridControl 
                                                        adtSummaryGridControl;
        private UltraGridBand mainBand;
        #endregion

        #region Constants

        private const int NO_OF_COLUMNS = 13;

        private const int GRIDCOL_NURSING_STATION = 0,
            GRIDCOL_PREVIOUS_CENSUS = 1,
            GRIDCOL_ADMIT_TODAY = 2,
            GRIDCOL_DISCHARGE_TODAY = 3,
            GRIDCOL_DEATHS_TODAY = 4,
            GRIDCOL_TRANSFER_TODAY = 5,
            GRIDCOL_AVAILABLE_BEDS = 6,
            GRIDCOL_CURRENT_CENSUS = 7,
            GRIDCOL_TOTAL_BEDS = 8,
            GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS = 9,
            GRIDCOL_TOTAL_OCCUPIED_BEDS_MONTH = 10,
            GRIDCOL_TOTAL_BEDS_MONTH = 11,
            GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS_MONTH = 12;
        #endregion

    }
}