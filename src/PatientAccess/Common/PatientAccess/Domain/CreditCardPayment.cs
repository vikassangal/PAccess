using System;

namespace PatientAccess.Domain
{
//TODO: Create XML summary comment for CreditCardPayment
	[Serializable]
	public class CreditCardPayment : PaymentMethod
	{
		#region Event Handlers
		#endregion

		#region Methods
		#endregion

		#region Properties
		public string CardNumber
		{
			get
			{
				return i_CardNumber;
			}
			set
			{
				i_CardNumber = value;
			}
		}

		public DateTime ExpirationDate
		{
			get
			{
				return i_ExpirationDate;
			}
			set
			{
				i_ExpirationDate = value;
			}
		}

		public CreditCardProvider CardType
		{
			get
			{
				return i_CardType;
			}
		    private set
			{
				i_CardType = value;
			}
		}
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public CreditCardPayment()
		{
			this.CardType = new CreditCardProvider();
		}

		public CreditCardPayment( CreditCardProvider cardType )
		{
			this.CardType = cardType;
		}
		#endregion

		#region Data Elements
		private string i_CardNumber;
		private DateTime i_ExpirationDate;
		private CreditCardProvider i_CardType;
		#endregion

		#region Constants
		#endregion
	}
}
