using PatientAccess.PriorAccountBalanceProxy;

namespace PatientAccess.Services
{
    public interface IPriorAccountBalanceService
    {
        void CancelAsync(object userState);
        priorAccountBalanceRequest identifyPriorAccountBalances(priorAccountBalanceRequest identifyPriorAccountBalances1);

        string Url { get; set; }
        bool UseDefaultCredentials { get; set; }
    }
}
