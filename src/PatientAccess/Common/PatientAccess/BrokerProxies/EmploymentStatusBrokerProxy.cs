using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.BrokerProxies
{

    public class EmploymentStatusBrokerProxy : AbstractBrokerProxy, IEmploymentStatusBroker
    {

        #region Event Handlers

        #endregion

        #region IEmploymentStatusBroker Member Methods

        public ICollection AllTypesOfEmploymentStatuses(long facilityID)
        {
            var cacheKey = "EMPLOYMENT_STATUS_BROKER_PROXY_ALL_TYPES_OF_EMPLOYMENT_STATUSES" + "_AND_FACILITY_" +
                           facilityID;
            ICollection allTypesOfEmpSt = (ICollection) this.Cache[cacheKey];

            if (null == allTypesOfEmpSt)
            {
                lock (cacheKey)
                {
                    allTypesOfEmpSt = this.i_EmploymentStatusBroker.AllTypesOfEmploymentStatuses(facilityID);
                    if (null == this.Cache[cacheKey])
                    {
                        this.Cache.Insert(cacheKey, allTypesOfEmpSt);
                    }
                }
            }

            return allTypesOfEmpSt;
        }

        public EmploymentStatus EmploymentStatusWith(long oid)
        {
            return this.i_EmploymentStatusBroker.EmploymentStatusWith(oid);
        }

        public EmploymentStatus EmploymentStatusWith(long facilityID, string code)
        {
            return this.i_EmploymentStatusBroker.EmploymentStatusWith(facilityID, code);
        }

        #endregion


        #region Properties

        #endregion

        #region Private Methods

        #endregion

        #region Private Properties

        #endregion

        #region Construction and Finalization

        public EmploymentStatusBrokerProxy()
        {
        }

        #endregion

        #region Data Elements

        private IEmploymentStatusBroker i_EmploymentStatusBroker =
            BrokerFactory.BrokerOfType<IEmploymentStatusBroker>();

        #endregion

    }

}
