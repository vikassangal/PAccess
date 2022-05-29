using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PreopDateRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class PreopDateRequired : LeafRule
    {
        #region Events
        public event EventHandler PreopDateRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.PreopDateRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.PreopDateRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.PreopDateRequiredEvent = null;
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
            if (anAccount.ShouldWeEnablePreopDate() && anAccount.PreopDate.Date.Equals(DateTime.MinValue))
            {
                if (this.FireEvents && PreopDateRequiredEvent != null)
                {
                    PreopDateRequiredEvent(this, null);
                }
                return false;
            }
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

        public PreopDateRequired()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
