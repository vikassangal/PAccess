using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    [Serializable]
    [UsedImplicitly]
    public class PatientPortalOptInRequired : LeafRule
    {

        [NonSerialized]
        private IPatientPortalOptInFeatureManager _patientPortalOptInFeatureManager;

        private IPatientPortalOptInFeatureManager PatientPortalOptInFeatureManager
        {
            get { return _patientPortalOptInFeatureManager; }
            set { _patientPortalOptInFeatureManager = value; }
        }

        #region Event Handlers
        public event EventHandler PatientPortalOptInRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            PatientPortalOptInRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            PatientPortalOptInRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            PatientPortalOptInRequiredEvent = null;
        }

        public override bool CanBeAppliedTo(object context)
        {
            if (context == null ||
                context.GetType() != typeof(Account))
            {
                return true;
            }

            PatientPortalOptInFeatureManager = new PatientPortalOptInFeatureManager();
            var anAccount = context as Account;


            if (anAccount == null )
            {
                return true;
            }

            if ( PatientPortalOptInFeatureManager.ShouldFeatureBeEnabled(anAccount) &&
                        anAccount.PatientPortalOptIn.Code == YesNoFlag.Blank.Code ) 
                {
                    if (FireEvents && PatientPortalOptInRequiredEvent != null)
                    {
                        PatientPortalOptInRequiredEvent(this, null);
                    }

                    return false;
                }
           
            return true;
        }

        public override void ApplyTo(object context)
        {

        }

        public override bool ShouldStopProcessing()
        {
            return false;
        }

        #endregion 
    }
}



