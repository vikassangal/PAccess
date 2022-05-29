using System;
using System.Collections;
using Extensions.OrganizationalService;
using PatientAccess.Annotations;
using PatientAccess.BrokerInterfaces;
using Peradigm.Framework.Domain.Parties;
using Facility = PatientAccess.Domain.Facility;

namespace PatientAccess.Persistence
{
    [Serializable]
    [UsedImplicitly]
    public class OrganizationalHierarchyProvider : IOrganizationalHierarchyProvider
    {
        #region Event Handlers
        #endregion

        #region Methods
        public  OrganizationalUnit GetOrganizationalHierarchy()
        {
            OrganizationalUnit hierarchyRoot = new OrganizationalUnit(DEFAULT_TOP_LEVEL_CODE, DEFAULT_TOP_LEVEL_NAME, DEFAULT_TOP_LEVEL_TYPE);
                        
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            ArrayList allFacilities = facilityBroker.AllFacilities() as ArrayList;

            foreach( Facility facility in allFacilities)
            {
                AbstractOrganizationalUnit securityFrameworkFacility = 
                    new Peradigm.Framework.Domain.Parties.Facility(facility.Oid, facility.Code, facility.Description);
                
                hierarchyRoot.AddRelationship( new OrganizationalRelationship( hierarchyRoot, securityFrameworkFacility ) );
                securityFrameworkFacility.AddRelationship( new OrganizationalRelationship( hierarchyRoot, securityFrameworkFacility ) );
            }
           
            return hierarchyRoot;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public OrganizationalHierarchyProvider()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        
        private const string
        DEFAULT_TOP_LEVEL_NAME      = "AllPatientAccessFacilities",
        DEFAULT_TOP_LEVEL_CODE      = "ALL",
        DEFAULT_TOP_LEVEL_TYPE      = "ALL";

        #endregion
    }
}
