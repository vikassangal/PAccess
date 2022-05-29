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
    public class InvalidBirthGenderCodeChange : LeafRule
    {
        #region Events

        public event EventHandler InvalidBirthGenderCodeChangeEvent;

        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidBirthGenderCodeChangeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidBirthGenderCodeChangeEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.InvalidBirthGenderCodeChangeEvent = null;
        }

        public override bool CanBeAppliedTo(object context)
        {
            if (context == null || context.GetType() != typeof(Account))
            {
                return true;
            }

            Account account = (Account) context;

            if ((account.Patient != null) && (account.Patient.BirthSex != null))
            {
                if (!(account.Patient.BirthSex.IsValid))

                {
                    if (this.FireEvents && InvalidBirthGenderCodeChangeEvent != null)
                    {
                        this.InvalidBirthGenderCodeChangeEvent(this, null);
                    }

                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        #endregion
    }
}
