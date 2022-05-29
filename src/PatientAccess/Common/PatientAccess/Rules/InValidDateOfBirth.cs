using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for DateOfBirthRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class InValidDateOfBirth : LeafRule
    {
        #region Event Handlers
        public event EventHandler InValidDateOfBirthEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            InValidDateOfBirthEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            InValidDateOfBirthEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.InValidDateOfBirthEvent = null;
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
            bool result = true;

            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }

            Account anAccount = context as Account;
            if( anAccount == null )
            {
                return false;
            }
            if( anAccount.Patient != null &&
                !anAccount.Patient.DateOfBirth.Equals( DateTime.MinValue ) &&
                anAccount.Patient.DateOfBirth < earliestDateOfBirth )
            {
                if( this.FireEvents && InValidDateOfBirthEvent != null )
                {
                    InValidDateOfBirthEvent( this, null );
                }
                result = false;
            }
            return result;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public InValidDateOfBirth()
        {
        }
        #endregion

        #region Data Elements
        private DateTime earliestDateOfBirth = new DateTime( 1800, 01, 01 );
        #endregion

        #region Constants
        #endregion
    }
}
