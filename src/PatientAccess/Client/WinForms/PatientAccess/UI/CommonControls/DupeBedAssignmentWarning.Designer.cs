namespace PatientAccess.UI.CommonControls
{
    partial class DupeBedAssignmentWarning
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( DupeBedAssignmentWarning ) );
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnNo = new System.Windows.Forms.Button();
            this.btnYes = new System.Windows.Forms.Button();
            this.lblText = new System.Windows.Forms.Label();
            this.lblContinue = new System.Windows.Forms.Label();
            this.lblAccountsText = new System.Windows.Forms.Label();
            this.paDataGridView1 = new PatientAccess.UI.CommonControls.PADataGridView();
            ( (System.ComponentModel.ISupportInitialize)( this.pictureBox ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize)( this.paDataGridView1 ) ).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Image = ( (System.Drawing.Image)( resources.GetObject( "pictureBox.Image" ) ) );
            this.pictureBox.Location = new System.Drawing.Point( 15, 26 );
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size( 32, 32 );
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point( 414, 200 );
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size( 75, 23 );
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler( this.btnOK_Click );
            // 
            // btnNo
            // 
            this.btnNo.Location = new System.Drawing.Point( 413, 200 );
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size( 75, 23 );
            this.btnNo.TabIndex = 3;
            this.btnNo.Text = "&No";
            this.btnNo.UseVisualStyleBackColor = true;
            this.btnNo.Click += new System.EventHandler( this.btnNo_Click );
            // 
            // btnYes
            // 
            this.btnYes.Location = new System.Drawing.Point( 331, 200 );
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size( 75, 23 );
            this.btnYes.TabIndex = 4;
            this.btnYes.Text = "&Yes";
            this.btnYes.UseVisualStyleBackColor = true;
            this.btnYes.Click += new System.EventHandler( this.btnYes_Click );
            // 
            // lblText
            // 
            this.lblText.Location = new System.Drawing.Point( 59, 17 );
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size( 428, 53 );
            this.lblText.TabIndex = 5;
            this.lblText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblContinue
            // 
            this.lblContinue.AutoSize = true;
            this.lblContinue.Location = new System.Drawing.Point( 7, 183 );
            this.lblContinue.Name = "lblContinue";
            this.lblContinue.Size = new System.Drawing.Size( 244, 13 );
            this.lblContinue.TabIndex = 6;
            this.lblContinue.Text = "Do you wish to continue with the bed assignment?";
            // 
            // lblAccountsText
            // 
            this.lblAccountsText.AutoSize = true;
            this.lblAccountsText.Location = new System.Drawing.Point( 11, 84 );
            this.lblAccountsText.Name = "lblAccountsText";
            this.lblAccountsText.Size = new System.Drawing.Size( 152, 13 );
            this.lblAccountsText.TabIndex = 7;
            this.lblAccountsText.Text = "Variable label for accounts text";
            // 
            // paDataGridView1
            // 
            this.paDataGridView1.AllowUserToAddRows = false;
            this.paDataGridView1.AllowUserToDeleteRows = false;
            this.paDataGridView1.AllowUserToResizeColumns = false;
            this.paDataGridView1.AllowUserToResizeRows = false;
            this.paDataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.paDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.paDataGridView1.Location = new System.Drawing.Point( 9, 108 );
            this.paDataGridView1.MultiSelect = false;
            this.paDataGridView1.Name = "paDataGridView1";
            this.paDataGridView1.ReadOnly = true;
            this.paDataGridView1.RowHeadersVisible = false;
            this.paDataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.paDataGridView1.ShowCellErrors = false;
            this.paDataGridView1.ShowEditingIcon = false;
            this.paDataGridView1.ShowRowErrors = false;
            this.paDataGridView1.Size = new System.Drawing.Size( 478, 65 );
            this.paDataGridView1.StandardTab = true;
            this.paDataGridView1.TabIndex = 0;
            // 
            // DupeBedAssignmentWarning
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 209 ) ) ) ), ( (int)( ( (byte)( 228 ) ) ) ), ( (int)( ( (byte)( 243 ) ) ) ) );
            this.ClientSize = new System.Drawing.Size( 499, 234 );
            this.Controls.Add( this.lblAccountsText );
            this.Controls.Add( this.lblContinue );
            this.Controls.Add( this.lblText );
            this.Controls.Add( this.btnYes );
            this.Controls.Add( this.btnNo );
            this.Controls.Add( this.btnOK );
            this.Controls.Add( this.pictureBox );
            this.Controls.Add( this.paDataGridView1 );
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DupeBedAssignmentWarning";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Warning for duplicate beds (variable)";
            ( (System.ComponentModel.ISupportInitialize)( this.pictureBox ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize)( this.paDataGridView1 ) ).EndInit();
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private PADataGridView paDataGridView1;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnNo;
        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.Label lblContinue;
        private System.Windows.Forms.Label lblAccountsText;
    }
}