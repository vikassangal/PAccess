using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using log4net;
using Xstream.Core;
using System.Reflection;
using System.Configuration;
using System.Threading;

namespace Hsd.PerotSystems.PatientAccess.Services.ViWeb
{
    
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [SoapDocumentService( RoutingStyle = SoapServiceRoutingStyle.RequestElement )]
    public class PreviousVisitService : System.Web.Services.WebService, IPreviousVisitServiceSoapBinding
    {

        private static ILog Logger = LogManager.GetLogger( typeof( PreviousVisitService ) );
        
        /// <summary>
        /// </summary>
        /// <param name="pvdr"></param>
        /// <returns></returns>
        /// <remarks/>
        public PreviousVisitDocumentResponse getPreviousVisitDocuments( PreviousVisitDocumentRequest documentRequest )
        {

            PreviousVisitDocumentResponse stubResponse = 
                new PreviousVisitDocumentResponse();


            #region Log Method Entrance
            if( Logger.IsDebugEnabled )
            {
                Logger.DebugFormat( "{0}() - {1}",
                MethodInfo.GetCurrentMethod().Name,
                "Entered" );
            }//if
            #endregion
        

            if( Logger.IsDebugEnabled )
            {
                
                Logger.Debug( "Proxying request for " + this.ToXmlString( documentRequest ) );

            }//if

            try
            {

                stubResponse.docType = documentRequest.docType;
                stubResponse.documentsWereFound = false;
                stubResponse.errorMessage = String.Empty;
                stubResponse.hspCode = documentRequest.hspCode;
                stubResponse.medicalRecordNumber = documentRequest.medicalRecordNumber;
                stubResponse.previousVisitDocuments = new PreviousVisitDocument[] { };
                stubResponse.recsFound = new PreviousVisitDocument[] { };

                int delay = 0;
                int.TryParse( ConfigurationManager.AppSettings["processingDelay"], out delay );

                if( delay > 0 )
                {
                    Thread.Sleep( delay );
                }

            }//try
            catch(Exception anyException)
            {

                Logger.Error( "Caught unknown error", anyException );
                throw;

            }//catch
            finally
            {

                #region Log Method Exit
            if( Logger.IsDebugEnabled )
            {
                Logger.DebugFormat( "{0}() - {1}",
                MethodInfo.GetCurrentMethod().Name,
                "Exited" );
            }//if
            #endregion

            }//finally
        
            return stubResponse;

        }//method


        /// <summary>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <remarks/>
        public AcceptPreviousVisitDocumentResponse setPreviousVisitDocument( AcceptPreviousVisitDocumentRequest request )
        {

            #region Log Method Entrance
            if( Logger.IsDebugEnabled )
            {
                Logger.DebugFormat( "{0}() - {1}",
                MethodInfo.GetCurrentMethod().Name,
                "Entered" );
            }//if
            #endregion
        

            if( Logger.IsDebugEnabled )
            {
                
                Logger.Debug( "Proxying request for " + this.ToXmlString( request ) );

            }//if


            try
            {

                Logger.Error( "Caller called a stub method that was not implementated" );
                throw new NotImplementedException(
                    "This is a stub service. It does not allow writeable operations" );

            }
            finally
            {

                #region Log Method Exit
                if( Logger.IsDebugEnabled )
                {
                    Logger.DebugFormat( "{0}() - {1}",
                    MethodInfo.GetCurrentMethod().Name,
                    "Exited" );
                }//if
                #endregion

            }//finally

        }//method


        /// <summary>
        /// Toes the XML string.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <returns></returns>
        private string ToXmlString( object targetObject )
        {

            XStream xstream = new XStream();

            return xstream.ToXml( targetObject );

        }//method

    }//class

}//namespace
