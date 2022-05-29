using System.Configuration;
using SHDocVw;

namespace PatientAccess.UI.HelpViews
{
	/// <summary>
	/// Summary description for HelpService.
	/// </summary>
	public class HelpService
	{

        #region Event Handlers
        #endregion

        #region Methods

        /// <summary>
        /// Static method to display the Patient Acess help PDF file
        /// in an external browser window.
        /// </summary>
        /// <returns></returns>
        public static void DisplayHelp()
        {   
            try
            {
                webBrowserApp.Refresh();
            }
            catch
            {
                internetExplorer    = new InternetExplorerClass();
                webBrowserApp       = internetExplorer;
            }

            webBrowserApp.MenuBar   = false;
            webBrowserApp.StatusBar = false;
            webBrowserApp.ToolBar   = 1;

            object objNull  = null;
            string helpUrl;
            helpUrl = ConfigurationManager.AppSettings["PatientAccess.HelpURL"];
           
            try
            {
                webBrowserApp.Visible = true;
                webBrowserApp.Navigate( helpUrl, 
                    ref objNull, ref objNull, ref objNull, ref objNull );
            }
            catch
            {
                webBrowserApp.Quit();
                internetExplorer = null;
            }
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public HelpService()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #endregion                                

        #region Data Elements

        private static InternetExplorer internetExplorer;
        private static IWebBrowserApp   webBrowserApp;
//        private static object           c_Sync = new Object();


        #endregion

        #region Constants
        #endregion

	}
}
