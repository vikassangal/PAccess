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
    public class ShareDataWithPublicHIERequired : LeafRule
    {

        public event EventHandler ShareHIEDataRequiredEventhandler;

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.ShareHIEDataRequiredEventhandler += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.ShareHIEDataRequiredEventhandler -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.ShareHIEDataRequiredEventhandler = null;
        }

        public override bool CanBeAppliedTo(object context)
        {

            if (context == null || context.GetType() != typeof(Account))
            {
                return true;
            }

            Account Model = ((Account)context);


            if (String.IsNullOrEmpty(Model.ShareDataWithPublicHieFlag.Code.Trim()))
            {
                if (this.FireEvents && ShareHIEDataRequiredEventhandler != null)
                {
                    this.ShareHIEDataRequiredEventhandler(this, null);
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
