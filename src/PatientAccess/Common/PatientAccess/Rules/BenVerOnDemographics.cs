using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for BenVerOnDemographics.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class BenVerOnDemographics : CompositeRule
    {
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

        public override bool ShouldStopProcessing()
        {
            return false;
        }
    }
}
