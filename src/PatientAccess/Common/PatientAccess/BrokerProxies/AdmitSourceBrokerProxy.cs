using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.BrokerProxies
{
    [Serializable]
    public class AdmitSourceBrokerProxy : AbstractBrokerProxy, IAdmitSourceBroker
    {
        #region Event Handlers
        #endregion

        #region Methods

        public ICollection AllTypesOfAdmitSources(long facilityID)
        {
            this.i_facilityID = facilityID;
            return this.AdmitSources;
        }

		// called from PreMSEDiagnosisView

        public AdmitSource AdmitSourceWith(long facilityID, string code)
        {
            this.i_facilityID = facilityID;

            return i_remoteAdmitSourceBroker.AdmitSourceWith(facilityID, code);
        }

        public ICollection AdmitSourcesForNotNewBorn(long facilityID)
        {
            this.i_facilityID = facilityID;

			ArrayList notNewBornAdmitSources = new ArrayList();
			
			foreach( AdmitSource admitSource in this.AdmitSources )
			{
				if( admitSource.Code != AdmitSource.NEWBORNADMITSOURCE )
				{
					notNewBornAdmitSources.Add((AdmitSource)admitSource.Clone());
				}
			}
			
			return notNewBornAdmitSources ;
        }

        public AdmitSource AdmitSourceForNewBorn(long facilityID)
        {
            throw new Exception("This method is not implemented in proxy");
        }
             
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
           
        private ICollection AdmitSources
        {
            get
            {
                var cacheKey = "ADMIT_SOURCE_COLLECTION_AND_FACILITY_" + i_facilityID;
                ICollection i_AdmitSources = (ICollection)this.Cache[cacheKey];
                if( i_AdmitSources == null )
                {
                    i_AdmitSources = i_remoteAdmitSourceBroker.AllTypesOfAdmitSources(i_facilityID);

                    lock (cacheKey)
                    {
                        if (this.Cache[cacheKey] == null)
                        {
                            this.Cache.Insert(cacheKey, i_AdmitSources);
                        }
                    }
                }

                return i_AdmitSources;
            }
        }

        #endregion

        #region Construction and Finalization
        public AdmitSourceBrokerProxy()
        {
        }
        #endregion

        #region Data Elements
       
        private IAdmitSourceBroker i_remoteAdmitSourceBroker = BrokerFactory.BrokerOfType< IAdmitSourceBroker >() ;

        #endregion

        #region Constants

        private const string             
            ADMIT_SOURCE_COLLECTION = "ADMIT_SOURCE_COLLECTION";

        private
            long i_facilityID;
        #endregion
    }
}
