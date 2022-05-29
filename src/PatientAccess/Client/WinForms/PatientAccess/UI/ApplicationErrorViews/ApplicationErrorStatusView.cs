using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces.CrashReporting;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.CrashReporting;

namespace PatientAccess.UI.ApplicationErrorViews
{
	/// <summary>
	/// Summary description for ApplicationErrorStatusView.
	/// </summary>
    [Serializable]
    public class ApplicationErrorStatusView : TimeOutFormView
    {
        #region Event Handlers
        private void ApplicationErrorStatusView_Load( object sender, EventArgs e )
        {
            this.Visible = true;
        }

        private void ApplicationErrorStatusView_Shown( object sender, EventArgs e )
        {
            ProcessCrashReport();
        }
        
        private void btnClose_Click( object sender, EventArgs e )
        {
            this.Close();
        }
        #endregion

        #region Methods
        #endregion

        #region Properties
        public Exception Exception
        {
            private get
            {
                return i_Exception;
            }
            set
            {
                i_Exception = value;
            }
        }

        public Bitmap ScreenCapture
        {
            private get
            {
                return i_ScreenCapture;
            }
            set
            {
                i_ScreenCapture = value;
            }
        }

        public PhoneNumber PhoneNumber
        {
            private get
            {
                return i_PhoneNumber;
            }
            set
            {
                i_PhoneNumber = value;
            }
        }

        public string Comments
        {
            private get
            {
                return i_Comments;
            }
            set
            {
                i_Comments = value;
            }
        }
        #endregion

        #region Private Methods
        public void ProcessCrashReport()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                this.Refresh();

                this.UpdatePreparingReportStatus(); 

                CrashReporter crashReporter = new CrashReporter();
            
                CrashReport crashReport = crashReporter.BuildReportFor( this.Exception, this.ScreenCapture, this.PhoneNumber, this.Comments );
            
                this.UpdateSendingReportStatus();

                crashReporter.Save( crashReport );

                this.UpdateReportCompleteStatus();
            
            }
            finally
            {

                if ( !this.lblPrepareErrorReportCheck.Visible ) 
                {
                    this.UpdatePreparingReportStatus(); 
                }

                if ( !this.lblSendErrorReportCheck.Visible )
                {
                    this.UpdateSendingReportStatus();
                }

                if (!this.lblErrorReportComplete.Visible)
                {
                    this.UpdateReportCompleteStatus();
                }

                Cursor.Current = Cursors.Default;

                this.btnClose.Enabled = true;
                this.btnClose.Refresh();
            }
        }

        private void UpdatePreparingReportStatus()
        {
            this.lblPrepareErrorReportCheck.Visible = true;
            this.lblPrepareErrorReportCheck.Refresh();
            this.Refresh();
            Thread.Sleep( 100 ); //Sleep for 1/10th of a second.
        }

        private void UpdateSendingReportStatus()
        {
            this.lblSendErrorReportCheck.Visible = true;
            this.lblSendErrorReportCheck.Refresh();
            this.Refresh();
            Thread.Sleep( 100 ); //Sleep for 1/10th of a second.
        }
        
        private void UpdateReportCompleteStatus()
        {
            this.lblErrorReportComplete.Visible = true;
            this.lblErrorReportComplete.Refresh();
            this.Refresh();
            Thread.Sleep( 100 ); //Sleep for 1/10th of a second.
        }
        #endregion

        #region Private Properties
        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ApplicationErrorStatusView ) );
            this.btnClose = new System.Windows.Forms.Button();
            this.lblPrepareErrorReport = new System.Windows.Forms.Label();
            this.lblSendErrorReport = new System.Windows.Forms.Label();
            this.lblErrorReportComplete = new System.Windows.Forms.Label();
            this.lblSendErrorReportCheck = new System.Windows.Forms.Label();
            this.lblPrepareErrorReportCheck = new System.Windows.Forms.Label();
            this.lblBorder = new System.Windows.Forms.Label();
            this.picBoxProgress = new System.Windows.Forms.PictureBox();
            ( (System.ComponentModel.ISupportInitialize)( this.picBoxProgress ) ).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Enabled = false;
            this.btnClose.Location = new System.Drawing.Point( 304, 112 );
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size( 80, 23 );
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "&Close";
            this.btnClose.Click += new System.EventHandler( this.btnClose_Click );
            // 
            // lblPrepareErrorReport
            // 
            this.lblPrepareErrorReport.Location = new System.Drawing.Point( 162, 16 );
            this.lblPrepareErrorReport.Name = "lblPrepareErrorReport";
            this.lblPrepareErrorReport.Size = new System.Drawing.Size( 176, 16 );
            this.lblPrepareErrorReport.TabIndex = 3;
            this.lblPrepareErrorReport.Text = "Preparing error report";
            // 
            // lblSendErrorReport
            // 
            this.lblSendErrorReport.Location = new System.Drawing.Point( 162, 44 );
            this.lblSendErrorReport.Name = "lblSendErrorReport";
            this.lblSendErrorReport.Size = new System.Drawing.Size( 176, 16 );
            this.lblSendErrorReport.TabIndex = 4;
            this.lblSendErrorReport.Text = "Sending error report";
            // 
            // lblErrorReportComplete
            // 
            this.lblErrorReportComplete.Location = new System.Drawing.Point( 138, 75 );
            this.lblErrorReportComplete.Name = "lblErrorReportComplete";
            this.lblErrorReportComplete.Size = new System.Drawing.Size( 176, 16 );
            this.lblErrorReportComplete.TabIndex = 5;
            this.lblErrorReportComplete.Text = "Error reporting completed";
            this.lblErrorReportComplete.Visible = false;
            // 
            // lblSendErrorReportCheck
            // 
            this.lblSendErrorReportCheck.Font = new System.Drawing.Font( "Wingdings 2", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 2 ) ) );
            this.lblSendErrorReportCheck.Location = new System.Drawing.Point( 138, 46 );
            this.lblSendErrorReportCheck.Name = "lblSendErrorReportCheck";
            this.lblSendErrorReportCheck.Size = new System.Drawing.Size( 16, 16 );
            this.lblSendErrorReportCheck.TabIndex = 11;
            this.lblSendErrorReportCheck.Text = "P";
            this.lblSendErrorReportCheck.Visible = false;
            // 
            // lblPrepareErrorReportCheck
            // 
            this.lblPrepareErrorReportCheck.Font = new System.Drawing.Font( "Wingdings 2", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 2 ) ) );
            this.lblPrepareErrorReportCheck.Location = new System.Drawing.Point( 138, 18 );
            this.lblPrepareErrorReportCheck.Name = "lblPrepareErrorReportCheck";
            this.lblPrepareErrorReportCheck.Size = new System.Drawing.Size( 16, 16 );
            this.lblPrepareErrorReportCheck.TabIndex = 13;
            this.lblPrepareErrorReportCheck.Text = "P";
            this.lblPrepareErrorReportCheck.Visible = false;
            // 
            // lblBorder
            // 
            this.lblBorder.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblBorder.Location = new System.Drawing.Point( 10, 105 );
            this.lblBorder.Name = "lblBorder";
            this.lblBorder.Size = new System.Drawing.Size( 375, 2 );
            this.lblBorder.TabIndex = 15;
            // 
            // picBoxProgress
            // 
            this.picBoxProgress.Image = ( (System.Drawing.Image)( resources.GetObject( "picBoxProgress.Image" ) ) );
            this.picBoxProgress.Location = new System.Drawing.Point( 19, 12 );
            this.picBoxProgress.Name = "picBoxProgress";
            this.picBoxProgress.Size = new System.Drawing.Size( 110, 46 );
            this.picBoxProgress.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picBoxProgress.TabIndex = 16;
            this.picBoxProgress.TabStop = false;
            // 
            // ApplicationErrorStatusView
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleBaseSize = new System.Drawing.Size( 5, 13 );
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size( 397, 142 );
            this.Controls.Add( this.picBoxProgress );
            this.Controls.Add( this.lblBorder );
            this.Controls.Add( this.lblPrepareErrorReportCheck );
            this.Controls.Add( this.lblSendErrorReportCheck );
            this.Controls.Add( this.lblErrorReportComplete );
            this.Controls.Add( this.lblSendErrorReport );
            this.Controls.Add( this.lblPrepareErrorReport );
            this.Controls.Add( this.btnClose );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ( (System.Drawing.Icon)( resources.GetObject( "$this.Icon" ) ) );
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ApplicationErrorStatusView";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Patient Access - Error Reporting";
            this.Shown += new System.EventHandler( this.ApplicationErrorStatusView_Shown );
            this.Load += new System.EventHandler( this.ApplicationErrorStatusView_Load );
            ( (System.ComponentModel.ISupportInitialize)( this.picBoxProgress ) ).EndInit();
            this.ResumeLayout( false );
            this.PerformLayout();

        }
        #endregion

        #region Construction and Finalization
        public ApplicationErrorStatusView()
        {
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
        private Container components = null;

        private Label lblPrepareErrorReport;
        private Label lblSendErrorReport;
        private Label lblErrorReportComplete;
        private Label lblSendErrorReportCheck;
        private Label lblPrepareErrorReportCheck;
        
        private Button btnClose;

        private Exception                               i_Exception;
        private Bitmap                                  i_ScreenCapture;
        private PhoneNumber                             i_PhoneNumber = new PhoneNumber();
        private Label              lblBorder;
        private PictureBox                              picBoxProgress;
        private string                                  i_Comments = string.Empty;
        #endregion

        #region Constants
        #endregion
    }
}
