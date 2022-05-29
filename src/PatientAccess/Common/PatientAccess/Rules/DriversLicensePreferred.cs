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
    public class DriversLicensePreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler DriversLicensePreferredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler( EventHandler eventHandler )
        {
			DriversLicensePreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler( EventHandler eventHandler )
        {
            DriversLicensePreferredEvent -= eventHandler;
            return true;
        }
                
        public override void UnregisterHandlers()
        {
            this.DriversLicensePreferredEvent = null;   
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
			

			if( result == true && anAccount.Patient.DriversLicense.Number == String.Empty  )
			{                
				result = false;
				
				string age = anAccount.Patient.Age();
				if( age == String.Empty || !age.EndsWith( "y" ) )
				{
					result = true;
				}
				else
				{
					string years = age.Remove( age.Length - 1, 1 );

					if( Convert.ToInt32( years ) < 18 )
					{
						result = true;
					}
					else
					{
						result = false;

					}
				}
			}
			
            if( !result && this.FireEvents && DriversLicensePreferredEvent != null )
            {
                DriversLicensePreferredEvent( this, null );
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
        public DriversLicensePreferred()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}

