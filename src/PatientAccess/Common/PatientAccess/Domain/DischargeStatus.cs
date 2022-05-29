using System;

namespace PatientAccess.Domain
{
	[Serializable]
	public class DischargeStatus : CodedReferenceValue
	{
		#region Event Handlers
		#endregion

		#region Methods
		public static DischargeStatus NotDischarged()
		{
			return new DischargeStatus( NOT_DISCHARGED_OID, "NOTDISCHARGED" );
		}

		public static DischargeStatus PendingDischarge()
		{
			return new DischargeStatus( PENDING_DISCHARGE_OID, "PENDINGDISCHARGE" );
		}

		public static DischargeStatus Discharged()
		{
			return new DischargeStatus( DISCHARGED_OID, "DISCHARGED" );
		}
		#endregion

		#region Properties
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public DischargeStatus()
		{
		}

		public DischargeStatus( long oid, string description )
			: base( oid, description )
		{
		}

		public DischargeStatus( long oid, string description, string code )
			: base( oid, description, code )
		{
		}

		public DischargeStatus( long oid, DateTime version, string code )
			: base( oid, version, code )
		{
		}

		public DischargeStatus( long oid, DateTime version, string description, string code )
			: base( oid, version, description, code )
		{
		}
		#endregion

		#region Data Elements
		#endregion

		#region Constants
		public const long
			NOT_DISCHARGED_OID = 0,
			PENDING_DISCHARGE_OID = 1,
			DISCHARGED_OID  = 2;
		#endregion
	}
}
