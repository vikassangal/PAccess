using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Extensions.Exceptions;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.ExceptionManagement;

namespace PatientAccess.UI.HelpViews
{
	/// <summary>
	/// Summary description for AboutPatientAcces.
	/// </summary>
	public class AboutPatientAccessView : TimeOutFormView
	{

        #region Event Handlers

        /// <summary>
        /// Handles the Click event of the btnException control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnException_Click(object sender, EventArgs e)
        {
            

            switch( this.ExceptionSeverity )
            {
                case ExceptionSeverityEnum.Unhandled:

                    Application.ThreadException -= new ThreadExceptionEventHandler( new ApplicationExceptionHandler().OnThreadException );
                    //throw new Exception( "Unhandled exception" );
                    this.ThrowUnhandledExeption();
                    //this.BeginInvoke( new UnhandledExceptionDelegate( this.ThrowUnhandledExeption ));                   
                    //Application.ThreadException += new ThreadExceptionEventHandler( new ApplicationExceptionHandler().OnThreadException );
                    break;

                case ExceptionSeverityEnum.Catastrophic:
                    throw new EnterpriseException( "Enterprise Exception - Catastrophic", Severity.Catastrophic );
                case ExceptionSeverityEnum.High:
                    throw new EnterpriseException( "Enterprise Exception - High", Severity.High );
                case ExceptionSeverityEnum.Low:
                    throw new EnterpriseException( "Enterprise Exception - Low", Severity.Low);
                case ExceptionSeverityEnum.Other:
                    throw new Exception( "Other Exception" );
                default:
                    throw new Exception( "Other Exception" );
            }
        }

        private void ThrowUnhandledExeption()
        {
            UnhandledExceptionDelegate del = new UnhandledExceptionDelegate( this.ThrowUnhandledException2 );
            del.BeginInvoke( null, null );            
        }

        private void ThrowUnhandledException2()
        {
            throw new Exception( "Unhandled Exception" );
        }

        #endregion

        #region Methods
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the exception severity.
        /// </summary>
        /// <value>The exception severity.</value>
        private ExceptionSeverityEnum ExceptionSeverity
        {
            get 
            {
                return i_ExceptionSeverity; 
            }
            set 
            {
                i_ExceptionSeverity =  value; 
            }
        }
        #endregion

        public AboutPatientAccessView()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            this.DisplayNameAndVersionNumber();
            this.DisplayImage();
            this.DisplayUserInfo();
            this.DisplayEnvironmentInfo();

            base.EnableThemesOn( this );
		}

	    private void DisplayNameAndVersionNumber()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            lblNameAndVersion.Text = String.Format( lblNameAndVersion.Text, version );
        }

	    private void DisplayImage()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream imageFile = 
                assembly.GetManifestResourceStream( "PatientAccess.Images.PA_icon.JPG" );
            this.pictureBox1.Image = Image.FromStream( imageFile );
        }

	    private void DisplaySystemInfo()
        {
            Process.Start( "msinfo32" );
        }

	    private void DisplayUserInfo()
        {
            User user = User.GetCurrent();
            
            lblUserName.Text        = String.Format( lblUserName.Text, user.SecurityUser.CanonicalName );
            lblUserId.Text          = String.Format( lblUserId.Text, user.SecurityUser.UPN );
            lblPbarId.Text          = String.Format( lblPbarId.Text, user.PBAREmployeeID );
            lblWorkstationId.Text   = String.Format( lblWorkstationId.Text, user.WorkstationID );
            lblCurrentFacility.Text = String.Format( lblCurrentFacility.Text, user.Facility.CodeAndDescription );
        }

	    private void DisplayEnvironmentInfo()
        {
            lblFramework.Text = String.Format( lblFramework.Text, Environment.Version );
            lblOs.Text        = String.Format( lblOs.Text, Environment.OSVersion );
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AboutPatientAccessView));
            this.buttonOK = new PatientAccess.UI.CommonControls.LoggingButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblHelpDeskNumber = new System.Windows.Forms.Label();
            this.lblNameAndVersion = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblOs = new System.Windows.Forms.Label();
            this.lblFramework = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.lblUserId = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblWorkstationId = new System.Windows.Forms.Label();
            this.lblCurrentFacility = new System.Windows.Forms.Label();
            this.lblPbarId = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnSystemInfo = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnException = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonOK.Location = new System.Drawing.Point(520, 416);
            this.buttonOK.Message = null;
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(96, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(316, 400);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // lblHelpDeskNumber
            // 
            this.lblHelpDeskNumber.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblHelpDeskNumber.Location = new System.Drawing.Point(328, 32);
            this.lblHelpDeskNumber.Name = "lblHelpDeskNumber";
            this.lblHelpDeskNumber.Size = new System.Drawing.Size(288, 16);
            this.lblHelpDeskNumber.TabIndex = 2;
            this.lblHelpDeskNumber.Text = "Tenet Help Desk: 1-800-639-7575";
            // 
            // lblNameAndVersion
            // 
            this.lblNameAndVersion.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblNameAndVersion.Location = new System.Drawing.Point(328, 8);
            this.lblNameAndVersion.Name = "lblNameAndVersion";
            this.lblNameAndVersion.Size = new System.Drawing.Size(288, 16);
            this.lblNameAndVersion.TabIndex = 0;
            this.lblNameAndVersion.Text = "Patient Access Version {0}";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.lblOs);
            this.panel2.Controls.Add(this.lblFramework);
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Controls.Add(this.lblUserId);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.lblWorkstationId);
            this.panel2.Controls.Add(this.lblCurrentFacility);
            this.panel2.Controls.Add(this.lblPbarId);
            this.panel2.Controls.Add(this.lblUserName);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Controls.Add(this.lblNameAndVersion);
            this.panel2.Controls.Add(this.lblHelpDeskNumber);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(626, 400);
            this.panel2.TabIndex = 5;
            // 
            // lblOs
            // 
            this.lblOs.Location = new System.Drawing.Point(328, 232);
            this.lblOs.Name = "lblOs";
            this.lblOs.Size = new System.Drawing.Size(288, 16);
            this.lblOs.TabIndex = 13;
            this.lblOs.Text = "{0}";
            // 
            // lblFramework
            // 
            this.lblFramework.Location = new System.Drawing.Point(328, 208);
            this.lblFramework.Name = "lblFramework";
            this.lblFramework.Size = new System.Drawing.Size(288, 16);
            this.lblFramework.TabIndex = 12;
            this.lblFramework.Text = "Microsoft .NET Framework v{0}";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.SystemColors.Control;
            this.panel5.Location = new System.Drawing.Point(328, 192);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(288, 2);
            this.panel5.TabIndex = 11;
            // 
            // lblUserId
            // 
            this.lblUserId.Location = new System.Drawing.Point(328, 96);
            this.lblUserId.Name = "lblUserId";
            this.lblUserId.Size = new System.Drawing.Size(288, 16);
            this.lblUserId.TabIndex = 10;
            this.lblUserId.Text = "User Id: {0}";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.Control;
            this.panel4.Location = new System.Drawing.Point(328, 56);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(288, 2);
            this.panel4.TabIndex = 9;
            // 
            // lblWorkstationId
            // 
            this.lblWorkstationId.Location = new System.Drawing.Point(328, 144);
            this.lblWorkstationId.Name = "lblWorkstationId";
            this.lblWorkstationId.Size = new System.Drawing.Size(288, 16);
            this.lblWorkstationId.TabIndex = 8;
            this.lblWorkstationId.Text = "Workstation Id: {0}";
            // 
            // lblCurrentFacility
            // 
            this.lblCurrentFacility.Location = new System.Drawing.Point(328, 168);
            this.lblCurrentFacility.Name = "lblCurrentFacility";
            this.lblCurrentFacility.Size = new System.Drawing.Size(288, 16);
            this.lblCurrentFacility.TabIndex = 7;
            this.lblCurrentFacility.Text = "Facility: {0}";
            // 
            // lblPbarId
            // 
            this.lblPbarId.Location = new System.Drawing.Point(328, 120);
            this.lblPbarId.Name = "lblPbarId";
            this.lblPbarId.Size = new System.Drawing.Size(288, 16);
            this.lblPbarId.TabIndex = 6;
            this.lblPbarId.Text = "PBAR Id: {0}";
            // 
            // lblUserName
            // 
            this.lblUserName.Location = new System.Drawing.Point(328, 72);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(288, 16);
            this.lblUserName.TabIndex = 5;
            this.lblUserName.Text = "User: {0}";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Location = new System.Drawing.Point(316, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(2, 400);
            this.panel1.TabIndex = 4;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.panel3.Location = new System.Drawing.Point(0, 400);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(626, 2);
            this.panel3.TabIndex = 6;
            // 
            // btnSystemInfo
            // 
            this.btnSystemInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSystemInfo.Location = new System.Drawing.Point(520, 448);
            this.btnSystemInfo.Message = null;
            this.btnSystemInfo.Name = "btnSystemInfo";
            this.btnSystemInfo.Size = new System.Drawing.Size(96, 23);
            this.btnSystemInfo.TabIndex = 7;
            this.btnSystemInfo.Text = "&System Info...";
            this.btnSystemInfo.Click += new System.EventHandler(this.btnSystemInfo_Click);
            // 
            // btnException
            // 
            this.btnException.Enabled = false;
            this.btnException.Location = new System.Drawing.Point(8, 448);
            this.btnException.Name = "btnException";
            this.btnException.Size = new System.Drawing.Size(168, 23);
            this.btnException.TabIndex = 9;
            this.btnException.Text = "&Throw Catastrophic Exception";
            this.btnException.Visible = false;
            this.btnException.Click += new System.EventHandler(this.btnException_Click);
            // 
            // AboutPatientAccessView
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleMode = AutoScaleMode.None;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.buttonOK;
            this.ClientSize = new System.Drawing.Size(626, 480);
            this.Controls.Add(this.btnSystemInfo);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.btnException);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutPatientAccessView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About Patient Access";
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSystemInfo_Click(object sender, EventArgs e)
        {
            this.DisplaySystemInfo();
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)	
        {
            const int WM_KEYDOWN = 0x100;
            const int WM_SYSKEYDOWN = 0x104;

            bool enableTestMode = Convert.ToBoolean( ConfigurationManager.AppSettings[ENABLE_TEST_MODE] );
            bool changeButtonStatus = false;

            if ( enableTestMode && ( ( msg.Msg == WM_KEYDOWN ) || ( msg.Msg == WM_SYSKEYDOWN ) ) )
            {
                switch( keyData )
                {
                    case Keys.Control | Keys.Shift | Keys.U:
                        ExceptionButtonShown = ( ( this.ExceptionSeverity == ExceptionSeverityEnum.Unhandled ) && ( ExceptionButtonShown ) ) ? true : false;
                        this.ExceptionSeverity = ExceptionSeverityEnum.Unhandled;
                        changeButtonStatus = true;
                        break;

                    case Keys.Control | Keys.Shift | Keys.C:
                        ExceptionButtonShown = ( ( this.ExceptionSeverity == ExceptionSeverityEnum.Catastrophic ) && ( ExceptionButtonShown ) ) ? true : false;
                        this.ExceptionSeverity = ExceptionSeverityEnum.Catastrophic;
                        changeButtonStatus = true;
                        break;

                    case Keys.Control | Keys.Shift | Keys.H:
                        ExceptionButtonShown = ( ( this.ExceptionSeverity == ExceptionSeverityEnum.High) && ( ExceptionButtonShown ) ) ? true : false;
                        this.ExceptionSeverity = ExceptionSeverityEnum.High;
                        changeButtonStatus = true;
                        break;

                    case Keys.Control | Keys.Shift | Keys.L:
                        ExceptionButtonShown = ( ( this.ExceptionSeverity == ExceptionSeverityEnum.Low) && ( ExceptionButtonShown ) ) ? true : false;
                        this.ExceptionSeverity = ExceptionSeverityEnum.Low;
                        changeButtonStatus = true;
                        break;

                    case Keys.Control | Keys.Shift | Keys.O:
                        ExceptionButtonShown = ( ( this.ExceptionSeverity == ExceptionSeverityEnum.Other) && ( ExceptionButtonShown ) ) ? true : false;
                        this.ExceptionSeverity = ExceptionSeverityEnum.Other;
                        changeButtonStatus = true;
                        break;
                }
                
                if( changeButtonStatus )
                {
                    this.btnException.Text = string.Concat("&Throw ", this.ExceptionSeverity.ToString(), " Exception" );
                    ToggleExceptionButton();
                }
            }
            return base.ProcessCmdKey(ref msg,keyData);
        }

        #region Private Methods
        private void ToggleExceptionButton()
        {
            if( ExceptionButtonShown )
            {
                this.btnException.Enabled = false;
                this.btnException.Visible = false;
                ExceptionButtonShown = false;
            } 
            else
            {
                this.btnException.Enabled = true;
                this.btnException.Visible = true;
                ExceptionButtonShown = true;
            }
            Invalidate();
        }
        #endregion

        #region Private Properties
        private bool ExceptionButtonShown
        {
            get
            {
                return i_ExceptionButtonShown;
            }
            set
            {
                i_ExceptionButtonShown = value;
            }
        }
        #endregion

        #region Construction and Finalization
        #endregion

        #region Data Elements

        private delegate void UnhandledExceptionDelegate();

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;
        private LoggingButton buttonOK;
        private PictureBox pictureBox1;
        private Panel panel2;
        private Panel panel3;
        private Panel panel1;
        private LoggingButton btnSystemInfo;
        private Label lblUserName;
        private Label lblPbarId;
        private Label lblCurrentFacility;
        private Label lblHelpDeskNumber;
        private Label lblNameAndVersion;
        private Panel panel4;
        private Label lblWorkstationId;
        private Label lblUserId;
        private Panel panel5;
        private Label lblFramework;
        private Label lblOs;
        private Button btnException;
        private ExceptionSeverityEnum i_ExceptionSeverity = ExceptionSeverityEnum.Other;
        private bool i_ExceptionButtonShown = true;
        #endregion

        #region Constants
        private const string 
            ENABLE_TEST_MODE                   = "PatientAccess.EnableTestMode";
        #endregion

        #region Enumerators
        /// <summary>
        /// Specifies the options for the execution of the Updater before of after the application.
        /// </summary>
        private enum ExceptionSeverityEnum
        {
            Unhandled,
            Catastrophic,
            High,
            Low,
            Other,
        }
        #endregion
    }
}
