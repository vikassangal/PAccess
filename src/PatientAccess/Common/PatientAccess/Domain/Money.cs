using System;

namespace PatientAccess.Domain
{
//TODO: Create XML summary comment for Money
	[Serializable]
	public class Money : Object
	{
		#region Event Handlers
		#endregion

		#region Methods
		public Money Add( Money money )
		{
			return this;
		}

		public Money Add( decimal anAmount )
		{
			return this;
		}

		public Money Subtract( Money money )
		{
			return this;
		}

		public Money Subtract( decimal anAmount )
		{
			return this;
		}

		public Money Multiply( Money money )
		{
			return this;
		}

		public Money Mutliply( decimal anAmount )
		{
			return this;
		}

		public Money Divide( Money money )
		{
			return this;
		}

		public Money Divide( decimal anAmount )
		{
			return this;
		}

		public Money AsCurrency( Currency aCurrency )
		{
			return this;
		}

		public Money AsPercentage( decimal aPercentage )
		{
			return this;
		}

		#endregion

		#region Properties
		public decimal Amount
		{
			get
			{
				return i_Amount;
			}
		    private set
			{
				i_Amount = value;
			}
		}

	    private Currency MyCurrency
		{
			get
			{
				return i_Currency;
			}
			set
			{
				i_Currency = value;
			}
		}
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public Money()
		{
			this.MyCurrency = Currency.USDollar();
		}

		public Money( decimal anAmount )
		{
			this.MyCurrency = Currency.USDollar();
			this.Amount = anAmount;
		}
		#endregion

		#region Data Elements
		private Currency i_Currency;
		private decimal i_Amount;
		#endregion

		#region Constants
		#endregion
	}
}
