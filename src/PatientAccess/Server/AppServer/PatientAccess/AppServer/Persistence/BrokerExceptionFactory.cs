using System;
using System.Data.SqlClient;
using System.Web.Services.Protocols;
using Extensions.Exceptions;
using Extensions.Persistence;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using log4net;
using PersistenceException = Extensions.Persistence.PersistenceException;
using RecordContentionException = Extensions.Persistence.RecordContentionException;
using WebException = System.Net.WebException;

namespace PatientAccess.Persistence
{
	/// <summary>
	/// Summary description for BrokerExceptionFactory.
	/// </summary>
	//TODO: Create XML summary comment for BrokerExceptionFactory
    [Serializable]
    public class BrokerExceptionFactory 
    {
        #region Event Handlers
        #endregion

        #region Methods

        // if it's "one of ours" pass it through and return it
        public static Exception BrokerExceptionFrom(
            Exception exception,
            ILog logger)
        {
            return BrokerExceptionFrom(exception.GetType().ToString(), exception, logger);
        }
        public static Exception BrokerExceptionFrom(
            Exception exception)
        {
            return BrokerExceptionFrom(exception.GetType().ToString(), exception);
        }
        public static Exception BrokerExceptionFrom(
            string msg, 
            Exception exception,
            ILog logger)
        {
            if( logger != null )
            {
                logger.Error(exception.GetType() + 
                    " " + msg,
                    exception);
            }

            if( exception.GetType().IsSubclassOf( typeof(EnterpriseException	) ) ||
                exception.GetType().Equals( typeof(EnterpriseException)))
            {
                // these exceptions are unique to the database implementations
                // so the exception could be of either Database. Since they are 
                // in the library that is not available to the client we must convert 
                // the exception to a broker exception
                if( 
                    exception.GetType().Equals( typeof( DatabaseServiceException)) ||
                    exception.GetType().Equals( typeof( Extensions.DB2Persistence.DatabaseServiceException))  ||
                    exception.GetType().Equals( typeof( PersistenceException))  ||
                    exception.GetType().Equals( typeof( Extensions.DB2Persistence.PersistenceException))  ||
                    exception.GetType().Equals( typeof( RecordContentionException))  ||
                    exception.GetType().Equals( typeof( Extensions.DB2Persistence.RecordContentionException))  
                    )
                {
                    return PABrokerExceptionFrom(msg,exception,logger);
                }
                else
                {
                        return exception;
                }   
            }
            else
            {
                return PABrokerExceptionFrom(msg,exception,logger);
            }
        }

        public static Exception BrokerExceptionFrom(string msg, Exception exception)
        {
            return BrokerExceptionFrom(msg,exception,null);
        }


        private static BrokerException PABrokerExceptionFrom(
            string msg, 
            Exception exception,
            ILog logger)
        {
            if( logger != null )
            {
                logger.Error(exception.GetType() +
                    " " + msg, 
                    exception);
            }
            BrokerException newException = null;

            if( exception.GetType().IsSubclassOf( typeof(iDB2Exception	) ) )
            {
                iDB2Exception db2ex = exception as iDB2Exception;
                if( exception.GetType() == typeof(iDB2ConnectionFailedException	) ||
                    exception.GetType() == typeof(iDB2CommErrorException) ||
                    exception.GetType() == typeof(iDB2ConnectionTimeoutException) ||
                    exception.GetType() == typeof(iDB2InvalidConnectionStringException) ||
                    exception.GetType() == typeof(iDB2UnsupportedHostVersionException)
                    )
                {
                    newException = new BrokerException(
                        db2ex.Message + " " + db2ex + " " +
                        db2ex.SqlState + " " + db2ex.MessageDetails + " " + 
                        (db2ex.InnerException == null ? " " : db2ex.InnerException.ToString()),
                        null,Severity.Catastrophic);
                }
                else
                {
                    newException = new BrokerException(
                        db2ex.Message + " " + db2ex + " " +
                        db2ex.SqlState + " " + db2ex.MessageDetails + " " + 
                        (db2ex.InnerException == null ? " " : db2ex.InnerException.ToString()),
                        null,Severity.High);
                }
            }
            else if( exception.GetType() == typeof(SqlException))
            {
                SqlException oex = exception as SqlException;
                switch( oex.Number )
                {
                        // these messages were derived from the Sql manual
                        // These are the ones most commonly associated with 
                        // connection problems.
                    case 1017: // invalid password
                    case 3113:
                    case 3121:
                    case 12154:
                    case 12170:
                    case 12500:
                    case 12514:
                    case 12520:
                    case 12521:
                    case 12525:
                    case 12533:
                    case 12540:
                    case 12541:
                    case 12549:
                    case 12560:
                        newException = new BrokerException(
                            oex.Message + " " + oex,
                            null,Severity.Catastrophic);
                        break;
                    default:
                        newException = new BrokerException(
                            oex.Message + " " + oex,
                            null,Severity.High);
                        break;
                }
            }
            else if( exception.GetType() == typeof( WebException ) )
            {
               
                if( exception.InnerException != null )
                {                   
                    WebException wex = exception as WebException;

                    if( wex.Response != null )
                    {
                        newException = new BrokerException(
                           msg + " " + exception.Message + " " + exception.InnerException.Message + " "
                           + " Status: " + wex.Status + " Response: " + wex.Response + " " + exception,
                           null, Severity.High );
                    }
                    else
                    {
                        newException = new BrokerException(
                            msg + " " + exception.Message + " " + exception.InnerException.Message + " "
                            + exception,
                            null, Severity.High );
                    }
                }
                else
                {
                    newException = new BrokerException(
                        msg + " " + exception.Message + " " + exception,
                        null, Severity.High );
                }
            }
            else if( exception.GetType() == typeof( SoapException ) ) 
            {
                if( exception.InnerException != null )
                {
                    SoapException soapEx = exception as SoapException;

                    if( soapEx.Code != null && soapEx.SubCode != null )
                    {
                        newException = new BrokerException(
                           msg + " " + exception.Message + " " + exception.InnerException.Message + " "
                           + " Status: " + soapEx.Code + " Response: " + soapEx.SubCode + " " + exception,
                           null, Severity.High );
                    }
                    else
                    {
                        newException = new BrokerException(
                            msg + " " + exception.Message + " " + exception.InnerException.Message + " "
                            + exception,
                            null, Severity.High );
                    }
                }
                else
                {
                    newException = new BrokerException(
                        msg + " " + exception.Message + " " + exception,
                        null, Severity.High );
                }
            }
            else
            {

                // some other type of exception has occured. Most likely a 
                // System.Exception. In any case it is unexpected. Wrap it and
                // pass it through with high Severity
                newException = new BrokerException(
                    msg + " " + exception.Message + " " + exception,
                    null, Severity.High );
            }
            return newException;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public BrokerExceptionFactory()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
