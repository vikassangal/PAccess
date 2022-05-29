using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.ShortRegistration.DiagnosisViews
{
    partial class ShortPatientTypeHSVLocationView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.lblHospService = new System.Windows.Forms.Label();
            this.cbxReregister = new System.Windows.Forms.CheckBox();
            this.lblPatientType = new System.Windows.Forms.Label();
            this.cmbHospitalService = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cmbPatientType = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.SuspendLayout();
            // 
            // lblHospService
            // 
            this.lblHospService.Location = new System.Drawing.Point( 4, 33 );
            this.lblHospService.Name = "lblHospService";
            this.lblHospService.Size = new System.Drawing.Size( 86, 13 );
            this.lblHospService.TabIndex = 26;
            this.lblHospService.Text = "Hospital service:";
            // 
            // cbxReregister
            // 
            this.cbxReregister.Enabled = false;
            this.cbxReregister.Location = new System.Drawing.Point( 234, 3 );
            this.cbxReregister.Name = "cbxReregister";
            this.cbxReregister.Size = new System.Drawing.Size( 91, 19 );
            this.cbxReregister.TabIndex = 29;
            this.cbxReregister.Text = "Reregister";
            this.cbxReregister.CheckedChanged += new System.EventHandler( this.cbxReregister_CheckedChanged );
            // 
            // lblPatientType
            // 
            this.lblPatientType.Location = new System.Drawing.Point( 4, 3 );
            this.lblPatientType.Name = "lblPatientType";
            this.lblPatientType.Size = new System.Drawing.Size( 68, 13 );
            this.lblPatientType.TabIndex = 27;
            this.lblPatientType.Text = "Patient type:";
            // 
            // cmbHospitalService
            // 
            this.cmbHospitalService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHospitalService.Enabled = false;
            this.cmbHospitalService.Location = new System.Drawing.Point( 103, 33 );
            this.cmbHospitalService.Name = "cmbHospitalService";
            this.cmbHospitalService.Size = new System.Drawing.Size( 202, 21 );
            this.cmbHospitalService.TabIndex = 30;
            this.cmbHospitalService.SelectedIndexChanged += new System.EventHandler( this.cmbHospitalService_SelectedIndexChanged );
            this.cmbHospitalService.Validating += new System.ComponentModel.CancelEventHandler( this.cmbHospitalService_Validating );
            // 
            // cmbPatientType
            // 
            this.cmbPatientType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPatientType.Location = new System.Drawing.Point( 103, 3 );
            this.cmbPatientType.Name = "cmbPatientType";
            this.cmbPatientType.Size = new System.Drawing.Size( 121, 21 );
            this.cmbPatientType.TabIndex = 28;
            this.cmbPatientType.SelectedIndexChanged += new System.EventHandler( this.cmbPatientType_SelectedIndexChanged );
            this.cmbPatientType.Validated += new System.EventHandler( this.cmbPatientType_Validated );
            // 
            // ShortPatientTypeHSVLocationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add( this.cmbHospitalService );
            this.Controls.Add( this.lblHospService );
            this.Controls.Add( this.cmbPatientType );
            this.Controls.Add( this.cbxReregister );
            this.Controls.Add( this.lblPatientType );
            this.Name = "ShortPatientTypeHSVLocationView";
            this.Size = new System.Drawing.Size( 338, 63 );
            this.Disposed += new System.EventHandler( this.PatientTypeHSVLocationView_Disposed );
            this.ResumeLayout( false );

        }

        #endregion

        private PatientAccessComboBox cmbHospitalService;
        private System.Windows.Forms.Label lblHospService;
        private PatientAccessComboBox cmbPatientType;
        private System.Windows.Forms.CheckBox cbxReregister;
        private System.Windows.Forms.Label lblPatientType;
    }
}
