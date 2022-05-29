using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for CountryOfAccidentOrCrimeRequired.
	/// </summary>
	//TODO: Create XML summary comment for CountryOfAccidentOrCrimeRequired
	[Serializable]
	[UsedImplicitly]
    public class CountryOfAccidentOrCrimeRequired : LeafRule
	{
		#region Event Handlers
		public event EventHandler CountryOfAccidentOrCrimeRequiredEvent;
		#endregion

		#region Methods
		public override bool RegisterHandler(EventHandler eventHandler)
		{
			this.CountryOfAccidentOrCrimeRequiredEvent += eventHandler;
			return true;
		}

		public override bool UnregisterHandler(EventHandler eventHandler)
		{
			this.CountryOfAccidentOrCrimeRequiredEvent -= eventHandler;
			return true;
		}
                
        public override void UnregisterHandlers()
        {
            this.CountryOfAccidentOrCrimeRequiredEvent = null;   
        }

		public override bool CanBeAppliedTo(object context)
		{
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            }  			
            
            if( ( ((Account)context).Diagnosis.Condition.GetType().Name == "Accident"
				|| ((Account)context).Diagnosis.Condition.GetType().Name == "Crime" ) )
			{
				TimeAndLocationBoundCondition condition = (TimeAndLocationBoundCondition)((Account)context).Diagnosis.Condition;
				if( condition.Country == null || condition.Country.Code.Trim() == String.Empty )
				{
					if( this.FireEvents && CountryOfAccidentOrCrimeRequiredEvent != null )
					{
						this.CountryOfAccidentOrCrimeRequiredEvent(this, null);
					}
            
					return false;
				}
				else
				{
					return true;
				}
			}
			else
			{
				return true;
			}          
		}

		public override void ApplyTo(object context)
		{
            
		}

		public override bool ShouldStopProcessing()
		{
			return false;
		}

		#endregion

		#region Properties
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public CountryOfAccidentOrCrimeRequired()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		#endregion

		#region Data Elements
		#endregion

		#region Constants
		#endregion
	}
}
