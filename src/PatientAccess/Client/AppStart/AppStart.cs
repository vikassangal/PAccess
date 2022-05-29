using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.ApplicationBlocks.Updater;
using PatientAccess.AppStart.ActivationTasks;
using PatientAccess.AppStart.HelperClasses;
using PatientAccess.AppStart.HelperClasses.Win32APIs;
using log4net;

// using PatientAccess.AppStart.ActivationProcessors;

namespace PatientAccess.AppStart
{
    /// <summary>
    /// AppStart
    /// </summary>
    [Serializable]
    public sealed class AppStart 
    {
        #region Constants
        private const string PATIENTACCESS_APPSTART_CONFIGURATION = "appStart";
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public void LaunchUpdater()
        {
            try
            {
                // Load and display the Splash screen.
                Splash = new SplashScreen();
            
                if ( Splash != null )
                {
                    Splash.Show();
                    Splash.Update();
                    Application.DoEvents();
                }

                // Update the status message.
                Splash.ShowMessage( StatusMessages.INITIALIZING_STATUSMSG );
                Thread.Sleep( 500 ); //Sleep for a half-second.

                // Check for updates and process results before launching the main application - Patient Access.
                StartAppProcess();
            }
            catch ( Exception e )
            {
                HandleFatalError( e );
            }
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private void StartAppProcess() 
        {
            bool processStarted = false;

            if ( config.UpdateTime == UpdateTimeEnum.BeforeStart )
            {
                UpdatePatientAccess();
            }

            Splash.ShowMessage( StatusMessages.START_APP_STATUSMSG );
            Thread.Sleep(500); //Sleep for a half-second.
			
            // Start the Patient Access application.
            try 
            {
                ProcessStartInfo p = new ProcessStartInfo( config.ExecutablePath );
                p.WorkingDirectory = Path.GetDirectoryName( config.ExecutablePath );
                applicationProcess = Process.Start( p );
                applicationProcess.WaitForInputIdle();
                
                processStarted = true;
                Debug.WriteLine( "APPLICATION STARTER:  Started app:  " + config.ExecutablePath );
            } 
            catch (Exception e)
            {
                Debug.WriteLine( "APPLICATION STARTER:  Failed to start process at:  " + config.ExecutablePath );
                //c_log.Fatal( "Failed to start process at:  " + config.ExecutablePath, e );
                HandleFatalError( e );
            }

            if ( processStarted && config.UpdateTime == UpdateTimeEnum.AfterShutdown )
            {
                applicationProcess.WaitForExit();
                UpdatePatientAccess();
            }
        }	

        private void UpdatePatientAccess()
        {
            Splash.ShowMessage( StatusMessages.CHECKING_FOR_UPDATES_STATUSMSG );
            Thread.Sleep(500); //Sleep for a half-second.

            // Get the updater manager; The system will default to use the current application ID but it's ok to specify the appropriate GUID.
            ApplicationUpdaterManager updaterManager = ApplicationUpdaterManager.GetUpdater( config.ApplicationID );

            // Subscribe to the Download Completed event
            updaterManager.DownloadCompleted += new DownloadCompletedEventHandler( updater_DownloadCompleted );
 
            // Check remote manifest for updates.
            Manifest[] manifests = updaterManager.CheckForUpdates( config.ManifestUri );

            if( manifests.Length > 0 )
            {

                Splash.ShowMessage( StatusMessages.UPDATES_DETECTED_STATUSMSG );
                Thread.Sleep( 500 ); //Sleep for a half-second.

                // Check to see if Patient Access is currently 
                // running and bring all instances to the foreground.
                //   * Use this Global\... version to ensure a single instance even 
                //     across terminal services, Citrix or XP multi-user sessions.
                bool isOwned = false;
                Mutex appStartMutex = new Mutex( true, @"Global\" + config.ExecutableName + config.ApplicationID, out isOwned );
                if ( !isOwned )
                {

                    //Display an error dialog
                    MessageBox.Show( ErrorMessages.APPSTART_DUPLICATE_PROCESSES_DURING_UPDATE_MSG, "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information, 
                        MessageBoxDefaultButton.Button1 );

                    try 
                    { 
                        string exeName = config.ExecutableName.Replace( ".exe", string.Empty );

                        Process[] processes = Process.GetProcessesByName( exeName ); 

                        foreach (Process proc in processes) 
                        { 
                            if (proc.Id != Process.GetCurrentProcess().Id) 
                            { 
                                WindowsHelper.ActivateWindowByHandle(UInt32.Parse(proc.MainWindowHandle.ToString())); 
                            } 
                        } 
                    } 
                    catch ( InvalidOperationException oex )
                    {
                        c_log.Error( "InvalidOperationException: " + oex.Message, oex );
                    }
                    catch ( Exception ex ) 
                    { 
                        c_log.Error( "Exception: " + ex.Message, ex );
                    } 
                    finally
                    {
                        Environment.Exit( 1 );
                    }
                }

                Splash.ShowMessage( StatusMessages.DOWNLOADING_UPDATES_STATUSMSG );
                Thread.Sleep( 500 ); //Sleep for a half-second.

                foreach(Manifest manifest in manifests)
                {
                    manifest.Application.Location = Path.GetDirectoryName( config.ExecutablePath );
                    manifest.Apply = true;
                }

                //Download Updates Synchronously. (The wait period is specified in minutes.)
                //updaterManager.Download( manifests, config.MaxWaitTime );

                //Download Updates Asynchronously to allow the UI to refresh as needed.
                updaterManager.BeginDownload( manifests );

                Application.DoEvents();

                while( DownloadComplete == false )
                {
                    Thread.Sleep( 100 ); //Sleep for 1/10th of a second.
                    Application.DoEvents();
                    //Splash.Refresh(); //do this to refresh the UI
                }

                Splash.ShowMessage( StatusMessages.ACTIVATING_UPDATES_STATUSMSG );
                Thread.Sleep( 500 ); //Sleep for a half-second.
                
                updaterManager.Activate( manifests );

                //Working directory of the Patient Access.AppStart executable
                string directoryPath = Path.GetDirectoryName( config.ExecutablePath );
                
                //Extract the files from the zip archive
                string appArchive = config.ZipArchive;
                UnZipTask unZip = new UnZipTask( directoryPath );
                unZip.UnZipFile( Path.Combine( directoryPath, appArchive ) );

                //Delete the zip archive
                FileInfo archiveToDelete = new FileInfo( Path.Combine( directoryPath, appArchive ) );
                if( archiveToDelete.Exists )
                {
                    File.Delete( archiveToDelete.FullName );
                }

                Splash.ShowMessage( StatusMessages.UPDATES_COMPLETE_STATUSMSG );
                Thread.Sleep( 500 ); //Sleep for a half-second.
            }			
            else
            {
                Splash.ShowMessage( StatusMessages.NO_UPDATES_STATUSMSG );
                Thread.Sleep( 500 ); //Sleep for a half-second.

            }
        }

		private void HandleFatalError( Exception e )
		{
            //Log the error with Log4Net
			c_log.Fatal( "Exception", e );

            // Create the StringBuilder to build the error message.
            StringBuilder displayText = new StringBuilder();

            // Get all the available error fields.
            string errorSource = e.Source;
            string errorTarget = e.TargetSite.ToString();
            string errorMessage = e.Message;
            
            //Generate the error display text.
            displayText.Append( ErrorMessages.APPSTART_ERROR_MSG );
 
            //Display the error dialog
            MessageBox.Show( displayText.ToString(), "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error, 
                MessageBoxDefaultButton.Button1 );

            Environment.Exit( 0 );
		}

        private void updater_DownloadCompleted(object sender, ManifestEventArgs e)
        {
            DownloadComplete = true;
            //Splash.ShowMessage( "Download completed for manifest: " + e.Manifest.ManifestId );
            //Thread.Sleep( 500 ); //Sleep for a half-second.
        }

        // These additional events are available if needed:			
        // private void updater_DownloadStarted(object sender, DownloadStartedEventArgs e)
        // {
        //     Splash.ShowMessage( "Download started for manifest:" + e.Manifest.ManifestId );
        //     Thread.Sleep( 2000 ); //Sleep for two seconds.
        //  }
        //
        // private void updater_DownloadProgress(object sender, DownloadProgressEventArgs e)
        // {
        //     Splash.ShowMessage( "Progress:" +
        //     "- Files: "+e.FilesTransferred+"/"+e.FilesTotal+
        //     " - Bytes: "+e.BytesTransferred+"/"+e.BytesTotal );
        //     Thread.Sleep( 2000 ); //Sleep for two seconds.
        // }
        //
        // private void updater_DownloadError(object sender, ManifestErrorEventArgs e)
        // {
        //     Splash.ShowMessage( "Download error for manifest:" +"\n"+e.Exception.Message );
        //     Thread.Sleep( 2000 ); //Sleep for two seconds.
        // }
        //
        // private void updater_ActivationInitializing(object sender, ManifestEventArgs e)
        // {
        //     Splash.ShowMessage( "Activation initializing for manifest:" + e.Manifest.ManifestId );
        //     Thread.Sleep( 2000 ); //Sleep for two seconds.
        // }
        //
        // private void updater_ActivationStarted(object sender, ManifestEventArgs e)
        // {
        //    Splash.ShowMessage( "Activation started for manifest:" + e.Manifest.ManifestId );
        //    Thread.Sleep( 2000 ); //Sleep for two seconds.
        // }
        //
        // private void updater_ActivationInitializationAborted(object sender, ManifestEventArgs e)
        // {
        //     Splash.ShowMessage( "Activation initialization aborted for manifest:" + e.Manifest.ManifestId +"\n"+
        //     "The Application needs to restart for applying the updates, please restart the application." );
        //     Thread.Sleep( 2000 ); //Sleep for two seconds.
        //     MessageBox.Show( "The Application needs to restart for applying the updates, please restart the application.",
        //     "Manual Inproc Updates",MessageBoxButtons.OK,MessageBoxIcon.Information );
        // }
        //
        // private void updater_ActivationError(object sender, ManifestErrorEventArgs e)
        // {
        //     Splash.ShowMessage( "Activation error for manifest:" +"\n"+e.Exception.Message );
        //     Thread.Sleep( 2000 ); //Sleep for two seconds.
        // }
        //
        // private void updater_ActivationCompleted(object sender, ActivationCompleteEventArgs e)
        // {
        //     // UpdateList("ActivationCompleted for manifest: " + e.Manifest.ManifestId);
        //     // MessageBox.Show("ActivationCompleted for manifest: " + e.Manifest.ManifestId);
        //     Splash.ShowMessage( "ActivationCompleted for manifest: " + e.Manifest.ManifestId );
        //     Thread.Sleep( 2000 ); //Sleep for two seconds.
        // }
		#endregion

		#region Private Properties
        private SplashScreen Splash
        {
            get
            {
                return i_Splash;
            }
            set
			{
			    i_Splash = value;
			}
		}

        private static AppStartConfiguration config
		{
		    get
            {
			    return i_config;
			}
			set
			{
			    i_config = value;
			}
		}

		private Process applicationProcess
		{
		    get
			{
			    return i_applicationProcess;
			}
			set
			{
			    i_applicationProcess = value;
			}
		}

        private bool DownloadComplete
        {
            get
            {
                return i_DownloadComplete;
            }
            set
            {
                i_DownloadComplete = value;
            }
        }
        #endregion

	    #region Construction and Finalization
        static AppStart()
        {
            // Grab our config instance which we use to read in application configuration (ie. app.config) params. 
            // This object is used to figure out WHERE our target app is, WHAT version, etc.
            config = (AppStartConfiguration)ConfigurationManager.GetSection( PATIENTACCESS_APPSTART_CONFIGURATION );
	    }
        #endregion

	    #region Data Elements
        private static readonly ILog c_log = LogManager.GetLogger( typeof( AppStart ) );
        private bool i_DownloadComplete = false;
        private Process i_applicationProcess;
        private static AppStartConfiguration i_config = null;
        // This returns an instance of the Splash Screen form object.
   		private SplashScreen i_Splash;
	    // This GUID is unique to the AppStart Application.
        private static readonly Guid appStartMutexGuid = new Guid( "{5BF23A8E-A22F-4d0c-B73C-6D8243798548}" );
        #endregion

        #region Static Methods
        /// <summary>
        /// Main AppStart entry point.
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        internal static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.DoEvents();

            Mutex appStartMutex;

            //Check to see if AppStart is already running FOR the particular versioned folder of the target application
            try
            {
                bool isOwned = false;

                // Set up a Mutex unique to the Patient Access AppStart Bootstrapper 
                // application to enforce one instance at a time.
                //   * Use this Global\... version to ensure a single instance even 
                //     across terminal services, Citrix or XP multi-user sessions.
                appStartMutex = new Mutex( true, @"Global\" + config.ExecutableName + appStartMutexGuid, out isOwned );
	
                //Check to see if the mutex has already been created and terminate new updater application requests.
                if ( !isOwned )
                {
                    //Display an error dialog
                    MessageBox.Show( ErrorMessages.APPSTART_DUPLICATE_PROCESS_MSG, "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information, 
                        MessageBoxDefaultButton.Button1 );

                    Environment.Exit( 1 );
                }
            }
            catch( Exception e )
            {
                c_log.Error( "Exception: Failed when checking for running mutex (" + AppDomain.CurrentDomain.FriendlyName + ")", e );
                throw;
            }

            //Check for updates and launch Patient Access
            try
            {
                AppStart appStart = new AppStart();
                appStart.LaunchUpdater();

                //Keep the mutex reference alive until the normal termination of the Bootstrapper.
                GC.KeepAlive( appStartMutex );
            }
            catch( Exception ex )
            {
                c_log.Error("Exception: Failed to start (" + AppDomain.CurrentDomain.FriendlyName + ")", ex );
            }
        }
        #endregion
    }
}