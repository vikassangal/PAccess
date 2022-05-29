using PatientAccess.AddressValidationProxy;

namespace PatientAccess.Services
{
    public interface IAddressValidationService
    {
        addressValidationResult validateAddress( addressValidationRequest addressValidationRequest );
    }
}
