using System;
using System.IO;
using System.Reflection;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerInterfaces.CrashReporting;
using PatientAccess.Domain.Parties;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for CrashReportBrokerTests.
    /// </summary>

    [TestFixture()]
    public class CrashReportBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown CrashReportBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpCrashReportBrokerTests()
        {
            //dbConnection = new OracleConnection();
            //dbConnection.ConnectionString =
            //    ApplicationConfiguration.Settings[CONFIG_CXN_STRING];
            //dbConnection.Open();
        }

        [TestFixtureTearDown()]
        public static void TearDownCrashReportBrokerTests()
        {
            //dbConnection.Close();
        }
        #endregion

        #region Test Methods

        [Test()]
        [Description( "VeryLongExecution" )]
        public void A_SaveCrashReports()
        {
           
            ICrashReportBroker crb = BrokerFactory.BrokerOfType<ICrashReportBroker>();

            CrashReport report = new CrashReport();
            Assembly currentTestAssembly = Assembly.GetExecutingAssembly();

            Stream screenShotStream = currentTestAssembly.GetManifestResourceStream( "Tests.Resources.CoreDump.png" );

            // Load the screen capture bytes from resources
            byte[] screenCaptureData = 
                new byte[screenShotStream.Length];
            screenShotStream.Read( screenCaptureData, 0, (int)screenShotStream.Length );
            screenShotStream.Close();

            // Load the log report bytes from resources
            Stream logFileStream = currentTestAssembly.GetManifestResourceStream(
                    "Tests.Resources.DummyBreadCrumbLog.log");
            
            byte[] logFileData = new byte[logFileStream.Length];
            
            logFileStream.Read( logFileData, 0, (int)logFileStream.Length );
            logFileStream.Close();

            // Create Crash Report to save
            report.CrashReportID = 0;
            report.EmailAddress = "new@abc.com";
            report.Upn = "PACCESS";
            report.PhoneNumber = new PhoneNumber();
            report.PhoneNumber.AreaCode = "111";
            report.PhoneNumber.Number = "1122222";
            report.Comments = TEST_COMMENTS;
            report.UserLocalPermissions = "Admin";
            report.FacilityHSPCode = "SRE";
            report.TimeOnPC = new DateTime( 2006, 11, 29 );
            report.ExceptionDetail = "Unhandled Exception - InvalidCastException";
            report.WorkstationID = "PADEVPC";
            report.VersionOfPatientAccess = "1.0.0.1111";
            report.RAMTotalOnSystem = 256;
            report.RAMUsedByPatientAccess = 200;
            report.HardDriveUtilization = "200";
            report.FrameworkVersion = ".Net 2005";
            report.OsVersion = "Windows 2000";
            report.InstalledHotfixes = "Hotfix2";
            report.InternetExplorerVersion = "6.0.2900";
            report.BitsVersion = "2.0.0.5000";
            report.ExceptionType = "InvalidCastException";
            report.ClassName = "AccountsBroker";
            report.MethodName = "SelectAccountsFor()";
            report.ExceptionMessage = "Unhandled Exception - InvalidCastException";
            report.DefectID = 0;
            report.BreadCrumbLogArchiveName = "PALog.txt";
            report.BreadCrumbLog = logFileData;
            report.ScreenCapture = screenCaptureData;
            report.ClientIP = "127.0.0.1";
            report.ComputerName = "PSCTEST";

            crb.Save( report);

            Assert.IsTrue( true, SUCCESS );
        }

        [Test()]
        [Description( "VeryLongExecution" )]
        public void D_DeleteAllNUnitCrashReports()
        {
            ICrashReportBroker crb = BrokerFactory.BrokerOfType<ICrashReportBroker>();

            crb.Delete( TEST_COMMENTS );
        }


        #endregion

        #region Support Methods
//        private CrashReportingService GetCrashReportingService()
//        {
//            CrashReportingService crashReportingService = null;
//            string server, urlSetting;
//
//            crashReportingService = new CrashReportingService();
//            server = ConfigurationManager.AppSettings["PatientAccess.AppServer"];
//            urlSetting = ConfigurationManager.AppSettings["CrashReportingServiceUrl"];
//
//            if( server == null || server.Trim().Equals( string.Empty ) ||
//                urlSetting == null || urlSetting.Trim().Equals( string.Empty ) )
//            {
//                crashReportingService.Url = "http://localhost/PatientAccess.AppServer/CrashReporting/CrashReportingService.asmx";
//            }
//            else
//            {
//                if( server.EndsWith( @"/" ) )
//                {
//                    server = server.Substring( 0, server.Length - 1 );
//                }
//                crashReportingService.Url = string.Concat( server, urlSetting );
//            }
//
//            return crashReportingService;
//        }
        #endregion

        #region Data Elements
        //CrashReportBroker crashReportBroker = null;

        //private IDbConnection dbConnection;
        //private IDbTransaction dbTransaction;

        #endregion

        #region Constants
        private const string
            SUCCESS = "Message processed sucessfully",
            TEST_COMMENTS = "nUnit Comments";
        #endregion
    }
}