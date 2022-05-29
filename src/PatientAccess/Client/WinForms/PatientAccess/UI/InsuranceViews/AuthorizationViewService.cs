using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews
{
    [Serializable]
    public class AuthorizationViewService
    {
        #region Event Handlers
        #endregion

        #region Methods
        public bool EnableTrackingNumber()
        {
            var enableTracking = !(Coverage != null && Coverage.GetType() == typeof( WorkersCompensationCoverage ));
            return enableTracking;
        }

        public bool IsCoverageValidForAuthorization()
        {
            bool isValidCoverage = true;

            if (this.Coverage != null)
            {
                if (Coverage.GetType() == typeof(SelfPayCoverage))
                {
                    isValidCoverage = false;
                }

                if (Coverage.GetType() == typeof(GovernmentMedicareCoverage) &&
                    !Coverage.IsMedicareCoverageValidForAuthorization)
                {
                    isValidCoverage = false;
                }
            }

            return isValidCoverage;
        }


        public ICollection GetAllAuthorizationStatuses()
        {
            IAuthorizationStatusBroker broker = BrokerFactory.BrokerOfType<IAuthorizationStatusBroker>();
            ICollection authorizationStatuses = broker.AllAuthorizationStatuses();

            return authorizationStatuses;
        }

        public Authorization GetAuthorization()
        {
            var authorization = new Authorization();
            if (this.Coverage != null &&
                this.Coverage.GetType().IsSubclassOf(typeof(CoverageGroup)))
            {
                authorization = ((CoverageGroup) this.Coverage).Authorization;
            }

            if (this.Coverage != null && Coverage.IsMedicareCoverageValidForAuthorization)
            {
                authorization = ((GovernmentMedicareCoverage) Coverage).Authorization;
            }

            return authorization;
        }

        #endregion

        #region Properties
        public Coverage Coverage
        {
            set
            {
                i_Coverage = value;
            }
            private get
            {
                return i_Coverage;
            }
        }
        #endregion
 
        #region Data Elements
        Coverage i_Coverage = null;
        #endregion
         
    }
}
