using System.ComponentModel;
using System.Windows.Forms;

namespace PatientAccess.UI.CommonControls
{
    /// <summary>
    /// Summary description for RadioButtonKeyHandler.
    /// </summary>
    public class RadioButtonKeyHandler : RadioButton
    {
        #region Event Handlers
        protected override bool IsInputKey( Keys keyData )
        {
            bool result = false;
            switch( keyData )
            {
                case Keys.Down:
                    result = true;
                    break;
                case Keys.Right:
                    result = true;
                    break;
                case Keys.Left:
                    result = true;
                    break;
                case Keys.Up:
                    result = true;
                    break;
            }
            return result;
        }
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion

        #region Constructors and Finalization
        public RadioButtonKeyHandler()
        {
            // This call is required by the Windows.Forms Form Designer.
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
        #endregion
    }
}
