using System;
using PatientAccess.Domain.Parties.Exceptions;

namespace PatientAccess.Domain.Parties
{
	/// <summary>
	/// Summary description for OperatingPhysician.
	/// </summary>
	//TODO: Create XML summary comment for OperatingPhysician
	[Serializable]
	public class OperatingPhysician : PhysicianRole
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
			return new PhysicianRole( OPERATING, OPERATINGNAME );
		}

		public override void ValidateFor( Account selectedAccount, Physician physician )
		{
			this.aPhysician = physician;
			ValidateIfPhysicianIsActive( );
			ValidatePatientTypesForOptional(selectedAccount);
		}
		#endregion

		#region Properties
		#endregion

		#region Private Methods
		private void ValidateIfPhysicianIsActive( )
		{
			if( aPhysician.ActiveInactiveFlag != ACTIVE )
			{
				ThrowAnException();
			}
		}

        private string[] GetPatientTypesForOptional()
        {
            string[] types = {
                string.Empty,
                VisitType.PREREG_PATIENT,
                VisitType.INPATIENT,
                VisitType.OUTPATIENT,
                VisitType.EMERGENCY_PATIENT,
                VisitType.RECURRING_PATIENT ,
                VisitType.NON_PATIENT };
            return types;
        }

        private void ValidatePatientTypesForOptional(Account selectedAccount)
        {
            bool aIsValid = false;
            string[] aPatientTypes = GetPatientTypesForOptional();
            string aPatientTypeCode = selectedAccount.KindOfVisit.Code;
			
            for( int i = 0; i < aPatientTypes.Length; i++ )
            {
                if( aPatientTypeCode == aPatientTypes[i] )
                {
                    if( ( aPatientTypeCode == VisitType.EMERGENCY_PATIENT || aPatientTypeCode == VisitType.OUTPATIENT )
                        && selectedAccount.FinancialClass != null
                        && selectedAccount.FinancialClass.Code == FinancialClass.MED_SCREEN_EXM_CODE)
                    {
                        aIsValid = false;
                    }
                    else
                    {
                        aIsValid = true;
                    }
                    break;
                }
            }

            if( !aIsValid )
            {
                ThrowAnException( );
            }
        }

		private void ThrowAnException( )
		{
			InvalidPhysicianAssignmentException aException = 
				new InvalidPhysicianAssignmentException();
			
			aException.PhysicianName = aPhysician.Name.DisplayString;
			aException.PhysicianNumber = aPhysician.PhysicianNumber;
			aException.RelationshipType = OPERATINGNAME;
			/*
			aException.Message = "The physician is invaliad for the specified physician relationship type int this " +
				"activity. Either select a different physician for the physician relationship type, " +
				"or record a nonstaff physician.";
			*/
			throw aException;
		}
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public OperatingPhysician()
            : base(OPERATING, OPERATINGNAME)
		{ 
		}
		#endregion

		#region Data Elements
		private Physician aPhysician = null;
		#endregion

		#region Constants
		#endregion
	}
}
