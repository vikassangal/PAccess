using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.BrokerProxies
{
    [Serializable]
    public class FacilityFlagBrokerProxy : AbstractBrokerProxy, IFacilityFlagBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        public IList FacilityFlagsFor( long facilityId )
        {
            Hashtable facilityFlagsByFacility = this.FacilityFlagsByFacilityHashtable;
            IList facilityFlags = null;

            if( facilityFlagsByFacility[facilityId] == null )
            {
                lock( CACHE_FACILITY_FLAGS_BY_FACILITY_HASHTABLE )
                {
                    // Ensure that we are still null before loading from the real broker
                    if( facilityFlagsByFacility[facilityId] == null )
                    {
                        facilityFlags = i_remoteFacilityFlagBroker.FacilityFlagsFor( facilityId );
                        facilityFlagsByFacility[facilityId] = facilityFlags;
                    }
                    else
                    {
                        // The facility flags were loaded on another thread before our double-check
                        facilityFlags = (IList)facilityFlagsByFacility[facilityId];
                    }
                }
            }
            else
            {
                // FacilityFlags for this facility were already loaded.
                facilityFlags = (IList)facilityFlagsByFacility[facilityId];
            }

            return facilityFlags;
        }

		// CodeReview: this method should be changed to use the cached collection

        public FacilityDeterminedFlag FacilityFlagWith( long facilityId, string code )
        {
            return i_remoteFacilityFlagBroker.FacilityFlagWith( facilityId, code );
        }

		// this method should be changed to use the cached collection

        public FacilityDeterminedFlag FacilityFlagWith( long facilityId, long facilityFlagID )
        {
            return i_remoteFacilityFlagBroker.FacilityFlagWith( facilityId, facilityFlagID );
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private Hashtable FacilityFlagsByFacilityHashtable
        { 
            get
            {
                Hashtable facilityFlagsByFacilityHashtable = null;
                facilityFlagsByFacilityHashtable = (Hashtable)Cache[CACHE_FACILITY_FLAGS_BY_FACILITY_HASHTABLE];
                if( facilityFlagsByFacilityHashtable == null )
                {
                    lock ( CACHE_FACILITY_FLAGS_BY_FACILITY_HASHTABLE )
                    {
                        facilityFlagsByFacilityHashtable = new Hashtable();
                        this.Cache.Insert( CACHE_FACILITY_FLAGS_BY_FACILITY_HASHTABLE, facilityFlagsByFacilityHashtable );
                    }
                }
                return facilityFlagsByFacilityHashtable;
            }
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public FacilityFlagBrokerProxy()
        {
        }
        #endregion

        #region Data Elements
        private IFacilityFlagBroker i_remoteFacilityFlagBroker = BrokerFactory.BrokerOfType< IFacilityFlagBroker >() ;
        #endregion

        #region Constants
        private const string 
            CACHE_FACILITY_FLAGS_BY_FACILITY_ID = "CACHE_FACILITY_FLAGS_BY_FACILITY_ID", 
            CACHE_FACILITY_FLAGS_BY_FACILITY_HASHTABLE = "CACHE_FACILITY_FLAGS_BY_FACILITY_HASHTABLE";
        #endregion
    }
}
