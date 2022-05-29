using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;


namespace PatientAccess.Rules
{
    [Serializable]
    [UsedImplicitly]
    public class HospitalCommunicationOptInRequired : LeafRule
    {
        [NonSerialized]
        private IHospitalCommunicationOptInFeatureManager hospitalCommunicationOptInFeatureManager;

        private IHospitalCommunicationOptInFeatureManager HospitalCommunicationOptInFeatureManager
        {
            get { return hospitalCommunicationOptInFeatureManager; }
            set { hospitalCommunicationOptInFeatureManager = value; }
        }

        #region Event Handlers

        public event EventHandler HospitalCommunicationOptInRequiredEvent;

        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            HospitalCommunicationOptInRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            HospitalCommunicationOptInRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            HospitalCommunicationOptInRequiredEvent = null;
        }

        public override bool CanBeAppliedTo(object context)
        {
            if (context == null ||
                context.GetType() != typeof(Account))
            {
                return true;
            }
            HospitalCommunicationOptInFeatureManager = new HospitalCommunicationOptInFeatureManager();
            var anAccount = context as Account;

            if (anAccount == null)
            {
                return true;
            }
            if (HospitalCommunicationOptInFeatureManager.ShouldFeatureBeEnabled(anAccount) &&
                anAccount.Patient.HospitalCommunicationOptIn.Code == YesNoFlag.Blank.Code)
            {

                if (FireEvents && HospitalCommunicationOptInRequiredEvent != null)
                {
                    HospitalCommunicationOptInRequiredEvent(this, null);
                }
                return false;
            }
            return true;
        }

        #endregion
    }
}
