using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for EthnicityRequired.
    /// </summary>
    //TODO: Create XML summary comment for EthnicityRequired
    [Serializable]
    [UsedImplicitly]
    public class EthnicityRequired : LeafRule
    {
        #region Event Handlers

        public event EventHandler EthnicityRequiredEvent;

        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            EthnicityRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            EthnicityRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            EthnicityRequiredEvent = null;
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
            if (context.GetType() != typeof(Account))
            {
                return true;
            }

            Account anAccount = context as Account;
            if (anAccount == null)
            {
                return false;
            }

            if (anAccount.Patient != null &&
                (anAccount.Patient.Ethnicity == null
                 || anAccount.Patient.Ethnicity.Code.Trim() == String.Empty
                 || anAccount.Patient.Ethnicity.Code.Trim() == Ethnicity.ZERO_CODE)
            )
            {
                if (FireEvents && EthnicityRequiredEvent != null)
                {
                    EthnicityRequiredEvent(this, null);
                }

                return false;
            }

            return true;
        }

        #endregion
    }
}

