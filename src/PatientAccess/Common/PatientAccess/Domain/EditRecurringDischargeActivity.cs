using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for EditRecurringDischargeActivity.
	/// </summary>
	//TODO: Create XML summary comment for EditRecurringDischargeActivity
	[Serializable]
	public class EditRecurringDischargeActivity : Activity
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
		public EditRecurringDischargeActivity()
		{
            this.Description        = "Edit Recurring &Outpatient Discharge Date";
            this.ContextDescription      = "Edit Recurring Outpatient Discharge Date";
		}
		#endregion

		#region Data Elements
		#endregion

		#region Constants
		#endregion
	}
}

