namespace PatientAccess.UI.PreRegistrationViews
{
    public interface ISupplementalInformationView
    {
        string ServiceLocation { get; set; }
        string PatientMaidenName { get; set; }
        string EmployerName { get; set; }
        string EmploymentStatus { get; set; }
        string EmployerAddress { get; set; }
        string EmployerCity { get; set; }
        string EmployerState { get; set; }
        string EmployerZipCode { get; set; }
        string EmployerPhone { get; set; }
        string EmployerOccupation { get; set; }
        string UnderWhatName { get; set; }
        string AdmittingPhysicianName { get; set; }
        string TreatingPhysicianName { get; set; }
        string PrimaryCarePhysicianName { get; set; }
        string GuarantorEmploymentStatus { get; set; }
        string GuarantorPlaceOfEmployment { get; set; }
        string GuarantorAddresOfEmployment { get; set; }
        string GuarantorCity { get; set; }
        string GuarantorStateProvince { get; set; }
        string GuarantorZipCode { get; set; }
        string GuarantorBusinessTelephoneNumber { get; set; }
        string AreYouInsured { get; set; }
        string PrimaryInsuranceCompanyName { get; set; } 
        string InsuranceCompanyTelephoneNumber { get; set; }
        string InsurancePreCertificationPhoneNumber { get; set; }
        string SubscribersFirstName { get; set; }
        string SubscribersLastName { get; set; }
        string SubscribersSsn { get; set; }
        string SubscribersDob { get; set; }
        string PolicyNumber { get; set; }
        string PolicyGroupName { get; set; }
        string PrimaryInsuranceAddress { get; set; }
        string PrimaryInsuranceCity { get; set; }
        string PrimaryInsuranceState { get; set; }
        string PrimaryInsuranceZipCode { get; set; }
        string DoYouHaveSecondaryInsurance { get; set; }
        string SecondaryInsuranceCompanyName { get; set; }
        string SecondaryInsuranceCompanyTelephoneNumber { get; set; }
        string SecondaryInsurancePreCertificationPhoneNumber { get; set; }
        string SecondarySubscribersFirstName { get; set; }
        string SecondarySubscribersLastName { get; set; }
        string SecondarySSN { get; set; }
        string SecondarySubscribersDOB { get; set; }
        string SecondaryPolicyNumber { get; set; }
        string SecondaryPolicyGroupName { get; set; }
        string SecondaryInsuranceAddress { get; set; }
        string SecondaryInsuranceCity { get; set; }
        string SecondaryInsuranceState { get; set; }
        string SecondaryInsuranceZipCode { get; set; }
        string BestWayToContactYou { get; set; }
        string BestTimeToContactYou { get; set; }
        string PreferredMethodOfPayment { get; set; }
        string EmailAddress { get; set; }
        string WouldYouLikeToReceiveNewsletter { get; set; }

        void ShowAsModelessDialog();
    }
}