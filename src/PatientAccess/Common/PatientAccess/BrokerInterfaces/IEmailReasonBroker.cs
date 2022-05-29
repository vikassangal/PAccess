using System.Collections.Generic;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    public interface IEmailReasonBroker
    {
        ICollection<EmailReason> AllEmailReasons();
        EmailReason EmailReasonWith(string code);

    }
}
