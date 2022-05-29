using System;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.Parties.Exceptions;
using PatientAccess.Rules;

namespace PatientAccess.UI.PhysicianSearchViews
{
	/// <summary>
	/// Summary description for PhysicianService.
	/// </summary>
	//TODO: Create XML summary comment for PhysicianService
	[Serializable]
	public class PhysicianService : object
	{
		#region Event Handlers
		#endregion

		#region Methods
        public static void SearchAndAssignPhysician( long physicianNumber,
            PhysicianRole aRole,
            Account selectedAccount )
        {
            if( physicianNumber != NON_STAFF_PHYSICIAN_NUMBER )
            {
                IPhysicianBroker physicianBroker = 
                BrokerFactory.BrokerOfType<IPhysicianBroker>();
                Physician aPhysician = physicianBroker.VerifyPhysicianWith( selectedAccount.Facility.Oid, 
                    physicianNumber );
                if( aPhysician == null || aPhysician.PhysicianNumber == 0 )
                {
                    throw new PhysicianNotFoundException( physicianNumber,
                        aRole.Role().Description );
                }

                aRole.ValidateFor( selectedAccount, aPhysician );

                PhysicianRelationship aRelationship = new PhysicianRelationship( aRole, aPhysician );
                selectedAccount.AddPhysicianRelationship( aRelationship );			
            }
        }

		public static void AssignPhysician( Physician aPhysician,
											PhysicianRole aRole,
											Account selectedAccount )
		{
			try
			{
				if( aPhysician.PhysicianNumber != NON_STAFF_PHYSICIAN_NUMBER )
				{
					aRole.ValidateFor( selectedAccount, aPhysician );
				}

				PhysicianRelationship aRelationship = new PhysicianRelationship( aRole, aPhysician );
                selectedAccount.RemovePhysicianRelationship( aRelationship );
				selectedAccount.AddPhysicianRelationship( aRelationship );
			}
            catch (Exception)
            {
                throw;
            }
            finally
            {
            }
		}

        public static bool VerifyPhysicians( Account Model, string ReferringPhysicianId, string AdmittingPhysicianId , 
            string AttendingPhysicianId, string OperatingPhysicianId, string PrimaryCarePhysicianId )
        {
            bool result = false;
        
            string errorMsg  = "The following physician(s) could not be recorded for this patient:";
            string errorMsg2 = "\n\nEither select a different physician for the physician relationship type, or\nrecord a nonstaff physician.";
            string notFoundReason = "                Reason: The physician number does not match any records in\n                the system.";
            string invalidReason  = "                Reason: The physician is invalid for the specified physician\n                relationship type in this activity."; 
            bool errOccur = false;

            //            int VerifyResult;
    
            if( ReferringPhysicianId != "" && ReferringPhysicianId != Physician.NEW_OID.ToString() )
            {
                try
                {
                    SearchAndAssignPhysician( Convert.ToInt64( ReferringPhysicianId ), 
                        PhysicianRole.Referring(),
                        Model );  

                }
                catch(  PhysicianNotFoundException )
                {
                    errOccur = true;
                    errorMsg += "\n\n        Ref: " + ReferringPhysicianId + "\n\n" + notFoundReason;                    
                }
                catch( InvalidPhysicianAssignmentException )
                {
                    errOccur = true;
                    errorMsg += "\n\n        Ref: " + ReferringPhysicianId + "\n\n" + invalidReason;                                                  
                }
            }            
            
            if( AdmittingPhysicianId != "" && AdmittingPhysicianId != Physician.NEW_OID.ToString() )
            {
                try
                {
                    SearchAndAssignPhysician( Convert.ToInt64( AdmittingPhysicianId ), 
                        PhysicianRole.Admitting(),
                        Model );  
                
                }
                catch(  PhysicianNotFoundException )
                {
                    errOccur = true;
                    errorMsg += "\n\n        Adm: " + AdmittingPhysicianId + "\n\n" + notFoundReason;                    
                }
                catch( InvalidPhysicianAssignmentException )
                {
                    errOccur = true;
                    errorMsg += "\n\n        Adm: " + AdmittingPhysicianId + "\n\n" + invalidReason;                                                  
                }
            }
            
            if( AttendingPhysicianId != "" && AttendingPhysicianId != Physician.NEW_OID.ToString() )
            {
                try
                {
                    SearchAndAssignPhysician( Convert.ToInt64(AttendingPhysicianId), 
                        PhysicianRole.Attending(),
                        Model );  
                }
                catch(  PhysicianNotFoundException )
                {
                    errOccur = true;
                    errorMsg += "\n\n        Att: " + AttendingPhysicianId + "\n\n" + notFoundReason;                    
                }
                catch( InvalidPhysicianAssignmentException )
                {
                    errOccur = true;                 
                    errorMsg += "\n\n        Att: " + AttendingPhysicianId + "\n\n" + invalidReason;                                                  
                }
            }

            if( OperatingPhysicianId != "" && OperatingPhysicianId != Physician.NEW_OID.ToString() )
            {
                try
                {
                    SearchAndAssignPhysician( Convert.ToInt64( OperatingPhysicianId ), 
                        PhysicianRole.Operating(),
                        Model );  

                }
                catch(  PhysicianNotFoundException )
                {
                    errOccur = true;
                    errorMsg += "\n\n        Opr: " + OperatingPhysicianId + "\n\n" + notFoundReason;                    
                }
                catch( InvalidPhysicianAssignmentException )
                {
                    errOccur = true;
                    errorMsg += "\n\n        Opr: " + OperatingPhysicianId + "\n\n" + invalidReason;                                                  
                }
            }

            if( PrimaryCarePhysicianId != "" && PrimaryCarePhysicianId != Physician.NEW_OID.ToString() )
            {
                try
                {
                    SearchAndAssignPhysician( Convert.ToInt64( PrimaryCarePhysicianId ), 
                        PhysicianRole.PrimaryCare(),
                        Model );  

                }
                catch(  PhysicianNotFoundException )
                {
                    errOccur = true;
                    errorMsg += PrimaryCarePhysicianException( Model.AccountCreatedDate, PrimaryCarePhysicianId, notFoundReason); 
                }
                catch( InvalidPhysicianAssignmentException )
                {
                    errOccur = true;
                    errorMsg += PrimaryCarePhysicianException(Model.AccountCreatedDate, PrimaryCarePhysicianId, invalidReason);                                               
                }
            }

            if( errOccur )
            {
                errorMsg += errorMsg2;

                MessageBox.Show( errorMsg, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
            }

            if( errOccur != true )
            {
                result = true;
            }
            return result;
        }

        public static bool VerifyPhysicians( Account Model, string ReferringPhysicianId, string AdmittingPhysicianId  )
        {
            bool result = false;

            string errorMsg = "The following physician(s) could not be recorded for this patient:";
            string errorMsg2 = "\n\nEither select a different physician for the physician relationship type, or\nrecord a nonstaff physician.";
            string notFoundReason = "                Reason: The physician number does not match any records in\n                the system.";
            string invalidReason = "                Reason: The physician is invalid for the specified physician\n                relationship type in this activity.";
            bool errOccur = false;

            //            int VerifyResult;

            if ( ReferringPhysicianId != "" && ReferringPhysicianId != Physician.NEW_OID.ToString() )
            {
                try
                {
                    SearchAndAssignPhysician( Convert.ToInt64( ReferringPhysicianId ),
                        PhysicianRole.Referring(),
                        Model );

                }
                catch ( PhysicianNotFoundException )
                {
                    errOccur = true;
                    errorMsg += "\n\n        Ref: " + ReferringPhysicianId + "\n\n" + notFoundReason;
                }
                catch ( InvalidPhysicianAssignmentException )
                {
                    errOccur = true;
                    errorMsg += "\n\n        Ref: " + ReferringPhysicianId + "\n\n" + invalidReason;
                }
            }

            if ( AdmittingPhysicianId != "" && AdmittingPhysicianId != Physician.NEW_OID.ToString() )
            {
                try
                {
                    SearchAndAssignPhysician( Convert.ToInt64( AdmittingPhysicianId ),
                        PhysicianRole.Admitting(),
                        Model );

                }
                catch ( PhysicianNotFoundException )
                {
                    errOccur = true;
                    errorMsg += "\n\n        Adm: " + AdmittingPhysicianId + "\n\n" + notFoundReason;
                }
                catch ( InvalidPhysicianAssignmentException )
                {
                    errOccur = true;
                    errorMsg += "\n\n        Adm: " + AdmittingPhysicianId + "\n\n" + invalidReason;
                }
            }

            if ( errOccur )
            {
                errorMsg += errorMsg2;

                MessageBox.Show( errorMsg, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
            }

            if ( errOccur != true )
            {
                result = true;
            }
            return result;
        }
        public static bool VerifyPhysicians( Account Model, string ReferringPhysicianId, string AdmittingPhysicianId,
            string AttendingPhysicianId, string PrimaryCarePhysicianId )
        {
            bool result = false;

            string errorMsg = "The following physician(s) could not be recorded for this patient:";
            string errorMsg2 = "\n\nEither select a different physician for the physician relationship type, or\nrecord a nonstaff physician.";
            string notFoundReason = "                Reason: The physician number does not match any records in\n                the system.";
            string invalidReason = "                Reason: The physician is invalid for the specified physician\n                relationship type in this activity.";
            bool errOccur = false;

            if ( ReferringPhysicianId != "" && ReferringPhysicianId != Physician.NEW_OID.ToString() )
            {
                try
                {
                    SearchAndAssignPhysician( Convert.ToInt64( ReferringPhysicianId ),
                        PhysicianRole.Referring(),
                        Model );

                }
                catch ( PhysicianNotFoundException )
                {
                    errOccur = true;
                    errorMsg += "\n\n        Ref: " + ReferringPhysicianId + "\n\n" + notFoundReason;
                }
                catch ( InvalidPhysicianAssignmentException )
                {
                    errOccur = true;
                    errorMsg += "\n\n        Ref: " + ReferringPhysicianId + "\n\n" + invalidReason;
                }
            }

            if ( AdmittingPhysicianId != "" && AdmittingPhysicianId != Physician.NEW_OID.ToString() )
            {
                try
                {
                    SearchAndAssignPhysician( Convert.ToInt64( AdmittingPhysicianId ),
                        PhysicianRole.Admitting(),
                        Model );

                }
                catch ( PhysicianNotFoundException )
                {
                    errOccur = true;
                    errorMsg += "\n\n        Adm: " + AdmittingPhysicianId + "\n\n" + notFoundReason;
                }
                catch ( InvalidPhysicianAssignmentException )
                {
                    errOccur = true;
                    errorMsg += "\n\n        Adm: " + AdmittingPhysicianId + "\n\n" + invalidReason;
                }
            }

            if ( AttendingPhysicianId != "" && AttendingPhysicianId != Physician.NEW_OID.ToString() )
            {
                try
                {
                    SearchAndAssignPhysician( Convert.ToInt64( AttendingPhysicianId ),
                        PhysicianRole.Attending(),
                        Model );
                }
                catch ( PhysicianNotFoundException )
                {
                    errOccur = true;
                    errorMsg += "\n\n        Att: " + AttendingPhysicianId + "\n\n" + notFoundReason;
                }
                catch ( InvalidPhysicianAssignmentException )
                {
                    errOccur = true;
                    errorMsg += "\n\n        Att: " + AttendingPhysicianId + "\n\n" + invalidReason;
                }
            }

            if ( PrimaryCarePhysicianId != "" && PrimaryCarePhysicianId != Physician.NEW_OID.ToString() )
            {
                try
                {
                    SearchAndAssignPhysician( Convert.ToInt64( PrimaryCarePhysicianId ),
                        PhysicianRole.PrimaryCare(),
                        Model );

                }
                catch ( PhysicianNotFoundException )
                {
                    errOccur = true;
                    errorMsg += PrimaryCarePhysicianException( Model.AccountCreatedDate, PrimaryCarePhysicianId, notFoundReason );
                }
                catch ( InvalidPhysicianAssignmentException )
                {
                    errOccur = true;
                    errorMsg += PrimaryCarePhysicianException( Model.AccountCreatedDate, PrimaryCarePhysicianId, invalidReason );
                }
            }

            if ( errOccur )
            {
                errorMsg += errorMsg2;

                MessageBox.Show( errorMsg, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
            }

            if ( errOccur != true )
            {
                result = true;
            }
            return result;
        }

	    private static string  PrimaryCarePhysicianException(DateTime accountCreatedDate, string physicianID, string reason)
	    {
            var primaryCarePhysicianFeatureManager = new PrimaryCarePhysicianFeatureManager();
            if (primaryCarePhysicianFeatureManager.IsPrimaryCarePhysicianEnabledForDate(accountCreatedDate))
            {
                return "\n\n        " + PhysicianRole.PRIMARYCAREPHYSICIAN_LABEL + physicianID + "\n\n" + reason; 
            }
            else
            {
                return "\n\n         " + PhysicianRole.OTHERPHYSICIAN_LABEL + physicianID + "\n\n" + reason; 
            }
                   
	    }
		#endregion

		#region Properties
		#endregion

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public PhysicianService()
		{
		}
		#endregion

		#region Data Elements
		#endregion

		#region Constants
		private const long
			NON_STAFF_PHYSICIAN_NUMBER = 8888L;
		#endregion
	}
}
