using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    [Serializable]
    [UsedImplicitly]
    public class DOFRAidCodeRequired : LeafRule
    {
        [NonSerialized]
        private IDOFRFeatureManager _DOFRFeatureManager;

        private IDOFRFeatureManager DOFRFeatureManager
        {
            get { return _DOFRFeatureManager; }
            set { _DOFRFeatureManager = value; }
        }
        #region Event Handlers
        public event EventHandler DOFRAidCodeRequiredEvent;
        #endregion
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            DOFRAidCodeRequiredEvent += eventHandler;
            return true;
        }
        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            DOFRAidCodeRequiredEvent -= eventHandler;
            return true;
        }
        public override void UnregisterHandlers()
        {
            DOFRAidCodeRequiredEvent = null;
        }

        public override void ApplyTo(object context)
        {

        }

        public override bool ShouldStopProcessing()
        {
            return false;
        }


        public override bool CanBeAppliedTo(object context)
        {
            if (context == null)
            {
                return true;
            }

            CoverageGroup anCoverage = context as CoverageGroup;

            if (anCoverage == null || anCoverage.InsurancePlan == null)
            {
                return true;
            }

            if (anCoverage.Account == null)
            {
                return true;
            }

            Account anAccount = anCoverage.Account;

            DOFRFeatureManager = new DOFRFeatureManager();
            bool IsDOFREnabledForFacility = DOFRFeatureManager.IsDOFREnabledForFacility(anAccount);
            bool IsPrimaryInsuranceCoverage = DOFRFeatureManager.IsPrimaryInsuranceCoverage(anCoverage.CoverageOrder);
            bool IsInsurancePlanCommercial = DOFRFeatureManager.IsInsurancePlanCommercial(anAccount);

            if (IsDOFREnabledForFacility && IsPrimaryInsuranceCoverage && IsInsurancePlanCommercial)
            {
                if (!DOFRFeatureManager.IsCalOptimaPlanID(anAccount))
                    return true;
                
                if (anCoverage != null && string.IsNullOrEmpty(((CommercialCoverage)anCoverage).AidCode)
                        && string.IsNullOrEmpty(((CommercialCoverage)anCoverage).AidCodeType))
                {
                    if (this.FireEvents && DOFRAidCodeRequiredEvent != null)
                    {
                        DOFRAidCodeRequiredEvent(this, null);
                    }
                    return false;
                }
            }

            return true;
        }
    }

}
