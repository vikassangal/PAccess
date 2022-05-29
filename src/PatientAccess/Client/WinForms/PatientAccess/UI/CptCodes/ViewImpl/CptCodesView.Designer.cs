using System.Windows.Forms;

namespace PatientAccess.UI.CptCodes.ViewImpl
{
    partial class CptCodesView
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
            this.lblCptCodes = new System.Windows.Forms.Label();
            this.btnView = new System.Windows.Forms.Button();
            this.rbNo = new PatientAccess.UI.CommonControls.RadioButtonKeyHandler();
            this.rbYes = new PatientAccess.UI.CommonControls.RadioButtonKeyHandler();
            this.SuspendLayout();
            // 
            // lblCptCodes
            // 
            this.lblCptCodes.Location = new System.Drawing.Point(3, 3);
            this.lblCptCodes.Name = "lblCptCodes";
            this.lblCptCodes.Size = new System.Drawing.Size(64, 17);
            this.lblCptCodes.TabIndex = 0;
            this.lblCptCodes.TabStop = false;
            this.lblCptCodes.Text = "CPT Codes:";
            // 
            // btnView
            // 
            this.btnView.Location = new System.Drawing.Point(156, 0);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(40, 23);
            this.btnView.TabIndex = 3;
            this.btnView.Text = "View";
            this.btnView.UseVisualStyleBackColor = true;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // rbNo
            // 
            this.rbNo.AutoCheck = false;
            this.rbNo.Location = new System.Drawing.Point(117, 2);
            this.rbNo.Name = "rbNo";
            this.rbNo.Size = new System.Drawing.Size(44, 15);
            this.rbNo.TabIndex = 2;
            this.rbNo.Text = "No";
            this.rbNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rbNo_KeyDown);
            this.rbNo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rbNo_MouseDown);
            // 
            // rbYes
            // 
            this.rbYes.AutoCheck = false;
            this.rbYes.Location = new System.Drawing.Point(70, 2);
            this.rbYes.Name = "rbYes";
            this.rbYes.Size = new System.Drawing.Size(44, 15);
            this.rbYes.TabIndex = 1;
            this.rbYes.Text = "Yes";
            this.rbYes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rbYes_KeyDown);
            this.rbYes.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rbYes_MouseDown);
            // 
            // CptCodesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnView);
            this.Controls.Add(this.lblCptCodes);
            this.Controls.Add(this.rbNo);
            this.Controls.Add(this.rbYes);
            this.Name = "CptCodesView";
            this.Size = new System.Drawing.Size(200, 25);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblCptCodes;
        private global::PatientAccess.UI.CommonControls.RadioButtonKeyHandler rbYes;
        private global::PatientAccess.UI.CommonControls.RadioButtonKeyHandler rbNo;
        private Button btnView;
    }
}