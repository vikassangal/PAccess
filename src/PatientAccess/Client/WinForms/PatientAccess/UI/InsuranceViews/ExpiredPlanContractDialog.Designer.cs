namespace PatientAccess.UI.InsuranceViews
{
    partial class ExpiredPlanContractDialog
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
            this.yesButton = new System.Windows.Forms.Button();
            this.noButton = new System.Windows.Forms.Button();
            this.labelExpiredPlanContract = new System.Windows.Forms.Label();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.cancellationDateLabelValue = new System.Windows.Forms.Label();
            this.cancellationDateLabel = new System.Windows.Forms.Label();
            this.terminationDateLabelValue = new System.Windows.Forms.Label();
            this.terminationDateLabel = new System.Windows.Forms.Label();
            this.approvalDateLabelValue = new System.Windows.Forms.Label();
            this.approvalDateLabel = new System.Windows.Forms.Label();
            this.effectiveDateLabelValue = new System.Windows.Forms.Label();
            this.effectiveDateLabel = new System.Windows.Forms.Label();
            this.instructionLabel = new System.Windows.Forms.Label();
            this.planNameLabelValue = new System.Windows.Forms.Label();
            this.planNameLabel = new System.Windows.Forms.Label();
            this.lobLabelValue = new System.Windows.Forms.Label();
            this.lobLabel = new System.Windows.Forms.Label();
            this.typeLabelValue = new System.Windows.Forms.Label();
            this.typeLabel = new System.Windows.Forms.Label();
            this.planidLabelValue = new System.Windows.Forms.Label();
            this.planidLabel = new System.Windows.Forms.Label();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // yesButton
            // 
            this.yesButton.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.yesButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.yesButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.yesButton.Location = new System.Drawing.Point( 211, 188 );
            this.yesButton.Name = "yesButton";
            this.yesButton.Size = new System.Drawing.Size( 75, 23 );
            this.yesButton.TabIndex = 1;
            this.yesButton.Text = "&Yes";
            this.yesButton.UseVisualStyleBackColor = false;
            // 
            // noButton
            // 
            this.noButton.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.noButton.DialogResult = System.Windows.Forms.DialogResult.No;
            this.noButton.Location = new System.Drawing.Point( 300, 188 );
            this.noButton.Name = "noButton";
            this.noButton.Size = new System.Drawing.Size( 75, 23 );
            this.noButton.TabIndex = 2;
            this.noButton.Text = "&No";
            this.noButton.UseVisualStyleBackColor = false;
            // 
            // labelExpiredPlanContract
            // 
            this.labelExpiredPlanContract.AutoSize = true;
            this.labelExpiredPlanContract.BackColor = System.Drawing.Color.White;
            this.labelExpiredPlanContract.Location = new System.Drawing.Point( 23, 163 );
            this.labelExpiredPlanContract.Name = "labelExpiredPlanContract";
            this.labelExpiredPlanContract.Size = new System.Drawing.Size( 278, 13 );
            this.labelExpiredPlanContract.TabIndex = 3;
            this.labelExpiredPlanContract.Text = "Would you like to continue with the selection of this plan?";
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.White;
            this.mainPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainPanel.Controls.Add( this.cancellationDateLabelValue );
            this.mainPanel.Controls.Add( this.cancellationDateLabel );
            this.mainPanel.Controls.Add( this.terminationDateLabelValue );
            this.mainPanel.Controls.Add( this.terminationDateLabel );
            this.mainPanel.Controls.Add( this.approvalDateLabelValue );
            this.mainPanel.Controls.Add( this.approvalDateLabel );
            this.mainPanel.Controls.Add( this.effectiveDateLabelValue );
            this.mainPanel.Controls.Add( this.effectiveDateLabel );
            this.mainPanel.Controls.Add( this.instructionLabel );
            this.mainPanel.Controls.Add( this.planNameLabelValue );
            this.mainPanel.Controls.Add( this.planNameLabel );
            this.mainPanel.Controls.Add( this.lobLabelValue );
            this.mainPanel.Controls.Add( this.lobLabel );
            this.mainPanel.Controls.Add( this.typeLabelValue );
            this.mainPanel.Controls.Add( this.typeLabel );
            this.mainPanel.Controls.Add( this.planidLabelValue );
            this.mainPanel.Controls.Add( this.planidLabel );
            this.mainPanel.Controls.Add( this.labelExpiredPlanContract );
            this.mainPanel.Controls.Add( this.noButton );
            this.mainPanel.Controls.Add( this.yesButton );
            this.mainPanel.Location = new System.Drawing.Point( 12, 12 );
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size( 396, 221 );
            this.mainPanel.TabIndex = 4;
            // 
            // cancellationDateLabelValue
            // 
            this.cancellationDateLabelValue.Location = new System.Drawing.Point( 130, 138 );
            this.cancellationDateLabelValue.Name = "cancellationDateLabelValue";
            this.cancellationDateLabelValue.Size = new System.Drawing.Size( 93, 13 );
            this.cancellationDateLabelValue.TabIndex = 20;
            // 
            // cancellationDateLabel
            // 
            this.cancellationDateLabel.AutoSize = true;
            this.cancellationDateLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.cancellationDateLabel.Location = new System.Drawing.Point( 23, 138 );
            this.cancellationDateLabel.Name = "cancellationDateLabel";
            this.cancellationDateLabel.Size = new System.Drawing.Size( 112, 13 );
            this.cancellationDateLabel.TabIndex = 19;
            this.cancellationDateLabel.Text = "Cancellation Date:";
            // 
            // terminationDateLabelValue
            // 
            this.terminationDateLabelValue.Location = new System.Drawing.Point( 126, 123 );
            this.terminationDateLabelValue.Name = "terminationDateLabelValue";
            this.terminationDateLabelValue.Size = new System.Drawing.Size( 93, 13 );
            this.terminationDateLabelValue.TabIndex = 18;
            // 
            // terminationDateLabel
            // 
            this.terminationDateLabel.AutoSize = true;
            this.terminationDateLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.terminationDateLabel.Location = new System.Drawing.Point( 23, 123 );
            this.terminationDateLabel.Name = "terminationDateLabel";
            this.terminationDateLabel.Size = new System.Drawing.Size( 108, 13 );
            this.terminationDateLabel.TabIndex = 17;
            this.terminationDateLabel.Text = "Termination Date:";
            // 
            // approvalDateLabelValue
            // 
            this.approvalDateLabelValue.Location = new System.Drawing.Point( 110, 109 );
            this.approvalDateLabelValue.Name = "approvalDateLabelValue";
            this.approvalDateLabelValue.Size = new System.Drawing.Size( 93, 13 );
            this.approvalDateLabelValue.TabIndex = 16;
            // 
            // approvalDateLabel
            // 
            this.approvalDateLabel.AutoSize = true;
            this.approvalDateLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.approvalDateLabel.Location = new System.Drawing.Point( 23, 109 );
            this.approvalDateLabel.Name = "approvalDateLabel";
            this.approvalDateLabel.Size = new System.Drawing.Size( 92, 13 );
            this.approvalDateLabel.TabIndex = 15;
            this.approvalDateLabel.Text = "Approval Date:";
            // 
            // effectiveDateLabelValue
            // 
            this.effectiveDateLabelValue.Location = new System.Drawing.Point( 111, 95 );
            this.effectiveDateLabelValue.Name = "effectiveDateLabelValue";
            this.effectiveDateLabelValue.Size = new System.Drawing.Size( 93, 13 );
            this.effectiveDateLabelValue.TabIndex = 14;
            // 
            // effectiveDateLabel
            // 
            this.effectiveDateLabel.AutoSize = true;
            this.effectiveDateLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.effectiveDateLabel.Location = new System.Drawing.Point( 23, 95 );
            this.effectiveDateLabel.Name = "effectiveDateLabel";
            this.effectiveDateLabel.Size = new System.Drawing.Size( 93, 13 );
            this.effectiveDateLabel.TabIndex = 13;
            this.effectiveDateLabel.Text = "Effective Date:";
            // 
            // instructionLabel
            // 
            this.instructionLabel.Location = new System.Drawing.Point( 23, 12 );
            this.instructionLabel.Name = "instructionLabel";
            this.instructionLabel.Size = new System.Drawing.Size( 348, 26 );
            this.instructionLabel.TabIndex = 4;
            this.instructionLabel.Text = "Please note that the contract you have selected for this plan is expired, though " +
                "it is still selectable according to your facility\'s grace period.";
            // 
            // planNameLabelValue
            // 
            this.planNameLabelValue.Location = new System.Drawing.Point( 90, 68 );
            this.planNameLabelValue.Name = "planNameLabelValue";
            this.planNameLabelValue.Size = new System.Drawing.Size( 278, 13 );
            this.planNameLabelValue.TabIndex = 12;
            // 
            // planNameLabel
            // 
            this.planNameLabel.AutoSize = true;
            this.planNameLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.planNameLabel.Location = new System.Drawing.Point( 23, 68 );
            this.planNameLabel.Name = "planNameLabel";
            this.planNameLabel.Size = new System.Drawing.Size( 72, 13 );
            this.planNameLabel.TabIndex = 11;
            this.planNameLabel.Text = "Plan Name:";
            // 
            // lobLabelValue
            // 
            this.lobLabelValue.Location = new System.Drawing.Point( 294, 50 );
            this.lobLabelValue.Name = "lobLabelValue";
            this.lobLabelValue.Size = new System.Drawing.Size( 74, 13 );
            this.lobLabelValue.TabIndex = 10;
            // 
            // lobLabel
            // 
            this.lobLabel.AutoSize = true;
            this.lobLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.lobLabel.Location = new System.Drawing.Point( 264, 50 );
            this.lobLabel.Name = "lobLabel";
            this.lobLabel.Size = new System.Drawing.Size( 35, 13 );
            this.lobLabel.TabIndex = 9;
            this.lobLabel.Text = "LOB:";
            // 
            // typeLabelValue
            // 
            this.typeLabelValue.Location = new System.Drawing.Point( 186, 50 );
            this.typeLabelValue.Name = "typeLabelValue";
            this.typeLabelValue.Size = new System.Drawing.Size( 70, 13 );
            this.typeLabelValue.TabIndex = 8;
            // 
            // typeLabel
            // 
            this.typeLabel.AutoSize = true;
            this.typeLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.typeLabel.Location = new System.Drawing.Point( 151, 50 );
            this.typeLabel.Name = "typeLabel";
            this.typeLabel.Size = new System.Drawing.Size( 39, 13 );
            this.typeLabel.TabIndex = 7;
            this.typeLabel.Text = "Type:";
            // 
            // planidLabelValue
            // 
            this.planidLabelValue.Location = new System.Drawing.Point( 71, 50 );
            this.planidLabelValue.Name = "planidLabelValue";
            this.planidLabelValue.Size = new System.Drawing.Size( 61, 13 );
            this.planidLabelValue.TabIndex = 6;
            // 
            // planidLabel
            // 
            this.planidLabel.AutoSize = true;
            this.planidLabel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.planidLabel.Location = new System.Drawing.Point( 23, 50 );
            this.planidLabel.Name = "planidLabel";
            this.planidLabel.Size = new System.Drawing.Size( 53, 13 );
            this.planidLabel.TabIndex = 5;
            this.planidLabel.Text = "Plan ID:";
            // 
            // ExpiredPlanContractDialog
            // 
            this.AccessibleName = "";
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.ClientSize = new System.Drawing.Size( 422, 245 );
            this.Controls.Add( this.mainPanel );
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExpiredPlanContractDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Expired Plan Contract";
            this.mainPanel.ResumeLayout( false );
            this.mainPanel.PerformLayout();
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.Button yesButton;
        private System.Windows.Forms.Button noButton;
        private System.Windows.Forms.Label labelExpiredPlanContract;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Label planNameLabelValue;
        private System.Windows.Forms.Label planNameLabel;
        private System.Windows.Forms.Label lobLabelValue;
        private System.Windows.Forms.Label lobLabel;
        private System.Windows.Forms.Label typeLabelValue;
        private System.Windows.Forms.Label typeLabel;
        private System.Windows.Forms.Label planidLabelValue;
        private System.Windows.Forms.Label planidLabel;
        private System.Windows.Forms.Label instructionLabel;
        private System.Windows.Forms.Label effectiveDateLabel;
        private System.Windows.Forms.Label terminationDateLabelValue;
        private System.Windows.Forms.Label terminationDateLabel;
        private System.Windows.Forms.Label approvalDateLabelValue;
        private System.Windows.Forms.Label approvalDateLabel;
        private System.Windows.Forms.Label effectiveDateLabelValue;
        private System.Windows.Forms.Label cancellationDateLabelValue;
        private System.Windows.Forms.Label cancellationDateLabel;

    }
}