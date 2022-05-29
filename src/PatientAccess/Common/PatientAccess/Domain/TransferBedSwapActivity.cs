using System;

namespace PatientAccess.Domain
{
//TODO: Create XML summary comment for TransferBedSwapActivity
	[Serializable]
	public class TransferBedSwapActivity : Activity
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
		public TransferBedSwapActivity()
		{
            this.Description    = "&Swap Patient Locations";
            this.ContextDescription  = "Swap Patient Locations";
		}
		#endregion

		#region Data Elements
		#endregion

		#region Constants
		#endregion
	}
}
