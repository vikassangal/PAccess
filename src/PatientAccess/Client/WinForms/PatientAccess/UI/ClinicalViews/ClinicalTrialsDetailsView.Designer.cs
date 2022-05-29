namespace PatientAccess.UI.ClinicalViews
{
    partial class ClinicalTrialsDetailsView
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClinicalTrialsDetailsView));
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new PatientAccess.UI.CommonControls.AlwaysShowingVerticalScrollBarDataGridView();
            this.ResearchSponsor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ResearchStudyCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Registrynumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnEnrollWithoutConsent = new System.Windows.Forms.Button();
            this.btnEnrollWithConsent = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRemoveSelectedStudy = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dataGridView2 = new PatientAccess.UI.CommonControls.FilterEnterDataGridView();
            this.PatientResearchStudyCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PatientStudyDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PatientResearchSponsor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.registry = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProofOfConsent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rootTable = new System.Windows.Forms.TableLayoutPanel();
            this.studiesToSelectTable = new System.Windows.Forms.TableLayoutPanel();
            this.studiesToSelectCommandsPanel = new System.Windows.Forms.Panel();
            this.showExpiredStudiesCheckBox = new System.Windows.Forms.CheckBox();
            this.expandCollapseStudiesToSelectButton = new System.Windows.Forms.Button();
            this.expandCollapseImageList = new System.Windows.Forms.ImageList(this.components);
            this.selectedStudiesPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.rootTable.SuspendLayout();
            this.studiesToSelectTable.SuspendLayout();
            this.studiesToSelectCommandsPanel.SuspendLayout();
            this.selectedStudiesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(770, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please select appropriate study and then click one of the enroll buttons to add i" +
    "t to the patient\'s account:";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ResearchSponsor,
            this.Description,
            this.ResearchStudyCode,
            this.Registrynumber});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 38);
            this.dataGridView1.MaximumSize = new System.Drawing.Size(762, 487);
            this.dataGridView1.MinimumSize = new System.Drawing.Size(762, 110);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 340;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(762, 134);
            this.dataGridView1.StandardTab = true;
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // ResearchSponsor
            // 
            this.ResearchSponsor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ResearchSponsor.DataPropertyName = "ResearchSponsor";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.ResearchSponsor.DefaultCellStyle = dataGridViewCellStyle2;
            this.ResearchSponsor.HeaderText = "Research Sponsor";
            this.ResearchSponsor.MinimumWidth = 185;
            this.ResearchSponsor.Name = "ResearchSponsor";
            this.ResearchSponsor.ReadOnly = true;
            this.ResearchSponsor.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ResearchSponsor.Width = 185;
            // 
            // Description
            // 
            this.Description.DataPropertyName = "Description";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.Description.DefaultCellStyle = dataGridViewCellStyle3;
            this.Description.HeaderText = "Description";
            this.Description.MinimumWidth = 310;
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            this.Description.Width = 310;
            // 
            // ResearchStudyCode
            // 
            this.ResearchStudyCode.DataPropertyName = "ResearchStudyCode";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.ResearchStudyCode.DefaultCellStyle = dataGridViewCellStyle4;
            this.ResearchStudyCode.HeaderText = "Research Study Code";
            this.ResearchStudyCode.MinimumWidth = 138;
            this.ResearchStudyCode.Name = "ResearchStudyCode";
            this.ResearchStudyCode.ReadOnly = true;
            this.ResearchStudyCode.Width = 138;
            // 
            // Registrynumber
            // 
            this.Registrynumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Registrynumber.DataPropertyName = "RegistryNumber";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.Registrynumber.DefaultCellStyle = dataGridViewCellStyle5;
            this.Registrynumber.HeaderText = "Registry Number";
            this.Registrynumber.MinimumWidth = 100;
            this.Registrynumber.Name = "Registrynumber";
            this.Registrynumber.ReadOnly = true;
            this.Registrynumber.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Registrynumber.Width = 115;
            // 
            // btnEnrollWithoutConsent
            // 
            this.btnEnrollWithoutConsent.AutoSize = true;
            this.btnEnrollWithoutConsent.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnEnrollWithoutConsent.BackColor = System.Drawing.SystemColors.Control;
            this.btnEnrollWithoutConsent.Location = new System.Drawing.Point(276, 6);
            this.btnEnrollWithoutConsent.Name = "btnEnrollWithoutConsent";
            this.btnEnrollWithoutConsent.Size = new System.Drawing.Size(191, 23);
            this.btnEnrollWithoutConsent.TabIndex = 3;
            this.btnEnrollWithoutConsent.Text = "Enroll in Selected Study: No Consent";
            this.btnEnrollWithoutConsent.UseVisualStyleBackColor = false;
            this.btnEnrollWithoutConsent.Click += new System.EventHandler(this.btnEnrollWithoutConsent_Click);
            // 
            // btnEnrollWithConsent
            // 
            this.btnEnrollWithConsent.AutoSize = true;
            this.btnEnrollWithConsent.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnEnrollWithConsent.BackColor = System.Drawing.SystemColors.Control;
            this.btnEnrollWithConsent.Location = new System.Drawing.Point(490, 6);
            this.btnEnrollWithConsent.Name = "btnEnrollWithConsent";
            this.btnEnrollWithConsent.Size = new System.Drawing.Size(219, 23);
            this.btnEnrollWithConsent.TabIndex = 4;
            this.btnEnrollWithConsent.Text = "Enroll in Selected Study: Consent Provided";
            this.btnEnrollWithConsent.UseVisualStyleBackColor = false;
            this.btnEnrollWithConsent.Click += new System.EventHandler(this.btnEnrollWithConsent_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 13);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 0, 3, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(219, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Research Studies Selected (Maximum of 10):";
            // 
            // btnRemoveSelectedStudy
            // 
            this.btnRemoveSelectedStudy.BackColor = System.Drawing.SystemColors.Control;
            this.btnRemoveSelectedStudy.Enabled = false;
            this.btnRemoveSelectedStudy.Location = new System.Drawing.Point(592, 291);
            this.btnRemoveSelectedStudy.Name = "btnRemoveSelectedStudy";
            this.btnRemoveSelectedStudy.Size = new System.Drawing.Size(173, 23);
            this.btnRemoveSelectedStudy.TabIndex = 7;
            this.btnRemoveSelectedStudy.Text = "Remove Selected Study";
            this.btnRemoveSelectedStudy.UseVisualStyleBackColor = false;
            this.btnRemoveSelectedStudy.Click += new System.EventHandler(this.btnRemoveSelectedStudy_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.SystemColors.Control;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(592, 327);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(690, 327);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.AllowUserToResizeColumns = false;
            this.dataGridView2.AllowUserToResizeRows = false;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView2.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PatientResearchStudyCode,
            this.PatientStudyDescription,
            this.PatientResearchSponsor,
            this.registry,
            this.ProofOfConsent});
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView2.DefaultCellStyle = dataGridViewCellStyle14;
            this.dataGridView2.Location = new System.Drawing.Point(0, 36);
            this.dataGridView2.MultiSelect = false;
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView2.RowHeadersDefaultCellStyle = dataGridViewCellStyle15;
            this.dataGridView2.RowHeadersVisible = false;
            this.dataGridView2.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridView2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView2.Size = new System.Drawing.Size(762, 240);
            this.dataGridView2.StandardTab = true;
            this.dataGridView2.TabIndex = 6;
            this.dataGridView2.SelectionChanged += new System.EventHandler(this.dataGridView2_SelectionChanged);
            // 
            // PatientResearchStudyCode
            // 
            this.PatientResearchStudyCode.DataPropertyName = "Code";
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.PatientResearchStudyCode.DefaultCellStyle = dataGridViewCellStyle9;
            this.PatientResearchStudyCode.HeaderText = "Research Study Code";
            this.PatientResearchStudyCode.MinimumWidth = 120;
            this.PatientResearchStudyCode.Name = "PatientResearchStudyCode";
            this.PatientResearchStudyCode.ReadOnly = true;
            this.PatientResearchStudyCode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.PatientResearchStudyCode.Width = 120;
            // 
            // PatientStudyDescription
            // 
            this.PatientStudyDescription.DataPropertyName = "Description";
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.PatientStudyDescription.DefaultCellStyle = dataGridViewCellStyle10;
            this.PatientStudyDescription.HeaderText = "Description";
            this.PatientStudyDescription.MinimumWidth = 260;
            this.PatientStudyDescription.Name = "PatientStudyDescription";
            this.PatientStudyDescription.ReadOnly = true;
            this.PatientStudyDescription.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.PatientStudyDescription.Width = 260;
            // 
            // PatientResearchSponsor
            // 
            this.PatientResearchSponsor.DataPropertyName = "ResearchSponsor";
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.PatientResearchSponsor.DefaultCellStyle = dataGridViewCellStyle11;
            this.PatientResearchSponsor.HeaderText = "Research Sponsor";
            this.PatientResearchSponsor.MinimumWidth = 160;
            this.PatientResearchSponsor.Name = "PatientResearchSponsor";
            this.PatientResearchSponsor.ReadOnly = true;
            this.PatientResearchSponsor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.PatientResearchSponsor.Width = 160;
            // 
            // registry
            // 
            this.registry.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.registry.DataPropertyName = "RegistryNumber";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.registry.DefaultCellStyle = dataGridViewCellStyle12;
            this.registry.HeaderText = "Registry Number";
            this.registry.MinimumWidth = 100;
            this.registry.Name = "registry";
            this.registry.ReadOnly = true;
            this.registry.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ProofOfConsent
            // 
            this.ProofOfConsent.DataPropertyName = "ProofOfConsent";
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.ProofOfConsent.DefaultCellStyle = dataGridViewCellStyle13;
            this.ProofOfConsent.HeaderText = "Proof of Consent?";
            this.ProofOfConsent.MinimumWidth = 120;
            this.ProofOfConsent.Name = "ProofOfConsent";
            this.ProofOfConsent.ReadOnly = true;
            this.ProofOfConsent.Width = 120;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewTextBoxColumn1.DataPropertyName = "ResearchSponsor";
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle16;
            this.dataGridViewTextBoxColumn1.HeaderText = "Research Sponsor";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 250;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn1.Width = 250;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Description";
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle17;
            this.dataGridViewTextBoxColumn2.HeaderText = "Description";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 290;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn2.Width = 290;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "Code";
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle18;
            this.dataGridViewTextBoxColumn3.HeaderText = "Research Study Code";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 185;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn3.Width = 185;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "ResearchStudy.Code";
            dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.dataGridViewTextBoxColumn4.DefaultCellStyle = dataGridViewCellStyle19;
            this.dataGridViewTextBoxColumn4.HeaderText = "Research Study Code";
            this.dataGridViewTextBoxColumn4.MinimumWidth = 150;
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn4.Width = 150;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "ResearchStudy.Description";
            dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.dataGridViewTextBoxColumn5.DefaultCellStyle = dataGridViewCellStyle20;
            this.dataGridViewTextBoxColumn5.HeaderText = "Description";
            this.dataGridViewTextBoxColumn5.MinimumWidth = 300;
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn5.Width = 300;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.DataPropertyName = "ResearchSponsor";
            dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.dataGridViewTextBoxColumn6.DefaultCellStyle = dataGridViewCellStyle21;
            this.dataGridViewTextBoxColumn6.HeaderText = "Research Sponsor";
            this.dataGridViewTextBoxColumn6.MinimumWidth = 150;
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            this.dataGridViewTextBoxColumn6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn6.Width = 150;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.DataPropertyName = "ProofOfConsent";
            dataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            this.dataGridViewTextBoxColumn7.DefaultCellStyle = dataGridViewCellStyle22;
            this.dataGridViewTextBoxColumn7.HeaderText = "Proof Of Consent?";
            this.dataGridViewTextBoxColumn7.MinimumWidth = 130;
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            this.dataGridViewTextBoxColumn7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn7.Width = 130;
            // 
            // rootTable
            // 
            this.rootTable.ColumnCount = 1;
            this.rootTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootTable.Controls.Add(this.studiesToSelectTable, 0, 0);
            this.rootTable.Controls.Add(this.selectedStudiesPanel, 0, 1);
            this.rootTable.Dock = System.Windows.Forms.DockStyle.Top;
            this.rootTable.Location = new System.Drawing.Point(0, 0);
            this.rootTable.Name = "rootTable";
            this.rootTable.RowCount = 2;
            this.rootTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rootTable.Size = new System.Drawing.Size(782, 580);
            this.rootTable.TabIndex = 8;
            // 
            // studiesToSelectTable
            // 
            this.studiesToSelectTable.AutoSize = true;
            this.studiesToSelectTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.studiesToSelectTable.ColumnCount = 1;
            this.studiesToSelectTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.studiesToSelectTable.Controls.Add(this.label1, 0, 0);
            this.studiesToSelectTable.Controls.Add(this.dataGridView1, 0, 1);
            this.studiesToSelectTable.Controls.Add(this.studiesToSelectCommandsPanel, 0, 2);
            this.studiesToSelectTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.studiesToSelectTable.Location = new System.Drawing.Point(3, 3);
            this.studiesToSelectTable.Name = "studiesToSelectTable";
            this.studiesToSelectTable.RowCount = 3;
            this.studiesToSelectTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.studiesToSelectTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.studiesToSelectTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.studiesToSelectTable.Size = new System.Drawing.Size(776, 215);
            this.studiesToSelectTable.TabIndex = 0;
            // 
            // studiesToSelectCommandsPanel
            // 
            this.studiesToSelectCommandsPanel.AutoSize = true;
            this.studiesToSelectCommandsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.studiesToSelectCommandsPanel.Controls.Add(this.showExpiredStudiesCheckBox);
            this.studiesToSelectCommandsPanel.Controls.Add(this.expandCollapseStudiesToSelectButton);
            this.studiesToSelectCommandsPanel.Controls.Add(this.btnEnrollWithoutConsent);
            this.studiesToSelectCommandsPanel.Controls.Add(this.btnEnrollWithConsent);
            this.studiesToSelectCommandsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.studiesToSelectCommandsPanel.Location = new System.Drawing.Point(3, 178);
            this.studiesToSelectCommandsPanel.Name = "studiesToSelectCommandsPanel";
            this.studiesToSelectCommandsPanel.Size = new System.Drawing.Size(770, 34);
            this.studiesToSelectCommandsPanel.TabIndex = 2;
            // 
            // showExpiredStudiesCheckBox
            // 
            this.showExpiredStudiesCheckBox.AutoSize = true;
            this.showExpiredStudiesCheckBox.Location = new System.Drawing.Point(3, 9);
            this.showExpiredStudiesCheckBox.Name = "showExpiredStudiesCheckBox";
            this.showExpiredStudiesCheckBox.Size = new System.Drawing.Size(129, 17);
            this.showExpiredStudiesCheckBox.TabIndex = 2;
            this.showExpiredStudiesCheckBox.Text = "Show Expired Studies";
            this.showExpiredStudiesCheckBox.UseVisualStyleBackColor = true;
            this.showExpiredStudiesCheckBox.CheckedChanged += new System.EventHandler(this.showExpiredStudiesCheckBox_CheckedChanged);
            // 
            // expandCollapseStudiesToSelectButton
            // 
            this.expandCollapseStudiesToSelectButton.AutoSize = true;
            this.expandCollapseStudiesToSelectButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.expandCollapseStudiesToSelectButton.BackColor = System.Drawing.SystemColors.Control;
            this.expandCollapseStudiesToSelectButton.ImageIndex = 0;
            this.expandCollapseStudiesToSelectButton.ImageList = this.expandCollapseImageList;
            this.expandCollapseStudiesToSelectButton.Location = new System.Drawing.Point(740, 6);
            this.expandCollapseStudiesToSelectButton.Name = "expandCollapseStudiesToSelectButton";
            this.expandCollapseStudiesToSelectButton.Size = new System.Drawing.Size(22, 22);
            this.expandCollapseStudiesToSelectButton.TabIndex = 5;
            this.expandCollapseStudiesToSelectButton.UseVisualStyleBackColor = false;
            this.expandCollapseStudiesToSelectButton.Click += new System.EventHandler(this.expandCollapseStudiesToSelectButton_Click);
            // 
            // expandCollapseImageList
            // 
            this.expandCollapseImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("expandCollapseImageList.ImageStream")));
            this.expandCollapseImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.expandCollapseImageList.Images.SetKeyName(0, "DownArrows.png");
            this.expandCollapseImageList.Images.SetKeyName(1, "UpArrows.png");
            // 
            // selectedStudiesPanel
            // 
            this.selectedStudiesPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.selectedStudiesPanel.Controls.Add(this.label2);
            this.selectedStudiesPanel.Controls.Add(this.btnCancel);
            this.selectedStudiesPanel.Controls.Add(this.btnOK);
            this.selectedStudiesPanel.Controls.Add(this.btnRemoveSelectedStudy);
            this.selectedStudiesPanel.Controls.Add(this.dataGridView2);
            this.selectedStudiesPanel.Location = new System.Drawing.Point(3, 224);
            this.selectedStudiesPanel.Name = "selectedStudiesPanel";
            this.selectedStudiesPanel.Size = new System.Drawing.Size(776, 353);
            this.selectedStudiesPanel.TabIndex = 9;
            // 
            // ClinicalTrialsDetailsView
            // 
            this.AcceptButton = this.btnCancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 588);
            this.Controls.Add(this.rootTable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ClinicalTrialsDetailsView";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Clinical Research Studies";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClinicalTrialsDetailsView_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.rootTable.ResumeLayout(false);
            this.rootTable.PerformLayout();
            this.studiesToSelectTable.ResumeLayout(false);
            this.studiesToSelectTable.PerformLayout();
            this.studiesToSelectCommandsPanel.ResumeLayout(false);
            this.studiesToSelectCommandsPanel.PerformLayout();
            this.selectedStudiesPanel.ResumeLayout(false);
            this.selectedStudiesPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private CommonControls.AlwaysShowingVerticalScrollBarDataGridView dataGridView1;
        private System.Windows.Forms.Button btnEnrollWithoutConsent;
        private System.Windows.Forms.Button btnEnrollWithConsent;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnRemoveSelectedStudy;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private CommonControls.FilterEnterDataGridView dataGridView2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn PatientResearchStudyCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn PatientStudyDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn PatientResearchSponsor;
        private System.Windows.Forms.DataGridViewTextBoxColumn registry;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProofOfConsent;
        private System.Windows.Forms.DataGridViewTextBoxColumn ResearchSponsor;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewTextBoxColumn ResearchStudyCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn Registrynumber;
        private System.Windows.Forms.TableLayoutPanel rootTable;
        private System.Windows.Forms.TableLayoutPanel studiesToSelectTable;
        private System.Windows.Forms.Panel studiesToSelectCommandsPanel;
        private System.Windows.Forms.Panel selectedStudiesPanel;
        private System.Windows.Forms.Button expandCollapseStudiesToSelectButton;
        private System.Windows.Forms.ImageList expandCollapseImageList;
        private System.Windows.Forms.CheckBox showExpiredStudiesCheckBox;


    }
}