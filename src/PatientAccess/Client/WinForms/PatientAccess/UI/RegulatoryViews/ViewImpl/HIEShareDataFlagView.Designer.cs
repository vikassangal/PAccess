using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.RegulatoryViews.ViewImpl
{
    partial class HIEShareDataFlagView
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
            this.lblShareDataWithPCPFlag = new System.Windows.Forms.Label();
            this.lblShareDataWithPublicHieFlag = new System.Windows.Forms.Label();
            this.cmbShareDataWithPCPFlag = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.cmbShareDataWithPublicHIEFlag = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.SuspendLayout();
            // 
            // lblShareDataWithPCPFlag
            // 
            this.lblShareDataWithPCPFlag.Location = new System.Drawing.Point(-1, 34);
            this.lblShareDataWithPCPFlag.Name = "lblShareDataWithPCPFlag";
            this.lblShareDataWithPCPFlag.Size = new System.Drawing.Size(135, 20);
            this.lblShareDataWithPCPFlag.TabIndex = 26;
            this.lblShareDataWithPCPFlag.Text = "Notify PCP of Visit?";
            // 
            // lblShareDataWithPublicHieFlag
            // 
            this.lblShareDataWithPublicHieFlag.Location = new System.Drawing.Point(-1, 3);
            this.lblShareDataWithPublicHieFlag.Name = "lblShareDataWithPublicHieFlag";
            this.lblShareDataWithPublicHieFlag.Size = new System.Drawing.Size(151, 50);
            this.lblShareDataWithPublicHieFlag.TabIndex = 27;
            this.lblShareDataWithPublicHieFlag.Text = "Share with Health\nInformation Exchange:";
            // 
            // cmbShareDataWithPCPFlag
            // 
            this.cmbShareDataWithPCPFlag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShareDataWithPCPFlag.Location = new System.Drawing.Point(156, 32);
            this.cmbShareDataWithPCPFlag.Name = "cmbShareDataWithPCPFlag";
            this.cmbShareDataWithPCPFlag.Size = new System.Drawing.Size(50, 21);
            this.cmbShareDataWithPCPFlag.TabIndex = 30;
            this.cmbShareDataWithPCPFlag.SelectedIndexChanged += new System.EventHandler(this.ShareDataWithPCP_SelectedIndexChanged);
            // 
            // cmbShareDataWithPublicHIEFlag
            // 
            this.cmbShareDataWithPublicHIEFlag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShareDataWithPublicHIEFlag.Location = new System.Drawing.Point(156, 1);
            this.cmbShareDataWithPublicHIEFlag.Name = "cmbShareDataWithPublicHIEFlag";
            this.cmbShareDataWithPublicHIEFlag.Size = new System.Drawing.Size(50, 21);
            this.cmbShareDataWithPublicHIEFlag.TabIndex = 28;
            this.cmbShareDataWithPublicHIEFlag.SelectedIndexChanged += new System.EventHandler(this.ShareDataWithPublicHIE_SelectedIndexChanged);
            // 
            // HIEShareDataFlagView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Controls.Add(this.cmbShareDataWithPCPFlag);
            this.Controls.Add(this.lblShareDataWithPCPFlag);
            this.Controls.Add(this.cmbShareDataWithPublicHIEFlag);
            this.Controls.Add(this.lblShareDataWithPublicHieFlag);
            this.Name = "HIEShareDataFlagView";
            this.Size = new System.Drawing.Size(222, 54);
            this.ResumeLayout(false);

        }

        #endregion

        private PatientAccessComboBox cmbShareDataWithPCPFlag;
        private System.Windows.Forms.Label lblShareDataWithPCPFlag;
        private PatientAccessComboBox cmbShareDataWithPublicHIEFlag;
        private System.Windows.Forms.Label lblShareDataWithPublicHieFlag;
    }
}
