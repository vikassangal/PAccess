using System;
using System.ComponentModel;
using Extensions.UI.Winforms;
using PatientAccess.UI.Logging;

namespace PatientAccess.UI.CommonControls
{
    //TODO: Create XML summary comment for LoggingFormView
    [Serializable]
    public class LoggingFormView : FormView
    {
        #region Event Handlers
        #endregion

        #region Methods
        protected override void OnClosing(CancelEventArgs e)
        {
            BreadCrumbLogger.GetInstance.Log( "closing " + this.Text ); 
            base.OnClosing (e);
        }

        protected override void OnLoad(EventArgs e)
        {           
            BreadCrumbLogger.GetInstance.Log( "loading " + this.Text ); 
            base.OnLoad (e);
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public LoggingFormView()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
