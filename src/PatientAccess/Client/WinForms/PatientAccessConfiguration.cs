using System;
using System.IO;

namespace PatientAccess
{
    /// <summary>
    /// Provides encapsulated view of the PatientAccess configuration information.
    /// </summary>
    [Serializable]
    internal class PatientAccessConfiguration 
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        /// <summary>
        /// The name of the application.
        /// </summary>
        public string AppName
        {
            get
            {
                return i_AppName;
            }
            set
            {
                i_AppName = value;
            }
        }

        /// <summary>
        /// The name of the folder where the application was installed.
        /// </summary>
        public string AppFolderName
        {
            private get
            {
                return i_AppFolderName; 
            }
            set
            {
                i_AppFolderName = value; 
            }
        }
		
        /// <summary>
        /// The name of the executable file for the application.
        /// </summary>
        public string ExecutableName
        {
            get
            {
                return i_ExecutableName; 
            }
            set
            {
                i_ExecutableName = value; 
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
        /// The full path for the executable of the application.
        /// </summary>
        public string ExecutablePath
        {
            get
            {
                if ( i_applicationExecutablePath == String.Empty )
                {
                    if ( Path.IsPathRooted( this.AppFolderName ) )
                    {
                        i_applicationExecutablePath = Path.Combine( this.AppFolderName, this.i_ExecutableName );
                    }
                    else
                    {
                        i_applicationExecutablePath = Path.Combine( Environment.CurrentDirectory, this.AppFolderName );
                        i_applicationExecutablePath = Path.GetFullPath( Path.Combine( i_applicationExecutablePath, this.i_ExecutableName ) );
                    }
                }
                return i_applicationExecutablePath;
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
        public PatientAccessConfiguration()
        {
        }
        #endregion

        #region Data Elements
 
        /// <summary>
        /// The name of the application.
        /// </summary>
        private string i_AppName = String.Empty;

        /// <summary>
        /// The the folder where the application was installed.
        /// </summary>
        private string i_AppFolderName = String.Empty;

        /// <summary>
        /// The name of the executable file for the application.
        /// </summary>
        private string i_ExecutableName = String.Empty;

        /// <summary>
        /// The id of the application.
        /// </summary>
        private string i_ApplicationID = String.Empty;

        /// <summary>
        /// The full path for the executable of the application.
        /// </summary>
        private string i_applicationExecutablePath = String.Empty;

        #endregion

        #region Constants
        #endregion
    }
}