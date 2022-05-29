using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web;

namespace PatientAccess.Http
{
    public class UnhandledExceptionModule : IHttpModule
    {

        static int _unhandledExceptionCount = 0;

        static string _sourceName = null;
        static object _initLock = new object();
        static bool _initialized = false;
        private static readonly log4net.ILog c_log = log4net.LogManager.GetLogger( typeof( UnhandledExceptionModule ) );


        /// <summary>
        /// Inits the specified app.
        /// </summary>
        /// <param name="app">The app.</param>
        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public void Init( HttpApplication app )
        {

            // Do this one time for each AppDomain.
            if( !_initialized )
            {
                lock( _initLock )
                {
                    if( !_initialized )
                    {

                        string webenginePath = Path.Combine( RuntimeEnvironment.GetRuntimeDirectory(), "webengine.dll" );

                        if( !File.Exists( webenginePath ) )
                        {
                            throw new Exception( String.Format( CultureInfo.InvariantCulture,
                                                                "Failed to locate webengine.dll at '{0}'.  This module requires .NET Framework 2.0.",
                                                                webenginePath ) );
                        }

                        _sourceName = string.Format(CultureInfo.InvariantCulture, "ASP.NET {0}.{1}.{2}.0", Environment.Version.Major, Environment.Version.Minor, Environment.Version.Build);

                        if( !EventLog.SourceExists( _sourceName ) )
                        {
                            throw new Exception( String.Format( CultureInfo.InvariantCulture,
                                                                "There is no EventLog source named '{0}'. This module requires .NET Framework 2.0.",
                                                                _sourceName ) );
                        }

                        AppDomain.CurrentDomain.UnhandledException += this.OnUnhandledException;

                        _initialized = true;
                    }
                }
            }
        }

        public void Dispose()
        {
        }

        void OnUnhandledException( object o, UnhandledExceptionEventArgs e )
        {
            // Let this occur one time for each AppDomain.
            if( Interlocked.Exchange( ref _unhandledExceptionCount, 1 ) != 0 )
                return;

            StringBuilder message = new StringBuilder( "\r\n\r\nUnhandledException logged by UnhandledExceptionModule:\r\n\r\nappId=" );

            string appId = (string)AppDomain.CurrentDomain.GetData( ".appId" );
            if( appId != null )
            {
                message.Append( appId );
            }


            Exception currentException;
            for( currentException = (Exception)e.ExceptionObject; currentException != null; currentException = currentException.InnerException )
            {
                message.AppendFormat( "\r\n\r\ntype={0}",
                                      currentException );
            }

            EventLog log = new EventLog();
            log.Source = _sourceName;
            log.WriteEntry( message.ToString(), EventLogEntryType.Error );

            c_log.Error( "UnhandledExceptionModule: " + message );
        }

    }
}