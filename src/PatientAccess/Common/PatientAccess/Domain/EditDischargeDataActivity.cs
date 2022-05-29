using System;

namespace PatientAccess.Domain
{
//TODO: Create XML summary comment for EditDischargeDataActivity
	[Serializable]
	public class EditDischargeDataActivity : Activity
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
		public EditDischargeDataActivity()
		{
            this.Description    = "&Edit Inpatient Discharge Information";
            this.ContextDescription  = "Edit Inpatient Discharge Information";
		}
		#endregion

		#region Data Elements
		#endregion

		#region Constants
		#endregion
	}
}
