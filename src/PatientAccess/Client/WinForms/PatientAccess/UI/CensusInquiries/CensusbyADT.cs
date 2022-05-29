using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
    /// Census by Admissions, Discharges and Transafers (ADT) Main Control
    /// </summary>
    [Serializable]
    public class CensusbyADT : ControlView
    {

        #region Events
        public event EventHandler ParentAcceptButtonChanged;
        #endregion

        #region Event Handlers
        
        private void PrintReport( object sender, EventArgs e )
        {
            string selectedADTReport = String.Empty;
            CensusADTSearchCriteria ADTSearchCriteria;
            ReportArgs reportArgs    = (ReportArgs)e;
            ADTSearchCriteria        = (CensusADTSearchCriteria)reportArgs.SearchCriteria;
            
            selectedADTReport  = ADTReportsHashtable[ADTSearchCriteria.ADTActivity].ToString();

            Assembly assembly  = Assembly.GetExecutingAssembly();
            Type ADTReportType = assembly.GetType( selectedADTReport );

            CensusbyADTReport ADTReport  = 
                (CensusbyADTReport)Activator.CreateInstance( ADTReportType );
            
            ADTReport.Model              = null;  
            ADTReport.Model              = (ArrayList)reportArgs.Context;
            ADTReport.SearchCriteria     = ADTSearchCriteria;
            ADTReport.SummaryInformation = (ArrayList)reportArgs.Summary;
            ADTReport.PrintPreview();
            ADTReport.CustomizeGridLayout();
            ADTReport.GeneratePrintPreview();
        }

        #endregion

        #region Methods

        private void SummaryFound( object sender, EventArgs e )
        {
            ADTCensusSummaryView.Model = null;
            ADTCensusSummaryView.Model = ( ArrayList )( ( LooseArgs )e ).Context;
            ADTCensusSummaryView.Show();
            ADTCensusSummaryView.UpdateView();
        }

        private void AccountsFound( object sender, EventArgs e )
        {
            ADTCensusResultsView.Model = null;
            ADTCensusSearchView.UpdateView();
            ADTCensusResultsView.Model = ( ArrayList )( ( LooseArgs )e ).Context;
            ADTCensusResultsView.ADTType = ADTCensusSearchView.ADTType;
            ADTCensusResultsView.SortByColumn = 
                ADTCensusSearchView.SortByColumn;
            ADTCensusResultsView.Show();
            ADTCensusResultsView.UpdateView();
        }

        private void ResetSearch( object sender, EventArgs e )
        {
            ADTCensusResultsView.Model = null;
            ADTCensusResultsView.ADTType = ADTCensusSearchView.ADTType;
            ADTCensusResultsView.SortByColumn = 
                ADTCensusSearchView.SortByColumn;
            ADTCensusResultsView.ResetView();

            ADTCensusSummaryView.Model = null;
            ADTCensusSummaryView.ResetView();
        }

        private void NoAccountsFound( object sender, EventArgs e )
        {
            ADTCensusSummaryView.Model = null;
            ADTCensusResultsView.NoAccountsFound();
        }

        private void SortChanged( object sender, EventArgs e )
        {
            ADTCensusResultsView.ADTType = ADTCensusSearchView.ADTType;
            ADTCensusResultsView.SortByColumn = 
                ADTCensusSearchView.SortByColumn;
            ADTCensusResultsView.SortChanged();
        }

        private void AcceptButtonChanged( object sender, EventArgs e )
        {
            if( ParentAcceptButtonChanged != null )
            {
                ParentAcceptButtonChanged( this, new LooseArgs( this ) );
            }  
        }

        public override void UpdateView()
        {
            base.UpdateView ();
            this.ADTCensusSearchView.UpdateView();
        }
        public void SetRowSelectionActiveAppearance()
        {
            if( this.ADTCensusResultsView.ContainsFocus  )
            {
                this.ADTCensusResultsView.SetRowSelectionActiveAppearance();
            }
            if( this.ADTCensusSummaryView.ContainsFocus  )
            {
                this.ADTCensusSummaryView.SetRowSelectionActiveAppearance();  
            }
        }
        public void SetRowSelectionInActiveAppearance()
        {
            this.ADTCensusResultsView.SetRowSelectionInActiveAppearance();
            this.ADTCensusSummaryView.SetRowSelectionInActiveAppearance();
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        
        private void PopulateADTReportsList()
        {
            ADTReportsHashtable = new Hashtable();
            ADTReportsHashtable.Add( ADT_TYPE_ADMISSION, ADT_ADMISSION_REPORT );
            ADTReportsHashtable.Add( ADT_TYPE_DISCHARGE, ADT_DISCHARGE_REPORT );
            ADTReportsHashtable.Add( ADT_TYPE_TRANSFER, ADT_TRANSFER_REPORT );
            ADTReportsHashtable.Add( ADT_TYPE_ALL, ADT_ALLADT_REPORT );
        }
        
        #endregion

        #region Private Properties
        #endregion

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.nsSearchViewPanel = new System.Windows.Forms.Panel();
            this.ADTCensusSearchView = new PatientAccess.UI.CensusInquiries.ADTCensusSearchView();
            this.nsResultViewPanel = new System.Windows.Forms.Panel();
            this.ADTCensusResultsView = new PatientAccess.UI.CensusInquiries.ADTCensusResultsView();
            this.nsSummaryViewPanel = new System.Windows.Forms.Panel();
            this.ADTCensusSummaryView = new PatientAccess.UI.CensusInquiries.ADTCensusSummaryView();
            this.lineLabel = new PatientAccess.UI.CommonControls.LineLabel();
            this.nsSearchViewPanel.SuspendLayout();
            this.nsResultViewPanel.SuspendLayout();
            this.nsSummaryViewPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // nsSearchViewPanel
            // 
            this.nsSearchViewPanel.BackColor = System.Drawing.Color.CadetBlue;
            this.nsSearchViewPanel.Controls.Add(this.ADTCensusSearchView);
            this.nsSearchViewPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.nsSearchViewPanel.Location = new System.Drawing.Point(0, 0);
            this.nsSearchViewPanel.Name = "nsSearchViewPanel";
            this.nsSearchViewPanel.Size = new System.Drawing.Size(926, 122);
            this.nsSearchViewPanel.TabIndex = 0;
            this.nsSearchViewPanel.TabStop = false;
            // 
            // ADTCensusSearchView
            // 
            this.ADTCensusSearchView.ADTType = null;
            this.ADTCensusSearchView.BackColor = System.Drawing.Color.CornflowerBlue;
            this.ADTCensusSearchView.Cursor = System.Windows.Forms.Cursors.Default;
            this.ADTCensusSearchView.Location = new System.Drawing.Point(0, 0);
            this.ADTCensusSearchView.Model = null;
            this.ADTCensusSearchView.Name = "ADTCensusSearchView";
            this.ADTCensusSearchView.Size = new System.Drawing.Size(926, 122);
            this.ADTCensusSearchView.SortByColumn = null;
            this.ADTCensusSearchView.TabIndex = 0;

            this.ADTCensusSearchView.NoAccountsFound += new System.EventHandler(this.NoAccountsFound);
            this.ADTCensusSearchView.SummaryFound += new System.EventHandler(this.SummaryFound);
            this.ADTCensusSearchView.AcceptButtonChanged += new System.EventHandler(this.AcceptButtonChanged);
            this.ADTCensusSearchView.SortChanged += new System.EventHandler(this.SortChanged);
            this.ADTCensusSearchView.ResetSearch += new System.EventHandler(this.ResetSearch);
            this.ADTCensusSearchView.PrintReport += new System.EventHandler(this.PrintReport);
            this.ADTCensusSearchView.AccountsFound += new System.EventHandler(this.AccountsFound);

            this.ADTCensusSearchView.BeforeWorkEvent +=
                new EventHandler(this.ADTCensusResultsView.BeforeWork);
            this.ADTCensusSearchView.AfterWorkEvent +=
                new EventHandler(this.ADTCensusResultsView.AfterWork);
            // 
            // nsResultViewPanel
            // 
            this.nsResultViewPanel.Controls.Add(this.ADTCensusResultsView);
            this.nsResultViewPanel.Location = new System.Drawing.Point(0, 127);
            this.nsResultViewPanel.Name = "nsResultViewPanel";
            this.nsResultViewPanel.Size = new System.Drawing.Size(926, 173);
            this.nsResultViewPanel.TabIndex = 1;
            this.nsResultViewPanel.TabStop = true;
            // 
            // ADTCensusResultsView
            // 
            this.ADTCensusResultsView.ADTType = "A";
            this.ADTCensusResultsView.BackColor = System.Drawing.Color.White;
            this.ADTCensusResultsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ADTCensusResultsView.Location = new System.Drawing.Point(0, 0);
            this.ADTCensusResultsView.Model = null;
            this.ADTCensusResultsView.Name = "ADTCensusResultsView";
            this.ADTCensusResultsView.Size = new System.Drawing.Size(926, 173);
            this.ADTCensusResultsView.SortByColumn = null;
            this.ADTCensusResultsView.TabIndex = 0;
            // 
            // nsSummaryViewPanel
            // 
            this.nsSummaryViewPanel.Controls.Add(this.ADTCensusSummaryView);
            this.nsSummaryViewPanel.Controls.Add(this.lineLabel);
            this.nsSummaryViewPanel.Location = new System.Drawing.Point(0, 325);
            this.nsSummaryViewPanel.Name = "nsSummaryViewPanel";
            this.nsSummaryViewPanel.Size = new System.Drawing.Size(926, 128);
            this.nsSummaryViewPanel.TabIndex = 2;
            this.nsSummaryViewPanel.TabStop = true;
            // 
            // ADTCensusSummaryView
            // 
            this.ADTCensusSummaryView.BackColor = System.Drawing.Color.White;
            this.ADTCensusSummaryView.Location = new System.Drawing.Point(0, 28);
            this.ADTCensusSummaryView.Model = null;
            this.ADTCensusSummaryView.Name = "ADTCensusSummaryView";
            this.ADTCensusSummaryView.Size = new System.Drawing.Size(926, 99);
            this.ADTCensusSummaryView.TabIndex = 0;
            // 
            // lineLabel
            // 
            this.lineLabel.Caption = "Statistical Summary by Nursing Station (Inpatients Only)";
            this.lineLabel.Location = new System.Drawing.Point(10, 3);
            this.lineLabel.Name = "lineLabel";
            this.lineLabel.Size = new System.Drawing.Size(890, 16);
            this.lineLabel.TabIndex = 1;
            this.lineLabel.TabStop = false;
            // 
            // CensusbyADT
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.nsSummaryViewPanel);
            this.Controls.Add(this.nsResultViewPanel);
            this.Controls.Add(this.nsSearchViewPanel);
            this.Name = "CensusbyADT";
            this.Size = new System.Drawing.Size(926, 470);
            this.nsSearchViewPanel.ResumeLayout(false);
            this.nsResultViewPanel.ResumeLayout(false);
            this.nsSummaryViewPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Construction and Finalization
        public CensusbyADT()
        {
            //try
            //{
            this.Cursor = Cursors.WaitCursor;
            InitializeComponent();
            base.EnableThemesOn( this );
            PopulateADTReportsList();
        }
        #endregion

        #region Data Elements
        private ADTCensusSearchView
            ADTCensusSearchView;
        private Panel nsSearchViewPanel;
        private Panel nsResultViewPanel;
        private LineLabel lineLabel;
        private ADTCensusResultsView
            ADTCensusResultsView;
        private ADTCensusSummaryView
            ADTCensusSummaryView;
        private Panel nsSummaryViewPanel;

        //private bool isAvailable;
        
        private Hashtable ADTReportsHashtable;
        #endregion

        #region Constants

        private const string
        ADT_TYPE_ADMISSION      = "A",
        ADT_TYPE_DISCHARGE      = "D",
        ADT_TYPE_TRANSFER       = "T",
        ADT_TYPE_ALL            = "E",
        ADT_ADMISSION_REPORT    = "PatientAccess.UI.CensusInquiries.CensusbyAdmissionReport",
        ADT_DISCHARGE_REPORT    = "PatientAccess.UI.CensusInquiries.CensusbyDischargeReport",
        ADT_TRANSFER_REPORT     = "PatientAccess.UI.CensusInquiries.CensusbyTransferReport",
        ADT_ALLADT_REPORT       = "PatientAccess.UI.CensusInquiries.CensusbyAllADTReport";

        #endregion
    }
}