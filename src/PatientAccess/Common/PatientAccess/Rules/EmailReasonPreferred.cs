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
    public class EmailReasonPreferred: LeafRule
    {

        private IEmailReasonFeatureManager _EmailReasonFeatureManager;

        private Account anAccount;

        private IEmailReasonFeatureManager EmailReasonFeatureManager
        {
            get { return _EmailReasonFeatureManager; }
            set { _EmailReasonFeatureManager = value; }
        }


        # region Event Handlers

        public event EventHandler EmailReasonPreferredEvent;

        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            EmailReasonPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            EmailReasonPreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            EmailReasonPreferredEvent = null;
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

            if ((EmailReasonFeatureManager.ShouldFeatureBeVisibleForAccountCreatedDate(anAccount))
                && String.IsNullOrEmpty(anAccount.Patient.EmailReason.Code))
            {
                if (this.FireEvents && EmailReasonPreferredEvent != null)
                {
                    this.EmailReasonPreferredEvent(this, null);
                }

                return false;
            }
            return true;
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
