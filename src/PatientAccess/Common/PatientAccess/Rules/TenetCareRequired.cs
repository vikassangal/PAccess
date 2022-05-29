using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for TenetCareRequired.
	/// </summary>
	//TODO: Create XML summary comment for TenetCareRequired
	[Serializable]
	[UsedImplicitly]
	public class TenetCareRequired : LeafRule
	{
		#region Event Handlers
		public event EventHandler TenetCareRequiredEvent;
		#endregion

		#region Methods
		public override bool RegisterHandler(EventHandler eventHandler)
		{
			this.TenetCareRequiredEvent += eventHandler;
			return true;
		}

		public override bool UnregisterHandler(EventHandler eventHandler)
		{
			this.TenetCareRequiredEvent -= eventHandler;
			return true;
		}

        public override void UnregisterHandlers()
        {
            this.TenetCareRequiredEvent = null;    
        }

		public override bool CanBeAppliedTo(object context)
		{
			if( ((Account)context).KindOfVisit != null &&
				((Account)context).KindOfVisit.Code != null &&
				((Account)context).KindOfVisit.Code.Trim() != string.Empty &&
				((Account)context).KindOfVisit.Code == VisitType.OUTPATIENT &&
				((Account)context).Facility != null &&
				((Account)context).Facility.TenetCare != null &&
				((Account)context).Facility.TenetCare.Code == YesNoFlag.CODE_YES &&
				( ((Account)context).TenetCare == null ||
				  ((Account)context).TenetCare.Code.Trim() == String.Empty ) ) 
			{
				if( TenetCareRequiredEvent != null )
				{
					this.TenetCareRequiredEvent(this, null);
				}
            
				return false;
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
		public TenetCareRequired()
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
