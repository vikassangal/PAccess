using System;
using System.ComponentModel;
using System.Drawing;
using Extensions.UI.Winforms;

namespace PatientAccess.UI.CommonControls 
{
    /// <summary>
    /// Base ControlView to be used throughout the Patient Access Appliction
    /// It specializes the Extensions.UI.Winforms.ControlView for a consitant
    /// look and feel
    /// </summary>
    public class PatientAccessControlView : ControlView
    {
        #region Methods
        #endregion

        #region Properties
        // Immutable Properties that are hidden in the Designer

        [Browsable(false)]
        private new Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = new Font("Tahoma", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((Byte)(0)));
            }
        }

        [Browsable(false)] // Blocks it from being show in the designer
            private new Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = Color.FromArgb(((Byte)(231)), ((Byte)(231)), ((Byte)(215)));
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

        #region Construction and Finalization
        public PatientAccessControlView()
        {
            InitializeComponent();

        }
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // PatientAccessControlView
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(231)), ((System.Byte)(231)), ((System.Byte)(215)));
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.Name = "PatientAccessControlView";

        }
        #endregion

        #region Data Elements
        private Container components = null;
        #endregion
    }

}
