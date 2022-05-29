using System;
using System.Collections;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
    ///  Census by Physician's Patients Results view
    /// </summary>
    [Serializable]
    public class PhysicianPatientsCensusResultsView : ControlView
    {
        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.physicianPatientsGridControl = new PatientAccess.UI.CommonControls.GridControl();
            this.physicianPatientsResultsPanel = new System.Windows.Forms.Panel();
            this.noPatientsFoundLabel = new System.Windows.Forms.Label();
            this.physicianPatientsResultsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // physicianPatientsGridControl
            // 
            this.physicianPatientsGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.physicianPatientsGridControl.Location = new System.Drawing.Point(0, 0);
            this.physicianPatientsGridControl.Model = null;
            this.physicianPatientsGridControl.Name = "physicianPatientsGridControl";
            this.physicianPatientsGridControl.Size = new System.Drawing.Size(895, 135);
            this.physicianPatientsGridControl.TabIndex = 1;
            this.physicianPatientsGridControl.GridControl_Click += new PatientAccess.UI.CommonControls.GridControl.UltraGridClickEventHandler(this.physicianPatientsGridControl_GridControl_Click);
            // 
            // physicianPatientsResultsPanel
            // 
            this.physicianPatientsResultsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.physicianPatientsResultsPanel.Controls.Add(this.noPatientsFoundLabel);
            this.physicianPatientsResultsPanel.Controls.Add(this.physicianPatientsGridControl);
            this.physicianPatientsResultsPanel.Location = new System.Drawing.Point(7, 7);
            this.physicianPatientsResultsPanel.Name = "physicianPatientsResultsPanel";
            this.physicianPatientsResultsPanel.Size = new System.Drawing.Size(897, 137);
            this.physicianPatientsResultsPanel.TabIndex = 0;
            // 
            // noPatientsFoundLabel
            // 
            this.noPatientsFoundLabel.Location = new System.Drawing.Point(7, 7);
            this.noPatientsFoundLabel.Name = "noPatientsFoundLabel";
            this.noPatientsFoundLabel.Size = new System.Drawing.Size(274, 16);
            this.noPatientsFoundLabel.TabIndex = 0;
            this.noPatientsFoundLabel.Text = "No patients were found based on the selected criteria.";
            // 
            // PhysicianPatientsCensusResultsView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.physicianPatientsResultsPanel);
            this.Name = "PhysicianPatientsCensusResultsView";
            this.Size = new System.Drawing.Size(911, 145);
            this.physicianPatientsResultsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        
        #endregion

        #region Event Handlers
        
        private void physicianPatientsGridControl_GridControl_Click( UltraGridRow ultraGridRow )
        {
            PreviousSelectedPatientMRN = Convert.ToInt64( 
                ultraGridRow.Cells[GRIDCOL_MRN].Value );
        }
        
        #endregion

        #region Methods

        public override void UpdateView()
        {
            this.noPatientsFoundLabel.Visible = false;
            this.physicianPatientsGridControl.Visible = true;
            FillDataSource();
            physicianPatientsGridControl.Focus(); 
        }

        public void NoPatientsFound()
        {
            this.noPatientsFoundLabel.Visible = true;
            this.physicianPatientsGridControl.Visible = false;
            PreviousSelectedPatientMRN = 0;
        }

        public void ResetPatientResults()
        {
            this.noPatientsFoundLabel.Visible = false;
            this.physicianPatientsGridControl.Visible = true;
            dataSource.Rows.Clear();
        }

        public void ResetPreviousSelectedPatient()
        {
            PreviousSelectedPatientMRN = 0;
        }
        public void SetRowSelectionActiveAppearance()
        {
           physicianPatientsGridControl.SetRowSelectionActiveAppearance();
        }
        public void SetRowSelectionInActiveAppearance()
        {
            physicianPatientsGridControl.SetRowSelectionDimAppearance();
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
            UltraDataBand mainBand = this.dataSource.Band;
            mainBand.Key = "Patient_Band";
            mainBand.Columns.Add( "Prvcy Opt", typeof(string) );
            mainBand.Columns.Add( "Location", typeof(string) );
            mainBand.Columns.Add( "Patient Name", typeof(string) );
            mainBand.Columns.Add( "MRN", typeof(long) );
            mainBand.Columns.Add( "Gen", typeof(string) );
            mainBand.Columns.Add( "Age", typeof(string) );
            mainBand.Columns.Add( "Relationship", typeof(string) );
            mainBand.Columns.Add( "Isol", typeof(string) );
            mainBand.Columns.Add( "PT", typeof(string) );            
            mainBand.Columns.Add( "Chief Complaint", typeof(string) );
            mainBand.Columns.Add( "Disch Status", typeof(string) );
        }

        private void CustomizeGridLayout()
        {
            UltraGridBand mainBand = 
                this.physicianPatientsGridControl.CensusGrid.DisplayLayout.Bands[0];

            mainBand.Columns[GRIDCOL_PRVCY_OPT].Width = 30;
            mainBand.Columns[GRIDCOL_LOCATION].Width = 40;
            mainBand.Columns[GRIDCOL_PATIENT].Width = 120;
            mainBand.Columns[GRIDCOL_MRN].Width = 35;
            mainBand.Columns[GRIDCOL_GENDER].Width = 20;
            mainBand.Columns[GRIDCOL_AGE].Width = 20;
            mainBand.Columns[GRIDCOL_RELATIONSHIP].Width = 70;
            mainBand.Columns[GRIDCOL_ISOL].Width = 15;
            mainBand.Columns[GRIDCOL_PATIENTTYPE].Width = 15;
            mainBand.Columns[GRIDCOL_CHIEFCOMPLAINT].Width = 95;
            mainBand.Columns[GRIDCOL_DISCHARGESTATUS].Width = 40;

            this.physicianPatientsGridControl.Visible = true;
            this.physicianPatientsGridControl.Show();
        }

        private void FillDataSource()
        {
            object [] accountProxyList = new object[ NUMBER_OF_COLUMNS ];
            ArrayList allAccountProxies = this.Model;
            int i = 0;

            foreach( AccountProxy accountProxy in allAccountProxies )
            {
                accountProxyList[GRIDCOL_PRVCY_OPT] = accountProxy.AddOnlyLegends();

                if( accountProxy.Location != null )
                {
                    accountProxyList[GRIDCOL_LOCATION] = accountProxy.Location.ToString();
                }

                var patientNameFormatter = new PatientNameFormatterForCensusReports(accountProxy,false);
                String patientName = patientNameFormatter.GetFormattedPatientName();

                accountProxyList[GRIDCOL_PATIENT] = patientName;
                accountProxyList[GRIDCOL_MRN] = accountProxy.Patient.MedicalRecordNumber;
                if( accountProxy.Patient.Sex != null )
                {
                    accountProxyList[GRIDCOL_GENDER] = accountProxy.Patient.Sex.Code;
                }
                accountProxyList[GRIDCOL_AGE] = accountProxy.Patient.AgeAt( 
                                DateTime.Today ).PadLeft( 4, '0').ToUpper();

                accountProxyList[GRIDCOL_RELATIONSHIP] = accountProxy.PhysicianRelationship;

                accountProxyList[GRIDCOL_ISOL] = accountProxy.IsolationCode;

                accountProxyList[GRIDCOL_PATIENTTYPE] = accountProxy.KindOfVisit.Code + 
                    "  " + accountProxy.KindOfVisit.Description;

                accountProxyList[GRIDCOL_CHIEFCOMPLAINT] = accountProxy.Diagnosis.ChiefComplaint;

                if( accountProxy.DischargeStatus != null && 
                    accountProxy.DischargeStatus.Description.Equals( PENDING_DISCHARGE ) )
                {
                    accountProxyList[GRIDCOL_DISCHARGESTATUS] = MSG_PENDING_DISCHARGE;
                }
                else
                {
                    if( accountProxy.DischargeDate.Date.Equals( DateTime.MinValue )  )
                    {
                        accountProxyList[GRIDCOL_DISCHARGESTATUS] = String.Empty;
                    }
                    else
                    {
                        accountProxyList[GRIDCOL_DISCHARGESTATUS] = 
                            accountProxy.DischargeDate.ToString( "MM/dd/yyyy" );
                    }
                }

                dataSource.Rows.Add( accountProxyList );
                
                if( PreviousSelectedPatientMRN != 0 &&
                    accountProxy.Patient.MedicalRecordNumber == 
                        PreviousSelectedPatientMRN )
                {
                    physicianPatientsGridControl.CensusGrid.ActiveRow = 
                        (UltraGridRow)physicianPatientsGridControl.CensusGrid.Rows[i];
                }
                i++;
            }
        }

        #endregion

        #region Private Properties

        private long PreviousSelectedPatientMRN
        {
            get
            {
                return previousSelectedPatientMRN;
            }
            set
            {
                previousSelectedPatientMRN = value;
            }
        }

        #endregion

        #region Construction and Finalization

        public PhysicianPatientsCensusResultsView()
        {
            this.InitializeComponent();
            this.noPatientsFoundLabel.Visible = false;
            CreateDataStructure();
            physicianPatientsGridControl.CensusGrid.DataSource = dataSource;
            CustomizeGridLayout();
            base.EnableThemesOn( this );
        }

        #endregion

        #region Data Elements
        
        private UltraDataSource dataSource;
        private Panel physicianPatientsResultsPanel;
        private Label noPatientsFoundLabel;   
        private GridControl physicianPatientsGridControl;
        private long previousSelectedPatientMRN;
        #endregion

        #region Constants

        private const string MSG_PENDING_DISCHARGE = "Pending",
            PENDING_DISCHARGE = "PENDINGDISCHARGE";
        private const int NUMBER_OF_COLUMNS = 11;
        private const int 
            GRIDCOL_PRVCY_OPT = 0,
            GRIDCOL_LOCATION = 1,
            GRIDCOL_PATIENT = 2,
            GRIDCOL_MRN = 3,
            GRIDCOL_GENDER = 4,
            GRIDCOL_AGE = 5,
            GRIDCOL_RELATIONSHIP = 6,
            GRIDCOL_ISOL = 7,
            GRIDCOL_PATIENTTYPE = 8,
            GRIDCOL_CHIEFCOMPLAINT = 9,
            GRIDCOL_DISCHARGESTATUS = 10;
        #endregion
    }
}