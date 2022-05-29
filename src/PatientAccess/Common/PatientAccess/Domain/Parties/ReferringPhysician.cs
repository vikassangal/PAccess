using System;

namespace PatientAccess.Domain.Parties
{
	/// <summary>
	/// Summary description for ReferringPhysician.
	/// </summary>
	//TODO: Create XML summary comment for ReferringPhysician
	[Serializable]
	public class ReferringPhysician : PhysicianRole
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
			//return new PhysicianRole( PhysicianRole.PhysiciansRole.REFERRING, "Referring" );
			return new PhysicianRole( REFERRING,REFERRINGNAME );
		}

		public override void ValidateFor( Account selectedAccount, Physician physician )
		{
			this.aPhysician = physician;
			ValidateMandatoryPatientTypes( selectedAccount );
		}
		#endregion

		#region Properties
		#endregion

		#region Private Methods
		private void ValidateMandatoryPatientTypes(Account selectedAccount)
		{
			bool aIsValiad = false;
			string[] aPatientTypes = GetMandatoryPatientTypes();
            string aPatientTypeCode = String.Empty;
            if( selectedAccount != null && selectedAccount.KindOfVisit != null )
            {
                aPatientTypeCode = selectedAccount.KindOfVisit.Code;
            }
		
	        // An exception occurs in this method. 
            // It could be that selectedAccount is somehow null or that its fields are null, in either case
            // the assignment above - string aPatientTypeCode - will be set to null causing 
            // the code below to never set aIsValid to true (only under another buggy condition) thereby triggering 
            // a throw exception. Alternatively, the Code in the KindOfVisit of selectedAccount contains a 
            // a code that does not match any mandatory patient types, if this is the case that's a deeper bug,
            // since the matching search in the for loop below will never find a match so it triggers an exception.
            //
            // DEFECT 34927: Could add a null check and allow the exception to trigger if true, in the normal way.
            //
            // Don't think this is the real cause of the bug, though - under what circumstances could this be null,
            // if not a background worker issue?
            // If you trace this bug it's connected with AccountView; it looks more and more like a 
            // background worker-related issue, but just to be safe.

            if ( aPatientTypeCode != null )
            {
                for (int i = 0; i < aPatientTypes.Length; i++)
                {
                    if (aPatientTypeCode == aPatientTypes[i])
                    {
                        aIsValiad = true;
                        break;
                    }
                }
            }

			if( !aIsValiad )
			{
				ThrowAnExcption( );
			}
		}

		private string[] GetMandatoryPatientTypes()
		{
			string[] types = {
                VisitType.PREREG_PATIENT,
                VisitType.INPATIENT,
                VisitType.OUTPATIENT,
                VisitType.EMERGENCY_PATIENT,
                VisitType.RECURRING_PATIENT ,
                VisitType.NON_PATIENT };
			return types;
		}
		
		private void ThrowAnExcption( )
		{
			Exceptions.InvalidPhysicianAssignmentException aException = 
				new Exceptions.InvalidPhysicianAssignmentException();
			
			aException.PhysicianName = aPhysician.Name.DisplayString;
			aException.PhysicianNumber = aPhysician.PhysicianNumber;
			aException.RelationshipType = REFERRINGNAME;
			
//			aException.Message = UIErrorMessages.INVALID_PHYSICIAN_ERRMSG;
			
			throw aException;
		}
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public ReferringPhysician()
            : base(REFERRING, REFERRINGNAME)
		{
		}
		#endregion

		#region Data Elements
		Physician aPhysician = null;
		#endregion

		#region Constants
		#endregion
	}
}
