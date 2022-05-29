using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.ApplicationErrorViews
{
	/// <summary>
	/// Summary description for ApplicationErrorView.
	/// </summary>
    [Serializable]
    public class ApplicationErrorView : TimeOutFormView
    {

        #region Event Handlers

        void phoneNumberControl_Leave( object sender, EventArgs e )
        {
            this.IsPhoneNumberValid();
        }

        private void btnSend_Click( object sender, EventArgs e )
        {
            if( this.IsPhoneNumberValid() )
            {
                this.PhoneNumber = this.phoneNumberControl.Model;
            }
            else
            {
                this.PhoneNumber = new PhoneNumber();
            }

            // Save the User's Phone Number to User Preferences file in isolated storage.
            this.UserPreferences.PhoneNumber = this.PhoneNumber;
            this.UserPreferences.Save( this.UserPreferences );

            this.Comments = this.txtComments.Text;

            this.Close();
        }

        private void lnkErrorMessageDetail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lnkErrorMessageDetail.LinkVisited = false;

            // Create a StringBuilder to format the error details.
            StringBuilder formattedErrorDetails = new StringBuilder();

            formattedErrorDetails.AppendFormat( "Patient Access - Error Details{0}{1}", Environment.NewLine, Environment.NewLine );
            formattedErrorDetails.AppendFormat( "{0}{1}{2}{3}{4}", DateTime.Now.ToLongDateString(), " ", DateTime.Now.ToLongTimeString(), Environment.NewLine, Environment.NewLine );
            formattedErrorDetails.AppendFormat( "Version: {0}{1}{2}", Assembly.GetExecutingAssembly().GetName().Version, Environment.NewLine, Environment.NewLine );
            formattedErrorDetails.AppendFormat( "OS version: {0}{1}", Environment.OSVersion, Environment.NewLine );
            formattedErrorDetails.AppendFormat( "Runtime version: {0}{1}", Environment.Version, Environment.NewLine );
            formattedErrorDetails.AppendFormat( "Program: {0}{1}{2}", Application.ExecutablePath, Environment.NewLine, Environment.NewLine );
            formattedErrorDetails.AppendFormat( "When using Patient Access, the following error was encountered: {0}", Environment.NewLine );
            formattedErrorDetails.AppendFormat( "{0}", i_ErrorDetails );

            // Get a temporary file with a unique name; will generate something like C:\Documents and Settings\youraccount\Local Settings\Temp\tmp62.tmp
            string tempFile = Path.GetTempFileName();

            using (FileStream fs = File.Open( tempFile, FileMode.Open, FileAccess.Write, FileShare.None ) ) 
            {
                Byte[] info = new UTF8Encoding(true).GetBytes( formattedErrorDetails.ToString() );
                // Add the error detail information to the file.
                fs.Write(info, 0, info.Length);
            }

            Process.Start( e.Link.LinkData.ToString(), tempFile );
        }

        private void ApplicationErrorView_KeyDown(object sender, KeyEventArgs e)
        {    
            if( e.KeyCode == Keys.Enter )
            {
                this.btnSend_Click(this, null);
            }
        }

	    private bool IsPhoneNumberValid()
        {
            if( this.phoneNumberControl.AreaCode.Length == 3 && this.phoneNumberControl.PhoneNumber.Length != 7 )
            {
                MessageBox.Show( UIErrorMessages.PHONE_NUMBER_REQUIRED,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1 );
                
                this.phoneNumberControl.FocusPhoneNumber();
                return false;
            }
            return true;
        }

        #endregion

        #region Methods
        public override void UpdateView()
        {
            // Set the error message.
            this.lblErrorMessage.Text = String.Format( "{0}{1}",
                Environment.NewLine,
                this.i_ErrorText
                );
            // TODO: FIGURE OUT WHY THIS DOESN'T WORK AND REMOVE THE ENVIRONMENT.NEWLINE ABOVE.

            // Create a StringBuilder to format the error details.
            StringBuilder formattedErrorDetails = new StringBuilder();

            formattedErrorDetails.AppendFormat( "{0}An error has occurred within the application.", Environment.NewLine );
            formattedErrorDetails.AppendFormat( "{0}{1}{2}{3}We have created an error report that will be sent to help us improve the Patient Access system.", Environment.NewLine, Environment.NewLine, Environment.NewLine, Environment.NewLine );
            formattedErrorDetails.AppendFormat( "{0}{1}To see what this error report contains, click here.", Environment.NewLine, Environment.NewLine );

            // Set the error detail text on the form.
            this.lnkErrorMessageDetail.Text = formattedErrorDetails.ToString();

            // Set the User's phone number on the form from the UserPreferences in isolated storage.
            if( UserPreferences.PhoneNumber != null )
            {
                this.phoneNumberControl.Model = UserPreferences.PhoneNumber;
            }

            this.lnkErrorMessageDetail.Links.Add( lnkErrorMessageDetail.Text.Length - 11, 11, "notepad.exe" );
            this.Invalidate();
        }

        public void SetButtonText(string newText)
        {
            this.btnSend.Text = newText;
        }
        #endregion

        #region Properties
        public string ErrorText
        {
            get
            {
                return i_ErrorText;
            }
            set
            {
                i_ErrorText = value;
            }
        }

        public string ErrorDetails
        {
            get
            {
                return i_ErrorDetails;
            }
            set
            {
                i_ErrorDetails = value;
            }
        }

        public PhoneNumber PhoneNumber
        {
            get
            {
                return i_PhoneNumber;
            }
            private set
            {
                i_PhoneNumber = value;
            }
        }

        public string Comments
        {
            get
            {
                if( i_Comments != null )
                {
                    return i_Comments;
                }
                else
                {
                    return string.Empty;
                }
            }
            private set 
            {
                if( value != null )
                {
                    i_Comments = value;
                } 
                else
                {
                    i_Comments = string.Empty;
                }
            }
        }
        #endregion

        #region Private Methods       
        #endregion

        #region Private Properties
        private UserPreference UserPreferences
        {
            get
            {
                if( this.i_UserPreferences == null )
                {
                    this.i_UserPreferences = UserPreference.Load();
                }

                return i_UserPreferences;
            }
        }
        #endregion

        #region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ApplicationErrorView ) );
            this.btnSend = new System.Windows.Forms.Button();
            this.lblPhoneNumber = new System.Windows.Forms.Label();
            this.txtComments = new System.Windows.Forms.TextBox();
            this.lblComments = new System.Windows.Forms.Label();
            this.pnlErrorMessage = new System.Windows.Forms.Panel();
            this.lblErrorMessage = new System.Windows.Forms.Label();
            this.pnlBorder = new System.Windows.Forms.Panel();
            this.pnlErrorMessageDetail = new System.Windows.Forms.Panel();
            this.lnkErrorMessageDetail = new System.Windows.Forms.LinkLabel();
            this.phoneNumberControl = new PatientAccess.UI.CommonControls.PhoneNumberControl();
            this.pnlErrorMessage.SuspendLayout();
            this.pnlErrorMessageDetail.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSend
            // 
            this.btnSend.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSend.Location = new System.Drawing.Point( 344, 336 );
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size( 112, 23 );
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "&Send Error Report";
            this.btnSend.Click += new System.EventHandler( this.btnSend_Click );
            // 
            // lblPhoneNumber
            // 
            this.lblPhoneNumber.Location = new System.Drawing.Point( 30, 308 );
            this.lblPhoneNumber.Name = "lblPhoneNumber";
            this.lblPhoneNumber.Size = new System.Drawing.Size( 148, 16 );
            this.lblPhoneNumber.TabIndex = 0;
            this.lblPhoneNumber.Text = "&Phone Number (optional):";
            // 
            // txtComments
            // 
            this.txtComments.Location = new System.Drawing.Point( 30, 240 );
            this.txtComments.MaxLength = 2000;
            this.txtComments.Multiline = true;
            this.txtComments.Name = "txtComments";
            this.txtComments.Size = new System.Drawing.Size( 404, 50 );
            this.txtComments.TabIndex = 1;
            // 
            // lblComments
            // 
            this.lblComments.Location = new System.Drawing.Point( 30, 224 );
            this.lblComments.Name = "lblComments";
            this.lblComments.Size = new System.Drawing.Size( 300, 16 );
            this.lblComments.TabIndex = 0;
            this.lblComments.Text = "&What were you doing when the error happened? (optional)";
            // 
            // pnlErrorMessage
            // 
            this.pnlErrorMessage.BackColor = System.Drawing.Color.White;
            this.pnlErrorMessage.Controls.Add( this.lblErrorMessage );
            this.pnlErrorMessage.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlErrorMessage.Location = new System.Drawing.Point( 0, 0 );
            this.pnlErrorMessage.Name = "pnlErrorMessage";
            this.pnlErrorMessage.Size = new System.Drawing.Size( 464, 92 );
            this.pnlErrorMessage.TabIndex = 21;
            // 
            // lblErrorMessage
            // 
            this.lblErrorMessage.BackColor = System.Drawing.Color.White;
            this.lblErrorMessage.Font = new System.Drawing.Font( "Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.lblErrorMessage.Location = new System.Drawing.Point( 20, 0 );
            this.lblErrorMessage.Name = "lblErrorMessage";
            this.lblErrorMessage.Size = new System.Drawing.Size( 424, 92 );
            this.lblErrorMessage.TabIndex = 0;
            this.lblErrorMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlBorder
            // 
            this.pnlBorder.BackColor = System.Drawing.Color.Black;
            this.pnlBorder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlBorder.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBorder.Location = new System.Drawing.Point( 0, 92 );
            this.pnlBorder.Name = "pnlBorder";
            this.pnlBorder.Size = new System.Drawing.Size( 464, 1 );
            this.pnlBorder.TabIndex = 22;
            // 
            // pnlErrorMessageDetail
            // 
            this.pnlErrorMessageDetail.Controls.Add( this.lnkErrorMessageDetail );
            this.pnlErrorMessageDetail.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlErrorMessageDetail.Location = new System.Drawing.Point( 0, 93 );
            this.pnlErrorMessageDetail.Name = "pnlErrorMessageDetail";
            this.pnlErrorMessageDetail.Size = new System.Drawing.Size( 464, 115 );
            this.pnlErrorMessageDetail.TabIndex = 23;
            // 
            // lnkErrorMessageDetail
            // 
            this.lnkErrorMessageDetail.Location = new System.Drawing.Point( 30, 0 );
            this.lnkErrorMessageDetail.Name = "lnkErrorMessageDetail";
            this.lnkErrorMessageDetail.Size = new System.Drawing.Size( 404, 115 );
            this.lnkErrorMessageDetail.TabIndex = 0;
            this.lnkErrorMessageDetail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler( this.lnkErrorMessageDetail_LinkClicked );
            // 
            // phoneNumberControl1
            // 
            this.phoneNumberControl.AreaCode = "";
            this.phoneNumberControl.Location = new System.Drawing.Point( 156, 301 );
            this.phoneNumberControl.Name = "phoneNumberControl";
            this.phoneNumberControl.PhoneNumber = "";
            this.phoneNumberControl.Size = new System.Drawing.Size( 94, 27 );
            this.phoneNumberControl.TabIndex = 24;
            this.phoneNumberControl.Leave += new EventHandler( phoneNumberControl_Leave );
            // 
            // ApplicationErrorView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.CancelButton = this.btnSend;
            this.ClientSize = new System.Drawing.Size( 464, 368 );
            this.ControlBox = false;
            this.Controls.Add( this.phoneNumberControl );
            this.Controls.Add( this.pnlErrorMessageDetail );
            this.Controls.Add( this.pnlBorder );
            this.Controls.Add( this.pnlErrorMessage );
            this.Controls.Add( this.txtComments );
            this.Controls.Add( this.lblComments );
            this.Controls.Add( this.lblPhoneNumber );
            this.Controls.Add( this.btnSend );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ( (System.Drawing.Icon)( resources.GetObject( "$this.Icon" ) ) );
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ApplicationErrorView";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Patient Access - Error";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler( this.ApplicationErrorView_KeyDown );
            this.pnlErrorMessage.ResumeLayout( false );
            this.pnlErrorMessageDetail.ResumeLayout( false );
            this.ResumeLayout( false );
            this.PerformLayout();

        }

		#endregion

        #region Construction and Finalization
        public ApplicationErrorView()
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
        private Container                     components = null;

        private Button btnSend;

        private TextBox                        txtComments;

        private Label                          lblPhoneNumber;
        private Label                          lblErrorMessage;
        private Label                          lblComments;

        private Panel                          pnlBorder;
        private Panel                          pnlErrorMessage;
        private Panel                          pnlErrorMessageDetail;

        private LinkLabel                      lnkErrorMessageDetail;

        private PhoneNumber                                         i_PhoneNumber = new PhoneNumber();

        private string                                              i_Comments = String.Empty;        
        private string                                              i_ErrorText;
        private string                                              i_ErrorDetails;

        private PhoneNumberControl                                  phoneNumberControl;
        private UserPreference                                      i_UserPreferences;

        #endregion

        #region Constants
        #endregion
    }
}
