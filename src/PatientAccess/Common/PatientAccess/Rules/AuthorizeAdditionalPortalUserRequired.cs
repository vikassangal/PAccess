using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    [Serializable]
    [UsedImplicitly]
    public class AuthorizeAdditionalPortalUserRequired : LeafRule
    {
        [NonSerialized]
        private IAuthorizePortalUserFeatureManager _authorizePortalUserFeatureManager;

        private IAuthorizePortalUserFeatureManager AuthorizePortalFeatureManager
        {
            get { return _authorizePortalUserFeatureManager; }
            set { _authorizePortalUserFeatureManager = value; }
        }
        #region Event Handlers
        public event EventHandler AuthorizePortalUserRequiredEvent;
        #endregion
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            AuthorizePortalUserRequiredEvent += eventHandler;
            return true;
        }
        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            AuthorizePortalUserRequiredEvent -= eventHandler;
            return true;
        }
        public override void UnregisterHandlers()
        {
            AuthorizePortalUserRequiredEvent = null;
        }
        public override bool CanBeAppliedTo(object context)
        {
            if (context == null ||
                context.GetType() != typeof(Account))
            {
                return true;
            }

            AuthorizePortalFeatureManager = new AuthorizePortalUserFeatureManager();
            var anAccount = context as Account;


            if (anAccount == null)
            {
                return true;
            }

            if (anAccount.Activity.IsMaintenanceActivity() || anAccount.Activity.IsShortMaintenanceActivity())
            {
                return true;
            }

            if (anAccount.Patient.MedicalRecordNumber != 0)
            {
                if (AuthorizePortalFeatureManager.IsAuthorizePortalUserRequiredForExistingPatient(anAccount))
                {
                    if (FireEvents && AuthorizePortalUserRequiredEvent != null)
                    {
                        AuthorizePortalUserRequiredEvent(this, null);
                    }

                    return false;
                }
            }
            else if (anAccount.Patient.MedicalRecordNumber == 0)
            {
                if (AuthorizePortalFeatureManager.IsAuthorizePortalUserRequiredForNewPatient(anAccount))
                {
                    if (FireEvents && AuthorizePortalUserRequiredEvent != null)
                    {
                        AuthorizePortalUserRequiredEvent(this, null);
                    }

                    return false;
                }
            }
            
            return true;
        }
    }
   
}
