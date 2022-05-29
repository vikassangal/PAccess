using System.Windows.Forms;

namespace PatientAccess.UI.RegulatoryViews.ViewImpl
{
    partial class PatientPortalOptInView
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblPatientPortalOptIn = new System.Windows.Forms.Label();
            this.rbYes = new PatientAccess.UI.CommonControls.RadioButtonKeyHandler();
            rbNo = new PatientAccess.UI.CommonControls.RadioButtonKeyHandler();
            this.rbUnable = new PatientAccess.UI.CommonControls.RadioButtonKeyHandler();
            this.SuspendLayout();
            // 
            // lblPatientPortalOptIn
            // 
            this.lblPatientPortalOptIn.Location = new System.Drawing.Point(-1, 17);
            this.lblPatientPortalOptIn.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblPatientPortalOptIn.Name = "lblPatientPortalOptIn";
            this.lblPatientPortalOptIn.Size = new System.Drawing.Size(310, 65);
            this.lblPatientPortalOptIn.TabIndex = 15;
            this.lblPatientPortalOptIn.Text = "Patient Opt-In to\nPatient Portal?";
            // 
            // rbYes
            // 
            this.rbYes.Location = new System.Drawing.Point(362, 33);
            this.rbYes.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.rbYes.Name = "rbYes";
            this.rbYes.Size = new System.Drawing.Size(144, 36);
            this.rbYes.TabIndex = 1;
            this.rbYes.TabStop = true;
            this.rbYes.Text = "Yes";
            this.rbYes.Click += new System.EventHandler(this.rbYes_Click);
            this.rbYes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rbYes_KeyDown);
            // 
            // rbNo
            // 
            rbNo.Location = new System.Drawing.Point(469, 33);
            rbNo.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            rbNo.Name = "rbNo";
            rbNo.Size = new System.Drawing.Size(101, 36);
            rbNo.TabIndex = 2;
            rbNo.TabStop = true;
            rbNo.Text = "No";
            rbNo.Click += new System.EventHandler(this.rbNo_Click);
            rbNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rbNo_KeyDown);
            // 
            // rbUnable
            // 
            rbUnable.Location = new System.Drawing.Point(571, 33);
            rbUnable.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            rbUnable.Name = "rbUnable";
            rbUnable.Size = new System.Drawing.Size(132, 36);
            rbUnable.TabIndex = 3;
            rbUnable.TabStop = true;
            rbUnable.Text = "Unable";
            rbUnable.Click += new System.EventHandler(this.rbUnable_Click);
            rbUnable.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rbUnable_KeyDown);
            
            // 
            // PatientPortalOptInView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblPatientPortalOptIn);
            this.Controls.Add(rbNo);
            this.Controls.Add(this.rbYes);
            this.Controls.Add(rbUnable);
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "PatientPortalOptInView";
            this.Size = new System.Drawing.Size(757, 125);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblPatientPortalOptIn;
        private global::PatientAccess.UI.CommonControls.RadioButtonKeyHandler rbYes;
        private global::PatientAccess.UI.CommonControls.RadioButtonKeyHandler rbNo;
        private global::PatientAccess.UI.CommonControls.RadioButtonKeyHandler rbUnable;

        
    }
}