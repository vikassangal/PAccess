using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;


namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for EmailReasonRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class EmailReasonRequired : LeafRule
    {

        private IEmailReasonFeatureManager _EmailReasonFeatureManager;

        private Account anAccount;

        private IEmailReasonFeatureManager EmailReasonFeatureManager
        {
            get { return _EmailReasonFeatureManager; }
            set { _EmailReasonFeatureManager = value; }
        }


        # region Event Handlers

        public event EventHandler EmailReasonRequiredEvent;

        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            EmailReasonRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            EmailReasonRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            EmailReasonRequiredEvent = null;
        }

        public override bool CanBeAppliedTo(object context)
        {
            if (context == null || context.GetType() != typeof(Account)
                && context.GetType().BaseType != typeof(Account))
            {
                return true;
            }
            anAccount = context as Account;
            if (anAccount == null)
            {
                return true;
            }
            EmailReasonFeatureManager = new EmailReasonFeatureManager();

            if ( HasValidEmailReason && 
                String.IsNullOrEmpty(anAccount.Patient.EmailReason.Code) )
            {
                if (FireEvents && EmailReasonRequiredEvent != null)
                {
                    EmailReasonRequiredEvent(this, null);
                }

                return false;
            }
            return true;
        }

        private bool HasValidEmailReason
        {
            get
            {
                return (EmailReasonFeatureManager.ShouldFeatureBeVisibleForAccountCreatedDate(anAccount) &&
                        !anAccount.COSSigned.IsRefused);
            }
        }
        public override
                void ApplyTo(object context)
        {

        }

        public override bool ShouldStopProcessing()
        {
            return false;
        }

        #endregion


    }
}
