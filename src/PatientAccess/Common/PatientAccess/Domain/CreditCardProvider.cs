using System;

namespace PatientAccess.Domain
{
//TODO: Create XML summary comment for CreditCardProvider
	[Serializable]
	public class CreditCardProvider : CodedReferenceValue
	{
		#region Event Handlers
		#endregion

		#region Methods
		public static CreditCardProvider AmericanExpress()
		{
			return new CreditCardProvider( AMEX_OID, "AMEX" );
		}

		public static CreditCardProvider Discover()
		{
			return new CreditCardProvider( DISCOVER_OID, "Discover" );
		}

		public static CreditCardProvider MasterCard()
		{
			return new CreditCardProvider( MASTERCARD_OID, "MasterCard" );
		}		

		public static CreditCardProvider Visa()
		{
			return new CreditCardProvider( VISA_OID, "Visa" );
		}		
		#endregion

		#region Properties
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties

		#endregion

		#region Construction and Finalization
		public CreditCardProvider()
		{
			this.Description = string.Empty;
		}

		private CreditCardProvider( long oid, string description )
			: base( oid, description )
		{
		}

		public CreditCardProvider( long oid, string description, string code )
			: base( oid, description, code )
		{
		}

		public CreditCardProvider( long oid, DateTime version, string code )
			: base( oid, version, code )
		{
		}

		public CreditCardProvider( long oid, DateTime version, string description, string code )
			: base( oid, version, description, code )
		{
		}
		#endregion

		#region Data Elements
		#endregion

		#region Constants

	    private const long
			AMEX_OID = 0,
	        DISCOVER_OID = 1,
	        VISA_OID = 3;

        public const long
            MASTERCARD_OID = 2;

	    #endregion
	}
}
