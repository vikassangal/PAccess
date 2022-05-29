using System.Windows.Forms;

namespace PatientAccess.UI.RegulatoryViews.ViewImpl
{
    partial class AuthorizeAdditionalPortalUsersView
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
            this.lblAuthoizePatientPortalUser = new System.Windows.Forms.Label();
            this.rbAuthorizeAdditionalPortalUsersYes = new PatientAccess.UI.CommonControls.RadioButtonKeyHandler();
            this.rbAuthorizeAdditionalPortalUsersNo = new PatientAccess.UI.CommonControls.RadioButtonKeyHandler();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblAuthoizePatientPortalUser
            // 
            this.lblAuthoizePatientPortalUser.Location = new System.Drawing.Point(-1, 17);
            this.lblAuthoizePatientPortalUser.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblAuthoizePatientPortalUser.Name = "lblAuthoizePatientPortalUser";
            this.lblAuthoizePatientPortalUser.Size = new System.Drawing.Size(299, 60);
            this.lblAuthoizePatientPortalUser.TabIndex = 15;
            this.lblAuthoizePatientPortalUser.Text = "Authorize Additional\nPatient Portal Users?";
            // 
            // rbAuthorizeAdditionalPortalUsersYes
            // 
            this.rbAuthorizeAdditionalPortalUsersYes.Location = new System.Drawing.Point(10, 7);
            this.rbAuthorizeAdditionalPortalUsersYes.Margin = new System.Windows.Forms.Padding(8, 7, 5, 7);
            this.rbAuthorizeAdditionalPortalUsersYes.Name = "rbAuthorizeAdditionalPortalUsersYes";
            this.rbAuthorizeAdditionalPortalUsersYes.Size = new System.Drawing.Size(108, 39);
            this.rbAuthorizeAdditionalPortalUsersYes.TabIndex = 3;
            this.rbAuthorizeAdditionalPortalUsersYes.TabStop = true;
            this.rbAuthorizeAdditionalPortalUsersYes.Text = "Yes";
            this.rbAuthorizeAdditionalPortalUsersYes.Click += new System.EventHandler(this.rbAuthorizeAdditionalPortalUsersYes_Click);
            this.rbAuthorizeAdditionalPortalUsersYes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rbAuthorizeAdditionalPortalUsersYes_KeyDown);
            // 
            // rbAuthorizeAdditionalPortalUsersNo
            // 
            this.rbAuthorizeAdditionalPortalUsersNo.Location = new System.Drawing.Point(118, 10);
            this.rbAuthorizeAdditionalPortalUsersNo.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.rbAuthorizeAdditionalPortalUsersNo.Name = "rbAuthorizeAdditionalPortalUsersNo";
            this.rbAuthorizeAdditionalPortalUsersNo.Size = new System.Drawing.Size(112, 36);
            this.rbAuthorizeAdditionalPortalUsersNo.TabIndex = 4;
            this.rbAuthorizeAdditionalPortalUsersNo.TabStop = true;
            this.rbAuthorizeAdditionalPortalUsersNo.Text = "No";
            this.rbAuthorizeAdditionalPortalUsersNo.Click += new System.EventHandler(this.rbAuthorizeAdditionalPortalUsersNo_Click);
            this.rbAuthorizeAdditionalPortalUsersNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rbAuthorizeAdditionalPortalUsersNo_KeyDown);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbAuthorizeAdditionalPortalUsersYes);
            this.panel1.Controls.Add(this.rbAuthorizeAdditionalPortalUsersNo);
            this.panel1.Location = new System.Drawing.Point(352, 17);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(274, 68);
            this.panel1.TabIndex = 16;
            // 
            // AuthorizeAdditionalPortalUsersView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblAuthoizePatientPortalUser);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "AuthorizeAdditionalPortalUsersView";
            this.Size = new System.Drawing.Size(757, 125);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
 
        private global::PatientAccess.UI.CommonControls.RadioButtonKeyHandler rbAuthorizeAdditionalPortalUsersNo;
        private System.Windows.Forms.Label lblAuthoizePatientPortalUser;
        private global::PatientAccess.UI.CommonControls.RadioButtonKeyHandler rbAuthorizeAdditionalPortalUsersYes;
        private AuthorizePortalUserView authorizePortalUserView;
        private Panel panel1;
    }
}