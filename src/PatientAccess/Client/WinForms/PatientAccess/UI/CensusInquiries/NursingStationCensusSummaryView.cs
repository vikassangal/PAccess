using System;
using System.Collections.Generic;
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
    /// 
    /// </summary>
    [Serializable]
    public class NursingStationCensusSummaryView : ControlView
    {
        #region Event Handlers
        #endregion

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.nsCensusSummaryViewpanel = new System.Windows.Forms.Panel();
            this.nsSummaryGridControl = new PatientAccess.UI.CommonControls.GridControl();
            this.nsCensusSummaryViewpanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // nsCensusSummaryViewpanel
            // 
            this.nsCensusSummaryViewpanel.Controls.Add(this.nsSummaryGridControl);
            this.nsCensusSummaryViewpanel.BackColor = System.Drawing.Color.White;
            this.nsCensusSummaryViewpanel.Location = new System.Drawing.Point(10, 0);
            this.nsCensusSummaryViewpanel.Name = "nsCensusSummaryViewpanel";
            this.nsCensusSummaryViewpanel.Size = new System.Drawing.Size(896, 155);
            this.nsCensusSummaryViewpanel.TabIndex = 0;
            this.nsCensusSummaryViewpanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            //
            // nsSummaryGridControl
            // 
            this.nsSummaryGridControl.BackColor = System.Drawing.Color.White;
            this.nsSummaryGridControl.Location = new System.Drawing.Point(0, 0);
            this.nsSummaryGridControl.Model = null;
            this.nsSummaryGridControl.Name = "nsSummaryGridControl";
            this.nsSummaryGridControl.Size = new System.Drawing.Size(896, 155);
            this.nsSummaryGridControl.TabIndex = 1;
            this.nsSummaryGridControl.TabStop = true;
            // 
            // NursingStationCensusSummaryView
            // 
            this.Controls.Add(this.nsCensusSummaryViewpanel);
            this.Name = "NursingStationCensusSummaryView";
            this.Size = new System.Drawing.Size(912, 160);
            this.ResumeLayout(false);

        }
        #endregion

        #region Event
        #endregion

        #region Methods
        public override void UpdateView()
        {
            FillDataSource();
            nsSummaryGridControl.CensusGrid.DataSource = dataSource;
            nsSummaryGridControl.CensusGrid.DataBind();
            CustomizeGridLayout();
            nsSummaryGridControl.Focus(); 
        }

        private void WireUpNursingStationCensusSummaryView()
        {
            CreateDataStructure();
            nsSummaryGridControl.CensusGrid.DataSource = dataSource;
            CustomizeGridLayout();
        }

        public void ResetView()
        {
            dataSource.Rows.Clear();
            this.nsSummaryGridControl.Visible = true;
        }
        public void SetRowSelectionActiveAppearance()
        {
           nsSummaryGridControl.SetRowSelectionActiveAppearance();
        }
        public void SetRowSelectionInActiveAppearance()
        {
            nsSummaryGridControl.SetRowSelectionDimAppearance();
        }
        #endregion

        #region Properties
        public new IList<NursingStation> Model
        {
            private get { return base.Model as IList<NursingStation>; }	
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
            this.dataSource.Band.Columns.Add("Trfr From", typeof(int));            
            this.dataSource.Band.Columns.Add("Trfr To", typeof(int));
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
            Appearance appearanceRightAlign = new Appearance();
            appearanceRightAlign.TextHAlign = HAlign.Right;
            Appearance appearanceLeftAlign = new Appearance();
            appearanceLeftAlign.TextHAlign = HAlign.Left;
            mainBand = this.nsSummaryGridControl.CensusGrid.DisplayLayout.Bands[0];

            mainBand.Columns[ GRIDCOL_NURSING_STATION ].Width = 20;
            mainBand.Columns[ GRIDCOL_PREVIOUS_CENSUS ].Width = 55;
            mainBand.Columns[ GRIDCOL_PREVIOUS_CENSUS ].CellAppearance = appearanceLeftAlign;
            mainBand.Columns[ GRIDCOL_ADMIT_TODAY ].Width = 55;
            mainBand.Columns[ GRIDCOL_ADMIT_TODAY ].CellAppearance = appearanceLeftAlign;
            mainBand.Columns[ GRIDCOL_DISCHARGE_TODAY ].Width = 55;
            mainBand.Columns[ GRIDCOL_DISCHARGE_TODAY ].CellAppearance = appearanceLeftAlign;
            mainBand.Columns[ GRIDCOL_DEATHS_TODAY ].Width = 60;
            mainBand.Columns[ GRIDCOL_DEATHS_TODAY ].CellAppearance = appearanceLeftAlign;
            mainBand.Columns[ GRIDCOL_TRANSFER_FROM ].Width = 45;
            mainBand.Columns[ GRIDCOL_TRANSFER_FROM ].CellAppearance = appearanceLeftAlign;
            mainBand.Columns[ GRIDCOL_TRANSFER_TO ].Width = 40;
            mainBand.Columns[ GRIDCOL_TRANSFER_TO ].CellAppearance = appearanceLeftAlign;
            mainBand.Columns[ GRIDCOL_AVAILABLE_BEDS ].Width = 40;
            mainBand.Columns[ GRIDCOL_AVAILABLE_BEDS ].CellAppearance = appearanceLeftAlign;
            mainBand.Columns[ GRIDCOL_CURRENT_CENSUS ].Width = 55;
            mainBand.Columns[ GRIDCOL_CURRENT_CENSUS ].CellAppearance = appearanceLeftAlign;
            mainBand.Columns[ GRIDCOL_TOTAL_BEDS ].Width = 50;
            mainBand.Columns[ GRIDCOL_TOTAL_BEDS ].CellAppearance = appearanceLeftAlign;
            mainBand.Columns[ GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS ].Width = 35;
            mainBand.Columns[ GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS ].CellAppearance = appearanceRightAlign;
            mainBand.Columns[ GRIDCOL_TOTAL_OCCUPIED_BEDS_MONTH ].Width = 50;
            mainBand.Columns[ GRIDCOL_TOTAL_OCCUPIED_BEDS_MONTH ].CellAppearance = appearanceLeftAlign;
            mainBand.Columns[ GRIDCOL_TOTAL_BEDS_MONTH ].Width = 60;
            mainBand.Columns[ GRIDCOL_TOTAL_BEDS_MONTH ].CellAppearance = appearanceLeftAlign;
            mainBand.Columns[ GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS_MONTH ].Width = 50;
            mainBand.Columns[ GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS_MONTH ].CellAppearance = appearanceRightAlign;
            Appearance headerAppearance = new Appearance();
            headerAppearance.BackColor = Color.LightGray;
            headerAppearance.TextHAlign = HAlign.Center;
            this.nsSummaryGridControl.CensusGrid.DisplayLayout.Override.HeaderAppearance = headerAppearance;
            // Will not select a default row.
            this.nsSummaryGridControl.CensusGrid.ActiveRow = null;
        }

        private void FillDataSource()
        {
            object [] summaryColumns;
            string attendingPhysicianName = String.Empty;
            dataSource.Rows.Clear();
            IList<NursingStation> nursingStationList = this.Model;
            int percentOccupiedBeds;
            int percentOccupiedBedsForMonth;

            if( nursingStationList != null
                && nursingStationList.Count > 0)
            {
                foreach( NursingStation nursingStation in nursingStationList )
                {
                    summaryColumns = new object[ NO_OF_COLUMNS ];
                    if( !nursingStation.Code.Trim().Equals( ALL_NURSINGSTATION_CODE ) )
                    { 
                        percentOccupiedBeds = 0;
                        percentOccupiedBedsForMonth = 0;               
                        summaryColumns[ GRIDCOL_NURSING_STATION ] = nursingStation.Code;
                        summaryColumns[ GRIDCOL_PREVIOUS_CENSUS ] = nursingStation.PreviousCensus;
                        summaryColumns[ GRIDCOL_ADMIT_TODAY ] = nursingStation.AdmitToday;
                        summaryColumns[ GRIDCOL_DISCHARGE_TODAY ] = nursingStation.DischargeToday;
                        summaryColumns[ GRIDCOL_DEATHS_TODAY ] = nursingStation.DeathsToday;
                        summaryColumns[ GRIDCOL_TRANSFER_FROM ] = nursingStation.TransferredFromToday;
                        summaryColumns[ GRIDCOL_TRANSFER_TO ] = nursingStation.TransferredToToday;
                        summaryColumns[ GRIDCOL_AVAILABLE_BEDS ] = nursingStation.AvailableBeds;
                        summaryColumns[ GRIDCOL_CURRENT_CENSUS ] = nursingStation.CurrentCensus;
                        summaryColumns[ GRIDCOL_TOTAL_BEDS ] = nursingStation.TotalBeds;
                        if( nursingStation.TotalBeds > 0 )
                        {
                            percentOccupiedBeds = (int) Math.Round( ( (decimal) 
                                nursingStation.CurrentCensus * 100 / 
                                (decimal) nursingStation.TotalBeds ) );
                            summaryColumns[ GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS ] 
                                = percentOccupiedBeds + "%";
                        }
                        else
                        {
                            summaryColumns[ GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS ] = "0%";
                        }
                        summaryColumns[ GRIDCOL_TOTAL_OCCUPIED_BEDS_MONTH ] 
                            = nursingStation.TotalOccupiedBedsForMonth;
                        summaryColumns[ GRIDCOL_TOTAL_BEDS_MONTH ] = nursingStation.TotalBedsForMonth;
                        if( nursingStation.TotalBedsForMonth > 0 )
                        {
                            percentOccupiedBedsForMonth = (int)Math.Round( ( (decimal) 
                                nursingStation.TotalOccupiedBedsForMonth * 100
                                / (decimal) nursingStation.TotalBedsForMonth ) );
                            summaryColumns[ GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS_MONTH ] 
                                = percentOccupiedBedsForMonth + "%";
                        }
                        else
                        {
                            summaryColumns[ GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS_MONTH ] = "0%";
                        }
                        dataSource.Rows.Add( summaryColumns );
                    }
                }
            }
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public NursingStationCensusSummaryView()
        {
            InitializeComponent();
            base.EnableThemesOn( this );
            WireUpNursingStationCensusSummaryView();
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
        private Panel nsCensusSummaryViewpanel;
        private Container components = null;
        private UltraDataSource dataSource;
        private GridControl nsSummaryGridControl;
        private UltraGridBand mainBand;
        #endregion

        #region Constants
        private const int NO_OF_COLUMNS = 14;
        private const int GRIDCOL_NURSING_STATION = 0,
            GRIDCOL_PREVIOUS_CENSUS = 1,
            GRIDCOL_ADMIT_TODAY = 2,
            GRIDCOL_DISCHARGE_TODAY = 3,
            GRIDCOL_DEATHS_TODAY = 4,
            GRIDCOL_TRANSFER_FROM = 5,
            GRIDCOL_TRANSFER_TO = 6,
            GRIDCOL_AVAILABLE_BEDS = 7,
            GRIDCOL_CURRENT_CENSUS = 8,
            GRIDCOL_TOTAL_BEDS = 9,
            GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS = 10,
            GRIDCOL_TOTAL_OCCUPIED_BEDS_MONTH = 11,
            GRIDCOL_TOTAL_BEDS_MONTH = 12,
            GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS_MONTH = 13;
        private const string ALL_NURSINGSTATION_CODE = "$$";

        #endregion
    }
}