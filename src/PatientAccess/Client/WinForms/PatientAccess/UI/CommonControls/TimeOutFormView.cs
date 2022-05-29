using System;
using System.Timers;
using System.Web;
using System.Windows.Forms;
using PatientAccess.UI.Logging;
using PatientAccess.UI.UserInactivityMonitoring;

namespace PatientAccess.UI.CommonControls
{
    public partial class TimeOutFormView : LoggingFormView
    {
        #region Event Handlers
        /// <summary>
        /// Handles the TimeElapsed( event of the TimeOutFormView.
        /// </summary>
        private void TimeElapsed( object sender, ElapsedEventArgs e )
        {
            BreadCrumbLogger.GetInstance.Log( "Inactivity TimeOut Elapsed : " + this.Text );
            inActivityMonitor.Enabled = false;
            this.ShowWarning = false;
            this.DialogResult = DialogResult.Abort;
        }

        private void TimeOutFormView_FormClosing( object sender, FormClosingEventArgs e )
        {
            inActivityMonitor.Elapsed -= new ElapsedEventHandler( TimeElapsed );
        }

        private void TimeOutFormView_Load( object sender, EventArgs e )
        {
            inActivityMonitor.Elapsed += new ElapsedEventHandler( TimeElapsed );
        }

        #endregion

        #region Methods
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        /// <summary>
        /// Inactivity time out will be activitated and initialized with 
        /// default values when this method is called.
        /// </summary>
        protected void InitializeTimeOutActivity()
        {
            inActivityMonitor.MonitorKeyboardEvents = true;
            inActivityMonitor.MonitorMouseEvents = true;
            inActivityMonitor.SynchronizingObject = this;
            inActivityMonitor.Interval = Convert.ToDouble( HttpRuntime.Cache[APPLICATION_INACTIVITY_TIMEOUT] );
            inActivityMonitor.Enabled = true;
        }
        #endregion

        #region Private Properties

        protected bool ShowWarning
        {
            get 
            { 
                return i_ShowWarning; 
            }
            private set 
            {
                i_ShowWarning = value; 
            }
        }

        #endregion

        #region Construction and Finalization
        public TimeOutFormView()
        {
            InitializeComponent();
        }
        #endregion

        #region Data Elements
        private IInactivityMonitor inActivityMonitor = MonitorCreator.CreateInstance( MonitorType.ApplicationMonitor );
        private bool i_ShowWarning = true;
        #endregion

        #region Constants

        internal const string APPLICATION_INACTIVITY_TIMEOUT = "CCApplicationInActivityTimeout";
        #endregion                  


      }
}