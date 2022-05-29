using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    [Serializable]
    [UsedImplicitly]
    public class DOFRInsurancePartIPARequired : LeafRule
    {
        [NonSerialized]
        private IDOFRFeatureManager _DOFRFeatureManager;

        private IDOFRFeatureManager DOFRFeatureManager
        {
            get { return _DOFRFeatureManager; }
            set { _DOFRFeatureManager = value; }
        }
        #region Event Handlers
        public event EventHandler DOFRInsurancePartIPARequiredEvent;
        #endregion
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            DOFRInsurancePartIPARequiredEvent += eventHandler;
            return true;
        }
        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            DOFRInsurancePartIPARequiredEvent -= eventHandler;
            return true;
        }
        public override void UnregisterHandlers()
        {
            DOFRInsurancePartIPARequiredEvent = null;
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
                if (anAccount.KindOfVisit != null && !string.IsNullOrEmpty(anAccount.KindOfVisit.Code))
                {
                    if (anAccount.KindOfVisit.Code == VisitType.PREREG_PATIENT || anAccount.KindOfVisit.Code == VisitType.NON_PATIENT)
                        return true;
                }

                if (anCoverage != null && (
                    ((CommercialCoverage)anCoverage).IsInsurancePlanPartOfIPA == true
                    || ((CommercialCoverage)anCoverage).IsInsurancePlanPartOfIPA == false) )
                {
                    return true;
                }

                if (anCoverage != null && ((CommercialCoverage)anCoverage).IsInsurancePlanPartOfIPA == null)
                {
                    if (this.FireEvents && DOFRInsurancePartIPARequiredEvent != null)
                    {
                        DOFRInsurancePartIPARequiredEvent(this, null);
                    }
                    return false;
                }
            }

            return true;
        }
    }

}
