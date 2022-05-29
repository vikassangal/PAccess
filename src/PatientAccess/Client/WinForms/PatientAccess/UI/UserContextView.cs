using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI
{
    /// <summary>
    /// Summary description for UserContextView.
    /// </summary>
    [Serializable]
    public class UserContextView : ControlView
    {
        #region Event Handlers
        private void UserContextView_Load( object sender, EventArgs e )
        {
            this.AdjustDisplay();
        }
        #endregion

        #region Methods
        #endregion

        #region Properties
        /// <summary>
        /// The description is the text that will provide an indication to the user
        /// as to their current context/task.
        /// </summary>
        public string Description
        {
            get
            {
                return this.userContextLabel.Text;
            }
            set
            {
                this.userContextLabel.Text = value;
            }
        }
        #endregion

        #region Private Methods
        private void AdjustDisplay()
        {
            this.userContextLabel.FlatStyle = FlatStyle.Flat;
            this.Height = userContextLabel.Height;
        }

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.userContextLabel = new GradientLabel();
			this.SuspendLayout();
			// 
			// userContextLabel
			// 
			this.userContextLabel.BackColor = Color.Empty;
			this.userContextLabel.Dock = DockStyle.Top;
			this.userContextLabel.Font = new Font("Microsoft Sans Serif", 15F, FontStyle.Bold, GraphicsUnit.Pixel, ((Byte)(0)));
			this.userContextLabel.ForeColor = Color.Black;
			this.userContextLabel.GradientColorOne = Color.FromArgb(((Byte)(128)), ((Byte)(162)), ((Byte)(200)));
			this.userContextLabel.GradientColorTwo = Color.FromArgb(((Byte)(209)), ((Byte)(228)), ((Byte)(243)));
			this.userContextLabel.GradientMode = LinearGradientMode.Vertical;
			this.userContextLabel.Location = new Point(0, 0);
			this.userContextLabel.Name = "userContextLabel";
			this.userContextLabel.Size = new Size(552, 23);
			this.userContextLabel.TabIndex = 3;
			this.userContextLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// UserContextView
			// 
			this.BackColor = SystemColors.Control;
			this.Controls.Add(this.userContextLabel);
			this.Name = "UserContextView";
			this.Size = new Size(552, 24);
			this.Load += new EventHandler(this.UserContextView_Load);
			this.ResumeLayout(false);

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

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public UserContextView()
        {
            this.InitializeComponent();
            this.AdjustDisplay();
        }

        public UserContextView( string descriptionOfContext ) : this()
        {
            this.Description = descriptionOfContext;
        }
        #endregion

        #region Data Elements
        private GradientLabel userContextLabel;
        private Container components = null;
        #endregion

        #region Constants
        #endregion
    }
}
