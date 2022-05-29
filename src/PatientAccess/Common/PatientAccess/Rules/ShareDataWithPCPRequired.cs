using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ShareDataWithPCPRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class ShareDataWithPCPRequired : LeafRule
    {

        public event EventHandler ShareDataWithPCPRequiredEvent;

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.ShareDataWithPCPRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.ShareDataWithPCPRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.ShareDataWithPCPRequiredEvent = null;
        }

        public override bool CanBeAppliedTo(object context)
        {

            if (context == null || context.GetType() != typeof(Account))
            {
                return true;
            }

            Account Model = ((Account)context);


            if (String.IsNullOrEmpty(Model.ShareDataWithPCPFlag.Code.Trim()))
            {
                if (this.FireEvents && ShareDataWithPCPRequiredEvent != null)
                {
                    this.ShareDataWithPCPRequiredEvent(this, null);
                }

                return false;
            }
            return true;
        }
 
        public override void ApplyTo(object context)
        {
            
        }

        public override bool ShouldStopProcessing()
        {
            return false;
        }

        

	}
}
