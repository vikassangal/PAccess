using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for HourOfAccidentOrCrimeRequired.
	/// </summary>
	//TODO: Create XML summary comment for HourOfAccidentOrCrimeRequired
	[Serializable]
	[UsedImplicitly]
    public class HourOfAccidentOrCrimeRequired : LeafRule
	{
		#region Event Handlers
		public event EventHandler HourOfAccidentOrCrimeRequiredEvent;
		#endregion

		#region Methods
		public override bool RegisterHandler(EventHandler eventHandler)
		{
			this.HourOfAccidentOrCrimeRequiredEvent += eventHandler;
			return true;
		}

		public override bool UnregisterHandler(EventHandler eventHandler)
		{
			this.HourOfAccidentOrCrimeRequiredEvent -= eventHandler;
			return true;
		}
                
        public override void UnregisterHandlers()
        {
            this.HourOfAccidentOrCrimeRequiredEvent = null;   
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
				if( condition.OccurredAtHour == null ||
					condition.OccurredAtHour == String.Empty )
				{
					if( this.FireEvents && HourOfAccidentOrCrimeRequiredEvent != null )
					{
						this.HourOfAccidentOrCrimeRequiredEvent(this, null);
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
		public HourOfAccidentOrCrimeRequired()
		{
		}
		#endregion

		#region Data Elements
		#endregion

		#region Constants
		#endregion
	}
}
