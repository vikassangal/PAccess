using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for InvalidRaceCode.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InvalidRaceCode : LeafRule
    {
        #region Events
        
        public event EventHandler InvalidRaceCodeEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.InvalidRaceCodeEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.InvalidRaceCodeEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.InvalidRaceCodeEvent = null;   
        }

        public override bool CanBeAppliedTo(object context)
        {
            if (context == null || context.GetType() != typeof(Account))
            {
                return true;
            }

            Account account = (Account) context;

            if (account.Patient != null &&
                (!account.Patient.IsRaceValid(account) ||
                 !account.Patient.IsNationalityValid(account)))
            {
                if (this.FireEvents && InvalidRaceCodeEvent != null)
                {
                    this.InvalidRaceCodeEvent(this, null);
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
            return false;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public InvalidRaceCode()
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
