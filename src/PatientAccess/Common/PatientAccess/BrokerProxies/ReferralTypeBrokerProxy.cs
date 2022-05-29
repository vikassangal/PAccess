using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.BrokerProxies
{
	//TODO: Create XML summary comment for ReferralTypeBrokerProxy
    [Serializable]
    public class ReferralTypeBrokerProxy : AbstractBrokerProxy, IReferralTypeBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        public ICollection ReferralTypesFor( long facilityNumber )
        {
            Hashtable referralTypesByFacility = this.ReferralTypesByFacilityHashtable;
            ICollection referralTypes = null;

            if( referralTypesByFacility[facilityNumber] == null )
            {
                lock( CACHE_REFERRAL_TYPE_BY_FACILITY_ID_HASHTABLE )
                {
                    // Ensure that we are still null before loading from the real broker
                    if( referralTypesByFacility[facilityNumber] == null )
                    {
                        referralTypes = i_referralTypebroker.ReferralTypesFor( facilityNumber );
                        referralTypesByFacility[facilityNumber] = referralTypes;
                    }
                    else
                    {
                        // The Referral types were loaded on another thread before our double-check
                        referralTypes = (ICollection)referralTypesByFacility[facilityNumber];
                    }
                }
            }
            else
            {
                // Referral types for this facility were already loaded.
                referralTypes = (ICollection)referralTypesByFacility[facilityNumber];
            }

            return referralTypes;
        }
        public ReferralType ReferralTypeWith(long facilityNumber, string code)
        {
            return i_referralTypebroker.ReferralTypeWith(facilityNumber, code);
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private Hashtable ReferralTypesByFacilityHashtable
        {
            get
            {
                Hashtable ht = (Hashtable)this.Cache[CACHE_REFERRAL_TYPE_BY_FACILITY_ID_HASHTABLE];
                if( ht == null )
                {
                    lock( CACHE_REFERRAL_TYPE_BY_FACILITY_ID_HASHTABLE )
                    {
                        ht = new Hashtable();
                        if( this.Cache[CACHE_REFERRAL_TYPE_BY_FACILITY_ID_HASHTABLE] == null )
                        {
                            this.Cache.Insert( CACHE_REFERRAL_TYPE_BY_FACILITY_ID_HASHTABLE, ht );
                        }
                    }
                }

                return ht;
            }
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ReferralTypeBrokerProxy()
        {
        }
        #endregion

        #region Data Elements
        private IReferralTypeBroker i_referralTypebroker = BrokerFactory.BrokerOfType< IReferralTypeBroker >();
        #endregion

        #region Constants
        private const string CACHE_REFERRAL_TYPE_BY_FACILITY_ID_HASHTABLE = "CACHE_REFERRAL_TYPE_BY_FACILITY_ID_HASHTABLE";
        #endregion
    }
}
