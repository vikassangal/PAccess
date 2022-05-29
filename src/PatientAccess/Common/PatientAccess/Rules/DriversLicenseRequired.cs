using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for DriversLicenseRequired.
    /// </summary>
    //TODO: Create XML summary comment for DriversLicenseRequired
    [Serializable]
    [UsedImplicitly]
    public class DriversLicenseRequired : LeafRule
    {
        #region Event Handlers
        public event EventHandler DriversLicenseRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
            DriversLicenseRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            DriversLicenseRequiredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.DriversLicenseRequiredEvent = null;   
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

                if( anAccount.Patient.DriversLicense.Number == String.Empty  )
                {                
                    result = false;
                }
            }

            if( !result && this.FireEvents && DriversLicenseRequiredEvent != null )
            {
                DriversLicenseRequiredEvent( this, null );
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
        public DriversLicenseRequired()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}

