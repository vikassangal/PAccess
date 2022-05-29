using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PatientAccess.UI.CommonControls
{
	/// <summary>
	/// Summary description for DateTimePickerPlus.
	/// </summary>
    public class DateTimePickerPlus : DateTimePicker
    {

        /// <summary>
        /// 
        /// </summary>
        [Description( "Set or get SuppressClick property used to determine if the control fires Click event.") ]
        [Browsable( true )]
        [DefaultValue( false )]
        public bool SuppressClick
        {
            get
            {                
                return i_SuppressClick;                 
            }
            set
            {                 
                i_SuppressClick = value;  
            }
        }

        /// <summary>
        /// Override WndProc method to implement different behaviors for the TextBox ContextMenu.        
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc( ref Message m )
        {
            bool handledMessage = false; // used to decide if call to base WndProc needed.

            switch ( m.Msg )
            {               
                case DTN_DROPDOWN :
                {
                    //may do some init for the calender
                }
                    break;
                case WM_LBUTTONDOWN :
                {
                    if( this.SuppressClick )
                    {
                        handledMessage = true;
                    }                    
                }
                    break;

            }
            if(handledMessage)
            {
                m.Result = new IntPtr( 0 ); //IntPtr.Zero
            }
            else
            {
                base.WndProc(ref m);
            }
        }

		public DateTimePickerPlus()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

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
			components = new System.ComponentModel.Container();
		}
		#endregion

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;
        private bool i_SuppressClick = false;

        private const int DTN_FIRST = -760;
        private const int DTN_DROPDOWN = (DTN_FIRST + 6);
        private const int DTN_CLOSEUP = (DTN_FIRST + 7);

        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x202;

	}
}
