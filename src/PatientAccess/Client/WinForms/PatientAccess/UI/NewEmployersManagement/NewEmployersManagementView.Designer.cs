using System;
using Extensions.UI.Winforms;


namespace PatientAccess.UI.NewEmployersManagement
{
    internal partial class NewEmployersManagementView
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.selectedEmployerLabel = new System.Windows.Forms.Label();
            this.selectedEmployerNationalIdLabel = new System.Windows.Forms.Label();
            this.selectedEmployerAddressLabel = new System.Windows.Forms.Label();
            this.selectedEmployerNameTextBox = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.selectedEmployerNationalIdTextBox = new System.Windows.Forms.MaskedTextBox();
            this.selectedEmployerAddressTextBox = new System.Windows.Forms.TextBox();
            this.masterEmployerListGroupBox = new System.Windows.Forms.GroupBox();
            this.noAddressesLabel = new System.Windows.Forms.Label();
            this.MasterListMessageLabel = new System.Windows.Forms.Label();
            this.employerSearchLabel = new System.Windows.Forms.Label();
            this.employerAddressLabel = new System.Windows.Forms.Label();
            this.masterEmployerAddressesDataGridView = new System.Windows.Forms.DataGridView();
            this.masterEmployerListDataGridView = new System.Windows.Forms.DataGridView();
            this.masterEmployerSearchTextBox = new System.Windows.Forms.MaskedTextBox();
            this.newEmployerDataGridView = new System.Windows.Forms.DataGridView();
            this.selectedEmployerPhoneTextBox = new System.Windows.Forms.MaskedTextBox();
            this.selectedEmployerPhoneLabel = new System.Windows.Forms.Label();
            this.noNewEmployersMessageLabel = new System.Windows.Forms.Label();
            this.phoneNumberErrorToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.featureIsLockedMessageLabel = new System.Windows.Forms.Label();
            this.stepOneLabel = new System.Windows.Forms.Label();
            this.stepTwoLabel = new System.Windows.Forms.Label();
            this.stepThreeLabel = new System.Windows.Forms.Label();
            this.moveButtonInstructionsLabel = new System.Windows.Forms.Label();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextLabel = new PatientAccess.UI.UserContextView();
            this.clearButton = new PatientAccess.UI.CommonControls.LoggingButton();
            this.employerInfoColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.newEmployerNationalIDColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.employerNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nationalIDColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.masterEmployerSearchButton = new PatientAccess.UI.CommonControls.LoggingButton();
            this.cancelButton = new PatientAccess.UI.CommonControls.LoggingButton();
            this.finishButton = new PatientAccess.UI.CommonControls.LoggingButton();
            this.undoButton = new PatientAccess.UI.CommonControls.LoggingButton();
            this.moveAddressAndPhoneButton = new PatientAccess.UI.CommonControls.LoggingButton();
            this.editAddressButton = new PatientAccess.UI.CommonControls.LoggingButton();
            this.deleteButton = new PatientAccess.UI.CommonControls.LoggingButton();
            this.moveAllInfoButton = new PatientAccess.UI.CommonControls.LoggingButton();
            this.masterEmployerListGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.masterEmployerAddressesDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.masterEmployerListDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.newEmployerDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // selectedEmployerLabel
            // 
            this.selectedEmployerLabel.AutoSize = true;
            this.selectedEmployerLabel.Location = new System.Drawing.Point(12, 289);
            this.selectedEmployerLabel.Name = "selectedEmployerLabel";
            this.selectedEmployerLabel.Size = new System.Drawing.Size(53, 13);
            this.selectedEmployerLabel.TabIndex = 4;
            this.selectedEmployerLabel.Text = "Employer:";
            // 
            // selectedEmployerNationalIdLabel
            // 
            this.selectedEmployerNationalIdLabel.AutoSize = true;
            this.selectedEmployerNationalIdLabel.Location = new System.Drawing.Point(12, 319);
            this.selectedEmployerNationalIdLabel.Name = "selectedEmployerNationalIdLabel";
            this.selectedEmployerNationalIdLabel.Size = new System.Drawing.Size(63, 13);
            this.selectedEmployerNationalIdLabel.TabIndex = 7;
            this.selectedEmployerNationalIdLabel.Text = "National ID:";
            // 
            // selectedEmployerAddressLabel
            // 
            this.selectedEmployerAddressLabel.AutoSize = true;
            this.selectedEmployerAddressLabel.Location = new System.Drawing.Point(12, 377);
            this.selectedEmployerAddressLabel.Name = "selectedEmployerAddressLabel";
            this.selectedEmployerAddressLabel.Size = new System.Drawing.Size(48, 13);
            this.selectedEmployerAddressLabel.TabIndex = 10;
            this.selectedEmployerAddressLabel.Text = "Address:";
            // 
            // selectedEmployerNameTextBox
            // 
            this.selectedEmployerNameTextBox.Location = new System.Drawing.Point(122, 285);
            this.selectedEmployerNameTextBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.selectedEmployerNameTextBox.Mask = "";
            this.selectedEmployerNameTextBox.Name = "selectedEmployerNameTextBox";
            this.selectedEmployerNameTextBox.MaxLength = 25;
            this.selectedEmployerNameTextBox.Size = new System.Drawing.Size(280, 20);
            this.selectedEmployerNameTextBox.TabIndex = 5;
            this.selectedEmployerNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.SelectedEmployerNameTextBoxOnValidating);
            this.selectedEmployerNameTextBox.Validated += new System.EventHandler(this.SelectedEmployerNameTextBoxOnValidated);
            this.selectedEmployerNameTextBox.Click += new System.EventHandler(this.SelectedEmployerNameTextBoxOnClick);
            // 
            // selectedEmployerNationalIdTextBox
            // 
            this.selectedEmployerNationalIdTextBox.AllowPromptAsInput = false;
            this.selectedEmployerNationalIdTextBox.CutCopyMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.selectedEmployerNationalIdTextBox.HidePromptOnLeave = true;
            this.selectedEmployerNationalIdTextBox.Location = new System.Drawing.Point(122, 315);
            this.selectedEmployerNationalIdTextBox.Mask = ">aaaaaaaaaa";
            this.selectedEmployerNationalIdTextBox.Name = "selectedEmployerNationalIdTextBox";
            this.selectedEmployerNationalIdTextBox.PromptChar = ' ';
            this.selectedEmployerNationalIdTextBox.ResetOnPrompt = false;
            this.selectedEmployerNationalIdTextBox.Size = new System.Drawing.Size(130, 20);
            this.selectedEmployerNationalIdTextBox.TabIndex = 6;
            this.selectedEmployerNationalIdTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.selectedEmployerNationalIdTextBox.Validated += new System.EventHandler(this.SelectedEmployerNationalIdTextBoxOnValidated);
            this.selectedEmployerNationalIdTextBox.Click += new System.EventHandler(this.SelectedEmployerNationalIdTextBoxOnClick);
            // 
            // selectedEmployerAddressTextBox
            // 
            this.selectedEmployerAddressTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.selectedEmployerAddressTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.selectedEmployerAddressTextBox.Location = new System.Drawing.Point(122, 377);
            this.selectedEmployerAddressTextBox.Multiline = true;
            this.selectedEmployerAddressTextBox.Name = "selectedEmployerAddressTextBox";
            this.selectedEmployerAddressTextBox.ReadOnly = true;
            this.selectedEmployerAddressTextBox.Size = new System.Drawing.Size(337, 48);
            this.selectedEmployerAddressTextBox.TabIndex = 11;
            this.selectedEmployerAddressTextBox.TabStop = false;
            // 
            // masterEmployerListGroupBox
            // 
            this.masterEmployerListGroupBox.Controls.Add(this.noAddressesLabel);
            this.masterEmployerListGroupBox.Controls.Add(this.MasterListMessageLabel);
            this.masterEmployerListGroupBox.Controls.Add(this.employerSearchLabel);
            this.masterEmployerListGroupBox.Controls.Add(this.employerAddressLabel);
            this.masterEmployerListGroupBox.Controls.Add(this.masterEmployerAddressesDataGridView);
            this.masterEmployerListGroupBox.Controls.Add(this.masterEmployerListDataGridView);
            this.masterEmployerListGroupBox.Controls.Add(this.masterEmployerSearchTextBox);
            this.masterEmployerListGroupBox.Controls.Add(this.masterEmployerSearchButton);
            this.masterEmployerListGroupBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.masterEmployerListGroupBox.Location = new System.Drawing.Point(536, 29);
            this.masterEmployerListGroupBox.Name = "masterEmployerListGroupBox";
            this.masterEmployerListGroupBox.Size = new System.Drawing.Size(473, 554);
            this.masterEmployerListGroupBox.TabIndex = 20;
            this.masterEmployerListGroupBox.TabStop = false;
            this.masterEmployerListGroupBox.Text = "Master Employer List";
            // 
            // noAddressesLabel
            // 
            this.noAddressesLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.noAddressesLabel.Location = new System.Drawing.Point(24, 330);
            this.noAddressesLabel.Name = "noAddressesLabel";
            this.noAddressesLabel.Size = new System.Drawing.Size(423, 200);
            this.noAddressesLabel.TabIndex = 5;
            this.noAddressesLabel.Text = "No items found";
            this.noAddressesLabel.Visible = false;
            // 
            // MasterListMessageLabel
            // 
            this.MasterListMessageLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MasterListMessageLabel.Location = new System.Drawing.Point(24, 76);
            this.MasterListMessageLabel.Name = "MasterListMessageLabel";
            this.MasterListMessageLabel.Size = new System.Drawing.Size(423, 210);
            this.MasterListMessageLabel.TabIndex = 3;
            this.MasterListMessageLabel.Visible = false;
            // 
            // employerSearchLabel
            // 
            this.employerSearchLabel.AutoSize = true;
            this.employerSearchLabel.Location = new System.Drawing.Point(21, 29);
            this.employerSearchLabel.Name = "employerSearchLabel";
            this.employerSearchLabel.Size = new System.Drawing.Size(50, 13);
            this.employerSearchLabel.TabIndex = 0;
            this.employerSearchLabel.Text = "Employer";
            // 
            // employerAddressLabel
            // 
            this.employerAddressLabel.AutoSize = true;
            this.employerAddressLabel.Location = new System.Drawing.Point(21, 313);
            this.employerAddressLabel.Name = "employerAddressLabel";
            this.employerAddressLabel.Size = new System.Drawing.Size(102, 13);
            this.employerAddressLabel.TabIndex = 4;
            this.employerAddressLabel.Text = "Employer Addresses";
            // 
            // masterEmployerAddressesDataGridView
            // 
            this.masterEmployerAddressesDataGridView.AllowUserToAddRows = false;
            this.masterEmployerAddressesDataGridView.AllowUserToDeleteRows = false;
            this.masterEmployerAddressesDataGridView.AllowUserToResizeColumns = false;
            this.masterEmployerAddressesDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.masterEmployerAddressesDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.masterEmployerAddressesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.masterEmployerAddressesDataGridView.ColumnHeadersVisible = false;
            this.masterEmployerAddressesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn5});
            this.masterEmployerAddressesDataGridView.GridColor = System.Drawing.Color.DarkGray;
            this.masterEmployerAddressesDataGridView.Location = new System.Drawing.Point(24, 330);
            this.masterEmployerAddressesDataGridView.Margin = new System.Windows.Forms.Padding(0);
            this.masterEmployerAddressesDataGridView.MultiSelect = false;
            this.masterEmployerAddressesDataGridView.Name = "masterEmployerAddressesDataGridView";
            this.masterEmployerAddressesDataGridView.ReadOnly = true;
            this.masterEmployerAddressesDataGridView.RowHeadersVisible = false;
            this.masterEmployerAddressesDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.masterEmployerAddressesDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.masterEmployerAddressesDataGridView.RowTemplate.Height = 45;
            this.masterEmployerAddressesDataGridView.RowTemplate.ReadOnly = true;
            this.masterEmployerAddressesDataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.masterEmployerAddressesDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.masterEmployerAddressesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.masterEmployerAddressesDataGridView.Size = new System.Drawing.Size(423, 200);
            this.masterEmployerAddressesDataGridView.StandardTab = true;
            this.masterEmployerAddressesDataGridView.TabIndex = 4;
            this.masterEmployerAddressesDataGridView.Enter += new System.EventHandler(this.MasterEmployerAddressesDataGridViewOnEnter);
            // 
            // masterEmployerListDataGridView
            // 
            this.masterEmployerListDataGridView.AllowUserToAddRows = false;
            this.masterEmployerListDataGridView.AllowUserToDeleteRows = false;
            this.masterEmployerListDataGridView.AllowUserToResizeColumns = false;
            this.masterEmployerListDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.masterEmployerListDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.masterEmployerListDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.masterEmployerListDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.employerNameColumn,
            this.nationalIDColumn});
            this.masterEmployerListDataGridView.GridColor = System.Drawing.Color.DarkGray;
            this.masterEmployerListDataGridView.Location = new System.Drawing.Point(24, 76);
            this.masterEmployerListDataGridView.Margin = new System.Windows.Forms.Padding(0);
            this.masterEmployerListDataGridView.MultiSelect = false;
            this.masterEmployerListDataGridView.Name = "masterEmployerListDataGridView";
            this.masterEmployerListDataGridView.ReadOnly = true;
            this.masterEmployerListDataGridView.RowHeadersVisible = false;
            this.masterEmployerListDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.masterEmployerListDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.masterEmployerListDataGridView.RowTemplate.ReadOnly = true;
            this.masterEmployerListDataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.masterEmployerListDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.masterEmployerListDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.masterEmployerListDataGridView.Size = new System.Drawing.Size(423, 210);
            this.masterEmployerListDataGridView.StandardTab = true;
            this.masterEmployerListDataGridView.TabIndex = 3;
            this.masterEmployerListDataGridView.SelectionChanged += new System.EventHandler(this.MasterEmployerListDataGridViewOnSelectionChanged);
            // 
            // masterEmployerSearchTextBox
            // 
            this.masterEmployerSearchTextBox.CutCopyMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.masterEmployerSearchTextBox.HidePromptOnLeave = true;
            this.masterEmployerSearchTextBox.Location = new System.Drawing.Point(24, 46);
            this.masterEmployerSearchTextBox.Mask = ">Laaaaaaaaaaaaaaaaaaaaaaaa";
            this.masterEmployerSearchTextBox.Name = "masterEmployerSearchTextBox";
            this.masterEmployerSearchTextBox.PromptChar = ' ';
            this.masterEmployerSearchTextBox.RejectInputOnFirstFailure = true;
            this.masterEmployerSearchTextBox.ResetOnPrompt = false;
            this.masterEmployerSearchTextBox.ResetOnSpace = false;
            this.masterEmployerSearchTextBox.Size = new System.Drawing.Size(334, 20);
            this.masterEmployerSearchTextBox.TabIndex = 1;
            this.masterEmployerSearchTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.masterEmployerSearchTextBox.TextChanged += new System.EventHandler(this.MasterEmployerSearchTextBoxOnTextChanged);
            this.masterEmployerSearchTextBox.Click += new System.EventHandler(this.MasterEmployerSearchTextBoxOnClick);            
            this.masterEmployerSearchTextBox.KeyDown+=new System.Windows.Forms.KeyEventHandler(this.MasterEmployerSearchTextBoxOnKeyDown);

            // 
            // newEmployerDataGridView
            // 
            this.newEmployerDataGridView.AllowUserToAddRows = false;
            this.newEmployerDataGridView.AllowUserToDeleteRows = false;
            this.newEmployerDataGridView.AllowUserToResizeColumns = false;
            this.newEmployerDataGridView.AllowUserToResizeRows = false;
            this.newEmployerDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.newEmployerDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.newEmployerDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.newEmployerDataGridView.ColumnHeadersVisible = false;
            this.newEmployerDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.employerInfoColumn,
            this.newEmployerNationalIDColumn});
            this.newEmployerDataGridView.GridColor = System.Drawing.Color.DarkGray;
            this.newEmployerDataGridView.Location = new System.Drawing.Point(15, 44);
            this.newEmployerDataGridView.Margin = new System.Windows.Forms.Padding(0);
            this.newEmployerDataGridView.MultiSelect = false;
            this.newEmployerDataGridView.Name = "newEmployerDataGridView";
            this.newEmployerDataGridView.ReadOnly = true;
            this.newEmployerDataGridView.RowHeadersVisible = false;
            this.newEmployerDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.newEmployerDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle8;
            this.newEmployerDataGridView.RowTemplate.Height = 95;
            this.newEmployerDataGridView.RowTemplate.ReadOnly = true;
            this.newEmployerDataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.newEmployerDataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.newEmployerDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.newEmployerDataGridView.ShowCellErrors = false;
            this.newEmployerDataGridView.ShowCellToolTips = false;
            this.newEmployerDataGridView.ShowEditingIcon = false;
            this.newEmployerDataGridView.ShowRowErrors = false;
            this.newEmployerDataGridView.Size = new System.Drawing.Size(443, 203);
            this.newEmployerDataGridView.StandardTab = true;
            this.newEmployerDataGridView.TabIndex = 1;
            this.newEmployerDataGridView.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.NewEmployerDataGridViewOnRowPrePaint);
            this.newEmployerDataGridView.SelectionChanged += new System.EventHandler(this.NewEmployerDataGridViewOnSelectionChanged);
            // 
            // selectedEmployerPhoneTextBox
            // 
            this.selectedEmployerPhoneTextBox.AllowPromptAsInput = false;
            this.selectedEmployerPhoneTextBox.HidePromptOnLeave = true;
            this.selectedEmployerPhoneTextBox.Location = new System.Drawing.Point(122, 435);
            this.selectedEmployerPhoneTextBox.Mask = "(000) 000-0000";
            this.selectedEmployerPhoneTextBox.Name = "selectedEmployerPhoneTextBox";
            this.selectedEmployerPhoneTextBox.PromptChar = ' ';
            this.selectedEmployerPhoneTextBox.ResetOnPrompt = false;
            this.selectedEmployerPhoneTextBox.ResetOnSpace = false;
            this.selectedEmployerPhoneTextBox.Size = new System.Drawing.Size(100, 20);
            this.selectedEmployerPhoneTextBox.TabIndex = 13;
            this.selectedEmployerPhoneTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.selectedEmployerPhoneTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.SelectedEmployerPhoneTextBoxOnValidating);
            this.selectedEmployerPhoneTextBox.Validated += new System.EventHandler(this.SelectedEmployerPhoneTextBoxOnValidated);
            this.selectedEmployerPhoneTextBox.MaskInputRejected += new System.Windows.Forms.MaskInputRejectedEventHandler(this.SelectedEmployerPhoneTextBoxOnMaskInputRejected);
            this.selectedEmployerPhoneTextBox.Click += new System.EventHandler(this.SelectedEmployerPhoneTextBoxOnClick);
            // 
            // selectedEmployerPhoneLabel
            // 
            this.selectedEmployerPhoneLabel.AutoSize = true;
            this.selectedEmployerPhoneLabel.Location = new System.Drawing.Point(12, 439);
            this.selectedEmployerPhoneLabel.Name = "selectedEmployerPhoneLabel";
            this.selectedEmployerPhoneLabel.Size = new System.Drawing.Size(97, 13);
            this.selectedEmployerPhoneLabel.TabIndex = 12;
            this.selectedEmployerPhoneLabel.Text = "Phone for Address:";
            // 
            // noNewEmployersMessageLabel
            // 
            this.noNewEmployersMessageLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.noNewEmployersMessageLabel.Location = new System.Drawing.Point(15, 44);
            this.noNewEmployersMessageLabel.Name = "noNewEmployersMessageLabel";
            this.noNewEmployersMessageLabel.Size = new System.Drawing.Size(443, 203);
            this.noNewEmployersMessageLabel.TabIndex = 2;
            this.noNewEmployersMessageLabel.Visible = false;
            // 
            // phoneNumberErrorToolTip
            // 
            this.phoneNumberErrorToolTip.ToolTipTitle = "Invalid Input";
            // 
            // featureIsLockedMessageLabel
            // 
            this.featureIsLockedMessageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.featureIsLockedMessageLabel.Location = new System.Drawing.Point(12, 26);
            this.featureIsLockedMessageLabel.Name = "featureIsLockedMessageLabel";
            this.featureIsLockedMessageLabel.Size = new System.Drawing.Size(999, 559);
            this.featureIsLockedMessageLabel.TabIndex = 0;
            this.featureIsLockedMessageLabel.Text = "\r\nThis function is unavailable because another user is accessing it for this faci" +
                "lity. Please try again later.";
            this.featureIsLockedMessageLabel.Visible = false;
            // 
            // stepOneLabel
            // 
            this.stepOneLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stepOneLabel.Location = new System.Drawing.Point(12, 27);
            this.stepOneLabel.Name = "stepOneLabel";
            this.stepOneLabel.Size = new System.Drawing.Size(235, 16);
            this.stepOneLabel.TabIndex = 1;
            this.stepOneLabel.Text = "Step 1: Select new employer.";
            this.stepOneLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // stepTwoLabel
            // 
            this.stepTwoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stepTwoLabel.Location = new System.Drawing.Point(12, 266);
            this.stepTwoLabel.Name = "stepTwoLabel";
            this.stepTwoLabel.Size = new System.Drawing.Size(334, 16);
            this.stepTwoLabel.TabIndex = 3;
            this.stepTwoLabel.Text = "Step 2 (optional): Edit selected new employer information.";
            // 
            // stepThreeLabel
            // 
            this.stepThreeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stepThreeLabel.Location = new System.Drawing.Point(12, 476);
            this.stepThreeLabel.Margin = new System.Windows.Forms.Padding(0);
            this.stepThreeLabel.Name = "stepThreeLabel";
            this.stepThreeLabel.Size = new System.Drawing.Size(450, 30);
            this.stepThreeLabel.TabIndex = 14;
            this.stepThreeLabel.Text = "Step 3: Move selected new employer information to Master Employer List, or delete" +
                " selected new employer.";
            // 
            // moveButtonInstructionsLabel
            // 
            this.moveButtonInstructionsLabel.BackColor = System.Drawing.SystemColors.Window;
            this.moveButtonInstructionsLabel.Location = new System.Drawing.Point(13, 510);
            this.moveButtonInstructionsLabel.Name = "moveButtonInstructionsLabel";
            this.moveButtonInstructionsLabel.Size = new System.Drawing.Size(447, 40);
            this.moveButtonInstructionsLabel.TabIndex = 15;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle9;
            this.dataGridViewTextBoxColumn1.HeaderText = "Employer";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopRight;
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle10;
            this.dataGridViewTextBoxColumn2.FillWeight = 119.2399F;
            this.dataGridViewTextBoxColumn2.HeaderText = "National ID";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopRight;
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle11;
            this.dataGridViewTextBoxColumn3.FillWeight = 80.76009F;
            this.dataGridViewTextBoxColumn3.HeaderText = "National ID";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridViewTextBoxColumn4.DefaultCellStyle = dataGridViewCellStyle12;
            this.dataGridViewTextBoxColumn4.HeaderText = "Employer";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.dataGridViewTextBoxColumn5.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewTextBoxColumn5.HeaderText = "Employer";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // contextLabel
            // 
            this.contextLabel.BackColor = System.Drawing.SystemColors.Control;
            this.contextLabel.Description = "Manage New Employers";
            this.contextLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.contextLabel.Location = new System.Drawing.Point(0, 0);
            this.contextLabel.Model = null;
            this.contextLabel.Name = "contextLabel";
            this.contextLabel.Size = new System.Drawing.Size(1024, 23);
            this.contextLabel.TabIndex = 0;
            // 
            // clearButton
            // 
            this.clearButton.BackColor = System.Drawing.SystemColors.Control;
            this.clearButton.Location = new System.Drawing.Point(211, 344);
            this.clearButton.Message = null;
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 23);
            this.clearButton.TabIndex = 9;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.ClearButtonOnClick);
            // 
            // employerInfoColumn
            // 
            this.employerInfoColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.employerInfoColumn.DefaultCellStyle = dataGridViewCellStyle6;
            this.employerInfoColumn.HeaderText = "Employer";
            this.employerInfoColumn.Name = "employerInfoColumn";
            this.employerInfoColumn.ReadOnly = true;
            // 
            // newEmployerNationalIDColumn
            // 
            this.newEmployerNationalIDColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopRight;
            this.newEmployerNationalIDColumn.DefaultCellStyle = dataGridViewCellStyle7;
            this.newEmployerNationalIDColumn.HeaderText = "National ID";
            this.newEmployerNationalIDColumn.Name = "newEmployerNationalIDColumn";
            this.newEmployerNationalIDColumn.ReadOnly = true;
            // 
            // employerNameColumn
            // 
            this.employerNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            this.employerNameColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.employerNameColumn.FillWeight = 142.9929F;
            this.employerNameColumn.HeaderText = "Employer";
            this.employerNameColumn.Name = "employerNameColumn";
            this.employerNameColumn.ReadOnly = true;
            // 
            // nationalIDColumn
            // 
            this.nationalIDColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopRight;
            this.nationalIDColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.nationalIDColumn.FillWeight = 57.00714F;
            this.nationalIDColumn.HeaderText = "National ID";
            this.nationalIDColumn.Name = "nationalIDColumn";
            this.nationalIDColumn.ReadOnly = true;
            // 
            // masterEmployerSearchButton
            // 
            this.masterEmployerSearchButton.BackColor = System.Drawing.SystemColors.Control;
            this.masterEmployerSearchButton.Location = new System.Drawing.Point(364, 45);
            this.masterEmployerSearchButton.Message = null;
            this.masterEmployerSearchButton.Name = "masterEmployerSearchButton";
            this.masterEmployerSearchButton.Size = new System.Drawing.Size(83, 23);
            this.masterEmployerSearchButton.TabIndex = 2;
            this.masterEmployerSearchButton.Text = "Search";
            this.masterEmployerSearchButton.UseVisualStyleBackColor = true;
            this.masterEmployerSearchButton.Click += new System.EventHandler(this.MasterEmployerSearchButtonOnClick);
            // 
            // cancelButton
            // 
            this.cancelButton.BackColor = System.Drawing.SystemColors.Control;
            this.cancelButton.CausesValidation = false;
            this.cancelButton.Location = new System.Drawing.Point(936, 589);
            this.cancelButton.Message = null;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 22;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButtonOnClick);
            // 
            // finishButton
            // 
            this.finishButton.BackColor = System.Drawing.SystemColors.Control;
            this.finishButton.Location = new System.Drawing.Point(855, 589);
            this.finishButton.Message = null;
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(75, 23);
            this.finishButton.TabIndex = 21;
            this.finishButton.Text = "Fini&sh";
            this.finishButton.UseVisualStyleBackColor = true;
            this.finishButton.Click += new System.EventHandler(this.FinishButtonOnClick);
            // 
            // undoButton
            // 
            this.undoButton.BackColor = System.Drawing.SystemColors.Control;
            this.undoButton.Location = new System.Drawing.Point(340, 558);
            this.undoButton.Message = null;
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(83, 23);
            this.undoButton.TabIndex = 19;
            this.undoButton.Text = "&Undo";
            this.undoButton.UseVisualStyleBackColor = true;
            this.undoButton.Click += new System.EventHandler(this.UndoButtonOnClick);
            // 
            // moveAddressAndPhoneButton
            // 
            this.moveAddressAndPhoneButton.BackColor = System.Drawing.SystemColors.Control;
            this.moveAddressAndPhoneButton.Location = new System.Drawing.Point(102, 558);
            this.moveAddressAndPhoneButton.Message = null;
            this.moveAddressAndPhoneButton.Name = "moveAddressAndPhoneButton";
            this.moveAddressAndPhoneButton.Size = new System.Drawing.Size(145, 23);
            this.moveAddressAndPhoneButton.TabIndex = 17;
            this.moveAddressAndPhoneButton.Text = "Move Address and Pho&ne";
            this.moveAddressAndPhoneButton.UseVisualStyleBackColor = true;
            this.moveAddressAndPhoneButton.Click += new System.EventHandler(this.MoveAddressAndPhoneButtonOnClick);
            // 
            // editAddressButton
            // 
            this.editAddressButton.BackColor = System.Drawing.SystemColors.Control;
            this.editAddressButton.Location = new System.Drawing.Point(121, 344);
            this.editAddressButton.Message = null;
            this.editAddressButton.Name = "editAddressButton";
            this.editAddressButton.Size = new System.Drawing.Size(83, 23);
            this.editAddressButton.TabIndex = 8;
            this.editAddressButton.Text = "Edit &Address...";
            this.editAddressButton.UseVisualStyleBackColor = true;
            this.editAddressButton.Click += new System.EventHandler(this.EditAddressButtonOnClick);
            // 
            // deleteButton
            // 
            this.deleteButton.BackColor = System.Drawing.SystemColors.Control;
            this.deleteButton.Location = new System.Drawing.Point(252, 558);
            this.deleteButton.Message = null;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(83, 23);
            this.deleteButton.TabIndex = 18;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.DeleteButtonOnClick);
            // 
            // moveAllInfoButton
            // 
            this.moveAllInfoButton.BackColor = System.Drawing.SystemColors.Control;
            this.moveAllInfoButton.Location = new System.Drawing.Point(14, 558);
            this.moveAllInfoButton.Message = null;
            this.moveAllInfoButton.Name = "moveAllInfoButton";
            this.moveAllInfoButton.Size = new System.Drawing.Size(83, 23);
            this.moveAllInfoButton.TabIndex = 16;
            this.moveAllInfoButton.Text = "Move All &Info";
            this.moveAllInfoButton.UseVisualStyleBackColor = true;
            this.moveAllInfoButton.Click += new System.EventHandler(this.MoveAllInfoButtonOnClick);
            // 
            // NewEmployersManagementView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.contextLabel);
            this.Controls.Add(this.moveButtonInstructionsLabel);
            this.Controls.Add(this.stepThreeLabel);
            this.Controls.Add(this.stepTwoLabel);
            this.Controls.Add(this.stepOneLabel);
            this.Controls.Add(this.noNewEmployersMessageLabel);
            this.Controls.Add(this.selectedEmployerPhoneLabel);
            this.Controls.Add(this.selectedEmployerPhoneTextBox);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.newEmployerDataGridView);
            this.Controls.Add(this.selectedEmployerLabel);
            this.Controls.Add(this.selectedEmployerNationalIdLabel);
            this.Controls.Add(this.masterEmployerListGroupBox);
            this.Controls.Add(this.selectedEmployerAddressLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.finishButton);
            this.Controls.Add(this.selectedEmployerNameTextBox);
            this.Controls.Add(this.undoButton);
            this.Controls.Add(this.selectedEmployerNationalIdTextBox);
            this.Controls.Add(this.moveAddressAndPhoneButton);
            this.Controls.Add(this.editAddressButton);
            this.Controls.Add(this.selectedEmployerAddressTextBox);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.moveAllInfoButton);
            this.Controls.Add(this.featureIsLockedMessageLabel);
            this.Name = "NewEmployersManagementView";
            this.Size = new System.Drawing.Size(1024, 619);
            this.masterEmployerListGroupBox.ResumeLayout(false);
            this.masterEmployerListGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.masterEmployerAddressesDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.masterEmployerListDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.newEmployerDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label selectedEmployerLabel;
        private System.Windows.Forms.Label selectedEmployerNationalIdLabel;
        private System.Windows.Forms.Label selectedEmployerAddressLabel;
        private MaskedEditTextBox selectedEmployerNameTextBox;
        private System.Windows.Forms.MaskedTextBox selectedEmployerNationalIdTextBox;
        private System.Windows.Forms.TextBox selectedEmployerAddressTextBox;
        private PatientAccess.UI.CommonControls.LoggingButton clearButton;
        private PatientAccess.UI.CommonControls.LoggingButton editAddressButton;
        private PatientAccess.UI.CommonControls.LoggingButton moveAllInfoButton;
        private PatientAccess.UI.CommonControls.LoggingButton deleteButton;
        private PatientAccess.UI.CommonControls.LoggingButton moveAddressAndPhoneButton;
        private PatientAccess.UI.CommonControls.LoggingButton undoButton;
        private PatientAccess.UI.CommonControls.LoggingButton finishButton;
        private PatientAccess.UI.CommonControls.LoggingButton cancelButton;
        private System.Windows.Forms.GroupBox masterEmployerListGroupBox;
        private System.Windows.Forms.MaskedTextBox masterEmployerSearchTextBox;
        private PatientAccess.UI.CommonControls.LoggingButton masterEmployerSearchButton;
        private System.Windows.Forms.DataGridView newEmployerDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.MaskedTextBox selectedEmployerPhoneTextBox;
        private System.Windows.Forms.Label selectedEmployerPhoneLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn employerInfoColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn newEmployerNationalIDColumn;
        private System.Windows.Forms.Label employerSearchLabel;
        private System.Windows.Forms.Label employerAddressLabel;
        private System.Windows.Forms.DataGridView masterEmployerAddressesDataGridView;
        private System.Windows.Forms.DataGridView masterEmployerListDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn employerNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nationalIDColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.Label noNewEmployersMessageLabel;
        private System.Windows.Forms.Label MasterListMessageLabel;
        private System.Windows.Forms.Label noAddressesLabel;
        private System.Windows.Forms.ToolTip phoneNumberErrorToolTip;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.Label featureIsLockedMessageLabel;
        private System.Windows.Forms.Label stepOneLabel;
        private System.Windows.Forms.Label stepTwoLabel;
        private System.Windows.Forms.Label stepThreeLabel;
        private System.Windows.Forms.Label moveButtonInstructionsLabel;
        private PatientAccess.UI.UserContextView contextLabel;
    }
}
