using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for HospitalServiceRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidEthnicity2CodeChange : LeafRule
    {
        #region Events

        public event EventHandler InvalidEthnicity2CodeChangeEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidEthnicity2CodeChangeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidEthnicity2CodeChangeEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.InvalidEthnicity2CodeChangeEvent = null;
        }

        public override bool CanBeAppliedTo(object context)
        {
            if (context == null || context.GetType() != typeof(Account))
            {
                return true;
            }
            Account account = (Account)context;

            if (account.DischargeDate != DateTime.MinValue)
            {
                return true;
            }

            if (account.Patient != null &&
                (!account.Patient.IsEthnicity2Valid(account) ||
                 !account.Patient.IsDescent2Valid(account)))
            {
                if (this.FireEvents && InvalidEthnicity2CodeChangeEvent != null)
                {
                    this.InvalidEthnicity2CodeChangeEvent(this, null);
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void ApplyTo(object context)
        {

        }

        public override bool ShouldStopProcessing()
        {
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

        public InvalidEthnicity2CodeChange()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
