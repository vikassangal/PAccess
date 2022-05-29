namespace PatientAccess.UI.InsuranceViews.InsuranceVerificationViews
{
    partial class DebugBenefitsResponse
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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.btnPrint = new System.Windows.Forms.Button();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.pageSetupDialog1 = new System.Windows.Forms.PageSetupDialog();
            this.debugCommMgdCareVerifyView1 = new PatientAccess.UI.InsuranceViews.InsuranceVerificationViews.DebugCommMgdCareVerifyView();
            this.btnSetup = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point( 21, 12 );
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size( 1143, 609 );
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point( 989, 672 );
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size( 75, 23 );
            this.btnPrint.TabIndex = 3;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler( this.btnPrint_Click );
            // 
            // printDocument1
            // 
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler( this.printDocument1_PrintPage );
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // debugCommMgdCareVerifyView1
            // 
            this.debugCommMgdCareVerifyView1.Account = null;
            this.debugCommMgdCareVerifyView1.BackColor = System.Drawing.Color.White;
            this.debugCommMgdCareVerifyView1.Location = new System.Drawing.Point( 21, 641 );
            this.debugCommMgdCareVerifyView1.Model = null;
            this.debugCommMgdCareVerifyView1.Model_Coverage = null;
            this.debugCommMgdCareVerifyView1.Name = "debugCommMgdCareVerifyView1";
            this.debugCommMgdCareVerifyView1.Size = new System.Drawing.Size( 1143, 3027 );
            this.debugCommMgdCareVerifyView1.TabIndex = 2;
            // 
            // btnSetup
            // 
            this.btnSetup.Location = new System.Drawing.Point( 880, 671 );
            this.btnSetup.Name = "btnSetup";
            this.btnSetup.Size = new System.Drawing.Size( 75, 23 );
            this.btnSetup.TabIndex = 4;
            this.btnSetup.Text = "Setup";
            this.btnSetup.UseVisualStyleBackColor = true;
            this.btnSetup.Click += new System.EventHandler( this.btnSetup_Click );
            // 
            // DebugBenefitsResponse
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size( 1224, 840 );
            this.Controls.Add( this.btnSetup );
            this.Controls.Add( this.btnPrint );
            this.Controls.Add( this.debugCommMgdCareVerifyView1 );
            this.Controls.Add( this.richTextBox1 );
            this.Name = "DebugBenefitsResponse";
            this.Text = "DebugBenefitsResponse";
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private DebugCommMgdCareVerifyView debugCommMgdCareVerifyView1;
        private System.Windows.Forms.Button btnPrint;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.PageSetupDialog pageSetupDialog1;
        private System.Windows.Forms.Button btnSetup;
    }
}