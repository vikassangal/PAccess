using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.QuickAccountCreation.ViewImpl
{
    partial class QuickPatientTypeHSVLocationView
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
            this.lblPatientType = new System.Windows.Forms.Label();
            this.cmbHospitalService = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cmbPatientType = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.SuspendLayout();
            // 
            // lblHospService
            // 
            this.lblHospService.Location = new System.Drawing.Point(0, 41);
            this.lblHospService.Name = "lblHospService";
            this.lblHospService.Size = new System.Drawing.Size(90, 13);
            this.lblHospService.TabIndex = 26;
            this.lblHospService.Text = "Hospital service:";
            // 
            // lblPatientType
            // 
            this.lblPatientType.Location = new System.Drawing.Point(100, 11);
            this.lblPatientType.Name = "lblPatientType";
            this.lblPatientType.Size = new System.Drawing.Size(68, 13);
            this.lblPatientType.TabIndex = 27;
            this.lblPatientType.Text = "Patient type:";
            // 
            // cmbHospitalService
            // 
            this.cmbHospitalService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHospitalService.Enabled = false;
            this.cmbHospitalService.Location = new System.Drawing.Point(96, 38);
            this.cmbHospitalService.Name = "cmbHospitalService";
            this.cmbHospitalService.Size = new System.Drawing.Size(202, 21);
            this.cmbHospitalService.TabIndex = 30;
            this.cmbHospitalService.SelectedIndexChanged += new System.EventHandler(this.cmbHospitalService_SelectedIndexChanged);
            this.cmbHospitalService.Validating += new System.ComponentModel.CancelEventHandler(this.cmbHospitalService_Validating);
            // 
            // cmbPatientType
            // 
            this.cmbPatientType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPatientType.Location = new System.Drawing.Point(176, 7);
            this.cmbPatientType.Name = "cmbPatientType";
            this.cmbPatientType.Size = new System.Drawing.Size(121, 21);
            this.cmbPatientType.TabIndex = 28;
            this.cmbPatientType.SelectedIndexChanged += new System.EventHandler(this.cmbPatientType_SelectedIndexChanged);
            this.cmbPatientType.Validated += new System.EventHandler(this.cmbPatientType_Validated);
            // 
            // QuickPatientTypeHSVLocationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmbHospitalService);
            this.Controls.Add(this.lblHospService);
            this.Controls.Add(this.cmbPatientType);
            this.Controls.Add(this.lblPatientType);
            this.Name = "QuickPatientTypeHSVLocationView";
            this.Size = new System.Drawing.Size(300, 63);
            this.Disposed += new System.EventHandler(this.PatientTypeHSVLocationView_Disposed);
            this.ResumeLayout(false);

        }

        #endregion

        private PatientAccessComboBox cmbHospitalService;
        private System.Windows.Forms.Label lblHospService;
        private PatientAccessComboBox cmbPatientType;
        private System.Windows.Forms.Label lblPatientType;
    }
}
