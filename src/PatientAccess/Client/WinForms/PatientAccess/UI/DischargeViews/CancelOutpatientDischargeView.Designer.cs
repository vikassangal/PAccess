namespace PatientAccess.UI.DischargeViews
{
    partial class CancelOutpatientDischargeView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelActions.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelPatientContext.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.panelMessages.SuspendLayout();
            this.gbScanDocuments.SuspendLayout();
            this.SuspendLayout();
            // 
            // userContextView1
            // 
            this.userContextView1.Description = "Cancel Outpatient Discharge";
            // 
            // mtbDischargeDate
            // 
            this.mtbDischargeDate.MaxLength = 10;
            this.mtbDischargeDate.Size = new System.Drawing.Size(89, 20);
            // 
            // mtbDischargeTime
            // 
            this.mtbDischargeTime.Location = new System.Drawing.Point(245, 163);
            // 
            // btnOk
            // 
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCloseActivity
            // 
            this.btnCloseActivity.TabIndex = 1;
            // 
            // btnRepeatActivity
            // 
            this.btnRepeatActivity.TabIndex = 2;
            // 
            // btnEditAccount
            // 
            this.btnEditAccount.Size = new System.Drawing.Size(125, 23);
            this.btnEditAccount.TabIndex = 3;
            // 
            // dtpDischargeDate
            // 
            this.dtpDischargeDate.TabStop = false;
            this.dtpDischargeDate.Visible = false;
            // 
            // panel2
            // 
            this.panel2.Size = new System.Drawing.Size(999, 515);
            this.panel2.TabIndex = 2;
            // 
            // lblInstructionalMessage
            // 
            this.lblInstructionalMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInstructionalMessage.Location = new System.Drawing.Point(12, 10);
            // 
            // label2
            // 
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label2.Size = new System.Drawing.Size(937, 16);
            // 
            // lblOutstandingActionItemsMsg
            // 
            this.lblOutstandingActionItemsMsg.Location = new System.Drawing.Point(12, 67);
            this.lblOutstandingActionItemsMsg.Size = new System.Drawing.Size(912, 35);
            // 
            // lblMessages
            // 
            this.lblMessages.Size = new System.Drawing.Size(957, 28);
            // 
            // lblCurrentOccupant
            // 
            this.lblCurrentOccupant.Size = new System.Drawing.Size(390, 13);
            // 
            // lblDischargeDispositionVal
            // 
            this.lblDischargeDispositionVal.Location = new System.Drawing.Point(128, 193);
            this.lblDischargeDispositionVal.Size = new System.Drawing.Size(213, 20);
            this.lblDischargeDispositionVal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblDischargeDispositionVal.Visible = true;
            // 
            // lblDischargeDateVal
            // 
            this.lblDischargeDateVal.Location = new System.Drawing.Point(98, 166);
            this.lblDischargeDateVal.Size = new System.Drawing.Size(99, 19);
            this.lblDischargeDateVal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblDischargeDateVal.Visible = true;
            // 
            // lblDischargeTimeVal
            // 
            this.lblDischargeTimeVal.Size = new System.Drawing.Size(56, 17);
            this.lblDischargeTimeVal.Text = "";
            this.lblDischargeTimeVal.Visible = true;
            // 
            // CancelOutpatientDischargeView
            // 
            this.AcceptButton = this.btnOk;
            this.Name = "CancelOutpatientDischargeView";
            this.Size = new System.Drawing.Size(1037, 618);
            this.panelActions.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panelPatientContext.ResumeLayout(false);
            this.panelButtons.ResumeLayout(false);
            this.panelMessages.ResumeLayout(false);
            this.gbScanDocuments.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
