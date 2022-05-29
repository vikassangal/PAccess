using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Management;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Web.Services.Protocols;
using Extensions.Exceptions;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Win32;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerInterfaces.CrashReporting;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;

namespace PatientAccess.UI.CrashReporting
{
    /// <summary> 
    /// Summary description for CrashReporter 
    /// </summary>
    [Serializable]
    public class CrashReporter
    {
        #region Event Handlers
        #endregion

        #region Methods
        /// <summary>
        /// Builds the Crash Report for the ApplicationExceptionHandler.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="screenCapture">The screen capture.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="comments">The comments.</param>
        /// <returns></returns>
        public CrashReport BuildReportFor( Exception exception, Bitmap screenCapture, PhoneNumber phoneNumber, string comments )
        {
            // Create an instance of the Crash Report.
            CrashReport report = new CrashReport();

            // Add all Exception Details to the crash report and set the fatal exception flag
            this.AddClassNameTo( exception, report );
            this.AddMethodNameTo( exception, report );
            report.ExceptionDetail = exception.ToString();
            report.ExceptionType = exception.GetType().Name;
            report.ExceptionMessage = exception.Message;
            if( exception.GetType() == typeof( EnterpriseException ) )
            {
                EnterpriseException ex = (EnterpriseException)exception;
                if( ex.Severity == Severity.Catastrophic )
                {
                    report.ExceptionIsFatal = true;
                }
            }

            // Add Screen Capture to the Crash Report
            using( MemoryStream memoryStream = new MemoryStream() )
            {
                screenCapture.Save( memoryStream, ImageFormat.Jpeg );
                memoryStream.Position = 0;
                byte[] screenCaptureData = new byte[memoryStream.Length];
                memoryStream.Read( screenCaptureData, 0, Convert.ToInt32( memoryStream.Length ) );
                report.ScreenCapture = screenCaptureData;
            }

            // Add Phone Number to the Crash Report
            report.PhoneNumber = phoneNumber;

            // Add Comments to the Crash Report
            report.Comments = comments;

            // Add Bread Crumb Log(s) to the Crash Report
            this.AddBreadcrumbLogTo( report );

            // Add Client Information to the Crash Report
            this.AddPatientAccessClientInformationTo( report );

            // Add User Information to the Crash Report
            this.AddUserInformationTo( report );

            // Add Workstation Information to the Crash Report
            this.AddWorkstationInformationTo( report );

            return report;
        }

        /// <summary>
        /// Saves the specified Crash Report.
        /// </summary>
        /// <param name="report">The Crash Report.</param>
        public void Save( CrashReport report )
        {
            if( !report.ExceptionIsFatal )
            {
                //PatientAccess.Services.CrashReportingService.CrashReportingService crashReportingService = null;
                ICrashReportBroker crashReportBroker = BrokerFactory.BrokerOfType<ICrashReportBroker>();
                try
                {
                    //crashReportingService = this.GetCrashReportingService();

                    if( crashReportBroker != null )
                    {
                        crashReportBroker.Save(report);
                        //crashReportingService.Save( PatientAccess.Services.CrashReportConverter.ToServiceReport(report)); //report.ToServiceReport() );
                    }
                }
                catch( Exception ex )
                {
                    string msg = "Failed to save non-fatal Crash Report to CrashReportingService. Will attempt to save to local storage.";
                    if( report != null )
                    {
                        msg = report.FacilityHSPCode + " - " + msg;
                    }
                    HandleException( msg, ex );
                    SaveToClientMachine( report );
                }
            }
            else
            {
                SaveToClientMachine( report );
            }
        }

        /// <summary>
        /// Sends each of the cached (from isolated storage) Crash Reports to the web service.
        /// </summary>
        public void SendCachedReports()
        {
            //PatientAccess.Services.CrashReportingService.CrashReportingService crashReportingService = null;

            try
            {
                CrashReportStore store = new CrashReportStore();

                ArrayList storedReports = store.GetSavedReports() as ArrayList;

                if( storedReports != null && storedReports.Count > 0 )
                {
                    //rashReportingService = this.GetCrashReportingService();
                    ICrashReportBroker crashReportBroker = BrokerFactory.BrokerOfType<ICrashReportBroker>();

                    if( crashReportBroker != null )
                    {
                        // Iterate through the list of Crash Reports and save each separately.
                        foreach( CrashReport report in storedReports )
                        {
                            try
                            {
                                //crashReportingService.Save(PatientAccess.Services.CrashReportConverter.ToServiceReport(report) ); //report.ToServiceReport() );
                                crashReportBroker.Save(report);
                            }
                            catch( SoapException se )
                            {
                                if( se.Message.ToLower().IndexOf( MAX_REQUEST_LENGTH_EXCEEDED.ToLower() ) != -1 )
                                {
                                    string msg = string.Format( "{0} - SendCachedReports generated a SoapException while attempting to save a crash report through the Crash Reporting web service.  The Crash Report file exceeded the maximum request length (httpRuntime maxRequestLength) configured in the Web.Config. The system will attempt to send the remaining Crash Reports and the offending Crash Report will be deleted from the user's workstation.", 
                                        report.FacilityHSPCode );
                                    HandleException( msg, se );
                                }
                            }
                        }

                        // Delete the Crash Reports from local storage
                        store.Delete();
                    }
                }
            }
            catch( Exception ex )
            {
                // Fix Defect : 8814 
                // Make sure we ignore the exception here to allow application to continue,
                // otherwise the application will thow another error.
                //string msg = string.Format( "{0} - SendCachedReports failed to send Crash Reports stored on the client workstation to the CrashReportingService web service.", 
                //    User.GetCurrent().Facility.Code );
                string msg = "SendCachedReports failed to send Crash Reports stored on the client workstation to the CrashReportingService web service.";
                HandleException( msg, ex );
            }
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        /// <summary>
        /// Adds the class name to the Crash Report.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="report">The crash report.</param>
        private void AddClassNameTo( Exception exception, CrashReport report )
        {

            try
            {
                report.ClassName = exception.TargetSite.DeclaringType.Name;
            }
            catch( Exception )
            {
                //If the error is from the server side assembly then the following error was raised.
                report.ClassName = exception.Source;
                //SR44423 - DC.  Stop logging this as an error since we understand it is probably coming back from the application server.
                //HandleException( "AddClassNameTo failed to add the Class Name to the Crash Report." + Environment.NewLine + ex.ToString(), ex );
            }
        }

        /// <summary>
        /// Adds the method name to the Crash Report.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="report">The crash report.</param>
        private void AddMethodNameTo( Exception exception, CrashReport report )
        {
            try
            {
                report.MethodName = exception.TargetSite.Name;
            }
            catch( Exception )
            {
                //If the error is from the server side assembly then the following error was raised.
                report.MethodName = exception.Source;
                //SR44423 - DC.  Stop logging this as an error since we understand it is probably coming back from the application server.
                //HandleException( "AddMethodNameTo failed to add the Method Name to the Crash Report." + Environment.NewLine + ex.Message, ex );
            }
        }

        /// <summary>
        /// Saves the Crash Report to the client machine.
        /// </summary>
        /// <param name="report">The Crash Report.</param>
        private void SaveToClientMachine( CrashReport report )
        {
            try
            {
                CrashReportStore reportLocalStore = new CrashReportStore();
                reportLocalStore.Save( report );
            }
            catch( Exception ex )
            {
                string msg = "SaveToClientMachine failed to save the Crash Report to the client workstation.";
                if( report != null )
                {
                    msg = report.FacilityHSPCode + " - " + msg;
                }
                HandleException( msg, ex );
            }
        }

        /// <summary>
        /// Adds the patient access client informationto.
        /// </summary>
        /// <param name="report">The crash report.</param>
        private void AddPatientAccessClientInformationTo( CrashReport report )
        {
            report.VersionOfPatientAccess = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            report.WorkstationID = User.GetCurrent().WorkstationID;
        }

        /// <summary>
        /// Adds the user information to.
        /// </summary>
        /// <param name="report">The report.</param>
        private void AddUserInformationTo( CrashReport report )
        {
            try
            {
                report.EmailAddress = User.GetCurrent().EmailAddress.ToString();
                report.Upn = User.GetCurrent().SecurityUser.UPN;
                report.FacilityHSPCode = User.GetCurrent().Facility.Code;

                StringBuilder groups = new StringBuilder();

                foreach( IdentityReference ir in WindowsIdentity.GetCurrent().Groups )
                {
                    groups.AppendFormat( "{0};", ir.Translate( typeof( NTAccount ) ).Value );
                }

                report.UserLocalPermissions = groups.ToString();

            }
            catch( Exception ex )
            {
                string msg = "AddUserInformationTo failed to retrieve the User's Information.";
                if( report != null )
                {
                    msg = report.FacilityHSPCode + " - " + msg;
                }
                HandleException( msg, ex );
            }
        }

        /// <summary>
        /// Adds the workstation information to.
        /// </summary>
        /// <param name="report">The report.</param>
        private void AddWorkstationInformationTo( CrashReport report )
        {
            try
            {
                report.TimeOnPC = DateTime.Now;
                report.OsVersion = Environment.OSVersion.ToString();
                report.FrameworkVersion = Environment.Version.ToString();
                report.HardDriveUtilization = Convert.ToString( getHardDriveUtilization() );
                report.RAMUsedByPatientAccess = this.GetRAMUsedByProcess( Process.GetCurrentProcess() );
                report.RAMTotalOnSystem = this.GetPhysicalMemory();
                report.InternetExplorerVersion = this.GetIEVersion();
                report.ComputerName = Dns.GetHostName();
                report.ClientIP = this.GetHostIpAddresses();
                report.InstalledHotfixes = this.getHotFixesInstalled();
                report.BitsVersion = this.getBitsVersion();
            }
            catch( Exception ex )
            {
                string msg = "AddWorkstationInformationTo failed to retrieve Workstation Information.";
                if( report != null )
                {
                    msg = report.FacilityHSPCode + " - " + msg;
                }
                HandleException( msg, ex );
            }
        }

        private string getBitsVersion()
        {
            string ret = String.Empty;

            try
            {
                StringBuilder path = new StringBuilder();
                path.AppendFormat( "{0}/{1}", Environment.SystemDirectory, "qmgr.dll" );
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo( path.ToString() );

                ret = fvi.FileVersion;
            }
            catch( Exception ex )
            {
                string msg = "Gathering BITS version failed.";
                HandleException( msg, ex );
            }

            return ret;
        }

        private string getHotFixesInstalled()
        {
            StringBuilder hotfixes = new StringBuilder();

            try
            {
                StringBuilder mgtPath = new StringBuilder();
                mgtPath.AppendFormat( @"\\{0}\root\cimv2:{1}", Environment.MachineName, "Win32_QuickFixEngineering" );
                ManagementClass managementClass = new ManagementClass( mgtPath.ToString() );

                ManagementObjectCollection objs = managementClass.GetInstances();

                foreach( ManagementObject obj in objs )
                {
                    hotfixes.AppendFormat
                    (
                        "{0},{1};",

                        ( obj.Properties["HotFixID"].Value != null ) ?
                            obj.Properties["HotFixID"].Value.ToString() :
                            String.Empty,

                        ( obj.Properties["ServicePackInEffect"].Value != null ) ?
                            obj.Properties["ServicePackInEffect"].Value.ToString() :
                            String.Empty
                    );
                }
            }
            catch( Exception ex )
            {
                string msg = "Gathering hotfixes failed.";
                HandleException( msg, ex );
            }

            return hotfixes.ToString();
        }

        /// <summary>
        /// Get the version of Internet Explorer from the system registry
        /// </summary>
        /// <returns></returns>
        private string GetIEVersion()
        {
            RegistryKey mainKey, subKey;
            string ieVersion = string.Empty;

            try
            {
                mainKey = RegistryKey.OpenRemoteBaseKey( RegistryHive.LocalMachine, "" );
                subKey = mainKey.OpenSubKey( IE_REGISTRY_KEY );
                ieVersion = subKey.GetValue( IE_VERSION_KEY ).ToString();
            }
            catch( Exception ex )
            {
                string msg = "GetIEVersion failed to retrieve system IE Version.";
                HandleException( msg, ex );
            }

            return ieVersion;
        }

        /// <summary>
        /// Returns the Internet Protocol (IP) addresses for the specified host.
        /// </summary>
        /// <returns></returns>
        private string GetHostIpAddresses()
        {
            StringBuilder hostIpAddresses = new StringBuilder();

            string hostName = string.Empty;

            try
            {
                hostName = Dns.GetHostName();

                if( hostName != string.Empty )
                {
                    IPHostEntry ipEntry = Dns.GetHostEntry( hostName );
                    IPAddress[] ipAddressList = ipEntry.AddressList;

                    for( int i = 0; i < ipAddressList.Length; i++ )
                    {
                        hostIpAddresses.Append( ipAddressList[i] );
                        hostIpAddresses.Append( ", " );
                    }
                    if( !hostIpAddresses.ToString().Equals( string.Empty ) )
                    {
                        hostIpAddresses.Remove( hostIpAddresses.Length - 2, 2 );
                    }
                }
            }
            catch( Exception ex )
            {
                string msg = "GetHostIpAddresses failed to retrieve a list of ip addresses for the host.";
                HandleException( msg, ex );
            }

            return hostIpAddresses.ToString();
        }

        // Get the amount of random access memory used by the process in megabytes (MB)
        private int GetRAMUsedByProcess( Process process )
        {
            long i = 0;

            try
            {
                i = process.PrivateMemorySize64 / 1024 / 1024;
            }
            catch( Exception ex )
            {
                string msg = "GetRAMUsedByProcess failed to retrieve memory used by the process.";
                HandleException( msg, ex );
            }

            return Convert.ToInt32( i );
        }

        // Get the hard drive free space in megabytes (MB)
        private int getHardDriveUtilization()
        {
            long i = 0;
            ManagementObjectSearcher query;
            ManagementObjectCollection queryCollection;
            string systemDrive;
            string pathRoot;

            try
            {
                systemDrive = Environment.SystemDirectory;
                pathRoot = Path.GetPathRoot( systemDrive );
                //  check if has terminal slash, remove if found
                if( pathRoot.EndsWith( @"\" ) )
                {
                    pathRoot = @pathRoot.Substring( 0, pathRoot.Length - 1 );
                }
                query = new ManagementObjectSearcher( "SELECT * From Win32_LogicalDisk Where DeviceID = '" + pathRoot + "'" );
                queryCollection = query.Get();

                foreach( ManagementObject mo in queryCollection )
                {
                    i = Convert.ToInt64( mo["FreeSpace"].ToString() );
                }
                i = i / 1024 / 1024;
            }
            catch( Exception ex )
            {
                string msg = "getHardDriveUtilization failed to retrieve system FreeSpace.";
                HandleException( msg, ex );
            }

            return Convert.ToInt32( i );
        }

        // Get the amount of random access memory (RAM) on the system in megabytes (MB)
        private int GetPhysicalMemory()
        {
            long i = 0;
            ManagementObjectSearcher query;
            ManagementObjectCollection queryCollection;

            try
            {
                query = new ManagementObjectSearcher( "SELECT * From Win32_ComputerSystem" );
                queryCollection = query.Get();

                foreach( ManagementObject mo in queryCollection )
                {
                    i = Convert.ToInt64( mo["TotalPhysicalMemory"].ToString() );
                }
                i = i / 1024 / 1024;
            }
            catch( Exception ex )
            {
                string msg = "GetPhysicalMemory failed to retrieve system RAM.";
                HandleException( msg, ex );
            }

            return Convert.ToInt32( i );
        }

        /// <summary>
        /// Adds the breadcrumb log to the Crash Report.
        /// </summary>
        /// <param name="report">The Crash Report.</param>
        private void AddBreadcrumbLogTo( CrashReport report )
        {
            try
            {
                report.BreadCrumbLogArchiveName = Path.ChangeExtension( BreadCrumbLogFileName, ".zip" );
                report.BreadCrumbLog = this.GetCompressedBreadCrumbLog();
            }
            catch( Exception ex )
            {
                string msg = "AddBreadcrumbLogTo failed to add the Bread Crumb Log to the Crash Report.";
                if( report != null )
                {
                    msg = report.FacilityHSPCode + " - " + msg;
                }
                HandleException( msg, ex );
            }
        }

        /// <summary>
        /// Creates a zip file for all available breadcrumb logs.
        /// </summary>
        /// <returns></returns>
        private byte[] GetCompressedBreadCrumbLog()
        {
            string[] filenames = Directory.GetFiles( BreadCrumbLogPath, BreadCrumbLogPattern );

            MemoryStream stream = new MemoryStream();
            byte[] retBuffer = new byte[1];

            using( ZipOutputStream s = new ZipOutputStream( stream ) )
            {

                s.SetLevel( 9 ); // 0 - store only to 9 - means best compression

                byte[] buffer = new byte[MAX_LENGTH];

                foreach( string file in filenames )
                {

                    // Using GetFileName makes the result compatible with XP
                    // as the resulting path is not absolute.
                    ZipEntry entry = new ZipEntry( Path.GetFileName( file ) );

                    entry.DateTime = DateTime.Now;
                    s.PutNextEntry( entry );

                    // Create a temporary bread crumb file copy to add to the zip.
                    File.Copy( file, COPY_OF_BREAD_CRUMB_FILE, true );

                    using( FileStream fs = new FileStream(
                                COPY_OF_BREAD_CRUMB_FILE,
                                FileMode.Open,
                                FileAccess.Read,
                                FileShare.Read, 1 ) )
                    {
                        // Using a fixed size buffer here makes no noticeable difference for output
                        // but keeps a lid on memory usage.
                        int sourceBytes;
                        do
                        {
                            sourceBytes = fs.Read( buffer, 0, buffer.Length );
                            s.Write( buffer, 0, sourceBytes );
                        } while( sourceBytes > 0 );
                    }

                    // Delete the temporary bread crumb file copy.
                    File.Delete( COPY_OF_BREAD_CRUMB_FILE );
                }

                // Finish is important here to ensure trailing information for a 
                // Zip file is appended. Without this the created file would be invalid.
                s.Finish();

                retBuffer = new byte[stream.Length];
                retBuffer = this.GetByteArray( stream );

                // Close is important to wrap things up and unlock the file.
                s.Close();
            }

            return retBuffer;
        }

        /// <summary>
        /// Reads data from a stream until the end is reached. The
        /// data is returned as a byte array. An IOException is
        /// thrown if any of the underlying IO calls fail.
        /// </summary>
        /// <param name="stream">The memory stream.</param>
        /// <returns></returns>
        private byte[] GetByteArray( MemoryStream stream )
        {
            stream.Seek( 0, 0 );
            byte[] buffer = new byte[stream.Length];
            int read = 0;

            int chunk;
            while( ( chunk = stream.Read( buffer, read, buffer.Length - read ) ) > 0 )
            {
                read += chunk;

                // If we've reached the end of our buffer, check to see if there's
                // any more information
                if( read == buffer.Length )
                {
                    int nextByte = stream.ReadByte();

                    // End of stream? If so, we're done
                    if( nextByte == -1 )
                    {
                        return buffer;
                    }

                    // Resize the buffer, put in the byte we've just
                    // read, and continue
                    byte[] newBuffer = new byte[buffer.Length * 2];
                    Array.Copy( buffer, newBuffer, buffer.Length );
                    newBuffer[read] = (byte)nextByte;
                    buffer = newBuffer;
                    read++;
                }
            }
            // Buffer is now too big. Shrink it.
            byte[] ret = new byte[read];
            Array.Copy( buffer, ret, read );
            return ret;
        }

        /// <summary>
        /// Handles exceptions in a way that will not cause another exception 
        /// to be thrown if logging fails.
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="e">The exception</param>
        private void HandleException( string msg, Exception e )
        {
            try
            {
                // Attempt to format and log the error with Log4Net, Error Level : 'Error'
                c_log.ErrorFormat( "Msg: {0}\nException Message: {1}\nStackTrace: {2}\nInnerException: {3}\nException: ",
                    msg,
                    e.Message,
                    e.StackTrace, 
                    e.InnerException,
                    e );
            }
            finally
            {
                // Swallow the exception and continue executing ... Otherwise we will end up in a loop.
            }
        }
       
        #endregion

        #region Private Properties
        private string BreadCrumbLogFullPath
        {
            get
            {
                if( i_BreadCrumbLogFullPath == string.Empty )
                {
                    try
                    {
                        Hierarchy hierarchy = LogManager.GetRepository() as Hierarchy;
                        Logger logger = hierarchy.GetLogger( BREAD_CRUMB_LOGGER_NAME ) as Logger;
                        RollingFileAppender appender = logger.GetAppender( BREAD_CRUMB_APPENDER_NAME ) as RollingFileAppender;

                        if( Path.IsPathRooted( appender.File ) )
                        {
                            i_BreadCrumbLogFullPath = appender.File;
                        }
                        else
                        {
                            i_BreadCrumbLogFullPath = Path.GetFullPath( Path.Combine( Environment.CurrentDirectory, appender.File ) );
                        }
                    }
                    catch( Exception ex )
                    {
                        i_BreadCrumbLogFullPath = BREAD_CRUMB_LOG_FILE_DEFAULT;
                        i_BreadCrumbLogFullPath = Path.GetFullPath( Path.Combine( Environment.CurrentDirectory, i_BreadCrumbLogFullPath ) );
                        string msg = "Failed to retrieve the BreadCrumbLogFullPath. Will attempt to use the default value - {1}";
                        HandleException( msg, ex );
                    }
                }

                return i_BreadCrumbLogFullPath;
            }
        }

        private string BreadCrumbLogPath
        {
            get
            {
                if( i_BreadCrumbLogPath == string.Empty )
                {
                    i_BreadCrumbLogPath = Path.GetDirectoryName( BreadCrumbLogFullPath );
                }
                return i_BreadCrumbLogPath;
            }
        }

        private string BreadCrumbLogFileName
        {
            get
            {
                if( i_BreadCrumbLogFileName == string.Empty )
                {
                    i_BreadCrumbLogFileName = Path.GetFileName( BreadCrumbLogFullPath );
                }
                return i_BreadCrumbLogFileName;
            }
        }

        private string BreadCrumbLogPattern
        {
            get
            {
                if( i_BreadCrumbLogPattern == string.Empty )
                {
                    i_BreadCrumbLogPattern = string.Concat( BreadCrumbLogFileName, ASTERISK );
                }
                return i_BreadCrumbLogPattern;
            }
        }
        #endregion

        #region Construction and Finalization
        public CrashReporter()
        {
        }

        public virtual void Dispose()
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log = LogManager.GetLogger( typeof( CrashReporter ) );
        private string i_BreadCrumbLogFullPath = string.Empty;
        private string i_BreadCrumbLogPath = string.Empty;
        private string i_BreadCrumbLogPattern = string.Empty;
        private string i_BreadCrumbLogFileName = string.Empty;
        #endregion

        #region Constants
        private const string ASTERISK = "*",
                             BREAD_CRUMB_LOG_FILE_DEFAULT = "BreadCrumbLog.txt",
                             BREAD_CRUMB_LOGGER_NAME = "PatientAccess.UI.Logging.BreadCrumbLogger",
                             BREAD_CRUMB_APPENDER_NAME = "BreadCrumbAppender",
                             COPY_OF_BREAD_CRUMB_FILE = "BC.txt",
                             IE_REGISTRY_KEY = "Software\\Microsoft\\Internet Explorer",
                             IE_VERSION_KEY = "Version",
                             MAX_REQUEST_LENGTH_EXCEEDED = "Maximum request length exceeded";
        private const int MAX_LENGTH = 5000000;
        #endregion
    }
}
