using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ProcedureRequired.
    /// </summary>
    //TODO: Create XML summary comment for ProcedureRequired
    [Serializable]
    [UsedImplicitly]
    public class ProcedureRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler ProcedureRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.ProcedureRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.ProcedureRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.ProcedureRequiredEvent = null;
        }

        public override bool CanBeAppliedTo(object context)
        {
            if (context == null ||
                context.GetType() != typeof(Account))
            {
                return true;
            }
            Account account = (Account)context;

            if (  account.KindOfVisit.Code.Equals(VisitType.EMERGENCY_PATIENT) )
            {
                return true;
            }
            if (string.IsNullOrEmpty(account.Diagnosis.Procedure.Trim()))
            {
                if (this.FireEvents && ProcedureRequiredEvent != null)
                {
                    this.ProcedureRequiredEvent(this, null);
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
        public ProcedureRequired()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
