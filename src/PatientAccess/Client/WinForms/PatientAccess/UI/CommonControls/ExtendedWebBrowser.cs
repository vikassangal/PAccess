using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace PatientAccess.UI.CommonControls
{
    
    /// <summary>
    /// Since the default WebBrowser control does not expose all the behavior
    /// of the underlying ActiveX control, this class is used to augment the
    /// control.
    /// </summary>
    public class ExtendedWebBrowser : WebBrowser
    {

		#region Fields 

        AxHost.ConnectionPointCookie _Cookie;
        WebBrowser2EventHelper _InteropHelper;

		#endregion Fields 

		#region Delegates and Events 

        public event WebBrowserNavigateErrorEventHandler NavigateError;

		#endregion Delegates and Events 

		#region Methods 

        public static void ClearSession()
        {

            InternetSetOption( IntPtr.Zero, 
                               42, //INTERNET_OPTION_END_BROWSER_SESSION
                               IntPtr.Zero, 
                               0 );

        }

        [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
        protected override void CreateSink()
        {

            base.CreateSink();

            // Create an instance of the client that will handle the event
            // and associate it with the underlying ActiveX control.
            this._InteropHelper = new WebBrowser2EventHelper(this);
            this._Cookie = new AxHost.ConnectionPointCookie(
                this.ActiveXInstance, this._InteropHelper, typeof(DWebBrowserEvents2));

        }

        [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
        protected override void DetachSink()
        {
            // Disconnect the client that handles the event
            // from the underlying ActiveX control.
            if (this._Cookie != null)
            {
                this._Cookie.Disconnect();
                this._Cookie = null;
            }
            base.DetachSink();
        }

        // Raises the NavigateError event.
        protected virtual void OnNavigateError(
            WebBrowserNavigateErrorEventArgs e)
        {
            if (this.NavigateError != null)
            {
                this.NavigateError(this, e);
            }
        }

        [DllImport( "wininet.dll" )]
        private extern static bool InternetSetOption( IntPtr hInternet,
        int Option,
        IntPtr Buffer,
        int BufferLength
        );

		#endregion Methods 

		#region Nested Classes 


        // Handles the NavigateError event from the underlying ActiveX 
        // control by raising the NavigateError event defined in this class.
        private class WebBrowser2EventHelper : 
            StandardOleMarshalObject, DWebBrowserEvents2
        {

		#region Fields 

            private ExtendedWebBrowser parent;

		#endregion Fields 

		#region Constructors 

            public WebBrowser2EventHelper(ExtendedWebBrowser parent)
            {
                this.parent = parent;
            }

		#endregion Constructors 

		#region Methods 

            public void NavigateError(object pDisp, ref object url, 
                ref object frame, ref object statusCode, ref bool cancel)
            {
                // Raise the NavigateError event.
                this.parent.OnNavigateError(
                    new WebBrowserNavigateErrorEventArgs(
                    (String)url, (String)frame, (Int32)statusCode, cancel));
            }

		#endregion Methods 



        }
		#endregion Nested Classes 

    }


    // Represents the method that will handle the NavigateError event.
    public delegate void WebBrowserNavigateErrorEventHandler(object sender, 
        WebBrowserNavigateErrorEventArgs e);


    // Provides data for the ExtendedWebBrowser.NavigateError event.
    public class WebBrowserNavigateErrorEventArgs : EventArgs
    {

		#region Fields 

        private Boolean cancelValue;
        private String frameValue;
        private Int32 statusCodeValue;
        private String urlValue;

		#endregion Fields 

		#region Constructors 

        public WebBrowserNavigateErrorEventArgs(
            String url, String frame, Int32 statusCode, Boolean cancel)
        {
            urlValue = url;
            frameValue = frame;
            statusCodeValue = statusCode;
            cancelValue = cancel;
        }

		#endregion Constructors 

		#region Properties 

        public Boolean Cancel
        {
            get { return cancelValue; }
            set { cancelValue = value; }
        }

        public String Frame
        {
            get { return frameValue; }
            set { frameValue = value; }
        }

        public Int32 StatusCode
        {
            get { return statusCodeValue; }
            set { statusCodeValue = value; }
        }

        public String Url
        {
            get { return urlValue; }
            set { urlValue = value; }
        }

		#endregion Properties 

    }


    // Imports the NavigateError method from the OLE DWebBrowserEvents2 
    // interface. 
    [ComImport, Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D"),
    InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
    TypeLibType(TypeLibTypeFlags.FHidden)]
    public interface DWebBrowserEvents2
    {
        [DispId(271)]
        void NavigateError(
            [In, MarshalAs(UnmanagedType.IDispatch)] object pDisp,
            [In] ref object URL, [In] ref object frame, 
            [In] ref object statusCode, [In, Out] ref bool cancel);
    }

}