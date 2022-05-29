using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;

namespace PatientAccess.Rules
{
	/// <summary>
	/// Summary description for AssignBedRequired.
	/// </summary>
	//TODO: Create XML summary comment for AssignBedRequired
	[Serializable]
	[UsedImplicitly]
    public class AssignBedRequired : LeafRule
	{
		#region Event Handlers
		#endregion

		#region Methods
		public override bool RegisterHandler(EventHandler eventHandler)
		{
			return true;
		}

		public override bool UnregisterHandler(EventHandler eventHandler)
		{
			return true;
		}
                                
        public override void UnregisterHandlers()
        {
        }

		public override bool CanBeAppliedTo(object context)
		{
            return true;
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
	    #endregion

		#region Data Elements
		#endregion
	}
}
