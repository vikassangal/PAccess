namespace PatientAccess.UI.FUSNotes
{
    partial class AddFUSNoteView
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
            this.pnlAdd = new System.Windows.Forms.Panel();
            this.cmbMonth = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblMonth = new System.Windows.Forms.Label();
            this.dtpDate2 = new System.Windows.Forms.DateTimePicker();
            this.mtbDate2 = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblDate2 = new System.Windows.Forms.Label();
            this.dtpDate1 = new System.Windows.Forms.DateTimePicker();
            this.mtbDate1 = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblDate1 = new System.Windows.Forms.Label();
            this.mtbDollar2 = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblDollar2 = new System.Windows.Forms.Label();
            this.mtbDollar1 = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblDollar1 = new System.Windows.Forms.Label();
            this.mtbRemarks = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblRemarks = new System.Windows.Forms.Label();
            this.dtpWorklistDate = new System.Windows.Forms.DateTimePicker();
            this.mtbWorklistDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblWorklist = new System.Windows.Forms.Label();
            this.fusLineLabel = new PatientAccess.UI.CommonControls.LineLabel();
            this.cmbDescription = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.btnSelect = new System.Windows.Forms.Button();
            this.mtbActivityCode = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblActivityCode = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlAdd.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlAdd
            // 
            this.pnlAdd.BackColor = System.Drawing.Color.White;
            this.pnlAdd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAdd.Controls.Add( this.cmbMonth );
            this.pnlAdd.Controls.Add( this.lblMonth );
            this.pnlAdd.Controls.Add( this.dtpDate2 );
            this.pnlAdd.Controls.Add( this.mtbDate2 );
            this.pnlAdd.Controls.Add( this.lblDate2 );
            this.pnlAdd.Controls.Add( this.dtpDate1 );
            this.pnlAdd.Controls.Add( this.mtbDate1 );
            this.pnlAdd.Controls.Add( this.lblDate1 );
            this.pnlAdd.Controls.Add( this.mtbDollar2 );
            this.pnlAdd.Controls.Add( this.lblDollar2 );
            this.pnlAdd.Controls.Add( this.mtbDollar1 );
            this.pnlAdd.Controls.Add( this.lblDollar1 );
            this.pnlAdd.Controls.Add( this.mtbRemarks );
            this.pnlAdd.Controls.Add( this.lblRemarks );
            this.pnlAdd.Controls.Add( this.dtpWorklistDate );
            this.pnlAdd.Controls.Add( this.mtbWorklistDate );
            this.pnlAdd.Controls.Add( this.lblWorklist );
            this.pnlAdd.Controls.Add( this.fusLineLabel );
            this.pnlAdd.Controls.Add( this.cmbDescription );
            this.pnlAdd.Controls.Add( this.lblDescription );
            this.pnlAdd.Controls.Add( this.btnSelect );
            this.pnlAdd.Controls.Add( this.mtbActivityCode );
            this.pnlAdd.Controls.Add( this.lblActivityCode );
            this.pnlAdd.Location = new System.Drawing.Point( 6, 8 );
            this.pnlAdd.Name = "pnlAdd";
            this.pnlAdd.Size = new System.Drawing.Size( 496, 300 );
            this.pnlAdd.TabIndex = 0;
            // 
            // cmbMonth
            // 
            this.cmbMonth.Enabled = false;
            this.cmbMonth.Location = new System.Drawing.Point( 307, 265 );
            this.cmbMonth.Name = "cmbMonth";
            this.cmbMonth.Size = new System.Drawing.Size( 91, 21 );
            this.cmbMonth.TabIndex = 10;
            this.cmbMonth.SelectedIndexChanged += new System.EventHandler( this.cmbMonth_SelectedIndexChanged );
            // 
            // lblMonth
            // 
            this.lblMonth.AutoSize = true;
            this.lblMonth.Location = new System.Drawing.Point( 257, 265 );
            this.lblMonth.Name = "lblMonth";
            this.lblMonth.Size = new System.Drawing.Size( 40, 13 );
            this.lblMonth.TabIndex = 0;
            this.lblMonth.Text = "Month:";
            // 
            // dtpDate2
            // 
            this.dtpDate2.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dtpDate2.Checked = false;
            this.dtpDate2.Enabled = false;
            this.dtpDate2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate2.Location = new System.Drawing.Point( 376, 234 );
            this.dtpDate2.MinDate = new System.DateTime( 1800, 1, 1, 0, 0, 0, 0 );
            this.dtpDate2.Name = "dtpDate2";
            this.dtpDate2.Size = new System.Drawing.Size( 22, 20 );
            this.dtpDate2.TabIndex = 0;
            this.dtpDate2.TabStop = false;
            this.dtpDate2.CloseUp += new System.EventHandler( this.dtpDate2_CloseUp );
            // 
            // mtbDate2
            // 
            this.mtbDate2.Enabled = false;
            this.mtbDate2.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbDate2.KeyPressExpression = "^\\d*";
            this.mtbDate2.Location = new System.Drawing.Point( 307, 234 );
            this.mtbDate2.Mask = "  /  /";
            this.mtbDate2.MaxLength = 10;
            this.mtbDate2.Name = "mtbDate2";
            this.mtbDate2.Size = new System.Drawing.Size( 70, 20 );
            this.mtbDate2.TabIndex = 9;
            this.mtbDate2.ValidationExpression = "^\\d*";
            this.mtbDate2.Validating += new System.ComponentModel.CancelEventHandler( this.mtbDate2_Validating );
            // 
            // lblDate2
            // 
            this.lblDate2.AutoSize = true;
            this.lblDate2.Location = new System.Drawing.Point( 258, 238 );
            this.lblDate2.Name = "lblDate2";
            this.lblDate2.Size = new System.Drawing.Size( 39, 13 );
            this.lblDate2.TabIndex = 0;
            this.lblDate2.Text = "Date2:";
            // 
            // dtpDate1
            // 
            this.dtpDate1.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dtpDate1.Checked = false;
            this.dtpDate1.Enabled = false;
            this.dtpDate1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate1.Location = new System.Drawing.Point( 376, 204 );
            this.dtpDate1.MinDate = new System.DateTime( 1800, 1, 1, 0, 0, 0, 0 );
            this.dtpDate1.Name = "dtpDate1";
            this.dtpDate1.Size = new System.Drawing.Size( 22, 20 );
            this.dtpDate1.TabIndex = 0;
            this.dtpDate1.TabStop = false;
            this.dtpDate1.CloseUp += new System.EventHandler( this.dtpDate1_CloseUp );
            // 
            // mtbDate1
            // 
            this.mtbDate1.Enabled = false;
            this.mtbDate1.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbDate1.KeyPressExpression = "^\\d*";
            this.mtbDate1.Location = new System.Drawing.Point( 307, 204 );
            this.mtbDate1.Mask = "  /  /";
            this.mtbDate1.MaxLength = 10;
            this.mtbDate1.Name = "mtbDate1";
            this.mtbDate1.Size = new System.Drawing.Size( 70, 20 );
            this.mtbDate1.TabIndex = 7;
            this.mtbDate1.ValidationExpression = "^\\d*";
            this.mtbDate1.Validating += new System.ComponentModel.CancelEventHandler( this.mtbDate1_Validating );
            // 
            // lblDate1
            // 
            this.lblDate1.AutoSize = true;
            this.lblDate1.Location = new System.Drawing.Point( 258, 208 );
            this.lblDate1.Name = "lblDate1";
            this.lblDate1.Size = new System.Drawing.Size( 39, 13 );
            this.lblDate1.TabIndex = 0;
            this.lblDate1.Text = "Date1:";
            // 
            // mtbDollar2
            // 
            this.mtbDollar2.Enabled = false;
            this.mtbDollar2.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbDollar2.KeyPressExpression = "^[0-9]{0,7}(\\.[0-9]{0,2})?$";
            this.mtbDollar2.Location = new System.Drawing.Point( 107, 235 );
            this.mtbDollar2.Mask = "";
            this.mtbDollar2.MaxLength = 10;
            this.mtbDollar2.Name = "mtbDollar2";
            this.mtbDollar2.Size = new System.Drawing.Size( 89, 20 );
            this.mtbDollar2.TabIndex = 8;
            this.mtbDollar2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbDollar2.ValidationExpression = "^[0-9]{0,7}(\\.[0-9]{0,2})?$";
            this.mtbDollar2.Validating += new System.ComponentModel.CancelEventHandler( this.mtbDollar2_Validating );
            // 
            // lblDollar2
            // 
            this.lblDollar2.AutoSize = true;
            this.lblDollar2.Location = new System.Drawing.Point( 15, 238 );
            this.lblDollar2.Name = "lblDollar2";
            this.lblDollar2.Size = new System.Drawing.Size( 82, 13 );
            this.lblDollar2.TabIndex = 0;
            this.lblDollar2.Text = "Dollar Amount2:";
            // 
            // mtbDollar1
            // 
            this.mtbDollar1.Enabled = false;
            this.mtbDollar1.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbDollar1.KeyPressExpression = "^[0-9]{0,7}(\\.[0-9]{0,2})?$";
            this.mtbDollar1.Location = new System.Drawing.Point( 107, 203 );
            this.mtbDollar1.Mask = "";
            this.mtbDollar1.MaxLength = 10;
            this.mtbDollar1.Name = "mtbDollar1";
            this.mtbDollar1.Size = new System.Drawing.Size( 89, 20 );
            this.mtbDollar1.TabIndex = 6;
            this.mtbDollar1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbDollar1.ValidationExpression = "^[0-9]{0,7}(\\.[0-9]{0,2})?$";
            this.mtbDollar1.Validating += new System.ComponentModel.CancelEventHandler( this.mtbDollar1_Validating );
            // 
            // lblDollar1
            // 
            this.lblDollar1.AutoSize = true;
            this.lblDollar1.Location = new System.Drawing.Point( 15, 206 );
            this.lblDollar1.Name = "lblDollar1";
            this.lblDollar1.Size = new System.Drawing.Size( 82, 13 );
            this.lblDollar1.TabIndex = 0;
            this.lblDollar1.Text = "Dollar Amount1:";
            // 
            // mtbRemarks
            // 
            this.mtbRemarks.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbRemarks.Enabled = false;
            this.mtbRemarks.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbRemarks.HideSelection = false;
            this.mtbRemarks.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbRemarks.Location = new System.Drawing.Point( 107, 130 );
            this.mtbRemarks.Mask = "";
            this.mtbRemarks.Multiline = true;
            this.mtbRemarks.Name = "mtbRemarks";
            this.mtbRemarks.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.mtbRemarks.Size = new System.Drawing.Size( 343, 60 );
            this.mtbRemarks.TabIndex = 5;
            this.mtbRemarks.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbRemarks.Validating += new System.ComponentModel.CancelEventHandler( this.mtbRemarks_Validating );            
            // 
            // lblRemarks
            // 
            this.lblRemarks.AutoSize = true;
            this.lblRemarks.Location = new System.Drawing.Point( 45, 130 );
            this.lblRemarks.Name = "lblRemarks";
            this.lblRemarks.Size = new System.Drawing.Size( 52, 13 );
            this.lblRemarks.TabIndex = 0;
            this.lblRemarks.Text = "Remarks:";
            // 
            // dtpWorklistDate
            // 
            this.dtpWorklistDate.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dtpWorklistDate.Checked = false;
            this.dtpWorklistDate.Enabled = false;
            this.dtpWorklistDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpWorklistDate.Location = new System.Drawing.Point( 174, 98 );
            this.dtpWorklistDate.MinDate = new System.DateTime( 1800, 1, 1, 0, 0, 0, 0 );
            this.dtpWorklistDate.Name = "dtpWorklistDate";
            this.dtpWorklistDate.Size = new System.Drawing.Size( 22, 20 );
            this.dtpWorklistDate.TabIndex = 0;
            this.dtpWorklistDate.TabStop = false;
            this.dtpWorklistDate.CloseUp += new System.EventHandler( this.dtpWorklistDate_CloseUp );
            // 
            // mtbWorklistDate
            // 
            this.mtbWorklistDate.Enabled = false;
            this.mtbWorklistDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbWorklistDate.KeyPressExpression = "^\\d*";
            this.mtbWorklistDate.Location = new System.Drawing.Point( 107, 98 );
            this.mtbWorklistDate.Mask = "  /  /";
            this.mtbWorklistDate.MaxLength = 10;
            this.mtbWorklistDate.Name = "mtbWorklistDate";
            this.mtbWorklistDate.Size = new System.Drawing.Size( 70, 20 );
            this.mtbWorklistDate.TabIndex = 4;
            this.mtbWorklistDate.ValidationExpression = "^\\d*";
            this.mtbWorklistDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbWorklistDate_Validating );
            // 
            // lblWorklist
            // 
            this.lblWorklist.AutoSize = true;
            this.lblWorklist.Location = new System.Drawing.Point( 49, 101 );
            this.lblWorklist.Name = "lblWorklist";
            this.lblWorklist.Size = new System.Drawing.Size( 48, 13 );
            this.lblWorklist.TabIndex = 0;
            this.lblWorklist.Text = "Worklist:";
            // 
            // fusLineLabel
            // 
            this.fusLineLabel.Caption = "";
            this.fusLineLabel.Location = new System.Drawing.Point( 11, 74 );
            this.fusLineLabel.Name = "fusLineLabel";
            this.fusLineLabel.Size = new System.Drawing.Size( 472, 18 );
            this.fusLineLabel.TabIndex = 0;
            this.fusLineLabel.TabStop = false;
            // 
            // cmbDescription
            // 
            this.cmbDescription.Location = new System.Drawing.Point( 107, 47 );
            this.cmbDescription.Name = "cmbDescription";
            this.cmbDescription.Size = new System.Drawing.Size( 344, 21 );
            this.cmbDescription.TabIndex = 3;
            this.cmbDescription.SelectedIndexChanged += new System.EventHandler( this.cmbDescription_SelectedIndexChanged );
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point( 34, 50 );
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size( 63, 13 );
            this.lblDescription.TabIndex = 0;
            this.lblDescription.Text = "Description:";
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point( 198, 14 );
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size( 75, 23 );
            this.btnSelect.TabIndex = 2;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler( this.btnSelect_Click );
            // 
            // mtbActivityCode
            // 
            this.mtbActivityCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbActivityCode.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbActivityCode.KeyPressExpression = "^[a-zA-Z][ a-zA-Z0-9\\ -]*$";
            this.mtbActivityCode.Location = new System.Drawing.Point( 107, 15 );
            this.mtbActivityCode.Mask = "";
            this.mtbActivityCode.MaxLength = 5;
            this.mtbActivityCode.Name = "mtbActivityCode";
            this.mtbActivityCode.Size = new System.Drawing.Size( 60, 20 );
            this.mtbActivityCode.TabIndex = 1;
            this.mtbActivityCode.ValidationExpression = "^[a-zA-Z][ a-zA-Z0-9\\ -]*$";
            this.mtbActivityCode.TextChanged += new System.EventHandler( this.mtbActivityCode_TextChanged );
            this.mtbActivityCode.Click += new System.EventHandler( this.mtbActivityCode_Click );
            // 
            // lblActivityCode
            // 
            this.lblActivityCode.AutoSize = true;
            this.lblActivityCode.Location = new System.Drawing.Point( 26, 18 );
            this.lblActivityCode.Name = "lblActivityCode";
            this.lblActivityCode.Size = new System.Drawing.Size( 71, 13 );
            this.lblActivityCode.TabIndex = 0;
            this.lblActivityCode.Text = "Activity code:";
            // 
            // btnOK
            // 
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point( 314, 318 );
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size( 92, 23 );
            this.btnOK.TabIndex = 11;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler( this.btnOK_Click );
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point( 413, 317 );
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size( 89, 24 );
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler( this.btnCancel_Click );
            // 
            // AddFUSNoteView
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size( 509, 348 );
            this.Controls.Add( this.btnCancel );
            this.Controls.Add( this.btnOK );
            this.Controls.Add( this.pnlAdd );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddFUSNoteView";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add FUS Note";
            this.Load += new System.EventHandler( this.AddFUSNote_Load );
            this.Enter += new System.EventHandler( this.AddFUSNoteView_Enter );
            this.Leave += new System.EventHandler( this.AddFUSNote_Leave );
            this.pnlAdd.ResumeLayout( false );
            this.pnlAdd.PerformLayout();
            this.ResumeLayout( false );

        }

        #endregion

        #region Data Elements

        private System.Windows.Forms.Panel pnlAdd;
        
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;

        private PatientAccess.UI.CommonControls.LineLabel fusLineLabel;

        private PatientAccess.UI.CommonControls.PatientAccessComboBox cmbDescription;
        private PatientAccess.UI.CommonControls.PatientAccessComboBox cmbMonth;

        private System.Windows.Forms.Label lblActivityCode;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblWorklist;
        private System.Windows.Forms.Label lblRemarks;
        private System.Windows.Forms.Label lblDollar1;
        private System.Windows.Forms.Label lblDate1;
        private System.Windows.Forms.Label lblDollar2;
        private System.Windows.Forms.Label lblDate2;
        private System.Windows.Forms.Label lblMonth;

        private System.Windows.Forms.DateTimePicker dtpWorklistDate;
        private System.Windows.Forms.DateTimePicker dtpDate1;
        private System.Windows.Forms.DateTimePicker dtpDate2;

        private Extensions.UI.Winforms.MaskedEditTextBox mtbActivityCode;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbWorklistDate;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbRemarks;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbDollar1;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbDate1;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbDollar2;
        private Extensions.UI.Winforms.MaskedEditTextBox mtbDate2;

        #endregion

    }
}