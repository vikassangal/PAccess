using System;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.InsuranceViews.DOFR.Presenter;

namespace PatientAccess.UI.InsuranceViews.InsuranceVerificationViews
{
    public partial class DOFRInitiateResponseView : TimeOutFormView
    {

        #region Event Handlers

        /// <summary>
        /// Call to Dofr API
        /// </summary>
        private void DOFRInitiateResponseView_Load(object sender, EventArgs e)
        {
            if (backgroundWorker == null || !backgroundWorker.IsBusy)
            {
                BeforeWork();
                backgroundWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
                backgroundWorker.DoWork += CallDOFRAPI;
                backgroundWorker.RunWorkerCompleted += AfterWork;
                backgroundWorker.RunWorkerAsync();
            }

        }

        /// <summary>
        /// Execute the btnOk_Click 
        /// </summary>
        private void btnOk_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Properties
        private DOFRInitiatePresenter DOFRInitiatePresenter { get; set; }
        #endregion

        private void BeforeWork()
        {
            this.progressPanel1.Visible = true;
            this.progressPanel1.BringToFront();
            this.richTextBox1.SendToBack();
        }

        private void CallDOFRAPI( object sender, DoWorkEventArgs e )
        {
            if (this.backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            var response = DOFRInitiatePresenter.CallDOFRAPI();
           
           if (response !=string.Empty)
           {
                string[] stringSeparators = new string[] { "[newline]" };

                string[] res = response.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                if (res.Length == 1)
                {
                    DialogResult dialog = MessageBox.Show(res[0], "Error!",
                                   MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                   MessageBoxDefaultButton.Button1);
                    if (dialog == DialogResult.OK || dialog==DialogResult.Cancel)
                    {
                        this.Invoke(new Action(() =>
                        {
                            this.Close();
                        }));
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(res[0]))
                    {
                        richTextBox1.Invoke(new Action(() =>
                        {
                            richTextBox1.Rtf = @"{\rtf1\ansi " + res[0] +"}";
                        }));
                    }

                    if (!string.IsNullOrEmpty(res[1]))
                    {
                        lblInstructions1.Invoke(new Action(() =>
                        {
                            lblInstructions1.Text = res[1];
                        }));
                    }
                }
            }
        }
       
        private void AfterWork(object sender, RunWorkerCompletedEventArgs e)
        {

            if (this.IsDisposed || this.Disposing)
                return;

            if (e.Cancelled)
            {
            }
            else if (e.Error != null)
            {
                // handle errors/exceptions thrown from within the DoWork method here

                // When the system communicates with ACE Prior Balance service through 
                // eDV, if an error occurs which would generate a connection related 
                // crash report then PAS should handle the error gracefully by:
                // a) Not displaying a crash report
                // b) Display the screen with same behavior as when connection cannot 
                //    be made to PBAR or SOS.


            }
            else
            {
                this.progressPanel1.Visible = false;
                this.progressPanel1.SendToBack();
                this.richTextBox1.BringToFront();
            }
        }

        

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DOFRInitiateResponseView));
            this.btnOk = new PatientAccess.UI.CommonControls.LoggingButton();
            this.progressPanel1 = new PatientAccess.UI.CommonControls.ProgressPanel();
            this.lblInstructions1 = new System.Windows.Forms.Label();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(408, 201);
            this.btnOk.Margin = new System.Windows.Forms.Padding(2);
            this.btnOk.Message = null;
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(82, 23);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "OK";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = System.Drawing.Color.White;
            this.progressPanel1.Location = new System.Drawing.Point(2, 2);
            this.progressPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new System.Drawing.Size(511, 257);
            this.progressPanel1.TabIndex = 3;
            // 
            // lblInstructions1
            // 
            this.lblInstructions1.Location = new System.Drawing.Point(15, 114);
            this.lblInstructions1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblInstructions1.Name = "lblInstructions1";
            this.lblInstructions1.Size = new System.Drawing.Size(484, 47);
            this.lblInstructions1.TabIndex = 0;
            this.lblInstructions1.Text = resources.GetString("lblInstructions1.Text");
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.White;
            this.mainPanel.Controls.Add(this.progressPanel1);
            this.mainPanel.Controls.Add(this.label2);
            this.mainPanel.Controls.Add(this.label3);
            this.mainPanel.Controls.Add(this.btnOk);
            this.mainPanel.Controls.Add(this.lblInstructions1);
            this.mainPanel.Controls.Add(this.richTextBox1);
            this.mainPanel.Location = new System.Drawing.Point(22, 33);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(2);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(515, 257);
            this.mainPanel.TabIndex = 4;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Location = new System.Drawing.Point(18, 23);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.richTextBox1.Size = new System.Drawing.Size(481, 58);
            this.richTextBox1.TabIndex = 5;
            this.richTextBox1.Text = "";
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(-1, 180);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(549, 1);
            this.label2.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Location = new System.Drawing.Point(-1, 97);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(546, 1);
            this.label3.TabIndex = 3;
            // 
            // DOFRInitiateResponseView
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(228)))), ((int)(((byte)(243)))));
            this.ClientSize = new System.Drawing.Size(560, 319);
            this.Controls.Add(this.mainPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DOFRInitiateResponseView";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Division of Financial Resposibility (DOFR) Decision Analysis Tool (DAT) Results";
            this.Load += new System.EventHandler(this.DOFRInitiateResponseView_Load);
            this.mainPanel.ResumeLayout(false);
            this.ResumeLayout(false);

		}
        #endregion

        #region Construction and Finalization
        public DOFRInitiateResponseView(DOFRInitiatePresenter dOFRInitiatePresenter)
        {
            DOFRInitiatePresenter = dOFRInitiatePresenter;
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            base.EnableThemesOn( this );
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
                CancelBackgroundWorker();
            }
            base.Dispose( disposing );
        }
        private void CancelBackgroundWorker()
        {
            // cancel the background worker here...
            if (this.backgroundWorker != null)
            {
                this.backgroundWorker.CancelAsync();
            }
        }
        #endregion

        #region Data Elements
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;
       
        private LoggingButton     btnOk;
        private Label lblInstructions1;
        private Panel mainPanel;
        private Label label2;
        private Label label3;
        private ProgressPanel progressPanel1;
        private RichTextBox richTextBox1;
        private BackgroundWorker backgroundWorker;
        //private Account                         i_Model_Account;
        #endregion
    }
}