using System;

namespace PatientAccess.Domain
{
//TODO: Create XML summary comment for PaymentAmount
	[Serializable]
	public class PaymentAmount : Object
	{
		#region Event Handlers
		#endregion

		#region Methods
		public decimal AmountPaid()
		{
			return this.Money.Amount;
		}

		#endregion

		#region Properties
		public Money Money
		{
			get
			{
				return i_Money;
			}
			set
			{
				i_Money = value;
			}
		}

		public PaymentMethod PaymentMethod
		{
			get
			{
				return i_PaymentMethod;
			}
			set
			{
				i_PaymentMethod = value;
			}
		}
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public PaymentAmount()
		{
		}

		public PaymentAmount( decimal anAmount, PaymentMethod aPaymentMethod )
		{
			this.Money = new Money( anAmount );
			this.PaymentMethod = aPaymentMethod;
		}

		public PaymentAmount( Money money, PaymentMethod aPaymentMethod )
		{
			this.Money = money;
			this.PaymentMethod = aPaymentMethod;
		}
		#endregion

		#region Data Elements
		private Money i_Money  = new Money();
		private PaymentMethod i_PaymentMethod;
		#endregion

		#region Constants
		#endregion
	}
}
