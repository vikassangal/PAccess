using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews
{
	/// <summary>
	/// Summary description for ESRDInfoDialog.
	/// </summary>
	public class ESRDInfoDialog : TimeOutFormView
	{
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.backPanel = new System.Windows.Forms.Panel();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new LoggingButton();
            this.backPanel.SuspendLayout();
            this.controlPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // backPanel
            // 
            this.backPanel.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(64)), ((System.Byte)(0)), ((System.Byte)(64)));
            this.backPanel.Controls.Add(this.controlPanel);
            this.backPanel.Location = new System.Drawing.Point(16, 16);
            this.backPanel.Name = "backPanel";
            this.backPanel.Size = new System.Drawing.Size(397, 170);
            this.backPanel.TabIndex = 0;
            // 
            // controlPanel
            // 
            this.controlPanel.BackColor = System.Drawing.Color.White;
            this.controlPanel.Controls.Add(this.label3);
            this.controlPanel.Controls.Add(this.label2);
            this.controlPanel.Controls.Add(this.label1);
            this.controlPanel.Location = new System.Drawing.Point(1, 1);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(395, 168);
            this.controlPanel.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(16, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(370, 52);
            this.label3.TabIndex = 0;
            this.label3.Text = "If the individual is participating in a self-dialysis training program or has a k" +
                "idney transplant during the 3-month waiting period, the 30-month coordination pe" +
                "riod starts with the first day of the month of dialysis or kidney transplant.";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(370, 42);
            this.label2.TabIndex = 0;
            this.label2.Text = "The 30-month coordination period that starts on the first day of the month an ind" +
                "ividual is eligible for Medicare (even if not yet enrolled in Medicare) because " +
                "of kidney failure (usually the fourth month of dialysis).";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(184, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "30-Month Coordination Period";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.Location = new System.Drawing.Point(337, 196);
            this.btnOK.Name = "btnOK";
            this.btnOK.TabIndex = 2;
            this.btnOK.TabStop = false;
            this.btnOK.Text = "&Close";
            // 
            // ESRDInfoDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.CancelButton = this.btnOK;
            this.ClientSize = new System.Drawing.Size(431, 228);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.backPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ESRDInfoDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "More Information";
            this.backPanel.ResumeLayout(false);
            this.controlPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        #region Construction and Finalization
        public ESRDInfoDialog()
        {
            InitializeComponent();
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
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;
        private LoggingButton     btnOK;
        private Label      label1;
        private Label      label2;
        private Label      label3;
        private Panel      backPanel;
        private Panel      controlPanel;
        #endregion
    }
}
