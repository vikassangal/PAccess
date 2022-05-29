using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Rules
{
    [Serializable]
    [UsedImplicitly]
    public class NonStaffNPIIsValid : LeafRule
    {
        #region Events
        public event EventHandler NonStaffNPIIsValidEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            NonStaffNPIIsValidEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            NonStaffNPIIsValidEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            NonStaffNPIIsValidEvent = null;
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }
        public override bool CanBeAppliedTo( object context )
        {
            if ( context.GetType() != typeof( Physician ) )
            {
                return true;
            }
            Physician physician = context as Physician;
            if ( physician == null || string.IsNullOrEmpty( physician.NPI ) )
            {
                return true;
            }

            if ( physician.PhysicianNumber == Physician.NON_STAFF_PHYSICIAN_NUMBER )
            {
                if ( !Physician.isValidNPI( physician.NPI ) )
                {
                    if ( FireEvents && NonStaffNPIIsValidEvent != null )
                    {
                        NonStaffNPIIsValidEvent( this, null );
                    }
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region Properties
        #endregion

    }
}
