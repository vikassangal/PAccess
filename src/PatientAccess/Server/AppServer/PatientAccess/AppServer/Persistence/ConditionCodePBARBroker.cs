using System;
using System.Collections;
using System.Data;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Implements ConditionCode related methods.
    /// </summary>
    [Serializable]
    public class ConditionCodePBARBroker : PBARCodesBroker, IConditionCodeBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        protected override void InitProcNames()
        {
            this.AllStoredProcName = SP_GETSELECTABLECONDITIONCODES;
            this.WithStoredProcName = SP_GETCONDITIONCODEWITH;
        }

        /// <summary>
        /// Get a list of ConditionCode objects including oid, code and description.
        /// </summary>
        public IList AllSelectableConditionCodes(long facilityID)
        {
            ICollection allConditionCodes = null;
            var key = CacheKeys.CACHE_KEY_FOR_CONDITIONCODES;
            this.InitFacility(facilityID);
            LoadCacheDelegate loadData = delegate()
            {
                try
                {
                    allConditionCodes = LoadDataToArrayList<ConditionCode>(facilityID);
                }
                catch (Exception e)
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom("ConditionCodeBroker failed to initialize", e,
                        c_log);
                }

                return (IList) allConditionCodes;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                this.AllStoredProcName = SP_GETSELECTABLECONDITIONCODES;
                allConditionCodes = cacheManager.GetCollectionBy(key, facilityID,
                    loadData);
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("ConditionCodeBroker failed to initialize", e, c_log);
            }

            return (IList) allConditionCodes;
        }

        public ConditionCode ConditionCodeWith(long facilityID, string code)
        {
            ConditionCode selectedConditionCode = null;
            if (code == null)
            {
                throw new ArgumentNullException("code cannot be null or empty");
            }
            code = code.Trim().ToUpper();
            this.InitFacility(facilityID);
            try
            { 
                ICollection allConditionCodes = this.AllSelectableConditionCodes(facilityID);
                foreach (ConditionCode conditionCode in allConditionCodes)
                {
                    if (conditionCode.Code.Equals(code))
                    {
                        selectedConditionCode = conditionCode;
                        break;
                    }
                }

                if (selectedConditionCode == null)
                {
                    this.WithStoredProcName = SP_GETCONDITIONCODEWITH;
                    selectedConditionCode = CodeWith<ConditionCode>(facilityID, code);
                }
            }
            catch (Exception e)
            {
                string msg = "ConditionCodeBroker failed to retrieve object";
                throw BrokerExceptionFactory.BrokerExceptionFrom(msg, e, c_log);
            }
            return selectedConditionCode;
        }       
        public ConditionCode ConditionCodeWith( long facilityID, long id )
        {
            throw new BrokerException("This method not implemented in DB2 Version");
        }
        
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ConditionCodePBARBroker()
            : base()
        {
        }
        public ConditionCodePBARBroker( string cxnString )
            : base( cxnString )
        {
        }

        public ConditionCodePBARBroker( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements

        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( ConditionCodePBARBroker ) );
        #endregion

        #region Constants
        private const string
            SP_GETSELECTABLECONDITIONCODES = "GetSelectableConditionCodes",
            SP_GETCONDITIONCODEWITH = "GetConditionCodeWith";

        #endregion
    }
}
