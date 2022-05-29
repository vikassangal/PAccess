using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for AdmitSourcePreferred.
	/// </summary>
	//TODO: Create XML summary comment for AdmitSourcePreferred
	[Serializable]
	[UsedImplicitly]
    public class AdmitSourcePreferred : LeafRule
	{
		#region Event Handlers
		public event EventHandler AdmitSourcePreferredEvent;
		#endregion

		#region Methods
		public override bool RegisterHandler(EventHandler eventHandler)
		{
			this.AdmitSourcePreferredEvent += eventHandler;
			return true;
		}

		public override bool UnregisterHandler(EventHandler eventHandler)
		{
			this.AdmitSourcePreferredEvent -= eventHandler;
			return true;
		}
                                
        public override void UnregisterHandlers()
        {
            this.AdmitSourcePreferredEvent = null;  
        }

		public override bool CanBeAppliedTo(object context)
		{
			if( ((Account)context).KindOfVisit.Code == VisitType.PREREG_PATIENT )
			{
				if( ((Account)context).AdmitSource == null 
					|| ((Account)context).AdmitSource.Code == null
					|| ((Account)context).AdmitSource.Code.Trim() == string.Empty )
				{
					if( AdmitSourcePreferredEvent != null )
					{
						this.AdmitSourcePreferredEvent(this, null);
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
				return false;
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
		public AdmitSourcePreferred()
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
