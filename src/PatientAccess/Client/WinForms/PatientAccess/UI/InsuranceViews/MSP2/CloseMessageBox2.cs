using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;

namespace PatientAccess.UI.InsuranceViews.MSP2
{
    

	/// <summary>
	/// Summary description for CloseMessageBox.
	/// </summary>
	
    [Serializable]
    public class CloseMessageBox2 : FormView
	{        
        #region Event Handlers

        /// <summary>
        /// btnClose - close the 'messagebox'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// rbYes_CheckedChanged - make rbNoNever always checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbYes_CheckedChanged(object sender, EventArgs e)
        {
            this.rbYes.Checked              = false;
            this.rbNoNever.Checked          = true;
        }

        /// <summary>
        /// rbNoRetired_CheckedChanged - make rbNoNever always checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbNoRetired_CheckedChanged(object sender, EventArgs e)
        {
            this.rbNoRetired.Checked        = false;
            this.rbNoNever.Checked          = true;
        }

        #endregion

        #region Methods
        #endregion

        #region Properties
        #endregion

        #region Private Methods

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnClose = new System.Windows.Forms.Button();
            this.lblBoldText = new System.Windows.Forms.Label();
            this.pnlBackground = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbYes = new System.Windows.Forms.RadioButton();
            this.rbNoNever = new System.Windows.Forms.RadioButton();
            this.rbNoRetired = new System.Windows.Forms.RadioButton();
            this.pnlBackground.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.Control;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(357, 202);
            this.btnClose.Name = "btnClose";
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "&Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblBoldText
            // 
            this.lblBoldText.BackColor = System.Drawing.Color.White;
            this.lblBoldText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblBoldText.Location = new System.Drawing.Point(15, 19);
            this.lblBoldText.Name = "lblBoldText";
            this.lblBoldText.Size = new System.Drawing.Size(407, 18);
            this.lblBoldText.TabIndex = 2;
            // 
            // pnlBackground
            // 
            this.pnlBackground.BackColor = System.Drawing.Color.White;
            this.pnlBackground.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlBackground.Controls.Add(this.groupBox1);
            this.pnlBackground.Location = new System.Drawing.Point(9, 7);
            this.pnlBackground.Name = "pnlBackground";
            this.pnlBackground.Size = new System.Drawing.Size(422, 188);
            this.pnlBackground.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbYes);
            this.groupBox1.Controls.Add(this.rbNoNever);
            this.groupBox1.Controls.Add(this.rbNoRetired);
            this.groupBox1.Location = new System.Drawing.Point(46, 56);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(327, 100);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Example";
            // 
            // rbYes
            // 
            this.rbYes.Location = new System.Drawing.Point(31, 23);
            this.rbYes.Name = "rbYes";
            this.rbYes.Size = new System.Drawing.Size(47, 24);
            this.rbYes.TabIndex = 0;
            this.rbYes.Text = "Yes";
            this.rbYes.CheckedChanged += new System.EventHandler(this.rbYes_CheckedChanged);
            // 
            // rbNoNever
            // 
            this.rbNoNever.Location = new System.Drawing.Point(119, 52);
            this.rbNoNever.Name = "rbNoNever";
            this.rbNoNever.Size = new System.Drawing.Size(189, 24);
            this.rbNoNever.TabIndex = 0;
            this.rbNoNever.Text = "No - (Spouse) Never employed";
            // 
            // rbNoRetired
            // 
            this.rbNoRetired.Location = new System.Drawing.Point(119, 23);
            this.rbNoRetired.Name = "rbNoRetired";
            this.rbNoRetired.Size = new System.Drawing.Size(193, 24);
            this.rbNoRetired.TabIndex = 0;
            this.rbNoRetired.Text = "No - (Spouse) Retired";
            this.rbNoRetired.CheckedChanged += new System.EventHandler(this.rbNoRetired_CheckedChanged);
            // 
            // CloseMessageBox2
            // 
            this.AutoScaleMode = AutoScaleMode.None;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(441, 232);
            this.ControlBox = false;
            this.Controls.Add(this.lblBoldText);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.pnlBackground);
            this.Name = "CloseMessageBox2";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "More Information";
            this.pnlBackground.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        
        public CloseMessageBox2()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            base.EnableThemesOn( this );

        }

        public CloseMessageBox2(string boldMessage)
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.lblBoldText.Text       = boldMessage;
            this.rbNoNever.Checked      = true;

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
            }
            base.Dispose( disposing );
        }

        #endregion

        #region Data Elements

        private Container                     components = null;

        private Button                         btnClose;

        private Label                          lblBoldText;

        private Panel                          pnlBackground;

        private GroupBox                       groupBox1;

        private RadioButton                    rbYes;
        private RadioButton                    rbNoRetired;
        private RadioButton                    rbNoNever;

        #endregion

        #region Constants
        #endregion


	}
}
