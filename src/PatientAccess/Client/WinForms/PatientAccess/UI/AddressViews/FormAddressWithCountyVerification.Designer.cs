namespace PatientAccess.UI.AddressViews
{
    partial class FormAddressWithCountyVerification
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
            this.addressEntryView1 = new PatientAccess.UI.AddressViews.AddressEntryWithCountyView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.matchingAddressView1 = new PatientAccess.UI.AddressViews.MatchingAddressView();
            this.lblMatching = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.cbxIgnore = new System.Windows.Forms.CheckBox();
            this.editAddressView1 = new PatientAccess.UI.AddressViews.EditAddressView();
            this.btnOk = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnCancelEdit = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // addressEntryView1
            // 
            this.addressEntryView1.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.addressEntryView1.IgnoreChecked = false;
            this.addressEntryView1.Location = new System.Drawing.Point(0, 0);
            this.addressEntryView1.Model = null;
            this.addressEntryView1.Name = "addressEntryView1";
            this.addressEntryView1.Size = new System.Drawing.Size(466, 245);
            this.addressEntryView1.TabIndex = 1;
            this.addressEntryView1.ResetMatchingAddresses += new System.EventHandler(this.addressEntryView1_ResetMatchingAddresses);
            this.addressEntryView1.DataModified += new System.EventHandler(this.OnDataModification);
            this.addressEntryView1.SetFormToOriginalSize += new System.EventHandler(this.addressEntryView1_SetFormToOriginalSize);
            this.addressEntryView1.AddressEntryCancelled += new System.EventHandler(this.OnAddressEntryCancelled);
            this.addressEntryView1.VerificationButtonEnabled += new System.EventHandler(this.addressEntryView1_VerificationButtonEnabled);
            this.addressEntryView1.VerifyAddress += new System.EventHandler(this.OnAddressVerification);
            this.addressEntryView1.NonUSAddress += new System.EventHandler(this.OnNonUSAddress);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.addressEntryView1);
            this.panel1.Location = new System.Drawing.Point(6, 9);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(467, 250);
            this.panel1.TabIndex = 0;
            // 
            // matchingAddressView1
            // 
            this.matchingAddressView1.CoverMessage = "No addresses match your entry.  Use the address entered (if complete), or enter a" +
            " new address to verify.";
            this.matchingAddressView1.CoverPadding = 30;
            this.matchingAddressView1.Ignoring = false;
            this.matchingAddressView1.Location = new System.Drawing.Point(7, 37);
            this.matchingAddressView1.Model = null;
            this.matchingAddressView1.Name = "matchingAddressView1";
            this.matchingAddressView1.ShowCover = true;
            this.matchingAddressView1.Size = new System.Drawing.Size(452, 97);
            this.matchingAddressView1.TabIndex = 1;
            this.matchingAddressView1.AddressSelected += new System.EventHandler(this.OnAddressSelected);
            // 
            // lblMatching
            // 
            this.lblMatching.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMatching.Location = new System.Drawing.Point(6, 5);
            this.lblMatching.Name = "lblMatching";
            this.lblMatching.Size = new System.Drawing.Size(156, 14);
            this.lblMatching.TabIndex = 1;
            this.lblMatching.Text = "Matching USPS Addresses";
            // 
            // lblMessage
            // 
            this.lblMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblMessage.Location = new System.Drawing.Point(16, 22);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(437, 13);
            this.lblMessage.TabIndex = 2;
            // 
            // cbxIgnore
            // 
            this.cbxIgnore.Location = new System.Drawing.Point(7, 137);
            this.cbxIgnore.Name = "cbxIgnore";
            this.cbxIgnore.Size = new System.Drawing.Size(425, 16);
            this.cbxIgnore.TabIndex = 2;
            this.cbxIgnore.Text = "Ignore results and use the address entered (all fields required)";
            this.cbxIgnore.Click += new System.EventHandler(this.cbxIgnore_Click);
            // 
            // editAddressView1
            // 
            this.editAddressView1.BackColor = System.Drawing.Color.White;
            this.editAddressView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editAddressView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.editAddressView1.Location = new System.Drawing.Point(0, 0);
            this.editAddressView1.Model = null;
            this.editAddressView1.Name = "editAddressView1";
            this.editAddressView1.Size = new System.Drawing.Size(465, 164);
            this.editAddressView1.TabIndex = 1;
            this.editAddressView1.validCharacterLimit = false;
            this.editAddressView1.EnableOKButton += new System.EventHandler(this.editAddressView1_EnableOKButton);
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.Control;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(314, 603);
            this.btnOk.Message = null;
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 21);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "OK";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancelEdit
            // 
            this.btnCancelEdit.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancelEdit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelEdit.Location = new System.Drawing.Point(394, 603);
            this.btnCancelEdit.Message = null;
            this.btnCancelEdit.Name = "btnCancelEdit";
            this.btnCancelEdit.Size = new System.Drawing.Size(75, 21);
            this.btnCancelEdit.TabIndex = 4;
            this.btnCancelEdit.Text = "Cancel";
            this.btnCancelEdit.Click += new System.EventHandler(this.btnCancelEdit_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.cbxIgnore);
            this.panel2.Controls.Add(this.lblMessage);
            this.panel2.Controls.Add(this.lblMatching);
            this.panel2.Controls.Add(this.matchingAddressView1);
            this.panel2.Location = new System.Drawing.Point(6, 265);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(467, 160);
            this.panel2.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.editAddressView1);
            this.panel3.Location = new System.Drawing.Point(6, 435);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(467, 166);
            this.panel3.TabIndex = 2;
            // 
            // FormAddressWithCountyVerification
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(228)))), ((int)(((byte)(243)))));
            this.ClientSize = new System.Drawing.Size(479, 630);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancelEdit);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAddressWithCountyVerification";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Address Entry";
            this.Load += new System.EventHandler(this.FormAddressWithCountyVerification_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblMatching;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.CheckBox cbxIgnore;
        private System.Windows.Forms.Panel panel3;
        private PatientAccess.UI.CommonControls.LoggingButton btnOk;
        private PatientAccess.UI.CommonControls.LoggingButton btnCancelEdit;

        private PatientAccess.UI.AddressViews.AddressEntryWithCountyView addressEntryView1;
        private PatientAccess.UI.AddressViews.MatchingAddressView matchingAddressView1;
        private PatientAccess.UI.AddressViews.EditAddressView editAddressView1;

    }
}