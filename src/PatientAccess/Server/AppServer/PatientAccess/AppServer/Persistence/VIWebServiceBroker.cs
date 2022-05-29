using System;
using System.Configuration;
using System.Data;
using System.Net;
using System.Threading;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Services;
using PatientAccess.Services.DocumentManagement;
using PatientAccess.VIWeb;
using log4net;
using System.Data.SqlClient;
using Extensions.Persistence;
using System.Web;
using System.Collections.Generic;
using System.Web.Caching;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using PatientAccess.Persistence.Nhibernate;

namespace PatientAccess.Persistence
{
	/// <summary>
	/// VIWebServiceBroker is used to call webservice components provided by VI team, including
    /// the Document Service and the Previous Documents Service
	/// </summary>
	[Serializable]
    public class VIWebServiceBroker : AbstractBroker, IVIWebServiceBroker
    {
        
        #region Construction and Finalization

        public VIWebServiceBroker()
        {
            viWebPreviousDocumentsUrl =
                ConfigurationManager.AppSettings["VIWebPreviousVisitServiceUrl"];
            
            PreviousVisitService = new PreviousVisitServiceService( viWebPreviousDocumentsUrl ) ;
        }

        //internal VIWebServiceBroker( IPreviousVisitServiceService prevVisitServiceService )
        public VIWebServiceBroker( IPreviousVisitServiceService prevVisitServiceService )
        {
            PreviousVisitService = prevVisitServiceService ;
        }        

        #endregion

        #region Public Methods

        /// <summary>
        /// GetPreviousVisitDocuments - retrieve documents from VIWeb for all previous document types.
        /// For each document returned, a Domain.VIWebPreviousDocument instance is created and added
        /// to the collection of Domain.VIWebPreviousDocuments
        /// </summary>
        /// <param name="facilityCode">facility HSP Code</param>
        /// <param name="medicalRecordNumber">medicalRecordNumber</param>
        /// <param name="insurancePlan">an Instance of a Domain.InsurancePlan</param>
        /// <returns>List of matching documents</returns>

        public VIWebPreviousDocumentCnts GetPreviousVisitDocumentCnts(
            string facilityCode,
            long medicalRecordNumber,
            InsurancePlan insurancePlan)
        {
            VIWebPreviousDocumentCnts documentCounts = null;

            if ( facilityCode != null && !facilityCode.Equals(string.Empty) && medicalRecordNumber != 0 )
            {
                documentCounts = new VIWebPreviousDocumentCnts();

                VIWebPreviousDocuments advPreviousDocuments = this.GetPreviousVisitDocuments( 
                    facilityCode,medicalRecordNumber,insurancePlan,
                    VIWebPreviousDocument.VIWebPreviousDocumentType.ADV);
                documentCounts.ADVDocumentCnt = advPreviousDocuments.PreviousDocumentList.Count;

                VIWebPreviousDocuments dlPreviousDocuments = this.GetPreviousVisitDocuments(
                    facilityCode,medicalRecordNumber,insurancePlan,
                    VIWebPreviousDocument.VIWebPreviousDocumentType.DL);
                documentCounts.DLDocumentCnt = dlPreviousDocuments.PreviousDocumentList.Count;

                VIWebPreviousDocuments insCardPreviousDocuments = this.GetPreviousVisitDocuments(
                    facilityCode,medicalRecordNumber,insurancePlan,
                    VIWebPreviousDocument.VIWebPreviousDocumentType.INSCARD);
                documentCounts.INSCardDocumentCnt = insCardPreviousDocuments.PreviousDocumentList.Count;

                VIWebPreviousDocuments nppPreviousDocuments = this.GetPreviousVisitDocuments(
                    facilityCode,medicalRecordNumber,insurancePlan,
                    VIWebPreviousDocument.VIWebPreviousDocumentType.NPP);
                documentCounts.NPPDocumentCnt = nppPreviousDocuments.PreviousDocumentList.Count;
            }
            return documentCounts == null ? new VIWebPreviousDocumentCnts() : documentCounts;
        }

        public VIWebPreviousDocuments GetPreviousVisitDocuments( 
            string facilityCode,
            long medicalRecordNumber,
            InsurancePlan insurancePlan )
        {
            VIWebPreviousDocuments previousDocuments = null;

            if ( facilityCode != null && !facilityCode.Equals(string.Empty))
            {
                previousDocuments = new VIWebPreviousDocuments();

                VIWebPreviousDocuments advPreviousDocuments = this.GetPreviousVisitDocuments(
                    facilityCode, medicalRecordNumber, insurancePlan,
                    VIWebPreviousDocument.VIWebPreviousDocumentType.ADV);
                previousDocuments.PreviousDocumentList.AddRange(advPreviousDocuments.PreviousDocumentList);

                VIWebPreviousDocuments dlPreviousDocuments = this.GetPreviousVisitDocuments(
                    facilityCode, medicalRecordNumber, insurancePlan,
                    VIWebPreviousDocument.VIWebPreviousDocumentType.DL);
                previousDocuments.PreviousDocumentList.AddRange(dlPreviousDocuments.PreviousDocumentList);

                VIWebPreviousDocuments insCardPreviousDocuments = this.GetPreviousVisitDocuments(
                    facilityCode, medicalRecordNumber, insurancePlan,
                    VIWebPreviousDocument.VIWebPreviousDocumentType.INSCARD);
                previousDocuments.PreviousDocumentList.AddRange(insCardPreviousDocuments.PreviousDocumentList);

                VIWebPreviousDocuments nppPreviousDocuments = this.GetPreviousVisitDocuments(
                    facilityCode, medicalRecordNumber, insurancePlan,
                    VIWebPreviousDocument.VIWebPreviousDocumentType.NPP);
                previousDocuments.PreviousDocumentList.AddRange(nppPreviousDocuments.PreviousDocumentList);
            }
            return previousDocuments == null ? new VIWebPreviousDocuments() : previousDocuments;
        }

        /// <summary>
        /// GetPreviousVisitDocuments - retrieve documents from VIWeb for the specified document type.
        /// For each document returned, a Domain.VIWebPreviousDocument instance is created and added
        /// to the collection of Domain.VIWebPreviousDocuments
        /// </summary>
        /// <param name="facilityCode">facility HSP Code</param>
        /// <param name="medicalRecordNumber">medicalRecordNumber</param>
        /// <param name="insurancePlan">an Instance of a Domain.Insurance Plan</param>
        /// <param name="documentType">documentType enumerication</param>
        /// <returns>List of matching documents</returns>
        
        public VIWebPreviousDocuments GetPreviousVisitDocuments( //Domain.Account anAccount, 
            string facilityCode,
            long medicalRecordNumber,
            InsurancePlan insurancePlan,
            VIWebPreviousDocument.VIWebPreviousDocumentType documentType )
        {
            if( facilityCode == null || facilityCode.Equals(string.Empty)
                || documentType.ToString() == VIWebPreviousDocument.VIWebPreviousDocumentType.UNKNOWN.ToString() )
            {
                return new VIWebPreviousDocuments();
            }

            VIWebPreviousDocuments previousDocuments = new VIWebPreviousDocuments();

            try
            {
                PreviousVisitDocumentRequest request = this.BuildPreviousDocumentRequest( 
                    facilityCode, medicalRecordNumber, insurancePlan, documentType );

                // if doc type not specified, return an empty list
 
                if( string.IsNullOrEmpty( request.docType ) )
                {
                    return previousDocuments;
                }

                // retrieve documents based on the request built in this.BuildPreviousDocumentRequest

                PreviousVisitDocumentResponse response = this.PreviousVisitService.getPreviousVisitDocuments( request );

                if( response != null
                    && response.documentsWereFound 
                    && response.previousVisitDocuments != null 
                    && response.previousVisitDocuments.Length > 0 )
                {
                    foreach( PreviousVisitDocument document in response.previousVisitDocuments )
                    {
                        VIWebPreviousDocument aDocument = new VIWebPreviousDocument( 
                            Convert.ToInt64(document.accountNumber.Trim() ), documentType, 
                            document.documentDate.HasValue ? document.documentDate.Value : DateTime.MinValue, 
                            document.documentId.Trim(), document.documentObject );

                        previousDocuments.PreviousDocumentList.Add( aDocument );
                    }
                }
            }
            catch( Exception ex )
            {
                c_log.DebugFormat( "Error retrieving previous visit documents for HSP: {0}, MRN: {1}, "
                + " DocumentType: {2} - {3}", 
                    facilityCode, medicalRecordNumber, 
                    documentType.ToString(), ex.Message );
            }

            return previousDocuments;
        }

        public DocumentListResponse GetDocumentList( long accountNumber, string facilityCode ) 
		{
			DocumentServiceService service    = new DocumentServiceService();
			DocumentListRequest request       = new DocumentListRequest();
            DocumentListResponse response     = new DocumentListResponse();

            try
            {
                request.accountNumber           = accountNumber.ToString();
                request.hospitalServiceCode     = facilityCode;

                request.appGUID                 = this.GetVIWebGuid();
                request.appKEY                  = this.GetVIWebAppKey();

                response = service.findDocumentsFor( request );
                
            }
            catch( WebException wex )
            {
                if( IsUnderlyingConnectionClosedException( wex ) 
                    && this.retryCount < this.retryAttempts )
                {
                    this.retryCount++;

                    c_log.Error( string.Format( CXN_CLOSED_MSG, "GetDocumentList" ), wex );
                    service.Dispose();
                    Thread.Sleep( 500 );
                    return this.GetDocumentList( accountNumber, facilityCode );
                }
                else
                {
                    throw;
                }
            }
            catch( Exception ex )
            {
                string msg = "VIWebServiceBroker failed to call web service: ";
                throw BrokerExceptionFactory.BrokerExceptionFrom( msg, ex, c_log );
            }

            return response;
		}

        public string GetVIWebAppKey()
        {
            return ConfigurationManager.AppSettings[APP_KEY];
        }

        public string GetVIWebGuid()
        {
            return ConfigurationManager.AppSettings[GUID];
        }

        public string GetScanURL()
        {
            return ConfigurationManager.AppSettings[SCAN_KEY];
        }
        
        public string GetViewURL()
        {
            return ConfigurationManager.AppSettings[VIEW_KEY];
        }

        public string GetScanHTML5URL()
        {
            return ConfigurationManager.AppSettings[HTML5_SCAN_KEY];
        }

        public string GetViewHTML5URL()
        {
            return ConfigurationManager.AppSettings[HTML5_VIEW_KEY];
        }

        public string GetVIwebLocalURL()
        {
            return ConfigurationManager.AppSettings[HTML5_LOCAL_KEY];
        }

        private IPreviousVisitServiceService PreviousVisitService
        {
            get { return i_service; }
            set { i_service = value; }
        }

        private Cache Cache
        {
            get
            {
                return HttpRuntime.Cache;
            }
        }

        public string PBAREmployeeId
        {
            get { return i_PBAREmployeeId; }
            set { i_PBAREmployeeId = value; }
        }

        private string PASViwebSessionIDKey
        {
            get { return PBAREmployeeId + VIWEB_SESSIONID; }
        }

        private string PASViwebSessionID
        {
            get
            {
                return this.GetViwebAccessToken(PASViwebSessionIDKey);
            }
            set
            {
                var SessionId = this.GetViwebAccessToken(PASViwebSessionIDKey);
                if (string.IsNullOrEmpty(SessionId))
                {
                   this.SaveViwebAccessToken(PASViwebSessionIDKey, value);
                }
            }
        }

        public void SaveViwebSessionID(string sessionId, string pbarEmployeeId)
        {
            this.PBAREmployeeId = pbarEmployeeId;
            this.PASViwebSessionID = sessionId;
        }

        public string GetViwebSessionID(string pbarEmployeeId)
        {
            this.PBAREmployeeId = pbarEmployeeId;
            return this.PASViwebSessionID;
        }

        public void ClearViwebSessionID(string pbarEmployeeId)
        {
            if (!string.IsNullOrEmpty(GetViwebSessionID(pbarEmployeeId)))
            {
                this.DeleteViwebAccessToken(PASViwebSessionIDKey);
            }
        }
        public string Encrypt(string clearText)
        {
            IEncryptor encryptor = new AesEncryptor();
            return encryptor.Encrypt(clearText);
         }

        public static string Decrypt(string cipherText)
        {
            IEncryptor encryptor = new AesEncryptor();
            return encryptor.Decrypt(cipherText);
        }
        #endregion

        #region private methods

        /// <summary>
        /// BuildPreviousDocumentRequest - builds the request object in preparation for a call to the 
        /// VI web service that returns previously scanned documents
        /// 
        /// Set required values on the request, including
        /// * facility
        /// * MRN
        /// * desired document type
        /// * return format (we will always return VIWeb document ids that are subsequently passed to
        ///   the FormWebBrowserView to be displayed).
        /// 
        /// Based on the doc type being requested (and coverage category for insurance cards), set
        /// * the number of months to search for the specified document type
        /// * wether or not multiple documents or a single document is returned
        /// 
        /// </summary>
        /// <param name="faciltiyCode">a facility HSP Code</param>
        /// <param name="medicalRecordNumber">a patients medical record number</param>
        /// <param name="insurancePlan">an instance of a Domain.InsurancePlan</param>
        /// <param name="documentType">a VIWebPreviousDocumentType (enum)</param>
        /// <returns>a PreviousVisitDocumentRequest instance</returns>

        private PreviousVisitDocumentRequest BuildPreviousDocumentRequest(
            string faciltiyCode,
            long medicalRecordNumber,
            InsurancePlan insurancePlan,
            VIWebPreviousDocument.VIWebPreviousDocumentType documentType )
        {
            PreviousVisitDocumentRequest aRequest = new PreviousVisitDocumentRequest();

            aRequest.docType = documentType.ToString();
            aRequest.hspCode = faciltiyCode;
            aRequest.medicalRecordNumber = medicalRecordNumber.ToString();
            aRequest.returnFormat = RETURN_FORMAT_DOC_ID;

            if( documentType == VIWebPreviousDocument.VIWebPreviousDocumentType.ADV )
            {
                aRequest.maxAge = REQUEST_MAX_NO_MAX;
                aRequest.multiple = REQUEST_SINGLE;
            }
            else if( documentType == VIWebPreviousDocument.VIWebPreviousDocumentType.DL )
            {
                aRequest.maxAge = REQUEST_MAX_2_YEARS;
                aRequest.multiple = REQUEST_SINGLE;
            }
            else if( documentType == VIWebPreviousDocument.VIWebPreviousDocumentType.INSCARD )
            {
                aRequest.multiple = REQUEST_MULTIPLE;

                if( insurancePlan.CoverageCategoryIsCommercial )
                {
                    aRequest.maxAge = REQUEST_MAX_3_MONTHS;
                }
                else if( insurancePlan.CoverageCategoryIsMedicaid )
                {
                    aRequest.maxAge = REQUEST_MAX_CURRENT_MONTH;
                }
                else if( insurancePlan.CoverageCategoryIsMedicare )
                {
                    aRequest.maxAge = REQUEST_MAX_1_YEAR;
                }
                else if( insurancePlan.CoverageCategoryIsTricare )
                {
                    aRequest.maxAge = REQUEST_MAX_6_MONTHS;
                }
                else
                {
                    aRequest = new PreviousVisitDocumentRequest();
                }
            }
            else if( documentType == VIWebPreviousDocument.VIWebPreviousDocumentType.NPP )
            {
                aRequest.maxAge = REQUEST_MAX_NO_MAX;
                aRequest.multiple = REQUEST_SINGLE;
            }

            return aRequest;
        }


        private bool IsUnderlyingConnectionClosedException( WebException wex )
        {
            bool retVal = false;

            if( wex != null )
            {
                retVal = ( wex.Message != null && wex.Message.IndexOf( ERR_CXN_CLOSED ) != -1 ) 
                    || 
                    ( wex.InnerException != null && wex.InnerException.Message != null &&
                        wex.InnerException.Message.IndexOf( ERR_CXN_CLOSED ) != -1 ) 
                    || 
                    ( wex.InnerException != null && wex.InnerException.Message != null &&
                        wex.InnerException.Message.IndexOf( ERR_TIMEOUT ) != -1 ) 
                    ||
                    ( wex.ToString().IndexOf( ERR_CXN_CLOSED ) != -1 )
                    ||
                    ( wex.ToString().IndexOf( ERR_TIMEOUT ) != -1 );
            }
            return retVal;
        }
        #endregion
        
        #region Data Elements

        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( VIWebServiceBroker ) );

        private int                         retryAttempts = Convert.ToInt32( ConfigurationManager.AppSettings["RetryAttempts"] );
        private int                         retryCount = 0;
        private string                      viWebPreviousDocumentsUrl = string.Empty;

        private IPreviousVisitServiceService i_service ;

        #endregion

        #region constants

        private const int                   RETURN_FORMAT_DOC_ID = 0;
        private const string                REQUEST_MULTIPLE = "Y",
                                            REQUEST_SINGLE = "N";

        private const int                   REQUEST_MAX_CURRENT_MONTH = 0,
                                            REQUEST_MAX_NO_MAX = 999,
                                            REQUEST_MAX_3_MONTHS = 3,
                                            REQUEST_MAX_6_MONTHS = 6,
                                            REQUEST_MAX_1_YEAR = 12,
                                            REQUEST_MAX_2_YEARS = 24;

        private const string                SCAN_KEY                = "PatientAccess.UI.DocumentImagingViews.ScanURL",
                                            VIEW_KEY                = "PatientAccess.UI.DocumentImagingViews.ViewURL",
                                            HTML5_SCAN_KEY          = "PatientAccess.UI.DocumentImagingViews.ScanHTML5Url",
                                            HTML5_VIEW_KEY          = "PatientAccess.UI.DocumentImagingViews.ViewHTML5Url",
                                            HTML5_LOCAL_KEY         = "LocalVIwebUrl",
                                            APP_KEY                 = "VIWebAppKey",
                                            GUID                    = "VIWebGuid";
        private const string 
                                            ERR_CXN_CLOSED          = "underlying connection was closed",
                                            ERR_TIMEOUT             = "The operation has timed-out",
                                            CXN_CLOSED_MSG          = "Connection to CIE was closed for the {0} web method.  Attempting connection again.";
        private const string DBPARAMETER_HSPCD = "@hspcd";
        private const string DBPARAMETER_PATNUMBER = "@patNumber";
        private const string DBPARAMETER_PATNAME = "@patName";
        private const string DBPARAMETER_PAYOR = "@Payor";
        private const string DBPARAMETER_CurrentDATE = "@currentDate";
        private const string DBPARAMETER_MRNUMBER = "@mrNumber";
        private const string DBPARAMETER_ID = "@id";
        private const string DBPARAMETER_DOCID = "@docID";
        private const string DBPARAMETER_KEY = "@key";
        private const string DBPARAMETER_VALUE= "@value";
        private const string DBCOLUMN_VALUE = "value";
        private const string DBPROCEDURE_ADDVIWEBSCANPARAMETER = "ViWeb.AddVIWebScanParameter";
        private const string DBPROCEDURE_ADDVIWEBVIEWPARAMETER = "ViWeb.AddVIWebViewParameter";
        private const string DBPROCEDURE_GETVIWEBSCANPARAMETER = "ViWeb.GetVIwebScanParameter";
        private const string DBPROCEDURE_GETVIWEBVIEWPARAMETER = "ViWeb.GetVIwebViewParameter";
        private const string DBPROCEDURE_SAVEVIWEBACEESSTOKEN = "ViWeb.SaveVIwebAccessToken";
        private const string DBPROCEDURE_GETVIWEBACCESSTOKEN = "ViWeb.GetVIwebAccessToken";
        private const string DBPROCEDURE_DELETEVIWEBACCESSTOKEN = "ViWeb.DeleteVIwebAccessToken";
        private string i_PBAREmployeeId = "";
        private const string VIWEB_SESSIONID = "ViwebSessionId";
        #endregion

        #region IVIWebServiceBroker Members

        /// <summary>
        /// Scan Document
        /// </summary>
        /// <param name="hspcd"></param>
        /// <param name="patNumber"></param>
        /// <param name="patName"></param>
        /// <param name="Payor"></param>
        /// <param name="currentDate"></param>
        /// <param name="mrNumber"></param>
        /// <returns></returns>
        public string SaveScanParameters(string hspcd, string patNumber, string patName, string Payor, string currentDate, string mrNumber)
        {

            SqlCommand sqlCommand = null;

            sqlCommand = this.CommandFor(DBPROCEDURE_ADDVIWEBSCANPARAMETER);

            var parameterHspcd =
                new SqlParameter(DBPARAMETER_HSPCD, SqlDbType.VarChar) {Value = hspcd};
            sqlCommand.Parameters.Add(parameterHspcd);

            var parameterPatNumber =
                new SqlParameter(DBPARAMETER_PATNUMBER, SqlDbType.VarChar) {Value = patNumber};
            sqlCommand.Parameters.Add(parameterPatNumber);

            var parameterPatName =
                new SqlParameter(DBPARAMETER_PATNAME, SqlDbType.VarChar) {Value = patName};
            sqlCommand.Parameters.Add(parameterPatName);

            var parameterPayor =
                new SqlParameter(DBPARAMETER_PAYOR, SqlDbType.VarChar) {Value = Payor};
            sqlCommand.Parameters.Add(parameterPayor);

            var parameterCurrentDate =
                new SqlParameter(DBPARAMETER_CurrentDATE, SqlDbType.VarChar) {Value = currentDate};
            sqlCommand.Parameters.Add(parameterCurrentDate);

            var parameterMrn =
                new SqlParameter(DBPARAMETER_MRNUMBER, SqlDbType.VarChar) {Value = mrNumber};
            sqlCommand.Parameters.Add(parameterMrn);

            var parameterId =
                        new SqlParameter(DBPARAMETER_ID, SqlDbType.Int);
            sqlCommand.Parameters.Add(parameterId).Direction = ParameterDirection.Output;

            this.ExecuteNonQuery(sqlCommand);
            var id = sqlCommand.Parameters["@id"].Value.ToString();
            return id;
        }
        /// <summary>
        /// View Document
        /// </summary>
        /// <param name="hspcd"></param>
        /// <param name="docID"></param>
        /// <returns></returns>
        public string SaveViewParameters(string hspcd, string docID)
        {
            SqlCommand sqlCommand = null;
            sqlCommand = this.CommandFor(DBPROCEDURE_ADDVIWEBVIEWPARAMETER);
            var parameterHspcd =
                new SqlParameter(DBPARAMETER_HSPCD, SqlDbType.VarChar) {Value = hspcd};
            sqlCommand.Parameters.Add(parameterHspcd);
            var parameterdocId =
                new SqlParameter(DBPARAMETER_DOCID, SqlDbType.VarChar) {Value = docID};
            sqlCommand.Parameters.Add(parameterdocId);
            var parameterId =
                        new SqlParameter(DBPARAMETER_ID, SqlDbType.Int);
            sqlCommand.Parameters.Add(parameterId).Direction = ParameterDirection.Output;
            this.ExecuteNonQuery(sqlCommand);
            string id = sqlCommand.Parameters["@id"].Value.ToString();
            return id;
        }

        public DataSet GetScanParam(int id)
        {
            SqlDataAdapter adapter = null;
            var parameterId =
                new SqlParameter(DBPARAMETER_ID, SqlDbType.Int) {Value = id};
            adapter = this.AdapterFor(DBPROCEDURE_GETVIWEBSCANPARAMETER);
            adapter.SelectCommand.Parameters.Add(parameterId);
            var ds = new DataSet();
            adapter.Fill(ds);
            return ds;
        }
        public DataSet GetViewParam(int id)
        {
            SqlDataAdapter adapter = null;
            var parameterId =
                new SqlParameter(DBPARAMETER_ID, SqlDbType.Int) {Value = id};
            adapter = this.AdapterFor(DBPROCEDURE_GETVIWEBVIEWPARAMETER);
            adapter.SelectCommand.Parameters.Add(parameterId);
            var ds = new DataSet();
            adapter.Fill(ds);
            return ds;
        }
        public string SaveViwebAccessToken(string PASViwebSessionIDKey,string PASViwebSessionIDValue)
        {
            SqlCommand sqlCommand = null;
            sqlCommand = this.CommandFor(DBPROCEDURE_SAVEVIWEBACEESSTOKEN);
            var parameterkey =
                new SqlParameter(DBPARAMETER_KEY, SqlDbType.VarChar) { Value = PASViwebSessionIDKey };
            sqlCommand.Parameters.Add(parameterkey);
            var parameterValue =
                new SqlParameter(DBPARAMETER_VALUE, SqlDbType.VarChar) { Value = PASViwebSessionIDValue };
            sqlCommand.Parameters.Add(parameterValue);
            var parameterId =
                        new SqlParameter(DBPARAMETER_ID, SqlDbType.Int);
            sqlCommand.Parameters.Add(parameterId).Direction = ParameterDirection.Output;
            this.ExecuteNonQuery(sqlCommand);
            string id = sqlCommand.Parameters["@id"].Value.ToString();
            return id;
        }

        public string GetViwebAccessToken(string PASViwebSessionIDKey)
        {
            string accessToken = "";
            SqlCommand sqlCommand = null;
            sqlCommand = this.CommandFor(DBPROCEDURE_GETVIWEBACCESSTOKEN);
            var parameterKey =
                new SqlParameter(DBPARAMETER_KEY, SqlDbType.VarChar) { Value = PASViwebSessionIDKey };
            sqlCommand.Parameters.Add(parameterKey);
            SafeReader reader = this.ExecuteReader(sqlCommand);

            while (reader.Read())
            {
                accessToken = reader.GetString(DBCOLUMN_VALUE);
            }
            return accessToken;
        }

        public void DeleteViwebAccessToken(string PASViwebSessionIDKey)
        {
            SqlCommand sqlCommand = null;
            sqlCommand = this.CommandFor(DBPROCEDURE_DELETEVIWEBACCESSTOKEN);
            var parameterkey =
                new SqlParameter(DBPARAMETER_KEY, SqlDbType.VarChar) { Value = PASViwebSessionIDKey };
            sqlCommand.Parameters.Add(parameterkey);
            this.ExecuteNonQuery(sqlCommand);
        }

        #endregion

    }
}
