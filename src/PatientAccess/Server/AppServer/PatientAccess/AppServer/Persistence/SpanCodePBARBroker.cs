using System;
using System.Collections;
using System.Data;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    public class SpanCodePBARBroker : PBARCodesBroker, ISpanCodeBroker
    {
        #region Events
        #endregion

        #region Properties
        #endregion

        #region Public Methods		
        /// <summary>
        /// Caches the complete list of SpanCode obj
        /// </summary>
        /// <returns></returns>
        protected override void InitProcNames()
        {
            this.AllStoredProcName = SP_GET_ALL_SPANCODES;
            this.WithStoredProcName = SP_GET_SPAN_CODE_WITH;
        }

        /// <summary>
        /// Get All span codes
        /// </summary>
        /// <param name="facilityID"></param>
        /// <returns></returns>
        
        public ICollection AllSpans(long facilityID)
        {
            ICollection allSpanCodes = null;
            var key = CacheKeys.CACHE_KEY_FOR_SPANCODES;
            this.InitFacility(facilityID);
            LoadCacheDelegate loadData = delegate()
            {
                try
                {
                    allSpanCodes = LoadDataToArrayList<SpanCode>(facilityID);
                }
                catch (Exception e)
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom("SpanCodePBARBroker failed to initialize", e, c_log);
                }
                return allSpanCodes;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                this.AllStoredProcName = SP_GET_ALL_SPANCODES;
                allSpanCodes = cacheManager.GetCollectionBy(key, facilityID,
                    loadData);
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("SpanCodePBARBroker failed to initialize", e, c_log);
            }
            return allSpanCodes;
        }

        /// <summary>
        /// Get one SpanCode object based on the code.
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public SpanCode SpanCodeWith(long facilityID, string code )
        { 
            SpanCode selectedSpanCode = null;
            if( code == null )
            {
                throw new ArgumentNullException( "code cannot be null or empty" );
            }
            code = code.Trim().ToUpper();
            this.InitFacility( facilityID );
            try
            {
                ICollection allSpanCodes = this.AllSpans( facilityID );
                foreach( SpanCode spanCode in allSpanCodes )
                {
                    if( spanCode.Code.Equals( code ) )
                    {
                        selectedSpanCode = spanCode;
                        break;
                    }
                }
                if( selectedSpanCode == null )
                {
                    if( code.Trim().Length != 0 &&
                        ( code.Length >= 2 ) )
                    {
                        code = code.Substring( 0, 2 );
                    }
                    selectedSpanCode = CodeWith<SpanCode>(facilityID, code);
                }
            }
            catch( Exception e )
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom( "SpanCodePBARBroker failed to initialize.", e, c_log );
            }
            
            return selectedSpanCode;
        }

        #endregion

        #region Private Methods	
        
        #endregion

        #region Construction and Finalization
        public SpanCodePBARBroker()
            : base()
        {
        }
        public SpanCodePBARBroker( string cxnString )
            : base( cxnString )
        {
        }

        public SpanCodePBARBroker( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Constants
        private const string
            SP_GET_ALL_SPANCODES = "SELECTALLSPANCODES",
            SP_GET_SPAN_CODE_WITH = "SELECTSPANCODEWITH";

        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( SpanCodePBARBroker ) );
        #endregion
    }
}
