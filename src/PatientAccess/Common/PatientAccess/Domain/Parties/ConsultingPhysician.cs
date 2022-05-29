using System;

namespace PatientAccess.Domain.Parties
{
	/// <summary>
	/// Summary description for ConsultingPhysician.
	/// </summary>
	//TODO: Create XML summary comment for ConsultingPhysician
	[Serializable]
	public class ConsultingPhysician : PhysicianRole
	{
		#region Event Handlers
		#endregion

		#region Methods
		public override bool IsValidFor( Account selectedAccount, Physician physician )
		{
			return true;
		}

		public override PhysicianRole Role()
		{
			return new PhysicianRole( CONSULTING,CONSULTINGNAME );
		}

		public override void ValidateFor( Account selectedAccount, Physician physician )
		{
			//Apply physician selection rules for this role based on physician
			//and account related information.  If a rule fails validation,
			//throw an ArgumentException.
		}
		#endregion

		#region Properties
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public ConsultingPhysician()
            : base(CONSULTING, CONSULTINGNAME)
		{
		}
		#endregion

		#region Data Elements
		#endregion

		#region Constants
		#endregion
	}
}
