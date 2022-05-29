using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using Extensions.Persistence;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerInterfaces.CrashReporting;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for CrashReportBroker.
    /// </summary>

    [Serializable]
    public class CrashReportBroker : AbstractBroker, ICrashReportBroker
    {
		#region Constants 

        private const string
            DBPARAMETER_BITSVERSION = "@BitsVersion";
        private const string
            DBPARAMETER_BREADCRUMBLOGARCHIVENAME = "@LogArchiveName";
        private const string
            DBPARAMETER_BREADCRUMBLOGBODY = "@LogBody";
        private const string
            DBPARAMETER_CLASSNAME = "@ClassName";
        private const string
            DBPARAMETER_CLIENTIP = "@ClientIp";
        private const string
            DBPARAMETER_USER_COMMENTS = "@UserComments";
        private const string
            DBPARAMETER_COMMENT = "@Comment";
        private const string
            DBPARAMETER_COMPUTERNAME = "@ComputerName";
        private const string
            DBPARAMETER_CRASHREPORTID = "@CrashReportId";
        private const string
            DBPARAMETER_DATEADDED = "@DateAdded";
        private const string
            DBPARAMETER_EXCEPTION = "@ExceptionDetail";
        private const string
            DBPARAMETER_EXCEPTIONMESSAGE = "@ExceptionMessage";
        private const string
            DBPARAMETER_EXCEPTIONTYPE = "@ExceptionType";
        private const string
            DBPARAMETER_FRAMEWORKVERSION = "@DotNetFrameworkVersion";
        private const string
            DBPARAMETER_HDDUTILIZATION = "@HddUtilization";
        private const string
            DBPARAMETER_HOTFIXESINSTALLED = "@HotFixesInstalled";
        private const string
            DBPARAMETER_HSPCODE = "@FacilityHospitalCode";
        private const string
            DBPARAMETER_IEVERSION = "@InternetExplorerVersion";
        private const string
            DBPARAMETER_METHODNAME = "@MethodName";
        private const string
            DBPARAMETER_OSVERSION = "@OperatingSystemVersion";
        private const string
            DBPARAMETER_PATIENTACCESSVERSION = "@VersionOfPatientAccess";
        private const string
            DBPARAMETER_PBARWORKSTATIONID = "@PbarWorkstationId";
        private const string
            DBPARAMETER_RAMTOTALONSYSTEM = "@RamTotalOnSystem";
        private const string
            DBPARAMETER_RAMUSEDBYPA = "@RamUsedByPatientAccess";
        private const string 
            DBPARAMETER_SCREEN_CAPTURE = "@ScreenCapture";
        private const string
            DBPARAMETER_TIMEONPC = "@TimeOnPc";
        private const string
            DBPARAMETER_USEREMAIL = "@UserEmail";
        private const string
            DBPARAMETER_USERPERMISSIONS = "@UserLocalPermissions";
        private const string
            DBPARAMETER_USERPHONENUMBER = "@UserPhoneNumber";
        private const string
            DBPARAMETER_USERUPN = "@UserUpn";
        private const string
            DBPROCEDURE_DELETE_CRASH_REPORT_BY_ID = "CrashDump.DeleteCrashReportsById";
        private const string
            DBPROCEDURE_DELETE_CRASH_REPORTS_BY_COMMENT = "CrashDump.DeleteCrashReportsByComment";
        private const string
            DBPROCEDURE_SAVE_CRASH_REPORT = "CrashDump.SaveCrashReport";

		#endregion Constants 

		#region Fields 

        private static readonly ILog _logger =
            LogManager.GetLogger( typeof( CrashReportBroker ) );

		#endregion Fields 

		#region Constructors 

        public CrashReportBroker( string cxnString )
            : base( cxnString )
        {
        }


        public CrashReportBroker( SqlTransaction txn )
            : base( txn )
        {
        }


        public CrashReportBroker()
        {
        }

		#endregion Constructors 

		#region Properties 

        private static ILog Logger
        {
            get
            {
                return _logger;
            }
        }

		#endregion Properties 

		#region Methods 

        /// <summary>
        /// Delete - delete a specific crash report (by ID)
        /// </summary>
        /// <param name="crashReportId">The crash report id.</param>
        public void Delete( long crashReportId )
        {
            SqlCommand sqlCommand = this.CommandFor( DBPROCEDURE_DELETE_CRASH_REPORT_BY_ID );

            SqlParameter idParameter = sqlCommand.CreateParameter();
            idParameter.ParameterName = DBPARAMETER_CRASHREPORTID;
            idParameter.Value = crashReportId;
            sqlCommand.Parameters.Add( idParameter );
            
            try
            {
                sqlCommand.ExecuteNonQuery();                
            }
            catch( Exception anyException )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( anyException, Logger );
            }
            finally
            {
                this.Close( sqlCommand );
            }
        }


        /// <summary>
        /// Delete - delete a specific crash report (by the value of the Comments column)
        /// </summary>
        /// <param name="comments">The comments.</param>
        public void Delete( string comments )
        {
            SqlCommand sqlCommand = this.CommandFor( DBPROCEDURE_DELETE_CRASH_REPORTS_BY_COMMENT );

            SqlParameter sqlParameter = sqlCommand.CreateParameter();
            sqlParameter.ParameterName = DBPARAMETER_COMMENT;
            sqlParameter.Value = comments;
            sqlCommand.Parameters.Add( sqlParameter );

            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception anyException)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(anyException, Logger);
            }
            finally
            {
                this.Close(sqlCommand);
            }
        }


        /// <summary>
        /// Saves ( inserts/updates ) a Crash Report record.
        /// </summary>
        public void Save( CrashReport report )
        {
            SqlCommand cmd = null;

            if( report == null )
            {
                return;
            }

            try
            {

                cmd = this.CommandFor( DBPROCEDURE_SAVE_CRASH_REPORT );

                SqlParameter userEmailParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_USEREMAIL, SqlDbType.VarChar ) );

                userEmailParam.Value = this.MakeSafeParamter( report.EmailAddress, 100 );

                SqlParameter userUpnParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_USERUPN, SqlDbType.VarChar ) );
                userUpnParam.Value = this.MakeSafeParamter(report.Upn,50);

                SqlParameter userPhoneNumberParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_USERPHONENUMBER, SqlDbType.VarChar ) );
                userPhoneNumberParam.Value = report.PhoneNumber.AsUnformattedString().Trim();

                SqlParameter commentsParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_USER_COMMENTS, SqlDbType.VarChar ) );
                commentsParam.Value = this.MakeSafeParamter(report.Comments,2000);

                SqlParameter userPermissionsParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_USERPERMISSIONS, SqlDbType.VarChar ) );
                userPermissionsParam.Value = this.MakeSafeParamter(report.UserLocalPermissions,256);

                SqlParameter hspCodeParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_HSPCODE, SqlDbType.VarChar ) );
                hspCodeParam.Value = this.MakeSafeParamter(report.FacilityHSPCode,5);

                SqlParameter timeOnPcParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_TIMEONPC, SqlDbType.DateTime ) );
                timeOnPcParam.Value = report.TimeOnPC;
            
                SqlParameter pbarWorkstationIdParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_PBARWORKSTATIONID, SqlDbType.VarChar ) );
                pbarWorkstationIdParam.Value = this.MakeSafeParamter(report.WorkstationID,10);

                SqlParameter paVersionParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_PATIENTACCESSVERSION, SqlDbType.VarChar ) );
                paVersionParam.Value = this.MakeSafeParamter(report.VersionOfPatientAccess,50);

                SqlParameter ramTotalOnSystemParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_RAMTOTALONSYSTEM, SqlDbType.Int ) );
                ramTotalOnSystemParam.Value = report.RAMTotalOnSystem;

                SqlParameter ramUsedByPasParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_RAMUSEDBYPA, SqlDbType.Int ) );
                ramUsedByPasParam.Value = report.RAMUsedByPatientAccess;

                SqlParameter hddUtilizationParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_HDDUTILIZATION, SqlDbType.VarChar ) );
                hddUtilizationParam.Value = this.MakeSafeParamter(report.HardDriveUtilization,50);

                SqlParameter frameworkVersionParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_FRAMEWORKVERSION, SqlDbType.VarChar ) );
                frameworkVersionParam.Value = this.MakeSafeParamter(report.FrameworkVersion,50);
    
                SqlParameter osVersionParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_OSVERSION, SqlDbType.VarChar ) );
                osVersionParam.Value = this.MakeSafeParamter(report.OsVersion,256);

                SqlParameter hotFixesInstalledParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_HOTFIXESINSTALLED, SqlDbType.VarChar ) );
                hotFixesInstalledParam.Value = this.MakeSafeParamter( report.InstalledHotfixes, 2000 );

                SqlParameter ieVersionParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_IEVERSION, SqlDbType.VarChar ) );
                ieVersionParam.Value = this.MakeSafeParamter(report.InternetExplorerVersion,50 );

                SqlParameter bitsVersionParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_BITSVERSION, SqlDbType.VarChar ) );
                bitsVersionParam.Value = this.MakeSafeParamter(report.BitsVersion, 50 );

                SqlParameter exceptionTypeParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_EXCEPTIONTYPE, SqlDbType.VarChar ) );
                exceptionTypeParam.Value = this.MakeSafeParamter(report.ExceptionType,256);

                SqlParameter classNameParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_CLASSNAME, SqlDbType.VarChar ) );
                classNameParam.Value = this.MakeSafeParamter(report.ClassName,256 );

                SqlParameter methodNameParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_METHODNAME, SqlDbType.VarChar ) );
                methodNameParam.Value = this.MakeSafeParamter(report.MethodName, 256 );

                SqlParameter exceptionMessageParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_EXCEPTIONMESSAGE, SqlDbType.VarChar ) );
                exceptionMessageParam.Value = this.MakeSafeParamter(report.ExceptionMessage,2000);

                SqlParameter exceptionDetailParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_EXCEPTION, SqlDbType.VarChar ) );
                exceptionDetailParam.Value = report.ExceptionDetail.Trim();

                SqlParameter breadCrumbLogArchiveNameParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_BREADCRUMBLOGARCHIVENAME, SqlDbType.VarChar ) );
                breadCrumbLogArchiveNameParam.Value = report.BreadCrumbLogArchiveName;

                SqlParameter breadCrumbLogBodyParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_BREADCRUMBLOGBODY, SqlDbType.VarBinary ) );
                breadCrumbLogBodyParam.Value = report.BreadCrumbLog;

                SqlParameter screenCaptureParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_SCREEN_CAPTURE, SqlDbType.VarBinary ) );
                screenCaptureParam.Direction = ParameterDirection.Input;
                screenCaptureParam.Value = report.ScreenCapture;

                SqlParameter addedDate =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_DATEADDED, SqlDbType.DateTime ) );
                addedDate.Value = DateTime.Now;

                SqlParameter clientIpParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_CLIENTIP, SqlDbType.VarChar ) );
                clientIpParam.Value = report.ClientIP.Trim();

                SqlParameter computerNameParam =
                    cmd.Parameters.Add( new SqlParameter( DBPARAMETER_COMPUTERNAME, SqlDbType.VarChar ) );
                computerNameParam.Value = this.MakeSafeParamter(report.ComputerName,100);

                cmd.ExecuteNonQuery();

            }
            catch( Exception anyException )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( anyException, Logger );
            }
            finally
            {
                base.Close( cmd );
            }
        }


        /// <summary>
        /// Calls the Save method to ( inserts/updates ) each Crash Report record in the collection.
        /// </summary>
        public void Save( ArrayList crashReports )
        {
            foreach( CrashReport report in crashReports )
            {
                this.Save( report );
            }
        }

        /// <summary>
        /// Makes the paramter safe for insertion.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <returns></returns>
        private string MakeSafeParamter( string target, int maxLength )
        {
            if( String.IsNullOrEmpty( target ) || maxLength < 1 )
            {
                return String.Empty;
            }

            string trimmed = target.Trim();

            if( target.Length > maxLength )
            {
                return target.Substring( 0, maxLength );
            }

            return target;
        }

		#endregion Methods 
    }
}