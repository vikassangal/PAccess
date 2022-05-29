using System;

namespace PatientAccess.Domain
{
//TODO: Create XML summary comment for CheckPayment
	[Serializable]
	public class CheckPayment : PaymentMethod
	{
		#region Event Handlers
		#endregion

		#region Methods
		#endregion

		#region Properties
		public string CheckNumber
		{
			get
			{
				return i_CheckNumber;
			}
		    private set
			{
				i_CheckNumber = value;
			}
		}
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public CheckPayment()
		{
			this.CheckNumber = string.Empty;
		}

		public CheckPayment( string checkNumber )
		{
			this.CheckNumber = checkNumber;
		}
		#endregion

		#region Data Elements
		private string i_CheckNumber;
		#endregion

		#region Constants
		#endregion
	}
}
