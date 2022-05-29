using System;

namespace PatientAccess.Domain
{
	//TODO: Create XML summary comment for Currency
	[Serializable]
	public class Currency : CodedReferenceValue
	{
		#region Event Handlers
		#endregion

		#region Methods
		public static Currency USDollar()
		{
			return new Currency( US_DOLLAR_OID, "US Dollar", 1.00f );
		}

		public static Currency CanadianDollar()
		{
			return new Currency( CANADIAN_DOLLAR_OID, "Canadian Dollar", 1.65f );
		}

		public static Currency BritishPound()
		{
			return new Currency( BRITISH_POUND_OID, "British Pound", 0.70f );
		}		

		#endregion

		#region Properties
		public float ExchangeRate
		{
			get
			{
				return i_ExchangeRate;
			}
		    private set
			{
				i_ExchangeRate = value;
			}
		}
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public Currency( float exchangeRate )
		{
			this.ExchangeRate = exchangeRate;
		}

		private Currency( long oid, string description, float exchangeRate )
			: base( oid, description )
		{
			this.ExchangeRate = exchangeRate;
		}

		public Currency( long oid, string description, string code, float exchangeRate )
			: base( oid, description, code )
		{
			this.ExchangeRate = exchangeRate;
		}

		public Currency( long oid, DateTime version, string code, float exchangeRate )
			: base( oid, version, code )
		{
			this.ExchangeRate = exchangeRate;
		}

		public Currency( long oid, DateTime version, string description, string code, float exchangeRate )
			: base( oid, version, description, code )
		{
			this.ExchangeRate = exchangeRate;
		}
		#endregion

		#region Data Elements
		private float i_ExchangeRate = 1.00f;
		#endregion

		#region Constants
		private const long
			US_DOLLAR_OID = 0,
			CANADIAN_DOLLAR_OID = 1,
			BRITISH_POUND_OID = 2;
		#endregion
	}
}
