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
    public class DriversLicenseStatePreferred : LeafRule
	{
		#region Event Handlers
		public event EventHandler DriversLicenseStatePreferredEvent;
		#endregion

		#region Methods
		public override bool RegisterHandler( EventHandler eventHandler )
		{
			DriversLicenseStatePreferredEvent += eventHandler;
			return true;
		}

		public override bool UnregisterHandler( EventHandler eventHandler )
		{
			DriversLicenseStatePreferredEvent -= eventHandler;
			return true;
		}
                
        public override void UnregisterHandlers()
        {
            this.DriversLicenseStatePreferredEvent = null;   
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
			
			if( result == true )
			{
				// If a driver's license number is entered, a state must be selected
				if( ( anAccount.Patient.DriversLicense.State == null 
					|| anAccount.Patient.DriversLicense.State.Description.Trim().Length <= 0) )
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

				if( anAccount.Activity.Equals( typeof( AdmitNewbornActivity ) ) )
				{
					return true;
				}
			}

			if( !result 
				&& this.FireEvents && DriversLicenseStatePreferredEvent != null )
			{
				DriversLicenseStatePreferredEvent( this, null );
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
		public DriversLicenseStatePreferred()
		{
		}
		#endregion

		#region Data Elements
		#endregion

		#region Constants
		#endregion
	}
}

