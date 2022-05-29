using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.PatientSearch;

namespace PatientAccess.UI.HistoricalAccountViews
{
    /// <summary>
    /// Summary description for HistoricalPatientSearch.
    /// </summary>
    public class HistoricalPatientSearch : TimeOutFormView
    {
        #region Events
        #endregion

        #region Event Handlers

        private void HistoricalPatientSearch_Load(object sender, EventArgs e)
        {
            patientSearchResultsView.Hide();
        }

        private void ViewAccountsButton_Click(object sender, EventArgs e)
        {
            patientSearchResultsView.GetSelectedPatientFromPatientSearchResult();
        }

        private void PatientSearchView_PatientsFound(object sender, EventArgs e)
        {
            List<PatientSearchResult> searchResults = ((LooseArgs)e).Context as List<PatientSearchResult>;

            patientSearchResultsView.Model = null;
            viewAccountsButton.Enabled = ( searchResults != null && searchResults.Count > 0 );
            patientSearchResultsView.Model = searchResults;
            patientSearchResultsView.Show();
            patientSearchResultsView.UpdateView();
        }

        private void PatientSearchView_SearchReset(object sender, EventArgs e)
        {
            patientSearchResultsView.Model = null;
            viewAccountsButton.Enabled = false;
            patientSearchResultsView.Hide();
        }

        private void PatientSearchResultsView_PatientSelected(object sender, PatientSelectedEventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if (e.Patient != null)
                {
                    HistoricalPatientAccounts historicalPatientAccounts = new HistoricalPatientAccounts();
                    try
                    {
                        historicalPatientAccounts.CurrentActivity = CurrentActivity;
                        historicalPatientAccounts.Model = e.Patient;
                        historicalPatientAccounts.UpdateView();
                        historicalPatientAccounts.Owner = this;
                        Hide();
                        historicalPatientAccounts.ShowDialog(this);
                    }
                    finally
                    {
                        historicalPatientAccounts.Dispose();
                    }
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Methods

        public void ShowPanel()
        {
            progressPanel1.Visible = true;
            progressPanel1.BringToFront();
        }

        public void HidePanel()
        {
            patientSearchResultsView.SetListViewDefaultItemSelected();
            patientSearchResultsView.FocusOnGrid();

            progressPanel1.Visible = false;
            progressPanel1.SendToBack();
        }

        #endregion

        #region Properties

        public Activity CurrentActivity
        {
            private get
            {
                return i_CurrentActivity;
            }
            set
            {
                i_CurrentActivity = value;
            }
        }

        #endregion

        #region Private Methods

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public HistoricalPatientSearch()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            EnableThemesOn(this);
            patientSearchResultsView.SearchByType = "HistoricalPatientSearch";
            patientSearchView.CurrentActivity = new ViewAccountActivity();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(HistoricalPatientSearch));
            this.viewAccountsButton = new PatientAccess.UI.CommonControls.LoggingButton();
            this.cancelButton = new PatientAccess.UI.CommonControls.LoggingButton();
            this.historicalPatientSearchPanel = new System.Windows.Forms.Panel();
            this.patientSearchResultsPanel = new System.Windows.Forms.Panel();
            this.patientSearchResultsView = new PatientAccess.UI.PatientSearch.PatientSearchResultsView();
            this.patientSearchPanel = new System.Windows.Forms.Panel();
            this.patientSearchView = new PatientAccess.UI.PatientSearch.ViewAccountFindAPatientView();
            this.progressPanel1 = new PatientAccess.UI.CommonControls.ProgressPanel();
            this.historicalPatientSearchPanel.SuspendLayout();
            this.patientSearchResultsPanel.SuspendLayout();
            this.patientSearchPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // viewAccountsButton
            // 
            this.viewAccountsButton.Enabled = false;
            this.viewAccountsButton.Location = new System.Drawing.Point(811, 538);
            this.viewAccountsButton.Message = null;
            this.viewAccountsButton.Name = "viewAccountsButton";
            this.viewAccountsButton.Size = new System.Drawing.Size(95, 23);
            this.viewAccountsButton.TabIndex = 2;
            this.viewAccountsButton.Text = "&View Accounts";
            this.viewAccountsButton.Click += new System.EventHandler(this.ViewAccountsButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(911, 538);
            this.cancelButton.Message = null;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            // 
            // historicalPatientSearchPanel
            // 
            this.historicalPatientSearchPanel.BackColor = System.Drawing.Color.White;
            this.historicalPatientSearchPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.historicalPatientSearchPanel.Controls.Add(this.patientSearchResultsPanel);
            this.historicalPatientSearchPanel.Controls.Add(this.patientSearchPanel);
            this.historicalPatientSearchPanel.Controls.Add(this.progressPanel1);
            this.historicalPatientSearchPanel.Location = new System.Drawing.Point(7, 7);
            this.historicalPatientSearchPanel.Name = "historicalPatientSearchPanel";
            this.historicalPatientSearchPanel.Size = new System.Drawing.Size(979, 524);
            this.historicalPatientSearchPanel.TabIndex = 0;
            // 
            // patientSearchResultsPanel
            // 
            this.patientSearchResultsPanel.Controls.Add(this.patientSearchResultsView);
            this.patientSearchResultsPanel.Location = new System.Drawing.Point(7, 198);
            this.patientSearchResultsPanel.Name = "patientSearchResultsPanel";
            this.patientSearchResultsPanel.Size = new System.Drawing.Size(984, 319);
            this.patientSearchResultsPanel.TabIndex = 1;
            // 
            // patientSearchResultsView
            // 
            this.patientSearchResultsView.BackColor = System.Drawing.Color.White;
            this.patientSearchResultsView.Location = new System.Drawing.Point(0, 0);
            this.patientSearchResultsView.Model = null;
            this.patientSearchResultsView.Name = "patientSearchResultsView";
            this.patientSearchResultsView.Size = new System.Drawing.Size(964, 319);
            this.patientSearchResultsView.TabIndex = 1;
            this.patientSearchResultsView.PatientSelectedFromSearchResult += new System.EventHandler<PatientSelectedEventArgs>(this.PatientSearchResultsView_PatientSelected);
            // 
            // patientSearchPanel
            // 
            this.patientSearchPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.patientSearchPanel.Controls.Add(this.patientSearchView);
            this.patientSearchPanel.Location = new System.Drawing.Point(7, 7);
            this.patientSearchPanel.Name = "patientSearchPanel";
            this.patientSearchPanel.Size = new System.Drawing.Size(964, 176);
            this.patientSearchPanel.TabIndex = 0;
            // 
            // patientSearchView
            // 
            this.patientSearchView.BackColor = System.Drawing.Color.White;
            this.patientSearchView.CurrentActivity = null;
            this.patientSearchView.Location = new System.Drawing.Point(0, 0);
            this.patientSearchView.Model = null;
            this.patientSearchView.Name = "patientSearchView";
            this.patientSearchView.Size = new System.Drawing.Size(990, 176);
            this.patientSearchView.TabIndex = 0;
            this.patientSearchView.SearchReset += new System.EventHandler(this.PatientSearchView_SearchReset);
            this.patientSearchView.PatientsFound += new System.EventHandler(this.PatientSearchView_PatientsFound);
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = System.Drawing.Color.White;
            this.progressPanel1.Location = new System.Drawing.Point(8, 192);
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new System.Drawing.Size(968, 312);
            this.progressPanel1.TabIndex = 4;
            // 
            // HistoricalPatientSearch
            // 
            this.AcceptButton = this.viewAccountsButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(993, 568);
            this.Controls.Add(this.historicalPatientSearchPanel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.viewAccountsButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HistoricalPatientSearch";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "View Account - Search by Patient";
            this.Load += new System.EventHandler(this.HistoricalPatientSearch_Load);
            this.historicalPatientSearchPanel.ResumeLayout(false);
            this.patientSearchResultsPanel.ResumeLayout(false);
            this.patientSearchPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        #region Data Elements

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private Panel historicalPatientSearchPanel;
        private Panel patientSearchPanel;
        private Panel patientSearchResultsPanel;

        private ViewAccountFindAPatientView patientSearchView;
        private PatientSearchResultsView patientSearchResultsView;
        private ProgressPanel progressPanel1;
        private Activity i_CurrentActivity;

        private LoggingButton viewAccountsButton;
        private LoggingButton cancelButton;

        #endregion

        #region Constants
        #endregion

    }
}
