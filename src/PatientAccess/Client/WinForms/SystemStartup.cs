using System;
using System.Collections;
using System.Configuration;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain.Parties.Exceptions;
using PatientAccess.RemotingServices;
using PatientAccess.UI.CrashReporting;
using PatientAccess.UI.ExceptionManagement;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.LogOnViews;
using PatientAccess.UI.Logging;
using log4net;

namespace PatientAccess
{
    /// <summary>
    /// System Startup for Patient Access.
    /// </summary>
    sealed class SystemStartup 
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        private static PatientAccessConfiguration Config
        {
            get
            {
                return c_Config;
            }
            set
            {
                c_Config = value;
            }
        }
        #endregion

        #region Construction and Finalization
        /// <summary>
        /// Instances of types that define only static members do not need to be created.
        /// Many compilers will automatically add a public default constructor if no 
        /// constructor is specified.  To prevent this, adding an empty private 
        /// constructor may be required.
        /// </summary>
        static SystemStartup()
        {           
        }
        #endregion
    
        #region Data Elements
        
        private static PatientAccessConfiguration c_Config = null;
        private static ILog c_log = LogManager.GetLogger( typeof( SystemStartup ) );
        #endregion

        #region Constants
       
        private const string PATIENTACCESS_CONFIGURATION    = "patientAccess";
        private const string MAX_IDLE_TIME                  = "CCMaxServicePointIdleTime";
        private const string CHECK_FOR_CROSS_THREADS        = "CCCheckForCrossThreads";
        private const string ANNOUNCEMENTS_AFTER_CRASH      = "CCAnnouncementsAfterCrash";
        private const string USE_REMOTING_COMPRESSION       = "CCUseRemotingCompression";
        private const string REMOTING_ERROR_RETRIES         = "CCRemotingErrorRetries";
        private const string REMOTING_TIMEOUT               = "CCRemotingTimeout";
        private const string APPLICATION_INACTIVITY_TIMEOUT = "CCApplicationInActivityTimeout";

        #endregion

        #region Static Methods
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() 
        {
            // Adds the ThreadException event handler to to the event.
            Application.ThreadException += new ThreadExceptionEventHandler( new ApplicationExceptionHandler().OnThreadException );

            // pull down client-side configuration values from the server (web.config)

            IClientConfigurationBroker cliConfigBroker = null;
            Hashtable ht = null;

            try
            {

                //RemotingConfigurationUtility.InitialzeDefaultRemoting();

                cliConfigBroker = BrokerFactory.BrokerOfType<IClientConfigurationBroker>();
                ht = cliConfigBroker.ConfigurationValues();

            }
            catch( VersionNotMatchedException )
            {
                MessageBox.Show( UIErrorMessages.VERSION_NOT_MATCHED, "Version is out-of-date", MessageBoxButtons.OK, MessageBoxIcon.Stop );
                return;
            }

            

            // Grab our config instance which we use to read app.config params, 
            // figure out WHERE our target app is and WHAT version.
            Config = (PatientAccessConfiguration)ConfigurationManager.GetSection( PATIENTACCESS_CONFIGURATION );

            // cache values for remoting

            lock( USE_REMOTING_COMPRESSION )
            {
                HttpRuntime.Cache.Add( USE_REMOTING_COMPRESSION, ht[USE_REMOTING_COMPRESSION].ToString(), null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null );
            }

            lock( REMOTING_ERROR_RETRIES )
            {
                HttpRuntime.Cache.Add( REMOTING_ERROR_RETRIES, ht[REMOTING_ERROR_RETRIES].ToString(), null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null );
            }

            lock( REMOTING_TIMEOUT )
            {
                HttpRuntime.Cache.Add( REMOTING_TIMEOUT, ht[REMOTING_TIMEOUT].ToString(), null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null );
            }

            lock( APPLICATION_INACTIVITY_TIMEOUT )
            {
                HttpRuntime.Cache.Add( APPLICATION_INACTIVITY_TIMEOUT, ht[APPLICATION_INACTIVITY_TIMEOUT].ToString(), null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null );
            }

            RemotingConfigurationUtility.InitializeParamaterizedRemoting(
                                            ht[REMOTING_ERROR_RETRIES].ToString(),
                                            ht[USE_REMOTING_COMPRESSION].ToString(),
                                            Int32.Parse( ht[REMOTING_TIMEOUT].ToString() ) );

            // Set up a Mutex unique to the Patient Access application
            // to check from the AppStart Bootstrapper prior to installing updates.
            // (The "Global\" is required to ensure a single instance across
            // terminal services, Citrix, and XP multi-user sessions.)
            Mutex patientAccessMutex = new Mutex( false, @"Global\" + Config.ExecutableName + Config.ApplicationID );

            BreadCrumbLogger.GetInstance.Log( " ****** Patient Access Application Started ****** ");
            BreadCrumbLogger.GetInstance.Log( "LOG FORMAT TYPE 1) LOGLEVEL, DATETIME - MESSAGE" );
            BreadCrumbLogger.GetInstance.Log( "LOG FORMAT TYPE 2) LOGLEVEL, DATETIME - ACTIVITY, ACCOUNT, MRN, MESSAGE" );

            c_log.Info( "Patient Access Application Started:  " + Config.ExecutablePath );
            
            Application.EnableVisualStyles();
            Application.DoEvents();

            // Set the time limit in milliseconds that a service point object 
            // can remain idle before it is marked eligible for garbage collection.  
            // This does not apply to connections that are created with Keep-alives disabled.

            // TLG 06/11/2007 Pull this value from the web.config file            

            string sTimeout = ht[MAX_IDLE_TIME] as string;
            int iTimeout;

            if( sTimeout != null && sTimeout != string.Empty )
            {
                iTimeout = int.Parse( sTimeout );

                if( iTimeout > 0 )
                {
                    ServicePointManager.MaxServicePointIdleTime = iTimeout;
                }
            }

            Control.CheckForIllegalCrossThreadCalls = bool.Parse( ht[CHECK_FOR_CROSS_THREADS] as string );

            // Fetch and send Crash Reports stored on the client workstation.
            try
            {
                CrashReporter crashReporter =  new CrashReporter();
                Thread thread = new Thread( new ThreadStart( crashReporter.SendCachedReports ) );
                thread.IsBackground = true;
                thread.Start();
            }
            catch( Exception ex )
            {
                c_log.Error( "SystemStartup :: CrashReporter failed to send crash reports on the client workstation.", ex );
            }

            lock( ANNOUNCEMENTS_AFTER_CRASH )
            {
                HttpRuntime.Cache.Add( ANNOUNCEMENTS_AFTER_CRASH, bool.Parse( ht[ANNOUNCEMENTS_AFTER_CRASH].ToString() ), null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null );
            }

            // Start the user interface and load the LogOn view.
            Application.Run( new LogOnView() );

            c_log.Info( "Patient Access Application Ended:  " + Config.ExecutablePath );
            BreadCrumbLogger.GetInstance.Log( " ****** Patient Access Application Ended ****** ");
        }
        #endregion
    }
}
