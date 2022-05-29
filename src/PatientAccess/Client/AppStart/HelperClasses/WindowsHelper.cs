using System.Runtime.InteropServices;

namespace PatientAccess.AppStart.HelperClasses.Win32APIs
{
    /// <summary>
    /// WindowsHelper implements managed wrappers for unmanaged Win32 APIs.
    /// </summary>
    public sealed class WindowsHelper
    {
        /// <summary>
        /// Point struct used for GetWindowPlacement API.
        /// </summary>
        [StructLayout( LayoutKind.Sequential )]
        public class ManagedPt
        {
            public int x = 0;
            public int y = 0;

            public ManagedPt()
            {
            }

            public ManagedPt( int x, int y )
            {
                this.x = x;
                this.y = y;
            }
        }

        /// <summary>
        /// Rect struct used for GetWindowPlacement API.
        /// </summary>
        [StructLayout( LayoutKind.Sequential )]
        public class ManagedRect
        {

            public int x = 0;

            public int y = 0;

            public int right = 0;
            public int bottom = 0;

            public ManagedRect()
            {
            }

            public ManagedRect( int x, int y, int right, int bottom )
            {
                this.x = x;
                this.y = y;
                this.right = right;
                this.bottom = bottom;
            }
        }

        /// <summary>
        /// WindowPlacement struct used for GetWindowPlacement API.
        /// </summary>
        [StructLayout( LayoutKind.Sequential )]
        public class ManagedWindowPlacement
        {
            public uint length = 0;
            public uint flags = 0;
            public uint showCmd = 0;
            public ManagedPt minPosition = null;
            public ManagedPt maxPosition = null;
            public ManagedRect normalPosition = null;

            public ManagedWindowPlacement()
            {
                this.length = (uint)Marshal.SizeOf( this );
            }
        }

        // External Win32 APIs that gets called directly
        [DllImport( "USER32.DLL", SetLastError = true )]
        private static extern uint ShowWindow( uint hwnd, int showCommand );

        [DllImport( "USER32.DLL", SetLastError = true )]
        private static extern uint SetForegroundWindow( uint hwnd );

        [DllImport( "USER32.DLL", SetLastError = true )]
        private static extern uint GetWindowPlacement( uint hwnd,
            [In, Out]ManagedWindowPlacement lpwndpl );



        // Windows defined constants.
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int WPF_RESTORETOMAXIMIZED = 2;


        /// <summary> 
        /// Finds the specified window by its Process ID. Then brings it to 
        /// the foreground. 
        /// </summary> 
        /// <param name="hwndInstance">Handle to the window to find and activate.</param> 
        static public void ActivateWindowByHandle( uint hwndInstance )
        {
            ManagedWindowPlacement placement = new ManagedWindowPlacement();
            GetWindowPlacement( hwndInstance, placement );

            if (placement.showCmd == SW_SHOWMINIMIZED)
            {
                // if the window is minimized, then we need to restore it to its 
                // previous size. we also take into account whether it was 
                // previously maximized. 
                int showCmd = ( placement.flags == WPF_RESTORETOMAXIMIZED ) ?
                SW_SHOWMAXIMIZED : SW_SHOWNORMAL;
                ShowWindow( hwndInstance, showCmd );
            }
            else
            {
                // if it's not minimized, then we just call SetForegroundWindow to 
                // bring it to the front. 
                SetForegroundWindow( hwndInstance );
            }
        }
    }
}