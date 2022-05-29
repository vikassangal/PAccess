using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;

namespace PatientAccess.UI.CommonControls
{
    /// <summary>
    /// Summary description for CoverControl.
    /// </summary>
    public class CoverControlView : ControlView
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        [
        Category("CoverControl"),
        Description("Message to show when ShowCover is true")
        ]
        [DefaultValue("")]
        public string CoverMessage
        {
            get
            {
                return labelCover.Text;
            }
            set
            {
                labelCover.Text = value;
                SetShowCover( ShowCover );

            }
        }

        [
        Category("CoverControl"),
        Description("Shows CoverMessage when true.")
        ]
        [DefaultValue(false)]
        public bool ShowCover
        {
            get
            {
                return labelCover.Visible;
            }
            set
            {
                SetShowCover( value );
            }
        }

        private int i_DockPaddingEdges = 20;
        [
        Category("CoverControl"),
        Description("Padding for CoverMessage control.")
        ]
        [DefaultValue(20)]
        public int CoverPadding
        {
            get
            {
                return i_DockPaddingEdges;
            }
            set
            {
                i_DockPaddingEdges = value;
                SetShowCover( ShowCover );
            }
        }
        #endregion

        #region Private Methods
        protected virtual void SetShowCover(bool newValue)
        {
            labelCover.Visible = newValue;
            if( newValue && CoverMessage != "" )
            {
                //                    this.labelCover.Dock = System.Windows.Forms.DockStyle.Fill;
                //                    this.panelCover.DockPadding.All = 20;
                //                    this.panelCover.BringToFront();
                labelCover.Left = CoverPadding;
                labelCover.Top = CoverPadding;
                labelCover.Height = this.Height-CoverPadding*2;
                labelCover.Width = this.Width-CoverPadding*2;
                labelCover.BringToFront();
            }
            else
            {
                labelCover.Left = 0;
                labelCover.Top = 0;
                labelCover.Height = 0;
                labelCover.Width = 0;
            }
            //labelCover.Parent.Enabled = !newValue;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Constructors and Finalization
        public CoverControlView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            ShowCover = false;

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
        private Label labelCover;

        #endregion

        #region Constants
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelCover = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelCover
            // 
            this.labelCover.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelCover.Location = new System.Drawing.Point(80, 62);
            this.labelCover.Name = "labelCover";
            this.labelCover.Size = new System.Drawing.Size(243, 73);
            this.labelCover.TabIndex = 0;
            this.labelCover.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelCover.Visible = false;
            // 
            // CoverControl
            // 
            this.Controls.Add(this.labelCover);
            this.Name = "CoverControl";
            this.Size = new System.Drawing.Size(455, 283);
            this.ResumeLayout(false);

        }
        #endregion
    }
}
