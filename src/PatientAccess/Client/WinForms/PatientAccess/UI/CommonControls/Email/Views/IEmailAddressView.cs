 using PatientAccess.Domain;

namespace PatientAccess.UI.CommonControls.Email.Views
{
    public interface IEmailAddressView
    {
        void DoNotShowEmailAddress();
        void SetEmailAddressToNormalColor();
        ContactPoint Model_ContactPoint { get; }
        void SetEmailAddressAsRequired();
        void SetEmailAddressAsPreferred();
        void SetGuarantorEmailAddressAsRequired();
        void SetGuarantorEmailAddressAsPreferred();
    }
}