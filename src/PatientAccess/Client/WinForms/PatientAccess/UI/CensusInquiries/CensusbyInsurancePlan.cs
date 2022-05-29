using System;
using System.Collections;
using System.Windows.Forms;
using Extensions.UI.Winforms;

namespace PatientAccess.UI.CensusInquiries
{
    [Serializable]
    public class CensusbyInsurancePlan : ControlView
    {
        #region Event

        public event EventHandler ParentAcceptButtonChanged;

        #endregion

        #region Event Handlers

        private void AccountsFound(object sender, EventArgs e)
        {
            //Setup Results View
            insurancePlanCensusResultView.Model = null;
            insurancePlanCensusResultView.Model = ((LooseArgs)e).Context;

            insurancePlanCensusResultView.PayorType = insurancePlanCensusSearchView.PayorType;
            insurancePlanCensusResultView.SortByColumn =
                insurancePlanCensusSearchView.SortByColumn;
            insurancePlanCensusResultView.Show();
            insurancePlanCensusResultView.UpdateView();

        }

        private void NoAccountsFound(object sender, EventArgs e)
        {
            this.insurancePlanCensusResultView.NoAccountsFound();

        }

        private void ResetView(object sender, EventArgs e)
        {
            insurancePlanCensusResultView.Model = null;
            this.insurancePlanCensusResultView.ResetResultView();
            insurancePlanCensusResultView.PayorType = insurancePlanCensusSearchView.PayorType;
            insurancePlanCensusResultView.SortByColumn =
            insurancePlanCensusSearchView.SortByColumn;
        }

        private void SortOnRadioButton(object sender, EventArgs e)
        {
            insurancePlanCensusResultView.PayorType = insurancePlanCensusSearchView.PayorType;
            this.insurancePlanCensusResultView.SortByColumn =
                this.insurancePlanCensusSearchView.SortByColumn;
            this.insurancePlanCensusResultView.SortOnRadioButton();
        }

        private void AcceptButtonChanged(object sender, EventArgs e)
        {
            if (ParentAcceptButtonChanged != null)
            {
                ParentAcceptButtonChanged(this, new LooseArgs(this));
            }
        }

        private void insurancePlanCensusSearchView_PrintReport(object sender, EventArgs e)
        {
            CensusByPayorReports payorReport = new CensusByPayorReports();

            payorReport.Model = (ArrayList)((ReportArgs)e).Context;
            payorReport.SearchCriteria = (ArrayList)((ReportArgs)e).SearchCriteria;
            payorReport.PrintPreview();
        }

        #endregion

        #region Methods
        public override void UpdateView()
        {
            this.insurancePlanCensusSearchView.UpdateView();
        }
        public void SetRowSelectionActiveAppearance()
        {
            if (this.insurancePlanCensusResultView.ContainsFocus)
            {
                this.insurancePlanCensusResultView.SetRowSelectionActiveAppearance();
            }
        }
        public void SetRowSelectionInActiveAppearance()
        {
            this.insurancePlanCensusResultView.SetRowSelectionInActiveAppearance();

        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.insuranceCensusPanel = new System.Windows.Forms.Panel();
            this.insurancePlanCensusResultView = new PatientAccess.UI.CensusInquiries.InsurancePlanCensusResultView();
            this.insurancePlanCensusSearchView = new PatientAccess.UI.CensusInquiries.InsurancePlanCensusSearchView();
            this.insuranceCensusPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // insuranceCensusPanel
            // 
            this.insuranceCensusPanel.BackColor = System.Drawing.Color.White;
            this.insuranceCensusPanel.Controls.Add(this.insurancePlanCensusResultView);
            this.insuranceCensusPanel.Controls.Add(this.insurancePlanCensusSearchView);
            this.insuranceCensusPanel.Location = new System.Drawing.Point(0, 0);
            this.insuranceCensusPanel.Name = "insuranceCensusPanel";
            this.insuranceCensusPanel.Size = new System.Drawing.Size(1100, 510);
            this.insuranceCensusPanel.TabIndex = 0;
            // 
            // insurancePlanCensusResultView
            // 
            this.insurancePlanCensusResultView.BackColor = System.Drawing.Color.White;
            this.insurancePlanCensusResultView.Location = new System.Drawing.Point(10, 157);
            this.insurancePlanCensusResultView.Model = null;
            this.insurancePlanCensusResultView.Name = "insurancePlanCensusResultView";
            this.insurancePlanCensusResultView.Size = new System.Drawing.Size(912, 296);
            this.insurancePlanCensusResultView.TabIndex = 1;
            this.insurancePlanCensusResultView.AcceptButton = base.AcceptButton;
            // 
            // insurancePlanCensusSearchView
            // 
            this.insurancePlanCensusSearchView.Location = new System.Drawing.Point(10, 8);
            this.insurancePlanCensusSearchView.Model = null;
            this.insurancePlanCensusSearchView.Name = "insurancePlanCensusSearchView";
            this.insurancePlanCensusSearchView.Size = new System.Drawing.Size(912, 152);
            this.insurancePlanCensusSearchView.TabIndex = 0;

            this.insurancePlanCensusSearchView.BeforeWorkEvent +=
                new EventHandler(insurancePlanCensusResultView.BeforeWork);
            this.insurancePlanCensusSearchView.AfterWorkEvent +=
                new EventHandler(insurancePlanCensusResultView.AfterWork);
            // 
            // CensusbyInsurancePlan
            // 
            this.Controls.Add(this.insuranceCensusPanel);
            this.Name = "CensusbyInsurancePlan";
            this.Size = new System.Drawing.Size(900, 510);
            this.insuranceCensusPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Construction and Finalization

        public CensusbyInsurancePlan()
        {
            InitializeComponent();
            this.insurancePlanCensusSearchView.AccountsFound += this.AccountsFound;

            this.insurancePlanCensusSearchView.NoAccountsFound += this.NoAccountsFound;

            this.insurancePlanCensusSearchView.SortOnRadioButton += this.SortOnRadioButton;

            this.insurancePlanCensusSearchView.ResetView += this.ResetView;

            this.insurancePlanCensusSearchView.AcceptButtonChanged += this.AcceptButtonChanged;

            this.insurancePlanCensusSearchView.PrintReport += this.insurancePlanCensusSearchView_PrintReport;

            EnableThemesOn(this);

        }


        #endregion

        #region Data Elements

        private InsurancePlanCensusSearchView insurancePlanCensusSearchView;
        private InsurancePlanCensusResultView insurancePlanCensusResultView;
        private Panel insuranceCensusPanel;

        #endregion

        #region Constants
        #endregion

    }
}