using System;
using System.Collections;
using System.Data;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
     /// <summary>
    /// Implements ReferralType related data loading.
    /// </summary>
    [Serializable]
    public class ReferralTypePBARBroker : PBARCodesBroker, IReferralTypeBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        protected override void InitProcNames()
        {
            this.AllStoredProcName = SP_SELECT_REFERRALTYPESFOR;
            this.WithStoredProcName = SP_SELECT_REFERRALTYPEWITH;
        }
        /// <summary>
        /// Get a list of all ReferralType objects including code and description.
        /// </summary>
        /// <returns></returns>
        public ICollection ReferralTypesFor( long facilityNumber )
        {
            ICollection facilityReferralTypes = null;
            string key = CacheKeys.CACHE_KEY_FOR_REFERRALTYPES;
            InitFacility( facilityNumber );

            try
            {
                CacheManager cacheManager = new CacheManager();
                
                facilityReferralTypes = cacheManager.GetCollectionBy<ReferralType>( key,
                    facilityNumber, LoadDataToArrayList<ReferralType> );
            }
            catch (Exception e)
            {   
                throw BrokerExceptionFactory.BrokerExceptionFrom("ReferralTypePBARBroker failed to initialize", e, c_log);
            }
            return facilityReferralTypes;
        }

        /// <summary>
        /// Get one ReferralType object based on the facility and code.
        /// </summary>
        /// <param name="facilityNumber"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ReferralType ReferralTypeWith( long facilityNumber, string code )
        {
            ReferralType selectedObj = null;
            if( code == null )
            {
                throw new ArgumentNullException( "code cannot be null or empty" );
            }
            code = code.Trim().ToUpper();
            InitFacility( facilityNumber );
            try
            {
                ArrayList allReferralTypes = (ArrayList)this.ReferralTypesFor(facilityNumber);
                foreach (ReferralType referralType in allReferralTypes)
                {
                    if (referralType.Code.Equals(code))
                    {
                        selectedObj = referralType;
                        break;
                    }
                }

                if (selectedObj == null)
                {
                    selectedObj = CodeWith<ReferralType>( facilityNumber, code );
                }
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "ReferralTypePBARBroker failed to initialize.", e, c_log );
            }
            return selectedObj;
        }     

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ReferralTypePBARBroker()
            : base()
        {
        }
        public ReferralTypePBARBroker( string cxnString )
            : base( cxnString )
        {
        }

        public ReferralTypePBARBroker( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( ReferralTypePBARBroker ) );
        #endregion

        #region Constants
        private const string
            SP_SELECT_REFERRALTYPESFOR = "SELECTALLREFERRALTYPESFOR",
            SP_SELECT_REFERRALTYPEWITH = "SELECTREFERRALTYPEWITH";
        #endregion
    }
}
