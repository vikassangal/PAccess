using PatientAccess.Domain;
using System;
using System.Net;

namespace PatientAccess.BrokerInterfaces
{
    public interface IDOFRInitiateBroker
    {
        Tuple<DOFRAPIResponse, HttpStatusCode> DOFRInitiate(DOFRAPIRequest dOFRAPIRequest, Account account);
        string GetInsurancePlanPartOfIPA(Account account);
        void SetInsurancePlanPartOfIPA(Account account);
    }
}
