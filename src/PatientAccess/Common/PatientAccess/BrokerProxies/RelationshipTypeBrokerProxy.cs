using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain.Parties;

namespace PatientAccess.BrokerProxies
{

    public class RelationshipTypeBrokerProxy : AbstractBrokerProxy, IRelationshipTypeBroker
    {

        #region Event Handlers
        #endregion


        #region IRelationshipTypeBroker Members

        public ICollection AllTypesOfRelationships( long facilityID )
        {
            var cacheKey = "RELATIONSHIP_TYPE_BROKER_PROXY_ALL_TYPES_OF_RELATIONSHIP_SINGLE_FACILITY_" + "_AND_FACILITY_" + facilityID;
            ICollection allTypesOfRel = (ICollection)this.Cache[cacheKey];

            if ( null == allTypesOfRel )
            {
                lock (cacheKey)
                {
                    allTypesOfRel = this.i_RelationshipTypeBroker.AllTypesOfRelationships( facilityID ) ;
                    if (null == this.Cache[cacheKey])
                    {
                        this.Cache.Insert(cacheKey, allTypesOfRel);
                    }
                }
            }
            
            return allTypesOfRel ;
        }

        public ICollection AllTypesOfRelationships( long facilityID, TypeOfRole typeOfRole )
        {
            string roleType = typeOfRole.ToString().ToUpper() ;
            string relationshipTypeCacheKey = "RELATIONSHIP_TYPE_BROKER_PROXY_ALL_TYPES_OF_RELATIONSHIP_" + roleType ;
            ICollection allTypesOfRel = (ICollection)this.Cache[ relationshipTypeCacheKey ] ;

            if ( null == allTypesOfRel )
            {
                lock ( relationshipTypeCacheKey )
                {
                    allTypesOfRel = this.i_RelationshipTypeBroker.AllTypesOfRelationships( facilityID, typeOfRole ) ;
                    if ( null == this.Cache[ relationshipTypeCacheKey ] ) 
                    {
                        this.Cache.Insert( relationshipTypeCacheKey, allTypesOfRel ) ;
                    }
                }
            }

            return allTypesOfRel ;
        }

        public RelationshipType RelationshipTypeWith( long facilityID, long oid )
        {
            return this.i_RelationshipTypeBroker.RelationshipTypeWith( facilityID, oid ) ;
        }

        public RelationshipType RelationshipTypeWith( long facilityID, string code )
        {
            return this.i_RelationshipTypeBroker.RelationshipTypeWith( facilityID, code ) ;
        }

        #endregion


        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties

        public RelationshipTypeBrokerProxy()
        {
        }

        #endregion

        #region Construction and Finalization
        #endregion

        #region Data Elements
        private IRelationshipTypeBroker i_RelationshipTypeBroker = BrokerFactory.BrokerOfType< IRelationshipTypeBroker >() ;
        #endregion

    

    }

}
