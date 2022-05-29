using System;
using System.Collections;
using System.Data;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    ///Implements Discharge related methods.
    /// </summary>
    [Serializable]
    public class DischargePBARBroker : PBARCodesBroker, IDischargeBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        protected override void InitProcNames()
        {
            this.AllStoredProcName = string.Empty;
            this.WithStoredProcName = string.Empty;
        }
        /// <summary>
        /// Get a list of all discharge disposition objects including oid, code and description.
        /// </summary>
        /// <returns></returns>
       public ICollection AllDischargeDispositions(long facilityID)
        {
            ICollection dispositions = null;
            var key = CacheKeys.CACHE_KEY_FOR_DISCHARGE_DISPOSITIONS;
            this.InitFacility(facilityID);
            LoadCacheDelegate loadData = delegate()
            {
                try
                {
                    dispositions = LoadDataToArrayList<DischargeDisposition>(facilityID);
                }
                catch (Exception e)
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom("DischargePBARBroker failed to initialize", e, c_log);
                }
                return dispositions;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                this.AllStoredProcName = SP_SELECT_DISPOSITIONS;
                dispositions = cacheManager.GetCollectionBy(key, facilityID,
                    loadData);
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("DischargePBARBroker failed to initialize", e, c_log);
            }
            return dispositions;
        }
        /// <summary>
        /// Get one DischargeDisposition object based on the code.
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public DischargeDisposition DischargeDispositionWith( long facilityID,string code )
        {
            DischargeDisposition selectedDischargeDisposition = null;
            if( code == null )
            {
                throw new ArgumentNullException( "code cannot be null or empty" );
            }
            code = code.Trim().ToUpper();
            this.InitFacility( facilityID );

          try
          {
              ICollection allDischargeDispositions = this.AllDischargeDispositions( facilityID );

                foreach (DischargeDisposition dischargeDisposition in allDischargeDispositions)
                {
                    if (dischargeDisposition.Code.Equals(code))
                    {
                        selectedDischargeDisposition = dischargeDisposition;
                        break;
                    }
                }
                if (selectedDischargeDisposition == null)
                {
                    this.WithStoredProcName = SP_SELECTDISPOSITION_WITH;
                    selectedDischargeDisposition = CodeWith<DischargeDisposition>(facilityID, code);
                }
             }
           catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("DischargePBARBroker failed to initialize", e, c_log);
            }
            return selectedDischargeDisposition;
        }


        #endregion

        #region Properties

        #endregion

      
        #region Private Properties
        #endregion

        #region Construction and Finalization
        public DischargePBARBroker()
            : base()
        {
        }
        public DischargePBARBroker( string cxnString )
            : base( cxnString )
        {
        }

        public DischargePBARBroker( IDbTransaction txn )
            : base( txn )
        {
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( DischargePBARBroker ) );
        #endregion

        #region Constants
        private const string
            SP_SELECT_DISPOSITIONS = "SELECTALLDISPOSITIONS",
            SP_SELECTDISPOSITION_WITH = "SELECTDISPOSITIONWITH";
        #endregion
    }
}
