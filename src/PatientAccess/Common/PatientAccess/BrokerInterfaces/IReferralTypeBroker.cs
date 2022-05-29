using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IReferralTypeBroker.
    /// </summary>
    public interface IReferralTypeBroker
    {
        ICollection ReferralTypesFor( long facilityNumber );
        ReferralType ReferralTypeWith(long facilityNumber, string code);
    }
}
