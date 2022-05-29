using System;
using System.Collections;
using System.Windows.Forms;
using Extensions.UI.Winforms;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
    /// Census by Religion View
    /// </summary>
    [Serializable]
    public class CensusbyReligionView : ControlView
    {
        #region Event

        public event EventHandler ParentAcceptButtonChanged;

        #endregion

        #region Event Handlers

        private void AccountsFound(object sender, EventArgs e)
        {
            //Setup Results View
            this.religionCensusResultsView.Model = null;
            this.religionCensusResultsView.Model = ((LooseArgs)e).Context;


            this.religionCensusResultsView.ReligionType = this.religionCensusSearchView.ReligionType;
            this.religionCensusResultsView.SortByColumn = religionCensusSearchView.SortByColumn;

            this.religionCensusResultsView.Show();
            this.religionCensusResultsView.UpdateView();

        }

        private void NoAccountsFound(object sender, EventArgs e)
        {
            this.religionCensusResultsView.NoAccountsFound();

        }

        private void NoSummaryAccountsFound(object sender, EventArgs e)
        {
            this.religionCensusSummaryView.NoSummaryAccountsFound();

        }

        private void AccountsSummaryFound(object sender, EventArgs e)
        {
            //Setup Summary View
            this.religionCensusSummaryView.Model = null;
            this.religionCensusSummaryView.Model = ((LooseArgs)e).Context;
            this.religionCensusSummaryView.Show();
            this.religionCensusSummaryView.UpdateView();

        }

        private void ResetView(object sender, EventArgs e)
        {
            this.religionCensusResultsView.ResetResultView();
            this.religionCensusSummaryView.ResetSummaryView();

            this.religionCensusResultsView.ReligionType = this.religionCensusSearchView.ReligionType;
            this.religionCensusResultsView.SortByColumn = religionCensusSearchView.SortByColumn;
        }

        private void SortChange(object sender, EventArgs e)
        {
            this.religionCensusResultsView.ReligionType = this.religionCensusSearchView.ReligionType;
            this.religionCensusResultsView.SortByColumn = this.religionCensusSearchView.SortByColumn;
            this.religionCensusResultsView.SortChanged();

        }

        private void AcceptButtonChanged(object sender, EventArgs e)
        {
            if (ParentAcceptButtonChanged != null)
            {
                ParentAcceptButtonChanged(this, new LooseArgs(this));
            }
        }

        #endregion

        #region Methods
        public override void UpdateView()
        {
            base.UpdateView();
            this.religionCensusSearchView.UpdateView();
        }
        public void SetRowSelectionActiveAppearance()
        {
            if (this.religionCensusResultsView.ContainsFocus)
            {
                this.religionCensusResultsView.SetRowSelectionActiveAppearance();
            }
            if (this.religionCensusSummaryView.ContainsFocus)
            {

                this.religionCensusSummaryView.SetRowSelectionActiveAppearance();
            }
        }
        public void SetRowSelectionInActiveAppearance()
        {
            this.religionCensusResultsView.SetRowSelectionInActiveAppearance();
            this.religionCensusSummaryView.SetRowSelectionInActiveAppearance();
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods

        private void WireupReligionCensusSearchView()
        {
            this.religionCensusSearchView.AccountsFound += this.AccountsFound;
            this.religionCensusSearchView.AccountsSummaryFound += this.AccountsSummaryFound;
            this.religionCensusSearchView.NoAccountsFound += this.NoAccountsFound;
            this.religionCensusSearchView.NoSummaryAccountsFound += this.NoSummaryAccountsFound;
            this.religionCensusSearchView.ResetView += this.ResetView;
            this.religionCensusSearchView.SortChange += this.SortChange;
            this.religionCensusSearchView.AcceptButtonChanged += this.AcceptButtonChanged;
            this.religionCensusSearchView.PrintReport += this.religionCensusSearchView_PrintReport;
        }
        //Event handler for religionCensusSearchView_PrintReport
        private void religionCensusSearchView_PrintReport(object sender, EventArgs e)
        {
            ReligionReport religionReport = new ReligionReport();
            religionReport.Model = (ArrayList)((ReportArgs)e).Context;
            religionReport.SearchCriteria = (ArrayList)((ReportArgs)e).SearchCriteria;
            religionReport.SummaryInformation = (ArrayList)((ReportArgs)e).Summary;
            religionReport.PrintPreview();
        }
        #endregion

        #region Private Properties
        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// InitializeComponent
        /// </summary>
        private void InitializeComponent()
        {
            this.religionCensusPanel = new System.Windows.Forms.Panel();
            this.religionCensusSearchView =
                new PatientAccess.UI.CensusInquiries.ReligionCensusSearchView();
            this.religionCensusResultsView =
                new PatientAccess.UI.CensusInquiries.ReligionCensusResultsView();
            this.religionCensusSummaryView =
                new PatientAccess.UI.CensusInquiries.ReligionCensusSummaryView();
            this.religionCensusPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // religionCensusPanel
            // 
            this.religionCensusPanel.BackColor = System.Drawing.Color.White;
            this.religionCensusPanel.Controls.Add(this.religionCensusSearchView);
            this.religionCensusPanel.Controls.Add(this.religionCensusResultsView);
            this.religionCensusPanel.Controls.Add(this.religionCensusSummaryView);
            this.religionCensusPanel.Location = new System.Drawing.Point(0, 0);
            this.religionCensusPanel.Name = "religionCensusPanel";
            this.religionCensusPanel.Size = new System.Drawing.Size(1100, 510);
            this.religionCensusPanel.TabIndex = 0;
            // 
            // religionCensusSearchView
            // 
            this.religionCensusSearchView.Location = new System.Drawing.Point(0, 0);
            this.religionCensusSearchView.Model = null;
            this.religionCensusSearchView.Name = "religionCensusSearchView";
            this.religionCensusSearchView.ReligionType = null;
            this.religionCensusSearchView.Size = new System.Drawing.Size(980, 44);
            this.religionCensusSearchView.SortByColumn = null;
            this.religionCensusSearchView.TabIndex = 0;

            this.religionCensusSearchView.BeforeWorkEvent +=
                new EventHandler(this.religionCensusResultsView.BeforeWork);
            this.religionCensusSearchView.AfterWorkEvent +=
                new EventHandler(this.religionCensusResultsView.AfterWork);

            // 
            // religionCensusResultsView
            // 
            this.religionCensusResultsView.BackColor = System.Drawing.Color.White;
            this.religionCensusResultsView.Location = new System.Drawing.Point(10, 56);
            this.religionCensusResultsView.Model = null;
            this.religionCensusResultsView.Name = "religionCensusResultsView";
            this.religionCensusResultsView.ReligionType = "R";
            this.religionCensusResultsView.Size = new System.Drawing.Size(960, 216);
            this.religionCensusResultsView.SortByColumn = null;
            this.religionCensusResultsView.TabIndex = 1;
            this.religionCensusResultsView.AcceptButton = base.AcceptButton;
            // 
            // religionCensusSummaryView
            // 
            this.religionCensusSummaryView.BackColor = System.Drawing.Color.White;
            this.religionCensusSummaryView.Location = new System.Drawing.Point(10, 272);
            this.religionCensusSummaryView.Model = null;
            this.religionCensusSummaryView.Name = "religionCensusSummaryView";
            this.religionCensusSummaryView.Size = new System.Drawing.Size(980, 168);
            this.religionCensusSummaryView.TabIndex = 2;
            this.religionCensusSummaryView.AcceptButton = base.AcceptButton;
            // 
            // CensusbyReligionView
            // 
            this.Controls.Add(this.religionCensusPanel);
            this.Name = "CensusbyReligionView";
            this.Size = new System.Drawing.Size(900, 510);
            this.religionCensusPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        #region Construction and Finalization

        public CensusbyReligionView()
        {
            InitializeComponent();
            WireupReligionCensusSearchView();
            EnableThemesOn(this);
        }


        #endregion

        #region Data Elements

        private ReligionCensusSummaryView religionCensusSummaryView;
        private ReligionCensusSearchView religionCensusSearchView;
        private ReligionCensusResultsView religionCensusResultsView;
        private Panel religionCensusPanel;

        #endregion

        #region Constants
        #endregion
    }
}
