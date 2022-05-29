namespace PatientAccess.UI.AddressViews
{
    partial class AddressEntryWithCountyView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnVerify = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.comboBox_Counties = new System.Windows.Forms.ComboBox();
            this.lblCounty = new System.Windows.Forms.Label();
            this.cmbZipCodeStatus = new System.Windows.Forms.ComboBox();
            this.lblCountry = new System.Windows.Forms.Label();
            this.lblStreet = new System.Windows.Forms.Label();
            this.lblStreet2 = new System.Windows.Forms.Label();
            this.lblZip = new System.Windows.Forms.Label();
            this.lblCity = new System.Windows.Forms.Label();
            this.lblState = new System.Windows.Forms.Label();
            this.comboBox_Countries = new System.Windows.Forms.ComboBox();
            this.mtbStreet = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbStreet2 = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbCity = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.comboBox_States = new System.Windows.Forms.ComboBox();
            this.lblCountryDesc = new System.Windows.Forms.Label();
            this.axZipCode = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.btnCancel = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblInfo
            // 
            this.lblInfo.Location = new System.Drawing.Point( 5, 4 );
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size( 448, 13 );
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "For US address verification, provide Street and either ZIP or City with State, or" +
                " all fields.";
            // 
            // btnVerify
            // 
            this.btnVerify.BackColor = System.Drawing.SystemColors.Control;
            this.btnVerify.Enabled = false;
            this.btnVerify.Location = new System.Drawing.Point( 307, 2 );
            this.btnVerify.Message = null;
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.Size = new System.Drawing.Size( 75, 22 );
            this.btnVerify.TabIndex = 1;
            this.btnVerify.Text = "&Verify";
            this.btnVerify.UseVisualStyleBackColor = false;
            this.btnVerify.BringToFront();
            this.btnVerify.Click += new System.EventHandler( this.btnVerify_Click );
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add( this.comboBox_Counties );
            this.panel2.Controls.Add( this.lblCounty );
            this.panel2.Controls.Add( this.cmbZipCodeStatus );
            this.panel2.Controls.Add( this.lblInfo );
            this.panel2.Controls.Add( this.lblCountry );
            this.panel2.Controls.Add( this.lblStreet );
            this.panel2.Controls.Add( this.lblStreet2 );
            this.panel2.Controls.Add( this.lblZip );
            this.panel2.Controls.Add( this.lblCity );
            this.panel2.Controls.Add( this.lblState );
            this.panel2.Controls.Add( this.comboBox_Countries );
            this.panel2.Controls.Add( this.mtbStreet );
            this.panel2.Controls.Add( this.mtbStreet2 );
            this.panel2.Controls.Add( this.mtbCity );
            this.panel2.Controls.Add( this.comboBox_States );
            this.panel2.Controls.Add( this.lblCountryDesc );
            this.panel2.Controls.Add( this.axZipCode );
            this.panel2.Location = new System.Drawing.Point( 0, 0 );
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(466, 215);
            this.panel2.TabIndex = 2;
            // 
            // comboBox_Counties
            // 
            this.comboBox_Counties.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Counties.Location = new System.Drawing.Point( 92, 184 );
            this.comboBox_Counties.MaxLength = 36;
            this.comboBox_Counties.Name = "comboBox_Counties";
            this.comboBox_Counties.Size = new System.Drawing.Size( 292, 21 );
            this.comboBox_Counties.TabIndex = 7;
            this.comboBox_Counties.Validating += new System.ComponentModel.CancelEventHandler( this.comboBox_Counties_Validating );
            this.comboBox_Counties.SelectedIndexChanged += new System.EventHandler( this.comboBox_Counties_SelectedIndexChanged );
            this.comboBox_Counties.DropDown += new System.EventHandler( this.comboBox_Counties_DropDown );
            // 
            // lblCounty
            // 
            this.lblCounty.Location = new System.Drawing.Point( 5, 189 );
            this.lblCounty.Name = "lblCounty";
            this.lblCounty.Size = new System.Drawing.Size( 57, 14 );
            this.lblCounty.TabIndex = 0;
            this.lblCounty.Text = "County:";
            // 
            // cmbZipCodeStatus
            // 
            this.cmbZipCodeStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbZipCodeStatus.Location = new System.Drawing.Point( 92, 102 );
            this.cmbZipCodeStatus.Name = "cmbZipCodeStatus";
            this.cmbZipCodeStatus.Size = new System.Drawing.Size( 187, 21 );
            this.cmbZipCodeStatus.TabIndex = 3;
            this.cmbZipCodeStatus.Visible = false;
            this.cmbZipCodeStatus.SelectedIndexChanged += new System.EventHandler( this.cmbZipCodeStatus_SelectedIndexChanged );
            // 
            // lblCountry
            // 
            this.lblCountry.Location = new System.Drawing.Point( 5, 28 );
            this.lblCountry.Name = "lblCountry";
            this.lblCountry.Size = new System.Drawing.Size( 48, 14 );
            this.lblCountry.TabIndex = 0;
            this.lblCountry.Text = "Country:";
            // 
            // lblStreet
            // 
            this.lblStreet.Location = new System.Drawing.Point( 5, 54 );
            this.lblStreet.Name = "lblStreet";
            this.lblStreet.Size = new System.Drawing.Size( 42, 14 );
            this.lblStreet.TabIndex = 0;
            this.lblStreet.Text = "Street1:";
            // 
            // lblStreet2
            // 
            this.lblStreet2.Location = new System.Drawing.Point(5, 80);
            this.lblStreet2.Name = "lblStreet2";
            this.lblStreet2.Size = new System.Drawing.Size(42, 14);
            this.lblStreet2.TabIndex = 0;
            this.lblStreet2.Text = "Street2:";
            // 
            // lblZip
            // 
            this.lblZip.Location = new System.Drawing.Point( 5, 108 );
            this.lblZip.Name = "lblZip";
            this.lblZip.Size = new System.Drawing.Size( 85, 14 );
            this.lblZip.TabIndex = 0;
            this.lblZip.Text = "Zip/Postal code:";
            // 
            // lblCity
            // 
            this.lblCity.Location = new System.Drawing.Point( 5, 135 );
            this.lblCity.Name = "lblCity";
            this.lblCity.Size = new System.Drawing.Size( 28, 14 );
            this.lblCity.TabIndex = 0;
            this.lblCity.Text = "City:";
            // 
            // lblState
            // 
            this.lblState.Location = new System.Drawing.Point( 5, 161 );
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size( 82, 14 );
            this.lblState.TabIndex = 0;
            this.lblState.Text = "State/Province:";
            // 
            // comboBox_Countries
            // 
            this.comboBox_Countries.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Countries.Location = new System.Drawing.Point( 92, 25 );
            this.comboBox_Countries.Name = "comboBox_Countries";
            this.comboBox_Countries.Size = new System.Drawing.Size( 187, 21 );
            this.comboBox_Countries.TabIndex = 1;
            this.comboBox_Countries.Validating += new System.ComponentModel.CancelEventHandler( this.comboBox_Countries_Validating );
            this.comboBox_Countries.SelectedIndexChanged += new System.EventHandler( this.comboBox_Countries_SelectedIndexChanged );
            this.comboBox_Countries.DropDown += new System.EventHandler( this.comboBox_Countries_DropDown );
            // 
            // mtbStreet
            // 
            this.mtbStreet.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbStreet.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbStreet.Location = new System.Drawing.Point( 92, 52 );
            this.mtbStreet.Mask = "";
            this.mtbStreet.MaxLength = 45;
            this.mtbStreet.Name = "mtbStreet";
            this.mtbStreet.Size = new System.Drawing.Size( 354, 20 );
            this.mtbStreet.TabIndex = 2;
            this.mtbStreet.TextChanged += new System.EventHandler( this.mtbStreet_TextChanged );
            this.mtbStreet.Enter += new System.EventHandler( this.mtbStreet_Enter );
            this.mtbStreet.Validating += new System.ComponentModel.CancelEventHandler( this.mtbStreet_Validating );
            //
            //mtbStreet2
            //
            this.mtbStreet2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbStreet2.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbStreet2.Location = new System.Drawing.Point(92, 78);
            this.mtbStreet2.Mask = "";
            this.mtbStreet2.MaxLength = 30;
            this.mtbStreet2.Name = "mtbStreet2";
            this.mtbStreet2.Size = new System.Drawing.Size(292, 20);
            this.mtbStreet2.TabIndex = 2;
            this.mtbStreet2.Validating += new System.ComponentModel.CancelEventHandler(this.mtbStreet2_Validating);
            // 
            // mtbCity
            // 
            this.mtbCity.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbCity.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbCity.Location = new System.Drawing.Point( 92, 130 );
            this.mtbCity.Mask = "";
            this.mtbCity.MaxLength = 15;
            this.mtbCity.Name = "mtbCity";
            this.mtbCity.Size = new System.Drawing.Size( 292, 20 );
            this.mtbCity.TabIndex = 5;
            this.mtbCity.TextChanged += new System.EventHandler( this.mtbCity_TextChanged );
            this.mtbCity.Enter += new System.EventHandler( this.mtbCity_Enter );
            this.mtbCity.Validating += new System.ComponentModel.CancelEventHandler( this.mtbCity_Validating );
            // 
            // comboBox_States
            // 
            this.comboBox_States.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_States.Location = new System.Drawing.Point( 92, 157 );
            this.comboBox_States.Name = "comboBox_States";
            this.comboBox_States.Size = new System.Drawing.Size( 187, 21 );
            this.comboBox_States.TabIndex = 6;
            this.comboBox_States.Validating += new System.ComponentModel.CancelEventHandler( this.comboBox_States_Validating );
            this.comboBox_States.SelectedIndexChanged += new System.EventHandler( this.comboBox_States_SelectedIndexChanged );
            // 
            // lblCountryDesc
            // 
            this.lblCountryDesc.Location = new System.Drawing.Point( 287, 28 );
            this.lblCountryDesc.Name = "lblCountryDesc";
            this.lblCountryDesc.Size = new System.Drawing.Size( 156, 14 );
            this.lblCountryDesc.TabIndex = 0;
            this.lblCountryDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // axZipCode
            // 
            this.axZipCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.axZipCode.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.axZipCode.Location = new System.Drawing.Point( 284, 104 );
            this.axZipCode.Mask = "";
            this.axZipCode.MaxLength = 9;
            this.axZipCode.Name = "axZipCode";
            this.axZipCode.Size = new System.Drawing.Size( 100, 20 );
            this.axZipCode.TabIndex = 4;
            this.axZipCode.TextChanged += new System.EventHandler( this.axZipCode_TextChanged );
            this.axZipCode.Validating += new System.ComponentModel.CancelEventHandler( this.axZipCode_Validating );
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point( 392, 2 );
            this.btnCancel.Message = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size( 75, 22 );
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler( this.btnCancel_Click );
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.panel1.Controls.Add( this.btnVerify );
            this.panel1.Controls.Add( this.btnCancel );
            this.panel1.Location = new System.Drawing.Point( 0, 220 );
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size( 468, 25 );
            this.panel1.TabIndex = 3;
            this.panel1.BringToFront();
            // 
            // AddressEntryWithCountyView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.Controls.Add( this.panel2 );
            this.Controls.Add( this.panel1 );
            this.Name = "AddressEntryWithCountyView";
            this.Size = new System.Drawing.Size( 468, 240 );
            this.Leave += new System.EventHandler( this.AddressEntryWithCountyView_Leave );
            this.Enter += new System.EventHandler( this.AddressEntryWithCountyView_Enter );
            this.panel2.ResumeLayout( false );
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout( false );
            this.ResumeLayout( false );

        }

        #endregion

        private System.ComponentModel.Container components = null;

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;

        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label lblCountry;
        private System.Windows.Forms.Label lblStreet;
        private System.Windows.Forms.Label lblStreet2;
        private System.Windows.Forms.Label lblZip;
        private System.Windows.Forms.Label lblCity;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.Label lblCounty;
        private System.Windows.Forms.Label lblCountryDesc;

        private System.Windows.Forms.ComboBox comboBox_Counties;
        private System.Windows.Forms.ComboBox comboBox_Countries;
        private System.Windows.Forms.ComboBox comboBox_States;
        private System.Windows.Forms.ComboBox cmbZipCodeStatus;

        private Extensions.UI.Winforms.MaskedEditTextBox mtbStreet;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbStreet2;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbCity;
        private Extensions.UI.Winforms.MaskedEditTextBox axZipCode;

        public PatientAccess.UI.CommonControls.LoggingButton btnVerify;
        private PatientAccess.UI.CommonControls.LoggingButton btnCancel;

    }
}
