using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.Reports.FaceSheet;

namespace PatientAccess.UI.PreMSEViews
{
    /// <summary>
    /// Summary description for PreMseConfirmation.
    /// </summary>
    [Serializable]
    public class PreMseConfirmation : ControlView
    {
        #region Events
        public event EventHandler RepeatActivity;
        public event EventHandler CloseActivity;
        #endregion

        #region Event Handlers

        private void btnPrintFaceSheet_Click(object sender, EventArgs e)
        {
            var faceSheetPrintService = new PrintService( this.Model_Account );
            faceSheetPrintService.Print( );        
        }

        private void btnRepeatActivity_Click(object sender, EventArgs e)
        {
            RepeatActivity( this, new LooseArgs( new Patient() ) );
        }

        private void btnCloseActivity_Click(object sender, EventArgs e)
        {
            CloseActivity( this, new LooseArgs( this.Model ) );
        }

        private void PreMseConfirmationView_Load(object sender, EventArgs e)
        {
            UpdateView();
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
                labelAccountNum.Text       = this.Model_Account.AccountNumber.ToString();

                if( patient != null )
                {
                    labelPatientName.Text = patient.Name.AsFormattedName();
                    labelMRN.Text         = patient.MedicalRecordNumber.ToString();
                }
            }
            Cursor = Cursors.Default;

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
            this.contextLabel = new PatientAccess.UI.UserContextView();
            this.panelPatientContext = new System.Windows.Forms.Panel();
            this.patientContextView = new PatientAccess.UI.PatientContextView();
            this.panelBackground = new System.Windows.Forms.Panel();
            this.panelMain = new System.Windows.Forms.Panel();
            this.btnCloseActivity = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnRepeatActivity = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lineLabel = new PatientAccess.UI.CommonControls.LineLabel();
            this.labelMRN = new System.Windows.Forms.Label();
            this.labelAccountNum = new System.Windows.Forms.Label();
            this.labelPatientName = new System.Windows.Forms.Label();
            this.labelMRNStatic = new System.Windows.Forms.Label();
            this.labelAccountNumStatic = new System.Windows.Forms.Label();
            this.labelPatNameStatic = new System.Windows.Forms.Label();
            this.labelRegConfirm = new System.Windows.Forms.Label();
            this.btnPrintFaceSheet = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panelPatientContext.SuspendLayout();
            this.panelBackground.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextLabel
            // 
            this.contextLabel.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(128)), ((System.Byte)(162)), ((System.Byte)(200)));
            this.contextLabel.Description = " Register ED Patient Pre-MSE - Confirmation";
            this.contextLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.contextLabel.ForeColor = System.Drawing.Color.White;
            this.contextLabel.Location = new System.Drawing.Point(0, 0);
            this.contextLabel.Model = null;
            this.contextLabel.Name = "contextLabel";
            this.contextLabel.Size = new System.Drawing.Size(1024, 23);
            this.contextLabel.TabIndex = 1;
            // 
            // panelPatientContext
            // 
            this.panelPatientContext.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelPatientContext.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.panelPatientContext.Controls.Add(this.patientContextView);
            this.panelPatientContext.Location = new System.Drawing.Point(8, 24);
            this.panelPatientContext.Name = "panelPatientContext";
            this.panelPatientContext.Size = new System.Drawing.Size(1008, 22);
            this.panelPatientContext.TabIndex = 0;
            // 
            // patientContextView
            // 
            this.patientContextView.Account = null;
            this.patientContextView.BackColor = System.Drawing.Color.White;
            this.patientContextView.DateOfBirthText = "";
            this.patientContextView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.patientContextView.GenderLabelText = "";
            this.patientContextView.Location = new System.Drawing.Point(0, 0);
            this.patientContextView.Model = null;
            this.patientContextView.Name = "patientContextView";
            this.patientContextView.PatientNameText = "";
            this.patientContextView.Size = new System.Drawing.Size(1008, 22);
            this.patientContextView.SocialSecurityNumber = "";
            this.patientContextView.TabIndex = 0;
            // 
            // panelBackground
            // 
            this.panelBackground.BackColor = System.Drawing.Color.Black;
            this.panelBackground.Controls.Add(this.panelMain);
            this.panelBackground.Location = new System.Drawing.Point(8, 56);
            this.panelBackground.Name = "panelBackground";
            this.panelBackground.Size = new System.Drawing.Size(1008, 512);
            this.panelBackground.TabIndex = 1;
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.White;
            this.panelMain.Controls.Add(this.btnPrintFaceSheet);
            this.panelMain.Controls.Add(this.btnCloseActivity);
            this.panelMain.Controls.Add(this.btnRepeatActivity);
            this.panelMain.Controls.Add(this.lineLabel);
            this.panelMain.Controls.Add(this.labelMRN);
            this.panelMain.Controls.Add(this.labelAccountNum);
            this.panelMain.Controls.Add(this.labelPatientName);
            this.panelMain.Controls.Add(this.labelMRNStatic);
            this.panelMain.Controls.Add(this.labelAccountNumStatic);
            this.panelMain.Controls.Add(this.labelPatNameStatic);
            this.panelMain.Controls.Add(this.labelRegConfirm);
            this.panelMain.Location = new System.Drawing.Point(1, 1);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1006, 510);
            this.panelMain.TabIndex = 2;
            // 
            // btnCloseActivity
            // 
            this.btnCloseActivity.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.btnCloseActivity.Location = new System.Drawing.Point(8, 176);
            this.btnCloseActivity.Message = null;
            this.btnCloseActivity.Name = "btnCloseActivity";
            this.btnCloseActivity.Size = new System.Drawing.Size(80, 23);
            this.btnCloseActivity.TabIndex = 8;
            this.btnCloseActivity.Text = "C&lose Activity";
            this.btnCloseActivity.Click += new System.EventHandler(this.btnCloseActivity_Click);
            // 
            // btnRepeatActivity
            // 
            this.btnRepeatActivity.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.btnRepeatActivity.Location = new System.Drawing.Point(95, 176);
            this.btnRepeatActivity.Message = null;
            this.btnRepeatActivity.Name = "btnRepeatActivity";
            this.btnRepeatActivity.Size = new System.Drawing.Size(88, 23);
            this.btnRepeatActivity.TabIndex = 0;
            this.btnRepeatActivity.Text = "Repeat Acti&vity";
            this.btnRepeatActivity.Click += new System.EventHandler(this.btnRepeatActivity_Click);
            // 
            // lineLabel
            // 
            this.lineLabel.Caption = "Next Action";
            this.lineLabel.Location = new System.Drawing.Point(8, 152);
            this.lineLabel.Name = "lineLabel";
            this.lineLabel.Size = new System.Drawing.Size(992, 18);
            this.lineLabel.TabIndex = 7;
            this.lineLabel.TabStop = false;
            // 
            // labelMRN
            // 
            this.labelMRN.Location = new System.Drawing.Point(90, 112);
            this.labelMRN.Name = "labelMRN";
            this.labelMRN.Size = new System.Drawing.Size(256, 23);
            this.labelMRN.TabIndex = 6;
            // 
            // labelAccountNum
            // 
            this.labelAccountNum.Location = new System.Drawing.Point(90, 88);
            this.labelAccountNum.Name = "labelAccountNum";
            this.labelAccountNum.Size = new System.Drawing.Size(256, 23);
            this.labelAccountNum.TabIndex = 5;
            // 
            // labelPatientName
            // 
            this.labelPatientName.Location = new System.Drawing.Point(90, 64);
            this.labelPatientName.Name = "labelPatientName";
            this.labelPatientName.Size = new System.Drawing.Size(376, 23);
            this.labelPatientName.TabIndex = 4;
            // 
            // labelMRNStatic
            // 
            this.labelMRNStatic.Location = new System.Drawing.Point(16, 112);
            this.labelMRNStatic.Name = "labelMRNStatic";
            this.labelMRNStatic.Size = new System.Drawing.Size(56, 23);
            this.labelMRNStatic.TabIndex = 3;
            this.labelMRNStatic.Text = "MRN:";
            // 
            // labelAccountNumStatic
            // 
            this.labelAccountNumStatic.Location = new System.Drawing.Point(16, 88);
            this.labelAccountNumStatic.Name = "labelAccountNumStatic";
            this.labelAccountNumStatic.Size = new System.Drawing.Size(56, 23);
            this.labelAccountNumStatic.TabIndex = 2;
            this.labelAccountNumStatic.Text = "Account:";
            // 
            // labelPatNameStatic
            // 
            this.labelPatNameStatic.Location = new System.Drawing.Point(16, 64);
            this.labelPatNameStatic.Name = "labelPatNameStatic";
            this.labelPatNameStatic.Size = new System.Drawing.Size(75, 23);
            this.labelPatNameStatic.TabIndex = 1;
            this.labelPatNameStatic.Text = "Patient name:";
            // 
            // labelRegConfirm
            // 
            this.labelRegConfirm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.labelRegConfirm.Location = new System.Drawing.Point(16, 16);
            this.labelRegConfirm.Name = "labelRegConfirm";
            this.labelRegConfirm.Size = new System.Drawing.Size(320, 23);
            this.labelRegConfirm.TabIndex = 0;
            this.labelRegConfirm.Text = "Register ED Patient Pre-MSE confirmed.";
            // 
            // btnPrintFaceSheet
            // 
            this.btnPrintFaceSheet.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.btnPrintFaceSheet.Location = new System.Drawing.Point(190, 176);
            this.btnPrintFaceSheet.Message = null;
            this.btnPrintFaceSheet.Name = "btnPrintFaceSheet";
            this.btnPrintFaceSheet.Size = new System.Drawing.Size(98, 23);
            this.btnPrintFaceSheet.TabIndex = 9;
            this.btnPrintFaceSheet.Text = "Print Face Sheet";
            this.btnPrintFaceSheet.Click += new System.EventHandler(this.btnPrintFaceSheet_Click);
            // 
            // PreMseConfirmation
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.Controls.Add(this.panelBackground);
            this.Controls.Add(this.contextLabel);
            this.Controls.Add(this.panelPatientContext);
            this.Name = "PreMseConfirmation";
            this.Size = new System.Drawing.Size(1024, 620);
            this.Load += new System.EventHandler(this.PreMseConfirmationView_Load);
            this.panelPatientContext.ResumeLayout(false);
            this.panelBackground.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public PreMseConfirmation()
        {
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
        private Container             components = null;

        private Panel                  panelPatientContext;
        private Panel                  panelBackground;
        private Panel                  panelMain;

        private Label                  labelRegConfirm;
        private Label                  labelPatNameStatic;
        private Label                  labelMRNStatic;
        private Label                  labelPatientName;
        private Label                  labelMRN;
        private Label                  labelAccountNum;
        private Label                  labelAccountNumStatic;

        private LoggingButton                               btnRepeatActivity;
        private LoggingButton                               btnCloseActivity;
        private LoggingButton                               btnPrintFaceSheet;

        private UserContextView            contextLabel;
        private PatientContextView         patientContextView;        
        private LineLabel   lineLabel;
        #endregion

        #region Constants
        #endregion
    }
}
