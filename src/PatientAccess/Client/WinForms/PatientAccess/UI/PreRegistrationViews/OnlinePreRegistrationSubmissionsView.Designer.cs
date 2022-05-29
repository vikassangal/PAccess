using System.Windows.Forms;
using PatientAccess.UI;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.WorklistViews;

namespace PatientAccess.UI.PreRegistrationViews
{
    partial class OnlinePreRegistrationSubmissionsView
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
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( OnlinePreRegistrationSubmissionsView ) );
            this.toolTipName = new System.Windows.Forms.ToolTip( this.components );
            this.onlinePreRegistrationsPanel = new System.Windows.Forms.Panel();
            this.onlinePreRegDataGrid = new PatientAccess.UI.CommonControls.FilterEnterDataGridView();
            this.RecordLock = new System.Windows.Forms.DataGridViewImageColumn();
            this.PatientName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VisitedBefore = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GenderColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DateOfBirth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SsnColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AddressColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AdmitDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.labelNoOnlineSubmissions = new System.Windows.Forms.Label();
            this.lblPossibleMatches = new System.Windows.Forms.Label();
            this.matchingPatientsPanel = new System.Windows.Forms.Panel();
            this.matchingPatientsDataGrid = new PatientAccess.UI.CommonControls.FilterEnterDataGridView();
            this.matchingPatientName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.matchingGender = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.matchingDob = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.matchingSsn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.matchingAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblNoMatchingPatients = new System.Windows.Forms.Label();
            this.progressPanelPatientSearch = new PatientAccess.UI.CommonControls.ProgressPanel();
            this.lblAwaitingProcessing = new System.Windows.Forms.Label();
            this.labelNoAccess = new System.Windows.Forms.Label();
            this.noAccessPanel = new System.Windows.Forms.Panel();
            this.lockImage = new System.Windows.Forms.ImageList( this.components );
            this.contextLabel = new PatientAccess.UI.UserContextView();
            this.i_FilterWorklistView = new PatientAccess.UI.WorklistViews.FilterWorklistView();
            this.btnCreateNewPatient = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnCreateNewAccount = new PatientAccess.UI.CommonControls.LoggingButton();
            this.progressPanel = new PatientAccess.UI.CommonControls.ProgressPanel();
            this.onlinePreRegistrationsPanel.SuspendLayout();
            ( (System.ComponentModel.ISupportInitialize)( this.onlinePreRegDataGrid ) ).BeginInit();
            this.matchingPatientsPanel.SuspendLayout();
            ( (System.ComponentModel.ISupportInitialize)( this.matchingPatientsDataGrid ) ).BeginInit();
            this.noAccessPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolTipName
            // 
            this.toolTipName.AutoPopDelay = 1000;
            this.toolTipName.InitialDelay = 1000;
            this.toolTipName.ReshowDelay = 1000;
            this.toolTipName.ShowAlways = true;
            // 
            // onlinePreRegistrationsPanel
            // 
            this.onlinePreRegistrationsPanel.Controls.Add( this.onlinePreRegDataGrid );
            this.onlinePreRegistrationsPanel.Controls.Add( this.labelNoOnlineSubmissions );
            this.onlinePreRegistrationsPanel.Location = new System.Drawing.Point( 7, 127 );
            this.onlinePreRegistrationsPanel.Name = "onlinePreRegistrationsPanel";
            this.onlinePreRegistrationsPanel.Size = new System.Drawing.Size( 992, 200 );
            this.onlinePreRegistrationsPanel.TabIndex = 1;
            // 
            // onlinePreRegDataGrid
            // 
            this.onlinePreRegDataGrid.AllowUserToAddRows = false;
            this.onlinePreRegDataGrid.AllowUserToDeleteRows = false;
            this.onlinePreRegDataGrid.AllowUserToResizeColumns = false;
            this.onlinePreRegDataGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.onlinePreRegDataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.onlinePreRegDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.onlinePreRegDataGrid.Columns.AddRange( new System.Windows.Forms.DataGridViewColumn[] {
            this.RecordLock,
            this.PatientName,
            this.VisitedBefore,
            this.GenderColumn,
            this.DateOfBirth,
            this.SsnColumn,
            this.AddressColumn,
            this.AdmitDate} );
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.onlinePreRegDataGrid.DefaultCellStyle = dataGridViewCellStyle8;
            this.onlinePreRegDataGrid.Location = new System.Drawing.Point( 0, 0 );
            this.onlinePreRegDataGrid.MultiSelect = false;
            this.onlinePreRegDataGrid.Name = "onlinePreRegDataGrid";
            this.onlinePreRegDataGrid.ReadOnly = true;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.onlinePreRegDataGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.onlinePreRegDataGrid.RowHeadersVisible = false;
            this.onlinePreRegDataGrid.RowHeadersWidth = 340;
            this.onlinePreRegDataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.onlinePreRegDataGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.onlinePreRegDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.onlinePreRegDataGrid.Size = new System.Drawing.Size( 992, 200 );
            this.onlinePreRegDataGrid.StandardTab = true;
            this.onlinePreRegDataGrid.TabIndex = 1;
            this.onlinePreRegDataGrid.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler( OnCellToolTipTextNeeded );
            this.onlinePreRegDataGrid.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler( this.OnlinePreRegDataGrid_DataBindingComplete );
            this.onlinePreRegDataGrid.SelectionChanged += new System.EventHandler( this.OnlineEntriesDataGrid_SelectionChanged );
            // 
            // RecordLock
            // 
            this.RecordLock.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.RecordLock.DataPropertyName = "IsLocked";
            this.RecordLock.HeaderText = "";
            this.RecordLock.MinimumWidth = 20;
            this.RecordLock.Name = "RecordLock";
            this.RecordLock.ReadOnly = true;
            this.RecordLock.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.RecordLock.Width = 20;
            // 
            // PatientName
            // 
            this.PatientName.DataPropertyName = "PatientName";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.PatientName.DefaultCellStyle = dataGridViewCellStyle2;
            this.PatientName.HeaderText = "Patient Name";
            this.PatientName.MinimumWidth = 190;
            this.PatientName.Name = "PatientName";
            this.PatientName.ReadOnly = true;
            this.PatientName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PatientName.Width = 190;
            // 
            // VisitedBefore
            // 
            this.VisitedBefore.DataPropertyName = "VisitedBefore";
            this.VisitedBefore.HeaderText = "Visited Before?";
            this.VisitedBefore.MinimumWidth = 90;
            this.VisitedBefore.Name = "VisitedBefore";
            this.VisitedBefore.ReadOnly = true;
            this.VisitedBefore.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.VisitedBefore.Width = 90;
            // 
            // GenderColumn
            // 
            this.GenderColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.GenderColumn.DataPropertyName = "Gender";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.GenderColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.GenderColumn.HeaderText = "Gender";
            this.GenderColumn.MinimumWidth = 70;
            this.GenderColumn.Name = "GenderColumn";
            this.GenderColumn.ReadOnly = true;
            this.GenderColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.GenderColumn.Width = 70;
            // 
            // DateOfBirth
            // 
            this.DateOfBirth.DataPropertyName = "DateOfBirth";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.Format = "d";
            dataGridViewCellStyle4.NullValue = null;
            this.DateOfBirth.DefaultCellStyle = dataGridViewCellStyle4;
            this.DateOfBirth.HeaderText = "DOB";
            this.DateOfBirth.MinimumWidth = 90;
            this.DateOfBirth.Name = "DateOfBirth";
            this.DateOfBirth.ReadOnly = true;
            this.DateOfBirth.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.DateOfBirth.Width = 90;
            // 
            // SsnColumn
            // 
            this.SsnColumn.DataPropertyName = "Ssn";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.SsnColumn.DefaultCellStyle = dataGridViewCellStyle5;
            this.SsnColumn.HeaderText = "SSN";
            this.SsnColumn.MinimumWidth = 90;
            this.SsnColumn.Name = "SsnColumn";
            this.SsnColumn.ReadOnly = true;
            this.SsnColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SsnColumn.Width = 90;
            // 
            // AddressColumn
            // 
            this.AddressColumn.DataPropertyName = "Address";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.AddressColumn.DefaultCellStyle = dataGridViewCellStyle6;
            this.AddressColumn.HeaderText = "Address";
            this.AddressColumn.MinimumWidth = 290;
            this.AddressColumn.Name = "AddressColumn";
            this.AddressColumn.ReadOnly = true;
            this.AddressColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.AddressColumn.Width = 290;
            // 
            // AdmitDate
            // 
            this.AdmitDate.DataPropertyName = "AdmitDate";
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.Format = "g";
            dataGridViewCellStyle7.NullValue = null;
            this.AdmitDate.DefaultCellStyle = dataGridViewCellStyle7;
            this.AdmitDate.HeaderText = "Admit Date/Time";
            this.AdmitDate.MinimumWidth = 150;
            this.AdmitDate.Name = "AdmitDate";
            this.AdmitDate.ReadOnly = true;
            this.AdmitDate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.AdmitDate.Width = 150;
            // 
            // labelNoOnlineSubmissions
            // 
            this.labelNoOnlineSubmissions.BackColor = System.Drawing.SystemColors.Window;
            this.labelNoOnlineSubmissions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelNoOnlineSubmissions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelNoOnlineSubmissions.Location = new System.Drawing.Point( 0, 0 );
            this.labelNoOnlineSubmissions.Name = "labelNoOnlineSubmissions";
            this.labelNoOnlineSubmissions.Size = new System.Drawing.Size( 992, 200 );
            this.labelNoOnlineSubmissions.TabIndex = 1;
            this.labelNoOnlineSubmissions.Text = "No submissions were found based on the applied filter settings.";
            // 
            // lblPossibleMatches
            // 
            this.lblPossibleMatches.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.lblPossibleMatches.Location = new System.Drawing.Point( 8, 360 );
            this.lblPossibleMatches.Name = "lblPossibleMatches";
            this.lblPossibleMatches.Size = new System.Drawing.Size( 219, 13 );
            this.lblPossibleMatches.TabIndex = 0;
            this.lblPossibleMatches.Text = "Possible Matches to Existing Patients";
            // 
            // matchingPatientsPanel
            // 
            this.matchingPatientsPanel.Controls.Add( this.matchingPatientsDataGrid );
            this.matchingPatientsPanel.Controls.Add( this.lblNoMatchingPatients );
            this.matchingPatientsPanel.Controls.Add( this.progressPanelPatientSearch );
            this.matchingPatientsPanel.Location = new System.Drawing.Point( 7, 385 );
            this.matchingPatientsPanel.Name = "matchingPatientsPanel";
            this.matchingPatientsPanel.Size = new System.Drawing.Size( 992, 166 );
            this.matchingPatientsPanel.TabIndex = 3;
            // 
            // matchingPatientsDataGrid
            // 
            this.matchingPatientsDataGrid.AllowUserToAddRows = false;
            this.matchingPatientsDataGrid.AllowUserToDeleteRows = false;
            this.matchingPatientsDataGrid.AllowUserToResizeColumns = false;
            this.matchingPatientsDataGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.matchingPatientsDataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.matchingPatientsDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.matchingPatientsDataGrid.Columns.AddRange( new System.Windows.Forms.DataGridViewColumn[] {
            this.matchingPatientName,
            this.matchingGender,
            this.matchingDob,
            this.matchingSsn,
            this.matchingAddress} );
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle12.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.matchingPatientsDataGrid.DefaultCellStyle = dataGridViewCellStyle12;
            this.matchingPatientsDataGrid.Location = new System.Drawing.Point( 0, 0 );
            this.matchingPatientsDataGrid.MultiSelect = false;
            this.matchingPatientsDataGrid.Name = "matchingPatientsDataGrid";
            this.matchingPatientsDataGrid.ReadOnly = true;
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle13.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            dataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.matchingPatientsDataGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle13;
            this.matchingPatientsDataGrid.RowHeadersVisible = false;
            this.matchingPatientsDataGrid.RowHeadersWidth = 340;
            this.matchingPatientsDataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.matchingPatientsDataGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.matchingPatientsDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.matchingPatientsDataGrid.Size = new System.Drawing.Size( 992, 166 );
            this.matchingPatientsDataGrid.StandardTab = true;
            this.matchingPatientsDataGrid.TabIndex = 3;
            this.matchingPatientsDataGrid.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler( OnCellToolTipTextNeeded );
            this.matchingPatientsDataGrid.SelectionChanged += new System.EventHandler( this.MatchingPatientsDataGrid_SelectionChanged );
            // 
            // matchingPatientName
            // 
            this.matchingPatientName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.matchingPatientName.DataPropertyName = "MatchingPatientName";
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.matchingPatientName.DefaultCellStyle = dataGridViewCellStyle11;
            this.matchingPatientName.HeaderText = "Patient Name";
            this.matchingPatientName.Name = "matchingPatientName";
            this.matchingPatientName.ReadOnly = true;
            this.matchingPatientName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.matchingPatientName.Width = 260;
            // 
            // matchingGender
            // 
            this.matchingGender.DataPropertyName = "MatchingGender";
            this.matchingGender.DefaultCellStyle = dataGridViewCellStyle11;
            this.matchingGender.HeaderText = "Gender";
            this.matchingGender.Name = "matchingGender";
            this.matchingGender.ReadOnly = true;
            this.matchingGender.Width = 90;
            // 
            // matchingDob
            // 
            this.matchingDob.DataPropertyName = "MatchingDateOfBirth";
            this.matchingDob.DefaultCellStyle = dataGridViewCellStyle11;
            this.matchingDob.HeaderText = "DOB";
            this.matchingDob.Name = "matchingDob";
            this.matchingDob.ReadOnly = true;
            this.matchingDob.Width = 110;
            // 
            // matchingSsn
            // 
            this.matchingSsn.DataPropertyName = "MatchingSsn";
            this.matchingSsn.DefaultCellStyle = dataGridViewCellStyle11;
            this.matchingSsn.HeaderText = "SSN";
            this.matchingSsn.Name = "matchingSsn";
            this.matchingSsn.ReadOnly = true;
            this.matchingSsn.Width = 130;
            // 
            // matchingAddress
            // 
            this.matchingAddress.DataPropertyName = "MatchingAddress";
            this.matchingAddress.DefaultCellStyle = dataGridViewCellStyle11;
            this.matchingAddress.HeaderText = "Address";
            this.matchingAddress.Name = "matchingAddress";
            this.matchingAddress.ReadOnly = true;
            this.matchingAddress.Width = 400;
            // 
            // lblNoMatchingPatients
            // 
            this.lblNoMatchingPatients.BackColor = System.Drawing.SystemColors.Window;
            this.lblNoMatchingPatients.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNoMatchingPatients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNoMatchingPatients.Location = new System.Drawing.Point( 0, 0 );
            this.lblNoMatchingPatients.Name = "lblNoMatchingPatients";
            this.lblNoMatchingPatients.Size = new System.Drawing.Size( 992, 166 );
            this.lblNoMatchingPatients.TabIndex = 7;
            this.lblNoMatchingPatients.Text = "No matches to display.";
            // 
            // progressPanelPatientSearch
            // 
            this.progressPanelPatientSearch.BackColor = System.Drawing.Color.White;
            this.progressPanelPatientSearch.Location = new System.Drawing.Point( 0, 0 );
            this.progressPanelPatientSearch.Name = "progressPanelPatientSearch";
            this.progressPanelPatientSearch.Size = new System.Drawing.Size( 992, 166 );
            this.progressPanelPatientSearch.TabIndex = 8;
            // 
            // lblAwaitingProcessing
            // 
            this.lblAwaitingProcessing.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.lblAwaitingProcessing.Location = new System.Drawing.Point( 9, 107 );
            this.lblAwaitingProcessing.Name = "lblAwaitingProcessing";
            this.lblAwaitingProcessing.Size = new System.Drawing.Size( 253, 13 );
            this.lblAwaitingProcessing.TabIndex = 0;
            this.lblAwaitingProcessing.Text = "Online Preregistrations Awaiting Processing";
            // 
            // labelNoAccess
            // 
            this.labelNoAccess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelNoAccess.Location = new System.Drawing.Point( 0, 0 );
            this.labelNoAccess.Name = "labelNoAccess";
            this.labelNoAccess.Size = new System.Drawing.Size( 996, 572 );
            this.labelNoAccess.TabIndex = 0;
            this.labelNoAccess.Text = "This screen is unavailable because you do not have permission to view this inform" +
                "ation.";
            // 
            // noAccessPanel
            // 
            this.noAccessPanel.Controls.Add( this.labelNoAccess );
            this.noAccessPanel.Location = new System.Drawing.Point( 7, 29 );
            this.noAccessPanel.Name = "noAccessPanel";
            this.noAccessPanel.Size = new System.Drawing.Size( 996, 572 );
            this.noAccessPanel.TabIndex = 8;
            // 
            // lockImage
            // 
            this.lockImage.ImageStream = ( (System.Windows.Forms.ImageListStreamer)( resources.GetObject( "lockImage.ImageStream" ) ) );
            this.lockImage.TransparentColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 94 ) ) ) ), ( (int)( ( (byte)( 137 ) ) ) ), ( (int)( ( (byte)( 185 ) ) ) ) );
            this.lockImage.Images.SetKeyName( 0, "lock_icon.gif" );
            // 
            // contextLabel
            // 
            this.contextLabel.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 128 ) ) ) ), ( (int)( ( (byte)( 162 ) ) ) ), ( (int)( ( (byte)( 200 ) ) ) ) );
            this.contextLabel.Description = " Online Preregistration";
            this.contextLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.contextLabel.ForeColor = System.Drawing.Color.White;
            this.contextLabel.Location = new System.Drawing.Point( 0, 0 );
            this.contextLabel.Model = null;
            this.contextLabel.Name = "contextLabel";
            this.contextLabel.Size = new System.Drawing.Size( 1024, 23 );
            this.contextLabel.TabIndex = 0;
            this.contextLabel.TabStop = false;
            // 
            // i_FilterWorklistView
            // 
            this.i_FilterWorklistView.BackColor = System.Drawing.Color.White;
            this.i_FilterWorklistView.Items = 0;
            this.i_FilterWorklistView.Location = new System.Drawing.Point( 7, 27 );
            this.i_FilterWorklistView.Model = null;
            this.i_FilterWorklistView.Model_WorklistSetting = null;
            this.i_FilterWorklistView.Name = "i_FilterWorklistView";
            this.i_FilterWorklistView.Size = new System.Drawing.Size( 996, 75 );
            this.i_FilterWorklistView.TabIndex = 0;
            this.i_FilterWorklistView.WorklistType = 0;
            this.i_FilterWorklistView.ResetButtonClick += new System.EventHandler( this.ResetButtonClickEvent );
            this.i_FilterWorklistView.ShowWorklistRequest += new System.EventHandler( this.ShowPreRegistrationSubmissionsRequestEvent );
            this.i_FilterWorklistView.PrintReport += new PatientAccess.UI.WorklistViews.FilterWorklistView.Print( this.PrintReportEvent );
            // 
            // btnCreateNewPatient
            // 
            this.btnCreateNewPatient.Location = new System.Drawing.Point( 848, 334 );
            this.btnCreateNewPatient.Message = null;
            this.btnCreateNewPatient.Name = "btnCreateNewPatient";
            this.btnCreateNewPatient.Size = new System.Drawing.Size( 152, 39 );
            this.btnCreateNewPatient.TabIndex = 2;
            this.btnCreateNewPatient.Text = "Create New Patient and Account From This Record";
            this.btnCreateNewPatient.UseVisualStyleBackColor = true;
            this.btnCreateNewPatient.Click += new System.EventHandler( this.btnCreateNewPatient_Click );
            // 
            // btnCreateNewAccount
            // 
            this.btnCreateNewAccount.Location = new System.Drawing.Point( 848, 558 );
            this.btnCreateNewAccount.Message = null;
            this.btnCreateNewAccount.Name = "btnCreateNewAccount";
            this.btnCreateNewAccount.Size = new System.Drawing.Size( 152, 39 );
            this.btnCreateNewAccount.TabIndex = 4;
            this.btnCreateNewAccount.Text = "Create New Account for Selected Patient";
            this.btnCreateNewAccount.UseVisualStyleBackColor = true;
            this.btnCreateNewAccount.Click += new System.EventHandler( this.btnCreateNewAccount_Click );
            // 
            // progressPanel
            // 
            this.progressPanel.BackColor = System.Drawing.Color.White;
            this.progressPanel.Location = new System.Drawing.Point( 7, 29 );
            this.progressPanel.Name = "progressPanel";
            this.progressPanel.Size = new System.Drawing.Size( 996, 300 );
            this.progressPanel.TabIndex = 3;
            // 
            // OnlinePreRegistrationSubmissionsView
            // 
            this.AcceptButton = this.btnCreateNewPatient;
            this.Controls.Add( this.contextLabel );
            this.Controls.Add( this.lblPossibleMatches );
            this.Controls.Add( this.i_FilterWorklistView );
            this.Controls.Add( this.lblAwaitingProcessing );
            this.Controls.Add( this.onlinePreRegistrationsPanel );
            this.Controls.Add( this.btnCreateNewPatient );
            this.Controls.Add( this.btnCreateNewAccount );
            this.Controls.Add( this.matchingPatientsPanel );
            this.Controls.Add( this.noAccessPanel );
            this.Controls.Add( this.progressPanel );
            this.Name = "OnlinePreRegistrationSubmissionsView";
            this.Size = new System.Drawing.Size( 1024, 619 );
            this.Leave += new System.EventHandler( this.OnlinePreRegWorklistView_Leave );
            this.Disposed += new System.EventHandler( this.OnlinePreRegWorklistView_Disposed );
            this.onlinePreRegistrationsPanel.ResumeLayout( false );
            ( (System.ComponentModel.ISupportInitialize)( this.onlinePreRegDataGrid ) ).EndInit();
            this.matchingPatientsPanel.ResumeLayout( false );
            ( (System.ComponentModel.ISupportInitialize)( this.matchingPatientsDataGrid ) ).EndInit();
            this.noAccessPanel.ResumeLayout( false );
            this.ResumeLayout( false );

        }

        #endregion

        private FilterWorklistView i_FilterWorklistView;
        private System.Windows.Forms.Panel onlinePreRegistrationsPanel;
        private ProgressPanel progressPanel;
        private System.Windows.Forms.Label labelNoOnlineSubmissions;
        private FilterEnterDataGridView onlinePreRegDataGrid;

        private UserContextView contextLabel;

        private ToolTip toolTipName;
        private LoggingButton btnCreateNewPatient;
        private FilterEnterDataGridView matchingPatientsDataGrid;
        private LoggingButton btnCreateNewAccount;
        private Label lblPossibleMatches;
        private Panel matchingPatientsPanel;
        private DataGridViewTextBoxColumn matchingPatientName;
        private DataGridViewTextBoxColumn matchingGender;
        private DataGridViewTextBoxColumn matchingDob;
        private DataGridViewTextBoxColumn matchingSsn;
        private DataGridViewTextBoxColumn matchingAddress;
        private Label lblAwaitingProcessing;
        private Label lblNoMatchingPatients;
        private ImageList lockImage;
        private Panel noAccessPanel;
        private Label labelNoAccess;
        private ProgressPanel progressPanelPatientSearch;
        private DataGridViewImageColumn RecordLock;
        private DataGridViewTextBoxColumn PatientName;
        private DataGridViewTextBoxColumn VisitedBefore;
        private DataGridViewTextBoxColumn GenderColumn;
        private DataGridViewTextBoxColumn DateOfBirth;
        private DataGridViewTextBoxColumn SsnColumn;
        private DataGridViewTextBoxColumn AddressColumn;
        private DataGridViewTextBoxColumn AdmitDate; 
    }
}
