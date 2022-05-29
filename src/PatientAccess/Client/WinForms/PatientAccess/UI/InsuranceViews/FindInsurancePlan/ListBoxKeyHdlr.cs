using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PatientAccess.UI.InsuranceViews.FindInsurancePlan
{
	/// <summary>
	/// Summary description for ListBoxKeyHdlr.
	/// </summary>
	public class ListBoxKeyHdlr : ListBox
	{
        #region Events
        public event EventHandler CustomKeyEvent;
        #endregion

        #region Event Handlers
        protected override bool IsInputKey( Keys keyData )
        {
            bool result = false;
            switch( keyData )
            {
                case Keys.Down:
                    result = true;
                    break;
                case Keys.Enter:
                    result = true;
                    break;
                case Keys.Up:
                    result = true;
                    break;
            }
            return result;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if( e.KeyData.Equals( Keys.Enter ) && CustomKeyEvent != null )
            {
                CustomKeyEvent( this, e );
            }
            base.OnKeyDown(e);
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
        public ListBoxKeyHdlr()
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
