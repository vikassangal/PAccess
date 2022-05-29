using System;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.InsuranceViews.MSP2
{
	/// <summary>
	/// CloseMessageBox - a standard format message box for displaying 'more info' to the user
	/// </summary>
    
    [Serializable]
    public class CloseMessageBox : TimeOutFormView
	{
        #region Event Handlers

        /// <summary>
        /// btnClose_Click - close this 'messagebox'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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
            this.lblMessageText = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblBoldText = new System.Windows.Forms.Label();
            this.pnlBackground = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // lblMessageText
            // 
            this.lblMessageText.BackColor = System.Drawing.Color.White;
            this.lblMessageText.Location = new System.Drawing.Point(15, 50);
            this.lblMessageText.Name = "lblMessageText";
            this.lblMessageText.Size = new System.Drawing.Size(370, 137);
            this.lblMessageText.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.SystemColors.Control;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(304, 202);
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
            this.lblBoldText.Size = new System.Drawing.Size(370, 18);
            this.lblBoldText.TabIndex = 2;
            // 
            // pnlBackground
            // 
            this.pnlBackground.BackColor = System.Drawing.Color.White;
            this.pnlBackground.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlBackground.Location = new System.Drawing.Point(7, 7);
            this.pnlBackground.Name = "pnlBackground";
            this.pnlBackground.Size = new System.Drawing.Size(386, 188);
            this.pnlBackground.TabIndex = 3;
            // 
            // CloseMessageBox
            // 
            this.AutoScaleMode = AutoScaleMode.None;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(400, 232);
            this.ControlBox = false;
            this.Controls.Add(this.lblBoldText);
            this.Controls.Add(this.lblMessageText);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.pnlBackground);
            this.Name = "CloseMessageBox";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "More Information";
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public CloseMessageBox()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            base.EnableThemesOn( this );

        }

        public CloseMessageBox(string boldMessage, string message)
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.lblBoldText.Text    = boldMessage;
            this.lblMessageText.Text = message;

            base.EnableThemesOn( this );
        }

        public CloseMessageBox(string message)
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.lblMessageText.Text = message;

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
        
        private Container                 components = null;

        private Button                     btnClose;

        private Label                      lblMessageText;        
        private Label                      lblBoldText;

        private Panel                      pnlBackground;

        #endregion

        #region Constants
        #endregion
	}
}
