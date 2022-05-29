using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PatientAccess.UI.CommonControls
{
    /// <summary>
    /// A customized list view that allows the column headers
    /// to be i_locked to prevent resizing.  The HeaderSizeLocked
    /// property turns on and off this functionality.  This property
    /// is active on the form designer.
    /// Developer: Chris Morgan, CodeProject open source code.
    /// </summary>
    public class LockingListView : ListView
    {
        #region Nested Class
        /// <summary>
        /// Class used to capture window messages for the header of the list view
        /// control.  
        /// </summary>
        private class HeaderControl : NativeWindow
        {
            #region Construction and Finalization
            public HeaderControl( LockingListView m )
            {
                parentListView = m;
                // Get the header control handle
                IntPtr header = SendMessage( m.Handle, (0x1000+31), IntPtr.Zero, IntPtr.Zero );
                this.AssignHandle( header );
            }
            #endregion

            #region Event Handlers
            protected override void WndProc( ref Message message )
            {
                bool callBase = true;

                switch( message.Msg )
                {
                    case WM_LBUTTONDBLCLK:
                    case WM_SETCURSOR:
                        if( parentListView.LockColumnSize )
                        {
                            //Don't change cursor to sizing cursor.  Also ignore
                            //double click, which sizes the column to fit the data.
                            message.Result = (IntPtr)1; //Return TRUE from message handler
                            callBase = false;           //Don't call the base class.
                        }
                        break;
                }

                if( callBase )
                {
                    // pass messages on to the base control for processing
                    base.WndProc( ref message );
                }
            }
            #endregion

            #region Data Elements
            private LockingListView parentListView = null;
            private const int WM_LBUTTONDBLCLK = 0x0203;
            private const int WM_SETCURSOR = 0x0020;

            [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError=true)]
            private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
            #endregion
        }
        #endregion

        #region Overridden Event Handlers
        /// <summary>
        /// When the control is created capture the messages for the header. 
        /// </summary>
        protected override void OnCreateControl()
        {
            // First actually create the control.
            base.OnCreateControl();

            // Now create the HeaderControl class to handle
            // the customization of the header messages.
            hdrCtrl = new HeaderControl(this);
        }

        /// <summary>
        /// Capture CTRL+ to prevent resize of all columns.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown( KeyEventArgs e )
        {
            if ( e.KeyValue == 107 && e.Modifiers == Keys.Control && LockColumnSize )
            {
                e.Handled = true;
            }
            else
            {
                base.OnKeyDown( e );
            }
        }

        /// <summary>
        /// Capture messages for the list view control.
        /// </summary>
        /// <param name="message"></param>
        protected override void WndProc( ref Message message )
        {
            bool callBase = true;

            switch( message.Msg )
            {
                case WM_NOTIFY:
                    NMHDR nmhdr = (NMHDR)message.GetLParam(typeof(NMHDR));

                switch( nmhdr.code )
                {
                    case HDN_BEGINTRACKA:
                    case HDN_BEGINTRACKW:
                        if ( LockColumnSize )
                        {
                            // Discard the begin tracking to prevent
                            // dragging of the column headers.
                            message.Result = (IntPtr)1;
                            callBase = false;
                        }
                        break;
                }
                    break;
            }

            if( callBase )
            {
                // pass messages on to the base control for processing
                base.WndProc( ref message );
            }
        }
        #endregion

        /// <summary>
        /// Property to turn on and off the ability to size the column headers.
        /// </summary>
        #region Properties
        [Category("Behavior"), Description("Prevent sizing of column headers.")]
        public bool LockColumnSize
        {
            get
            {
                return i_locked;
            }
            set
            {
                i_locked = value;
            }
        }
        #endregion

        #region Data Elements
        /// <summary>
        /// Notify message header structure.
        /// </summary>
        // ReSharper disable MemberCanBePrivate.Local
        [StructLayout(LayoutKind.Sequential)]
        private struct NMHDR
        {
            public IntPtr hwndFrom;

            public int idFrom;

            public int code;
        }
        // ReSharper restore MemberCanBePrivate.Local

        private const int WM_NOTIFY       = 0x004E;
        private const int HDN_FIRST       = (0-300);
        private const int HDN_BEGINTRACKA = (HDN_FIRST-6);
        private const int HDN_BEGINTRACKW = (HDN_FIRST-26);
        private HeaderControl hdrCtrl = null;
        private bool i_locked = false;
        #endregion
    }
}
