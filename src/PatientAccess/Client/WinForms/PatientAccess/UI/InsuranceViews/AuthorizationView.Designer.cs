namespace PatientAccess.UI.InsuranceViews
{
    partial class AuthorizationView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if( disposing && ( components != null ) )
            {
                this.UnRegisterEvents();
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblAuthorizationUnAvailable = new System.Windows.Forms.Label();
            this.panelAuthorization = new System.Windows.Forms.Panel();
            this.mtbAuthorizationNumber = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbRemarks = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblRemarks = new System.Windows.Forms.Label();
            this.lblLastName = new System.Windows.Forms.Label();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.tbCompanyRepLastName = new System.Windows.Forms.TextBox();
            this.tbCompanyRepFirstName = new System.Windows.Forms.TextBox();
            this.tbTrackingNumber = new System.Windows.Forms.TextBox();
            this.lblAuthorizationCompanyRep = new System.Windows.Forms.Label();
            this.lblAuthorizationNumber = new System.Windows.Forms.Label();
            this.lblTrackingNumber = new System.Windows.Forms.Label();
            this.grpBxAuthorizationDetails = new System.Windows.Forms.GroupBox();
            this.mtbExpirationDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.dtpExpirationDate = new System.Windows.Forms.DateTimePicker();
            this.mtbEffectiveDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.dtpEffectiveDate = new System.Windows.Forms.DateTimePicker();
            this.cmbStatus = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblExpirationDate = new System.Windows.Forms.Label();
            this.lblEffectiveDate = new System.Windows.Forms.Label();
            this.tbServicesAuthorized = new System.Windows.Forms.TextBox();
            this.tbDaysAuthorized = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblServicesAuthorized = new System.Windows.Forms.Label();
            this.lblDaysAuthorized = new System.Windows.Forms.Label();
            this.insuranceVerificationSummary = new PatientAccess.UI.InsuranceViews.AuthorizationViews.InsuranceVerificationSummary();
            this.panel1.SuspendLayout();
            this.panelAuthorization.SuspendLayout();
            this.grpBxAuthorizationDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add( this.lblAuthorizationUnAvailable );
            this.panel1.Controls.Add( this.panelAuthorization );
            this.panel1.Controls.Add( this.insuranceVerificationSummary );
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point( 0, 0 );
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size( 880, 450 );
            this.panel1.TabIndex = 0;
            // 
            // lblAuthorizationUnAvailable
            // 
            this.lblAuthorizationUnAvailable.Enabled = false;
            this.lblAuthorizationUnAvailable.Location = new System.Drawing.Point( 7, 10 );
            this.lblAuthorizationUnAvailable.Name = "lblAuthorizationUnAvailable";
            this.lblAuthorizationUnAvailable.Size = new System.Drawing.Size( 649, 23 );
            this.lblAuthorizationUnAvailable.TabIndex = 3;
            this.lblAuthorizationUnAvailable.Text = "The Authorization screen is unavailable because the plan/coverage information ind" +
                "icated does not require authorization information.";
            this.lblAuthorizationUnAvailable.Visible = false;
            // 
            // panelAuthorization
            // 
            this.panelAuthorization.Controls.Add( this.mtbAuthorizationNumber );
            this.panelAuthorization.Controls.Add( this.mtbRemarks );
            this.panelAuthorization.Controls.Add( this.lblRemarks );
            this.panelAuthorization.Controls.Add( this.lblLastName );
            this.panelAuthorization.Controls.Add( this.lblFirstName );
            this.panelAuthorization.Controls.Add( this.tbCompanyRepLastName );
            this.panelAuthorization.Controls.Add( this.tbCompanyRepFirstName );
            this.panelAuthorization.Controls.Add( this.tbTrackingNumber );
            this.panelAuthorization.Controls.Add( this.lblAuthorizationCompanyRep );
            this.panelAuthorization.Controls.Add( this.lblAuthorizationNumber );
            this.panelAuthorization.Controls.Add( this.lblTrackingNumber );
            this.panelAuthorization.Controls.Add( this.grpBxAuthorizationDetails );
            this.panelAuthorization.Location = new System.Drawing.Point( 0, 77 );
            this.panelAuthorization.Name = "panelAuthorization";
            this.panelAuthorization.Size = new System.Drawing.Size( 880, 373 );
            this.panelAuthorization.TabIndex = 1;
            // 
            // mtbAuthorizationNumber
            // 
            this.mtbAuthorizationNumber.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.mtbAuthorizationNumber.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbAuthorizationNumber.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbAuthorizationNumber.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbAuthorizationNumber.Location = new System.Drawing.Point( 168, 32 );
            this.mtbAuthorizationNumber.Mask = "";
            this.mtbAuthorizationNumber.MaxLength = 30;
            this.mtbAuthorizationNumber.Name = "mtbAuthorizationNumber";
            this.mtbAuthorizationNumber.Size = new System.Drawing.Size( 239, 20 );
            this.mtbAuthorizationNumber.TabIndex = 1;
            this.mtbAuthorizationNumber.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbAuthorizationNumber.Validating += new System.ComponentModel.CancelEventHandler( this.mtbAuthorizationNumber_Validating );
            this.mtbAuthorizationNumber.TextChanged += new System.EventHandler( this.mtbAuthorizationNumber_TextChanged );
            // 
            // mtbRemarks
            // 
            this.mtbRemarks.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbRemarks.Location = new System.Drawing.Point( 94, 282 );
            this.mtbRemarks.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbRemarks.MaxLength = 120;
            this.mtbRemarks.Mask = "";
            this.mtbRemarks.Multiline = true;
            this.mtbRemarks.Name = "tbRemarks";
            this.mtbRemarks.Size = new System.Drawing.Size( 294, 69 );
            this.mtbRemarks.TabIndex = 9;
            this.mtbRemarks.prePasteEdit = PatientAccess.UI.HelperClasses.CommonFormatting.PreFilter;
            // 
            // lblRemarks
            // 
            this.lblRemarks.AutoSize = true;
            this.lblRemarks.Location = new System.Drawing.Point( 36, 282 );
            this.lblRemarks.Name = "lblRemarks";
            this.lblRemarks.Size = new System.Drawing.Size( 52, 13 );
            this.lblRemarks.TabIndex = 0;
            this.lblRemarks.Text = "Remarks:";
            // 
            // lblLastName
            // 
            this.lblLastName.AutoSize = true;
            this.lblLastName.Location = new System.Drawing.Point( 351, 82 );
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size( 56, 13 );
            this.lblLastName.TabIndex = 0;
            this.lblLastName.Text = "Last name";
            // 
            // lblFirstName
            // 
            this.lblFirstName.AutoSize = true;
            this.lblFirstName.Location = new System.Drawing.Point( 180, 83 );
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size( 55, 13 );
            this.lblFirstName.TabIndex = 0;
            this.lblFirstName.Text = "First name";
            // 
            // tbCompanyRepLastName
            // 
            this.tbCompanyRepLastName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbCompanyRepLastName.Location = new System.Drawing.Point( 330, 59 );
            this.tbCompanyRepLastName.MaxLength = 25;
            this.tbCompanyRepLastName.Name = "tbCompanyRepLastName";
            this.tbCompanyRepLastName.Size = new System.Drawing.Size( 242, 20 );
            this.tbCompanyRepLastName.TabIndex = 3;
            this.tbCompanyRepLastName.Validating += new System.ComponentModel.CancelEventHandler( this.tbCompanyRepLastName_Validating );
            this.tbCompanyRepLastName.TextChanged += new System.EventHandler( this.tbCompanyRepLastName_TextChanged );
            // 
            // tbCompanyRepFirstName
            // 
            this.tbCompanyRepFirstName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbCompanyRepFirstName.Location = new System.Drawing.Point( 168, 59 );
            this.tbCompanyRepFirstName.MaxLength = 15;
            this.tbCompanyRepFirstName.Name = "tbCompanyRepFirstName";
            this.tbCompanyRepFirstName.Size = new System.Drawing.Size( 143, 20 );
            this.tbCompanyRepFirstName.TabIndex = 2;
            this.tbCompanyRepFirstName.Validating += new System.ComponentModel.CancelEventHandler( this.tbCompanyRepFirstName_Validating );
            this.tbCompanyRepFirstName.TextChanged += new System.EventHandler( this.tbCompanyRepFirstName_TextChanged );
            // 
            // tbTrackingNumber
            // 
            this.tbTrackingNumber.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbTrackingNumber.Location = new System.Drawing.Point( 168, 6 );
            this.tbTrackingNumber.MaxLength = 20;
            this.tbTrackingNumber.Name = "tbTrackingNumber";
            this.tbTrackingNumber.Size = new System.Drawing.Size( 159, 20 );
            this.tbTrackingNumber.TabIndex = 0;
            this.tbTrackingNumber.Validating += new System.ComponentModel.CancelEventHandler( this.tbTrackingNumber_Validating );
            // 
            // lblAuthorizationCompanyRep
            // 
            this.lblAuthorizationCompanyRep.AutoSize = true;
            this.lblAuthorizationCompanyRep.Location = new System.Drawing.Point( 17, 63 );
            this.lblAuthorizationCompanyRep.Name = "lblAuthorizationCompanyRep";
            this.lblAuthorizationCompanyRep.Size = new System.Drawing.Size( 135, 13 );
            this.lblAuthorizationCompanyRep.TabIndex = 0;
            this.lblAuthorizationCompanyRep.Text = "Authorization company rep:";
            // 
            // lblAuthorizationNumber
            // 
            this.lblAuthorizationNumber.AutoSize = true;
            this.lblAuthorizationNumber.Location = new System.Drawing.Point( 17, 36 );
            this.lblAuthorizationNumber.Name = "lblAuthorizationNumber";
            this.lblAuthorizationNumber.Size = new System.Drawing.Size( 109, 13 );
            this.lblAuthorizationNumber.TabIndex = 0;
            this.lblAuthorizationNumber.Text = "Authorization number:";
            // 
            // lblTrackingNumber
            // 
            this.lblTrackingNumber.AutoSize = true;
            this.lblTrackingNumber.Location = new System.Drawing.Point( 17, 9 );
            this.lblTrackingNumber.Name = "lblTrackingNumber";
            this.lblTrackingNumber.Size = new System.Drawing.Size( 90, 13 );
            this.lblTrackingNumber.TabIndex = 0;
            this.lblTrackingNumber.Text = "Tracking number:";
            // 
            // grpBxAuthorizationDetails
            // 
            this.grpBxAuthorizationDetails.Controls.Add( this.mtbExpirationDate );
            this.grpBxAuthorizationDetails.Controls.Add( this.dtpExpirationDate );
            this.grpBxAuthorizationDetails.Controls.Add( this.mtbEffectiveDate );
            this.grpBxAuthorizationDetails.Controls.Add( this.dtpEffectiveDate );
            this.grpBxAuthorizationDetails.Controls.Add( this.cmbStatus );
            this.grpBxAuthorizationDetails.Controls.Add( this.lblStatus );
            this.grpBxAuthorizationDetails.Controls.Add( this.lblExpirationDate );
            this.grpBxAuthorizationDetails.Controls.Add( this.lblEffectiveDate );
            this.grpBxAuthorizationDetails.Controls.Add( this.tbServicesAuthorized );
            this.grpBxAuthorizationDetails.Controls.Add( this.tbDaysAuthorized );
            this.grpBxAuthorizationDetails.Controls.Add( this.lblServicesAuthorized );
            this.grpBxAuthorizationDetails.Controls.Add( this.lblDaysAuthorized );
            this.grpBxAuthorizationDetails.Location = new System.Drawing.Point( 17, 106 );
            this.grpBxAuthorizationDetails.Name = "grpBxAuthorizationDetails";
            this.grpBxAuthorizationDetails.Size = new System.Drawing.Size( 387, 160 );
            this.grpBxAuthorizationDetails.TabIndex = 4;
            this.grpBxAuthorizationDetails.TabStop = false;
            this.grpBxAuthorizationDetails.Text = "Authorization Details";
            // 
            // mtbExpirationDate
            // 
            this.mtbExpirationDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbExpirationDate.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbExpirationDate.Location = new System.Drawing.Point( 140, 103 );
            this.mtbExpirationDate.Mask = "  /  /";
            this.mtbExpirationDate.MaxLength = 10;
            this.mtbExpirationDate.Name = "mtbExpirationDate";
            this.mtbExpirationDate.Size = new System.Drawing.Size( 70, 20 );
            this.mtbExpirationDate.TabIndex = 7;
            this.mtbExpirationDate.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbExpirationDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbExpirationDate_Validating );
            // 
            // dtpExpirationDate
            // 
            this.dtpExpirationDate.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dtpExpirationDate.Checked = false;
            this.dtpExpirationDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpExpirationDate.Location = new System.Drawing.Point( 210, 103 );
            this.dtpExpirationDate.MinDate = new System.DateTime( 1800, 1, 1, 0, 0, 0, 0 );
            this.dtpExpirationDate.Name = "dtpExpirationDate";
            this.dtpExpirationDate.Size = new System.Drawing.Size( 21, 20 );
            this.dtpExpirationDate.TabIndex = 7;
            this.dtpExpirationDate.TabStop = false;
            this.dtpExpirationDate.CloseUp += new System.EventHandler( this.dtpExpirationDate_CloseUp );
            // 
            // mtbEffectiveDate
            // 
            this.mtbEffectiveDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbEffectiveDate.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbEffectiveDate.Location = new System.Drawing.Point( 140, 77 );
            this.mtbEffectiveDate.Mask = "  /  /";
            this.mtbEffectiveDate.MaxLength = 10;
            this.mtbEffectiveDate.Name = "mtbEffectiveDate";
            this.mtbEffectiveDate.Size = new System.Drawing.Size( 70, 20 );
            this.mtbEffectiveDate.TabIndex = 6;
            this.mtbEffectiveDate.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbEffectiveDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbEffectiveDate_Validating );
            // 
            // dtpEffectiveDate
            // 
            this.dtpEffectiveDate.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dtpEffectiveDate.Checked = false;
            this.dtpEffectiveDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpEffectiveDate.Location = new System.Drawing.Point( 210, 77 );
            this.dtpEffectiveDate.MinDate = new System.DateTime( 1800, 1, 1, 0, 0, 0, 0 );
            this.dtpEffectiveDate.Name = "dtpEffectiveDate";
            this.dtpEffectiveDate.Size = new System.Drawing.Size( 21, 20 );
            this.dtpEffectiveDate.TabIndex = 0;
            this.dtpEffectiveDate.TabStop = false;
            this.dtpEffectiveDate.CloseUp += new System.EventHandler( this.dtpEffectiveDate_CloseUp );
            // 
            // cmbStatus
            // 
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.FormattingEnabled = true;
            this.cmbStatus.Location = new System.Drawing.Point( 140, 127 );
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size( 121, 21 );
            this.cmbStatus.TabIndex = 8;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point( 19, 131 );
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size( 40, 13 );
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Status:";
            // 
            // lblExpirationDate
            // 
            this.lblExpirationDate.AutoSize = true;
            this.lblExpirationDate.Location = new System.Drawing.Point( 19, 106 );
            this.lblExpirationDate.Name = "lblExpirationDate";
            this.lblExpirationDate.Size = new System.Drawing.Size( 80, 13 );
            this.lblExpirationDate.TabIndex = 0;
            this.lblExpirationDate.Text = "Expiration date:";
            // 
            // lblEffectiveDate
            // 
            this.lblEffectiveDate.AutoSize = true;
            this.lblEffectiveDate.Location = new System.Drawing.Point( 19, 81 );
            this.lblEffectiveDate.Name = "lblEffectiveDate";
            this.lblEffectiveDate.Size = new System.Drawing.Size( 76, 13 );
            this.lblEffectiveDate.TabIndex = 0;
            this.lblEffectiveDate.Text = "Effective date:";
            // 
            // tbServicesAuthorized
            // 
            this.tbServicesAuthorized.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbServicesAuthorized.Location = new System.Drawing.Point( 140, 52 );
            this.tbServicesAuthorized.MaxLength = 30;
            this.tbServicesAuthorized.Name = "tbServicesAuthorized";
            this.tbServicesAuthorized.Size = new System.Drawing.Size( 231, 20 );
            this.tbServicesAuthorized.TabIndex = 5;
            this.tbServicesAuthorized.Validating += new System.ComponentModel.CancelEventHandler( this.tbServicesAuthorized_Validating );
            this.tbServicesAuthorized.TextChanged += new System.EventHandler( this.tbServicesAuthorized_TextChanged );
            // 
            // tbDaysAuthorized
            // 
            this.tbDaysAuthorized.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.tbDaysAuthorized.KeyPressExpression = "[0-9]*";
            this.tbDaysAuthorized.Location = new System.Drawing.Point( 140, 27 );
            this.tbDaysAuthorized.Mask = "";
            this.tbDaysAuthorized.MaxLength = 3;
            this.tbDaysAuthorized.Name = "tbDaysAuthorized";
            this.tbDaysAuthorized.Size = new System.Drawing.Size( 36, 20 );
            this.tbDaysAuthorized.TabIndex = 4;
            this.tbDaysAuthorized.ValidationExpression = "[0-9]*";
            // 
            // lblServicesAuthorized
            // 
            this.lblServicesAuthorized.AutoSize = true;
            this.lblServicesAuthorized.Location = new System.Drawing.Point( 19, 56 );
            this.lblServicesAuthorized.Name = "lblServicesAuthorized";
            this.lblServicesAuthorized.Size = new System.Drawing.Size( 103, 13 );
            this.lblServicesAuthorized.TabIndex = 1;
            this.lblServicesAuthorized.Text = "Services authorized:";
            // 
            // lblDaysAuthorized
            // 
            this.lblDaysAuthorized.AutoSize = true;
            this.lblDaysAuthorized.Location = new System.Drawing.Point( 19, 31 );
            this.lblDaysAuthorized.Name = "lblDaysAuthorized";
            this.lblDaysAuthorized.Size = new System.Drawing.Size( 86, 13 );
            this.lblDaysAuthorized.TabIndex = 0;
            this.lblDaysAuthorized.Text = "Days authorized:";
            // 
            // insuranceVerificationSummary
            // 
            this.insuranceVerificationSummary.Account = null;
            this.insuranceVerificationSummary.BackColor = System.Drawing.Color.White;
            this.insuranceVerificationSummary.Location = new System.Drawing.Point( 0, 0 );
            this.insuranceVerificationSummary.Model = null;
            this.insuranceVerificationSummary.Name = "insuranceVerificationSummary";
            this.insuranceVerificationSummary.Size = new System.Drawing.Size( 880, 78 );
            this.insuranceVerificationSummary.TabIndex = 0;
            this.insuranceVerificationSummary.TabStop = false;
            // 
            // AuthorizationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.panel1 );
            this.Name = "AuthorizationView";
            this.Size = new System.Drawing.Size( 880, 450 );
            this.Leave += new System.EventHandler( this.AuthorizationView_Leave );
            this.panel1.ResumeLayout( false );
            this.panelAuthorization.ResumeLayout( false );
            this.panelAuthorization.PerformLayout();
            this.grpBxAuthorizationDetails.ResumeLayout( false );
            this.grpBxAuthorizationDetails.PerformLayout();
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private PatientAccess.UI.InsuranceViews.AuthorizationViews.InsuranceVerificationSummary insuranceVerificationSummary;
        private System.Windows.Forms.Panel panelAuthorization;
        private System.Windows.Forms.GroupBox grpBxAuthorizationDetails;
        private System.Windows.Forms.Label lblTrackingNumber;
        private System.Windows.Forms.Label lblAuthorizationCompanyRep;
        private System.Windows.Forms.Label lblAuthorizationNumber;
        private System.Windows.Forms.TextBox tbTrackingNumber;
        private System.Windows.Forms.TextBox tbCompanyRepFirstName;
        private System.Windows.Forms.TextBox tbCompanyRepLastName;
        private System.Windows.Forms.Label lblLastName;
        private System.Windows.Forms.Label lblFirstName;
        private System.Windows.Forms.Label lblDaysAuthorized;
        private Extensions.UI.Winforms.MaskedEditTextBox tbDaysAuthorized;
        private System.Windows.Forms.Label lblServicesAuthorized;
        private System.Windows.Forms.TextBox tbServicesAuthorized;
        private System.Windows.Forms.Label lblExpirationDate;
        private System.Windows.Forms.Label lblEffectiveDate;
        private PatientAccess.UI.CommonControls.PatientAccessComboBox cmbStatus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblRemarks;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbRemarks;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbEffectiveDate;
        private System.Windows.Forms.DateTimePicker dtpEffectiveDate;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbExpirationDate;
        private System.Windows.Forms.DateTimePicker dtpExpirationDate;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbAuthorizationNumber;
        private System.Windows.Forms.Label lblAuthorizationUnAvailable;
    }
}
