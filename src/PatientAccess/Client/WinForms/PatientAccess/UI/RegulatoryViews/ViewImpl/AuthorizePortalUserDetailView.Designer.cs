using Extensions.UI.Winforms;
using System.Windows.Forms;
namespace PatientAccess.UI.RegulatoryViews.ViewImpl
{
    partial class AuthorizePortalUserDetailView
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
            this.lblFirstName = new System.Windows.Forms.Label();
            this.lblLastName = new System.Windows.Forms.Label();
            this.lblDateOfBirth = new System.Windows.Forms.Label();
            this.lblEmailAddress = new System.Windows.Forms.Label();
            this.mtbFirstName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbLastName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbDateOfBirth = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbEmailAddress = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.chkRemoveUser = new System.Windows.Forms.CheckBox();
            this.lblRemoveUser = new System.Windows.Forms.Label();
            this.PnlDivider = new System.Windows.Forms.Panel();
            this.pnlHide = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // lblFirstName
            // 
            this.lblFirstName.Location = new System.Drawing.Point(65, 71);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(158, 41);
            this.lblFirstName.Text = "First Name";
            // 
            // lblLastName
            // 
            this.lblLastName.Location = new System.Drawing.Point(467, 71);
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size(158, 41);
            this.lblLastName.Text = "Last Name";
            // 
            // lblDateOfBirth
            // 
            this.lblDateOfBirth.Location = new System.Drawing.Point(826, 71);
            this.lblDateOfBirth.Name = "lblDateOfBirth";
            this.lblDateOfBirth.Size = new System.Drawing.Size(82, 38);
            this.lblDateOfBirth.Text = "DOB";
            // 
            // lblEmailAddress
            // 
            this.lblEmailAddress.Location = new System.Drawing.Point(1074, 71);
            this.lblEmailAddress.Name = "lblEmailAddress";
            this.lblEmailAddress.Size = new System.Drawing.Size(212, 38);
            this.lblEmailAddress.Text = "Email Address";
            // 
            // mtbFirstName
            // 
            this.mtbFirstName.BackColor = System.Drawing.Color.White;
            this.mtbFirstName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbFirstName.ForeColor = System.Drawing.SystemColors.WindowText;
            this.mtbFirstName.Location = new System.Drawing.Point(19, 18);
            this.mtbFirstName.Mask = "";
            this.mtbFirstName.MaxLength = 13;
            this.mtbFirstName.Name = "mtbFirstName";
            this.mtbFirstName.Size = new System.Drawing.Size(287, 38);
            this.mtbFirstName.TabIndex = 0;
            this.mtbFirstName.Validating += new System.ComponentModel.CancelEventHandler(this.mtbFirstName_Validating);
            // 
            // mtbLastName
            // 
            this.mtbLastName.BackColor = System.Drawing.Color.White;
            this.mtbLastName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbLastName.ForeColor = System.Drawing.SystemColors.WindowText;
            this.mtbLastName.Location = new System.Drawing.Point(331, 18);
            this.mtbLastName.Mask = "";
            this.mtbLastName.MaxLength = 25;
            this.mtbLastName.Name = "mtbLastName";
            this.mtbLastName.Size = new System.Drawing.Size(432, 38);
            this.mtbLastName.TabIndex = 1;
            this.mtbLastName.Validating += new System.ComponentModel.CancelEventHandler(this.mtbLastName_Validating);
            // 
            // mtbDateOfBirth
            // 
            this.mtbDateOfBirth.Location = new System.Drawing.Point(792, 18);
            this.mtbDateOfBirth.KeyPressExpression = "^\\d*$";
            this.mtbDateOfBirth.Mask = "  /  /    ";
            this.mtbDateOfBirth.MaxLength = 10;
            this.mtbDateOfBirth.Name = "mtbDateOfBirth";
            this.mtbDateOfBirth.Size = new System.Drawing.Size(173, 38);
            this.mtbDateOfBirth.TabIndex = 2;
            this.mtbDateOfBirth.ValidationExpression = "^\\d*$";
            this.mtbDateOfBirth.Validating += new System.ComponentModel.CancelEventHandler(this.mtbDateOfBirth_Validating);
            // 
            // mtbEmailAddress
            // 
            this.mtbEmailAddress.Location = new System.Drawing.Point(990, 18);
            this.mtbEmailAddress.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbEmailAddress.Mask = "";
            this.mtbEmailAddress.MaxLength = 64;
            this.mtbEmailAddress.Name = "mtbEmailAddress";
            this.mtbEmailAddress.Size = new System.Drawing.Size(384, 38);
            this.mtbEmailAddress.TabIndex = 3;
            this.mtbEmailAddress.Validating += new System.ComponentModel.CancelEventHandler(this.mtbEmailAddress_Validating);
            // 
            // chkRemoveUser
            // 
            this.chkRemoveUser.AutoSize = true;
            this.chkRemoveUser.Location = new System.Drawing.Point(1469, 18);
            this.chkRemoveUser.Name = "chkRemoveUser";
            this.chkRemoveUser.Text = " ";
            this.chkRemoveUser.Size = new System.Drawing.Size(60, 36);
            this.chkRemoveUser.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.chkRemoveUser.UseVisualStyleBackColor = true;
            this.chkRemoveUser.TabStop = false;
            // 
            // lblRemoveUser
            // 
            this.lblRemoveUser.Location = new System.Drawing.Point(1398, 71);
            this.lblRemoveUser.Name = "lblRemoveUser";
            this.lblRemoveUser.Size = new System.Drawing.Size(194, 43);
            this.lblRemoveUser.Text = "Remove User";
            // 
            // PnlDivider
            // 
            this.PnlDivider.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PnlDivider.Location = new System.Drawing.Point(19, 116);
            this.PnlDivider.Name = "PnlDivider";
            this.PnlDivider.Size = new System.Drawing.Size(1550, 1);
            this.PnlDivider.TabIndex = 52;
            // 
            // pnlHide
            // 
            this.pnlHide.Location = new System.Drawing.Point(1508, 18);
            this.pnlHide.Name = "pnlHide";
            this.pnlHide.Size = new System.Drawing.Size(43, 50);
            this.pnlHide.TabIndex = 59;
            // 
            // AuthorizePortalUserDetailView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pnlHide);
            this.Controls.Add(this.PnlDivider);
            this.Controls.Add(this.lblRemoveUser);
            this.Controls.Add(this.chkRemoveUser);
            this.Controls.Add(this.lblFirstName);
            this.Controls.Add(this.lblLastName);
            this.Controls.Add(this.lblDateOfBirth);
            this.Controls.Add(this.lblEmailAddress);
            this.Controls.Add(this.mtbFirstName);
            this.Controls.Add(this.mtbLastName);
            this.Controls.Add(this.mtbDateOfBirth);
            this.Controls.Add(this.mtbEmailAddress);
            this.Name = "AuthorizePortalUserDetailView";
            this.Size = new System.Drawing.Size(1580, 151);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CheckBox chkRemoveUser;
        private Label lblRemoveUser;
        private Label lblFirstName;
        private Label lblLastName;
        private Label lblDateOfBirth;
        private Label lblEmailAddress;
        private MaskedEditTextBox mtbFirstName;
        private MaskedEditTextBox mtbLastName;
        private MaskedEditTextBox mtbDateOfBirth;
        private MaskedEditTextBox mtbEmailAddress;
        private Panel PnlDivider;
        private Panel pnlHide;

    }
}