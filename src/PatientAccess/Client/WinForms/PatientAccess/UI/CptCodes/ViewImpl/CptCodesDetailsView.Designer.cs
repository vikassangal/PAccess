using PatientAccess.Utilities;

namespace PatientAccess.UI.CptCodes.ViewImpl
{
    partial class CptCodesDetailsView
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
            this.lblCptCode1 = new System.Windows.Forms.Label();
            this.mtbCptCode1 = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbCptCode2 = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblCptCode2 = new System.Windows.Forms.Label();
            this.mtbCptCode3 = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblCptCode3 = new System.Windows.Forms.Label();
            this.mtbCptCode4 = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblCptCode4 = new System.Windows.Forms.Label();
            this.mtbCptCode5 = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblCptCode5 = new System.Windows.Forms.Label();
            this.mtbCptCode6 = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblCptCode6 = new System.Windows.Forms.Label();
            this.mtbCptCode7 = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblCptCode7 = new System.Windows.Forms.Label();
            this.mtbCptCode8 = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblCptCode8 = new System.Windows.Forms.Label();
            this.mtbCptCode9 = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblCptCode9 = new System.Windows.Forms.Label();
            this.mtbCptCode10 = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblCptCode10 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblCptCode1
            // 
            this.lblCptCode1.Location = new System.Drawing.Point(12, 26);
            this.lblCptCode1.Name = "lblCptCode1";
            this.lblCptCode1.Size = new System.Drawing.Size(69, 17);
            this.lblCptCode1.TabIndex = 16;
            this.lblCptCode1.Text = "CPT Code 1:";
            // 
            // mtbCptCode1
            // 
            this.mtbCptCode1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbCptCode1.Location = new System.Drawing.Point(115, 23);
            this.mtbCptCode1.Mask = "";
            this.mtbCptCode1.MaxLength = 5;
            this.mtbCptCode1.Name = "mtbCptCode1";
            this.mtbCptCode1.Size = new System.Drawing.Size(108, 20);
            this.mtbCptCode1.TabIndex = 26;
            this.mtbCptCode1.Validating += new System.ComponentModel.CancelEventHandler(this.mtbCptCode1_Validating);
            // 
            // mtbCptCode2
            // 
            this.mtbCptCode2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbCptCode2.Location = new System.Drawing.Point(115, 54);
            this.mtbCptCode2.Mask = "";
            this.mtbCptCode2.MaxLength = 5;
            this.mtbCptCode2.Name = "mtbCptCode2";
            this.mtbCptCode2.Size = new System.Drawing.Size(108, 20);
            this.mtbCptCode2.TabIndex = 28;
            this.mtbCptCode2.Validating += new System.ComponentModel.CancelEventHandler(this.mtbCptCode2_Validating);
            // 
            // lblCptCode2
            // 
            this.lblCptCode2.Location = new System.Drawing.Point(12, 57);
            this.lblCptCode2.Name = "lblCptCode2";
            this.lblCptCode2.Size = new System.Drawing.Size(69, 17);
            this.lblCptCode2.TabIndex = 27;
            this.lblCptCode2.Text = "CPT Code 2:";
            // 
            // mtbCptCode3
            // 
            this.mtbCptCode3.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper; 
            this.mtbCptCode3.Location = new System.Drawing.Point(115, 85);
            this.mtbCptCode3.Mask = "";
            this.mtbCptCode3.MaxLength = 5;
            this.mtbCptCode3.Name = "mtbCptCode3";
            this.mtbCptCode3.Size = new System.Drawing.Size(108, 20);
            this.mtbCptCode3.TabIndex = 30;
            this.mtbCptCode3.Validating += new System.ComponentModel.CancelEventHandler(this.mtbCptCode3_Validating);
            // 
            // lblCptCode3
            // 
            this.lblCptCode3.Location = new System.Drawing.Point(12, 88);
            this.lblCptCode3.Name = "lblCptCode3";
            this.lblCptCode3.Size = new System.Drawing.Size(69, 17);
            this.lblCptCode3.TabIndex = 29;
            this.lblCptCode3.Text = "CPT Code 3:";
            // 
            // mtbCptCode4
            // 
            this.mtbCptCode4.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbCptCode4.Location = new System.Drawing.Point(115, 117);
            this.mtbCptCode4.Mask = "";
            this.mtbCptCode4.MaxLength = 5;
            this.mtbCptCode4.Name = "mtbCptCode4";
            this.mtbCptCode4.Size = new System.Drawing.Size(108, 20);
            this.mtbCptCode4.TabIndex = 32;
            this.mtbCptCode4.Validating += new System.ComponentModel.CancelEventHandler(this.mtbCptCode4_Validating);
            // 
            // lblCptCode4
            // 
            this.lblCptCode4.Location = new System.Drawing.Point(12, 120);
            this.lblCptCode4.Name = "lblCptCode4";
            this.lblCptCode4.Size = new System.Drawing.Size(69, 17);
            this.lblCptCode4.TabIndex = 31;
            this.lblCptCode4.Text = "CPT Code 4:";
            // 
            // mtbCptCode5
            // 
            this.mtbCptCode5.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbCptCode5.Location = new System.Drawing.Point(115, 148);
            this.mtbCptCode5.Mask = "";
            this.mtbCptCode5.MaxLength = 5;
            this.mtbCptCode5.Name = "mtbCptCode5";
            this.mtbCptCode5.Size = new System.Drawing.Size(108, 20);
            this.mtbCptCode5.TabIndex = 34;
            this.mtbCptCode5.Validating += new System.ComponentModel.CancelEventHandler(this.mtbCptCode5_Validating);
            // 
            // lblCptCode5
            // 
            this.lblCptCode5.Location = new System.Drawing.Point(12, 151);
            this.lblCptCode5.Name = "lblCptCode5";
            this.lblCptCode5.Size = new System.Drawing.Size(69, 17);
            this.lblCptCode5.TabIndex = 33;
            this.lblCptCode5.Text = "CPT Code 5:";
            // 
            // mtbCptCode6
            // 
            this.mtbCptCode6.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbCptCode6.Location = new System.Drawing.Point(115, 180);
            this.mtbCptCode6.Mask = "";
            this.mtbCptCode6.MaxLength = 5;
            this.mtbCptCode6.Name = "mtbCptCode6";
            this.mtbCptCode6.Size = new System.Drawing.Size(108, 20);
            this.mtbCptCode6.TabIndex = 36;
            this.mtbCptCode6.Validating += new System.ComponentModel.CancelEventHandler(this.mtbCptCode6_Validating);
            // 
            // lblCptCode6
            // 
            this.lblCptCode6.Location = new System.Drawing.Point(12, 183);
            this.lblCptCode6.Name = "lblCptCode6";
            this.lblCptCode6.Size = new System.Drawing.Size(69, 17);
            this.lblCptCode6.TabIndex = 35;
            this.lblCptCode6.Text = "CPT Code 6:";
            // 
            // mtbCptCode7
            // 
            this.mtbCptCode7.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbCptCode7.Location = new System.Drawing.Point(115, 212);
            this.mtbCptCode7.Mask = "";
            this.mtbCptCode7.MaxLength = 5;
            this.mtbCptCode7.Name = "mtbCptCode7";
            this.mtbCptCode7.Size = new System.Drawing.Size(108, 20);
            this.mtbCptCode7.TabIndex = 38;
            this.mtbCptCode7.Validating += new System.ComponentModel.CancelEventHandler(this.mtbCptCode7_Validating);
            // 
            // lblCptCode7
            // 
            this.lblCptCode7.Location = new System.Drawing.Point(12, 215);
            this.lblCptCode7.Name = "lblCptCode7";
            this.lblCptCode7.Size = new System.Drawing.Size(69, 17);
            this.lblCptCode7.TabIndex = 37;
            this.lblCptCode7.Text = "CPT Code 7:";
            // 
            // mtbCptCode8
            // 
            this.mtbCptCode8.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbCptCode8.Location = new System.Drawing.Point(115, 243);
            this.mtbCptCode8.Mask = "";
            this.mtbCptCode8.MaxLength = 5;
            this.mtbCptCode8.Name = "mtbCptCode8";
            this.mtbCptCode8.Size = new System.Drawing.Size(108, 20);
            this.mtbCptCode8.TabIndex = 40;
            this.mtbCptCode8.Validating += new System.ComponentModel.CancelEventHandler(this.mtbCptCode8_Validating);
            // 
            // lblCptCode8
            // 
            this.lblCptCode8.Location = new System.Drawing.Point(12, 246);
            this.lblCptCode8.Name = "lblCptCode8";
            this.lblCptCode8.Size = new System.Drawing.Size(69, 17);
            this.lblCptCode8.TabIndex = 39;
            this.lblCptCode8.Text = "CPT Code 8:";
            // 
            // mtbCptCode9
            //
            this.mtbCptCode9.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbCptCode9.Location = new System.Drawing.Point(115, 276);
            this.mtbCptCode9.Mask = "";
            this.mtbCptCode9.MaxLength = 5;
            this.mtbCptCode9.Name = "mtbCptCode9";
            this.mtbCptCode9.Size = new System.Drawing.Size(108, 20);
            this.mtbCptCode9.TabIndex = 42;
            this.mtbCptCode9.Validating += new System.ComponentModel.CancelEventHandler(this.mtbCptCode9_Validating);
            // 
            // lblCptCode9
            // 
            this.lblCptCode9.Location = new System.Drawing.Point(12, 279);
            this.lblCptCode9.Name = "lblCptCode9";
            this.lblCptCode9.Size = new System.Drawing.Size(69, 17);
            this.lblCptCode9.TabIndex = 41;
            this.lblCptCode9.Text = "CPT Code 9:";
            // 
            // mtbCptCode10
            // 
            this.mtbCptCode10.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbCptCode10.Location = new System.Drawing.Point(115, 309);
            this.mtbCptCode10.Mask = "";
            this.mtbCptCode10.MaxLength = 5;
            this.mtbCptCode10.Name = "mtbCptCode10";
            this.mtbCptCode10.Size = new System.Drawing.Size(108, 20);
            this.mtbCptCode10.TabIndex = 44;
            this.mtbCptCode10.Validating += new System.ComponentModel.CancelEventHandler(this.mtbCptCode10_Validating);
            // 
            // lblCptCode10
            // 
            this.lblCptCode10.Location = new System.Drawing.Point(12, 312);
            this.lblCptCode10.Name = "lblCptCode10";
            this.lblCptCode10.Size = new System.Drawing.Size(82, 17);
            this.lblCptCode10.TabIndex = 43;
            this.lblCptCode10.Text = "CPT Code 10:";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(29, 352);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(65, 23);
            this.btnOK.TabIndex = 45;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(115, 352);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(65, 23);
            this.btnCancel.TabIndex = 46;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // CptCodesDetailsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(254, 411);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.mtbCptCode10);
            this.Controls.Add(this.lblCptCode10);
            this.Controls.Add(this.mtbCptCode9);
            this.Controls.Add(this.lblCptCode9);
            this.Controls.Add(this.mtbCptCode8);
            this.Controls.Add(this.lblCptCode8);
            this.Controls.Add(this.mtbCptCode7);
            this.Controls.Add(this.lblCptCode7);
            this.Controls.Add(this.mtbCptCode6);
            this.Controls.Add(this.lblCptCode6);
            this.Controls.Add(this.mtbCptCode5);
            this.Controls.Add(this.lblCptCode5);
            this.Controls.Add(this.mtbCptCode4);
            this.Controls.Add(this.lblCptCode4);
            this.Controls.Add(this.mtbCptCode3);
            this.Controls.Add(this.lblCptCode3);
            this.Controls.Add(this.mtbCptCode2);
            this.Controls.Add(this.lblCptCode2);
            this.Controls.Add(this.mtbCptCode1);
            this.Controls.Add(this.lblCptCode1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CptCodesDetailsView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CPT Codes";
            this.Shown += new System.EventHandler(this.CptCodesDetailsView_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCptCode1;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbCptCode1;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbCptCode2;
        private System.Windows.Forms.Label lblCptCode2;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbCptCode3;
        private System.Windows.Forms.Label lblCptCode3;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbCptCode4;
        private System.Windows.Forms.Label lblCptCode4;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbCptCode5;
        private System.Windows.Forms.Label lblCptCode5;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbCptCode6;
        private System.Windows.Forms.Label lblCptCode6;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbCptCode7;
        private System.Windows.Forms.Label lblCptCode7;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbCptCode8;
        private System.Windows.Forms.Label lblCptCode8;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbCptCode9;
        private System.Windows.Forms.Label lblCptCode9;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbCptCode10;
        private System.Windows.Forms.Label lblCptCode10;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}