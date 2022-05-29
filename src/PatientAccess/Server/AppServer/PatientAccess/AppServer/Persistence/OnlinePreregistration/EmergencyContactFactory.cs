using System;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Messaging;

namespace PatientAccess.Persistence.OnlinePreregistration
{
    public class EmergencyContactFactory
    {

        #region Public Methods
        public EmergencyContact BuildEmergencyContact( emergencyContactType emergencyContactType, Type patientType )
        {
            EmergencyContact emergencyContact = new EmergencyContact { Name = emergencyContactType.name };

            string relationshipCode = PatientFactory.EnumToCode( emergencyContactType.relationship );
            RelationshipType aRelationshipType = relationshipTypeBroker.RelationshipTypeWith( Facility.Oid, relationshipCode );

            if ( aRelationshipType == null ) return emergencyContact;

            Relationship aRelationship = new Relationship( aRelationshipType, patientType, emergencyContact.GetType() );

            emergencyContact.RemoveRelationship( aRelationshipType );
            emergencyContact.AddRelationship( aRelationship );
            emergencyContact.RelationshipType = aRelationshipType;

            ContactPoint contactPoint =
                ContactPointFactory.BuildContactPoint( emergencyContactType, TypeOfContactPoint.NewPhysicalContactPointType() );

            emergencyContact.AddContactPoint( contactPoint );

            return emergencyContact;
        }
        #endregion

        #region Properties

        private ContactPointFactory ContactPointFactory
        {
            get { return contactPointFactory; }
        }

        private Facility Facility
        {
            get { return i_Facility; }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public EmergencyContactFactory( Facility facility )
        {
            i_Facility = facility;
            contactPointFactory = new ContactPointFactory( Facility );
        }

        #endregion

        #region Data Elements

        private readonly Facility i_Facility;
        private readonly ContactPointFactory contactPointFactory;
        private readonly IRelationshipTypeBroker relationshipTypeBroker = BrokerFactory.BrokerOfType<IRelationshipTypeBroker>();

        #endregion
    }
}
