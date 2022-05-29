using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for OnInsuredFormForPrimaryInsurance.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class OnInsuredFormForPrimaryInsurance : CompositeRule
    {
        public OnInsuredFormForPrimaryInsurance()
        {
        }

        public override bool RegisterHandler(EventHandler eventHandler)
        {        
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {        
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            
        }

        public override bool CanBeAppliedTo(object context)
        {
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
