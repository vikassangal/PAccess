namespace PatientAccess.UI.PreRegistrationViews
{
	partial class ShowDuplicatePreRegAccountsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShowDuplicatePreRegAccountsDialog));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblHeader2 = new System.Windows.Forms.Label();
            this.lblHeader3 = new System.Windows.Forms.Label();
            this.btnYes = new System.Windows.Forms.Button();
            this.btnNo = new System.Windows.Forms.Button();
            this.lvDuplicatePreregAccounts = new System.Windows.Forms.ListView();
            this.DummyColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.AdmitDateTimeColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.HospitalServiceColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.Clinic1ColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.AccountColumnHeader = new System.Windows.Forms.ColumnHeader();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(228)))), ((int)(((byte)(243)))));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(13, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // lblHeader
            // 
            this.lblHeader.Location = new System.Drawing.Point(60, 10);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(528, 40);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = resources.GetString("lblHeader.Text");
            // 
            // lblHeader2
            // 
            this.lblHeader2.Location = new System.Drawing.Point(60, 59);
            this.lblHeader2.Name = "lblHeader2";
            this.lblHeader2.Size = new System.Drawing.Size(193, 23);
            this.lblHeader2.TabIndex = 2;
            this.lblHeader2.Text = "Matching pre-registered accounts:";
            // 
            // lblHeader3
            // 
            this.lblHeader3.Location = new System.Drawing.Point(60, 188);
            this.lblHeader3.Name = "lblHeader3";
            this.lblHeader3.Size = new System.Drawing.Size(134, 23);
            this.lblHeader3.TabIndex = 3;
            this.lblHeader3.Text = "Do you wish to continue?";
            // 
            // btnYes
            // 
            this.btnYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnYes.Location = new System.Drawing.Point(222, 215);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(75, 23);
            this.btnYes.TabIndex = 0;
            this.btnYes.Text = "&Yes";
            this.btnYes.UseVisualStyleBackColor = true;
            // 
            // btnNo
            // 
            this.btnNo.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnNo.Location = new System.Drawing.Point(322, 215);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(75, 23);
            this.btnNo.TabIndex = 1;
            this.btnNo.Text = "&No";
            this.btnNo.UseVisualStyleBackColor = true;
            // 
            // lvDuplicatePreregAccounts
            // 
            this.lvDuplicatePreregAccounts.AutoArrange = false;
            this.lvDuplicatePreregAccounts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.DummyColumnHeader,
            this.AdmitDateTimeColumnHeader,
            this.HospitalServiceColumnHeader,
            this.Clinic1ColumnHeader,
            this.AccountColumnHeader});
            this.lvDuplicatePreregAccounts.FullRowSelect = true;
            this.lvDuplicatePreregAccounts.GridLines = true;
            this.lvDuplicatePreregAccounts.HideSelection = false;
            this.lvDuplicatePreregAccounts.Location = new System.Drawing.Point(29, 85);
            this.lvDuplicatePreregAccounts.MultiSelect = false;
            this.lvDuplicatePreregAccounts.Name = "lvDuplicatePreregAccounts";
            this.lvDuplicatePreregAccounts.Size = new System.Drawing.Size(560, 92);
            this.lvDuplicatePreregAccounts.TabIndex = 2;
            this.lvDuplicatePreregAccounts.UseCompatibleStateImageBehavior = false;
            this.lvDuplicatePreregAccounts.View = System.Windows.Forms.View.Details;
            // 
            // DummyColumnHeader
            // 
            this.DummyColumnHeader.Text = "Dummy";
            this.DummyColumnHeader.Width = 0;
            // 
            // AdmitDateTimeColumnHeader
            // 
            this.AdmitDateTimeColumnHeader.Text = "Admit Date/Time";
            this.AdmitDateTimeColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.AdmitDateTimeColumnHeader.Width = 160;
            // 
            // HospitalServiceColumnHeader
            // 
            this.HospitalServiceColumnHeader.Text = "Hospital Service";
            this.HospitalServiceColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.HospitalServiceColumnHeader.Width = 160;
            // 
            // Clinic1ColumnHeader
            // 
            this.Clinic1ColumnHeader.Text = "Clinic 1";
            this.Clinic1ColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Clinic1ColumnHeader.Width = 150;
            // 
            // AccountColumnHeader
            // 
            this.AccountColumnHeader.Text = "Account";
            this.AccountColumnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.AccountColumnHeader.Width = 88;
            // 
            // ShowDuplicatePreRegAccountsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(228)))), ((int)(((byte)(243)))));
            this.ClientSize = new System.Drawing.Size(619, 253);
            this.Controls.Add(this.lvDuplicatePreregAccounts);
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.btnYes);
            this.Controls.Add(this.lblHeader3);
            this.Controls.Add(this.lblHeader2);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ShowDuplicatePreRegAccountsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " Possible Duplicate Pre-Registration";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblHeader2;
        private System.Windows.Forms.Label lblHeader3;
        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.Button btnNo;
        private System.Windows.Forms.ListView lvDuplicatePreregAccounts;
        private System.Windows.Forms.ColumnHeader AdmitDateTimeColumnHeader;
        private System.Windows.Forms.ColumnHeader HospitalServiceColumnHeader;
        private System.Windows.Forms.ColumnHeader Clinic1ColumnHeader;
        private System.Windows.Forms.ColumnHeader AccountColumnHeader;
        private System.Windows.Forms.ColumnHeader DummyColumnHeader;
        
	}
}