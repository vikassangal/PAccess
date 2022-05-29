using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for BloodlessPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class BloodlessPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler BloodlessPreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            BloodlessPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            BloodlessPreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.BloodlessPreferredEvent = null;
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo(object context)
        {
        }

        /// <summary>
        /// CanBeAppliedTo - this rule is a member of a composite (to PersonInfo).
        /// Refer to PersonInfo.cs for additional logic applied to this rule.
        /// </summary>
        /// <param FirstName="context"></param>
        /// <returns></returns>
        public override bool CanBeAppliedTo(object context)
        {
            if (context == null || context.GetType() != typeof(Account))
            {
                return true;
            }

            var anAccount = context as Account;

            if (anAccount != null
                && anAccount.Bloodless != null
                && anAccount.Bloodless.Description.Trim().Length > 0)
            {
                return true;
            }
            else
            {
                if (this.FireEvents && BloodlessPreferredEvent != null)
                {
                    BloodlessPreferredEvent(this, new PropertyChangedArgs(this.AssociatedControl));
                }
                return false;
            }
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public BloodlessPreferred()
        {
        }
        #endregion

        #region Data Elements

        #endregion

        #region Constants
        #endregion
    }
}
