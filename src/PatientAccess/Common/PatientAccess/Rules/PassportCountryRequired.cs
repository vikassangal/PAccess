using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PassportCountryRequired.
    /// </summary>
    //TODO: Create XML summary comment for PassportCountryRequired
    [Serializable]
    [UsedImplicitly]
    public class PassportCountryRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler PassportCountryRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            PassportCountryRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            PassportCountryRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.PassportCountryRequiredEvent = null;
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
            bool result = true;

            if (context == null || context.GetType() != typeof(Account))
            {
                return true;
            }

            Account anAccount = context as Account;
            if (anAccount == null
                || anAccount.Patient == null
                || anAccount.Patient.Passport == null)
            {
                return true;
            }
            else
            {

                // If a passport number is entered, a country must be selected
                if (anAccount.Patient.Passport.Number.Trim() != String.Empty &&
                    (anAccount.Patient.Passport.Country == null
                    || anAccount.Patient.Passport.Country.Description.Trim().Length <= 0))
                {
                    result = false;
                }
            }

            if (!result
                && this.FireEvents && PassportCountryRequiredEvent != null)
            {
                PassportCountryRequiredEvent(this, null);
            }

            return result;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public PassportCountryRequired()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}

