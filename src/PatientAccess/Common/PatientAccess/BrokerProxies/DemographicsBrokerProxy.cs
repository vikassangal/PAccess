using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.BrokerProxies
{
	/// <summary>
	/// Summary description for DemographicsBrokerProxy.
	/// </summary>
    //TODO: Create XML summary comment for DemographicsBrokerProxy
    [Serializable]
    public class DemographicsBrokerProxy : AbstractBrokerProxy, IDemographicsBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        public ICollection AllTypesOfGenders( long facilityID )
        {
            var cacheKey = "DEMOGRAPHICS_BROKER_ALL_TYPE_OF_GENDERS_AND_FACILITY_" + facilityID;
            ICollection allTypesOfGenders =
                (ICollection)this.Cache[cacheKey];

            if( allTypesOfGenders == null )
            {
                lock (cacheKey)
                {
                    allTypesOfGenders = i_DemographicsBroker.AllTypesOfGenders( facilityID );
                    if (this.Cache[cacheKey] == null)
                    {
                        this.Cache.Insert(cacheKey, allTypesOfGenders );
                    }
                }
            }
            
            return allTypesOfGenders;        
        }


        public ICollection AllMaritalStatuses( long facilityID )
        {
            var cacheKey = "DEMOGRAPHICS_BROKER_ALL_MARITAL_STATUS_AND_FACILITY_" + facilityID;
            ICollection allMaritalStatuses = (ICollection)this.Cache[cacheKey];

            if( allMaritalStatuses == null )
            {
                lock (cacheKey)
                {
                    allMaritalStatuses = i_DemographicsBroker.AllMaritalStatuses( facilityID );
                    if (this.Cache[cacheKey] == null)
                    {
                        this.Cache.Insert(cacheKey, allMaritalStatuses );
                    }
                }
            }
            
            return allMaritalStatuses;
        }

        public ICollection AllLanguages( long facilityID )
        {
            var cacheKey = "DEMOGRAPHICS_BROKER_ALL_LANGUAGES_AND_FACILITY_" + facilityID;
            ICollection allLanguages = (ICollection)this.Cache[cacheKey];

            if( allLanguages == null )
            {
                lock (cacheKey)
                {
                    allLanguages = i_DemographicsBroker.AllLanguages( facilityID );
                    if (this.Cache[cacheKey] == null)
                    {
                        this.Cache.Insert(cacheKey, allLanguages );
                    }
                }
            }
            
            return allLanguages;     
        }

        public Gender GenderWith( long facilityID, string code )
        {
            return i_DemographicsBroker.GenderWith( facilityID, code );
        }

        public MaritalStatus MaritalStatusWith(long facilityID, string code)
        {
            throw new Exception("This method not implemented in proxy");
        }

        public Language LanguageWith(long facilityID, string code)
        {
            throw new Exception("This method not implemented in proxy");
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public DemographicsBrokerProxy()
        {
        }
        #endregion

        #region Data Elements
        private IDemographicsBroker i_DemographicsBroker = BrokerFactory.BrokerOfType< IDemographicsBroker >() ;
        #endregion

        #region Constants
       
        #endregion
    }
}
