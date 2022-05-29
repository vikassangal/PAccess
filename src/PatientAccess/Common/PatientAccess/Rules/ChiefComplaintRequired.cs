using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for ChiefComplaintRequired.
	/// </summary>
	//TODO: Create XML summary comment for ChiefComplaintRequired
	[Serializable]
	[UsedImplicitly]
    public class ChiefComplaintRequired : LeafRule
	{
		#region Event Handlers
		public event EventHandler ChiefComplaintRequiredEvent;
		#endregion

		#region Methods
		public override bool RegisterHandler(EventHandler eventHandler)
		{
			this.ChiefComplaintRequiredEvent += eventHandler;
			return true;
		}

		public override bool UnregisterHandler(EventHandler eventHandler)
		{
			this.ChiefComplaintRequiredEvent -= eventHandler;
			return true;
		}
                                   
        public override void UnregisterHandlers()
        {
            this.ChiefComplaintRequiredEvent = null;  
        }

		public override bool CanBeAppliedTo(object context)
		{
            if( context == null || 
                context.GetType() != typeof( Account ) )
            {                
                return true;
            } 			
            
            if( ((Account)context).Diagnosis.ChiefComplaint == null 
				|| ((Account)context).Diagnosis.ChiefComplaint.Trim() == string.Empty )
			{
				if( this.FireEvents && ChiefComplaintRequiredEvent != null )
				{
					this.ChiefComplaintRequiredEvent(this, null);
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
		public ChiefComplaintRequired()
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
