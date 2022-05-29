using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using PatientAccess.BrokerInterfaces.CrashReporting;
using log4net;

namespace PatientAccess.UI.CrashReporting {
	  /// <summary> 
    /// Summary description for CrashReportStore 
    /// </summary>
    [Serializable]
    public class CrashReportStore
    {
        #region Event Handlers
        #endregion

        #region Methods
        /// <summary>
        /// Gets the saved report(s) from the client workstaion.
        /// </summary>
        /// <returns>A collection of Crash Reports.</returns>
        public IList GetSavedReports()
        {
            ArrayList savedReports = null;
            XmlSerializer serializer;
            IsolatedStorageFile store;

            try
            {
                savedReports = new ArrayList();
                serializer = new XmlSerializer( typeof( CrashReport ));
                store = IsolatedStorageFile.GetUserStoreForDomain();

                String [] fileNames = store.GetFileNames( CrashReportFilePattern );    

                foreach( string file in fileNames )
                {
                    if( file.ToLower().EndsWith( CrashReportFileExtension ) )
                    {
                        try
                        {
                            using( IsolatedStorageFileStream stream = new IsolatedStorageFileStream( file, FileMode.Open, store ) ) 
                            {
                                using( StreamReader streamReader = new StreamReader( stream ) ) 
                                {
                                    CrashReport savedReport = serializer.Deserialize( streamReader ) as CrashReport;
                                    savedReports.Add( savedReport );
                                }
                            }
                        }
                        catch( InvalidOperationException ioex )
                        {
                            store.DeleteFile( file );
                            c_log.Error( "GetSavedReports failed to deserialize a crash report from isolated storage.  The system has deleted the offending crash report from the user's workstation.", ioex );
                        }
                    }
                }
                store.Dispose();
                store.Close();
            }
            catch( Exception ex )
            {
                c_log.Error( "GetSavedReports failed to generate a collection of crash reports from isolated storage.", ex  );
            }

            return savedReports;
        }

        /// <summary>
        /// Deletes all stored Crash Reports from the client workstation.
        /// </summary>
        public void Delete()
        {
            XmlSerializer serializer = new XmlSerializer( typeof( CrashReport ));

            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForDomain();

            String [] fileNames = store.GetFileNames( CrashReportFilePattern );

            foreach( string file in fileNames )
            {
                if( file.ToLower().EndsWith( CrashReportFileExtension ) )
                {
                    store.DeleteFile( file );                 
                }
            }
            store.Dispose();
            store.Close();
        }

        /// <summary>
        /// Saves the specified report to the client workstation.
        /// </summary>
        /// <param name="report">A Crash Report.</param>
        public void Save( CrashReport report )
        {
            XmlSerializer serializer = new XmlSerializer( typeof( CrashReport ));
        
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForDomain();

            string storageFileName = String.Format( "{0}{1}", 
                                                    Guid.NewGuid(), 
                                                    CrashReportFileExtension );
                
            IsolatedStorageFileStream stream = new IsolatedStorageFileStream( storageFileName, FileMode.Create, store );

            StreamWriter streamWriter = new StreamWriter( stream );
            serializer.Serialize( streamWriter, report );

            streamWriter.Close();
            stream.Close();
            store.Dispose();
            store.Close();
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods 
        #endregion

        #region Private Properties
        private string CrashReportFileExtension
        {
            get
            {
                if( i_CrashReportFileExtension == string.Empty )
                {
                    i_CrashReportFileExtension = ConfigurationManager.AppSettings[CRASH_REPORT_FILE_EXTENSION].ToLower();
                }
                return i_CrashReportFileExtension;
            }
        }

        private string CrashReportFilePattern
        {
            get
            {
                if( i_CrashReportFilePattern == string.Empty )
                {
                    i_CrashReportFilePattern = string.Concat( ASTERISK, CrashReportFileExtension );
                }
                return i_CrashReportFilePattern;
            }
        }
        #endregion

        #region Construction and Finalization
        public CrashReportStore()
        {            
        }

        public virtual void Dispose()
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log = LogManager.GetLogger( typeof( CrashReportStore ) );
        private string i_CrashReportFileExtension = string.Empty;
        private string i_CrashReportFilePattern   = string.Empty;
        #endregion

        #region Constants
        private const string ASTERISK = "*", 
                             CRASH_REPORT_FILE_EXTENSION = "CrashReportFileExtension";
        #endregion
    }
}