using PatientAccess.ComplianceCheckerProxy;

namespace PatientAccess.Services
{
    public interface IComplianceCheckerService
    {
        void CancelAsync(object userState);
        createComplianceCheckerRequestResponse createComplianceCheckerRequest(createComplianceCheckerRequest createComplianceCheckerRequest1);
        void createComplianceCheckerRequestAsync(createComplianceCheckerRequest createComplianceCheckerRequest1);
        void createComplianceCheckerRequestAsync(createComplianceCheckerRequest createComplianceCheckerRequest1, object userState);
        event createComplianceCheckerRequestCompletedEventHandler createComplianceCheckerRequestCompleted;
        string Url { get; set; }
        bool UseDefaultCredentials { get; set; }
    }
}
