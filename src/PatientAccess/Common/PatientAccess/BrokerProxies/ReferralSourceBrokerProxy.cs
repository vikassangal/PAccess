using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.BrokerProxies
{
    [Serializable]
    public class ReferralSourceBrokerProxy : AbstractBrokerProxy, IReferralSourceBroker
    {
        #region Event Handlers
        #endregion

        #region Methods

        public ICollection AllReferralSources( long facilityID )
        {
            return this.ReferralSourceCollection( facilityID );
        }
             
        public ReferralSource ReferralSourceWith( long facilityID, string referralSource )
        {
            foreach( ReferralSource rc in this.ReferralSourceCollection( facilityID ) )
			{
				if( rc.Code == referralSource )
				{
					return rc;
				}
			}

			return this.i_remoteReferralSourceBroker.ReferralSourceWith( facilityID, referralSource );
        }
  
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
           
        private ICollection ReferralSourceCollection( long facilityID )
        {
            ICollection aList = (ICollection)this.Cache[REFERRAL_SOURCE_COLLECTION];
            if( aList == null )
            {
                lock( REFERRAL_SOURCE_COLLECTION )
                {
                    aList = i_remoteReferralSourceBroker.AllReferralSources( facilityID );
                    if( this.Cache[REFERRAL_SOURCE_COLLECTION] == null )
                    {
                        this.Cache.Insert( REFERRAL_SOURCE_COLLECTION, aList );
                    }
                }
            }
            return aList;
        }

        #endregion

        #region Construction and Finalization
        public ReferralSourceBrokerProxy()
        {
        }
        #endregion

        #region Data Elements
        
			private IReferralSourceBroker i_remoteReferralSourceBroker = BrokerFactory.BrokerOfType< IReferralSourceBroker  >();            

        #endregion

        #region Constants

        private const string  
           
            REFERRAL_SOURCE_COLLECTION = "REFERRAL_SOURCE_COLLECTION";

        #endregion
    }
}
