using System;

namespace PatientAccess.Domain
{
//TODO: Create XML summary comment for TransferActivity
	[Serializable]
	public class TransferActivity : Activity
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
		public TransferActivity()
		{
            this.Description    = "Transfer Patient to &New Location";
            this.ContextDescription  = "Transfer Patient to New Location";
		}
		#endregion

		#region Data Elements
		#endregion

		#region Constants
		#endregion
	}
}
