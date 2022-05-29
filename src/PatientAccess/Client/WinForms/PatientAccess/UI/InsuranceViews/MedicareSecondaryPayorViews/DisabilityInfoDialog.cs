using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews
{
	/// <summary>
	/// Summary description for DisabilityInfoDialog.
	/// </summary>
	public class DisabilityInfoDialog : TimeOutFormView
	{
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.btnOK = new LoggingButton();
            this.backPanel = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.controlPanel.SuspendLayout();
            this.backPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(300, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "If the patient is not married, choose No-Never employed.";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton3);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Location = new System.Drawing.Point(16, 50);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(385, 70);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Example";
            // 
            // radioButton3
            // 
            this.radioButton3.Checked = true;
            this.radioButton3.Enabled = false;
            this.radioButton3.Location = new System.Drawing.Point(205, 24);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(178, 24);
            this.radioButton3.TabIndex = 0;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "No - (Spouse) Never employed";
            // 
            // radioButton2
            // 
            this.radioButton2.Enabled = false;
            this.radioButton2.Location = new System.Drawing.Point(65, 24);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(132, 24);
            this.radioButton2.TabIndex = 0;
            this.radioButton2.Text = "No - (Spouse) Retired";
            // 
            // radioButton1
            // 
            this.radioButton1.Enabled = false;
            this.radioButton1.Location = new System.Drawing.Point(10, 24);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(50, 24);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.Text = "Yes";
            // 
            // controlPanel
            // 
            this.controlPanel.BackColor = System.Drawing.Color.White;
            this.controlPanel.Controls.Add(this.groupBox1);
            this.controlPanel.Controls.Add(this.label1);
            this.controlPanel.Location = new System.Drawing.Point(1, 1);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(417, 136);
            this.controlPanel.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.Location = new System.Drawing.Point(358, 164);
            this.btnOK.Name = "btnOK";
            this.btnOK.TabIndex = 1;
            this.btnOK.TabStop = false;
            this.btnOK.Text = "&Close";
            // 
            // backPanel
            // 
            this.backPanel.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(64)), ((System.Byte)(0)), ((System.Byte)(64)));
            this.backPanel.Controls.Add(this.controlPanel);
            this.backPanel.Location = new System.Drawing.Point(16, 16);
            this.backPanel.Name = "backPanel";
            this.backPanel.Size = new System.Drawing.Size(419, 138);
            this.backPanel.TabIndex = 2;
            // 
            // DisabilityInfoDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.CancelButton = this.btnOK;
            this.ClientSize = new System.Drawing.Size(452, 196);
            this.Controls.Add(this.backPanel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DisabilityInfoDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "More Information";
            this.groupBox1.ResumeLayout(false);
            this.controlPanel.ResumeLayout(false);
            this.backPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        #region Construction and Finalization
        public DisabilityInfoDialog()
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
        private Container     components = null;
        private LoggingButton         btnOK;
        private GroupBox       groupBox1;
        private Label          label1;
        private Panel          controlPanel;
        private Panel          backPanel;
        private RadioButton    radioButton1;
        private RadioButton    radioButton2;
        private RadioButton    radioButton3;
        #endregion
    }
}
