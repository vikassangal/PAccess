using System.Windows.Forms;


namespace PatientAccess.UI.DiagnosisViews.Specialized
{
    partial class ClinicalTrialsBoardDiagnosisView
    {

		#region Fields 

        private System.Windows.Forms.Label _clinicalTrialsBoardFlagValueLabel;
        private System.Windows.Forms.Label _clinicalTrialsBoardLabel;
        private System.Windows.Forms.Panel _clinicalTrialsBoardPanel;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

		#endregion Fields 

		#region Properties 

        /// <summary>
        /// Gets the clinical trials board flag value label.
        /// </summary>
        /// <value>The clinical trials board flag value label.</value>
        public Label ClinicalTrialsBoardFlagValueLabel
        {
            get
            {
                return this._clinicalTrialsBoardFlagValueLabel;
            }
        }

        /// <summary>
        /// Gets the clinical trials board label.
        /// </summary>
        /// <value>The clinical trials board label.</value>
        public Label ClinicalTrialsBoardLabel
        {
            get
            {
                return this._clinicalTrialsBoardLabel;
            }
        }

        /// <summary>
        /// Gets the clinical trials board panel.
        /// </summary>
        /// <value>The clinical trials board panel.</value>
        public Panel ClinicalTrialsBoardPanel
        {
            get
            {
                return this._clinicalTrialsBoardPanel;
            }
        }

		#endregion Properties 

		#region Methods 

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if( disposing && ( this.components != null ) )
            {
                this.components.Dispose();
            }
            base.Dispose( disposing );
        }

		#endregion Methods 

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._clinicalTrialsBoardPanel = new System.Windows.Forms.Panel();
            this._clinicalTrialsBoardFlagValueLabel = new System.Windows.Forms.Label();
            this._clinicalTrialsBoardLabel = new System.Windows.Forms.Label();
            this._clinicalTrialsBoardPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _clinicalTrialsBoardPanel
            // 
            this._clinicalTrialsBoardPanel.Controls.Add(this._clinicalTrialsBoardFlagValueLabel);
            this._clinicalTrialsBoardPanel.Controls.Add(this._clinicalTrialsBoardLabel);
            this._clinicalTrialsBoardPanel.Location = new System.Drawing.Point(362, 364);
            this._clinicalTrialsBoardPanel.Name = "_clinicalTrialsBoardPanel";
            this._clinicalTrialsBoardPanel.Size = new System.Drawing.Size(307, 18);
            this._clinicalTrialsBoardPanel.TabIndex = 24;
            this._clinicalTrialsBoardPanel.Visible = false;
            // 
            // _clinicalTrialsBoardFlagValueLabel
            // 
            this._clinicalTrialsBoardFlagValueLabel.AutoSize = true;
            this._clinicalTrialsBoardFlagValueLabel.Location = new System.Drawing.Point(90, 3);
            this._clinicalTrialsBoardFlagValueLabel.Name = "_clinicalTrialsBoardFlagValueLabel";
            this._clinicalTrialsBoardFlagValueLabel.Size = new System.Drawing.Size(44, 13);
            this._clinicalTrialsBoardFlagValueLabel.TabIndex = 1;
            this._clinicalTrialsBoardFlagValueLabel.Text = "Yes/No";
            // 
            // _clinicalTrialsBoardLabel
            // 
            this._clinicalTrialsBoardLabel.AutoSize = true;
            this._clinicalTrialsBoardLabel.Location = new System.Drawing.Point(1, 3);
            this._clinicalTrialsBoardLabel.Name = "_clinicalTrialsBoardLabel";
            this._clinicalTrialsBoardLabel.Size = new System.Drawing.Size(62, 13);
            this._clinicalTrialsBoardLabel.TabIndex = 0;
            this._clinicalTrialsBoardLabel.Text = "Clinical trial:";
            // 
            // ClinicalTrialsBoardDiagnosisView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._clinicalTrialsBoardPanel);
            this.Name = "ClinicalTrialsBoardDiagnosisView";
            this.Size = new System.Drawing.Size(983, 401);
            this.Controls.SetChildIndex(this._clinicalTrialsBoardPanel, 0);
            this._clinicalTrialsBoardPanel.ResumeLayout(false);
            this._clinicalTrialsBoardPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

    }
}