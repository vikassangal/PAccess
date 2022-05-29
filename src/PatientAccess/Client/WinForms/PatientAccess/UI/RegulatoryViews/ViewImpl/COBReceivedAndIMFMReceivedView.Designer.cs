namespace PatientAccess.UI.RegulatoryViews.ViewImpl
{
    partial class COBReceivedAndIMFMReceivedView
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
            this.rbIMFMNo = new System.Windows.Forms.RadioButton();
            this.rbIMFMYes = new System.Windows.Forms.RadioButton();
            this.rbCOBNo = new System.Windows.Forms.RadioButton();
            this.rbCOBYes = new System.Windows.Forms.RadioButton();
            this.lblIMFMReceived = new System.Windows.Forms.Label();
            this.lblCOBReceived = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // rbIMFMNo
            // 
            this.rbIMFMNo.AutoSize = true;
            this.rbIMFMNo.Location = new System.Drawing.Point(179, 7);
            this.rbIMFMNo.Name = "rbIMFMNo";
            this.rbIMFMNo.Size = new System.Drawing.Size(39, 17);
            this.rbIMFMNo.TabIndex = 5;
            this.rbIMFMNo.TabStop = true;
            this.rbIMFMNo.Text = "No";
            this.rbIMFMNo.UseVisualStyleBackColor = true;
            this.rbIMFMNo.Click += new System.EventHandler(this.rbIMFMNo_Click);
            // 
            // rbIMFMYes
            // 
            this.rbIMFMYes.AutoSize = true;
            this.rbIMFMYes.Location = new System.Drawing.Point(137, 7);
            this.rbIMFMYes.Name = "rbIMFMYes";
            this.rbIMFMYes.Size = new System.Drawing.Size(43, 17);
            this.rbIMFMYes.TabIndex = 4;
            this.rbIMFMYes.TabStop = true;
            this.rbIMFMYes.Text = "Yes";
            this.rbIMFMYes.UseVisualStyleBackColor = true;
            this.rbIMFMYes.Click += new System.EventHandler(this.rbIMFMYes_Click);
            // 
            // rbCOBNo
            // 
            this.rbCOBNo.AutoSize = true;
            this.rbCOBNo.Location = new System.Drawing.Point(179, 7);
            this.rbCOBNo.Name = "rbCOBNo";
            this.rbCOBNo.Size = new System.Drawing.Size(39, 17);
            this.rbCOBNo.TabIndex = 3;
            this.rbCOBNo.TabStop = true;
            this.rbCOBNo.Text = "No";
            this.rbCOBNo.UseVisualStyleBackColor = true;
            this.rbCOBNo.Click += new System.EventHandler(this.rbCOBNo_Click);
            // 
            // rbCOBYes
            // 
            this.rbCOBYes.AutoSize = true;
            this.rbCOBYes.Location = new System.Drawing.Point(137, 7);
            this.rbCOBYes.Name = "rbCOBYes";
            this.rbCOBYes.Size = new System.Drawing.Size(43, 17);
            this.rbCOBYes.TabIndex = 2;
            this.rbCOBYes.TabStop = true;
            this.rbCOBYes.Text = "Yes";
            this.rbCOBYes.UseVisualStyleBackColor = true;
            this.rbCOBYes.Click += new System.EventHandler(this.rbCOBYes_Click);
            // 
            // lblIMFMReceived
            // 
            this.lblIMFMReceived.AutoSize = true;
            this.lblIMFMReceived.Location = new System.Drawing.Point(1, 7);
            this.lblIMFMReceived.Name = "lblIMFMReceived";
            this.lblIMFMReceived.Size = new System.Drawing.Size(92, 13);
            this.lblIMFMReceived.TabIndex = 1;
            this.lblIMFMReceived.Text = "IMFM Received?";
            // 
            // lblCOBReceived
            // 
            this.lblCOBReceived.AutoSize = true;
            this.lblCOBReceived.Location = new System.Drawing.Point(1, 7);
            this.lblCOBReceived.Name = "lblCOBReceived";
            this.lblCOBReceived.Size = new System.Drawing.Size(87, 13);
            this.lblCOBReceived.TabIndex = 0;
            this.lblCOBReceived.Text = "COB Received?";
            // 
            // COBReceivedAndIMFMReceivedView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rbIMFMNo);
            this.Controls.Add(this.rbIMFMYes);
            this.Controls.Add(this.rbCOBNo);
            this.Controls.Add(this.rbCOBYes);
            this.Controls.Add(this.lblIMFMReceived);
            this.Controls.Add(this.lblCOBReceived);
            this.Name = "COBReceivedAndIMFMReceivedView";
            this.Size = new System.Drawing.Size(284, 34);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCOBReceived;
        private System.Windows.Forms.Label lblIMFMReceived;
        private System.Windows.Forms.RadioButton rbCOBYes;
        private System.Windows.Forms.RadioButton rbCOBNo;
        private System.Windows.Forms.RadioButton rbIMFMYes;
        private System.Windows.Forms.RadioButton rbIMFMNo;
    }
}