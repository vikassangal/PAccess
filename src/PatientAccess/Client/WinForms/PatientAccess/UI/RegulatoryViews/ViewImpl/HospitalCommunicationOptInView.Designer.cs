using System.Windows.Forms;
namespace PatientAccess.UI.RegulatoryViews.ViewImpl
{
    partial class HospitalCommunicationOptInView
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
            this.lblHospitalCommunicationOptIn = new System.Windows.Forms.Label();
            this.rbYes = new System.Windows.Forms.RadioButton();
            this.rbNo = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // lblHospitalCommunicationOptIn
            // 
            this.lblHospitalCommunicationOptIn.AutoSize = true;
            this.lblHospitalCommunicationOptIn.Location = new System.Drawing.Point(3, 13);
            this.lblHospitalCommunicationOptIn.Name = "lblHospitalCommunicationOptIn";
            this.lblHospitalCommunicationOptIn.Size = new System.Drawing.Size(102, 13);
            this.lblHospitalCommunicationOptIn.TabIndex = 0;
            this.lblHospitalCommunicationOptIn.Text = "Hosp Comm Opt-In?";
            // 
            // rbYes
            // 
            this.rbYes.AutoSize = true;
            this.rbYes.Location = new System.Drawing.Point(140, 11);
            this.rbYes.Name = "rbYes";
            this.rbYes.Size = new System.Drawing.Size(43, 17);
            this.rbYes.TabIndex = 1;
            this.rbYes.TabStop = true;
            this.rbYes.Text = "Yes";
            this.rbYes.UseVisualStyleBackColor = true;
            this.rbYes.Click += new System.EventHandler(this.rbYes_Click);
            this.rbYes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rbYes_KeyDown);
            // 
            // rbNo
            // 
            this.rbNo.AutoSize = true;
            this.rbNo.Location = new System.Drawing.Point(181, 11);
            this.rbNo.Name = "rbNo";
            this.rbNo.Size = new System.Drawing.Size(39, 17);
            this.rbNo.TabIndex = 2;
            this.rbNo.TabStop = true;
            this.rbNo.Text = "No";
            this.rbNo.UseVisualStyleBackColor = true;
            this.rbNo.Click += new System.EventHandler(this.rbNo_Click);
            this.rbNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rbNo_KeyDown);
            // 
            // HospitalCommunicationOptInView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rbNo);
            this.Controls.Add(this.rbYes);
            this.Controls.Add(this.lblHospitalCommunicationOptIn);
            this.Name = "HospitalCommunicationOptInView";
            this.Size = new System.Drawing.Size(219, 37);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHospitalCommunicationOptIn;
        private System.Windows.Forms.RadioButton rbYes;
        private System.Windows.Forms.RadioButton rbNo;
    }
}
