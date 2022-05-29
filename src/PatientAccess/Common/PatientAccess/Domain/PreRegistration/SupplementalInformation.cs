using System;

namespace PatientAccess.Domain.PreRegistration
{
    [Serializable]
    public class SupplementalInformation
    {
        public      string  PatientMaidenName { get; set; }
        public      string  EmployerName { get; set; }
        public      string  EmploymentStatus { get; set; }
        public      string  EmployerAddress { get; set; }
        public      string  EmployerCity { get; set; }
        public      string  EmployerState { get; set; }
        public      string  EmployerZipCode { get; set; }
        public      string  EmployerPhone { get; set; }
        public      string  EmployerOccupation { get; set; }
        public      string  UnderWhatName { get; set; }
        public      string  AdmittingPhysicianName { get; set; }
        public      string  TreatingPhysicianName { get; set; }
        public      string  PrimaryCarePhysicianName { get; set; }
        public      string  GuarantorEmploymentStatus { get; set; }
        public      string  GuarantorPlaceOfEmployment { get; set; }
        public      string  GuarantorAddresOfEmployment { get; set; }
        public      string  GuarantorCity { get; set; }
        public      string  GuarantorStateProvince { get; set; }
        public      string  GuarantorZipCode { get; set; }
        public      string  GuarantorBusinessTelephoneNumber { get; set; }
        public      string  AreYouInsured { get; set; }
        public      string  PrimaryInsuranceCompanyName { get; set; }
        public      string  InsuranceCompanyTelephoneNumber { get; set; }
        public      string  InsurancePreCertificationPhoneNumber { get; set; }
        public      string  SubscribersFirstName { get; set; }
        public      string  SubscribersLastName { get; set; }
        public      string  SubscribersSSN { get; set; }
        public      string  SubscribersDOB { get; set; }
        public      string  PolicyNumber { get; set; }
        public      string  PolicyGroupName { get; set; }
        public      string  PrimaryInsuranceAddress { get; set; }
        public      string  PrimaryInsuranceCity { get; set; }
        public      string  PrimaryInsuranceState { get; set; }
        public      string  PrimaryInsuranceZipCode { get; set; }
        public      string  DoYouHaveSecondaryInsurance { get; set; }
        public      string  SecondaryInsuranceCompanyName { get; set; }
        public      string  SecondaryInsuranceCompanyTelephoneNumber { get; set; }
        public      string  SecondaryInsurancePreCertificationPhoneNumber { get; set; }
        public      string  SecondarySubscribersFirstName { get; set; }
        public      string  SecondarySubscribersLastName { get; set; }
        public      string  SecondarySSN { get; set; }
        public      string  SecondarySubscribersDOB { get; set; }
        public      string  SecondaryPolicyNumber { get; set; }
        public      string  SecondaryPolicyGroupName { get; set; }
        public      string  SecondaryInsuranceAddress { get; set; }
        public      string  SecondaryInsuranceCity { get; set; }
        public      string  SecondaryInsuranceState { get; set; }
        public      string  SecondaryInsuranceZipCode { get; set; }
        public      string  BestWayToContactYou { get; set; }
        public      string  BestTimeToContactYou { get; set; }
        public      string  PreferredMethodOfPayment { get; set; }
        public      string  EmailAddress { get; set; }
        public      string  WouldYouLikeToReceiveNewsletter { get; set; }
        public      string  ServiceLocation { get; set; }

    }
}
