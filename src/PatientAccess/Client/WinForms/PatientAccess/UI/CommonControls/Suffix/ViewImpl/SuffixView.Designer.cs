using System.Windows.Forms;

namespace PatientAccess.UI.CommonControls.Suffix.ViewImpl
{
    partial class SuffixView
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
            this.lblSuffix = new System.Windows.Forms.Label();
            this.cmbSuffix = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.SuspendLayout();
            // 
            // lblSuffix
            // 
            this.lblSuffix.Location = new System.Drawing.Point(1, 7);
            this.lblSuffix.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSuffix.Name = "lblSuffix";
            this.lblSuffix.Size = new System.Drawing.Size(55, 23);
            this.lblSuffix.TabIndex = 1;
            this.lblSuffix.Text = "Suffix:";
            // 
            // cmbSuffix
            // 
            this.cmbSuffix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSuffix.Location = new System.Drawing.Point(60, 1);
            this.cmbSuffix.Margin = new System.Windows.Forms.Padding(2);
            this.cmbSuffix.Name = "cmbSuffix";
            this.cmbSuffix.Size = new System.Drawing.Size(58, 28);
            this.cmbSuffix.TabIndex = 13;
            this.cmbSuffix.SelectedIndexChanged += new System.EventHandler(this.cmbSuffix_SelectedIndexChanged);
            // 
            // SuffixView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmbSuffix);
            this.Controls.Add(this.lblSuffix);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "SuffixView";
            this.Size = new System.Drawing.Size(130, 40);
            this.ResumeLayout(false);

        }

        #endregion

        private Label lblSuffix;
        private PatientAccessComboBox cmbSuffix;

    }
}