using System;
using Extensions.PersistenceCommon;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.BrokerInterfaces
{
    [Serializable]
    public class CoveredGroupPlansLoader : IValueLoader
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
            return broker.InsurancePlansFor(this.coveredGroup, this.FacilityOid, this.AdmitDate, this.i_PlanCategory);
        }
        #endregion

        #region Properties

        private CoveredGroup coveredGroup
        {
            get
            {
                return i_coveredGroup;
            }
            set
            {
                i_coveredGroup= value;
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
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
  
        public CoveredGroupPlansLoader( CoveredGroup aCoveredGroup , long facilityOid, DateTime admitDate )
        {
            this.coveredGroup   = aCoveredGroup;
            this.FacilityOid    = facilityOid;
            this.AdmitDate      = admitDate;
        }
        #endregion

        #region Data Elements
       
        private CoveredGroup            i_coveredGroup;
        private long                    i_FacilityOid;
        private DateTime                i_AdmitDate;
        private InsurancePlanCategory   i_PlanCategory = null;
        #endregion

        #region Constants
        #endregion
    }
}
