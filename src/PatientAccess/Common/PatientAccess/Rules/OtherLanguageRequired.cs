using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for OtherLanguageRequired.
    /// </summary>
    //TODO: Create XML summary comment for OtherLanguageRequired
    [Serializable]
    [UsedImplicitly]
    public class OtherLanguageRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler OtherLanguageRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            OtherLanguageRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            OtherLanguageRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.OtherLanguageRequiredEvent = null;
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo( object context )
        {
        }
        private static bool IsFacilityInCalifornia( Facility facility )
        {
            facility.SetFacilityStateCode();
            return facility.IsFacilityInState( State.CALIFORNIA_CODE );
        }

        public override bool CanBeAppliedTo( object context )
        {
            if ( context.GetType() != typeof( Account ) )
            {
                return true;
            }
            Account anAccount = context as Account;
            if ( anAccount == null || anAccount.Patient == null || anAccount.Patient.Language ==null )
            {
                return true;
            }

            Facility facility = anAccount.Facility;
            Language language = anAccount.Patient.Language;

            if ( facility != null && IsFacilityInCalifornia( facility ) &&
                language.IsOtherLanguage() &&
                String.IsNullOrEmpty(anAccount.Patient.OtherLanguage ))
            {
                if ( this.FireEvents && OtherLanguageRequiredEvent != null )
                {
                    OtherLanguageRequiredEvent( this, null );
                }
                return false;
            }

            return true;
        }

        #endregion
    }
}

