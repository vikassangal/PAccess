using System;
using Extensions.UI.Winforms;
using PatientAccess.Utilities;
using PatientAccess.UI.CommonControls;
using System.Windows.Forms;

namespace PatientAccess.UI.DemographicsViews.ViewImpl
{
    partial class AdditionalRacesView
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
            this.grpboxMessage = new System.Windows.Forms.Panel();
            this.lblRace5 = new System.Windows.Forms.Label();
            this.lblRace4 = new System.Windows.Forms.Label();
            this.lblRace3 = new System.Windows.Forms.Label();
            this.cmbRace3 = new PatientAccessComboBox();
            this.cmbRace4 = new PatientAccessComboBox();
            this.cmbRace5 = new PatientAccessComboBox();
            this.PnlDivider = new System.Windows.Forms.Panel();
            this.grpboxMessage.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSaveResponse
            // 
            this.btnSaveResponse.Location = new System.Drawing.Point(489, 165);
            this.btnSaveResponse.Name = "btnSaveResponse";
            this.btnSaveResponse.Size = new System.Drawing.Size(100, 23);
            this.btnSaveResponse.TabIndex = 55;
            this.btnSaveResponse.Text = "Save Responses";
            this.btnSaveResponse.UseVisualStyleBackColor = true;
            this.btnSaveResponse.Click += new System.EventHandler(this.btnSaveResponse_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(418, 165);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(65, 23);
            this.btnCancel.TabIndex = 54;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpboxMessage
            // 
            this.grpboxMessage.BackColor = System.Drawing.Color.White;
            this.grpboxMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.grpboxMessage.Controls.Add(this.PnlDivider);
            this.grpboxMessage.Controls.Add(this.cmbRace5);
            this.grpboxMessage.Controls.Add(this.cmbRace4);
            this.grpboxMessage.Controls.Add(this.cmbRace3);
            this.grpboxMessage.Controls.Add(this.lblRace5);
            this.grpboxMessage.Controls.Add(this.lblRace4);
            this.grpboxMessage.Controls.Add(this.lblRace3);
            this.grpboxMessage.Location = new System.Drawing.Point(8, 10);
            this.grpboxMessage.Margin = new System.Windows.Forms.Padding(1);
            this.grpboxMessage.Name = "grpboxMessage";
            this.grpboxMessage.Size = new System.Drawing.Size(581, 151);
            this.grpboxMessage.TabIndex = 53;
           
            // 
            // lblRace5
            // 
            this.lblRace5.AutoSize = true;
            this.lblRace5.Location = new System.Drawing.Point(425, 37);
            this.lblRace5.Name = "lblRace5";
            this.lblRace5.Size = new System.Drawing.Size(39, 13);
            this.lblRace5.TabIndex = 61;
            this.lblRace5.Text = "Race 5";
           
            // 
            // lblRace4
            // 
            this.lblRace4.AutoSize = true;
            this.lblRace4.Location = new System.Drawing.Point(269, 37);
            this.lblRace4.Name = "lblRace4";
            this.lblRace4.Size = new System.Drawing.Size(39, 13);
            this.lblRace4.TabIndex = 60;
            this.lblRace4.Text = "Race 4";
            // 
            // lblRace3
            // 
            this.lblRace3.AutoSize = true;
            this.lblRace3.Location = new System.Drawing.Point(90, 37);
            this.lblRace3.Name = "lblRace3";
            this.lblRace3.Size = new System.Drawing.Size(39, 13);
            this.lblRace3.TabIndex = 59;
            this.lblRace3.Text = "Race 3";
            // 
            // cboRace3
            // 
            this.cmbRace3.FormattingEnabled = true;
            this.cmbRace3.Location = new System.Drawing.Point(47, 66);
            this.cmbRace3.Name = "cboRace3";
            this.cmbRace3.Size = new System.Drawing.Size(121, 21);
            this.cmbRace3.TabIndex = 62;
            this.cmbRace3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRace3.SelectedIndexChanged += new EventHandler(cmbRace3_SelectedIndexChanged);
            // 
            // cboRace4
            // 
            this.cmbRace4.FormattingEnabled = true;
            this.cmbRace4.Location = new System.Drawing.Point(224, 66);
            this.cmbRace4.Name = "cboRace4";
            this.cmbRace4.Size = new System.Drawing.Size(121, 21);
            this.cmbRace4.TabIndex = 63;
            this.cmbRace4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRace4.SelectedIndexChanged += new EventHandler(cmbRace4_SelectedIndexChanged);
            // 
            // cboRace5
            // 
            this.cmbRace5.FormattingEnabled = true;
            this.cmbRace5.Location = new System.Drawing.Point(389, 66);
            this.cmbRace5.Name = "cboRace5";
            this.cmbRace5.Size = new System.Drawing.Size(121, 21);
            this.cmbRace5.TabIndex = 64;
            this.cmbRace5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            //this.cmbRace5.SelectedIndexChanged += new EventHandler(cmbRace5_SelectedIndexChanged);
            // 
            // PnlDivider
            // 
            this.PnlDivider.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PnlDivider.Location = new System.Drawing.Point(14, 101);
            this.PnlDivider.Margin = new System.Windows.Forms.Padding(1);
            this.PnlDivider.Name = "PnlDivider";
            this.PnlDivider.Size = new System.Drawing.Size(550, 2);
            this.PnlDivider.TabIndex = 65;
            // 
            // AuthorizePortalUserView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(228)))), ((int)(((byte)(243)))));
            this.ClientSize = new System.Drawing.Size(598, 191);
            this.Controls.Add(this.grpboxMessage);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSaveResponse);
            this.Margin = new System.Windows.Forms.Padding(1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdditionalRacesView";
            this.Padding = new System.Windows.Forms.Padding(0, 0, 4, 4);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Multiple Race Details for California Reporting";
            this.grpboxMessage.ResumeLayout(false);
            this.grpboxMessage.PerformLayout();
            this.ResumeLayout(false);

        }
        
        #endregion

        private System.Windows.Forms.Button btnSaveResponse;
        private System.Windows.Forms.Button btnCancel;
        private Panel grpboxMessage;
        private Label lblRace5;
        private Label lblRace4;
        private Label lblRace3;
        private PatientAccessComboBox cmbRace5;
        private PatientAccessComboBox cmbRace4;
        private PatientAccessComboBox cmbRace3;
        private Panel PnlDivider;
    }
}