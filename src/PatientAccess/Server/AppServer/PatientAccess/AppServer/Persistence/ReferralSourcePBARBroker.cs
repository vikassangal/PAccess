using System;
using System.Collections;
using System.Data;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Implements ReferralSource related methods.
    /// </summary>
    [Serializable]
    public class ReferralSourcePBARBroker : PBARCodesBroker, IReferralSourceBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        protected override void InitProcNames()
        {
            this.AllStoredProcName = SP_SELECT_ALL_REFERRAL_SOURCES;
            this.WithStoredProcName = SP_SELECT_REFERRAL_SOURCE_WITH;
        }

        /// <summary>
        /// Get a list of ReferralSource objects including oid, code and description.
        /// </summary>
        public ICollection AllReferralSources( long facilityID )
        {
            ICollection allReferralSources = null;
            string key = CacheKeys.CACHE_KEY_FOR_REFERRALSOURCES;
            this.InitFacility( facilityID );

            try
            {
                CacheManager cacheManager = new CacheManager();
                allReferralSources = cacheManager.GetCollectionBy<ReferralSource>(key, facilityID, 
                    LoadDataToArrayList<ReferralSource> );
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "ReferralSourcePBARBroker failed to initialize", e, c_log );
            }
            return allReferralSources;
        }

        /// <summary>
        /// Get one ReferralSource object based on the facility and code.
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ReferralSource ReferralSourceWith( long facilityID, string code )
        {
            if( code == null )
            {
                throw new ArgumentNullException( "ReferralSource code cannot be null." );
            }
            code = code.Trim().ToUpper();
            // PatientInsertStrategy will put code 0 for blank referral source in db2, since the column is numeric 
            if (code == "0")
            {
                code = string.Empty;
            }

            ReferralSource retReferralSource = null;
            this.InitFacility( facilityID );

            try
            {
                ICollection allReferralSources = this.AllReferralSources( facilityID );
                foreach( ReferralSource referralSource in allReferralSources )
                {
                    if( referralSource.Code.Equals( code ) )
                    {
                        retReferralSource = referralSource;
                        break;
                    }
                }
                if( retReferralSource == null )
                {
                    retReferralSource = CodeWith<ReferralSource>( code );
                }
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "ReferralSourcePBARBroker failed to initialize.", e, c_log );
            }
            return retReferralSource;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ReferralSourcePBARBroker()
            : base()
        {
        }
        public ReferralSourcePBARBroker( string cxnString )
            : base( cxnString )
        {
        }

        public ReferralSourcePBARBroker( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( ReferralSourcePBARBroker ) );
        #endregion

        #region Constants
        private const string
            SP_SELECT_ALL_REFERRAL_SOURCES = "SELECTALLREFERRALSOURCES",
            SP_SELECT_REFERRAL_SOURCE_WITH = "SELECTREFERRALSOURCEWITH";
        #endregion
    }
}
