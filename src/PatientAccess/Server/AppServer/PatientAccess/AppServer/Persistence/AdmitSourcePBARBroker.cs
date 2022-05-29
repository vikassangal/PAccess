using System;
using System.Collections;
using System.Data;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Implements Demographics related methods.
    /// </summary>
    [Serializable]
    public class AdmitSourcePBARBroker : PBARCodesBroker, IAdmitSourceBroker
    {
        #region Event Handlers
        #endregion

        #region Methods

        protected override void InitProcNames()
        {
            this.AllStoredProcName = SP_SELECTALLADMITSOURCES;
            this.WithStoredProcName = SP_SELECTADMITSOURCEWITH;
        }
        /// <summary>
        /// Get a list of all Gender objects including oid, code and description.
        /// </summary>
        public ICollection AllTypesOfAdmitSources(long facilityID)
        {
            ICollection typesOfAdmitSources = null;
            var key = CacheKeys.CACHE_KEY_FOR_ADMITSOURCES;
            this.InitFacility(facilityID);
            LoadCacheDelegate loadData = delegate()
            {
                try
                {
                    typesOfAdmitSources = LoadDataToArrayList<AdmitSource>(facilityID);
                }
                catch (Exception e)
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom("AdmitSourceBroker failed to initialize", e, c_log);
                }
                return typesOfAdmitSources;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                this.AllStoredProcName = SP_SELECTALLADMITSOURCES;
                typesOfAdmitSources = cacheManager.GetCollectionBy(key, facilityID,
                    loadData);
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("AdmitSourceBroker failed to initialize", e, c_log);
            }
            return typesOfAdmitSources;
        }
    
        public AdmitSource AdmitSourceForNewBorn(long facilityID)
        {
            AdmitSource newBornAdmitSource  = new AdmitSource();

            newBornAdmitSource = this.AdmitSourceWith(facilityID, "L");
            
            return newBornAdmitSource ;
        }

        public ICollection AdmitSourcesForNotNewBorn(long facilityID)
        {
            ArrayList notNewBornAdmitSources = new ArrayList();
            ArrayList allAdmitSources = (ArrayList)this.AllTypesOfAdmitSources(facilityID);
            foreach( AdmitSource admitSource in allAdmitSources )
            {
                if( admitSource.Code != AdmitSource.NEWBORNADMITSOURCE )
                {
                    notNewBornAdmitSources.Add(admitSource.Clone());
                }
            }
            return notNewBornAdmitSources ;
        }

        public AdmitSource AdmitSourceWith(long facilityID, string code)
        {
            AdmitSource selectedAdmitSource  = null;

            if ( null == code )
            {
                throw new ArgumentNullException( "Parameter, code, cannot be null." ) ;
            }
            code = code.Trim();

            try
            {
                ArrayList allTypesOfAdmitSources = (ArrayList)this.AllTypesOfAdmitSources(facilityID);
                foreach (AdmitSource admitSource in allTypesOfAdmitSources)
                {
                    if (admitSource.Code.Equals(code))
                    {
                        selectedAdmitSource = admitSource;
                        break;
                    }
                }

                // didn't find it in the list see if it is in the database
                if (selectedAdmitSource == null)
                {
                    selectedAdmitSource = CodeWith<AdmitSource>(facilityID, code );
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                throw BrokerExceptionFactory.BrokerExceptionFrom("AdmitSourceBroker failed to retrieve object", e, c_log);
            }
            return selectedAdmitSource;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AdmitSourcePBARBroker()
            : base()
        {
        }
        public AdmitSourcePBARBroker( string cxnString )
            : base( cxnString )
        {
        }

        public AdmitSourcePBARBroker( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements

        private static readonly ILog c_log = 
            LogManager.GetLogger( typeof( AdmitSourcePBARBroker ) );
        #endregion

        #region Constants
        private const string 
            SP_SELECTALLADMITSOURCES =
                        "SelectAllAdmitSources",
            SP_SELECTADMITSOURCEWITH =
                        "SelectAdmitSourceWith";

        #endregion
    }
}
