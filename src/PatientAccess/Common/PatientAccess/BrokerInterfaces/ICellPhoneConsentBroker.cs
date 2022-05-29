using System.Collections.Generic;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    public interface ICellPhoneConsentBroker
    {
        ICollection<CellPhoneConsent> AllCellPhoneConsents();
        CellPhoneConsent ConsentWith(string code);
    }
}
