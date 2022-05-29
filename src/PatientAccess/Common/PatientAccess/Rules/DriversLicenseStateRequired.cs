using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for DriversLicenseStateRequired.
    /// </summary>
    //TODO: Create XML summary comment for DriversLicenseStateRequired
    [Serializable]
    [UsedImplicitly]
    public class DriversLicenseStateRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler DriversLicenseStateRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            DriversLicenseStateRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            DriversLicenseStateRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.DriversLicenseStateRequiredEvent = null;   
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
            if( anAccount == null 
                || anAccount.Patient == null
                || anAccount.Patient.DriversLicense == null )
            {
                result = false;
            }
            else
            {

                // If a driver's license number is entered, a state must be selected
                if( anAccount.Patient.DriversLicense.Number != String.Empty &&
                    ( anAccount.Patient.DriversLicense.State == null 
                    || anAccount.Patient.DriversLicense.State.Description.Trim().Length <= 0))
                {
                    result = false;
                }
            }

            if( !result 
                && this.FireEvents && DriversLicenseStateRequiredEvent != null )
            {
                DriversLicenseStateRequiredEvent( this, null );
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
        public DriversLicenseStateRequired()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}

