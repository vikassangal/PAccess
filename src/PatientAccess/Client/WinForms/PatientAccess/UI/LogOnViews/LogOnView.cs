using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Reflection;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties.Exceptions;
using PatientAccess.Domain.Security;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Logging;
using log4net;

namespace PatientAccess.UI.LogOnViews
{
    [Serializable]
    public class LogOnView : LoggingFormView
    {
        #region Event Handlers
        /// <summary>
        /// Handles the Load event of the LogOnView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void LogOnView_Load( object sender, EventArgs e )
        {                    
            // Fade-in the Login form.
            this.Opacity = 0.0;
            this.Activate();
            this.Refresh();
            fadeTimer.Start();
            this.Refresh();

            // Get an instance of the LogOnService.
            this.LogOnService = new LogOnService( AuditLogger );

            try
            {
                // Get the user's preferences from the machine.
                this.Preferences = this.LogOnService.GetUserPreference();

                // Get the default user credentials for Dev and populate the form.
                this.Credentials();

                this.DisplayFacilities();
            }
            catch( Exception ex )
            {                
                c_log.Error( "Exception", ex );
                //this.ToggleControls( false );

                if( ex is VersionNotMatchedException )
                {
                    MessageBox.Show( UIErrorMessages.VERSION_NOT_MATCHED, "Version is out-of-date", MessageBoxButtons.OK, MessageBoxIcon.Stop );
                    this.Close();
                }
                else
                {
                    MessageBox.Show( UIErrorMessages.ERROR_INITIALIZING_APP,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                }
                this.btnCancel.Enabled = true;
                return;
            }
        }

        /// <summary>
        /// Attempt to log the user onto the application.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnOk_Click(object sender, EventArgs e)
        {
           LogIn();
        }

        private void LogIn()
        {
            this.Cursor = Cursors.WaitCursor;
            this.ToggleControls( false ); //disable everything on the form while validating/logging in.
            this.Refresh();
            Application.DoEvents();

            try
            {
                if ( ValidateUserCredentials( this.txtUserName.Text , this.txtPassword.Text ) && 
                    ValidateWorkstationID( this.mtbWorkstationID.UnMaskedText ) )
                {
                    SecurityResponse securityResponse = this.LogOnService.LogOn( this.txtUserName.Text , this.txtPassword.Text, this.SelectedFacility );

                    if( securityResponse.PatientAccessUser != null )
                    {
                        securityResponse.PatientAccessUser.WorkstationID = this.mtbWorkstationID.UnMaskedText;
                        BreadCrumbLogger.GetInstance.Log( "credentials verified" );
                        BreadCrumbLogger.GetInstance.Log( string.Format("for facility - {0}", securityResponse.PatientAccessUser.Facility.Code ) );
                    }

                    this.ProcessSecurityResponse( securityResponse );
                }
                else
                {   
                     BreadCrumbLogger.GetInstance.Log( "authentication failed" );
                    //display Login failure message
                    MessageBox.Show( ErrorMessage, 
                        UIErrorMessages.LOGIN_FAILURE_MESSAGE_TITLE,
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Exclamation );
                
                    ToggleControls( true );  // enable everything except do not enable mtbWorkstationID if chkDisableWorkstationID is checked.
                    this.Refresh();
                    Application.DoEvents();

                    if( this.Controls[ControlToFocus].CanSelect )
                    {
                        this.Controls[ControlToFocus].Select();
                    }
                }
            }
            catch( Exception ex )
            {
                // 12/8/2006 Handle the same as in _Load... 

                c_log.Error( ex.Message, ex );

                this.ToggleControls( true );

                if( ex is VersionNotMatchedException )
                {
                    MessageBox.Show( UIErrorMessages.VERSION_NOT_MATCHED, "Version is out-of-date", MessageBoxButtons.OK, MessageBoxIcon.Stop );
                    this.Close();
                }
                else
                {
                    MessageBox.Show( UIErrorMessages.ERROR_INITIALIZING_APP,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                }
                this.btnCancel.Enabled = true;
                return;

            }
            finally
            {
                DialogResult = DialogResult.None;
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Closes the login window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Handles the TextChanged event of the mtbWorkstationID control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void mtbWorkstationID_TextChanged( object sender, EventArgs e )
        {
            if( mtbWorkstationID == null || 
                mtbWorkstationID.UnMaskedText.Trim().Equals( string.Empty ) ||
                mtbWorkstationID.UnMaskedText.Trim().Length == 0 )
            {
                chkDisableWorkstationID.Enabled = false;
            } 
            else
            {
                if( !mtbWorkstationID.IsValid )
                {
                    mtbWorkstationID.UnMaskedText = string.Empty;
                    chkDisableWorkstationID.Enabled = false;
                }
                else
                {
                    chkDisableWorkstationID.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Handles the CheckedChanged event of the chkDisableWorkstationID control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void chkDisableWorkstationID_CheckedChanged( object sender, EventArgs e )
        {
            if( this.chkDisableWorkstationID.Checked ) 
            {
                this.DisableWorkstationID();
            }
            else
            {
                this.EnableWorkstationID();
            }
        }
        #endregion

        #region Methods
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        /// <summary>
        /// Processes the security response.
        /// </summary>
        /// <param name="securityResponse">The security response.</param>
        private void ProcessSecurityResponse( SecurityResponse securityResponse )
        {
            if( securityResponse.CanLogin() )
            {
                this.ShowInTaskbar = false;
                this.SaveUserPreferences();
                this.SelectedFacility.SetFacilityStateCode();
                PatientAccessView patientAccessView = ViewFactory.Instance.CreateView<PatientAccessView>();
                
                //Hide the logon view here to reduce the delay betwen dialogs.
                this.Hide(); 

                //Display the Patient Access View as a modal window.
                DialogResult result = patientAccessView.ShowDialog( this );

                this.ResetControls();   //remove the user id and password values from the form.
                ToggleControls( true ); //enable everything except mtbWorkstationID if chkDisableWorkstationID is checked.
                this.Refresh();
                Application.DoEvents();

                this.Credentials();
                this.Show();
                this.txtUserName.Focus();
                this.ShowInTaskbar = true;

                if( result == DialogResult.Yes)
                {
                    patientAccessView.Dispose();
                }
                else if( result == DialogResult.Retry )
                {
                    patientAccessView.Close();
                }
                else if( result == DialogResult.Abort )
                {
                    patientAccessView.CloseAccountSupplementalInformationView();
                    patientAccessView.Dispose();
                    MessageBox.Show( UIErrorMessages.INACTIVITY_TIMEOUT_MESSAGE, "TimeOut",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1 );
                }
                else
                {
                    this.Close();
                    Application.Exit();            
                }
            }
            else
            {
                //display Login failure message
                MessageBox.Show( securityResponse.LoginFailureMessage, 
                    UIErrorMessages.LOGIN_FAILURE_MESSAGE_TITLE,
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Exclamation );

                ToggleControls( true );  // enable everything except do not enable chkDisableWorkstationID if mtbWorkstationID is empty.
                this.txtUserName.Focus();
                this.txtUserName.SelectAll();
            }
        }

        private void Credentials()
        {
            string userName      = "";
            string password      = "";
            string workstationID = string.Empty;
            bool obtainDevCredentials = Convert.ToBoolean( ConfigurationManager.AppSettings[OBTAIN_DEV_CREDENTIALS] );
            if( obtainDevCredentials )
            {   //try to get credentials from server
                this.LogOnService.Credentials( out userName, out password, out workstationID );
                if( userName == null || userName.Trim().Equals( string.Empty ) ||
                    password == null || password.Trim().Equals( string.Empty ) ||
                    workstationID == null || workstationID.Trim().Equals( string.Empty ))
                {
                    this.ResetControls();

                    if( Preferences.WorkstationID.Length != 0 )
                    {
                        this.SetWorkstationID( Preferences.WorkstationID, Preferences.DisableWorkstationID );

                    }
                }
                else
                {
                    this.txtUserName.Text = userName;
                    this.txtPassword.Text = password;

                    if( Preferences.WorkstationID.Length != 0 )
                    {
                        this.SetWorkstationID( Preferences.WorkstationID, Preferences.DisableWorkstationID );
                    }
                    else
                    {
                        this.SetWorkstationID( workstationID, Convert.ToString( true ) );
                    }
                }
            }
            else
            {
                if( Preferences.WorkstationID != null )
                {
                    this.SetWorkstationID( Preferences.WorkstationID, Preferences.DisableWorkstationID );
                }
            }
        }

        /// <summary>
        /// Saves the user's preferences to disk
        /// </summary>
        private void SaveUserPreferences()
        {
            Preferences.LastUsedFacilityCode = this.SelectedFacility.Code;
            Preferences.WorkstationID        = this.mtbWorkstationID.UnMaskedText;
            Preferences.DisableWorkstationID = Convert.ToString( this.chkDisableWorkstationID.Checked );
            LogOnService.SaveUserPreference( Preferences );
        }

        /// <summary>
        /// Resets the username and password fields to blank values.
        /// </summary>
        private void ResetControls()
        {
            this.txtUserName.Text = string.Empty;
            this.txtPassword.Text = string.Empty;
        }

        //HACK: Clean this routine up.
        private void ToggleControls( bool enabled )
        {
            try
            {
                foreach( Control ctl in this.Controls )
                {
                    //ctl.Enabled = ( enabled && ctl.Name == "chkDisableWorkstationID" && mtbWorkstationID.UnMaskedText.Length == 0 ? !enabled : ctl.Enabled );

                    //On 'true' do not enable mtbWorkstationID if chkDisableWorkstationID is checked.
                    if( enabled && ctl.Name == "mtbWorkstationID" && chkDisableWorkstationID.Checked )
                    {
                        ctl.Enabled = !enabled;
                    }
                    //On 'true' do not enable chkDisableWorkstationID if mtbWorkstationID is empty.
                    else if( enabled && ctl.Name == "chkDisableWorkstationID" && mtbWorkstationID.UnMaskedText.Length == 0 )
                    {
                        ctl.Enabled = !enabled;
                    }
                    //On 'false' enable mtbWorkstationID if chkDisableWorkstationID is checked.
                    else if( !enabled && ctl.Name == "mtbWorkstationID" && chkDisableWorkstationID.Checked )
                    {
                        ctl.Enabled = enabled;
                    }
                    //On 'false' enable chkDisableWorkstationID if mtbWorkstationID is not empty.
                    else if( !enabled && ctl.Name == "chkDisableWorkstationID" && mtbWorkstationID.UnMaskedText.Length != 0 )
                    {
                        ctl.Enabled = enabled;
                    }
                    //All others, change enabled setting.
                    else
                    {
                        ctl.Enabled = enabled;
                    }
                }
            }
            catch( Exception Ex )
            {
                c_log.Error( "Exception", Ex );
                throw;
            }
        }

        // Determine whether the User ID or Password contain a zero-length string.
        private bool ValidateUserCredentials( string userName, string password )
        {
            if( userName == null || userName.Trim().Equals( string.Empty ) ||
                password == null || password.Trim().Equals( string.Empty ) )
            {
                ErrorMessage = UIErrorMessages.LOGIN_FAILURE_MESSAGE;
                ControlToFocus = this.Controls.IndexOf( this.txtUserName );
                return false;                
            }
            else
            {
                ErrorMessage = string.Empty;
                ControlToFocus = 0;
                return true;
            }
        }

        // Determine whether the Workstation text box contains a zero-length string.
        private bool ValidateWorkstationID( string workstationID )
        {
            if( workstationID == null || workstationID.Trim().Equals( string.Empty ) )
            {
                ErrorMessage = UIErrorMessages.WORKSTATION_INVALID_MESSAGE;
                ControlToFocus = this.Controls.IndexOf( this.mtbWorkstationID );
                return false;                
            }
            else
            {
                ErrorMessage = String.Empty;
                ControlToFocus = 0;            
                return true;
            }
        }

        private void DisplayFacilities()
        {   
            ICollection allFacilities = null;
                        
            allFacilities = this.LogOnService.AllFacilities();
             
            cmbFacilities.ValueMember   = "Code";
            cmbFacilities.DisplayMember = "Description";
            bool firstFacility = true;

            foreach( Facility facility in allFacilities )
            {;
                if( IsNotStandardFacility( facility ) )
                {
                    cmbFacilities.Items.Add( facility );
                }
                
                //pre-select first Facility from the list in case we won't find a matching Facility later:
                if( firstFacility )
                {
                    cmbFacilities.SelectedItem = facility;
                    firstFacility = false;
                }
                if( facility.Code.Equals( Preferences.LastUsedFacilityCode ) ) 
                {
                    cmbFacilities.SelectedItem = facility;
                }
            }
        }

        /// <summary>
        /// STD is a facility that is used by PBAR to store 'default' values, it is not
        /// a real facility.  We do not want to show this as a potential facility for which
        /// users can log in.
        /// </summary>
        /// <param name="aFacility">A Facility to evaluate.</param>
        /// <returns>true if the facility is not STD, false if it is.</returns>
        private static bool IsNotStandardFacility( Facility aFacility )
        {
            return !aFacility.Code.Trim().Equals( "STD" );
        }

        private void SetWorkstationID( string workstationID, string disableWorkstationID )
        {
            try
            {
                if( workstationID.Length != 0 )
                {
                    mtbWorkstationID.UnMaskedText = workstationID;
                    chkDisableWorkstationID.Enabled = true;

                    if( disableWorkstationID.Length != 0 )
                    {
                        chkDisableWorkstationID.Checked = Convert.ToBoolean( disableWorkstationID );
                    }

                    if( !mtbWorkstationID.IsValid )
                    {
                        mtbWorkstationID.UnMaskedText = string.Empty;
                        chkDisableWorkstationID.Enabled = false;
                    }
                }
            }
            catch( Exception Ex )
            {
                c_log.Error( "Exception", Ex );
                mtbWorkstationID.UnMaskedText = string.Empty;
                chkDisableWorkstationID.Enabled = false;
            }
        }

        private void DisableWorkstationID()
        {
            this.mtbWorkstationID.Enabled = false;
        }

        private void EnableWorkstationID()
        {
            this.mtbWorkstationID.Enabled = true;
        }

        private void fadeTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (i_showing)
                {
                    double d = 1000.0 / fadeTimer.Interval / 100.0;
                    if (Opacity + d >= 1.0)
                    {
                        Opacity = 1.0;
                        fadeTimer.Stop();
                    }
                    else
                    {
                        Opacity += d;
                    }
                }
                else
                {
                    double d = 1000.0 / fadeTimer.Interval / 100.0;
                    if (Opacity - d <= 0.0)
                    {
                        Opacity = 0.0;
                        fadeTimer.Stop();
                    }
                    else
                    {
                        Opacity -= d;
                    }
                }
            }
            catch( Exception Ex )
            {
                c_log.Error( "Exception", Ex );
            }
        }

        private void LogOnView_Closing(object sender, CancelEventArgs e)
        {
            i_showing = false;
            fadeTimer.Start();
        }

        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if (components != null) 
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        private void DisplayVersionNumber()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            lblVersion.Text = String.Format( lblVersion.Text, version );
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(LogOnView));
            this.fadeTimer = new System.Windows.Forms.Timer(this.components);
            this.picBoxLogin = new System.Windows.Forms.PictureBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblFacility = new System.Windows.Forms.Label();
            this.lblWorkstationID = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.cmbFacilities = new PatientAccess.UI.CommonControls.LogonFacilityAutoWidthComboBox();
            this.mtbWorkstationID = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.chkDisableWorkstationID = new System.Windows.Forms.CheckBox();
            this.btnOk = new LoggingButton();
            this.btnCancel = new LoggingButton();
            this.logOnErrors = new System.Windows.Forms.ErrorProvider();
            this.lblVersion = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // fadeTimer
            // 
            this.fadeTimer.Interval = 50;
            this.fadeTimer.Tick += new System.EventHandler(this.fadeTimer_Tick);
            // 
            // picBoxLogin
            // 
            this.picBoxLogin.Image = ((System.Drawing.Image)(resources.GetObject("picBoxLogin.Image")));
            this.picBoxLogin.Location = new System.Drawing.Point(0, 0);
            this.picBoxLogin.Name = "picBoxLogin";
            this.picBoxLogin.Size = new System.Drawing.Size(314, 58);
            this.picBoxLogin.TabIndex = 10;
            this.picBoxLogin.TabStop = false;
            // 
            // lblUserName
            // 
            this.lblUserName.Location = new System.Drawing.Point(14, 76);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(88, 23);
            this.lblUserName.TabIndex = 0;
            this.lblUserName.Text = "&User ID:";
            this.lblUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPassword
            // 
            this.lblPassword.Location = new System.Drawing.Point(14, 108);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(88, 23);
            this.lblPassword.TabIndex = 2;
            this.lblPassword.Text = "&Password:";
            this.lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblFacility
            // 
            this.lblFacility.Location = new System.Drawing.Point(14, 140);
            this.lblFacility.Name = "lblFacility";
            this.lblFacility.Size = new System.Drawing.Size(88, 23);
            this.lblFacility.TabIndex = 4;
            this.lblFacility.Text = "&Facility:";
            this.lblFacility.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblWorkstationID
            // 
            this.lblWorkstationID.Location = new System.Drawing.Point(14, 172);
            this.lblWorkstationID.Name = "lblWorkstationID";
            this.lblWorkstationID.Size = new System.Drawing.Size(88, 23);
            this.lblWorkstationID.TabIndex = 6;
            this.lblWorkstationID.Text = "&Workstation ID:";
            this.lblWorkstationID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtUserName
            // 
            this.txtUserName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserName.Location = new System.Drawing.Point(104, 72);
            this.txtUserName.MaxLength = 34;
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(200, 20);
            this.txtUserName.TabIndex = 1;
            this.txtUserName.Text = "";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(104, 104);
            this.txtPassword.MaxLength = 100;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '\u25CF';
            this.txtPassword.Size = new System.Drawing.Size(200, 20);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.Text = "";
            // 
            // cmbFacilities
            // 
            this.cmbFacilities.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbFacilities.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFacilities.DropDownWidth = 200;
            this.cmbFacilities.Location = new System.Drawing.Point(104, 136);
            this.cmbFacilities.Name = "cmbFacilities";
            this.cmbFacilities.Size = new System.Drawing.Size(200, 21);
            this.cmbFacilities.TabIndex = 5;
            // 
            // mtbWorkstationID
            // 
            this.mtbWorkstationID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mtbWorkstationID.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbWorkstationID.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbWorkstationID.KeyPressExpression = "^[^\\d&*%+= ][^&*%+= ']*$";
            this.mtbWorkstationID.Location = new System.Drawing.Point(104, 168);
            this.mtbWorkstationID.Mask = "";
            this.mtbWorkstationID.MaxLength = 10;
            this.mtbWorkstationID.Name = "mtbWorkstationID";
            this.mtbWorkstationID.Size = new System.Drawing.Size(200, 20);
            this.mtbWorkstationID.TabIndex = 7;
            this.mtbWorkstationID.ValidationExpression = "^[^\\d&*%+= ][^&*%+= ']*$";
            this.mtbWorkstationID.TextChanged += new System.EventHandler(this.mtbWorkstationID_TextChanged);
            // 
            // chkDisableWorkstationID
            // 
            this.chkDisableWorkstationID.Location = new System.Drawing.Point(104, 194);
            this.chkDisableWorkstationID.Name = "chkDisableWorkstationID";
            this.chkDisableWorkstationID.Size = new System.Drawing.Size(224, 24);
            this.chkDisableWorkstationID.TabIndex = 8;
            this.chkDisableWorkstationID.Text = "&Do not allow editing of Workstation ID";
            this.chkDisableWorkstationID.CheckedChanged += new System.EventHandler(this.chkDisableWorkstationID_CheckedChanged);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(130, 224);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(80, 23);
            this.btnOk.TabIndex = 9;
            this.btnOk.Text = "&Log On";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(224, 224);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // logOnErrors
            // 
            this.logOnErrors.ContainerControl = this;
            // 
            // lblVersion
            // 
            this.lblVersion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblVersion.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblVersion.Location = new System.Drawing.Point(0, 245);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(128, 23);
            this.lblVersion.TabIndex = 11;
            this.lblVersion.Text = " Version: {0}";
            // 
            // LogOnView
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(314, 260);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.picBoxLogin);
            this.Controls.Add(this.lblUserName);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.lblFacility);
            this.Controls.Add(this.lblWorkstationID);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.mtbWorkstationID);
            this.Controls.Add(this.cmbFacilities);
            this.Controls.Add(this.chkDisableWorkstationID);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LogOnView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Patient Access Log On";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.LogOnView_Closing);
            this.Load += new System.EventHandler(this.LogOnView_Load);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Private Properties

        private LogOnService LogOnService
        {
            get
            {
                return i_LogOnService;
            }
            set
            {
                i_LogOnService = value;
            }
        }

        private Facility SelectedFacility
        {
            get
            {
                return this.cmbFacilities.SelectedItem as Facility;
            }
        }

        private UserPreference Preferences
        {
            get
            {
                return i_Preferences;
            }
            set
            {
                i_Preferences = value;
            }
        }

        private string ErrorMessage
        {
            get
            {
                if( i_ErrorMessage != null )
                {
                    return i_ErrorMessage;
                } 
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if( value != null )
                {
                    i_ErrorMessage = value;
                } 
                else
                {
                    i_ErrorMessage = string.Empty;
                }
            }
        }

        private int ControlToFocus
        {
            get
            {
                return i_ControlToFocus;
            }
            set
            {
                i_ControlToFocus = value;
            }
        }
        #endregion

        #region Construction and Finalization
        /// <summary>
        /// LogOnView Constructor.
        /// </summary>
        public LogOnView()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.DisplayVersionNumber();

            base.EnableThemesOn( this );
            this.btnOk.Message = "verifying credentials";
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log = LogManager.GetLogger( typeof( LogOnView ) );
        private static readonly ILog AuditLogger = LogManager.GetLogger( AuditLoggerName );
        private Timer fadeTimer;
        private PictureBox picBoxLogin;
        private Label lblUserName;
        private Label lblPassword;
        private Label lblFacility;
        private Label lblWorkstationID;
        private Label lblVersion;
        private TextBox txtUserName;
        private TextBox txtPassword;
        private LogonFacilityAutoWidthComboBox cmbFacilities;
        private MaskedEditTextBox mtbWorkstationID;
        private CheckBox chkDisableWorkstationID;
        private LoggingButton btnOk;
        private LoggingButton btnCancel;
        private ErrorProvider logOnErrors;
        private IContainer components;
        private LogOnService i_LogOnService;
        private bool i_showing = true;
        private string i_ErrorMessage = string.Empty;
        private int i_ControlToFocus;
        private UserPreference i_Preferences;
        private const string AuditLoggerName = "AuditLogger";

        #endregion
 
        #region Constants
        private const string 
            OBTAIN_DEV_CREDENTIALS  = "ObtainDevCredentials",
            ADAM_ERROR_MSG = "An error has occured while contacting the ADAM Security service to validate requesting application.";
        #endregion
    }
}
