using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;

namespace PatientAccess.Rules
{
    /// <summary>
    /// LocationFieldsRequired - composite rule for the fields required to determine if duplicate bed assignments exist
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class LocationFieldsRequired : CompositeRule
    {
        public LocationFieldsRequired()
        {
        }

        public override bool RegisterHandler( EventHandler eventHandler )
        {
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            return true;
        }

        public override void UnregisterHandlers()
        {

        }

        public override bool CanBeAppliedTo( object context )
        {
            return true;
        }

        public override void ApplyTo( object context )
        {

        }

        public override bool ShouldStopProcessing()
        {
            return false;
        }
    }
}
