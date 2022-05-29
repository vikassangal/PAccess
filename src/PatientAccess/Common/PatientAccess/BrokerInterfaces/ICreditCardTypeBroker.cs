using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for ICreditCardTypeBroker.
    /// </summary>
    public interface ICreditCardTypeBroker
    {
        IList AllCreditCardTypes();
        CreditCardProvider CreditCardTypeWith( string code );
    }
}
