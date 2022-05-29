using System;
using System.Reflection;
using System.Xml.Serialization;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.PreRegistration;
using PatientAccess.Messaging;

namespace PatientAccess.Persistence.OnlinePreregistration
{
    public class SupplementalInformationFactory
    {
        public SupplementalInformationFactory(preRegistration preRegistrationDetails)
        {
            PreRegistrationDetails = preRegistrationDetails;
            addressFactory = new AddressFactory(Facility);
        }

        private preRegistration PreRegistrationDetails { get; set; }

        private Facility Facility
        {
            get
            {
                return facility ??
                       ( facility = facilityBroker.FacilityWith( PreRegistrationDetails.facility.code ) );
            }
        }

        #region Data Elements

        private readonly AddressFactory addressFactory;
       

        private readonly IEmploymentStatusBroker employmentStatusBroker =
            BrokerFactory.BrokerOfType<IEmploymentStatusBroker>();

        private readonly IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();

       
        private Facility facility;

        #endregion

        public SupplementalInformation Build()
        {


            var supplementalInformation = new SupplementalInformation();

            #region Location Information

            if ( PreRegistrationDetails.facility.serviceLocation != null )
            {
                supplementalInformation.ServiceLocation = PreRegistrationDetails.facility.serviceLocation.name;
            }
            else
            {
                supplementalInformation.ServiceLocation = Facility.Description;
            }


            #endregion
            
            #region Patient Information

            supplementalInformation.PatientMaidenName = PreRegistrationDetails.patient.name.maiden;

            #endregion

            #region Employment information
            supplementalInformation.EmployerName = GetEmployerName( PreRegistrationDetails.patient.employment.Item );
            supplementalInformation.EmploymentStatus =
                GetEmploymentStatus(PreRegistrationDetails.patient.employment.Item);
            var employerAddress = GetEmployerAddress(PreRegistrationDetails.patient.employment.Item);
            supplementalInformation.EmployerName = GetEmployerName( PreRegistrationDetails.patient.employment.Item );
            supplementalInformation.EmployerAddress = employerAddress.Address1 + employerAddress.Address2;
            supplementalInformation.EmployerCity = employerAddress.City;
            supplementalInformation.EmployerState = employerAddress.State.Description;
            supplementalInformation.EmployerZipCode = employerAddress.ZipCode.FormattedPostalCodeFor(employerAddress.IsUnitedStatesAddress());
            supplementalInformation.EmployerPhone = PreRegistrationDetails.patient.employment.telephoneNumber;
            supplementalInformation.EmployerOccupation = PreRegistrationDetails.patient.employment.industry;

            #endregion

            #region Admission Information

            supplementalInformation.UnderWhatName = PreRegistrationDetails.visit.previousVisitName;
            supplementalInformation.AdmittingPhysicianName = PreRegistrationDetails.visit.admittingPhysicianName;
            supplementalInformation.TreatingPhysicianName = PreRegistrationDetails.visit.treatingPhysicianName;
            supplementalInformation.PrimaryCarePhysicianName = PreRegistrationDetails.visit.primaryCarePhysician;

            #endregion

            #region Primary Insurance Information


            if ( AreYouInsured() )
            {
                supplementalInformation.AreYouInsured = "Yes";
                insuranceType primaryInsuranceInformation =
                    PreRegistrationDetails.visit.insuranceInformation.primaryInsurance;
                supplementalInformation.PrimaryInsuranceCompanyName = primaryInsuranceInformation.companyName;
                supplementalInformation.InsuranceCompanyTelephoneNumber =
                    primaryInsuranceInformation.phoneNumber;
                supplementalInformation.InsurancePreCertificationPhoneNumber =
                    primaryInsuranceInformation.preCertificationTelephoneNumber;
                supplementalInformation.SubscribersFirstName = primaryInsuranceInformation.subscriberName.first;
                supplementalInformation.SubscribersLastName = primaryInsuranceInformation.subscriberName.last;
                supplementalInformation.SubscribersSSN =
                    primaryInsuranceInformation.subscriberSocialSecurityNumber;
                supplementalInformation.SubscribersDOB =
                    primaryInsuranceInformation.subscriberDateOfBirth.ToString( "MM/dd/yyyy" );
                supplementalInformation.PolicyNumber = primaryInsuranceInformation.policyNumber;
                supplementalInformation.PolicyGroupName = primaryInsuranceInformation.policyGroupName;
                Address primaryAddress =
                    addressFactory.BuildAddress(primaryInsuranceInformation.address);
                supplementalInformation.PrimaryInsuranceAddress = primaryAddress.Address1 +
                                                                  primaryAddress.Address2;
                supplementalInformation.PrimaryInsuranceCity = primaryAddress.City;
                supplementalInformation.PrimaryInsuranceState = primaryAddress.State.Description;
                supplementalInformation.PrimaryInsuranceZipCode =
                    primaryAddress.ZipCode.FormattedPostalCodeFor(primaryAddress.IsUnitedStatesAddress());
            }
            else
            {
                supplementalInformation.AreYouInsured = "No";
            }
            

            #endregion

                #region Secondary Insurance Information

                if ( DoYouHaveSecondaryInsurance() )
                {
                    supplementalInformation.DoYouHaveSecondaryInsurance = "Yes";
                    var secondaryInsuranceInformation =
                        PreRegistrationDetails.visit.insuranceInformation.secondaryInsurance;
                    supplementalInformation.SecondaryInsuranceCompanyName =
                        secondaryInsuranceInformation.companyName;
                    supplementalInformation.SecondaryInsuranceCompanyTelephoneNumber =
                        secondaryInsuranceInformation.phoneNumber;
                    supplementalInformation.SecondaryInsurancePreCertificationPhoneNumber =
                        secondaryInsuranceInformation.preCertificationTelephoneNumber;
                    supplementalInformation.SecondarySubscribersFirstName = secondaryInsuranceInformation.subscriberName.first;
                    supplementalInformation.SecondarySubscribersLastName = secondaryInsuranceInformation.subscriberName.last;
                    supplementalInformation.SecondarySSN =
                        secondaryInsuranceInformation.subscriberSocialSecurityNumber;
                    supplementalInformation.SecondarySubscribersDOB =
                        secondaryInsuranceInformation.subscriberDateOfBirth.ToString( "MM/dd/yyyy" );
                    supplementalInformation.SecondaryPolicyNumber = secondaryInsuranceInformation.policyNumber;
                    supplementalInformation.SecondaryPolicyGroupName =
                        secondaryInsuranceInformation.policyGroupName;
                    Address secondaryAddress = addressFactory.BuildAddress(secondaryInsuranceInformation.address);
                    supplementalInformation.SecondaryInsuranceAddress = secondaryAddress.Address1 +
                                                                        secondaryAddress.Address2;
                    supplementalInformation.SecondaryInsuranceCity = secondaryAddress.City;
                    supplementalInformation.SecondaryInsuranceState = secondaryAddress.State.Description;
                    supplementalInformation.SecondaryInsuranceZipCode = secondaryAddress.ZipCode.FormattedPostalCodeFor( secondaryAddress.IsUnitedStatesAddress() );
                }
                else
                {
                    supplementalInformation.DoYouHaveSecondaryInsurance = "No";
                }
        

            #endregion

                #region Guarantor and Guarantor employment information


                supplementalInformation.GuarantorEmploymentStatus =
                    GetEmploymentStatus(PreRegistrationDetails.patient.guarantor.employment.Item);
                Address guarantorEmploymentAddress = GetEmployerAddress(PreRegistrationDetails.patient.guarantor.employment.Item);
                supplementalInformation.GuarantorPlaceOfEmployment = GetEmployerName( PreRegistrationDetails.patient.guarantor.employment.Item );
                supplementalInformation.GuarantorAddresOfEmployment = guarantorEmploymentAddress.Address1 +
                                                                      guarantorEmploymentAddress.Address2;
                supplementalInformation.GuarantorCity = guarantorEmploymentAddress.City;
                supplementalInformation.GuarantorStateProvince = guarantorEmploymentAddress.State.Description;
                supplementalInformation.GuarantorZipCode = guarantorEmploymentAddress.ZipCode.FormattedPostalCodeFor( guarantorEmploymentAddress.IsUnitedStatesAddress() );
            supplementalInformation.GuarantorBusinessTelephoneNumber =
                    PreRegistrationDetails.patient.guarantor.businessPhoneNumber ;

                #endregion



                #region Method of Contact information

                supplementalInformation.BestWayToContactYou = string.Empty;
                supplementalInformation.BestTimeToContactYou = string.Empty;
                supplementalInformation.PreferredMethodOfPayment = string.Empty;

                if (PreRegistrationDetails.visit.bestWayToContactSpecified)
                {
                    supplementalInformation.BestWayToContactYou =
                        PreRegistrationDetails.visit.bestWayToContact.ToString();
                }
                if (PreRegistrationDetails.visit.bestTimeToContactSpecified)
                {
                    supplementalInformation.BestTimeToContactYou =
                        PreRegistrationDetails.visit.bestTimeToContact.ToString();
                }
                if (PreRegistrationDetails.visit.preferredMethodOfPaymentSpecified)
                {
                    supplementalInformation.PreferredMethodOfPayment =
                        ConvertToString(PreRegistrationDetails.visit.preferredMethodOfPayment);
                }

                #endregion

                #region Newsletter Registration information

                supplementalInformation.WouldYouLikeToReceiveNewsletter =
                    PreRegistrationDetails.visit.newsletterRegistration.receiveNewsletter
                        ? "Yes"
                        : "No";
            supplementalInformation.EmailAddress = PreRegistrationDetails.visit.newsletterRegistration.emailAddress;
                #endregion

                return supplementalInformation;
            }


        private bool AreYouInsured()
        {
            if ( ( PreRegistrationDetails.visit.insuranceInformation != null ) &&
                ( PreRegistrationDetails.visit.insuranceInformation.primaryInsurance != null )
                 
                )
            {
                return true;
            }
            return false;
        }
        private bool DoYouHaveSecondaryInsurance()
        {
            if ( ( PreRegistrationDetails.visit.insuranceInformation != null ) &&
                ( PreRegistrationDetails.visit.insuranceInformation.secondaryInsurance != null )
                )
            {
                return true;
            }
            return false;
        }
        private string GetEmploymentStatus( object inputEmploymentType )
        {
            string employmentStatus = string.Empty;
            if ( inputEmploymentType is fullTimePartTimeMilitaryOrOtherEmploymentType )
            {
                var employmentType =
                    (fullTimePartTimeMilitaryOrOtherEmploymentType) inputEmploymentType;
                var status = employmentType.status;
                employmentStatus = PatientFactory.EnumToCode(status);
            }
            else if ( inputEmploymentType is retiredNotEmployedOrSelfEmployedStatusType )
            {
                var employmentType =
                    (retiredNotEmployedOrSelfEmployedStatusType)inputEmploymentType;

                var status = employmentType;
                employmentStatus = PatientFactory.EnumToCode( status );
            }
            var es = employmentStatusBroker.EmploymentStatusWith( Facility.Oid, employmentStatus );

            return es.Description;
        }
        private static string GetEmployerName( object inputEmploymentType )
        {
            var employerName = string.Empty;
          
            if ( inputEmploymentType is fullTimePartTimeMilitaryOrOtherEmploymentType )
            {
                var employmentType =
                    (fullTimePartTimeMilitaryOrOtherEmploymentType) inputEmploymentType;
                 employerName = employmentType.name;
            }

            return employerName;
        }
        private Address GetEmployerAddress( object inputEmploymentType )
        {
            var employerAddress = new Address();
            addressType employerAddressType = null;
            
            if ( inputEmploymentType is fullTimePartTimeMilitaryOrOtherEmploymentType )
            {
                var employmentType =
                    (fullTimePartTimeMilitaryOrOtherEmploymentType)inputEmploymentType;
                employerAddressType   = employmentType.address;
            }
            if ( employerAddressType != null )
            {
                employerAddress = addressFactory.BuildAddress(employerAddressType);
            }
            return employerAddress;
        }

        private static string ConvertToString( Enum e )
        {
            // Get the Type of the enum
            Type t = e.GetType();

            // Get the FieldInfo for the member field with the enums name
            FieldInfo info = t.GetField( e.ToString( "G" ) );

            // Check to see if the XmlEnumAttribute is defined on this field
            if ( !info.IsDefined( typeof( XmlEnumAttribute ), false ) )
            {
                // If no XmlEnumAttribute then return the string version of the enum.
                return e.ToString( "G" );
            }

            // Get the XmlEnumAttribute
            object[] o = info.GetCustomAttributes( typeof( XmlEnumAttribute ), false );
            var attribute = (XmlEnumAttribute)o[0];
            return attribute.Name;
        }
     
    }
}