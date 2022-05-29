using System;
using System.ComponentModel;
using Extensions.UI.Winforms;
using PatientAccess.UI.Logging;

namespace PatientAccess.UI.CommonControls
{
    //TODO: Create XML summary comment for LoggingControlView
    [Serializable]
    public class LoggingControlView : ControlView
    {
        #region Event Handlers
        #endregion

        #region Methods
        protected override void OnLeave(EventArgs e)
        {
            if( this.Message != null )
            {
                BreadCrumbLogger.GetInstance.Log( String.Format( "leaving {0}", this.Message ) );                 
            }
            base.OnLeave (e);
        } 

        protected override void OnLoad(EventArgs e)
        {           
            if( this.Message != null )
            {
                BreadCrumbLogger.GetInstance.Log( String.Format( "loading {0}", this.Message ) );                 
            }
            base.OnLoad (e);
        }

        #endregion

        #region Properties
        [Browsable(true)]
        public string Message
        {
            get
            {
                return i_Message;
            }
            set
            {
                i_Message = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public LoggingControlView()
        {
        }
        #endregion

        #region Data Elements
        private string i_Message;
        #endregion

        #region Constants
        #endregion
    }
}
