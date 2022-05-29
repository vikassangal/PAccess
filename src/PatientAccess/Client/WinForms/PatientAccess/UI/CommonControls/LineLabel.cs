using System.ComponentModel;
using System.Windows.Forms;

namespace PatientAccess.UI.CommonControls
{
    /// <summary>
    /// Summary description for LineLabel.
    /// </summary>
    public class LineLabel : UserControl
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public string Caption
        {
            get
            {
                return label.Text;
            }
            set
            {
                label.Text = value;
            }
        }
        #endregion

        #region Private Methods
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

        #region Constructors and Finalization
        public LineLabel()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call

        }

        #endregion

        #region Data Elements
        
        private Label label;
        private Panel panelTop;
        private Panel panelLine;
        
        /// <summary> 
        /// Required designer variable.
        /// </summary>

        private Container components = null;

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
			this.panelTop = new System.Windows.Forms.Panel();
			this.panelLine = new System.Windows.Forms.Panel();
			this.label = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// panelTop
			// 
			this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelTop.Location = new System.Drawing.Point(0, 0);
			this.panelTop.Name = "panelTop";
			this.panelTop.Size = new System.Drawing.Size(476, 12);
			this.panelTop.TabIndex = 0;
			// 
			// panelLine
			// 
			this.panelLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelLine.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelLine.Location = new System.Drawing.Point(0, 12);
			this.panelLine.Name = "panelLine";
			this.panelLine.Size = new System.Drawing.Size(476, 1);
			this.panelLine.TabIndex = 1;
			// 
			// label
			// 
			this.label.AutoSize = true;
			this.label.ForeColor = System.Drawing.Color.Black;
			this.label.Location = new System.Drawing.Point(0, 2);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(35, 16);
			this.label.TabIndex = 2;
			this.label.Text = "label1";
			// 
			// LineLabel
			// 
			this.Controls.Add(this.label);
			this.Controls.Add(this.panelLine);
			this.Controls.Add(this.panelTop);
			this.Name = "LineLabel";
			this.Size = new System.Drawing.Size(476, 18);
			this.ResumeLayout(false);

		}
        #endregion

    }
}
