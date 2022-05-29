using System.Collections;
using PatientAccess.Domain.Parties;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Interface lists the RelationshipTypeBroker Broker related methods.
    /// </summary>
    public interface IRelationshipTypeBroker
    {
        //Get all Relationship types for a facility.
        ICollection AllTypesOfRelationships( long facilityID );
        //Get all Relationship types for a facility and type of role
        ICollection AllTypesOfRelationships(long facilityID, TypeOfRole typeOfRole );
        //Get the Relationship type for a facility and oid
        RelationshipType RelationshipTypeWith(long facilityID, long oid );
        RelationshipType RelationshipTypeWith(long facilityID, string code);
       
    }
}
