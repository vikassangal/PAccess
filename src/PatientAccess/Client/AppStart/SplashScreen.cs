using System.ComponentModel;
using System.Windows.Forms;

namespace PatientAccess.AppStart
{
	/// <summary>
	/// Patient Access Splash Screen form.
	/// </summary>
	public class SplashScreen : Form
	{
        #region Event Handlers
        #endregion

        #region Methods
        public void ShowMessage( string msg )
        {
            if( !this.Visible )
            {
                this.Visible = true;
            }
            //Change the status message on the splash form and update text as events occur.
            label_StatusBox.Text = msg;

            //Do not call DoEvents... removed for performance reasons.
            //Application.DoEvents();
            label_StatusBox.Refresh();
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public SplashScreen()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        private Container components = null;
        private Label label_StatusBox;
        #endregion

        #region Constants
        #endregion

    		#region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SplashScreen));
            this.label_StatusBox = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label_StatusBox
            // 
            this.label_StatusBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label_StatusBox.BackColor = System.Drawing.Color.Transparent;
            this.label_StatusBox.CausesValidation = false;
            this.label_StatusBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.label_StatusBox.ForeColor = System.Drawing.Color.White;
            this.label_StatusBox.Location = new System.Drawing.Point(168, 8);
            this.label_StatusBox.Name = "label_StatusBox";
            this.label_StatusBox.Size = new System.Drawing.Size(400, 16);
            this.label_StatusBox.TabIndex = 1;
            this.label_StatusBox.Text = "AppStart status message...";
            this.label_StatusBox.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // SplashScreen
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(639, 346);
            this.ControlBox = false;
            this.Controls.Add(this.label_StatusBox);
            this.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SplashScreen";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Patient Access Launcher";
            this.ResumeLayout(false);
        }
        #endregion
	}
}
