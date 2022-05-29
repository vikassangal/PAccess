using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
    /// Summary description for BloodlessPatientsResultsView.
    /// </summary>
    public class BloodlessPatientsResultsView : ControlView
    {

        #region Events
        #endregion

        #region Event Handlers

        private void BloodlessPatientsResultsView_Load(object sender, EventArgs e)
        {
            this.noPatientsFoundLabel.Visible   = false;
            this.progressPanel1.Visible         = false;
        }

        #endregion
		
        #region Construction and Finalization
        
        public BloodlessPatientsResultsView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            CreateDataStructure();
            bloodlessGridControl.CensusGrid.DataSource = dataSource;
            CustomizeGridLayout();
            base.EnableThemesOn( this);
             
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bloodlessCensusResultsPanel = new System.Windows.Forms.Panel();
            this.bloodlessGridControl = new PatientAccess.UI.CommonControls.GridControl();
            this.progressPanel1 = new PatientAccess.UI.CommonControls.ProgressPanel();
            this.bloodlessCensusResultsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // bloodlessCensusResultsPanel
            //            
            this.bloodlessCensusResultsPanel.Controls.Add(this.bloodlessGridControl);
            this.bloodlessCensusResultsPanel.Controls.Add( noPatientsFoundLabel );
            this.bloodlessCensusResultsPanel.Location = new System.Drawing.Point(0, 0);
            this.bloodlessCensusResultsPanel.Name = "bloodlessCensusResultsPanel";
            this.bloodlessCensusResultsPanel.Size = new System.Drawing.Size(897, 374);
            this.bloodlessCensusResultsPanel.TabIndex = 0;
            // 
            // bloodlessGridControl
            // 
            this.bloodlessGridControl.BackColor = System.Drawing.Color.White;
            this.bloodlessGridControl.Location = new System.Drawing.Point(0, 0);
            this.bloodlessGridControl.Model = null;
            this.bloodlessGridControl.Name = "bloodlessGridControl";
            this.bloodlessGridControl.Size = new System.Drawing.Size(897, 374);
            this.bloodlessGridControl.TabIndex = 0;
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = System.Drawing.Color.White;
            this.progressPanel1.Location = new System.Drawing.Point(0, 0);
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new System.Drawing.Size(900, 370);
            this.progressPanel1.TabIndex = 0;
            // 
            // BloodlessPatientsResultsView
            // 
            this.Load +=new EventHandler(BloodlessPatientsResultsView_Load);
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.progressPanel1);
            this.Controls.Add(this.bloodlessCensusResultsPanel);
            this.Name = "BloodlessPatientsResultsView";
            this.Size = new System.Drawing.Size(897, 374);
            this.bloodlessCensusResultsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
  
        #region Methods

        public void BeforeWork(object sender, EventArgs e)
        {
            this.progressPanel1.Visible = true;
            this.progressPanel1.BringToFront();
            this.bloodlessCensusResultsPanel.Visible = false;
        }

        public void AfterWork(object sender, EventArgs e)
        {
            if (this.IsDisposed || this.Disposing)

                return;
            this.progressPanel1.Visible = false;
            this.progressPanel1.SendToBack();
            this.bloodlessCensusResultsPanel.Visible = true;
        }

        public override void UpdateView()
        {
            FillDataSource();
            bloodlessGridControl.Focus(); 
        }
        public void SetRowSelectionActiveAppearance()
        {
        bloodlessGridControl.SetRowSelectionActiveAppearance();
        }
        public void SetRowSelectionInActiveAppearance()
        {
            bloodlessGridControl.SetRowSelectionDimAppearance();
        }

        public void ResetResultsView()
        {
            this.noPatientsFoundLabel.Text = String.Empty;
            this.bloodlessCensusResultsPanel.Visible = true;
            this.dataSource.Rows.Clear();
            this.bloodlessGridControl.Visible = true;
            this.bloodlessGridControl.Show();  
        }

        public void NoAccountsFound()
        {
            this.noPatientsFoundLabel.Location = new Point(0,0);
            this.noPatientsFoundLabel.AutoSize = true;
            this.noPatientsFoundLabel.Text = MSG_NO_PATIENTS_FOUND;
            this.noPatientsFoundLabel.Visible = true;
            this.bloodlessCensusResultsPanel.Visible = true;
            this.bloodlessGridControl.Hide();
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
            mainBand.Columns.Add( "Patient", typeof(string) );
            mainBand.Columns.Add( "Physicians", typeof(string) );
            mainBand.Columns.Add( "Gen", typeof(string) );
            mainBand.Columns.Add( "Age", typeof(string) );
            mainBand.Columns.Add( "PT", typeof(string) );            
            mainBand.Columns.Add( "HSV", typeof(string) );
            mainBand.Columns.Add( "Admit Date", typeof(string) );
            mainBand.Columns.Add( "Disch Status", typeof(string) );
  
        }

        private void CustomizeGridLayout()
        {
            UltraGridBand mainBand = this.bloodlessGridControl.CensusGrid.DisplayLayout.Bands[0];

            mainBand.Columns[GRIDCOL_PATIENT].CellMultiLine = DefaultableBoolean.True;
            mainBand.Columns[GRIDCOL_PHYSICIAN].CellMultiLine = DefaultableBoolean.True;            

            mainBand.Columns[GRIDCOL_PRVCY_OPT].Width = 20;
            mainBand.Columns[GRIDCOL_LOCATION].Width = 25;
            mainBand.Columns[GRIDCOL_PATIENT].Width = 75;
            mainBand.Columns[GRIDCOL_PHYSICIAN].Width = 70;
            mainBand.Columns[GRIDCOL_SEX].Width = 15;
            mainBand.Columns[GRIDCOL_AGE].Width = 15;
            mainBand.Columns[GRIDCOL_PATIENT_TYPE].Width = 15;
            mainBand.Columns[GRIDCOL_HSV].Width = 15;
            mainBand.Columns[GRIDCOL_ADMIT_DATE].Width = 25;
            mainBand.Columns[GRIDCOL_DISCH_STATUS].Width = 25;
        }

        private void FillDataSource()
        {
            dataSource.Rows.Clear();
            object [] bloodlessPatientsInformation;  
            string [] consultingPhysiciansList;
            string attendingPhysicianName;
            string consultingPhysicianNames;
            string physicianNames;
            
            ArrayList allAccountProxies = this.Model;

            if( allAccountProxies != null
                && allAccountProxies.Count > 0)
            {
                foreach( AccountProxy accountProxy in allAccountProxies )
                {
                    bloodlessPatientsInformation = new object[NUMBER_OF_COLUMNS];
                    attendingPhysicianName = String.Empty;
                    consultingPhysicianNames = String.Empty;
                    physicianNames = String.Empty;
   
                    bloodlessPatientsInformation[ GRIDCOL_PRVCY_OPT] = accountProxy.AddOnlyLegends();
                
                    if( accountProxy.Location != null )
                    {
                        bloodlessPatientsInformation[ GRIDCOL_LOCATION ] = 
                            accountProxy.Location.ToString();
                    }

                    var patientNameFormatter = new PatientNameFormatterForCensusReports(accountProxy,false);
                    String patientName = patientNameFormatter.GetFormattedPatientName();
                        
                    bloodlessPatientsInformation[ GRIDCOL_PATIENT ] = String.Format( 
                        "{0}\n          Account:{1}",
                        patientName, accountProxy.AccountNumber);
                    if( accountProxy.AttendingPhysician != null )
                    {
                        attendingPhysicianName = String.Format( "Att:   {0}" ,
                            accountProxy.AttendingPhysician );
                    }
                    
                    else
                    {
                        attendingPhysicianName = "Att:   ";
                    }
               
                    if( accountProxy.ConsultingPhysicians != null )
                    {
                        consultingPhysiciansList = 
                            new string[accountProxy.ConsultingPhysicians.Count];
                        for( int index = 0; index < accountProxy.ConsultingPhysicians.Count; index++ )
                        {
                            if( accountProxy.ConsultingPhysicians[index] != null )
                            {
                                Physician consultingPhysician = 
                                    ( Physician )accountProxy.ConsultingPhysicians[index];
                                consultingPhysiciansList[index] = consultingPhysician.ToString();
                            }
                        }
                    
                        consultingPhysicianNames = string.Join( "\n", consultingPhysiciansList );
                    }
                
                    if( consultingPhysicianNames.Trim().Equals( string.Empty ) )
                    {
                        physicianNames = attendingPhysicianName;
                    }
                    
                    else
                    {
                        physicianNames = String.Format( "{0}\nCon: {1}",
                            attendingPhysicianName ,consultingPhysicianNames );
                    }
                
                
                    bloodlessPatientsInformation[ GRIDCOL_PHYSICIAN ] = physicianNames;
                    if( accountProxy.Patient.Sex != null )
                    {
                        bloodlessPatientsInformation[ GRIDCOL_SEX ] = accountProxy.Patient.Sex.Code;
                    }
                    bloodlessPatientsInformation[ GRIDCOL_AGE ] = 
                        accountProxy.Patient.AgeAt( DateTime.Today ).
                        PadLeft( 4, '0').ToUpper();
                
                    if( accountProxy.KindOfVisit != null )
                    {
                        bloodlessPatientsInformation[ GRIDCOL_PATIENT_TYPE ] = 
                            String.Format( "{0} {1}",
                            accountProxy.KindOfVisit.Code, 
                            accountProxy.KindOfVisit.Description );
                    }

                    if( accountProxy.HospitalService != null )
                    {
                        bloodlessPatientsInformation[ GRIDCOL_HSV ] = 
                            String.Format( "{0} {1}",
                            accountProxy.HospitalService.Code, 
                            accountProxy.HospitalService.Description );
                    }
                    bloodlessPatientsInformation[ GRIDCOL_ADMIT_DATE ] = 
                        accountProxy.AdmitDate.ToString( "MM/dd/yyyy" );

                    if( accountProxy.DischargeStatus != null && 
                        accountProxy.DischargeStatus.Description.Equals( PENDING_DISCHARGE ) )
                    {
                        bloodlessPatientsInformation[ GRIDCOL_DISCH_STATUS ] = 
                            MSG_PENDING_DISCHARGE;
                    }

                    else
                    {
                        if( accountProxy.DischargeDate.Equals( DateTime.MinValue )  )
                        {
                            bloodlessPatientsInformation[ GRIDCOL_DISCH_STATUS ] = String.Empty;
                        }

                        else
                        {
                            bloodlessPatientsInformation[ GRIDCOL_DISCH_STATUS ] = 
                                accountProxy.DischargeDate.ToString( "MM/dd/yyyy" );
                        }
                    }
           
                    dataSource.Rows.Add( bloodlessPatientsInformation );
                }

            }
        }

        #endregion

        #region Data Elements

        private Container                     components = null;

        private UltraDataSource                                     dataSource;
        private GridControl                                         bloodlessGridControl;

        private ProgressPanel       progressPanel1;

        private Panel                          bloodlessCensusResultsPanel;        
        private Label                          noPatientsFoundLabel = new Label();

        #endregion       

        #region Constants
        private const string MSG_PENDING_DISCHARGE = "Pending",
            MSG_NO_PATIENTS_FOUND = "No patients were found based on the selected criteria.",
            PENDING_DISCHARGE = "PENDINGDISCHARGE";

        private const int GRIDCOL_PRVCY_OPT = 0,
            GRIDCOL_LOCATION = 1,
            GRIDCOL_PATIENT = 2,
            GRIDCOL_PHYSICIAN = 3,
            GRIDCOL_SEX = 4,
            GRIDCOL_AGE = 5,
            GRIDCOL_PATIENT_TYPE = 6,
            GRIDCOL_HSV = 7,
            GRIDCOL_ADMIT_DATE = 8,
            GRIDCOL_DISCH_STATUS = 9,
            NUMBER_OF_COLUMNS = 10;

        #endregion

    }
}
