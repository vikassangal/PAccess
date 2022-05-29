using PatientAccess.CreditValidationProxy;

namespace PatientAccess.Services
{
    public interface ICreditValidationService
    {
        void CancelAsync(object userState);
        initiateValidateCreditResponse initiateValidateCredit(initiateValidateCredit initiateValidateCredit1);
        void initiateValidateCreditAsync(initiateValidateCredit initiateValidateCredit1, object userState);
        void initiateValidateCreditAsync(initiateValidateCredit initiateValidateCredit1);
        event initiateValidateCreditCompletedEventHandler initiateValidateCreditCompleted;
        obtainCreditResultResponse obtainCreditResult(obtainCreditResult obtainCreditResult1);
        void obtainCreditResultAsync(obtainCreditResult obtainCreditResult1, object userState);
        void obtainCreditResultAsync(obtainCreditResult obtainCreditResult1);
        event obtainCreditResultCompletedEventHandler obtainCreditResultCompleted;
        string Url { get; set; }
        bool UseDefaultCredentials { get; set; }
    }
}
