using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.PatientSearch;

namespace PatientAccess.UI.PreRegistrationViews
{
    /// <summary>
    /// Summary description for CancelPreRegConfirmationView.
    /// </summary>
    public class CancelPreRegConfirmationView : ControlView
    {
        #region Events
        public event EventHandler CloseActivity;
        #endregion

        #region Event Handlers
        private void btnCloseActivity_Click(object sender, EventArgs e)
        {
            CloseActivity( this, new LooseArgs( this.Model ) );
        }

        private void btnRepeatActivity_Click(object sender, EventArgs e)
        { 
            cancelPreRegistrationView.Dispose();
            SearchEventAggregator.GetInstance().RaiseCancelPreRegistrationEvent( this, null );
            Dispose();
        }

        private void CancelPreRegConfirmationView_Load(object sender, EventArgs e)
        {
            this.UpdateView();

            // clear any FUS notes that were manually entered so that - if the View FUS notes icon/menu option is selected,
            // the newly added notes do not show twice.

            if( ViewFactory.Instance.CreateView<PatientAccessView>().Model != null )
            {
                ( ViewFactory.Instance.CreateView<PatientAccessView>().Model as Account ).ClearFusNotes();
            }

        }

        #endregion

        #region Methods
        /// <summary>
        /// UpdateView method.
        /// </summary>
        public override void UpdateView()
        {
            if( Model_Account != null )
            {
                Patient patient            = Model_Account.Patient;
                patientContextView.Model   = patient;
                patientContextView.Account = Model_Account;
                patientContextView.UpdateView();

                lblAccount.Text            = Model_Account.AccountNumber.ToString();
                lblPatientType.Text        = Model_Account.DerivedVisitType;
                if( Model_Account.AdmittingPhysician != null )
                {
                    lblAdmittingPhysician.Text = String.Format("{0:00000} {1}", 
                                                               Model_Account.AdmittingPhysician.PhysicianNumber, 
                                                               Model_Account.AdmittingPhysician.FormattedName );  
                }
                if( Model_Account.HospitalClinic != null )
                {
                    lblClinic1.Text            = Model_Account.HospitalClinic.ToCodedString();
                }
                lblContact1.Text           = Model_Account.EmergencyContact1.Name;
                lblPatientName.Text        = patient.Name.AsFormattedName();
                lblDOB.Text                = patient.DateOfBirth.ToString( "MM/dd/yyyy" );
//                lblAdmitDate.Text           = Model_Account.AdmitDate.ToString( "MM/dd/yyyy" );
//                lblTime.Text                = Model_Account.AdmitDate.ToString( "mm:ss" );
                Address address            = patient.ContactPointWith(
                    TypeOfContactPoint.NewMailingContactPointType() ).Address;
                txtMailingAddress.Text     = address.AsMailingLabel();
            }

            // OTD# 37031 fix - Disable icon/menu options for FUS notes upon confirmation
            ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( false );
        }

        #endregion

        #region Properties

        private Account Model_Account
        {
            get
            {
                return (Account)this.Model;
            }
        }
        #endregion

        #region Private Methods

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelPatientContext = new System.Windows.Forms.Panel();
            this.patientContextView = new PatientAccess.UI.PatientContextView();
            this.panelBackground = new System.Windows.Forms.Panel();
            this.panelControls = new System.Windows.Forms.Panel();
            this.btnRepeatActivity = new LoggingButton();
            this.btnCloseActivity = new LoggingButton();
            this.lineLabel = new PatientAccess.UI.CommonControls.LineLabel();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblStaticTime = new System.Windows.Forms.Label();
            this.lblAdmitDate = new System.Windows.Forms.Label();
            this.lblStaticAdmitDate = new System.Windows.Forms.Label();
            this.lblContact1 = new System.Windows.Forms.Label();
            this.lblStaticContact1 = new System.Windows.Forms.Label();
            this.lblClinic5 = new System.Windows.Forms.Label();
            this.lblStaticClinic5 = new System.Windows.Forms.Label();
            this.lblClinic4 = new System.Windows.Forms.Label();
            this.lblStaticClinic4 = new System.Windows.Forms.Label();
            this.lblClinic3 = new System.Windows.Forms.Label();
            this.lblStaticClinic3 = new System.Windows.Forms.Label();
            this.lblClinic2 = new System.Windows.Forms.Label();
            this.lblStaticClinic2 = new System.Windows.Forms.Label();
            this.lblClinic1 = new System.Windows.Forms.Label();
            this.lblStaticClinic1 = new System.Windows.Forms.Label();
            this.lblAdmittingPhysician = new System.Windows.Forms.Label();
            this.lblStaticPhysician = new System.Windows.Forms.Label();
            this.txtMailingAddress = new System.Windows.Forms.TextBox();
            this.lblDOB = new System.Windows.Forms.Label();
            this.lblPatientType = new System.Windows.Forms.Label();
            this.lblAccount = new System.Windows.Forms.Label();
            this.lblPatientName = new System.Windows.Forms.Label();
            this.lblStaticAddress = new System.Windows.Forms.Label();
            this.lblStaticDOB = new System.Windows.Forms.Label();
            this.lblStaticPatientType = new System.Windows.Forms.Label();
            this.lblStaticAccount = new System.Windows.Forms.Label();
            this.lblStaticPatientName = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.userContextView = new PatientAccess.UI.UserContextView();
            this.panelPatientContext.SuspendLayout();
            this.panelBackground.SuspendLayout();
            this.panelControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelPatientContext
            // 
            this.panelPatientContext.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPatientContext.Controls.Add(this.patientContextView);
            this.panelPatientContext.Location = new System.Drawing.Point(8, 24);
            this.panelPatientContext.Name = "panelPatientContext";
            this.panelPatientContext.Size = new System.Drawing.Size(1008, 22);
            this.panelPatientContext.TabIndex = 5;
            // 
            // patientContextView
            // 
            this.patientContextView.Account = null;
            this.patientContextView.BackColor = System.Drawing.Color.White;
            this.patientContextView.DateOfBirthText = "";
            this.patientContextView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.patientContextView.ForeColor = System.Drawing.SystemColors.ControlText;
            this.patientContextView.GenderLabelText = "";
            this.patientContextView.Location = new System.Drawing.Point(0, 0);
            this.patientContextView.Model = null;
            this.patientContextView.Name = "patientContextView";
            this.patientContextView.PatientNameText = "";
            this.patientContextView.Size = new System.Drawing.Size(1008, 22);
            this.patientContextView.TabIndex = 0;
            this.patientContextView.TabStop = false;
            // 
            // panelBackground
            // 
            this.panelBackground.BackColor = System.Drawing.Color.Black;
            this.panelBackground.Controls.Add(this.panelControls);
            this.panelBackground.Location = new System.Drawing.Point(8, 56);
            this.panelBackground.Name = "panelBackground";
            this.panelBackground.Size = new System.Drawing.Size(1008, 524);
            this.panelBackground.TabIndex = 1;
            // 
            // panelControls
            // 
            this.panelControls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.panelControls.BackColor = System.Drawing.Color.White;
            this.panelControls.Controls.Add(this.btnRepeatActivity);
            this.panelControls.Controls.Add(this.btnCloseActivity);
            this.panelControls.Controls.Add(this.lineLabel);
            this.panelControls.Controls.Add(this.lblTime);
            this.panelControls.Controls.Add(this.lblStaticTime);
            this.panelControls.Controls.Add(this.lblAdmitDate);
            this.panelControls.Controls.Add(this.lblStaticAdmitDate);
            this.panelControls.Controls.Add(this.lblContact1);
            this.panelControls.Controls.Add(this.lblStaticContact1);
            this.panelControls.Controls.Add(this.lblClinic5);
            this.panelControls.Controls.Add(this.lblStaticClinic5);
            this.panelControls.Controls.Add(this.lblClinic4);
            this.panelControls.Controls.Add(this.lblStaticClinic4);
            this.panelControls.Controls.Add(this.lblClinic3);
            this.panelControls.Controls.Add(this.lblStaticClinic3);
            this.panelControls.Controls.Add(this.lblClinic2);
            this.panelControls.Controls.Add(this.lblStaticClinic2);
            this.panelControls.Controls.Add(this.lblClinic1);
            this.panelControls.Controls.Add(this.lblStaticClinic1);
            this.panelControls.Controls.Add(this.lblAdmittingPhysician);
            this.panelControls.Controls.Add(this.lblStaticPhysician);
            this.panelControls.Controls.Add(this.txtMailingAddress);
            this.panelControls.Controls.Add(this.lblDOB);
            this.panelControls.Controls.Add(this.lblPatientType);
            this.panelControls.Controls.Add(this.lblAccount);
            this.panelControls.Controls.Add(this.lblPatientName);
            this.panelControls.Controls.Add(this.lblStaticAddress);
            this.panelControls.Controls.Add(this.lblStaticDOB);
            this.panelControls.Controls.Add(this.lblStaticPatientType);
            this.panelControls.Controls.Add(this.lblStaticAccount);
            this.panelControls.Controls.Add(this.lblStaticPatientName);
            this.panelControls.Controls.Add(this.lblMessage);
            this.panelControls.Location = new System.Drawing.Point(1, 1);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(1006, 522);
            this.panelControls.TabIndex = 0;
            // 
            // btnRepeatActivity
            // 
            this.btnRepeatActivity.BackColor = System.Drawing.SystemColors.Control;
            this.btnRepeatActivity.Location = new System.Drawing.Point(98, 464);
            this.btnRepeatActivity.Name = "btnRepeatActivity";
            this.btnRepeatActivity.Size = new System.Drawing.Size(88, 23);
            this.btnRepeatActivity.TabIndex = 2;
            this.btnRepeatActivity.Text = "Repeat Acti&vity";
            this.btnRepeatActivity.Click += new System.EventHandler(this.btnRepeatActivity_Click);
            // 
            // btnCloseActivity
            // 
            this.btnCloseActivity.BackColor = System.Drawing.SystemColors.Control;
            this.btnCloseActivity.Location = new System.Drawing.Point(8, 464);
            this.btnCloseActivity.Name = "btnCloseActivity";
            this.btnCloseActivity.Size = new System.Drawing.Size(80, 23);
            this.btnCloseActivity.TabIndex = 1;
            this.btnCloseActivity.Text = "C&lose Activity";
            this.btnCloseActivity.Click += new System.EventHandler(this.btnCloseActivity_Click);
            // 
            // lineLabel
            // 
            this.lineLabel.Caption = "Next Action";
            this.lineLabel.Location = new System.Drawing.Point(8, 432);
            this.lineLabel.Name = "lineLabel";
            this.lineLabel.Size = new System.Drawing.Size(992, 18);
            this.lineLabel.TabIndex = 0;
            this.lineLabel.TabStop = false;
            // 
            // lblTime
            // 
            this.lblTime.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTime.Location = new System.Drawing.Point(242, 406);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(56, 23);
            this.lblTime.TabIndex = 0;
            // 
            // lblStaticTime
            // 
            this.lblStaticTime.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticTime.Location = new System.Drawing.Point(208, 406);
            this.lblStaticTime.Name = "lblStaticTime";
            this.lblStaticTime.Size = new System.Drawing.Size(33, 23);
            this.lblStaticTime.TabIndex = 0;
            this.lblStaticTime.Text = "Time:";
            // 
            // lblAdmitDate
            // 
            this.lblAdmitDate.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblAdmitDate.Location = new System.Drawing.Point(130, 406);
            this.lblAdmitDate.Name = "lblAdmitDate";
            this.lblAdmitDate.Size = new System.Drawing.Size(75, 23);
            this.lblAdmitDate.TabIndex = 0;
            // 
            // lblStaticAdmitDate
            // 
            this.lblStaticAdmitDate.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticAdmitDate.Location = new System.Drawing.Point(16, 406);
            this.lblStaticAdmitDate.Name = "lblStaticAdmitDate";
            this.lblStaticAdmitDate.Size = new System.Drawing.Size(118, 23);
            this.lblStaticAdmitDate.TabIndex = 0;
            this.lblStaticAdmitDate.Text = "Scheduled admit date:";
            // 
            // lblContact1
            // 
            this.lblContact1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblContact1.Location = new System.Drawing.Point(130, 380);
            this.lblContact1.Name = "lblContact1";
            this.lblContact1.Size = new System.Drawing.Size(300, 23);
            this.lblContact1.TabIndex = 0;
            // 
            // lblStaticContact1
            // 
            this.lblStaticContact1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticContact1.Location = new System.Drawing.Point(16, 380);
            this.lblStaticContact1.Name = "lblStaticContact1";
            this.lblStaticContact1.Size = new System.Drawing.Size(113, 23);
            this.lblStaticContact1.TabIndex = 0;
            this.lblStaticContact1.Text = "Emergency contact 1:";
            // 
            // lblClinic5
            // 
            this.lblClinic5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblClinic5.Location = new System.Drawing.Point(130, 354);
            this.lblClinic5.Name = "lblClinic5";
            this.lblClinic5.Size = new System.Drawing.Size(300, 23);
            this.lblClinic5.TabIndex = 0;
            // 
            // lblStaticClinic5
            // 
            this.lblStaticClinic5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticClinic5.Location = new System.Drawing.Point(16, 354);
            this.lblStaticClinic5.Name = "lblStaticClinic5";
            this.lblStaticClinic5.TabIndex = 0;
            this.lblStaticClinic5.Text = "Clinic 5:";
            // 
            // lblClinic4
            // 
            this.lblClinic4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblClinic4.Location = new System.Drawing.Point(130, 334);
            this.lblClinic4.Name = "lblClinic4";
            this.lblClinic4.Size = new System.Drawing.Size(300, 23);
            this.lblClinic4.TabIndex = 0;
            // 
            // lblStaticClinic4
            // 
            this.lblStaticClinic4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticClinic4.Location = new System.Drawing.Point(16, 334);
            this.lblStaticClinic4.Name = "lblStaticClinic4";
            this.lblStaticClinic4.TabIndex = 0;
            this.lblStaticClinic4.Text = "Clinic 4:";
            // 
            // lblClinic3
            // 
            this.lblClinic3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblClinic3.Location = new System.Drawing.Point(130, 314);
            this.lblClinic3.Name = "lblClinic3";
            this.lblClinic3.Size = new System.Drawing.Size(300, 23);
            this.lblClinic3.TabIndex = 0;
            // 
            // lblStaticClinic3
            // 
            this.lblStaticClinic3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticClinic3.Location = new System.Drawing.Point(16, 314);
            this.lblStaticClinic3.Name = "lblStaticClinic3";
            this.lblStaticClinic3.TabIndex = 0;
            this.lblStaticClinic3.Text = "Clinic 3:";
            // 
            // lblClinic2
            // 
            this.lblClinic2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lblClinic2.Location = new System.Drawing.Point(130, 294);
            this.lblClinic2.Name = "lblClinic2";
            this.lblClinic2.Size = new System.Drawing.Size(300, 23);
            this.lblClinic2.TabIndex = 0;
            // 
            // lblStaticClinic2
            // 
            this.lblStaticClinic2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticClinic2.Location = new System.Drawing.Point(16, 294);
            this.lblStaticClinic2.Name = "lblStaticClinic2";
            this.lblStaticClinic2.TabIndex = 0;
            this.lblStaticClinic2.Text = "Clinic 2:";
            // 
            // lblClinic1
            // 
            this.lblClinic1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblClinic1.Location = new System.Drawing.Point(130, 274);
            this.lblClinic1.Name = "lblClinic1";
            this.lblClinic1.Size = new System.Drawing.Size(300, 23);
            this.lblClinic1.TabIndex = 0;
            // 
            // lblStaticClinic1
            // 
            this.lblStaticClinic1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticClinic1.Location = new System.Drawing.Point(16, 274);
            this.lblStaticClinic1.Name = "lblStaticClinic1";
            this.lblStaticClinic1.TabIndex = 0;
            this.lblStaticClinic1.Text = "Clinic 1:";
            // 
            // lblAdmittingPhysician
            // 
            this.lblAdmittingPhysician.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblAdmittingPhysician.Location = new System.Drawing.Point(130, 248);
            this.lblAdmittingPhysician.Name = "lblAdmittingPhysician";
            this.lblAdmittingPhysician.Size = new System.Drawing.Size(300, 23);
            this.lblAdmittingPhysician.TabIndex = 0;
            // 
            // lblStaticPhysician
            // 
            this.lblStaticPhysician.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticPhysician.Location = new System.Drawing.Point(16, 248);
            this.lblStaticPhysician.Name = "lblStaticPhysician";
            this.lblStaticPhysician.Size = new System.Drawing.Size(105, 23);
            this.lblStaticPhysician.TabIndex = 0;
            this.lblStaticPhysician.Text = "Admitting physician:";
            // 
            // txtMailingAddress
            // 
            this.txtMailingAddress.BackColor = System.Drawing.Color.White;
            this.txtMailingAddress.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMailingAddress.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtMailingAddress.Location = new System.Drawing.Point(130, 171);
            this.txtMailingAddress.Multiline = true;
            this.txtMailingAddress.Name = "txtMailingAddress";
            this.txtMailingAddress.ReadOnly = true;
            this.txtMailingAddress.Size = new System.Drawing.Size(200, 55);
            this.txtMailingAddress.TabIndex = 0;
            this.txtMailingAddress.TabStop = false;
            this.txtMailingAddress.Text = "";
            // 
            // lblDOB
            // 
            this.lblDOB.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDOB.Location = new System.Drawing.Point(130, 145);
            this.lblDOB.Name = "lblDOB";
            this.lblDOB.Size = new System.Drawing.Size(150, 23);
            this.lblDOB.TabIndex = 0;
            // 
            // lblPatientType
            // 
            this.lblPatientType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblPatientType.Location = new System.Drawing.Point(130, 119);
            this.lblPatientType.Name = "lblPatientType";
            this.lblPatientType.Size = new System.Drawing.Size(200, 23);
            this.lblPatientType.TabIndex = 0;
            // 
            // lblAccount
            // 
            this.lblAccount.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblAccount.Location = new System.Drawing.Point(130, 93);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(200, 23);
            this.lblAccount.TabIndex = 0;
            // 
            // lblPatientName
            // 
            this.lblPatientName.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblPatientName.Location = new System.Drawing.Point(130, 66);
            this.lblPatientName.Name = "lblPatientName";
            this.lblPatientName.Size = new System.Drawing.Size(500, 23);
            this.lblPatientName.TabIndex = 0;
            // 
            // lblStaticAddress
            // 
            this.lblStaticAddress.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticAddress.Location = new System.Drawing.Point(16, 171);
            this.lblStaticAddress.Name = "lblStaticAddress";
            this.lblStaticAddress.Size = new System.Drawing.Size(90, 23);
            this.lblStaticAddress.TabIndex = 0;
            this.lblStaticAddress.Text = "Mailing address:";
            // 
            // lblStaticDOB
            // 
            this.lblStaticDOB.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticDOB.Location = new System.Drawing.Point(16, 145);
            this.lblStaticDOB.Name = "lblStaticDOB";
            this.lblStaticDOB.Size = new System.Drawing.Size(90, 23);
            this.lblStaticDOB.TabIndex = 0;
            this.lblStaticDOB.Text = "DOB:";
            // 
            // lblStaticPatientType
            // 
            this.lblStaticPatientType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticPatientType.Location = new System.Drawing.Point(16, 119);
            this.lblStaticPatientType.Name = "lblStaticPatientType";
            this.lblStaticPatientType.Size = new System.Drawing.Size(90, 23);
            this.lblStaticPatientType.TabIndex = 0;
            this.lblStaticPatientType.Text = "Patient type:";
            // 
            // lblStaticAccount
            // 
            this.lblStaticAccount.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticAccount.Location = new System.Drawing.Point(16, 93);
            this.lblStaticAccount.Name = "lblStaticAccount";
            this.lblStaticAccount.Size = new System.Drawing.Size(90, 23);
            this.lblStaticAccount.TabIndex = 0;
            this.lblStaticAccount.Text = "Account:";
            // 
            // lblStaticPatientName
            // 
            this.lblStaticPatientName.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStaticPatientName.Location = new System.Drawing.Point(16, 66);
            this.lblStaticPatientName.Name = "lblStaticPatientName";
            this.lblStaticPatientName.Size = new System.Drawing.Size(90, 23);
            this.lblStaticPatientName.TabIndex = 0;
            this.lblStaticPatientName.Text = "Patient name:";
            // 
            // lblMessage
            // 
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblMessage.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblMessage.Location = new System.Drawing.Point(16, 16);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(975, 23);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "Cancel Preregistration submitted for processing.";
            // 
            // userContextView
            // 
            this.userContextView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.userContextView.BackColor = System.Drawing.SystemColors.Control;
            this.userContextView.Description = "Cancel Preregistration - Submitted";
            this.userContextView.Location = new System.Drawing.Point(0, 0);
            this.userContextView.Model = null;
            this.userContextView.Name = "userContextView";
            this.userContextView.Size = new System.Drawing.Size(1024, 23);
            this.userContextView.TabIndex = 4;
            this.userContextView.TabStop = false;
            // 
            // CancelPreRegConfirmationView
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.Controls.Add(this.panelPatientContext);
            this.Controls.Add(this.panelBackground);
            this.Controls.Add(this.userContextView);
            this.Name = "CancelPreRegConfirmationView";
            this.Size = new System.Drawing.Size(1024, 620);
            this.Load += new System.EventHandler(this.CancelPreRegConfirmationView_Load);
            this.panelPatientContext.ResumeLayout(false);
            this.panelBackground.ResumeLayout(false);
            this.panelControls.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
        
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public CancelPreRegConfirmationView( CancelPreRegistrationView parent )
        {
            cancelPreRegistrationView = parent;
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            base.EnableThemesOn( this );
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

        #region Data Elements
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;
        private Panel panelPatientContext;
        private PatientContextView patientContextView;
        private Panel panelBackground;
        private Panel panelControls;
        private Label lblTime;
        private Label lblStaticTime;
        private Label lblAdmitDate;
        private Label lblStaticAdmitDate;
        private Label lblContact1;
        private Label lblStaticContact1;
        private Label lblClinic5;
        private Label lblStaticClinic5;
        private Label lblClinic4;
        private Label lblStaticClinic4;
        private Label lblClinic3;
        private Label lblStaticClinic3;
        private Label lblClinic2;
        private Label lblStaticClinic2;
        private Label lblClinic1;
        private Label lblStaticClinic1;
        private Label lblAdmittingPhysician;
        private Label lblStaticPhysician;
        private TextBox txtMailingAddress;
        private Label lblDOB;
        private Label lblPatientType;
        private Label lblAccount;
        private Label lblPatientName;
        private Label lblStaticAddress;
        private Label lblStaticDOB;
        private Label lblStaticPatientType;
        private Label lblStaticAccount;
        private Label lblStaticPatientName;
        private Label lblMessage;
        private UserContextView userContextView;
        private LineLabel lineLabel;
        private LoggingButton btnCloseActivity;
        private LoggingButton btnRepeatActivity;
        private CancelPreRegistrationView       cancelPreRegistrationView;
        #endregion

        #region Constants
        #endregion
    }
}
