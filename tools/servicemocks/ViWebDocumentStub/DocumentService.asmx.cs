using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Configuration;
using System.Threading;

namespace Hsd.PerotSystems.PatientAccess.Services.ViWeb
{

    /// <summary>
    /// 
    /// </summary>
    public class DocumentService : IDocumentServiceSoapBinding
    {

        /// <summary>
        /// </summary>
        /// <param name="dlReq"></param>
        /// <returns></returns>
        /// <remarks/>
        public DocumentListResponse findDocumentsFor( DocumentListRequest documentRequest )
        {

            DocumentListResponse stubResponse = new DocumentListResponse();

            stubResponse.account = new Account();
            stubResponse.account.accountNumber = documentRequest.accountNumber;
            stubResponse.account.cashDocuments = new CashDocument[]{};
            stubResponse.account.faxDocuments = new FaxDocument[]{};
            stubResponse.account.hspCode = documentRequest.hospitalServiceCode;
            stubResponse.account.nonCashDocuments = new NonCashDocument[]{};
            stubResponse.documentsWereFound = false;

            int delay = 0;
            int.TryParse( ConfigurationManager.AppSettings["processingDelay"], out delay );

            if( delay > 0 )
            {
                Thread.Sleep( delay );
            }

            return stubResponse;

        }//method

    }//class

}//method
