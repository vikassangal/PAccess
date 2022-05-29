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
    public class DateOfBirthRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler DateOfBirthRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            DateOfBirthRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            DateOfBirthRequiredEvent -= eventHandler;
            return true;
        }
        
        public override void UnregisterHandlers()
        {
            this.DateOfBirthRequiredEvent = null;   
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
                anAccount.Patient.DateOfBirth.Equals( DateTime.MinValue ) )
            {
                if( this.FireEvents && DateOfBirthRequiredEvent != null )
                {
                    DateOfBirthRequiredEvent( this, null );
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
        public DateOfBirthRequired()
        {
        }
        #endregion

        #region Data Elements
        
        #endregion

        #region Constants
        #endregion
    }
}
