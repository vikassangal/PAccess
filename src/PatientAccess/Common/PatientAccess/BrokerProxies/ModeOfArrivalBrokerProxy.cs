using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.BrokerProxies
{
	/// <summary>
	/// Summary description for ModeOfArrivalBrokerProxy.
	/// </summary>
	//TODO: Create XML summary comment for ModeOfArrivalBrokerProxy
    [Serializable]
    public class ModeOfArrivalBrokerProxy : AbstractBrokerProxy, IModeOfArrivalBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        public ArrayList ModesOfArrivalFor( long facilityID )
        {
            ArrayList statuses = new ArrayList();
            Hashtable modeOfArrivalByFacilityIDHashtable = this.ModeOfArrivalByFacilityIDHashtable;

            if( ModeOfArrivalByFacilityIDHashtable[facilityID] == null )
            {
                lock( CACHE_MODE_OF_ARRIVAL_BY_FACILITYID_HASHTABLE )
                {
                    // Ensure that we are still null before loading from the real broker
                    if( modeOfArrivalByFacilityIDHashtable[facilityID] == null )
                    {
                        statuses = i_ModeOfArrivalBroker.ModesOfArrivalFor( facilityID );
                        modeOfArrivalByFacilityIDHashtable[facilityID] = statuses;
                    }
                    else
                    {
                        // The ssnStatuses flags were loaded on another thread before our double-check
                        statuses = (ArrayList)modeOfArrivalByFacilityIDHashtable[facilityID];
                    }
                }
            }
            else
            {
                // ssnStatuses for this facility were already loaded.
                statuses = (ArrayList)modeOfArrivalByFacilityIDHashtable[facilityID];
            }
            return statuses;

        }

        public ModeOfArrival ModeOfArrivalWith(long facilityNumber, string code)
        {
            return i_ModeOfArrivalBroker.ModeOfArrivalWith(facilityNumber, code);
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private Hashtable ModeOfArrivalByFacilityIDHashtable
        {
            get
            {
                Hashtable modeOfArrivalByFacilityIDHashtable = null;
                modeOfArrivalByFacilityIDHashtable = (Hashtable)this.Cache[CACHE_MODE_OF_ARRIVAL_BY_FACILITYID_HASHTABLE];
                if( modeOfArrivalByFacilityIDHashtable == null )
                {
                    lock( CACHE_MODE_OF_ARRIVAL_BY_FACILITYID_HASHTABLE )
                    {
                        modeOfArrivalByFacilityIDHashtable = new Hashtable();
                        this.Cache.Insert( CACHE_MODE_OF_ARRIVAL_BY_FACILITYID_HASHTABLE, modeOfArrivalByFacilityIDHashtable );
                    }
                }

                return modeOfArrivalByFacilityIDHashtable;
            }
        }

	    #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ModeOfArrivalBrokerProxy()
        {
        }
        #endregion

        #region Data Elements
        private IModeOfArrivalBroker i_ModeOfArrivalBroker = BrokerFactory.BrokerOfType< IModeOfArrivalBroker >();
        #endregion

        #region Constants
        private const string CACHE_MODE_OF_ARRIVAL_BY_FACILITYID_HASHTABLE = "CACHE_MODE_OF_ARRIVAL_BY_FACILITYID_HASHTABLE";
        #endregion
    }
}
