using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
    /// 
    /// </summary>
    public class CensusOfBloodlessPatientsView : ControlView
    {
        #region Event Handlers

        private void AccountsFound( object sender, EventArgs e )
        {
            this.bloodlessPatientsResultsView.Model = null;
            this.bloodlessPatientsResultsView.Model = ( ArrayList )( ( LooseArgs )e ).Context;
            this.bloodlessPatientsResultsView.Show();
            this.bloodlessPatientsResultsView.UpdateView();
        }

        private void ResetView( object sender, EventArgs e )
        {
            this.bloodlessPatientsResultsView.ResetResultsView();
        }

        private void NoAccountsFound( object sender, EventArgs e )
        {            
            this.bloodlessPatientsResultsView.NoAccountsFound();
        }

        private void bloodlessPatientsSearchView_PrintReport(object sender, EventArgs e)
        {
            BloodlessReport bloodlessReport = new BloodlessReport();
            bloodlessReport.Model = ( ArrayList )( ( ReportArgs )e ).Context;
            bloodlessReport.SearchCriteria = ( ArrayList )( ( ReportArgs )e ).SearchCriteria;
            bloodlessReport.PrintPreview();
        }

        #endregion

        #region Methods
        public void SetRowSelectionActiveAppearance()
        {
            if( this.bloodlessPatientsResultsView.ContainsFocus  )
            {
                this.bloodlessPatientsResultsView.SetRowSelectionActiveAppearance();
            }
        }
        public void SetRowSelectionInActiveAppearance()
        {
            this.bloodlessPatientsResultsView.SetRowSelectionInActiveAppearance();
        }
        #endregion

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {            
            this.bloodlessPatientsSearchView = new PatientAccess.UI.CensusInquiries.BloodlessPatientsSearchView();
            this.bloodlessPatientsResultsView = new PatientAccess.UI.CensusInquiries.BloodlessPatientsResultsView();
            this.searchViewPanel = new System.Windows.Forms.Panel();
            this.resultsViewPanel = new System.Windows.Forms.Panel();
            this.searchViewPanel.SuspendLayout();
            this.resultsViewPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // bloodlessPatientsSearchView
            // 
            this.bloodlessPatientsSearchView.BackColor = System.Drawing.Color.White;
            this.bloodlessPatientsSearchView.Location = new System.Drawing.Point(0, 0);
            this.bloodlessPatientsSearchView.Model = null;
            this.bloodlessPatientsSearchView.Name = "bloodlessPatientsSearchView";
            this.bloodlessPatientsSearchView.Size = new System.Drawing.Size(897, 60);
            this.bloodlessPatientsSearchView.TabIndex = 0;

            this.bloodlessPatientsSearchView.BeforeWorkEvent +=
                new EventHandler(bloodlessPatientsResultsView.BeforeWork);
            this.bloodlessPatientsSearchView.AfterWorkEvent +=
                new EventHandler(bloodlessPatientsResultsView.AfterWork);
            // 
            // searchViewPanel
            // 
            this.searchViewPanel.Controls.Add(this.bloodlessPatientsSearchView);
            this.searchViewPanel.Location = new System.Drawing.Point(10, 0);
            this.searchViewPanel.Name = "searchViewPanel";
            this.searchViewPanel.Size = new System.Drawing.Size(897, 60);
            this.searchViewPanel.TabIndex = 0;
            // 
            // resultsViewPanel
            // 
            this.resultsViewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.resultsViewPanel.Controls.Add( this.bloodlessPatientsResultsView );
            this.resultsViewPanel.Location = new System.Drawing.Point(10, 60);
            this.resultsViewPanel.Name = "resultsViewPanel";
            this.resultsViewPanel.Size = new System.Drawing.Size(897, 374);
            this.resultsViewPanel.TabIndex = 0;
            // 
            // CensusOfBloodlessPatientsView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.searchViewPanel);
            this.Controls.Add(this.resultsViewPanel);
            this.Name = "CensusOfBloodlessPatientsView";
            this.Size = new System.Drawing.Size(926, 444);
            this.searchViewPanel.ResumeLayout(false);
            this.resultsViewPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        
        #region Properties
        #endregion

        #region Private Methods

        private void WireupBloodlessPatientsCensusSearchView()
        {
            this.bloodlessPatientsSearchView.AccountsFound   += new EventHandler( this.AccountsFound );
            this.bloodlessPatientsSearchView.ResetView     += new EventHandler( this.ResetView );
            this.bloodlessPatientsSearchView.NoAccountsFound += new EventHandler( this.NoAccountsFound );
            this.bloodlessPatientsSearchView.PrintReport += new EventHandler(bloodlessPatientsSearchView_PrintReport);

            // 
            // bloodlessPatientsResultsView
            // 
            this.bloodlessPatientsResultsView.BackColor = Color.White;
            this.bloodlessPatientsResultsView.Location = new Point(0, 0);
            this.bloodlessPatientsResultsView.Model = null;
            this.bloodlessPatientsResultsView.Name = "bloodlessPatientsResultsView";
            this.bloodlessPatientsResultsView.Size = new Size(897, 374);
            this.bloodlessPatientsResultsView.TabIndex = 1;
            this.bloodlessPatientsResultsView.AcceptButton = base.AcceptButton;
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public CensusOfBloodlessPatientsView()
        {
            InitializeComponent();
            WireupBloodlessPatientsCensusSearchView();
            base.EnableThemesOn( this );
        }

        /// <summary>
        /// Dispose method.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if ( components != null ) 
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements

        private IContainer components = null;
        private Panel searchViewPanel;
        private Panel resultsViewPanel;         

        private BloodlessPatientsSearchView bloodlessPatientsSearchView;
        private BloodlessPatientsResultsView bloodlessPatientsResultsView;        
        
        #endregion     
        
        #region Constants
        
        #endregion
        
    }
}