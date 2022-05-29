using System.ComponentModel;
using System.Windows.Forms;

namespace PatientAccess.UI.CommonControls
{
    /// <summary>
    /// The control is used on the SpecialPrograms view of the MSP Wizard so that
    /// when the Welcome screen is disabled and the user clicks on the questions
    /// user control, the first RadioButton, which must have it's TabStop property
    /// set, does not go to the checked state.  This control accepts the focus instead.
    /// To prevent the text from going to the selected state, the overridden WndProc
    /// consumes the WM_SETFOCUS message and prevents it from being sent to the base class.
    /// </summary>
    public class NonSelectableTextBox : TextBox
    {
        #region Methods
        protected override void WndProc( ref Message m )
        {
            if( m.Msg == WM_SETFOCUS )
            {
                return;
            }
            base.WndProc( ref m );
        }
        #endregion

        #region Construction
        public NonSelectableTextBox()
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

        #region Data Elements
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;
        private int WM_SETFOCUS = 0x0007;
        #endregion
    }
}
