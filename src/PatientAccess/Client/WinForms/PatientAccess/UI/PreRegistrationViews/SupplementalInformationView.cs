using System;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;

namespace PatientAccess.UI.PreRegistrationViews
{
    public partial class SupplementalInformationView : TimeOutFormView, ISupplementalInformationView
    {
        public SupplementalInformationView()
        {
            InitializeComponent();
        }

        #region ISupplementalInformationView Members

        public void ShowAsDialog()
        {
            ShowDialog();
        }

        public void ShowAsModelessDialog()
        {
            Show();
        }

        #endregion

        private void CloseButtonOnClick(object sender, EventArgs e)
        {
            AccountClosing = false;
            Close();
        }

        private void OnClosing( object sender, EventArgs e )
        {
            // Enable Account Supplemental Information button if user is still 
            // on the account and wants to reopen the view again
            if ( !AccountClosing )
            {
                ViewFactory.Instance.CreateView<PatientAccessView>().EnableSupplementalInformationButton();
            }
        }

        #region Location Information

        public string ServiceLocation
        {
            get { return serviceLocation.Text; }
            set { serviceLocation.Text = value; }

        }

        #endregion

        #region Patient Information


        public string PatientMaidenName
        {
            get { return patientMaidenName.Text; }
            set { patientMaidenName.Text = value; }
        }

        #endregion

        #region Employment information

        public string EmployerName
        {
            get { return employerName.Text; }
            set { employerName.Text = value; }
        }

        public string EmploymentStatus
        {
            get { return employmentStatus.Text; }
            set { employmentStatus.Text = value; }
        }

        public string EmployerAddress
        {
            get { return employerAddress.Text; }
            set { employerAddress.Text = value; }
        }

        public string EmployerCity
        {
            get { return employerCity.Text; }
            set { employerCity.Text = value; }
        }

        public string EmployerState
        {
            get { return employerStatePrivince.Text; }
            set { employerStatePrivince.Text = value; }
        }

        public string EmployerZipCode
        {
            get { return employerZipCode.Text; }
            set { employerZipCode.Text = value; }
        }

        public string EmployerPhone
        {
            get { return employerPhone.Text; }
            set { employerPhone.Text = value; }
        }

        public string EmployerOccupation
        {
            get { return employmentOccIndustry.Text; }
            set { employmentOccIndustry.Text = value; }
        }

        #endregion

        #region Admission Information

        public string UnderWhatName
        {
            get { return underWhatName.Text; }
            set { underWhatName.Text = value; }
        }

        public string AdmittingPhysicianName
        {
            get { return admittingPhysicianName.Text; }
            set { admittingPhysicianName.Text = value; }
        }

        public string TreatingPhysicianName
        {
            get { return treatingPhysicianName.Text; }
            set { treatingPhysicianName.Text = value; }
        }

        public string PrimaryCarePhysicianName
        {
            get { return primaryCarePhysician.Text; }
            set { primaryCarePhysician.Text = value; }
        }

        #endregion

        #region Spouse/guarantor information

        public string GuarantorEmploymentStatus
        {
            get { return guarantorsEmploymentStatus.Text; }
            set { guarantorsEmploymentStatus.Text = value; }
        }

        public string GuarantorPlaceOfEmployment
        {
            get { return guarantorPlaceOfEmployment.Text; }
            set { guarantorPlaceOfEmployment.Text = value; }
        }

        public string GuarantorAddresOfEmployment
        {
            get { return guarantorAddresOfEmployment.Text; }
            set { guarantorAddresOfEmployment.Text = value; }
        }

        public string GuarantorCity
        {
            get { return guarantorCity.Text; }
            set { guarantorCity.Text = value; }
        }

        public string GuarantorStateProvince
        {
            get { return guarantorStateProvince.Text; }
            set { guarantorStateProvince.Text = value; }
        }

        public string GuarantorZipCode
        {
            get { return guarantorZipCode.Text; }
            set { guarantorZipCode.Text = value; }
        }

        public string GuarantorBusinessTelephoneNumber
        {
            get { return guarantorBusinessTelephoneNumber.Text; }
            set { guarantorBusinessTelephoneNumber.Text = value; }
        }

        #endregion

        #region Primary Insurance Information

        public string AreYouInsured
        {
            get { return areYouInsured.Text; }
            set { areYouInsured.Text = value; }
        }

        public string PrimaryInsuranceCompanyName
        {
            get { return primaryInsuranceCompanyName.Text; }
            set { primaryInsuranceCompanyName.Text = value; }
        }

        public string InsuranceCompanyTelephoneNumber
        {
            get { return insuranceCompanyTelephoneNumber.Text; }
            set { insuranceCompanyTelephoneNumber.Text = value; }
        }

        public string InsurancePreCertificationPhoneNumber
        {
            get { return insurancePrecertificationNumber.Text; }
            set { insurancePrecertificationNumber.Text = value; }
        }

        public string SubscribersFirstName
        {
            get { return subscribersFirstName.Text; }
            set { subscribersFirstName.Text = value; }
        }
        
        public string SubscribersLastName
        {
            get { return subscribersLastName.Text; }
            set { subscribersLastName.Text = value; }
        }

        public string SubscribersSsn
        {
            get { return subscribersSSN.Text; }
            set { subscribersSSN.Text = value; }
        }

        public string SubscribersDob
        {
            get { return subscribersDOB.Text; }
            set { subscribersDOB.Text = value; }
        }

        public string PolicyNumber
        {
            get { return policyNumber.Text; }
            set { policyNumber.Text = value; }
        }

        public string PolicyGroupName
        {
            get { return policyGroupName.Text; }
            set { policyGroupName.Text = value; }
        }

        public string PrimaryInsuranceAddress
        {
            get { return primaryInsuranceAddress.Text; }
            set { primaryInsuranceAddress.Text = value; }
        }

        public string PrimaryInsuranceCity
        {
            get { return primaryInsuranceCity.Text; }
            set { primaryInsuranceCity.Text = value; }
        }

        public string PrimaryInsuranceState
        {
            get { return primaryInsuranceState.Text; }
            set { primaryInsuranceState.Text = value; }
        }

        public string PrimaryInsuranceZipCode
        {
            get { return primaryInsuranceZipCode.Text; }
            set { primaryInsuranceZipCode.Text = value; }
        }

        #endregion

        #region secondary insurance information

        public string DoYouHaveSecondaryInsurance
        {
            get { return doYouHaveSecondaryInsurance.Text; }
            set { doYouHaveSecondaryInsurance.Text = value; }
        }

        public string SecondaryInsuranceCompanyName
        {
            get { return secondaryInsuranceCompanyName.Text; }
            set { secondaryInsuranceCompanyName.Text = value; }
        }

        public string SecondaryInsuranceCompanyTelephoneNumber
        {
            get { return secondaryInsuranceCompanyTelephoneNumber.Text; }
            set { secondaryInsuranceCompanyTelephoneNumber.Text = value; }
        }

        public string SecondaryInsurancePreCertificationPhoneNumber
        {
            get { return secondaryInsurancePreCertificationTelephoneNumber.Text; }
            set { secondaryInsurancePreCertificationTelephoneNumber.Text = value; }
        }

        public string SecondarySubscribersFirstName
        {
            get { return secondarySubscribersFirstName.Text; }
            set { secondarySubscribersFirstName.Text = value; }
        }

        public string SecondarySubscribersLastName
        {
            get { return secondarySubscribersLastName.Text; }
            set { secondarySubscribersLastName.Text = value; }
        }

        public string SecondarySSN
        {
            get { return secondarySSN.Text; }
            set { secondarySSN.Text = value; }
        }

        public string SecondarySubscribersDOB
        {
            get { return secondaryDOB.Text; }
            set { secondaryDOB.Text = value; }
        }

        public string SecondaryPolicyNumber
        {
            get { return secondaryPolicyNumber.Text; }
            set { secondaryPolicyNumber.Text = value; }
        }

        public string SecondaryPolicyGroupName
        {
            get { return secondaryPolicyGroupName.Text; }
            set { secondaryPolicyGroupName.Text = value; }
        }

        public string SecondaryInsuranceAddress
        {
            get { return secondaryAddress.Text; }
            set { secondaryAddress.Text = value; }
        }

        public string SecondaryInsuranceCity
        {
            get { return secondaryCity.Text; }
            set { secondaryCity.Text = value; }
        }

        public string SecondaryInsuranceState
        {
            get { return secondaryState.Text; }
            set { secondaryState.Text = value; }
        }

        public string SecondaryInsuranceZipCode
        {
            get { return secondaryZipCode.Text; }
            set { secondaryZipCode.Text = value; }
        }

        #endregion

        #region Method of contact

        public string BestWayToContactYou
        {
            get { return bestWayToContactYou.Text; }
            set { bestWayToContactYou.Text = value; }
        }

        public string BestTimeToContactYou
        {
            get { return bestTimeToContactYou.Text; }
            set { bestTimeToContactYou.Text = value; }
        }

        public string PreferredMethodOfPayment
        {
            get { return preferredMethodOfPayment.Text; }
            set { preferredMethodOfPayment.Text = value; }
        }

        #endregion

        #region Newsletter registration

        public string EmailAddress
        {
            get { return emailAddress.Text; }
            set { emailAddress.Text = value; }
        }

        public string WouldYouLikeToReceiveNewsletter
        {
            get { return wouldYouLikeToReceiveNewsletter.Text; }
            set { wouldYouLikeToReceiveNewsletter.Text = value; }
        }

        public bool AccountClosing { private get; set; }

        #endregion



        #region Data Elements

        #endregion

    }
}