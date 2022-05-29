using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.QuickAccountCreation.ViewImpl
{
    partial class QuickPhysicianSelectionView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            UnRegisterRulesEvents();

            if ( disposing )
            {
                if ( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpPhysicians = new System.Windows.Forms.GroupBox();
            this.lblRefDisplayVal = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblAdmDisplayVal = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAdmClear = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnAdmViewDetails = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnRefClear = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnRefViewDetails = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnRecNonStaff = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblRecNonStaffPhysician = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblFindPhysician = new System.Windows.Forms.Label();
            this.btnFind = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panelSpecifyPhysician = new System.Windows.Forms.Panel();
            this.lblAdm = new System.Windows.Forms.Label();
            this.mtbAdm = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblRef = new System.Windows.Forms.Label();
            this.lblSpecifyByName = new System.Windows.Forms.Label();
            this.btnVerify = new PatientAccess.UI.CommonControls.LoggingButton();
            this.mtbRef = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lineLabel1 = new PatientAccess.UI.CommonControls.LineLabel();
            this.lineLabel2 = new PatientAccess.UI.CommonControls.LineLabel();
            this.lineLabel3 = new PatientAccess.UI.CommonControls.LineLabel();
            this.lblFind = new System.Windows.Forms.Label();
            this.grpPhysicians.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelSpecifyPhysician.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPhysicians
            // 
            this.grpPhysicians.Controls.Add(this.lblRefDisplayVal);
            this.grpPhysicians.Controls.Add(this.label8);
            this.grpPhysicians.Controls.Add(this.lblAdmDisplayVal);
            this.grpPhysicians.Controls.Add(this.label2);
            this.grpPhysicians.Controls.Add(this.btnAdmClear);
            this.grpPhysicians.Controls.Add(this.btnAdmViewDetails);
            this.grpPhysicians.Controls.Add(this.btnRefClear);
            this.grpPhysicians.Controls.Add(this.btnRefViewDetails);
            this.grpPhysicians.Controls.Add(this.panel2);
            this.grpPhysicians.Controls.Add(this.panel1);
            this.grpPhysicians.Controls.Add(this.panelSpecifyPhysician);
            this.grpPhysicians.Controls.Add(this.lineLabel1);
            this.grpPhysicians.Controls.Add(this.lineLabel2);
            this.grpPhysicians.Controls.Add(this.lineLabel3);
            this.grpPhysicians.Location = new System.Drawing.Point(0, 0);
            this.grpPhysicians.Margin = new System.Windows.Forms.Padding(0);
            this.grpPhysicians.Name = "grpPhysicians";
            this.grpPhysicians.Padding = new System.Windows.Forms.Padding(0);
            this.grpPhysicians.Size = new System.Drawing.Size(859, 125);
            this.grpPhysicians.TabIndex = 0;
            this.grpPhysicians.TabStop = false;
            this.grpPhysicians.Text = "Physicians";
            // 
            // lblRefDisplayVal
            // 
            this.lblRefDisplayVal.Location = new System.Drawing.Point(372, 27);
            this.lblRefDisplayVal.Name = "lblRefDisplayVal";
            this.lblRefDisplayVal.Size = new System.Drawing.Size(288, 17);
            this.lblRefDisplayVal.TabIndex = 63;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(335, 57);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 13);
            this.label8.TabIndex = 66;
            this.label8.Text = "Adm:";
            // 
            // lblAdmDisplayVal
            // 
            this.lblAdmDisplayVal.Location = new System.Drawing.Point(372, 56);
            this.lblAdmDisplayVal.Name = "lblAdmDisplayVal";
            this.lblAdmDisplayVal.Size = new System.Drawing.Size(288, 15);
            this.lblAdmDisplayVal.TabIndex = 68;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(335, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 12);
            this.label2.TabIndex = 61;
            this.label2.Text = "Ref:";
            // 
            // btnAdmClear
            // 
            this.btnAdmClear.Location = new System.Drawing.Point(775, 55);
            this.btnAdmClear.Message = null;
            this.btnAdmClear.Name = "btnAdmClear";
            this.btnAdmClear.Size = new System.Drawing.Size(71, 23);
            this.btnAdmClear.TabIndex = 12;
            this.btnAdmClear.Text = "Clear";
            this.btnAdmClear.Click += new System.EventHandler(this.btnAdmClear_Click);
            // 
            // btnAdmViewDetails
            // 
            this.btnAdmViewDetails.Location = new System.Drawing.Point(679, 54);
            this.btnAdmViewDetails.Message = null;
            this.btnAdmViewDetails.Name = "btnAdmViewDetails";
            this.btnAdmViewDetails.Size = new System.Drawing.Size(87, 23);
            this.btnAdmViewDetails.TabIndex = 11;
            this.btnAdmViewDetails.Text = "View Details";
            this.btnAdmViewDetails.Click += new System.EventHandler(this.btnAdmViewDetails_Click);
            // 
            // btnRefClear
            // 
            this.btnRefClear.Location = new System.Drawing.Point(775, 23);
            this.btnRefClear.Message = null;
            this.btnRefClear.Name = "btnRefClear";
            this.btnRefClear.Size = new System.Drawing.Size(71, 23);
            this.btnRefClear.TabIndex = 10;
            this.btnRefClear.Text = "Clear";
            this.btnRefClear.Click += new System.EventHandler(this.btnRefClear_Click);
            // 
            // btnRefViewDetails
            // 
            this.btnRefViewDetails.Location = new System.Drawing.Point(679, 23);
            this.btnRefViewDetails.Message = null;
            this.btnRefViewDetails.Name = "btnRefViewDetails";
            this.btnRefViewDetails.Size = new System.Drawing.Size(87, 23);
            this.btnRefViewDetails.TabIndex = 9;
            this.btnRefViewDetails.Text = "View Details";
            this.btnRefViewDetails.Click += new System.EventHandler(this.btnRefViewDetails_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(142)))));
            this.panel2.Controls.Add(this.btnRecNonStaff);
            this.panel2.Controls.Add(this.lblRecNonStaffPhysician);
            this.panel2.Location = new System.Drawing.Point(8, 93);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(317, 28);
            this.panel2.TabIndex = 2;
            // 
            // btnRecNonStaff
            // 
            this.btnRecNonStaff.Location = new System.Drawing.Point(187, 3);
            this.btnRecNonStaff.Message = "Click record nonstaff";
            this.btnRecNonStaff.Name = "btnRecNonStaff";
            this.btnRecNonStaff.Size = new System.Drawing.Size(96, 23);
            this.btnRecNonStaff.TabIndex = 8;
            this.btnRecNonStaff.Text = "&Record Nonstaff";
            this.btnRecNonStaff.Click += new System.EventHandler(this.btnRecNonStaff_Click);
            // 
            // lblRecNonStaffPhysician
            // 
            this.lblRecNonStaffPhysician.Location = new System.Drawing.Point(5, 3);
            this.lblRecNonStaffPhysician.Name = "lblRecNonStaffPhysician";
            this.lblRecNonStaffPhysician.Size = new System.Drawing.Size(158, 26);
            this.lblRecNonStaffPhysician.TabIndex = 50;
            this.lblRecNonStaffPhysician.Text = "Record nonstaff physician";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(142)))));
            this.panel1.Controls.Add(this.lblFindPhysician);
            this.panel1.Controls.Add(this.btnFind);
            this.panel1.Location = new System.Drawing.Point(8, 65);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(317, 28);
            this.panel1.TabIndex = 1;
            // 
            // lblFindPhysician
            // 
            this.lblFindPhysician.Location = new System.Drawing.Point(2, 5);
            this.lblFindPhysician.Name = "lblFindPhysician";
            this.lblFindPhysician.Size = new System.Drawing.Size(181, 31);
            this.lblFindPhysician.TabIndex = 54;
            this.lblFindPhysician.Text = "Find physician by name or specialty";
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(187, 4);
            this.btnFind.Message = "Click find physician";
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(66, 23);
            this.btnFind.TabIndex = 7;
            this.btnFind.Text = "F&ind";
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // panelSpecifyPhysician
            // 
            this.panelSpecifyPhysician.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(142)))));
            this.panelSpecifyPhysician.Controls.Add(this.lblAdm);
            this.panelSpecifyPhysician.Controls.Add(this.mtbAdm);
            this.panelSpecifyPhysician.Controls.Add(this.lblRef);
            this.panelSpecifyPhysician.Controls.Add(this.lblSpecifyByName);
            this.panelSpecifyPhysician.Controls.Add(this.btnVerify);
            this.panelSpecifyPhysician.Controls.Add(this.mtbRef);
            this.panelSpecifyPhysician.Location = new System.Drawing.Point(8, 15);
            this.panelSpecifyPhysician.Name = "panelSpecifyPhysician";
            this.panelSpecifyPhysician.Size = new System.Drawing.Size(317, 50);
            this.panelSpecifyPhysician.TabIndex = 0;
            // 
            // lblAdm
            // 
            this.lblAdm.Location = new System.Drawing.Point(202, 6);
            this.lblAdm.Name = "lblAdm";
            this.lblAdm.Size = new System.Drawing.Size(29, 13);
            this.lblAdm.TabIndex = 55;
            this.lblAdm.Text = "Adm:";
            // 
            // mtbAdm
            // 
            this.mtbAdm.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbAdm.KeyPressExpression = "^\\d*$";
            this.mtbAdm.Location = new System.Drawing.Point(201, 25);
            this.mtbAdm.Mask = "";
            this.mtbAdm.MaxLength = 5;
            this.mtbAdm.Multiline = true;
            this.mtbAdm.Name = "mtbAdm";
            this.mtbAdm.Size = new System.Drawing.Size(37, 20);
            this.mtbAdm.TabIndex = 2;
            this.mtbAdm.ValidationExpression = "^\\d*$";
            this.mtbAdm.Enter += new System.EventHandler(this.PhysicianNum_Enter);
            this.mtbAdm.Validating += new System.ComponentModel.CancelEventHandler(this.PhysicianNum_Validating);
            // 
            // lblRef
            // 
            this.lblRef.Location = new System.Drawing.Point(157, 6);
            this.lblRef.Name = "lblRef";
            this.lblRef.Size = new System.Drawing.Size(32, 13);
            this.lblRef.TabIndex = 0;
            this.lblRef.Text = "Ref:";
            // 
            // lblSpecifyByName
            // 
            this.lblSpecifyByName.Location = new System.Drawing.Point(5, 25);
            this.lblSpecifyByName.Name = "lblSpecifyByName";
            this.lblSpecifyByName.Size = new System.Drawing.Size(147, 18);
            this.lblSpecifyByName.TabIndex = 50;
            this.lblSpecifyByName.Text = "Specify physician by number";
            // 
            // btnVerify
            // 
            this.btnVerify.Location = new System.Drawing.Point(249, 25);
            this.btnVerify.Message = "Click physician verify";
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.Size = new System.Drawing.Size(56, 23);
            this.btnVerify.TabIndex = 6;
            this.btnVerify.Text = "&Verify";
            this.btnVerify.Click += new System.EventHandler(this.btnVerify_Click);
            // 
            // mtbRef
            // 
            this.mtbRef.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbRef.KeyPressExpression = "^\\d*$";
            this.mtbRef.Location = new System.Drawing.Point(158, 25);
            this.mtbRef.Mask = "";
            this.mtbRef.MaxLength = 5;
            this.mtbRef.Multiline = true;
            this.mtbRef.Name = "mtbRef";
            this.mtbRef.Size = new System.Drawing.Size(37, 20);
            this.mtbRef.TabIndex = 1;
            this.mtbRef.ValidationExpression = "^\\d*$";
            this.mtbRef.Enter += new System.EventHandler(this.PhysicianNum_Enter);
            this.mtbRef.Validating += new System.ComponentModel.CancelEventHandler(this.PhysicianNum_Validating);
            // 
            // lineLabel1
            // 
            this.lineLabel1.Caption = "";
            this.lineLabel1.Location = new System.Drawing.Point(334, 7);
            this.lineLabel1.Name = "lineLabel1";
            this.lineLabel1.Size = new System.Drawing.Size(518, 15);
            this.lineLabel1.TabIndex = 84;
            this.lineLabel1.TabStop = false;
            // 
            // lineLabel2
            // 
            this.lineLabel2.Caption = "";
            this.lineLabel2.Location = new System.Drawing.Point(335, 38);
            this.lineLabel2.Name = "lineLabel2";
            this.lineLabel2.Size = new System.Drawing.Size(517, 15);
            this.lineLabel2.TabIndex = 85;
            this.lineLabel2.TabStop = false;
            // 
            // lineLabel3
            // 
            this.lineLabel3.Caption = "";
            this.lineLabel3.Location = new System.Drawing.Point(334, 70);
            this.lineLabel3.Name = "lineLabel3";
            this.lineLabel3.Size = new System.Drawing.Size(517, 18);
            this.lineLabel3.TabIndex = 86;
            this.lineLabel3.TabStop = false;
            // 
            // lblFind
            // 
            this.lblFind.Location = new System.Drawing.Point(0, 0);
            this.lblFind.Name = "lblFind";
            this.lblFind.Size = new System.Drawing.Size(100, 23);
            this.lblFind.TabIndex = 0;
            // 
            // QuickPhysicianSelectionView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.grpPhysicians);
            this.Name = "QuickPhysicianSelectionView";
            this.Size = new System.Drawing.Size(861, 168);
            this.Leave += new System.EventHandler(this.QuickPhysicianSelectionView_Leave);
            this.grpPhysicians.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panelSpecifyPhysician.ResumeLayout(false);
            this.panelSpecifyPhysician.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        #region Data Elements

        private GroupBox grpPhysicians;
        private Label lblAdm;
        private MaskedEditTextBox mtbAdm;
        private Label lblRef;
        private Label lblSpecifyByName;
        private LoggingButton btnVerify;
        private MaskedEditTextBox mtbRef;
        private Panel panelSpecifyPhysician;
        private Panel panel1;
        private LoggingButton btnFind;
        private Label lblFind;
        private Panel panel2;
        private LoggingButton btnRecNonStaff;
        private Label label2;
        private Label label8;
        private LoggingButton btnAdmViewDetails;
        private LoggingButton btnRefViewDetails;
        private LoggingButton btnRefClear;
        private LoggingButton btnAdmClear;
        private Label lblAdmDisplayVal;
        private Label lblRefDisplayVal;
        private Label lblRecNonStaffPhysician;
        private Label lblFindPhysician;
        private LineLabel lineLabel1;
        private LineLabel lineLabel2;
        private LineLabel lineLabel3;
        private LoggingButton i_btnOriginalAcceptButton;

        #endregion
    }
}
