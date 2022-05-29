using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
    /// Census by Nursing Station Main Control
    /// </summary>
    public class CensusbyNursingStationView : ControlView
    {
        #region Events
        #endregion

        #region Event Handlers

        #endregion

        #region Methods

        private void SummaryFound( object sender, EventArgs e )
        {
            nursingStationCensusSummaryView.Model = null;
            nursingStationCensusSummaryView.Model = ( IList<NursingStation>  )((LooseArgs)e).Context;
            nursingStationCensusSummaryView.UpdateView();
        }

        private void AccountsFound( object sender, EventArgs e )
        {
            nursingStationCensusResultsView.Model = null;
            nursingStationCensusResultsView.Model = ( ArrayList )((LooseArgs)e).Context;            
            nursingStationCensusResultsView.UpdateView();
        }

        private void ResetSearch( object sender, EventArgs e )
        {
            this.nursingStationCensusResultsView.Model = null;
            this.nursingStationCensusResultsView.ResetView();

            this.nursingStationCensusSummaryView.Model = null;
            this.nursingStationCensusSummaryView.ResetView();
        }

        private void NoAccountsFound( object sender, EventArgs e )
        {
            this.nursingStationCensusSummaryView.Model = null;
            this.nursingStationCensusResultsView.NoAccountsFound();
        }

        private void PrintReport(object sender, EventArgs e)
        {
            string selectedReportType = String.Empty;
            ArrayList searchCriteria = ( ArrayList ) ( ( ReportArgs )e ).SearchCriteria;
            selectedReportType = reportTypeHashtable[searchCriteria[NS_REPORT_TYPE]].ToString();

            Assembly assembly  = Assembly.GetExecutingAssembly();
            Type NSReportType = assembly.GetType( selectedReportType );

            CensusByNursingStationReport NSReport = 
                ( CensusByNursingStationReport )Activator.
                CreateInstance( NSReportType );

            NSReport.Model = null;  
            NSReport.Model = ( ArrayList )( ( ReportArgs )e ).Context;
            NSReport.SearchCriteria = 
                ( ArrayList )( ( ReportArgs )e ).SearchCriteria;
            NSReport.SummaryInformation =
               (IList<NursingStation> )((ReportArgs)e).Summary;
            NSReport.PrintPreview();
            NSReport.CustomizeGridLayout();
            NSReport.GeneratePrintPreview();
        }

        public override void UpdateView()
        {
            base.UpdateView ();
            this.nursingStationCensusSearchView.UpdateView();
        }
        public void SetRowSelectionActiveAppearance()
        {
            if( this.nursingStationCensusResultsView.ContainsFocus  )
            {
                this.nursingStationCensusResultsView.SetRowSelectionActiveAppearance();
            }
            if( this.nursingStationCensusSummaryView.ContainsFocus  )
            {
                this.nursingStationCensusSummaryView.SetRowSelectionActiveAppearance();  
            }
        }
        public void SetRowSelectionInActiveAppearance()
        {
            this.nursingStationCensusResultsView.SetRowSelectionInActiveAppearance();
            this.nursingStationCensusSummaryView.SetRowSelectionInActiveAppearance();
        }
        #endregion

        #region Properties
        #endregion        

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            
            this.nsSearchViewPanel = new System.Windows.Forms.Panel();
            this.nursingStationCensusSearchView =
                new PatientAccess.UI.CensusInquiries.NursingStationCensusSearchView();
            this.nsResultViewPanel = new System.Windows.Forms.Panel();
            this.nursingStationCensusResultsView =
                new PatientAccess.UI.CensusInquiries.NursingStationCensusResultsView();
            this.nsSummaryViewPanel = new System.Windows.Forms.Panel();
            this.nursingStationCensusSummaryView =
                new PatientAccess.UI.CensusInquiries.NursingStationCensusSummaryView();
            this.lineLabel = new PatientAccess.UI.CommonControls.LineLabel();
            this.nsSearchViewPanel.SuspendLayout();
            this.nsResultViewPanel.SuspendLayout();
            this.nsSummaryViewPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // nsSearchViewPanel
            // 
            this.nsSearchViewPanel.Controls.Add(this.nursingStationCensusSearchView);
            this.nsSearchViewPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.nsSearchViewPanel.Location = new System.Drawing.Point(0, 0);
            this.nsSearchViewPanel.Name = "nsSearchViewPanel";
            this.nsSearchViewPanel.Size = new System.Drawing.Size(926, 48);
            this.nsSearchViewPanel.TabIndex = 0;
            this.nsSearchViewPanel.TabStop = false;
            // 
            // nursingStationCensusSearchView
            // 
            this.nursingStationCensusSearchView.Location = new System.Drawing.Point(0, 0);
            this.nursingStationCensusSearchView.Model = null;
            this.nursingStationCensusSearchView.Name = "nursingStationCensusSearchView";
            this.nursingStationCensusSearchView.Size = new System.Drawing.Size(926, 48);
            this.nursingStationCensusSearchView.TabIndex = 0;

            this.nursingStationCensusSearchView.BeforeWorkEvent +=
                new EventHandler(this.nursingStationCensusResultsView.BeforeWork);
            this.nursingStationCensusSearchView.AfterWorkEvent +=
                new EventHandler(this.nursingStationCensusResultsView.AfterWork);

            this.nursingStationCensusSearchView.AccountsFound +=
                new System.EventHandler(this.AccountsFound);
            this.nursingStationCensusSearchView.NoAccountsFound +=
                new System.EventHandler(this.NoAccountsFound);
            this.nursingStationCensusSearchView.SummaryFound +=
                new System.EventHandler(this.SummaryFound);
            this.nursingStationCensusSearchView.ResetSearch +=
                new System.EventHandler(this.ResetSearch);
            this.nursingStationCensusSearchView.PrintReport += 
                new EventHandler( PrintReport );
            //
            // nsResultViewPanel
            // 
            this.nsResultViewPanel.Controls.Add(this.nursingStationCensusResultsView);
            this.nsResultViewPanel.Location = new System.Drawing.Point(0, 48);
            this.nsResultViewPanel.Name = "nsResultViewPanel";
            this.nsResultViewPanel.Size = new System.Drawing.Size(926, 240);
            this.nsResultViewPanel.TabIndex = 1;
            this.nsResultViewPanel.TabStop = true;
            // 
            // nursingStationCensusResultsView
            // 
            this.nursingStationCensusResultsView.BackColor = System.Drawing.Color.White;
            this.nursingStationCensusResultsView.Location = new System.Drawing.Point(0, 0);
            this.nursingStationCensusResultsView.Model = null;
            this.nursingStationCensusResultsView.Name = "nursingStationCensusResultsView";
            this.nursingStationCensusResultsView.Size = new System.Drawing.Size(926, 240);
            this.nursingStationCensusResultsView.TabIndex = 0;
            // 
            // nsSummaryViewPanel
            // 
            this.nsSummaryViewPanel.Controls.Add(this.nursingStationCensusSummaryView);
            this.nsSummaryViewPanel.Controls.Add(this.lineLabel);
            this.nsSummaryViewPanel.Location = new System.Drawing.Point(0, 266);
            this.nsSummaryViewPanel.Name = "nsSummaryViewPanel";
            this.nsSummaryViewPanel.Size = new System.Drawing.Size(926, 220);
            this.nsSummaryViewPanel.TabIndex = 2;
            this.nsSummaryViewPanel.TabStop = true;
            // 
            // nursingStationCensusSummaryView
            // 
            this.nursingStationCensusSummaryView.Location = new System.Drawing.Point(0, 28);
            this.nursingStationCensusSummaryView.Model = null;
            this.nursingStationCensusSummaryView.Name = "nursingStationCensusSummaryView";
            this.nursingStationCensusSummaryView.Size = new System.Drawing.Size(926, 220);
            this.nursingStationCensusSummaryView.TabIndex = 0;
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
            // CensusbyNursingStationView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.nsSummaryViewPanel);
            this.Controls.Add(this.nsResultViewPanel);
            this.Controls.Add(this.nsSearchViewPanel);
            this.Name = "CensusbyNursingStationView";
            this.Size = new System.Drawing.Size(926, 470);
            this.nsSearchViewPanel.ResumeLayout(false);
            this.nsResultViewPanel.ResumeLayout(false);
            this.nsSummaryViewPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Construction and Finalization
        public CensusbyNursingStationView()
        {
            InitializeComponent();
            base.EnableThemesOn( this );
                PopulateHashtable();
        }
        #endregion

        #region Private Methods
        private void PopulateHashtable()
        {
            reportTypeHashtable = new Hashtable();
            reportTypeHashtable.Add( NS_DEPARTMENT_KEY, NS_DEPARTMENT_REPORT );
            reportTypeHashtable.Add( NS_INFO_DESK_KEY, NS_INFORMATION_DESK_REPORT );
        }
        #endregion

        #region Private Properties
        #endregion

        #region Data Elements

        private Hashtable                                                           reportTypeHashtable;

        private NursingStationCensusSearchView     nursingStationCensusSearchView;
        private NursingStationCensusResultsView    nursingStationCensusResultsView;
        private NursingStationCensusSummaryView    nursingStationCensusSummaryView;
                
        private Panel                                          nsSearchViewPanel;
        private Panel                                          nsResultViewPanel;
        private Panel                                          nsSummaryViewPanel;
        
        private LineLabel                           lineLabel;

        //private bool                                                                isAvailable;

        #endregion

        #region Constants
        private const string NS_DEPARTMENT_REPORT = "PatientAccess.UI.CensusInquiries.NursingStationByDepartmentReport",
            NS_DEPARTMENT_KEY = "Department",
            NS_INFORMATION_DESK_REPORT = "PatientAccess.UI.CensusInquiries.NursingStationByInfoDeskReport",
            NS_INFO_DESK_KEY = "Information Desk";

        private const int NS_REPORT_TYPE = 2;

        #endregion

    }
}
