using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.PAIWalkinOutpatientCreation;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.Utilities;

namespace PatientAccess.Rules
{
    [Serializable]
    [UsedImplicitly]
    public class NonStaffPhysicianNPIRequired : LeafRule
    {
        #region Events
        public event EventHandler NonStaffPhysicianNPIRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            NonStaffPhysicianNPIRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            NonStaffPhysicianNPIRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            NonStaffPhysicianNPIRequiredEvent = null;
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }

        public static bool IsValidActivity( Activity activity )
        {
            return ( activity is PreRegistrationActivity ||
                     activity is RegistrationActivity ||
                     activity is ShortRegistrationActivity ||
                     activity is ShortPreRegistrationActivity ||
                     activity is ShortMaintenanceActivity ||
                     activity is PreMSERegisterActivity ||
                     activity is PostMSERegistrationActivity ||
                     activity is UCCPreMSERegistrationActivity ||
                     activity is UCCPostMseRegistrationActivity ||
                     activity is AdmitNewbornActivity ||
                     activity is PreAdmitNewbornActivity ||
                     activity is MaintenanceActivity ||
                     activity is EditAccountActivity ||
                     activity is EditPreMseActivity ||
                     activity is PreRegistrationWithOfflineActivity ||
                     activity is RegistrationWithOfflineActivity ||
                     activity is PreMSERegistrationWithOfflineActivity ||
                     activity is AdmitNewbornWithOfflineActivity ||
                     activity is PreAdmitNewbornWithOfflineActivity ||
                     activity is ActivatePreRegistrationActivity ||
                     activity is QuickAccountCreationActivity ||
                     activity is QuickAccountMaintenanceActivity ||
                     activity is TransferOutToInActivity ||
                     activity is TransferOutpatientToERActivity ||
                     activity is TransferERToOutpatientActivity ||
                     activity is PAIWalkinOutpatientCreationActivity
                   );
        }

        public override bool CanBeAppliedTo( object context )
        {
            Guard.ThrowIfArgumentIsNull( context, "context" );
            if ( context.GetType() != typeof( Physician ) )
            {
                return true;
            }

            Physician physician = context as Physician;

            if ( AssociatedControl == null )
            {
                return true;
            }

            Activity activity = AssociatedControl as Activity;

            if ( activity == null || physician == null )
            {
                return true;
            }

            bool isValidActivity = IsValidActivity( activity );

            if ( !isValidActivity )
            {
                return true;
            }

            if ( physician.PhysicianNumber == Physician.NON_STAFF_PHYSICIAN_NUMBER )
            {
                if ( string.IsNullOrEmpty( physician.NPI ) )
                {
                    if ( FireEvents && NonStaffPhysicianNPIRequiredEvent != null )
                    {
                        NonStaffPhysicianNPIRequiredEvent( this, null );
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
