using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    [Serializable]
    [UsedImplicitly]
    public class DOFRInitiateRequired : LeafRule
    {
        [NonSerialized]
        private IDOFRFeatureManager _DOFRFeatureManager;

        private IDOFRFeatureManager DOFRFeatureManager
        {
            get { return _DOFRFeatureManager; }
            set { _DOFRFeatureManager = value; }
        }
        #region Event Handlers
        public event EventHandler DOFRInitiateRequiredEvent;
        #endregion
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            DOFRInitiateRequiredEvent += eventHandler;
            return true;
        }
        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            DOFRInitiateRequiredEvent -= eventHandler;
            return true;
        }
        public override void UnregisterHandlers()
        {
            DOFRInitiateRequiredEvent = null;
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
            if (context == null ||
                 context.GetType() != typeof(Account))
            {
                return true;
            }

            DOFRFeatureManager = new DOFRFeatureManager();
            var anAccount = context as Account;


            if (anAccount == null)
            {
                return true;
            }
            if (anAccount.Facility == null || string.IsNullOrEmpty(anAccount.Facility.Code))
            {
                return true;
            }

            if (DOFRFeatureManager.IsDOFREnabledForFacility(anAccount))
            {
                if(DOFRFeatureManager.IsDOFRValid(anAccount))
                {
                    if (DOFRFeatureManager.IsDOFRInsurancePlanPartOfIPAValid(anAccount) == false)
                        return true;

                    if (DOFRFeatureManager.IsMedicalGroupIPACodeValid(anAccount) == false)
                        return true;

                    if (anAccount.IsDOFRInitiated)
                        return true;

                    if (this.FireEvents && DOFRInitiateRequiredEvent != null)
                    {
                        DOFRInitiateRequiredEvent(this, null);
                    }
                    return false;

                }
            }

            return true;
        }
    }

}
