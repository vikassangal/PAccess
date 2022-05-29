using System;
using System.ComponentModel;
using System.Windows.Forms;
using PatientAccess.UI.Logging;

namespace PatientAccess.UI.CommonControls
{
	//TODO: Create XML summary comment for LoggingButton
    public class LoggingButton : Button 
    {
        #region Event Handlers
        #endregion

        #region Methods
        protected override void OnClick(EventArgs e)
        {   
            if( this.Message == null )
            {
                string parentName = string.Empty;
                if( this.Parent != null && this.Parent.Name != null )
                {
                    parentName = this.Parent.Name;
                }
               this.Message = String.Format( "Click {0} from {1}",
                   this.Text.Replace( "&", string.Empty ), parentName );
            }
            
            BreadCrumbLogger.GetInstance.Log( this.Message );
 
            if (!base.IsDisposed && !base.Disposing)
                base.OnClick (e);
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
        public LoggingButton()
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
