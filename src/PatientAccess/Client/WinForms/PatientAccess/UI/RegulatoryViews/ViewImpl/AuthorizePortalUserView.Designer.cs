using Extensions.UI.Winforms;
using PatientAccess.Utilities;
using System.Windows.Forms;

namespace PatientAccess.UI.RegulatoryViews.ViewImpl
{
    partial class AuthorizePortalUserView
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
            this.btnSaveResponse = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.grpboxMessage = new System.Windows.Forms.Panel();
            this.PnlDivider = new System.Windows.Forms.Panel();
            this.authorizePortalUserDetailView3 = new PatientAccess.UI.RegulatoryViews.ViewImpl.AuthorizePortalUserDetailView();
            this.authorizePortalUserDetailView2 = new PatientAccess.UI.RegulatoryViews.ViewImpl.AuthorizePortalUserDetailView();
            this.authorizePortalUserDetailView1 = new PatientAccess.UI.RegulatoryViews.ViewImpl.AuthorizePortalUserDetailView();
            this.authorizePortalUserDetailView0 = new PatientAccess.UI.RegulatoryViews.ViewImpl.AuthorizePortalUserDetailView();
            this.grpboxMessage.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSaveResponse
            // 
            this.btnSaveResponse.Location = new System.Drawing.Point(1355, 705);
            this.btnSaveResponse.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.btnSaveResponse.Name = "btnSaveResponse";
            this.btnSaveResponse.Size = new System.Drawing.Size(267, 55);
            this.btnSaveResponse.Text = "Save Responses";
            this.btnSaveResponse.UseVisualStyleBackColor = true;
            this.btnSaveResponse.Click += new System.EventHandler(this.btnSaveResponse_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(1157, 705);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(173, 55);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.Location = new System.Drawing.Point(390, 10);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(750, 34);
            this.lblMessage.TabIndex = 52;
            this.lblMessage.Text = "All fields are required to validate Authorized User";
            this.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //
            // 
            // grpboxMessage
            // 
            this.grpboxMessage.BackColor = System.Drawing.Color.White;
            this.grpboxMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.grpboxMessage.Controls.Add(this.PnlDivider);
            this.grpboxMessage.Controls.Add(this.authorizePortalUserDetailView3);
            this.grpboxMessage.Controls.Add(this.authorizePortalUserDetailView2);
            this.grpboxMessage.Controls.Add(this.lblMessage);
            this.grpboxMessage.Controls.Add(this.authorizePortalUserDetailView1);
            this.grpboxMessage.Controls.Add(this.authorizePortalUserDetailView0);
            this.grpboxMessage.Location = new System.Drawing.Point(20, 25);
            this.grpboxMessage.Name = "grpboxMessage";
            this.grpboxMessage.Size = new System.Drawing.Size(1590, 660);
            this.grpboxMessage.TabIndex = 53;
          
            // 
            // PnlDivider
            // 
            this.PnlDivider.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PnlDivider.Location = new System.Drawing.Point(21, 66);
            this.PnlDivider.Name = "PnlDivider";
            this.PnlDivider.Size = new System.Drawing.Size(1545, 1);
            this.PnlDivider.TabIndex = 58;
            // 
            // authorizePortalUserDetailView3
            // 
            this.authorizePortalUserDetailView3.BackColor = System.Drawing.Color.White;
            this.authorizePortalUserDetailView3.Location = new System.Drawing.Point(1, 484);
            this.authorizePortalUserDetailView3.Model = null;
            this.authorizePortalUserDetailView3.Name = "authorizePortalUserDetailView3";
            this.authorizePortalUserDetailView3.Size = new System.Drawing.Size(1590, 164);
            this.authorizePortalUserDetailView3.TabIndex = 4;
            // 
            // authorizePortalUserDetailView2
            // 
            this.authorizePortalUserDetailView2.BackColor = System.Drawing.Color.White;
            this.authorizePortalUserDetailView2.Location = new System.Drawing.Point(1, 349);
            this.authorizePortalUserDetailView2.Model = null;
            this.authorizePortalUserDetailView2.Name = "authorizePortalUserDetailView2";
            this.authorizePortalUserDetailView2.Size = new System.Drawing.Size(1590, 150);
            this.authorizePortalUserDetailView2.TabIndex = 3;
            // 
            // authorizePortalUserDetailView1
            // 
            this.authorizePortalUserDetailView1.BackColor = System.Drawing.Color.White;
            this.authorizePortalUserDetailView1.Location = new System.Drawing.Point(1, 220);
            this.authorizePortalUserDetailView1.Model = null;
            this.authorizePortalUserDetailView1.Name = "authorizePortalUserDetailView1";
            this.authorizePortalUserDetailView1.Size = new System.Drawing.Size(1590, 136);
            this.authorizePortalUserDetailView1.TabIndex = 2;
            // 
            // authorizePortalUserDetailView0
            // 
            this.authorizePortalUserDetailView0.BackColor = System.Drawing.Color.White;
            this.authorizePortalUserDetailView0.Location = new System.Drawing.Point(1, 87);
            this.authorizePortalUserDetailView0.Model = null;
            this.authorizePortalUserDetailView0.Name = "authorizePortalUserDetailView0";
            this.authorizePortalUserDetailView0.Size = new System.Drawing.Size(1590, 139);
            this.authorizePortalUserDetailView0.TabIndex = 1;
            // 
            // AuthorizePortalUserView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true; 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(228)))), ((int)(((byte)(243)))));
            this.ClientSize = new System.Drawing.Size(1700, 771);
            this.Controls.Add(this.grpboxMessage);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSaveResponse);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AuthorizePortalUserView";
            this.Padding = new System.Windows.Forms.Padding(0, 0, 12, 10);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Patient Portal Authorized User Details";
            this.grpboxMessage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion


        public AuthorizePortalUserDetailView authorizePortalUserDetailView0; 
        private System.Windows.Forms.Button btnSaveResponse;
        private System.Windows.Forms.Button btnCancel;
       
      
        private Label lblMessage;
        private Panel grpboxMessage;
        public AuthorizePortalUserDetailView authorizePortalUserDetailView1;
        public AuthorizePortalUserDetailView authorizePortalUserDetailView2;
        public AuthorizePortalUserDetailView authorizePortalUserDetailView3;
        private Panel PnlDivider;
    }
}