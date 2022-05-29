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
    public class ScheduleCodePBARBroker : PBARCodesBroker, IScheduleCodeBroker
    {
        #region Event Handlers
        #endregion

        #region Methods
        protected override void InitProcNames()
        {
            this.AllStoredProcName = SP_SELECT_ALL_SCHEDULE_CODES;
            this.WithStoredProcName = SP_SELECT_SCHEDULE_CODE_WITH;
        }
        /// <summary>
        /// Get a list of ScheduleCode objects including oid, code and description.
        /// </summary>
       
        public ICollection AllScheduleCodes( long facilityID )
        {
            ICollection allScheduleCodes = null;
            var key = CacheKeys.CACHE_KEY_FOR_SCHEDULECODES;
            this.InitFacility(facilityID);
            LoadCacheDelegate loadData = delegate()
            {
                try
                {
                    allScheduleCodes = LoadDataToArrayList<ScheduleCode>(facilityID);
                }
                catch (Exception e)
                {
                    throw BrokerExceptionFactory.BrokerExceptionFrom("ScheduleCodeBroker failed to initialize", e, c_log);
                }
                return allScheduleCodes;
            };
            try
            {
                CacheManager cacheManager = new CacheManager();
                this.AllStoredProcName = SP_SELECT_ALL_SCHEDULE_CODES;
                allScheduleCodes = cacheManager.GetCollectionBy(key, facilityID,
                    loadData);
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("ScheduleCodeBroker failed to initialize", e, c_log);
            }
            return allScheduleCodes;
        }
     
        public ScheduleCode ScheduleCodeWith(long facilityID, string code)
        {
            ScheduleCode selectedScheduleCode = null;
            if (code == null)
            {
                throw new ArgumentNullException("code cannot be null or empty");
            }
            code = code.Trim().ToUpper();
            this.InitFacility(facilityID);
            try
            {
                ICollection allScheduleCodes = this.AllScheduleCodes(facilityID);

                foreach (ScheduleCode scheduleCode in allScheduleCodes)
                {
                    if (scheduleCode.Code.Equals(code))
                    {
                        selectedScheduleCode = scheduleCode;
                        break;
                    }
                }

                if (selectedScheduleCode == null)
                {
                    this.WithStoredProcName = SP_SELECT_SCHEDULE_CODE_WITH;
                    selectedScheduleCode = CodeWith<ScheduleCode>(facilityID, code);
                }
            }
            catch (Exception e)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("ScheduleCodeBroker failed to retrieve object.", e, c_log);
            }
            return selectedScheduleCode;
        }       

        #endregion

        #region Data Elements
        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( ScheduleCodePBARBroker ) );
        #endregion

        #region Constants
        private const string
            SP_SELECT_ALL_SCHEDULE_CODES = "SelectAllScheduleCodes",
            SP_SELECT_SCHEDULE_CODE_WITH = "SelectScheduleCodeWith";
        #endregion
    }
}
