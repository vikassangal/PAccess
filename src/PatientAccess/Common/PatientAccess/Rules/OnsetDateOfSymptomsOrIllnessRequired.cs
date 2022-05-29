using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for OnsetDateOfSymptomsOrIllnessRequired.
	/// </summary>
	//TODO: Create XML summary comment for OnsetDateOfSymptomsOrIllnessRequired
	[Serializable]
	[UsedImplicitly]
    public class OnsetDateOfSymptomsOrIllnessRequired : LeafRule
	{
		#region Event Handlers
		public event EventHandler OnsetDateOfSymptomsOrIllnessRequiredEvent;
		#endregion

		#region Methods
		public override bool RegisterHandler(EventHandler eventHandler)
		{
			this.OnsetDateOfSymptomsOrIllnessRequiredEvent += eventHandler;
			return true;
		}

		public override bool UnregisterHandler(EventHandler eventHandler)
		{
			this.OnsetDateOfSymptomsOrIllnessRequiredEvent -= eventHandler;
			return true;
		}
                
        public override void UnregisterHandlers()
        {
            this.OnsetDateOfSymptomsOrIllnessRequiredEvent = null;
        }

		public override bool CanBeAppliedTo(object context)
		{
            if( context == null || context.GetType() != typeof( Account ) )
            {
                return true;
            } 

            Account account = (Account)context;
			if( account.Diagnosis.Condition.GetType().Equals( typeof( Accident ) ) ||
				account.Diagnosis.Condition.GetType().Equals( typeof( Crime ) ) )
			{
				return true;
			}
			else
			{                  
                if( !HasOCC11( account ) )
                {
                    if( OnsetDateOfSymptomsOrIllnessRequiredEvent != null )
                    {
                        this.OnsetDateOfSymptomsOrIllnessRequiredEvent(this, null);
                    }
    
                    return false;
                }
                else
                {
                    return true;
                }			
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
        private bool HasOCC11( Account account )
        {
            bool result = false;
            foreach( OccurrenceCode occ in account.OccurrenceCodes )
            {
                if( occ.Code == "11" )
                {
                    result = true;
                    break;
                }
            } 
            return result;
        }
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public OnsetDateOfSymptomsOrIllnessRequired()
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
