using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.InsuranceViews
{
    [Serializable]
	public class InsuranceDetailsTabView : ControlView
	{

        #region Construction and Finalization

		public InsuranceDetailsTabView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if (components != null) 
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #endregion

		#region Private methods

        private bool checkForError()
        {            
            bool rcErrors = false;

            rcErrors = RuleEngine.GetInstance().AccountHasFailedError();

            return rcErrors;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ( keyData == Keys.Right )
            {
            }
            else if ( keyData == Keys.Left )
            {
            }
            else
            {
                this.lastKeyPressed = string.Empty;
            }

            return base.ProcessCmdKey (ref msg, keyData);
        }

        

        protected override void WndProc(ref Message m)
        {
            const uint WM_NOTIFY        = 0x004E;
            const uint TCN_FIRST = 0xFFFFFDDA;
            const uint TCN_SELCHANGING = TCN_FIRST - 2; 

            base.WndProc(ref m);

            switch ((uint)m.Msg)
            {
                case WM_NOTIFY:
                {
                    // System.Diagnostics.Trace.WriteLine( "Yep!: " + m.Msg.ToString());

                    NMHDR nm = new NMHDR();
                    nm.hwndFrom = IntPtr.Zero;
                    nm.idFrom = 0;
                    nm.code = 0;

                    int idCtrl = (int)m.WParam;
                    NMHDR nmh = (NMHDR)m.GetLParam(typeof(NMHDR));

                    // System.Diagnostics.Trace.WriteLine( "TCN: " + nmh.code.ToString());

                    if ( nmh.code == TCN_SELCHANGING )                       
                    {   
                        if( !this.checkForError() )
                        {
                            this.Validate();
                        }
                        
                        bool rc = this.checkForError();
                        int  irc = 0;
                        if( rc )
                        {
                            irc = 1;
                        }
                        
                        Convert.ToInt32(rc);
                        m.Result = (IntPtr)irc;
                    }
                    break;
                }

                default:
                {
                    // System.Diagnostics.Trace.WriteLine( "Nope: " + m.Msg.ToString() );
                    break;
                }
            }
        }

        #endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion


        #region Data elements

        private IContainer    components = null;
        private string                              lastKeyPressed;

        #endregion

	}
}
