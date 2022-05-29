using System;
using System.Collections;
using System.Data;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Implements ReferralFacility code related data loading.
    /// </summary>
    [Serializable]
    public class ReferralFacilityPBARBroker : PBARCodesBroker, IReferralFacilityBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        protected override void InitProcNames()
        {
            this.AllStoredProcName = SP_SELECT_REFERRAL_FACILITIES_FOR;
            this.WithStoredProcName = SP_SELECT_REFERRAL_FACILITY_WITH;
        }
        /// <summary>
        /// Get a list of all ReferralFacility objects including code and description.
        /// </summary>
        /// <returns></returns>
        public ICollection ReferralFacilitiesFor( long facilityNumber )
        {
            ICollection facilityReferralFacilities;
            string key = CacheKeys.CACHE_KEY_FOR_REFERRALFACILITIES;
            InitFacility( facilityNumber );

            LoadCacheDelegate LoadData = delegate()
            {
                try
                {
                    facilityReferralFacilities = LoadDataToArrayList<ReferralFacility>( facilityNumber );
                }
                catch( Exception e )
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom( "ReferralFacilityPBARBroker failed to initialize.", e, c_log );
                }
                return facilityReferralFacilities;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                facilityReferralFacilities = cacheManager.GetCollectionBy( key, facilityNumber, LoadData );
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "ReferralFacilityPBARBroker failed to initialize", e, c_log );
            }
            return facilityReferralFacilities;
        }

        /// <summary>
        /// Get one ReferralFacility object based on the facility and code.
        /// </summary>
        /// <param name="facilityNumber"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ReferralFacility ReferralFacilityWith( long facilityNumber, string code )
        {
            if( code == null )
            {
                throw new ArgumentNullException( "ReferralFacility code cannot be null." );
            }
            code = code.Trim().ToUpper();
            ReferralFacility selectedReferralFacility = null;
            InitFacility( facilityNumber );
            try
            {
                ICollection allReferralFacilities = this.ReferralFacilitiesFor( facilityNumber );

                foreach( ReferralFacility referralFacility in allReferralFacilities )
                {
                    if( referralFacility.Code.Equals( code ) )
                    {
                        selectedReferralFacility = referralFacility;
                        break;
                    }
                }
                if( selectedReferralFacility == null )
                {
                    selectedReferralFacility = CodeWith<ReferralFacility>( facilityNumber, code );
                }
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "ReferralFacilityPBARBroker failed to initialize.", e, c_log );
            }
            return selectedReferralFacility;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ReferralFacilityPBARBroker()
            : base()
        {
        }
        public ReferralFacilityPBARBroker( string cxnString )
            : base( cxnString )
        {
        }

        public ReferralFacilityPBARBroker( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( ReferralFacilityPBARBroker ) );
        #endregion

        #region Constants
        private const string
            SP_SELECT_REFERRAL_FACILITIES_FOR = "SELECTALLREFERRALFACILITYFOR",
            SP_SELECT_REFERRAL_FACILITY_WITH = "SELECTALLREFERRALFACILITYWITH";
        #endregion
    }
}
