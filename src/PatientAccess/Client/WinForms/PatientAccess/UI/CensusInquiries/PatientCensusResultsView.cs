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
    /// Summary description for PatientCensusResultsView.
    /// </summary>
    [Serializable]
    public class PatientCensusResultsView : ControlView
    {

        #region Events
        #endregion

        #region Event Handlers

        private void PatientCensusResultsView_Load(object sender, EventArgs e)
        {
            this.progressPanel1.Visible         = false;
            this.noPatientsFoundLabel.Visible   = false;
        }

        #endregion

        #region Construction and Finalization

        public PatientCensusResultsView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            CreateDataStructure();
            patientGridControl.CensusGrid.DataSource = dataSource;
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
            this.progressPanel1 = new ProgressPanel();
            this.patientCensusResultsPanel = new System.Windows.Forms.Panel();
            this.patientCensusResultsGridPanel = new System.Windows.Forms.Panel();
            this.headingLabel = new System.Windows.Forms.Label();
            this.messageLabel = new System.Windows.Forms.Label();
            this.patientGridControl = 
                new PatientAccess.UI.CommonControls.GridControl();
            this.patientCensusResultsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // patientCensusResultsPanel
            // 
            this.patientGridControl.BackColor = System.Drawing.Color.Bisque;
            this.patientCensusResultsPanel.Controls.Add(this.messageLabel);
            this.patientCensusResultsPanel.Controls.Add(this.headingLabel);
            this.patientCensusResultsPanel.Controls.Add(this.patientCensusResultsGridPanel);
            this.patientCensusResultsPanel.Location = new System.Drawing.Point(0, 0);
            this.patientCensusResultsPanel.Name = "patientCensusResultsPanel";
            this.patientCensusResultsPanel.Size = new System.Drawing.Size(896, 360);
            this.patientCensusResultsPanel.TabIndex = 0;
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = System.Drawing.Color.White;
            this.progressPanel1.Location = new System.Drawing.Point(0,0);
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new System.Drawing.Size(900,370);
            this.progressPanel1.TabIndex = 3;

            // 
            // patientCensusResultsGridPanel
            // 
            this.patientCensusResultsGridPanel.Controls.Add(this.patientGridControl);
            this.patientCensusResultsGridPanel.Controls.Add( 
                noPatientsFoundLabel );
            this.patientCensusResultsGridPanel.BorderStyle 
                = System.Windows.Forms.BorderStyle.FixedSingle;
            this.patientCensusResultsGridPanel.Location = new System.Drawing.Point(0, 55);
            this.patientCensusResultsGridPanel.Name = "patientCensusResultsGridPanel";
            this.patientCensusResultsGridPanel.Size 
                = new System.Drawing.Size(896, 302); 
            this.patientCensusResultsGridPanel.TabIndex = 1;
            this.patientCensusResultsGridPanel.TabStop  = true;
            // 
            // headingLabel
            // 
            this.headingLabel.Location = new System.Drawing.Point(0, 0);
            this.headingLabel.Font 
                = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.headingLabel.Size = new System.Drawing.Size(872, 24);
            this.headingLabel.Name = "headingLabel";
            this.headingLabel.TabIndex = 0;
            this.headingLabel.Text = "Search Results";
            // 
            // messageLabel
            // 
            this.messageLabel.Location = new System.Drawing.Point(0, 26);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Size = new System.Drawing.Size(872, 16);
            this.messageLabel.TabIndex = 0;
            this.messageLabel.Text 
                = "If no appropriate result is found, try modifying your search criteria above and search again. If y" +
                "ou still cannot locate the appropriate patient, use the Edit/Maintain Account activity.";

            // 
            // patientGridControl
            // 
            this.patientGridControl.BackColor = System.Drawing.Color.White;
            this.patientGridControl.Location = new System.Drawing.Point(0, 0);
            this.patientGridControl.Model = null;
            this.patientGridControl.Name = "patientGridControl";
            this.patientGridControl.Size = new System.Drawing.Size(896, 302); 
            this.patientGridControl.TabIndex = 0;

            // 
            // PatientCensusResultsView
            // 
            this.Load +=new EventHandler(PatientCensusResultsView_Load);
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.progressPanel1);
            this.Controls.Add(this.patientCensusResultsPanel);
            this.Name = "PatientCensusResultsView";
            this.Size = new System.Drawing.Size(896, 340); 
            this.patientCensusResultsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        

        #endregion

        #region Methods

        public void BeforeWork(object sender, EventArgs e)
        {
            this.progressPanel1.Visible = true;
            this.progressPanel1.BringToFront();
            this.patientCensusResultsPanel.Visible = false;
        }

        public void AfterWork(object sender, EventArgs e)
        {
            if (this.IsDisposed || this.Disposing)
                return;
            this.progressPanel1.Visible = false;
            this.progressPanel1.SendToBack();
            this.patientCensusResultsPanel.Visible = true;
        }

        public void DisplayNoAccountsFound()
        {
            noPatientsFoundLabel.CreateControl();
            noPatientsFoundLabel.Location = new Point(8,0);
            noPatientsFoundLabel.AutoSize = true;
            noPatientsFoundLabel.Text = MSG_NO_PATIENTS_FOUND;
            this.patientCensusResultsGridPanel.BackColor 
                = Color.White;

            this.noPatientsFoundLabel.Visible = true;

            this.patientCensusResultsGridPanel.Visible = true;
            this.patientGridControl.Hide();
        }

        public void ResetView()
        {
            this.noPatientsFoundLabel.Text = "";
            this.patientCensusResultsGridPanel.Visible = true;
            dataSource.Rows.Clear();
            this.patientGridControl.Visible = true;
            this.patientGridControl.Show();            
        }
        public override void UpdateView()
        {
            FillDataSource();
            patientGridControl.Focus();
           
        }
        public void SetRowSelectionActiveAppearance()
        {
            patientGridControl.SetRowSelectionActiveAppearance();
        }
        public void SetRowSelectionInActiveAppearance()
        {
            patientGridControl.SetRowSelectionDimAppearance();
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
            mainBand.Columns.Add( "Isol", typeof(string) );
            mainBand.Columns.Add( "PT", typeof(string) );            
            mainBand.Columns.Add( "HSV", typeof(string) );
            mainBand.Columns.Add( "Bldls", typeof(string) );
            mainBand.Columns.Add( "Admit Date", typeof(string) );
            mainBand.Columns.Add( "Disch Status", typeof(string) );
        }

        private void CustomizeGridLayout()
        {
            UltraGridBand mainBand = 
                this.patientGridControl.CensusGrid.DisplayLayout.Bands[0];

            mainBand.Columns[ GRIDCOL_PATIENT ].CellMultiLine = 
                DefaultableBoolean.True;
            mainBand.Columns[ GRIDCOL_PHYSICIANS ].CellMultiLine = 
                DefaultableBoolean.True;

            mainBand.Columns[GRIDCOL_PRVCY_OPT].Width = 20;
            mainBand.Columns[ GRIDCOL_LOCATION ].Width = 25;
            mainBand.Columns[ GRIDCOL_PATIENT ].Width = 75;
            mainBand.Columns[ GRIDCOL_PHYSICIANS ].Width = 70;
            mainBand.Columns[ GRIDCOL_ISOLATION ].Width = 6;
            mainBand.Columns[ GRIDCOL_PATIENTTYPE ].Width = 4;
            mainBand.Columns[ GRIDCOL_HOSPITALSERVICECODE ].Width = 10;
            mainBand.Columns[ GRIDCOL_BLOODLESSPATIENT ].Width = 10;
            mainBand.Columns[ GRIDCOL_ADMITDATE ].Width = 20;
            mainBand.Columns[ GRIDCOL_DISCHARGESTATUS ].Width = 22;
        }

        private void FillDataSource()
        {
            dataSource.Rows.Clear();
            object [] PatientCensusInformation;  
            string [] consultingPhysiciansList;
            string admittingPhysicianFullName;
            string attendingPhysicianFullName;
            string consultingPhysicianNames;
            string physicianNames;
            
            ArrayList allAccountProxies = this.Model;

            if( allAccountProxies != null
                && allAccountProxies.Count > 0)
            {
                foreach( AccountProxy accountProxy in allAccountProxies )
                {
                    PatientCensusInformation = new object[ NUMBER_OF_COLUMNS ];
                    admittingPhysicianFullName = String.Empty;
                    attendingPhysicianFullName= String.Empty;
                    consultingPhysicianNames = String.Empty;
                    physicianNames = String.Empty;

                    PatientCensusInformation[GRIDCOL_PRVCY_OPT] = 
                        accountProxy.AddOnlyLegends();
   
                    if( accountProxy.Location != null )
                    {
                        PatientCensusInformation[ GRIDCOL_LOCATION ] = 
                            accountProxy.Location.ToString();
                    }

                    var patientNameFormatter = new PatientNameFormatterForCensusReports(accountProxy,false);
                    String patientName = patientNameFormatter.GetFormattedPatientName();

                    PatientCensusInformation[GRIDCOL_PATIENT] =
                        patientName+
                        "\n          Account: " + accountProxy.AccountNumber;

                    if( accountProxy.AdmittingPhysician != null )
                    {
                        admittingPhysicianFullName = "Adm:  " 
                            + accountProxy.AdmittingPhysician;
                    }
                    else
                    {
                        admittingPhysicianFullName = "Adm:  ";
                    }

                    if( accountProxy.AttendingPhysician != null )
                    {
                        attendingPhysicianFullName= "Att:     " 
                            + accountProxy.AttendingPhysician;
                    }
                    else
                    {
                        attendingPhysicianFullName= "Att:     ";
                    }
               
                    if( accountProxy.ConsultingPhysicians != null )
                    {
                        consultingPhysiciansList = 
                            new string[accountProxy.ConsultingPhysicians.Count];

                        for( int index = 0; 
                            index < accountProxy.ConsultingPhysicians.Count; 
                            index++ )
                        {
                            if( accountProxy.ConsultingPhysicians[index] != null )
                            {
                                Physician consultingPhysician = 
                                    ( Physician )
                                    accountProxy.ConsultingPhysicians[ index ];

                                consultingPhysiciansList[index] 
                                    = "  " + consultingPhysician;
                            }
                        }
                    
                        consultingPhysicianNames 
                            = string.Join( "\n", consultingPhysiciansList );
                    }
                
                    if( consultingPhysicianNames.Trim().Equals( string.Empty ) )
                    {
                        physicianNames = admittingPhysicianFullName + "\n" + 
                            attendingPhysicianFullName; 
                    }
                    else
                    {
                        physicianNames = admittingPhysicianFullName + "\n" + 
                            attendingPhysicianFullName+ "\n" + 
                            "Con: " + consultingPhysicianNames;
                    }
                
                    PatientCensusInformation[ GRIDCOL_PHYSICIANS ] = 
                        physicianNames;

                    PatientCensusInformation[ GRIDCOL_ISOLATION ] = 
                        accountProxy.IsolationCode;

                    PatientCensusInformation[ GRIDCOL_PATIENTTYPE ] = 
                        accountProxy.KindOfVisit.Code + 
                        " " + accountProxy.KindOfVisit.Description; 

                    if( accountProxy.HospitalService != null )
                    {
                        PatientCensusInformation[ GRIDCOL_HOSPITALSERVICECODE ] = 
                            accountProxy.HospitalService.Code + " " + 
                            accountProxy.HospitalService.Description ;  
                    }
                                    
                    PatientCensusInformation[ GRIDCOL_BLOODLESSPATIENT ] = 
                        accountProxy.Patient.BloodlessPatient;

                    PatientCensusInformation[ GRIDCOL_ADMITDATE ] = 
                        accountProxy.AdmitDate.Date.ToString( "MM/dd/yyyy" );

                    if( accountProxy.DischargeStatus != null && 
                        accountProxy.DischargeStatus.
                        Description.Equals( PENDINGDISCHARGE ) )
                    {
                        PatientCensusInformation[ GRIDCOL_DISCHARGESTATUS ] = 
                            DISCHARGESTATUS;
                    }
                    else
                    {
                        if( 
                            accountProxy.DischargeDate.Date.Equals( 
                            DateTime.MinValue )  )
                        {
                            PatientCensusInformation[ GRIDCOL_DISCHARGESTATUS ] =
                                String.Empty;
                        }
                        else
                        {
                            PatientCensusInformation[ GRIDCOL_DISCHARGESTATUS ] = 
                                accountProxy.DischargeDate.Date.ToString( "MM/dd/yyyy" );
                        }
                    }
           
                    dataSource.Rows.Add( PatientCensusInformation );
                }

            }
        }


        #endregion

        #region Private Properties
        #endregion        

        #region Data Elements

        private Container                     components = null;

        private ProgressPanel                                       progressPanel1;
        private UltraDataSource                                     dataSource;  
        private GridControl                                         patientGridControl;

        private Label                          headingLabel;
        private Label                          messageLabel;
        private Label                          noPatientsFoundLabel = new Label();

        private Panel                          patientCensusResultsPanel;        
        private Panel                          patientCensusResultsGridPanel;
        
        #endregion

        #region Constants
        private const string DISCHARGESTATUS            =   "Pending";
        private const string PENDINGDISCHARGE           =   "PENDINGDISCHARGE";
        private const string MSG_NO_PATIENTS_FOUND 
            =  "No patients were found for the patient information entered.";
        private const int 
            GRIDCOL_PRVCY_OPT             = 0,
            GRIDCOL_LOCATION              = 1,
            GRIDCOL_PATIENT               = 2,
            GRIDCOL_PHYSICIANS            = 3,
            GRIDCOL_ISOLATION             = 4,
            GRIDCOL_PATIENTTYPE           = 5,
            GRIDCOL_HOSPITALSERVICECODE   = 6,
            GRIDCOL_BLOODLESSPATIENT      = 7,
            GRIDCOL_ADMITDATE             = 8,
            GRIDCOL_DISCHARGESTATUS       = 9,
            NUMBER_OF_COLUMNS             = 10;
        #endregion

    }
}
