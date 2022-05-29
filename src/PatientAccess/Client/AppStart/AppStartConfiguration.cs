using System;
using System.IO;

namespace PatientAccess.AppStart
{
    /// <summary>
    /// Provides encapsulated view of AppStart configuration information.
    /// </summary>
    [Serializable]
    internal class AppStartConfiguration 
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        /// <summary>
        /// The name of the folder where the application was installed.
        /// </summary>
        public string FolderName
        {
            get
            {
                return i_ApplicationFolderName; 
            }
            set
            {
                i_ApplicationFolderName = value; 
            }
        }
		
        /// <summary>
        /// The name of the executable file for the application.
        /// </summary>
        public string ExecutableName
        {
            get
            {
                return i_ApplicationExecutableName; 
            }
            set
            {
                i_ApplicationExecutableName = value; 
            }
        }

        /// <summary>
        /// The id of the application.
        /// </summary>
        public string ApplicationID
        {
            get 
            {
                return i_ApplicationID;
            }
            set 
            {
                i_ApplicationID = value; 
            }
        }

        /// <summary>
        /// Specifes when the application should be run.
        /// </summary>
        public UpdateTimeEnum UpdateTime
        {
            get 
            {
                return i_WhenToRunApplicationUpdate; 
            }
            set 
            {
                i_WhenToRunApplicationUpdate =  value; 
            }
        }

        /// <summary>
        /// The URI of the manifest.
        /// </summary>
        public Uri ManifestUri
        {
            get 
            {
                return i_ManifestUri; 
            }
            set 
            { 
                i_ManifestUri = value; 
            }
        }

        public TimeSpan MaxWaitTime
        {
            get
            {
                return i_MaxWaitTime;
            }
            set
            {
                i_MaxWaitTime = value;
            }
        }

        /// <summary>
        /// The file name of the ZIP archive file for the application.
        /// </summary>
        public string ZipArchive
        {
            get
            {
                return i_ZipArchive; 
            }
            set
            {
                i_ZipArchive = value; 
            }
        }

        /// <summary>
        /// The full path for the executable of the application.
        /// </summary>
        public string ExecutablePath
        {
            get
            {
                if ( i_ApplicationExecutablePath == String.Empty )
                {
                    if ( Path.IsPathRooted( this.FolderName ) )
                    {
                        i_ApplicationExecutablePath = Path.Combine( this.FolderName, this.ExecutableName );
                    }
                    else
                    {
                        i_ApplicationExecutablePath = Path.Combine( Environment.CurrentDirectory, this.FolderName );
                        i_ApplicationExecutablePath = Path.GetFullPath( Path.Combine( i_ApplicationExecutablePath, this.ExecutableName ) );
                    }
                }
                return i_ApplicationExecutablePath;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        /// <summary>
        /// Default constructor.
        /// </summary>
        public AppStartConfiguration()
        {
        }
        #endregion

        #region Data Elements
        /// <summary>
        /// The name of the folder where the application was installed.
        /// </summary>
        private string i_ApplicationFolderName = String.Empty;

        /// <summary>
        /// The name of the executable file for the application.
        /// </summary>
        private string i_ApplicationExecutableName = String.Empty;

        /// <summary>
        /// The id of the application.
        /// </summary>
        private string i_ApplicationID;

        /// <summary>
        /// Specifes when the application should be run.
        /// </summary>
        private UpdateTimeEnum i_WhenToRunApplicationUpdate = UpdateTimeEnum.BeforeStart;

        /// <summary>
        /// The URI of the manifest.
        /// </summary>
        private Uri i_ManifestUri;

        /// <summary>
        /// Specifes the max timeout period for the downloader.
        /// </summary>
        private TimeSpan i_MaxWaitTime;

        /// <summary>
        /// Specifes the zip file namespace for the application.
        /// </summary>
        private string i_ZipArchive;

        /// <summary>
        /// The full path for the executable of the application.
        /// </summary>
        private string i_ApplicationExecutablePath = String.Empty;
        #endregion

        #region Constants
        #endregion
    }
    
    /// <summary>
    /// Specifies the options for the execution of the Updater before of after the application.
    /// </summary>
    internal enum UpdateTimeEnum
    {
        /// <summary>
        /// Check for updates before the application is executed.
        /// </summary>
        BeforeStart,

        /// <summary>
        /// Check for updates after the application is shutdown.
        /// </summary>
        AfterShutdown,
    }
}