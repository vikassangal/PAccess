using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Messaging;

namespace PatientAccess.Persistence.OnlinePreregistration
{
    public class ContactPointFactory
    {
        public ContactPoint BuildContactPoint( emergencyContactType emergencyContactType, TypeOfContactPoint typeOfContact )
        {
            ContactPoint contactPoint = new ContactPoint
            {
                Address = ( emergencyContactType.address != null ) ? AddressFactory.BuildAddress( emergencyContactType.address ) : new Address(),
                PhoneNumber = new PhoneNumber( emergencyContactType.phoneNumber ),
                TypeOfContactPoint = typeOfContact
            };

            return contactPoint;
        }

        public ContactPoint BuildContactPoint( patientType patientType, TypeOfContactPoint typeOfContact )
        {
            ContactPoint contactPoint = new ContactPoint
            {
                Address = ( patientType.address != null ) ? AddressFactory.BuildAddress( patientType.address ) : new Address(),
                PhoneNumber = ( patientType.phoneNumber != null ) ? new PhoneNumber( patientType.phoneNumber ) : new PhoneNumber(),
                EmailAddress = ( patientType.emailAddress != null ) ? new EmailAddress( patientType.emailAddress ) : new EmailAddress(),
                TypeOfContactPoint = typeOfContact
            };

            return contactPoint;
        }

        public ContactPoint BuildContactPoint( guarantorType guarantorType, TypeOfContactPoint typeOfContact )
        {
            ContactPoint contactPoint = new ContactPoint
            {
                Address = ( guarantorType.address != null ) ? AddressFactory.BuildAddress( guarantorType.address ) : new Address(),
                PhoneNumber = new PhoneNumber( guarantorType.phoneNumber ),
                TypeOfContactPoint = typeOfContact
            };

            return contactPoint;
        }

        public ContactPoint BuildMobileContactPoint( patientType patientType )
        {
            ContactPoint contactPoint = new ContactPoint
            {
                PhoneNumber = ( patientType.cellPhoneNumber != null ) ? new PhoneNumber( patientType.cellPhoneNumber ) : new PhoneNumber(),
                TypeOfContactPoint = TypeOfContactPoint.NewMobileContactPointType()
            };

            return contactPoint;
        }

        #region Properties

        private AddressFactory AddressFactory
        {
            get { return addressFactory; }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ContactPointFactory( Facility facility )
        {
            i_Facility = facility;
            addressFactory = new AddressFactory( i_Facility );
        }

        #endregion

        #region Data Elements

        private readonly Facility i_Facility;
        private readonly AddressFactory addressFactory;

        #endregion
    }
}
