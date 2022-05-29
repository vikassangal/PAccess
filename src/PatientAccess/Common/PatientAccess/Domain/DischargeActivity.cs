using System;

namespace PatientAccess.Domain
{
//TODO: Create XML summary comment for DischargeActivity
	[Serializable]
	public class DischargeActivity : Activity
	{
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
		public DischargeActivity()
		{
            this.Description    = "&Discharge Patient";
            this.ContextDescription  = "Discharge Patient";
		}
		#endregion

		#region Data Elements
		#endregion

		#region Constants
		#endregion
	}
}
