using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IGuarantorBroker.
    /// </summary>
    public interface IGuarantorBroker
    {
        Guarantor GuarantorFor( Account anAccount ); 
        void GuarantorEmployerDataFor( Account anAccount );       
    }
}
