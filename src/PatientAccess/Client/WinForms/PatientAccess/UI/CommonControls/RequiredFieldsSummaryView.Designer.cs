namespace PatientAccess.UI.CommonControls
{
    partial class RequiredFieldsSummaryView
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( RequiredFieldsSummaryView ) );
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblHeader = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.requiredFieldsSummary_ItemInListBindingSource = new System.Windows.Forms.BindingSource( this.components );
            this.btnOK = new System.Windows.Forms.Button();
            this.dgvActionItems = new PatientAccess.UI.CommonControls.PADataGridView();
            this.tabDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fieldDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ( (System.ComponentModel.ISupportInitialize)( this.pictureBox ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize)( this.requiredFieldsSummary_ItemInListBindingSource ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize)( this.dgvActionItems ) ).BeginInit();
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.Location = new System.Drawing.Point( 50, 9 );
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size( 455, 53 );
            this.lblHeader.TabIndex = 0;
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBox
            // 
            this.pictureBox.Image = ( (System.Drawing.Image)( resources.GetObject( "pictureBox.Image" ) ) );
            this.pictureBox.Location = new System.Drawing.Point( 12, 24 );
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size( 32, 32 );
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // requiredFieldsSummary_ItemInListBindingSource
            // 
            this.requiredFieldsSummary_ItemInListBindingSource.DataSource = typeof( PatientAccess.UI.CommonControls.RequiredFieldItem );
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point( 430, 300 );
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size( 75, 23 );
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler( this.btnOK_Click );
            // 
            // dgvActionItems
            // 
            this.dgvActionItems.AllowUserToAddRows = false;
            this.dgvActionItems.AllowUserToDeleteRows = false;
            this.dgvActionItems.AllowUserToResizeColumns = false;
            this.dgvActionItems.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding( 0, 5, 0, 5 );
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvActionItems.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvActionItems.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvActionItems.AutoGenerateColumns = false;
            this.dgvActionItems.BackgroundColor = System.Drawing.Color.White;
            this.dgvActionItems.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dgvActionItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvActionItems.Columns.AddRange( new System.Windows.Forms.DataGridViewColumn[] {
            this.tabDataGridViewTextBoxColumn,
            this.fieldDataGridViewTextBoxColumn} );
            this.dgvActionItems.DataSource = this.requiredFieldsSummary_ItemInListBindingSource;
            this.dgvActionItems.Location = new System.Drawing.Point( 8, 72 );
            this.dgvActionItems.MultiSelect = false;
            this.dgvActionItems.Name = "dgvActionItems";
            this.dgvActionItems.ReadOnly = true;
            this.dgvActionItems.RowHeadersVisible = false;
            this.dgvActionItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvActionItems.ShowCellErrors = false;
            this.dgvActionItems.ShowEditingIcon = false;
            this.dgvActionItems.ShowRowErrors = false;
            this.dgvActionItems.Size = new System.Drawing.Size( 495, 221 );
            this.dgvActionItems.StandardTab = true;
            this.dgvActionItems.TabIndex = 2;
            this.dgvActionItems.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler( this.dgvActionItems_CellMouseDoubleClick );
            // 
            // tabDataGridViewTextBoxColumn
            // 
            this.tabDataGridViewTextBoxColumn.DataPropertyName = "Tab";
            this.tabDataGridViewTextBoxColumn.HeaderText = "Field Location";
            this.tabDataGridViewTextBoxColumn.Name = "tabDataGridViewTextBoxColumn";
            this.tabDataGridViewTextBoxColumn.ReadOnly = true;
            this.tabDataGridViewTextBoxColumn.Width = 225;
            // 
            // fieldDataGridViewTextBoxColumn
            // 
            this.fieldDataGridViewTextBoxColumn.DataPropertyName = "Field";
            this.fieldDataGridViewTextBoxColumn.HeaderText = "Required Field";
            this.fieldDataGridViewTextBoxColumn.Name = "fieldDataGridViewTextBoxColumn";
            this.fieldDataGridViewTextBoxColumn.ReadOnly = true;
            this.fieldDataGridViewTextBoxColumn.Width = 248;
            // 
            // RequiredFieldsSummaryView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.ClientSize = new System.Drawing.Size( 511, 329 );
            this.Controls.Add( this.btnOK );
            this.Controls.Add( this.dgvActionItems );
            this.Controls.Add( this.pictureBox );
            this.Controls.Add( this.lblHeader );
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RequiredFieldsSummaryView";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler( this.RequiredFieldsSummary_Load );
            ( (System.ComponentModel.ISupportInitialize)( this.pictureBox ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize)( this.requiredFieldsSummary_ItemInListBindingSource ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize)( this.dgvActionItems ) ).EndInit();
            this.ResumeLayout( false );

        }

        void RequiredFieldsSummary_Load( object sender, System.EventArgs e )
        {
            this.btnOK.Focus();
            this.dgvActionItems.DoEnterAction += new System.EventHandler( dgvActionItems_DoEnterAction );
        }

        void dgvActionItems_DoEnterAction( object sender, System.EventArgs e )
        {
            this.btnOK_Click( this, null );
        }

        #endregion

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.PictureBox pictureBox;
        internal PADataGridView dgvActionItems;
        private System.Windows.Forms.BindingSource requiredFieldsSummary_ItemInListBindingSource;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.DataGridViewTextBoxColumn tabDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fieldDataGridViewTextBoxColumn;
    }
}