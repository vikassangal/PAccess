using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PhysicianPatientsCensusSearchView : ControlView
    {
        
        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
			this.physicianPatientsPanel = new System.Windows.Forms.Panel();
			this.informationLabel = new PatientAccess.UI.CommonControls.LineLabel();
			this.printReportButton = new LoggingButton();
			this.resetButton = new LoggingButton();
			this.showButton = new LoggingButton();
			this.operatingCheckBox = new System.Windows.Forms.CheckBox();
			this.consultingCheckBox = new System.Windows.Forms.CheckBox();
			this.referringCheckBox = new System.Windows.Forms.CheckBox();
			this.attendingCheckBox = new System.Windows.Forms.CheckBox();
			this.admittingCheckBox = new System.Windows.Forms.CheckBox();
			this.relationshipsLabel = new System.Windows.Forms.Label();
			this.physicianPatientsPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// physicianPatientsPanel
			// 
			this.physicianPatientsPanel.Controls.Add(this.informationLabel);
			this.physicianPatientsPanel.Controls.Add(this.printReportButton);
			this.physicianPatientsPanel.Controls.Add(this.resetButton);
			this.physicianPatientsPanel.Controls.Add(this.showButton);
			this.physicianPatientsPanel.Controls.Add(this.operatingCheckBox);
			this.physicianPatientsPanel.Controls.Add(this.consultingCheckBox);
			this.physicianPatientsPanel.Controls.Add(this.referringCheckBox);
			this.physicianPatientsPanel.Controls.Add(this.attendingCheckBox);
			this.physicianPatientsPanel.Controls.Add(this.admittingCheckBox);
			this.physicianPatientsPanel.Controls.Add(this.relationshipsLabel);
			this.physicianPatientsPanel.Location = new System.Drawing.Point(0, 0);
			this.physicianPatientsPanel.Name = "physicianPatientsPanel";
			this.physicianPatientsPanel.Size = new System.Drawing.Size(911, 61);
			this.physicianPatientsPanel.TabIndex = 0;
			// 
			// informationLabel
			// 
			this.informationLabel.Caption = "Patients of Selected Physician";
			this.informationLabel.Location = new System.Drawing.Point(7, 7);
			this.informationLabel.Name = "informationLabel";
			this.informationLabel.Size = new System.Drawing.Size(895, 16);
			this.informationLabel.TabIndex = 0;
			this.informationLabel.TabStop = false;
			// 
			// printReportButton
			// 
			this.printReportButton.Location = new System.Drawing.Point(829, 31);
			this.printReportButton.Name = "printReportButton";
			this.printReportButton.TabIndex = 8;
			this.printReportButton.Text = "Pri&nt Report";
			this.printReportButton.Click += new System.EventHandler(this.printReportButton_Click);
			// 
			// resetButton
			// 
			this.resetButton.Location = new System.Drawing.Point(697, 31);
			this.resetButton.Name = "resetButton";
			this.resetButton.TabIndex = 7;
			this.resetButton.Text = "Rese&t";
			this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
			// 
			// showButton
			// 
			this.showButton.Location = new System.Drawing.Point(615, 31);
			this.showButton.Name = "showButton";
			this.showButton.TabIndex = 6;
			this.showButton.Text = "Sh&ow";
			this.showButton.Click += new System.EventHandler(this.showButton_Click);
			// 
			// operatingCheckBox
			// 
			this.operatingCheckBox.Location = new System.Drawing.Point(530, 34);
			this.operatingCheckBox.Name = "operatingCheckBox";
			this.operatingCheckBox.Size = new System.Drawing.Size(73, 16);
			this.operatingCheckBox.TabIndex = 5;
			this.operatingCheckBox.Text = "Operating";
			// 
			// consultingCheckBox
			// 
			this.consultingCheckBox.Location = new System.Drawing.Point(446, 34);
			this.consultingCheckBox.Name = "consultingCheckBox";
			this.consultingCheckBox.Size = new System.Drawing.Size(77, 16);
			this.consultingCheckBox.TabIndex = 4;
			this.consultingCheckBox.Text = "Consulting";
			// 
			// referringCheckBox
			// 
			this.referringCheckBox.Location = new System.Drawing.Point(369, 34);
			this.referringCheckBox.Name = "referringCheckBox";
			this.referringCheckBox.Size = new System.Drawing.Size(71, 16);
			this.referringCheckBox.TabIndex = 3;
			this.referringCheckBox.Text = "Referring";
			// 
			// attendingCheckBox
			// 
			this.attendingCheckBox.Location = new System.Drawing.Point(291, 34);
			this.attendingCheckBox.Name = "attendingCheckBox";
			this.attendingCheckBox.Size = new System.Drawing.Size(71, 16);
			this.attendingCheckBox.TabIndex = 2;
			this.attendingCheckBox.Text = "Attending";
			// 
			// admittingCheckBox
			// 
			this.admittingCheckBox.Location = new System.Drawing.Point(213, 34);
			this.admittingCheckBox.Name = "admittingCheckBox";
			this.admittingCheckBox.Size = new System.Drawing.Size(71, 16);
			this.admittingCheckBox.TabIndex = 1;
			this.admittingCheckBox.Text = "Admitting";
			// 
			// relationshipsLabel
			// 
			this.relationshipsLabel.Location = new System.Drawing.Point(7, 31);
			this.relationshipsLabel.Name = "relationshipsLabel";
			this.relationshipsLabel.Size = new System.Drawing.Size(206, 16);
			this.relationshipsLabel.TabIndex = 0;
			this.relationshipsLabel.Text = "Show patients for physician relationship:";
			// 
			// PhysicianPatientsCensusSearchView
			// 
			this.AcceptButton = this.showButton;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.physicianPatientsPanel);
			this.Name = "PhysicianPatientsCensusSearchView";
			this.Size = new System.Drawing.Size(911, 61);
			this.physicianPatientsPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

        #endregion
        
        #region Event Handlers

        public event EventHandler AccountsFound;
        public event EventHandler NoAccountsFound;
        public event EventHandler PhysicianStatisitcsFound;
        public event EventHandler PatientSearchReset;
        public event EventHandler PreviousSelectedPatientMRNReset;
        public event EventHandler PrintReport;

        private void showButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                if( !admittingCheckBox.Checked &&
                    !attendingCheckBox.Checked &&
                    !referringCheckBox.Checked &&
                    !consultingCheckBox.Checked &&
                    !operatingCheckBox.Checked )
                {
                    MessageBox.Show( 
                        UIErrorMessages.ERR_MSG_PATIENTS_BY_PHYSICIAN,
                        "Error", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    return;
                }
                if( PatientSearchReset != null )
                {
                    PatientSearchReset( sender, e );
                }                           
                PhysicianPatientsSearchCriteria patientCriteria =  
                    new PhysicianPatientsSearchCriteria( 
                    User.GetCurrent().Facility,
                    SelectedPhysicianNumber,
                    this.admittingCheckBox.Checked ? 1 : 0,
                    this.attendingCheckBox.Checked ? 1 : 0,
                    this.referringCheckBox.Checked ? 1 : 0,
                    this.consultingCheckBox.Checked ? 1 : 0,
                    this.operatingCheckBox.Checked ? 1 : 0 );
            
                IAccountBroker broker = BrokerFactory.BrokerOfType<IAccountBroker >();
            
                patientsCollection = 
                    broker.AccountsMatching( patientCriteria );

                if( AccountsFound != null && patientsCollection.Count > 0 )
                {
                    AccountsFound( this, new LooseArgs( patientsCollection ) );
                    this.printReportButton.Enabled = true;
                }
                else
                {
                    if( NoAccountsFound != null )
                    {
                        NoAccountsFound( sender, e );
                    }
                    this.printReportButton.Enabled = false;
                }

                IPhysicianBroker physicianBroker = BrokerFactory.BrokerOfType<IPhysicianBroker >();
                i_Physician = 
                    physicianBroker.PhysicianStatisticsFor( 
                    User.GetCurrent().Facility.Oid, 
                    SelectedPhysicianNumber );
                i_Physician.PhysicianNumber = SelectedPhysicianNumber;
                i_Physician.Name.LastName = i_SelectedPhysicianName;

                if( PhysicianStatisitcsFound != null && i_Physician != null )
                {
                    PhysicianStatisitcsFound( this, new LooseArgs( i_Physician ) );
                }
            }
            catch (RemotingTimeoutException)
            {
                MessageBox.Show(UIErrorMessages.TIMEOUT_CENSUS_REPORT_DISPLAY);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            } 
        }

        private void printReportButton_Click(object sender, EventArgs e)
        {
            if( ( PrintReport != null ) )
            {
                PrintReport( this, new ReportArgs( patientsCollection, i_Physician, i_Physician ) );
            }
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            this.ResetPatientSearchCriteria();
            if( PreviousSelectedPatientMRNReset != null )
            {
                PreviousSelectedPatientMRNReset( sender, e );
            }
        }

        public void GetCensusForSelectedPhysician( object sender, EventArgs e )
        {
            this.showButton_Click( sender, e );
        }
        #endregion

        #region Methods

        public void ResetPatientSearchCriteria()
        {
            this.resetButton.Enabled = true;
            this.admittingCheckBox.Enabled = true;
            this.attendingCheckBox.Enabled = true;
            this.referringCheckBox.Enabled = true;
            this.consultingCheckBox.Enabled = true;
            this.operatingCheckBox.Enabled = true;

            this.admittingCheckBox.Checked   = true;
            this.attendingCheckBox.Checked   = true;
            this.referringCheckBox.Checked   = true;
            this.consultingCheckBox.Checked  = true;
            this.operatingCheckBox.Checked   = true;
            if( PatientSearchReset != null )
            {
                PatientSearchReset( this, new LooseArgs( (object)0 ) );
            }
            if( SelectedPhysicianNumber != 0 )
            {
                this.showButton.Enabled    = true;
            }
            else
            {
                this.showButton.Enabled    = false;
            }
            this.printReportButton.Enabled = false;
        }

        public void DisablePatientSearch()
        {
            this.showButton.Enabled = false;
            this.resetButton.Enabled = false;
            this.printReportButton.Enabled = false;
            this.admittingCheckBox.Enabled = false;
            this.attendingCheckBox.Enabled = false;
            this.referringCheckBox.Enabled = false;
            this.consultingCheckBox.Enabled = false;
            this.operatingCheckBox.Enabled = false;
        }
        #endregion

        #region Properties

        public long SelectedPhysicianNumber
        {
            private get
            {
                return selectedPhysicianNumber;
            }
            set
            {
                selectedPhysicianNumber = value;
            }
        }

        public string SelectedPhysicianName
        {
            get
            {
                return i_SelectedPhysicianName;
            }
            set
            {
                i_SelectedPhysicianName = value;
            }
        }

        public Physician Physician
        {
            get
            {
                return i_Physician;
            }
            set
            {
                i_Physician = value;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public PhysicianPatientsCensusSearchView()
        {
            this.InitializeComponent();
            //this.ResetPatientSearchCriteria();
            this.admittingCheckBox.Checked   = true;
            this.attendingCheckBox.Checked   = true;
            this.referringCheckBox.Checked   = true;
            this.consultingCheckBox.Checked  = true;
            this.operatingCheckBox.Checked   = true;
            this.DisablePatientSearch();
            base.EnableThemesOn( this );
        }

        #endregion

        #region Data Elements

        private Label relationshipsLabel;
        private CheckBox admittingCheckBox;
        private CheckBox attendingCheckBox;
        private CheckBox referringCheckBox;
        private CheckBox consultingCheckBox;
        private CheckBox operatingCheckBox;
        private LoggingButton showButton;
        private LoggingButton resetButton;
        private LineLabel informationLabel;
        private Panel physicianPatientsPanel;
        private LoggingButton printReportButton;
        private long selectedPhysicianNumber;
        private ICollection patientsCollection;
        private Physician i_Physician;
        private string i_SelectedPhysicianName;

        #endregion

        #region Constants
        #endregion
    }
}