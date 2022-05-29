using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for CancelInpatientStatus.
	/// </summary>
	//TODO: Create XML summary comment for CancelInpatientStatus
	[Serializable]
	public class CancelInpatientStatusActivity : Activity
	{
		#region Constants
		#endregion

		#region Event Handlers
		#endregion

		#region Methods
		public override bool ReadOnlyAccount()
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
		public CancelInpatientStatusActivity()
		{
			this.Description    = "Cancel Inpatient &Status";
			this.ContextDescription  = "Cancel Inpatient Status";
		}
		#endregion

		#region Data Elements
		#endregion
	}
}
