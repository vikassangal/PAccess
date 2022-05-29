namespace PatientAccess.UI.CommonControls
{
    partial class PatientTypeHSVLocationView
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
            this.lblAccommodation = new System.Windows.Forms.Label();
            this.lblHospService = new System.Windows.Forms.Label();
            this.cbxReregister = new System.Windows.Forms.CheckBox();
            this.lblPatientType = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbReasonForPrivateAccommodation = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.locationView = new PatientAccess.UI.CommonControls.LocationView();
            this.cmbAccommodation = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cmbHospitalService = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cmbPatientType = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.SuspendLayout();
            // 
            // lblAccommodation
            // 
            this.lblAccommodation.Location = new System.Drawing.Point(4, 182);
            this.lblAccommodation.Name = "lblAccommodation";
            this.lblAccommodation.Size = new System.Drawing.Size(88, 14);
            this.lblAccommodation.TabIndex = 25;
            this.lblAccommodation.Text = "Accommodation:";
            // 
            // lblHospService
            // 
            this.lblHospService.Location = new System.Drawing.Point(4, 33);
            this.lblHospService.Name = "lblHospService";
            this.lblHospService.Size = new System.Drawing.Size(86, 13);
            this.lblHospService.TabIndex = 26;
            this.lblHospService.Text = "Hospital service:";
            // 
            // cbxReregister
            // 
            this.cbxReregister.Enabled = false;
            this.cbxReregister.Location = new System.Drawing.Point(234, 3);
            this.cbxReregister.Name = "cbxReregister";
            this.cbxReregister.Size = new System.Drawing.Size(91, 19);
            this.cbxReregister.TabIndex = 29;
            this.cbxReregister.Text = "Reregister";
            this.cbxReregister.CheckedChanged += new System.EventHandler(this.cbxReregister_CheckedChanged);
            // 
            // lblPatientType
            // 
            this.lblPatientType.Location = new System.Drawing.Point(4, 3);
            this.lblPatientType.Name = "lblPatientType";
            this.lblPatientType.Size = new System.Drawing.Size(68, 13);
            this.lblPatientType.TabIndex = 27;
            this.lblPatientType.Text = "Patient type:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(4, 209);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 30);
            this.label1.TabIndex = 33;
            this.label1.Text = "Reason For Private Accommodation:";
            // 
            // cmbReasonForPrivateAccommodation
            // 
            this.cmbReasonForPrivateAccommodation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReasonForPrivateAccommodation.Enabled = false;
            this.cmbReasonForPrivateAccommodation.Location = new System.Drawing.Point(108, 209);
            this.cmbReasonForPrivateAccommodation.Name = "cmbReasonForPrivateAccommodation";
            this.cmbReasonForPrivateAccommodation.Size = new System.Drawing.Size(217, 21);
            this.cmbReasonForPrivateAccommodation.TabIndex = 34;
            this.cmbReasonForPrivateAccommodation.Validating += new System.ComponentModel.CancelEventHandler(this.cmbReasonForPrivateAccommodation_Validating);
            this.cmbReasonForPrivateAccommodation.SelectedIndexChanged += new System.EventHandler(this.cmbReasonForPrivateAccommodation_SelectedIndexChanged);
            // 
            // locationView
            // 
            this.locationView.EditFindButtonText = "Find...";
            this.locationView.EditVerifyButtonText = "Verify";
            this.locationView.Location = new System.Drawing.Point(5, 60);
            this.locationView.Model = null;
            this.locationView.Name = "locationView";
            this.locationView.Size = new System.Drawing.Size(330, 114);
            this.locationView.TabIndex = 31;
            // 
            // cmbAccommodation
            // 
            this.cmbAccommodation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAccommodation.Enabled = false;
            this.cmbAccommodation.Location = new System.Drawing.Point(108, 182);
            this.cmbAccommodation.Name = "cmbAccommodation";
            this.cmbAccommodation.Size = new System.Drawing.Size(121, 21);
            this.cmbAccommodation.TabIndex = 32;
            this.cmbAccommodation.Validating += new System.ComponentModel.CancelEventHandler(this.cmbAccommodation_Validating);
            this.cmbAccommodation.SelectedIndexChanged += new System.EventHandler(this.cmbAccommodation_SelectedIndexChanged);
            // 
            // cmbHospitalService
            // 
            this.cmbHospitalService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHospitalService.Enabled = false;
            this.cmbHospitalService.Location = new System.Drawing.Point(103, 33);
            this.cmbHospitalService.Name = "cmbHospitalService";
            this.cmbHospitalService.Size = new System.Drawing.Size(202, 21);
            this.cmbHospitalService.TabIndex = 30;
            this.cmbHospitalService.Validating += new System.ComponentModel.CancelEventHandler(this.cmbHospitalService_Validating);
            this.cmbHospitalService.SelectedIndexChanged += new System.EventHandler(this.cmbHospitalService_SelectedIndexChanged);
            // 
            // cmbPatientType
            // 
            this.cmbPatientType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPatientType.Location = new System.Drawing.Point(103, 3);
            this.cmbPatientType.Name = "cmbPatientType";
            this.cmbPatientType.Size = new System.Drawing.Size(121, 21);
            this.cmbPatientType.TabIndex = 28;
            this.cmbPatientType.SelectedIndexChanged += new System.EventHandler(this.cmbPatientType_SelectedIndexChanged);
            this.cmbPatientType.Validated += new System.EventHandler(this.cmbPatientType_Validated);
            // 
            // PatientTypeHSVLocationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmbReasonForPrivateAccommodation);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.locationView);
            this.Controls.Add(this.cmbAccommodation);
            this.Controls.Add(this.lblAccommodation);
            this.Controls.Add(this.cmbHospitalService);
            this.Controls.Add(this.lblHospService);
            this.Controls.Add(this.cmbPatientType);
            this.Controls.Add(this.cbxReregister);
            this.Controls.Add(this.lblPatientType);
            this.Name = "PatientTypeHSVLocationView";
            this.Size = new System.Drawing.Size(338, 241);
            this.Load += new System.EventHandler(this.PatientTypeHSVLocationView_Load);
            this.Disposed += new System.EventHandler(this.PatientTypeHSVLocationView_Disposed);
            this.ResumeLayout(false);

        }

        #endregion

        private LocationView locationView;
        private PatientAccessComboBox cmbAccommodation;
        private System.Windows.Forms.Label lblAccommodation;
        private PatientAccessComboBox cmbHospitalService;
        private System.Windows.Forms.Label lblHospService;
        private PatientAccessComboBox cmbPatientType;
        private System.Windows.Forms.CheckBox cbxReregister;
        private System.Windows.Forms.Label lblPatientType;
        private PatientAccessComboBox cmbReasonForPrivateAccommodation;
        private System.Windows.Forms.Label label1;
    }
}
