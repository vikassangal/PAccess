using System;
using Extensions.PersistenceCommon;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    [Serializable]
    public class PlansLoader : IValueLoader
    {
        #region Event Handlers
        #endregion

        #region Methods

        public object Load( object o )
        {
            return this.Load();
        }

        public object Load()
        {
            IInsuranceBroker broker = BrokerFactory.BrokerOfType< IInsuranceBroker >() ;
            return broker.InsurancePlansFor( this.Provider, this.FacilityOid, this.AdmitDate, this.i_PlanCategory );
        }
        #endregion

        #region Properties

        private AbstractProvider Provider
        {
            get
            {
                return i_Provider;
            }
            set
            {
                i_Provider = value;
            }
        }
        public InsurancePlanCategory PlanCategory
        {
            get
            {
                return i_PlanCategory;
            }
            set
            {
                i_PlanCategory = value;
            }
        }

        private long FacilityOid
        {
            get
            {
                return i_FacilityOid;
            }
            set
            {
                i_FacilityOid = value;
            }
        }

        private DateTime AdmitDate
        {
            get
            {
                return i_AdmitDate;
            }
            set
            {
                i_AdmitDate = value;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public PlansLoader( Payor aPayor, long facilityOid, DateTime admitDate, InsurancePlanCategory planCategory )
        {
            this.Provider       = aPayor;
            this.FacilityOid    = facilityOid;
            this.AdmitDate      = admitDate;
            this.i_PlanCategory = planCategory;
        }

        public PlansLoader( Broker aBroker, long facilityOid, DateTime admitDate, InsurancePlanCategory planCategory )
        {
            this.Provider       = aBroker;
            this.FacilityOid    = facilityOid;
            this.AdmitDate      = admitDate;
            this.i_PlanCategory = planCategory;
        }
        #endregion

        #region Data Elements
        private AbstractProvider        i_Provider;
        private InsurancePlanCategory   i_PlanCategory;
        private long                    i_FacilityOid;
        private DateTime                i_AdmitDate;

        #endregion

        #region Constants
        #endregion
    }
}
