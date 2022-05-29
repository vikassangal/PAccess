using System;
using System.Collections;
using System.Data;
using Extensions.DB2Persistence;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for BenefitsCategoryPBARBroker.
	/// </summary>
    //TODO: Create XML summary comment for BenefitsCategoryPBARBroker
    [Serializable]
    public class BenefitsCategoryPBARBroker : PBARCodesBroker, IBenefitsCategoryBroker
    {
        #region Event Handlers
        #endregion

        #region Properties
        #endregion

        #region Methods
        protected override void InitProcNames()
        {
            this.AllStoredProcName = SP_SELECT_ALL_BENEFITSCATEGORIES;
            this.WithStoredProcName = string.Empty;
        }

        /// <summary>
        /// Get one BenefitsCategory based on the OID
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="benefitsCategoryID"></param>
        /// <returns></returns>
        public BenefitsCategory BenefitsCategoryWith( long facilityID, long benefitsCategoryID )
        {
            BenefitsCategory benefitsCategory = new BenefitsCategory();
            ICollection list = this.AllBenefitsCategories( facilityID );

            foreach( BenefitsCategory aBenefitsCategory in list )
            {
                if( aBenefitsCategory.Oid == benefitsCategoryID )
                {
                    benefitsCategory = aBenefitsCategory;
                }
            }
            return benefitsCategory;
        }

        /// <summary>
        /// Get a list of all BenefitsCategories
        /// </summary>
        /// <returns></returns>
        public ICollection AllBenefitsCategories( long facilityID )
        {
            ICollection benefitsCategories = null;
            string key = CacheKeys.CACHE_KEY_FOR_BENEFITS;
            InitFacility( facilityID );
            try
            {
                CacheManager cacheManager = new CacheManager();
                this.AllStoredProcName = SP_SELECT_ALL_BENEFITSCATEGORIES;
                benefitsCategories = cacheManager.GetCollectionBy(
                    key,
                    this.HubName,
                    LoadUncodedDataToArrayList<BenefitsCategory> );
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "BenefitsCategoryPBARBroker failed to initialize", e, c_log );
            }
           return benefitsCategories;
        }

        /// <summary>
        /// Get One BenefitsCategory based on the code.
        /// </summary>
        /// <param name="facility"></param>
        /// <param name="hsvCode"></param>
        /// <returns></returns>
        public ICollection BenefitsCategoriesFor( Facility facility, string hsvCode )
        {
            ArrayList benefitsCategories = new ArrayList();
            InitFacility( facility.Oid );
            SafeReader reader = null;
            iDB2Command cmd = null;
            string key = CacheKeys.CACHE_KEY_FOR_BENEFITS;
            LoadCacheDelegate LoadData = delegate()
           {
            try
            {
                cmd = CommandFor( "CALL " + SP_SELECT_CATEGORIESFOR +
                    "(" + PARAM_SERVICECODE + ")",
                    CommandType.Text,
                    facility );

                cmd.Parameters[PARAM_SERVICECODE].Value = hsvCode;
                reader = this.ExecuteReader( cmd );
                
                while (reader.Read())
                   {
                       benefitsCategories.Add(
                           this.BenefitsCategoryFrom( reader ) );
                   }
               }
               catch (Exception e)
               {
                   throw BrokerExceptionFactory.BrokerExceptionFrom( "BenefitsCategoryPBARBroker failed to initialize", e, c_log );
               }
               finally
               {
                   base.Close( reader );
                   base.Close( cmd );
               }
               return (ICollection)benefitsCategories;
           };
            try
            {
                CacheManager cacheManager = new CacheManager();
                benefitsCategories = (ArrayList)cacheManager.GetCollectionBy(key, hsvCode, LoadData);
            }
            catch (Exception e)
            {   
                throw BrokerExceptionFactory.BrokerExceptionFrom("BenefitsCategory failed to initialize", e, c_log);
            }
            return benefitsCategories;
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// BenefitsCategory From reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private BenefitsCategory BenefitsCategoryFrom( SafeReader reader )
        {
            BenefitsCategory result = null;

            long benefitsCategoryID = reader.GetInt64( COL_BENEFITSCATEGORYID );
            string description = reader.GetString( COL_DESCRIPTION );

            result =
                new BenefitsCategory(
                benefitsCategoryID,
                ReferenceValue.NEW_VERSION,
                description );

            return result;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public BenefitsCategoryPBARBroker()
        {
        }
        public BenefitsCategoryPBARBroker(string cxnString)
            : base(cxnString)
        {
        }
        public BenefitsCategoryPBARBroker(IDbTransaction txn)
            : base(txn)
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log = 
            LogManager.GetLogger( typeof( BenefitsCategoryPBARBroker ));
        Account account = new Account();
        #endregion

        #region Constants
        private const string
            SP_SELECT_ALL_BENEFITSCATEGORIES      = "SELECTALLBENEFITSCATEGORIES",
            SP_SELECT_CATEGORIESFOR              = "SELECTCATEGORIESFOR";
        private const string
            COL_BENEFITSCATEGORYID = "BenefitCategoryID",
            COL_DESCRIPTION = "Description",
            PARAM_SERVICECODE = "@P_CODE";
        #endregion
    }
}
