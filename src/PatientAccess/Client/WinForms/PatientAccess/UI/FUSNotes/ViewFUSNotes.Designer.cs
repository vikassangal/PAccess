namespace PatientAccess.UI.FUSNotes
{
    partial class ViewFUSNotes
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
            if( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ViewFUSNotes ) );
            this.btnOK = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dgFUSNotes = new System.Windows.Forms.DataGridView();
            this.PostedDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ExpandContract = new System.Windows.Forms.DataGridViewImageColumn();
            this.ActivityCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActivityDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WorklistDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UserID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Remarks = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NoteText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParentActivityCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NoteType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Amount1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Amount2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Month = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SortableActivityCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SortSequence = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ParentActivityDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblExpandContract = new System.Windows.Forms.Label();
            this.pbExpandContract = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lblIfNo = new System.Windows.Forms.Label();
            this.lblSearchResults = new System.Windows.Forms.Label();
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.btnShow = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.cmbActivityCodes = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.mtbActivityCode = new System.Windows.Forms.MaskedTextBox();
            this.rbSearchDescription = new System.Windows.Forms.RadioButton();
            this.rbSearchByActivity = new System.Windows.Forms.RadioButton();
            this.rbAll = new System.Windows.Forms.RadioButton();
            this.lblShowActivityCodes = new System.Windows.Forms.Label();
            this.lblShowAll = new System.Windows.Forms.Label();
            this.lblPatientValue = new System.Windows.Forms.Label();
            this.lblPatient = new System.Windows.Forms.Label();
            this.lblAccountValue = new System.Windows.Forms.Label();
            this.lblAccount = new System.Windows.Forms.Label();
            this.pnlNoResults = new System.Windows.Forms.Panel();
            this.lblNoResults = new System.Windows.Forms.Label();
            this.lblPendingCompletion = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ( (System.ComponentModel.ISupportInitialize)( this.dgFUSNotes ) ).BeginInit();
            this.pnlSearch.SuspendLayout();
            this.pnlNoResults.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point( 888, 626 );
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size( 83, 23 );
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler( this.btnOK_Click );
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.panel1.Controls.Add( this.dgFUSNotes );
            this.panel1.Controls.Add( this.lblExpandContract );
            this.panel1.Controls.Add( this.pbExpandContract );
            this.panel1.Controls.Add( this.btnRefresh );
            this.panel1.Controls.Add( this.btnAdd );
            this.panel1.Controls.Add( this.lblIfNo );
            this.panel1.Controls.Add( this.lblSearchResults );
            this.panel1.Controls.Add( this.pnlSearch );
            this.panel1.Controls.Add( this.lblShowAll );
            this.panel1.Controls.Add( this.lblPatientValue );
            this.panel1.Controls.Add( this.lblPatient );
            this.panel1.Controls.Add( this.lblAccountValue );
            this.panel1.Controls.Add( this.lblAccount );
            this.panel1.Controls.Add( this.pnlNoResults );
            this.panel1.Location = new System.Drawing.Point( 12, 0 );
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size( 960, 620 );
            this.panel1.TabIndex = 0;
            // 
            // dgFUSNotes
            // 
            this.dgFUSNotes.AllowUserToAddRows = false;
            this.dgFUSNotes.AllowUserToDeleteRows = false;
            this.dgFUSNotes.AllowUserToResizeColumns = false;
            this.dgFUSNotes.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 243 ) ) ) ), ( (int)( ( (byte)( 245 ) ) ) ), ( (int)( ( (byte)( 254 ) ) ) ) );
            dataGridViewCellStyle1.Font = new System.Drawing.Font( "Lucida Sans Typewriter", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding( 0, 5, 0, 5 );
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgFUSNotes.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgFUSNotes.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgFUSNotes.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding( 0, 5, 0, 5 );
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgFUSNotes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgFUSNotes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgFUSNotes.Columns.AddRange( new System.Windows.Forms.DataGridViewColumn[] {
            this.PostedDate,
            this.ExpandContract,
            this.ActivityCode,
            this.ActivityDescription,
            this.Description,
            this.WorklistDate,
            this.UserID,
            this.Remarks,
            this.NoteText,
            this.ParentActivityCode,
            this.NoteType,
            this.Amount1,
            this.Amount2,
            this.Date1,
            this.Date2,
            this.Month,
            this.SortableActivityCode,
            this.SortSequence,
            this.ParentActivityDescription} );
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font( "Lucida Sans Typewriter", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding( 0, 5, 0, 5 );
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgFUSNotes.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgFUSNotes.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgFUSNotes.GridColor = System.Drawing.Color.White;
            this.dgFUSNotes.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.dgFUSNotes.Location = new System.Drawing.Point( 0, 231 );
            this.dgFUSNotes.MultiSelect = false;
            this.dgFUSNotes.Name = "dgFUSNotes";
            this.dgFUSNotes.ReadOnly = true;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgFUSNotes.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgFUSNotes.RowHeadersVisible = false;
            this.dgFUSNotes.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle6.Font = new System.Drawing.Font( "Lucida Sans Typewriter", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            dataGridViewCellStyle6.Padding = new System.Windows.Forms.Padding( 0, 5, 0, 5 );
            this.dgFUSNotes.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dgFUSNotes.RowTemplate.DividerHeight = 1;
            this.dgFUSNotes.RowTemplate.ReadOnly = true;
            this.dgFUSNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgFUSNotes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgFUSNotes.ShowEditingIcon = false;
            this.dgFUSNotes.Size = new System.Drawing.Size( 960, 386 );
            this.dgFUSNotes.TabIndex = 11;
            this.dgFUSNotes.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler( this.dgFUSNotes_SortCompare );
            this.dgFUSNotes.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler( this.dgFUSNotes_CellDoubleClick );
            this.dgFUSNotes.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler( this.dgFUSNotes_CellClick );
            // 
            // PostedDate
            // 
            this.PostedDate.HeaderText = "Posted Date/Time";
            this.PostedDate.Name = "PostedDate";
            this.PostedDate.ReadOnly = true;
            this.PostedDate.Width = 140;
            // 
            // ExpandContract
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle3.NullValue = ( (object)( resources.GetObject( "dataGridViewCellStyle3.NullValue" ) ) );
            this.ExpandContract.DefaultCellStyle = dataGridViewCellStyle3;
            this.ExpandContract.HeaderText = "+/-";
            this.ExpandContract.Name = "ExpandContract";
            this.ExpandContract.ReadOnly = true;
            this.ExpandContract.Width = 27;
            // 
            // ActivityCode
            // 
            this.ActivityCode.HeaderText = "Activity";
            this.ActivityCode.Name = "ActivityCode";
            this.ActivityCode.ReadOnly = true;
            this.ActivityCode.Width = 66;
            // 
            // ActivityDescription
            // 
            this.ActivityDescription.HeaderText = "Activity Description";
            this.ActivityDescription.Name = "ActivityDescription";
            this.ActivityDescription.ReadOnly = true;
            this.ActivityDescription.Visible = false;
            this.ActivityDescription.Width = 95;
            // 
            // Description
            // 
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            this.Description.Width = 555;
            // 
            // WorklistDate
            // 
            this.WorklistDate.HeaderText = "Worklist";
            this.WorklistDate.Name = "WorklistDate";
            this.WorklistDate.ReadOnly = true;
            this.WorklistDate.Width = 75;
            // 
            // UserID
            // 
            this.UserID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.UserID.HeaderText = "User ID";
            this.UserID.Name = "UserID";
            this.UserID.ReadOnly = true;
            // 
            // Remarks
            // 
            this.Remarks.HeaderText = "Remarks";
            this.Remarks.Name = "Remarks";
            this.Remarks.ReadOnly = true;
            this.Remarks.Visible = false;
            // 
            // NoteText
            // 
            this.NoteText.HeaderText = "NoteText";
            this.NoteText.Name = "NoteText";
            this.NoteText.ReadOnly = true;
            this.NoteText.Visible = false;
            // 
            // ParentActivityCode
            // 
            this.ParentActivityCode.HeaderText = "Parent Activity Code";
            this.ParentActivityCode.Name = "ParentActivityCode";
            this.ParentActivityCode.ReadOnly = true;
            this.ParentActivityCode.Visible = false;
            this.ParentActivityCode.Width = 95;
            // 
            // NoteType
            // 
            this.NoteType.HeaderText = "NoteType";
            this.NoteType.Name = "NoteType";
            this.NoteType.ReadOnly = true;
            this.NoteType.Visible = false;
            // 
            // Amount1
            // 
            this.Amount1.HeaderText = "Amount1";
            this.Amount1.Name = "Amount1";
            this.Amount1.ReadOnly = true;
            this.Amount1.Visible = false;
            // 
            // Amount2
            // 
            this.Amount2.HeaderText = "Amount2";
            this.Amount2.Name = "Amount2";
            this.Amount2.ReadOnly = true;
            this.Amount2.Visible = false;
            // 
            // Date1
            // 
            this.Date1.HeaderText = "Date1";
            this.Date1.Name = "Date1";
            this.Date1.ReadOnly = true;
            this.Date1.Visible = false;
            // 
            // Date2
            // 
            this.Date2.HeaderText = "Date2";
            this.Date2.Name = "Date2";
            this.Date2.ReadOnly = true;
            this.Date2.Visible = false;
            // 
            // Month
            // 
            this.Month.HeaderText = "Month";
            this.Month.Name = "Month";
            this.Month.ReadOnly = true;
            this.Month.Visible = false;
            // 
            // SortableActivityCode
            // 
            this.SortableActivityCode.HeaderText = "Sortable Activity Code";
            this.SortableActivityCode.Name = "SortableActivityCode";
            this.SortableActivityCode.ReadOnly = true;
            this.SortableActivityCode.Visible = false;
            this.SortableActivityCode.Width = 95;
            // 
            // SortSequence
            // 
            this.SortSequence.HeaderText = "SortSequence";
            this.SortSequence.Name = "SortSequence";
            this.SortSequence.ReadOnly = true;
            this.SortSequence.Visible = false;
            this.SortSequence.Width = 95;
            // 
            // ParentActivityDescription
            // 
            this.ParentActivityDescription.HeaderText = "ParentActivityDescription";
            this.ParentActivityDescription.Name = "ParentActivityDescription";
            this.ParentActivityDescription.ReadOnly = true;
            this.ParentActivityDescription.Visible = false;
            this.ParentActivityDescription.Width = 95;
            // 
            // lblExpandContract
            // 
            this.lblExpandContract.AutoSize = true;
            this.lblExpandContract.Location = new System.Drawing.Point( 448, 201 );
            this.lblExpandContract.Name = "lblExpandContract";
            this.lblExpandContract.Size = new System.Drawing.Size( 57, 13 );
            this.lblExpandContract.TabIndex = 0;
            this.lblExpandContract.Text = "Expand All";
            // 
            // pbExpandContract
            // 
            this.pbExpandContract.FlatAppearance.BorderSize = 0;
            this.pbExpandContract.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pbExpandContract.Location = new System.Drawing.Point( 420, 197 );
            this.pbExpandContract.Name = "pbExpandContract";
            this.pbExpandContract.Size = new System.Drawing.Size( 21, 21 );
            this.pbExpandContract.TabIndex = 9;
            this.pbExpandContract.Click += new System.EventHandler( this.pbExpandContract_Click );
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point( 876, 200 );
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size( 83, 23 );
            this.btnRefresh.TabIndex = 10;
            this.btnRefresh.Text = "Refres&h";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler( this.btnRefresh_Click );
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point( 2, 200 );
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size( 83, 23 );
            this.btnAdd.TabIndex = 8;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler( this.btnAdd_Click );
            // 
            // lblIfNo
            // 
            this.lblIfNo.AutoSize = true;
            this.lblIfNo.Location = new System.Drawing.Point( 0, 176 );
            this.lblIfNo.Name = "lblIfNo";
            this.lblIfNo.Size = new System.Drawing.Size( 446, 13 );
            this.lblIfNo.TabIndex = 7;
            this.lblIfNo.Text = "If no appropriate result is found, try modifying your search crtiteria above and " +
                "searching again.";
            // 
            // lblSearchResults
            // 
            this.lblSearchResults.AutoSize = true;
            this.lblSearchResults.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.lblSearchResults.Location = new System.Drawing.Point( 0, 159 );
            this.lblSearchResults.Name = "lblSearchResults";
            this.lblSearchResults.Size = new System.Drawing.Size( 93, 13 );
            this.lblSearchResults.TabIndex = 6;
            this.lblSearchResults.Text = "Search Results";
            // 
            // pnlSearch
            // 
            this.pnlSearch.BackColor = System.Drawing.Color.White;
            this.pnlSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSearch.Controls.Add( this.btnShow );
            this.pnlSearch.Controls.Add( this.btnReset );
            this.pnlSearch.Controls.Add( this.cmbActivityCodes );
            this.pnlSearch.Controls.Add( this.mtbActivityCode );
            this.pnlSearch.Controls.Add( this.rbSearchDescription );
            this.pnlSearch.Controls.Add( this.rbSearchByActivity );
            this.pnlSearch.Controls.Add( this.rbAll );
            this.pnlSearch.Controls.Add( this.lblShowActivityCodes );
            this.pnlSearch.Location = new System.Drawing.Point( 0, 59 );
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Size = new System.Drawing.Size( 960, 93 );
            this.pnlSearch.TabIndex = 5;
            // 
            // btnShow
            // 
            this.btnShow.Location = new System.Drawing.Point( 777, 12 );
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new System.Drawing.Size( 83, 23 );
            this.btnShow.TabIndex = 11;
            this.btnShow.Text = "Sh&ow";
            this.btnShow.UseVisualStyleBackColor = true;
            this.btnShow.Click += new System.EventHandler( this.btnShow_Click );
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point( 868, 12 );
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size( 83, 23 );
            this.btnReset.TabIndex = 12;
            this.btnReset.Text = "Rese&t";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler( this.btnReset_Click );
            // 
            // cmbActivityCodes
            // 
            this.cmbActivityCodes.FormattingEnabled = true;
            this.cmbActivityCodes.Location = new System.Drawing.Point( 300, 60 );
            this.cmbActivityCodes.Name = "cmbActivityCodes";
            this.cmbActivityCodes.Size = new System.Drawing.Size( 357, 21 );
            this.cmbActivityCodes.TabIndex = 10;
            this.cmbActivityCodes.SelectedIndexChanged += new System.EventHandler( this.cmbActivityCodes_SelectedIndexChanged );
            this.cmbActivityCodes.Leave += new System.EventHandler( this.cmbActivityCodes_SelectedIndexChanged );
            // 
            // mtbActivityCode
            // 
            this.mtbActivityCode.AllowPromptAsInput = false;
            this.mtbActivityCode.CutCopyMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.mtbActivityCode.HideSelection = false;
            this.mtbActivityCode.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.mtbActivityCode.Location = new System.Drawing.Point( 300, 34 );
            this.mtbActivityCode.Mask = ">AAAAA";
            this.mtbActivityCode.Name = "mtbActivityCode";
            this.mtbActivityCode.PromptChar = ' ';
            this.mtbActivityCode.ResetOnSpace = false;
            this.mtbActivityCode.Size = new System.Drawing.Size( 61, 20 );
            this.mtbActivityCode.SkipLiterals = false;
            this.mtbActivityCode.TabIndex = 8;
            this.mtbActivityCode.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.mtbActivityCode.ValidatingType = typeof( int );
            this.mtbActivityCode.GotFocus += new System.EventHandler( this.mtbActivityCode_GotFocus );
            this.mtbActivityCode.TextChanged += new System.EventHandler( this.mtbActivityCode_TextChanged );
            this.mtbActivityCode.Click += new System.EventHandler( this.mtbActivityCode_Click );
            // 
            // rbSearchDescription
            // 
            this.rbSearchDescription.AutoSize = true;
            this.rbSearchDescription.Location = new System.Drawing.Point( 132, 58 );
            this.rbSearchDescription.Name = "rbSearchDescription";
            this.rbSearchDescription.Size = new System.Drawing.Size( 129, 17 );
            this.rbSearchDescription.TabIndex = 9;
            this.rbSearchDescription.TabStop = true;
            this.rbSearchDescription.Text = "Search by Description";
            this.rbSearchDescription.UseVisualStyleBackColor = true;
            this.rbSearchDescription.CheckedChanged += new System.EventHandler( this.rbSearchDescription_CheckedChanged );
            // 
            // rbSearchByActivity
            // 
            this.rbSearchByActivity.AutoSize = true;
            this.rbSearchByActivity.Location = new System.Drawing.Point( 132, 35 );
            this.rbSearchByActivity.Name = "rbSearchByActivity";
            this.rbSearchByActivity.Size = new System.Drawing.Size( 137, 17 );
            this.rbSearchByActivity.TabIndex = 7;
            this.rbSearchByActivity.TabStop = true;
            this.rbSearchByActivity.Text = "Search by Activity code";
            this.rbSearchByActivity.UseVisualStyleBackColor = true;
            this.rbSearchByActivity.CheckedChanged += new System.EventHandler( this.rbSearchByActivity_CheckedChanged );
            // 
            // rbAll
            // 
            this.rbAll.AutoSize = true;
            this.rbAll.Checked = true;
            this.rbAll.Location = new System.Drawing.Point( 132, 12 );
            this.rbAll.Name = "rbAll";
            this.rbAll.Size = new System.Drawing.Size( 36, 17 );
            this.rbAll.TabIndex = 6;
            this.rbAll.TabStop = true;
            this.rbAll.Text = "All";
            this.rbAll.UseVisualStyleBackColor = true;
            this.rbAll.CheckedChanged += new System.EventHandler( this.rbAll_CheckedChanged );
            // 
            // lblShowActivityCodes
            // 
            this.lblShowActivityCodes.AutoSize = true;
            this.lblShowActivityCodes.Location = new System.Drawing.Point( 20, 12 );
            this.lblShowActivityCodes.Name = "lblShowActivityCodes";
            this.lblShowActivityCodes.Size = new System.Drawing.Size( 105, 13 );
            this.lblShowActivityCodes.TabIndex = 0;
            this.lblShowActivityCodes.Text = "Show activity codes:";
            // 
            // lblShowAll
            // 
            this.lblShowAll.AutoSize = true;
            this.lblShowAll.Location = new System.Drawing.Point( 0, 38 );
            this.lblShowAll.Name = "lblShowAll";
            this.lblShowAll.Size = new System.Drawing.Size( 312, 13 );
            this.lblShowAll.TabIndex = 4;
            this.lblShowAll.Text = "Show All activity codes or search by Activity code or Description.";
            // 
            // lblPatientValue
            // 
            this.lblPatientValue.AutoSize = true;
            this.lblPatientValue.Location = new System.Drawing.Point( 251, 15 );
            this.lblPatientValue.Name = "lblPatientValue";
            this.lblPatientValue.Size = new System.Drawing.Size( 0, 13 );
            this.lblPatientValue.TabIndex = 3;
            // 
            // lblPatient
            // 
            this.lblPatient.AutoSize = true;
            this.lblPatient.Location = new System.Drawing.Point( 173, 15 );
            this.lblPatient.Name = "lblPatient";
            this.lblPatient.Size = new System.Drawing.Size( 72, 13 );
            this.lblPatient.TabIndex = 2;
            this.lblPatient.Text = "Patient name:";
            // 
            // lblAccountValue
            // 
            this.lblAccountValue.AutoSize = true;
            this.lblAccountValue.Location = new System.Drawing.Point( 56, 15 );
            this.lblAccountValue.Name = "lblAccountValue";
            this.lblAccountValue.Size = new System.Drawing.Size( 0, 13 );
            this.lblAccountValue.TabIndex = 1;
            // 
            // lblAccount
            // 
            this.lblAccount.AutoSize = true;
            this.lblAccount.Location = new System.Drawing.Point( 0, 15 );
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size( 50, 13 );
            this.lblAccount.TabIndex = 0;
            this.lblAccount.Text = "Account:";
            // 
            // pnlNoResults
            // 
            this.pnlNoResults.BackColor = System.Drawing.Color.White;
            this.pnlNoResults.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlNoResults.Controls.Add( this.lblNoResults );
            this.pnlNoResults.Location = new System.Drawing.Point( 0, 231 );
            this.pnlNoResults.Name = "pnlNoResults";
            this.pnlNoResults.Size = new System.Drawing.Size( 953, 386 );
            this.pnlNoResults.TabIndex = 0;
            this.pnlNoResults.Visible = false;
            // 
            // lblNoResults
            // 
            this.lblNoResults.AutoSize = true;
            this.lblNoResults.Location = new System.Drawing.Point( 15, 15 );
            this.lblNoResults.Name = "lblNoResults";
            this.lblNoResults.Size = new System.Drawing.Size( 263, 13 );
            this.lblNoResults.TabIndex = 0;
            this.lblNoResults.Text = "There are no FUS Notes associated with this account.";
            // 
            // lblPendingCompletion
            // 
            this.lblPendingCompletion.AutoSize = true;
            this.lblPendingCompletion.Location = new System.Drawing.Point( 15, 624 );
            this.lblPendingCompletion.Name = "lblPendingCompletion";
            this.lblPendingCompletion.Size = new System.Drawing.Size( 155, 13 );
            this.lblPendingCompletion.TabIndex = 0;
            this.lblPendingCompletion.Text = "* Pending completion of activity";
            // 
            // ViewFUSNotes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.ClientSize = new System.Drawing.Size( 983, 656 );
            this.Controls.Add( this.lblPendingCompletion );
            this.Controls.Add( this.panel1 );
            this.Controls.Add( this.btnOK );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ( (System.Drawing.Icon)( resources.GetObject( "$this.Icon" ) ) );
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ViewFUSNotes";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "View FUS Notes";
            this.Load += new System.EventHandler( this.ViewFUSNotes_Load );
            this.Leave += new System.EventHandler( this.ViewFUSNotes_Leave );
            this.panel1.ResumeLayout( false );
            this.panel1.PerformLayout();
            ( (System.ComponentModel.ISupportInitialize)( this.dgFUSNotes ) ).EndInit();
            this.pnlSearch.ResumeLayout( false );
            this.pnlSearch.PerformLayout();
            this.pnlNoResults.ResumeLayout( false );
            this.pnlNoResults.PerformLayout();
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnlSearch;

        private System.Windows.Forms.Label lblShowAll;
        private System.Windows.Forms.Label lblPatientValue;
        private System.Windows.Forms.Label lblPatient;
        private System.Windows.Forms.Label lblAccountValue;
        private System.Windows.Forms.Label lblAccount;
        private System.Windows.Forms.Label lblIfNo;
        private System.Windows.Forms.Label lblSearchResults;
        private System.Windows.Forms.Label lblExpandContract;
        private System.Windows.Forms.Label lblShowActivityCodes;
        
        private System.Windows.Forms.MaskedTextBox mtbActivityCode;

        private System.Windows.Forms.RadioButton rbSearchDescription;
        private System.Windows.Forms.RadioButton rbSearchByActivity;
        private System.Windows.Forms.RadioButton rbAll;

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnShow;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnAdd;

        // this is a button to act like a picture box
        private System.Windows.Forms.Button pbExpandContract;

        private System.Windows.Forms.DataGridView dgFUSNotes;

        private PatientAccess.UI.CommonControls.PatientAccessComboBox cmbActivityCodes;
        private System.Windows.Forms.Label lblPendingCompletion;
        private System.Windows.Forms.Panel pnlNoResults;
        private System.Windows.Forms.Label lblNoResults;
        private System.Windows.Forms.DataGridViewTextBoxColumn PostedDate;
        private System.Windows.Forms.DataGridViewImageColumn ExpandContract;
        private System.Windows.Forms.DataGridViewTextBoxColumn ActivityCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ActivityDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewTextBoxColumn WorklistDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn UserID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Remarks;
        private System.Windows.Forms.DataGridViewTextBoxColumn NoteText;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParentActivityCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn NoteType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Amount1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Amount2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Month;
        private System.Windows.Forms.DataGridViewTextBoxColumn SortableActivityCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn SortSequence;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParentActivityDescription;       
    }
}
