using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace PatientAccess.UI.CrashReporting 
{
    /// <summary> 
    /// Summary description for ScreenCaptureTool 
    /// </summary>
    [Serializable]
    public class ScreenCaptureTool : IScreenCaptureTool
    {
        #region Event Handlers
        #endregion

        #region Methods

        public Bitmap GetScreenCapture()
        {
            Bitmap screenCapture = null;
            int hdcSrc = int.MinValue,
                hdcDest = int.MinValue, 
                hBitmap = int.MinValue;

            try
            {
                hdcSrc = User32.GetWindowDC( User32.GetDesktopWindow() );
                hdcDest = GDI32.CreateCompatibleDC( hdcSrc );
                hBitmap = GDI32.CreateCompatibleBitmap( hdcSrc,
                                                        GDI32.GetDeviceCaps(hdcSrc,8), 
                                                        GDI32.GetDeviceCaps(hdcSrc,10) );
                GDI32.SelectObject( hdcDest, hBitmap );
                GDI32.BitBlt(hdcDest,0,0,GDI32.GetDeviceCaps( hdcSrc, 8 ),
                             GDI32.GetDeviceCaps(hdcSrc,10),
                             hdcSrc,0,0,0x00CC0020);

                screenCapture = 
                    new Bitmap(Image.FromHbitmap(new IntPtr(hBitmap)),
                    Image.FromHbitmap(new IntPtr(hBitmap)).Width,
                    Image.FromHbitmap(new IntPtr(hBitmap)).Height);

            }
            finally
            {
                if( hdcSrc != int.MinValue )
                {
                    User32.ReleaseDC( User32.GetDesktopWindow(), hdcSrc );
                }
                if( hdcDest != int.MinValue )
                {
                    GDI32.DeleteDC( hdcDest );
                }
                if( hBitmap != int.MinValue )
                {
                    GDI32.DeleteObject( hBitmap );
                }
            }
            return screenCapture;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        
        public virtual void Dispose()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }

    class GDI32
    {
        [DllImport("GDI32.dll")]
        public static extern bool BitBlt(int hdcDest,int nXDest,int nYDest,
            int nWidth,int nHeight,int hdcSrc,
            int nXSrc,int nYSrc,int dwRop);
        [DllImport("GDI32.dll")]
        public static extern int CreateCompatibleBitmap(int hdc,int nWidth, 
            int nHeight);
        [DllImport("GDI32.dll")]
        public static extern int CreateCompatibleDC(int hdc);
        [DllImport("GDI32.dll")]
        public static extern bool DeleteDC(int hdc);
        [DllImport("GDI32.dll")]
        public static extern bool DeleteObject(int hObject);
        [DllImport("GDI32.dll")]
        public static extern int GetDeviceCaps(int hdc,int nIndex);
        [DllImport("GDI32.dll")]
        public static extern int SelectObject(int hdc,int hgdiobj);
    }

    class User32
    {
        [DllImport("User32.dll")]
        public static extern int GetDesktopWindow();
        [DllImport("User32.dll")]
        public static extern int GetWindowDC(int hWnd);
        [DllImport("User32.dll")]
        public static extern int ReleaseDC(int hWnd,int hDC);
    }
}