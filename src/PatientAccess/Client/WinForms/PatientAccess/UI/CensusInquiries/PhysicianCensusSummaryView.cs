using System;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain.Parties;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
    /// Census by Physician Summary view
    /// </summary>
    [Serializable]
    public class PhysicianCensusSummaryView : ControlView
    {
        
        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.physicianStatisticsPanel = new System.Windows.Forms.Panel();
            this.physicianStatisticsGroupBox = new System.Windows.Forms.GroupBox();
            this.operatingPhysicianValueLabel = new System.Windows.Forms.Label();
            this.consultingPhysicianValueLabel = new System.Windows.Forms.Label();
            this.operatingPhysicianLabel = new System.Windows.Forms.Label();
            this.consultingPhysicianLabel = new System.Windows.Forms.Label();
            this.referringPhysicianValueLabel = new System.Windows.Forms.Label();
            this.admittingPhysicianValueLabel = new System.Windows.Forms.Label();
            this.referringPhysicianLabel = new System.Windows.Forms.Label();
            this.admittingPhysicianLabel = new System.Windows.Forms.Label();
            this.attendingPhysicianValueLabel = new System.Windows.Forms.Label();
            this.totalPatientsValueLabel = new System.Windows.Forms.Label();
            this.attendingPhysicianLabel = new System.Windows.Forms.Label();
            this.totalPatientsLabel = new System.Windows.Forms.Label();
            this.physicianStatisticsPanel.SuspendLayout();
            this.physicianStatisticsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // physicianStatisticsPanel
            // 
            this.physicianStatisticsPanel.Controls.Add(this.physicianStatisticsGroupBox);
            this.physicianStatisticsPanel.Location = new System.Drawing.Point(0, 0);
            this.physicianStatisticsPanel.Name = "physicianStatisticsPanel";
            this.physicianStatisticsPanel.Size = new System.Drawing.Size(702, 77);
            this.physicianStatisticsPanel.TabIndex = 0;
            this.physicianStatisticsPanel.Tag = "";
            // 
            // physicianStatisticsGroupBox
            // 
            this.physicianStatisticsGroupBox.Controls.Add(this.operatingPhysicianValueLabel);
            this.physicianStatisticsGroupBox.Controls.Add(this.consultingPhysicianValueLabel);
            this.physicianStatisticsGroupBox.Controls.Add(this.operatingPhysicianLabel);
            this.physicianStatisticsGroupBox.Controls.Add(this.consultingPhysicianLabel);
            this.physicianStatisticsGroupBox.Controls.Add(this.referringPhysicianValueLabel);
            this.physicianStatisticsGroupBox.Controls.Add(this.admittingPhysicianValueLabel);
            this.physicianStatisticsGroupBox.Controls.Add(this.referringPhysicianLabel);
            this.physicianStatisticsGroupBox.Controls.Add(this.admittingPhysicianLabel);
            this.physicianStatisticsGroupBox.Controls.Add(this.attendingPhysicianValueLabel);
            this.physicianStatisticsGroupBox.Controls.Add(this.totalPatientsValueLabel);
            this.physicianStatisticsGroupBox.Controls.Add(this.attendingPhysicianLabel);
            this.physicianStatisticsGroupBox.Controls.Add(this.totalPatientsLabel);
            this.physicianStatisticsGroupBox.Location = new System.Drawing.Point(7, 7);
            this.physicianStatisticsGroupBox.Name = "physicianStatisticsGroupBox";
            this.physicianStatisticsGroupBox.Size = new System.Drawing.Size(695, 70);
            this.physicianStatisticsGroupBox.TabIndex = 0;
            this.physicianStatisticsGroupBox.TabStop = false;
            this.physicianStatisticsGroupBox.Text = "Statistical summary for selected physician (Inpatient Only)";
            // 
            // operatingPhysicianValueLabel
            // 
            this.operatingPhysicianValueLabel.Location = new System.Drawing.Point(647, 47);
            this.operatingPhysicianValueLabel.Name = "operatingPhysicianValueLabel";
            this.operatingPhysicianValueLabel.Size = new System.Drawing.Size(40, 16);
            this.operatingPhysicianValueLabel.TabIndex = 0;
            // 
            // consultingPhysicianValueLabel
            // 
            this.consultingPhysicianValueLabel.Location = new System.Drawing.Point(647, 23);
            this.consultingPhysicianValueLabel.Name = "consultingPhysicianValueLabel";
            this.consultingPhysicianValueLabel.Size = new System.Drawing.Size(40, 16);
            this.consultingPhysicianValueLabel.TabIndex = 0;
            // 
            // operatingPhysicianLabel
            // 
            this.operatingPhysicianLabel.Location = new System.Drawing.Point(474, 47);
            this.operatingPhysicianLabel.Name = "operatingPhysicianLabel";
            this.operatingPhysicianLabel.Size = new System.Drawing.Size(168, 16);
            this.operatingPhysicianLabel.TabIndex = 0;
            this.operatingPhysicianLabel.Text = "Patients as operating physician:";
            // 
            // consultingPhysicianLabel
            // 
            this.consultingPhysicianLabel.Location = new System.Drawing.Point(474, 23);
            this.consultingPhysicianLabel.Name = "consultingPhysicianLabel";
            this.consultingPhysicianLabel.Size = new System.Drawing.Size(168, 16);
            this.consultingPhysicianLabel.TabIndex = 0;
            this.consultingPhysicianLabel.Text = "Patients as consulting physician:";
            // 
            // referringPhysicianValueLabel
            // 
            this.referringPhysicianValueLabel.Location = new System.Drawing.Point(414, 47);
            this.referringPhysicianValueLabel.Name = "referringPhysicianValueLabel";
            this.referringPhysicianValueLabel.Size = new System.Drawing.Size(40, 16);
            this.referringPhysicianValueLabel.TabIndex = 0;
            // 
            // admittingPhysicianValueLabel
            // 
            this.admittingPhysicianValueLabel.Location = new System.Drawing.Point(414, 23);
            this.admittingPhysicianValueLabel.Name = "admittingPhysicianValueLabel";
            this.admittingPhysicianValueLabel.Size = new System.Drawing.Size(40, 16);
            this.admittingPhysicianValueLabel.TabIndex = 0;
            // 
            // referringPhysicianLabel
            // 
            this.referringPhysicianLabel.Location = new System.Drawing.Point(241, 47);
            this.referringPhysicianLabel.Name = "referringPhysicianLabel";
            this.referringPhysicianLabel.Size = new System.Drawing.Size(168, 16);
            this.referringPhysicianLabel.TabIndex = 0;
            this.referringPhysicianLabel.Text = "Patients as referring physician:";
            // 
            // admittingPhysicianLabel
            // 
            this.admittingPhysicianLabel.Location = new System.Drawing.Point(241, 23);
            this.admittingPhysicianLabel.Name = "admittingPhysicianLabel";
            this.admittingPhysicianLabel.Size = new System.Drawing.Size(168, 16);
            this.admittingPhysicianLabel.TabIndex = 0;
            this.admittingPhysicianLabel.Text = "Patients as admitting physician:";
            // 
            // attendingPhysicianValueLabel
            // 
            this.attendingPhysicianValueLabel.Location = new System.Drawing.Point(181, 47);
            this.attendingPhysicianValueLabel.Name = "attendingPhysicianValueLabel";
            this.attendingPhysicianValueLabel.Size = new System.Drawing.Size(40, 16);
            this.attendingPhysicianValueLabel.TabIndex = 0;
            // 
            // totalPatientsValueLabel
            // 
            this.totalPatientsValueLabel.Location = new System.Drawing.Point(181, 23);
            this.totalPatientsValueLabel.Name = "totalPatientsValueLabel";
            this.totalPatientsValueLabel.Size = new System.Drawing.Size(40, 16);
            this.totalPatientsValueLabel.TabIndex = 0;
            // 
            // attendingPhysicianLabel
            // 
            this.attendingPhysicianLabel.Location = new System.Drawing.Point(8, 47);
            this.attendingPhysicianLabel.Name = "attendingPhysicianLabel";
            this.attendingPhysicianLabel.Size = new System.Drawing.Size(168, 16);
            this.attendingPhysicianLabel.TabIndex = 0;
            this.attendingPhysicianLabel.Text = "Patients as attending physician:";
            // 
            // totalPatientsLabel
            // 
            this.totalPatientsLabel.Location = new System.Drawing.Point(8, 23);
            this.totalPatientsLabel.Name = "totalPatientsLabel";
            this.totalPatientsLabel.Size = new System.Drawing.Size(168, 16);
            this.totalPatientsLabel.TabIndex = 0;
            this.totalPatientsLabel.Text = "Total patients:";
            // 
            // PhysicianCensusSummaryView
            // 
            this.Controls.Add(this.physicianStatisticsPanel);
            this.BackColor = System.Drawing.Color.White;
            this.Name = "PhysicianCensusSummaryView";
            this.Size = new System.Drawing.Size(702, 77);
            this.physicianStatisticsPanel.ResumeLayout(false);
            this.physicianStatisticsGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        
        #region Event Handlers
        #endregion

        #region Methods

        public override void UpdateView()
        {
            physician = ( Physician )this.Model;
            ShowPhysicianSummary();
        }

        public void ResetPhysicianSummary()
        {
            this.totalPatientsValueLabel.Text          = "";
            this.attendingPhysicianValueLabel.Text     = "";
            this.admittingPhysicianValueLabel.Text     = "";
            this.referringPhysicianValueLabel.Text     = "";
            this.consultingPhysicianValueLabel.Text    = "";
            this.operatingPhysicianValueLabel.Text     = "";
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        
        private void ShowPhysicianSummary()
        {
            this.totalPatientsValueLabel.Text          = 
                            physician.TotalPatients.ToString();
            this.attendingPhysicianValueLabel.Text     = 
                            physician.TotalAttendingPatients.ToString();
            this.admittingPhysicianValueLabel.Text     = 
                            physician.TotalAdmittingPatients.ToString();
            this.referringPhysicianValueLabel.Text     = 
                            physician.TotalReferringPatients.ToString();
            this.consultingPhysicianValueLabel.Text    = 
                            physician.TotalConsultingPatients.ToString();
            this.operatingPhysicianValueLabel.Text     = 
                            physician.TotalOperatingPatients.ToString();
        
        }
        
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public PhysicianCensusSummaryView()
        {
            this.InitializeComponent();
            base.EnableThemesOn( this );
        }

        #endregion

        #region Data Elements

        private GroupBox physicianStatisticsGroupBox;
        private Label totalPatientsLabel;
        private Label attendingPhysicianLabel;
        private Label totalPatientsValueLabel;
        private Label attendingPhysicianValueLabel;
        private Label referringPhysicianValueLabel;
        private Label admittingPhysicianValueLabel;
        private Label referringPhysicianLabel;
        private Label admittingPhysicianLabel;
        private Label operatingPhysicianValueLabel;
        private Label consultingPhysicianValueLabel;
        private Label operatingPhysicianLabel;
        private Label consultingPhysicianLabel;
        private Panel physicianStatisticsPanel;
        private Physician physician;

        #endregion

        #region Constants
        #endregion

    }
}