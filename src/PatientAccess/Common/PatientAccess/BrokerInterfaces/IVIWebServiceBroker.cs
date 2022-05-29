using PatientAccess.Domain;
using PatientAccess.Services.DocumentManagement;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IVIWebServiceBroker.
    /// </summary>
    public interface IVIWebServiceBroker
    {
        DocumentListResponse GetDocumentList( long accountNumber, string facilityCode ); 
        string GetScanURL();
        string GetViewURL();
        string GetVIWebAppKey();
        string GetVIWebGuid();
        string SaveScanParameters(string hspcd, string patNumber, string patName, string Payor, string admitDate, string mrNumber);
        string SaveViewParameters(string hspcd, string docID);
        string GetScanHTML5URL();
        string GetViewHTML5URL();
        string GetVIwebLocalURL();
        VIWebPreviousDocuments GetPreviousVisitDocuments( string facilityCode,long medicalRecordNumber,InsurancePlan insurancePlan, VIWebPreviousDocument.VIWebPreviousDocumentType docType );
        VIWebPreviousDocuments GetPreviousVisitDocuments( string facilityCode,long medicalRecordNumber,InsurancePlan insurancePlan ); //Account anAccount );
        VIWebPreviousDocumentCnts GetPreviousVisitDocumentCnts( string facilityCode,long medicalRecordNumber,InsurancePlan insurancePlan );
        string GetViwebSessionID(string pbarEmployeeId);
        void SaveViwebSessionID(string sessionId,string pbarEmployeeId);
        void ClearViwebSessionID(string pbarEmployeeId);
        string Encrypt(string clearText);
    }

}
