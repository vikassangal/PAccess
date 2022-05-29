using System;
using System.Collections;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain.Parties;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CensusbyPhysicianView : ControlView
    {

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.physicianSearchPanel = new System.Windows.Forms.Panel();
            this.physicianCensusSearchView = new PatientAccess.UI.CensusInquiries.PhysicianCensusSearchView();
            this.physicianResultsPanel = new System.Windows.Forms.Panel();
            this.physicianCensusResultsView = new PatientAccess.UI.CensusInquiries.PhysicianCensusResultsView();
            this.physicianPatientSearchPanel = new System.Windows.Forms.Panel();
            this.physicianPatientsCensusSearchView = new PatientAccess.UI.CensusInquiries.PhysicianPatientsCensusSearchView();
            this.physicianPatientResultsPanel = new System.Windows.Forms.Panel();
            this.physicianPatientsCensusResultsView = new PatientAccess.UI.CensusInquiries.PhysicianPatientsCensusResultsView();
            this.physicianCensusSummaryPanel = new System.Windows.Forms.Panel();
            this.physicianCensusSummaryView = new PatientAccess.UI.CensusInquiries.PhysicianCensusSummaryView();
            this.physicianSearchPanel.SuspendLayout();
            this.physicianResultsPanel.SuspendLayout();
            this.physicianPatientSearchPanel.SuspendLayout();
            this.physicianPatientResultsPanel.SuspendLayout();
            this.physicianCensusSummaryPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // physicianSearchPanel
            // 
            this.physicianSearchPanel.Controls.Add(this.physicianCensusSearchView);
            this.physicianSearchPanel.Location = new System.Drawing.Point(0, 0);
            this.physicianSearchPanel.Name = "physicianSearchPanel";
            this.physicianSearchPanel.Size = new System.Drawing.Size(416, 167);
            this.physicianSearchPanel.TabIndex = 0;
            // 
            // physicianCensusSearchView
            // 
            this.physicianCensusSearchView.BackColor = System.Drawing.Color.White;
            this.physicianCensusSearchView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.physicianCensusSearchView.Location = new System.Drawing.Point(0, 0);
            this.physicianCensusSearchView.Model = null;
            this.physicianCensusSearchView.Name = "physicianCensusSearchView";
            this.physicianCensusSearchView.Size = new System.Drawing.Size(416, 167);
            this.physicianCensusSearchView.TabIndex = 0;
            // 
            // physicianResultsPanel
            // 
            this.physicianResultsPanel.Controls.Add(this.physicianCensusResultsView);
            this.physicianResultsPanel.Location = new System.Drawing.Point(440, 0);
            this.physicianResultsPanel.Name = "physicianResultsPanel";
            this.physicianResultsPanel.Size = new System.Drawing.Size(416, 167);
            this.physicianResultsPanel.TabIndex = 1;
            // 
            // physicianCensusResultsView
            // 
            this.physicianCensusResultsView.BackColor = System.Drawing.Color.White;
            this.physicianCensusResultsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.physicianCensusResultsView.Location = new System.Drawing.Point(0, 0);
            this.physicianCensusResultsView.Model = null;
            this.physicianCensusResultsView.Name = "physicianCensusResultsView";
            this.physicianCensusResultsView.SelectedPhysicianName = null;
            this.physicianCensusResultsView.Size = new System.Drawing.Size(416, 167);
            this.physicianCensusResultsView.TabIndex = 0;
            // 
            // physicianPatientSearchPanel
            // 
            this.physicianPatientSearchPanel.Controls.Add(this.physicianPatientsCensusSearchView);
            this.physicianPatientSearchPanel.Location = new System.Drawing.Point(0, 168);
            this.physicianPatientSearchPanel.Name = "physicianPatientSearchPanel";
            this.physicianPatientSearchPanel.Size = new System.Drawing.Size(911, 61);
            this.physicianPatientSearchPanel.TabIndex = 2;
            // 
            // physicianPatientsCensusSearchView
            // 
            this.physicianPatientsCensusSearchView.BackColor = System.Drawing.Color.White;
            this.physicianPatientsCensusSearchView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.physicianPatientsCensusSearchView.Location = new System.Drawing.Point(0, 0);
            this.physicianPatientsCensusSearchView.Model = null;
            this.physicianPatientsCensusSearchView.Name = "physicianPatientsCensusSearchView";
            this.physicianPatientsCensusSearchView.Physician = null;
            this.physicianPatientsCensusSearchView.SelectedPhysicianName = null;
            this.physicianPatientsCensusSearchView.SelectedPhysicianNumber = ((long)(0));
            this.physicianPatientsCensusSearchView.Size = new System.Drawing.Size(911, 61);
            this.physicianPatientsCensusSearchView.TabIndex = 0;
            // 
            // physicianPatientResultsPanel
            // 
            this.physicianPatientResultsPanel.Controls.Add(this.physicianPatientsCensusResultsView);
            this.physicianPatientResultsPanel.Location = new System.Drawing.Point(0, 229);
            this.physicianPatientResultsPanel.Name = "physicianPatientResultsPanel";
            this.physicianPatientResultsPanel.Size = new System.Drawing.Size(911, 145);
            this.physicianPatientResultsPanel.TabIndex = 3;
            // 
            // physicianPatientsCensusResultsView
            // 
            this.physicianPatientsCensusResultsView.BackColor = System.Drawing.Color.White;
            this.physicianPatientsCensusResultsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.physicianPatientsCensusResultsView.Location = new System.Drawing.Point(0, 0);
            this.physicianPatientsCensusResultsView.Model = null;
            this.physicianPatientsCensusResultsView.Name = "physicianPatientsCensusResultsView";
            this.physicianPatientsCensusResultsView.Size = new System.Drawing.Size(911, 145);
            this.physicianPatientsCensusResultsView.TabIndex = 0;
            // 
            // physicianCensusSummaryPanel
            // 
            this.physicianCensusSummaryPanel.Controls.Add(this.physicianCensusSummaryView);
            this.physicianCensusSummaryPanel.Location = new System.Drawing.Point(104, 374);
            this.physicianCensusSummaryPanel.Name = "physicianCensusSummaryPanel";
            this.physicianCensusSummaryPanel.Size = new System.Drawing.Size(716, 90);
            this.physicianCensusSummaryPanel.TabIndex = 4;
            // 
            // physicianCensusSummaryView
            // 
            this.physicianCensusSummaryView.BackColor = System.Drawing.Color.White;
            this.physicianCensusSummaryView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.physicianCensusSummaryView.Location = new System.Drawing.Point(0, 0);
            this.physicianCensusSummaryView.Model = null;
            this.physicianCensusSummaryView.Name = "physicianCensusSummaryView";
            this.physicianCensusSummaryView.Size = new System.Drawing.Size(716, 90);
            this.physicianCensusSummaryView.TabIndex = 0;
			this.physicianCensusSummaryView.TabStop = false;
            // 
            // CensusbyPhysicianView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.physicianCensusSummaryPanel);
            this.Controls.Add(this.physicianPatientResultsPanel);
            this.Controls.Add(this.physicianPatientSearchPanel);
            this.Controls.Add(this.physicianResultsPanel);
            this.Controls.Add(this.physicianSearchPanel);
            this.Name = "CensusbyPhysicianView";
            this.Size = new System.Drawing.Size(911, 464);
            this.physicianSearchPanel.ResumeLayout(false);
            this.physicianResultsPanel.ResumeLayout(false);
            this.physicianPatientSearchPanel.ResumeLayout(false);
            this.physicianPatientResultsPanel.ResumeLayout(false);
            this.physicianCensusSummaryPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

		#region Event

		public event EventHandler ParentAcceptButtonChanged;
        public event EventHandler FocusToBtnClose;
		#endregion
      
        #region Event Handlers

        private void PhysiciansFound( object sender, EventArgs e )
        {
            this.i_HasPhysicians = true;

            physicianCensusResultsView.Model = null;
            physicianCensusResultsView.Model = ((LooseArgs)e).Context;
            physicianCensusResultsView.Show();
            physicianCensusResultsView.UpdateView();
        }

        private void NoPhysiciansFound( object sender, EventArgs e )
        {
            this.i_HasPhysicians = false;

            physicianCensusResultsView.NoPhysiciansFound();
            physicianPatientsCensusSearchView.DisablePatientSearch();
            physicianPatientsCensusResultsView.ResetPatientResults();
            physicianCensusSummaryView.ResetPhysicianSummary();
        }

        private void AccountsFound( object sender, EventArgs e )
        {
            physicianPatientsCensusResultsView.Model = null;
            physicianPatientsCensusResultsView.Model = ( ArrayList )((LooseArgs)e).Context;
            physicianPatientsCensusResultsView.Show();
            physicianPatientsCensusResultsView.UpdateView();
        }

        private void NoAccountsFound( object sender, EventArgs e )
        {
            physicianPatientsCensusResultsView.NoPatientsFound();
            physicianCensusSummaryView.ResetPhysicianSummary();
        }

        private void PhysicianStatisitcsFound( object sender, EventArgs e )
        {
            physicianCensusSummaryView.Model = null;
            physicianCensusSummaryView.Model = ((LooseArgs)e).Context;
            physicianCensusSummaryView.Show();
            physicianCensusSummaryView.UpdateView();
        }

        private void PhysicianSelectionChanged( object sender, EventArgs e )
        {
            physicianPatientsCensusSearchView.SelectedPhysicianNumber = 
                Convert.ToInt64( ((LooseArgs)e).Context );
            physicianPatientsCensusSearchView.SelectedPhysicianName = 
                physicianCensusResultsView.SelectedPhysicianName;
            physicianPatientsCensusSearchView.ResetPatientSearchCriteria();
            physicianPatientsCensusResultsView.ResetPatientResults();
            physicianCensusSummaryView.ResetPhysicianSummary();
        }

        private void PhysicianSearchReset( object sender, EventArgs e )
        {
            this.i_HasPhysicians = false;

            physicianCensusResultsView.ResetPhysicianResults();
            physicianPatientsCensusSearchView.DisablePatientSearch();
            physicianPatientsCensusResultsView.ResetPatientResults();
            physicianCensusSummaryView.ResetPhysicianSummary();
        }

        private void PreviousSelectedPhysicianReset( object sender, EventArgs e )
        {
            physicianCensusResultsView.ResetPreviousSelectedPhysician();
        }

        private void PreviousSelectedPatientMRNReset( object sender, EventArgs e )
        {
            physicianPatientsCensusResultsView.ResetPreviousSelectedPatient();
        }

        private void PatientSearchReset( object sender, EventArgs e )
        {
            physicianPatientsCensusResultsView.ResetPatientResults();
            physicianCensusSummaryView.ResetPhysicianSummary();
        }

        private void PrintReport( object sender, EventArgs e )
        {
            PhysicianPatientsCensusReport physicianPatientsCensusReport
                = new PhysicianPatientsCensusReport();
            physicianPatientsCensusReport.Model = null;
            physicianPatientsCensusReport.Model = ( ArrayList )( ( ReportArgs )e ).Context;
            physicianPatientsCensusReport.SearchCriteria = 
                ( Physician )( ( ReportArgs )e ).SearchCriteria;
            physicianPatientsCensusReport.SummaryInformation = 
                ( Physician )( ( ReportArgs )e ).Summary;                
            physicianPatientsCensusReport.PrintPreview();
        }

        private void AcceptButtonChanged( object sender, EventArgs e )
		{
			if( ParentAcceptButtonChanged != null )
			{
				ParentAcceptButtonChanged( this, new LooseArgs( this ) );
			}  
		}

        private void FocusOutOfBtnResetHandler( object sender, EventArgs e )
        {
            if( FocusToBtnClose != null && !this.i_HasPhysicians )
            {
                this.FocusToBtnClose( null, EventArgs.Empty );
            }
        }

        private void PhysicianDoubleClicked( object sender, EventArgs e )
        {
            physicianPatientsCensusSearchView.SelectedPhysicianNumber =
                Convert.ToInt64( ( (LooseArgs)e ).Context );
            physicianPatientsCensusSearchView.SelectedPhysicianName =
                physicianCensusResultsView.SelectedPhysicianName;
            physicianPatientsCensusSearchView.ResetPatientSearchCriteria();

            physicianPatientsCensusSearchView.GetCensusForSelectedPhysician( sender, e );
        }
        #endregion

        #region Methods
        public void SetRowSelectionActiveAppearance()
        {
            if( this.physicianCensusResultsView.ContainsFocus  )
            {
                this.physicianCensusResultsView.SetRowSelectionActiveAppearance();
            }
            if( this.physicianPatientsCensusResultsView.ContainsFocus  )
            {
                this.physicianPatientsCensusResultsView.SetRowSelectionActiveAppearance();
            }
          
        }
        public void SetRowSelectionInActiveAppearance()
        {
            this.physicianCensusResultsView.SetRowSelectionInActiveAppearance();
            this.physicianPatientsCensusResultsView.SetRowSelectionInActiveAppearance();
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        
        private void WireupPhysicianSearchView()
        {
            physicianCensusSearchView.PhysiciansFound      += 
                                new EventHandler( this.PhysiciansFound );
            physicianCensusSearchView.NoPhysiciansFound    += 
                                new EventHandler( this.NoPhysiciansFound );
            physicianCensusSearchView.PhysicianSearchReset += 
                                new EventHandler( PhysicianSearchReset );
            physicianCensusSearchView.PreviousSelectedPhysicianReset +=
                        new EventHandler( this.PreviousSelectedPhysicianReset );
			physicianCensusSearchView.AcceptButtonChanged +=
				new EventHandler( this.AcceptButtonChanged );
            physicianCensusSearchView.FocusOutOfBtnReset +=
                new EventHandler( this.FocusOutOfBtnResetHandler );
        }
        
        private void WireupPhysicianPatientsSearchView()
        {
            physicianPatientsCensusSearchView.AccountsFound        += 
                                    new EventHandler( this.AccountsFound );
            physicianPatientsCensusSearchView.NoAccountsFound      += 
                                    new EventHandler( this.NoAccountsFound );
            physicianPatientsCensusSearchView.PatientSearchReset   +=
                                    new EventHandler( this.PatientSearchReset );
            physicianPatientsCensusSearchView.PreviousSelectedPatientMRNReset +=
                new EventHandler( this.PreviousSelectedPatientMRNReset );
        }
        
        private void WireupPhysicianCensusSummaryView()
        {
            physicianPatientsCensusSearchView.PhysicianStatisitcsFound  += 
                            new EventHandler( this.PhysicianStatisitcsFound );
        }

        private void WireupPhysicianCensusResultsView()
        {
            physicianCensusResultsView.PhysicianSelectionChanged   += 
                new EventHandler( this.PhysicianSelectionChanged );
            physicianCensusResultsView.PhysicianDoubleClicked +=
                new EventHandler( this.PhysicianDoubleClicked );
            physicianPatientsCensusSearchView.PrintReport += 
                new EventHandler(PrintReport);
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public CensusbyPhysicianView()
        {
            this.InitializeComponent();
            WireupPhysicianSearchView();
            WireupPhysicianCensusResultsView();
            WireupPhysicianPatientsSearchView();
            WireupPhysicianCensusSummaryView();
            base.EnableThemesOn( this );
        }
       
        #endregion

        #region Data Elements

        private Panel physicianSearchPanel;
        private Panel physicianResultsPanel;
        private Panel physicianPatientSearchPanel;
        private Panel physicianPatientResultsPanel;
        private Panel physicianCensusSummaryPanel;
        private PhysicianCensusSearchView physicianCensusSearchView;
        private PhysicianCensusResultsView physicianCensusResultsView;
        private PhysicianCensusSummaryView physicianCensusSummaryView;
        private PhysicianPatientsCensusResultsView physicianPatientsCensusResultsView;
        private PhysicianPatientsCensusSearchView physicianPatientsCensusSearchView;
       
        private bool i_HasPhysicians = false;            
        #endregion

        #region Constants
        #endregion
    }
}