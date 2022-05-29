using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PatientAccess.UI.CommonControls
{
	/// <summary>
	/// Summary description for InfoControl.
	/// </summary>
    public class InfoControl : UserControl
    {
	    private enum DisplayTypes
        {
            Info,Error
        }

        #region Methods
        public void DisplayInfoMessage( string aMessage )
        {
            Message = aMessage;
            DisplayType = DisplayTypes.Info;
        }

        public void DisplayErrorMessage( string aMessage )
        {
            Message = aMessage;
            DisplayType = DisplayTypes.Error;
        }

        #endregion

        #region Properties
        [DefaultValue( DisplayTypes.Info )]
        private DisplayTypes DisplayType
        {
            get
            {
                return i_DisplayType;
            }
            set
            {
                i_DisplayType = value;
                UpdateDisplayType();
            }
        }

        [DefaultValue( "Info" )]
        public string Message
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
        private void UpdateDisplayType()
        {
            if( DisplayType == DisplayTypes.Info )
            {
                this.label.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((Byte)(0)));
                label.ForeColor = Color.Black;
            }
            else
            {
                this.label.Font = new Font("Microsoft Sans Serif", 8.25F, ((FontStyle)((FontStyle.Bold | FontStyle.Italic))), GraphicsUnit.Point, ((Byte)(0)));
                this.label.ForeColor = Color.FromArgb(((Byte)(192)), ((Byte)(0)), ((Byte)(0)));
            }
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

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label.Location = new System.Drawing.Point(0, 0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(632, 47);
            this.label.TabIndex = 0;
            this.label.Text = "label1";
            // 
            // InfoControl
            // 
            this.Controls.Add(this.label);
            this.Name = "InfoControl";
            this.Size = new System.Drawing.Size(632, 47);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public InfoControl()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            DisplayType = DisplayTypes.Info;
            Message = "";

        }
        #endregion

        #region Data Elements
        private Label label;
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private DisplayTypes i_DisplayType;
        #endregion
    }
}
