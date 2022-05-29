using System;
using System.IO;
using System.Xml.Serialization;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces.CrashReporting;
using PatientAccess.Domain.Parties;

namespace Tests.Integration.PatientAccess.BrokerInterfaces.CrashReporting
{
    /// <summary>
    /// Summary description for CrashReportTests.
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class CrashReportTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown CrashReportTests
        [TestFixtureSetUp()]
        public static void SetUpCrashReportTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownCrashReportTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestCrashReportType()
        {
            CrashReport aCrashReport = new CrashReport();
            
            Assert.IsNotNull( aCrashReport );
            Assert.AreEqual( aCrashReport.GetType(), typeof( CrashReport ) );
        }

        [Test()]
        public void TestSerializeCrashReportToXml()
        {
            CrashReport aCrashReport                = new CrashReport();
            aCrashReport.ApplicationName            = "ApplicationName";
            aCrashReport.AssemblyName               = "AssemblyName";
            aCrashReport.BitsVersion                = "BitsVersion";
            aCrashReport.BreadCrumbLog              = new Byte[]{ Byte.MaxValue, Byte.MinValue };
            aCrashReport.BreadCrumbLogArchiveName   = "BreadCrumbLogArchiveName";
            aCrashReport.ClassName                  = "ClassName";
            aCrashReport.Comments                   = "Comments";
            aCrashReport.CrashReportID              = long.MaxValue;
            aCrashReport.DefectID                   = int.MaxValue;
            aCrashReport.EmailAddress               = "EmailAddress";
            aCrashReport.ExceptionDetail            = "ExceptionDetail";
            aCrashReport.ExceptionMessage           = "ExceptionMessage";
            aCrashReport.ExceptionType              = "ExceptionType";
            aCrashReport.FacilityHSPCode            = "FacilityHSPCode";
            aCrashReport.FrameworkVersion           = "FrameworkVersion";
            aCrashReport.HardDriveUtilization       = "HardDriveUtilization";
            aCrashReport.InstalledHotfixes          = "InstalledHotfixes";
            aCrashReport.InternetExplorerVersion    = "InternetExplorerVersion";
            aCrashReport.MethodName                 = "MethodName";
            aCrashReport.Namespace                  = "Namespace";
            aCrashReport.OsVersion                  = "OsVersion";
            aCrashReport.PhoneNumber                = new PhoneNumber( "1", "972", "5777254" );
            aCrashReport.RAMTotalOnSystem           = int.MaxValue;
            aCrashReport.RAMUsedByPatientAccess     = int.MaxValue;
            aCrashReport.ScreenCapture              = new Byte[]{ Byte.MinValue, Byte.MinValue, Byte.MaxValue, Byte.MinValue, Byte.MinValue, Byte.MaxValue, Byte.MinValue };
            aCrashReport.TimeOnPC                   = new DateTime( 2000, 1, 1 );
            aCrashReport.Upn                        = "Upn";
            aCrashReport.UserLocalPermissions       = "UserLocalPermissions";
            aCrashReport.VersionOfPatientAccess     = "VersionOfPatientAccess";
            aCrashReport.WorkstationID              = "WorkstationID";
            aCrashReport.ClientIP = "127.0.0.1";
            aCrashReport.ComputerName = "PSCCOMP1";
            
            string fileName = String.Format( @"{0}\{1}.xml", Environment.CurrentDirectory, Guid.NewGuid() );
            XmlSerializer serializer = new XmlSerializer( typeof( CrashReport ) );

            // Write the object as XML to the file declared above.
            TextWriter writer = new StreamWriter( fileName );
            serializer.Serialize( writer, aCrashReport );
            writer.Close();

            // Read XML back into a new object and validate that the objects are equal
            TextReader reader = new StreamReader( fileName );
            CrashReport deserializedCrashReport = serializer.Deserialize( reader ) as CrashReport;
            reader.Close();
            
            // Delete the file from disk
            File.Delete( fileName );

            Assert.IsNotNull( deserializedCrashReport );
            Assert.AreEqual( aCrashReport.ApplicationName, deserializedCrashReport.ApplicationName );
            Assert.AreEqual( aCrashReport.AssemblyName, deserializedCrashReport.AssemblyName );
            Assert.AreEqual( aCrashReport.BitsVersion, deserializedCrashReport.BitsVersion );
            Assert.AreEqual( aCrashReport.BreadCrumbLog.ToString(), deserializedCrashReport.BreadCrumbLog.ToString() );
            Assert.AreEqual( aCrashReport.BreadCrumbLogArchiveName, deserializedCrashReport.BreadCrumbLogArchiveName );
            Assert.AreEqual( aCrashReport.ClassName, deserializedCrashReport.ClassName );
            Assert.AreEqual( aCrashReport.Comments, deserializedCrashReport.Comments );
            Assert.AreEqual( aCrashReport.CrashReportID, deserializedCrashReport.CrashReportID );
            Assert.AreEqual( aCrashReport.CrashReportName, deserializedCrashReport.CrashReportName );
            Assert.AreEqual( aCrashReport.DefectID, deserializedCrashReport.DefectID );
            Assert.AreEqual( aCrashReport.EmailAddress, deserializedCrashReport.EmailAddress );
            Assert.AreEqual( aCrashReport.ExceptionDetail, deserializedCrashReport.ExceptionDetail );
            Assert.AreEqual( aCrashReport.ExceptionMessage, deserializedCrashReport.ExceptionMessage );
            Assert.AreEqual( aCrashReport.ExceptionType, deserializedCrashReport.ExceptionType );
            Assert.AreEqual( aCrashReport.FacilityHSPCode, deserializedCrashReport.FacilityHSPCode );
            Assert.AreEqual( aCrashReport.FrameworkVersion, deserializedCrashReport.FrameworkVersion );
            Assert.AreEqual( aCrashReport.HardDriveUtilization, deserializedCrashReport.HardDriveUtilization );
            Assert.AreEqual( aCrashReport.InstalledHotfixes, deserializedCrashReport.InstalledHotfixes );
            Assert.AreEqual( aCrashReport.InternetExplorerVersion, deserializedCrashReport.InternetExplorerVersion );
            Assert.AreEqual( aCrashReport.MethodName, deserializedCrashReport.MethodName );
            Assert.AreEqual( aCrashReport.Namespace, deserializedCrashReport.Namespace );
            Assert.AreEqual( aCrashReport.OsVersion, deserializedCrashReport.OsVersion );
            Assert.AreEqual( aCrashReport.PhoneNumber, deserializedCrashReport.PhoneNumber );
            Assert.AreEqual( aCrashReport.RAMTotalOnSystem, deserializedCrashReport.RAMTotalOnSystem );
            Assert.AreEqual( aCrashReport.RAMUsedByPatientAccess, deserializedCrashReport.RAMUsedByPatientAccess );
            Assert.AreEqual( aCrashReport.ScreenCapture.ToString(), deserializedCrashReport.ScreenCapture.ToString() );
            Assert.AreEqual( aCrashReport.TimeOnPC, deserializedCrashReport.TimeOnPC );
            Assert.AreEqual( aCrashReport.Upn, deserializedCrashReport.Upn );
            Assert.AreEqual( aCrashReport.UserLocalPermissions, deserializedCrashReport.UserLocalPermissions );
            Assert.AreEqual( aCrashReport.VersionOfPatientAccess, deserializedCrashReport.VersionOfPatientAccess );
            Assert.AreEqual( aCrashReport.WorkstationID, deserializedCrashReport.WorkstationID );
            Assert.AreEqual( aCrashReport.ClientIP, deserializedCrashReport.ClientIP );
            Assert.AreEqual( aCrashReport.ComputerName, deserializedCrashReport.ComputerName );
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}