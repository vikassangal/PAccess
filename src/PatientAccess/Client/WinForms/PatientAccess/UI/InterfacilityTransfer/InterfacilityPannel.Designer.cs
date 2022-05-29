namespace PatientAccess.UI.InterfacilityTransfer
{
    partial class InterfacilityPannel
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
            this.pnl_ift = new System.Windows.Forms.Panel();
            this.lbl_HSVView = new System.Windows.Forms.Label();
            this.lbl_PTView = new System.Windows.Forms.Label();
            this.lbl_PT = new System.Windows.Forms.Label();
            this.lbl_HSV = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmb_ift_hospital = new System.Windows.Forms.ComboBox();
            this.cmb_ift_account = new System.Windows.Forms.ComboBox();
            this.lbl_ift_hospital_view = new System.Windows.Forms.Label();
            this.lbl_ift_account_view = new System.Windows.Forms.Label();
            this.lbl_ift_hospital = new System.Windows.Forms.Label();
            this.lbl_ift_account = new System.Windows.Forms.Label();
            this.lblNote = new System.Windows.Forms.Label();
            this.pnl_ift.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnl_ift
            // 
            this.pnl_ift.Controls.Add(this.lbl_HSVView);
            this.pnl_ift.Controls.Add(this.lbl_PTView);
            this.pnl_ift.Controls.Add(this.lbl_PT);
            this.pnl_ift.Controls.Add(this.lbl_HSV);
            this.pnl_ift.Controls.Add(this.label1);
            this.pnl_ift.Controls.Add(this.cmb_ift_hospital);
            this.pnl_ift.Controls.Add(this.cmb_ift_account);
            this.pnl_ift.Controls.Add(this.lbl_ift_hospital_view);
            this.pnl_ift.Controls.Add(this.lbl_ift_account_view);
            this.pnl_ift.Controls.Add(this.lbl_ift_hospital);
            this.pnl_ift.Controls.Add(this.lbl_ift_account);
            this.pnl_ift.Location = new System.Drawing.Point(21, 2);
            this.pnl_ift.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnl_ift.Name = "pnl_ift";
            this.pnl_ift.Size = new System.Drawing.Size(447, 238);
            this.pnl_ift.TabIndex = 10;
            // 
            // lbl_HSVView
            // 
            this.lbl_HSVView.AutoSize = true;
            this.lbl_HSVView.Location = new System.Drawing.Point(157, 167);
            this.lbl_HSVView.Name = "lbl_HSVView";
            this.lbl_HSVView.Size = new System.Drawing.Size(54, 17);
            this.lbl_HSVView.TabIndex = 28;
            this.lbl_HSVView.Text = "label10";
            // 
            // lbl_PTView
            // 
            this.lbl_PTView.AutoSize = true;
            this.lbl_PTView.Location = new System.Drawing.Point(157, 135);
            this.lbl_PTView.Name = "lbl_PTView";
            this.lbl_PTView.Size = new System.Drawing.Size(54, 17);
            this.lbl_PTView.TabIndex = 27;
            this.lbl_PTView.Text = "label10";
            // 
            // lbl_PT
            // 
            this.lbl_PT.AutoSize = true;
            this.lbl_PT.Location = new System.Drawing.Point(39, 135);
            this.lbl_PT.Name = "lbl_PT";
            this.lbl_PT.Size = new System.Drawing.Size(92, 17);
            this.lbl_PT.TabIndex = 26;
            this.lbl_PT.Text = "Patient Type:";
            // 
            // lbl_HSV
            // 
            this.lbl_HSV.AutoSize = true;
            this.lbl_HSV.Location = new System.Drawing.Point(37, 167);
            this.lbl_HSV.Name = "lbl_HSV";
            this.lbl_HSV.Size = new System.Drawing.Size(114, 17);
            this.lbl_HSV.TabIndex = 25;
            this.lbl_HSV.Text = "Hospital Service:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(37, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(245, 25);
            this.label1.TabIndex = 23;
            this.label1.Text = "Interfacility Transfer Details";
            // 
            // cmb_ift_hospital
            // 
            this.cmb_ift_hospital.FormattingEnabled = true;
            this.cmb_ift_hospital.Location = new System.Drawing.Point(160, 71);
            this.cmb_ift_hospital.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmb_ift_hospital.Name = "cmb_ift_hospital";
            this.cmb_ift_hospital.Size = new System.Drawing.Size(185, 24);
            this.cmb_ift_hospital.TabIndex = 22;
            this.cmb_ift_hospital.Visible = false;
            this.cmb_ift_hospital.SelectedIndexChanged += new System.EventHandler(this.cmb_ift_hospital_SelectedIndexChanged);
            // 
            // cmb_ift_account
            // 
            this.cmb_ift_account.FormattingEnabled = true;
            this.cmb_ift_account.Location = new System.Drawing.Point(160, 104);
            this.cmb_ift_account.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmb_ift_account.Name = "cmb_ift_account";
            this.cmb_ift_account.Size = new System.Drawing.Size(185, 24);
            this.cmb_ift_account.TabIndex = 21;
            this.cmb_ift_account.Visible = false;
            // 
            // lbl_ift_hospital_view
            // 
            this.lbl_ift_hospital_view.AutoSize = true;
            this.lbl_ift_hospital_view.Location = new System.Drawing.Point(157, 71);
            this.lbl_ift_hospital_view.Name = "lbl_ift_hospital_view";
            this.lbl_ift_hospital_view.Size = new System.Drawing.Size(54, 17);
            this.lbl_ift_hospital_view.TabIndex = 20;
            this.lbl_ift_hospital_view.Text = "label11";
            // 
            // lbl_ift_account_view
            // 
            this.lbl_ift_account_view.AutoSize = true;
            this.lbl_ift_account_view.Location = new System.Drawing.Point(157, 107);
            this.lbl_ift_account_view.Name = "lbl_ift_account_view";
            this.lbl_ift_account_view.Size = new System.Drawing.Size(54, 17);
            this.lbl_ift_account_view.TabIndex = 19;
            this.lbl_ift_account_view.Text = "label10";
            // 
            // lbl_ift_hospital
            // 
            this.lbl_ift_hospital.AutoSize = true;
            this.lbl_ift_hospital.Location = new System.Drawing.Point(37, 71);
            this.lbl_ift_hospital.Name = "lbl_ift_hospital";
            this.lbl_ift_hospital.Size = new System.Drawing.Size(84, 17);
            this.lbl_ift_hospital.TabIndex = 17;
            this.lbl_ift_hospital.Text = "To Hospital:";
            // 
            // lbl_ift_account
            // 
            this.lbl_ift_account.AutoSize = true;
            this.lbl_ift_account.Location = new System.Drawing.Point(39, 104);
            this.lbl_ift_account.Name = "lbl_ift_account";
            this.lbl_ift_account.Size = new System.Drawing.Size(84, 17);
            this.lbl_ift_account.TabIndex = 16;
            this.lbl_ift_account.Text = "To Account:";
            // 
            // lblNote
            // 
            this.lblNote.AutoSize = true;
            this.lblNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNote.ForeColor = System.Drawing.Color.Red;
            this.lblNote.Location = new System.Drawing.Point(25, 198);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(0, 25);
            this.lblNote.TabIndex = 11;
            // 
            // InterfacilityPannel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.pnl_ift);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "InterfacilityPannel";
            this.Size = new System.Drawing.Size(471, 244);
            this.pnl_ift.ResumeLayout(false);
            this.pnl_ift.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnl_ift;
        private System.Windows.Forms.ComboBox cmb_ift_hospital;
        private System.Windows.Forms.ComboBox cmb_ift_account;
        private System.Windows.Forms.Label lbl_ift_hospital_view;
        private System.Windows.Forms.Label lbl_ift_account_view;
        private System.Windows.Forms.Label lbl_ift_hospital;
        private System.Windows.Forms.Label lbl_ift_account;
        private System.Windows.Forms.Label lblNote;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_HSVView;
        private System.Windows.Forms.Label lbl_PTView;
        private System.Windows.Forms.Label lbl_PT;
        private System.Windows.Forms.Label lbl_HSV;
    }
}
