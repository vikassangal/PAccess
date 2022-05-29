using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.BrokerProxies
{
    /// <summary>
    /// Summary description for ScheduleCodeBrokerProxy.
    /// </summary>
    //TODO: Create XML summary comment for ScheduleCodeBrokerProxy
    [Serializable]
    public class ScheduleCodeBrokerProxy : AbstractBrokerProxy, IScheduleCodeBroker
    {
        #region Event Handlers

        #endregion

        #region Methods

        public ICollection AllScheduleCodes(long facilityID)
        {
            ICollection scheduleCodes = null;
            var cacheKey = "SCHEDULE_CODE_BROKER_ALL_SCHEDULE_CODES" + "_AND_FACILITY_" +
                           facilityID;
            if (this.Cache[cacheKey] == null)
            {
                lock (cacheKey)
                {
                    if (this.Cache[cacheKey] == null)
                    {
                        scheduleCodes = i_ScheduleCodeBroker.AllScheduleCodes(facilityID);
                        this.Cache.Insert(cacheKey,
                            scheduleCodes);
                    }
                    else
                    {
                        scheduleCodes = (ICollection) this.Cache[cacheKey];
                    }
                }
            }
            else
            {
                scheduleCodes = (ICollection) this.Cache[cacheKey];
            }

            return scheduleCodes;
        }

        public ScheduleCode ScheduleCodeWith(long facilityID, string code)
        {
            return i_ScheduleCodeBroker.ScheduleCodeWith(facilityID, code);
        }


        #endregion

        #region Properties

        #endregion

        #region Private Methods

        #endregion

        #region Private Properties

        #endregion

        #region Construction and Finalization

        public ScheduleCodeBrokerProxy()
        {
        }

        #endregion

        #region Data Elements

        private IScheduleCodeBroker i_ScheduleCodeBroker = BrokerFactory.BrokerOfType<IScheduleCodeBroker>();

        #endregion
    }
}
