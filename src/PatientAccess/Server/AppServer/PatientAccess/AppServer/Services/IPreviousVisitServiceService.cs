using PatientAccess.VIWeb;

namespace PatientAccess.Services
{
    public interface IPreviousVisitServiceService
    {
        void CancelAsync( object userState );
        PreviousVisitDocumentResponse getPreviousVisitDocuments( PreviousVisitDocumentRequest pvdr );
        AcceptPreviousVisitDocumentResponse setPreviousVisitDocument( AcceptPreviousVisitDocumentRequest request );
        string Url { get; set; }
        bool UseDefaultCredentials { get; set; }
    }
}
