using System;
using System.Collections;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for InsurancePlanSSNRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InsurancePlanIPARequired : LeafRule
    {
        [NonSerialized]
        private IDOFRFeatureManager _DOFRFeatureManager;

        private IDOFRFeatureManager DOFRFeatureManager
        {
            get { return _DOFRFeatureManager; }
            set { _DOFRFeatureManager = value; }
        }
        #region Event Handlers
        public event EventHandler InsurancePlanIPARequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            InsurancePlanIPARequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            InsurancePlanIPARequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.InsurancePlanIPARequiredEvent = null;
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo(object context)
        {
        }

        public override bool CanBeAppliedTo(object context)
        {
            if( context == null )
            {
                return true;
            }

            if( context.GetType() != typeof( CoverageForCommercialOther )
                && context.GetType() != typeof( GovernmentOtherCoverage )
                && context.GetType() != typeof( OtherCoverage )
                && context.GetType() != typeof( GovernmentMedicaidCoverage)
                && context.GetType().BaseType != typeof( CoverageForCommercialOther )
                && context.GetType().BaseType != typeof( GovernmentOtherCoverage )
                && context.GetType().BaseType != typeof( OtherCoverage )
                && context.GetType().BaseType != typeof( GovernmentMedicaidCoverage ))
            {
                return true;
            }

            CoverageGroup anCoverage = context as CoverageGroup;

            if (anCoverage == null || anCoverage.InsurancePlan == null)
                return true;

            if (anCoverage.Account == null)
                return true;

            Account anAccount = anCoverage.Account;

            DOFRFeatureManager = new DOFRFeatureManager();
            bool IsDOFREnabledForFacility = DOFRFeatureManager.IsDOFREnabledForFacility(anAccount);
            bool IsPrimaryInsuranceCoverage = DOFRFeatureManager.IsPrimaryInsuranceCoverage(anCoverage.CoverageOrder);
            bool IsInsurancePlanCommercial = DOFRFeatureManager.IsInsurancePlanCommercial(anAccount);
            
            if (IsDOFREnabledForFacility && IsPrimaryInsuranceCoverage && IsInsurancePlanCommercial)
            {
                if (anCoverage != null && ((CommercialCoverage)anCoverage).IsInsurancePlanPartOfIPA == false)
                {
                    return true;
                }
            }

            if (IsDOFREnabledForFacility && IsPrimaryInsuranceCoverage && IsInsurancePlanCommercial &&
                   ((CommercialCoverage)anCoverage).IsInsurancePlanPartOfIPA == true)
            {
            }
            else
            {
                if (!this.lstIPAPlanIDs.Contains(anCoverage.InsurancePlan.PlanID.Substring(3, 1)))
                {
                    return true;
                }

                if (anAccount.Facility == null ||
                    anAccount.Facility.ContactPoints == null ||
                    anAccount.Facility.ContactPoints.Count == 0)
                {
                    return true;
                }

                ContactPoint aContactPoint = null;
                foreach (ContactPoint cp in anAccount.Facility.ContactPoints)
                {
                    if (cp != null)
                        aContactPoint = cp;
                }

                if (aContactPoint.Address == null ||
                    aContactPoint.Address.State == null ||
                    aContactPoint.Address.State.Code != "CA")
                {
                    return true;
                }

            }
          

            if (anAccount.MedicalGroupIPA == null 
                || anAccount.MedicalGroupIPA.Name.Trim() == string.Empty
                || anAccount.MedicalGroupIPA.Clinics == null
                || anAccount.MedicalGroupIPA.Clinics.Count == 0)
            {
                if (this.FireEvents && InsurancePlanIPARequiredEvent != null)
                {
                    InsurancePlanIPARequiredEvent(this, null);
                }
                return false;
            }
            return true;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public InsurancePlanIPARequired()
        {
            for (int i = 0; i < arrIPAPlanIDs.Length; i++)
            {
                lstIPAPlanIDs.Add(arrIPAPlanIDs[i]);
            }
        }
        #endregion

        #region Data Elements
        private ArrayList lstIPAPlanIDs = new ArrayList();
        #endregion

        #region Constants
        string[] arrIPAPlanIDs = { "A", "B", "D", "F", "G", "S", "T", "U", "V", "W", "X" };
        #endregion
    }
}
