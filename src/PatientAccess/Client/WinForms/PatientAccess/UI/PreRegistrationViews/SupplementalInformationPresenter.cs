using PatientAccess.Domain.PreRegistration;

namespace PatientAccess.UI.PreRegistrationViews
{
    internal class SupplementalInformationPresenter
    {
        public SupplementalInformationPresenter(ISupplementalInformationView supplementalInformationView,
                                                SupplementalInformation supplementalInformation)
        {
            View = supplementalInformationView;
            SupplementalInformation = supplementalInformation;
        }

        private SupplementalInformation SupplementalInformation { get; set; }

        private ISupplementalInformationView View { get; set; }

        public void ShowInformationAsDialog()
        {
            View.ServiceLocation = SupplementalInformation.ServiceLocation;
            View.PatientMaidenName = SupplementalInformation.PatientMaidenName;
            View.EmployerName = SupplementalInformation.EmployerName;
            View.EmploymentStatus = SupplementalInformation.EmploymentStatus;
            View.EmployerAddress = SupplementalInformation.EmployerAddress;
            View.EmployerCity = SupplementalInformation.EmployerCity;
            View.EmployerState = SupplementalInformation.EmployerState;
            View.EmployerZipCode = SupplementalInformation.EmployerZipCode;
            View.EmployerPhone = SupplementalInformation.EmployerPhone;
            View.EmployerOccupation = SupplementalInformation.EmployerOccupation;

            View.UnderWhatName = SupplementalInformation.UnderWhatName;
            View.AdmittingPhysicianName = SupplementalInformation.AdmittingPhysicianName;
            View.TreatingPhysicianName = SupplementalInformation.TreatingPhysicianName;
            View.PrimaryCarePhysicianName = SupplementalInformation.PrimaryCarePhysicianName;

            View.GuarantorEmploymentStatus = SupplementalInformation.GuarantorEmploymentStatus;
            View.GuarantorPlaceOfEmployment = SupplementalInformation.GuarantorPlaceOfEmployment;
            View.GuarantorAddresOfEmployment = SupplementalInformation.GuarantorAddresOfEmployment;
            View.GuarantorCity = SupplementalInformation.GuarantorCity;
            View.GuarantorStateProvince = SupplementalInformation.GuarantorStateProvince;
            View.GuarantorZipCode = SupplementalInformation.GuarantorZipCode;
            View.GuarantorBusinessTelephoneNumber = SupplementalInformation.GuarantorBusinessTelephoneNumber;

            View.AreYouInsured = SupplementalInformation.AreYouInsured;
            View.PrimaryInsuranceCompanyName = SupplementalInformation.PrimaryInsuranceCompanyName;
            View.InsuranceCompanyTelephoneNumber = SupplementalInformation.InsuranceCompanyTelephoneNumber;
            View.InsurancePreCertificationPhoneNumber = SupplementalInformation.InsurancePreCertificationPhoneNumber;
            View.SubscribersFirstName = SupplementalInformation.SubscribersFirstName;
            View.SubscribersLastName = SupplementalInformation.SubscribersLastName;
            View.SubscribersSsn = SupplementalInformation.SubscribersSSN;
            View.SubscribersDob = SupplementalInformation.SubscribersDOB;
            View.PolicyNumber = SupplementalInformation.PolicyNumber;
            View.PolicyGroupName = SupplementalInformation.PolicyGroupName;
            View.PrimaryInsuranceAddress = SupplementalInformation.PrimaryInsuranceAddress;
            View.PrimaryInsuranceCity = SupplementalInformation.PrimaryInsuranceCity;
            View.PrimaryInsuranceState = SupplementalInformation.PrimaryInsuranceState;
            View.PrimaryInsuranceZipCode = SupplementalInformation.PrimaryInsuranceZipCode;

            View.DoYouHaveSecondaryInsurance = SupplementalInformation.DoYouHaveSecondaryInsurance;
            View.SecondaryInsuranceCompanyName = SupplementalInformation.SecondaryInsuranceCompanyName;
            View.SecondaryInsuranceCompanyTelephoneNumber =
                SupplementalInformation.SecondaryInsuranceCompanyTelephoneNumber;
            View.SecondaryInsurancePreCertificationPhoneNumber =
                SupplementalInformation.SecondaryInsurancePreCertificationPhoneNumber;
            View.SecondarySubscribersFirstName = SupplementalInformation.SecondarySubscribersFirstName;
            View.SecondarySubscribersLastName = SupplementalInformation.SecondarySubscribersLastName;
            View.SecondarySSN = SupplementalInformation.SecondarySSN;
            View.SecondarySubscribersDOB = SupplementalInformation.SecondarySubscribersDOB;
            View.SecondaryPolicyNumber = SupplementalInformation.SecondaryPolicyNumber;
            View.SecondaryPolicyGroupName = SupplementalInformation.SecondaryPolicyGroupName;
            View.SecondaryInsuranceAddress = SupplementalInformation.SecondaryInsuranceAddress;
            View.SecondaryInsuranceCity = SupplementalInformation.SecondaryInsuranceCity;
            View.SecondaryInsuranceState = SupplementalInformation.SecondaryInsuranceState;
            View.SecondaryInsuranceZipCode = SupplementalInformation.SecondaryInsuranceZipCode;

            View.BestWayToContactYou = SupplementalInformation.BestWayToContactYou;
            View.BestTimeToContactYou = SupplementalInformation.BestTimeToContactYou;
            View.PreferredMethodOfPayment = SupplementalInformation.PreferredMethodOfPayment;

            View.WouldYouLikeToReceiveNewsletter = SupplementalInformation.WouldYouLikeToReceiveNewsletter;
            View.EmailAddress = SupplementalInformation.EmailAddress;
            View.ShowAsModelessDialog();
        }
    }
}