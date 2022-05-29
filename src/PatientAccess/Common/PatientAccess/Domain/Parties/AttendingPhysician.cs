using System;
using PatientAccess.Domain.UCCRegistration;

namespace PatientAccess.Domain.Parties
{
    /// <summary>
    /// Summary description for AttendingPhysician.
    /// </summary>
    //TODO: Create XML summary comment for AttendingPhysician
    [Serializable]
    public class AttendingPhysician : PhysicianRole
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool IsValidFor(Account selectedAccount, Physician physician)
        {
            return true;
        }

        public override PhysicianRole Role()
        {
            return new PhysicianRole(ATTENDING, ATTENDINGNAME);
        }

        public override void ValidateFor(Account selectedAccount, Physician physician)
        {
            this.aPhysician = physician;

            bool isStatusValid = IsStatusValidFor(selectedAccount.KindOfVisit, selectedAccount.Activity, this.aPhysician);

            if (!isStatusValid)
            {
                this.ThrowAnExcption();
            }

            ValidateIfExcluded();

            ValidatePatientTypes(selectedAccount);
        }


        internal static bool IsStatusValidFor( VisitType visitKind, Activity activity, Physician physician )
        {
            bool isValid = false;
            string aPatientType = visitKind.Code;
            Type activityType = activity.GetType();
            
            if ( physician.ActiveInactiveFlag == ACTIVE )
            {
                isValid = true;
            }

            else
            {
                if (aPatientType == VisitType.EMERGENCY_PATIENT &&
                    (
                        activityType == typeof (RegistrationActivity) ||
                        activityType == typeof (PreMSERegisterActivity) ||
                        activityType == typeof (EditPreMseActivity) ||
                        activityType == typeof (MaintenanceActivity)
                        ) ||

                    activity.AssociatedActivityType == typeof (PreMSERegisterActivity) ||
                    activity.AssociatedActivityType == typeof (EditPreMseActivity) ||
                    activity.AssociatedActivityType == typeof (UCCPreMSERegistrationActivity) ||
                    activity.AssociatedActivityType == typeof (EditUCCPreMSEActivity)
                    )
                {
                    isValid = true;
                }
            }
            return isValid;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods

        private string[] GetMandatoryPatientTypes()
        {
            string[] types = {
                VisitType.INPATIENT,
                VisitType.OUTPATIENT,
                VisitType.EMERGENCY_PATIENT,
                VisitType.RECURRING_PATIENT ,
                VisitType.NON_PATIENT };
            return types;
        }


        private void ValidatePatientTypes(Account selectedAccount)
        {
            bool isValid = false;

            string aPatientTypeCode = selectedAccount.KindOfVisit.Code;

            if ( selectedAccount.Activity.IsTransferOutToInActivity() ||
                selectedAccount.Activity.IsTransferERToOutpatientActivity() ||
                selectedAccount.Activity.IsTransferOutpatientToERActivity() )
            {
                isValid = true;
            }
            else
            {
                if (selectedAccount.HospitalService != null && selectedAccount.HospitalService.DayCare == "Y")
                {
                    isValid = IsValidPatientType(GetMandatoryPatientTypes(), aPatientTypeCode);
                }
                else
                {
                    isValid = true;
                }
            }

            if (!isValid)
            {
                ThrowAnExcption();
            }
        }

        private bool IsValidPatientType(string[] aPatientTypes, string aPatientTypeCode)
        {
            bool aIsValiad = false;
            for (int i = 0; i < aPatientTypes.Length; i++)
            {
                if (aPatientTypeCode == aPatientTypes[i])
                {
                    aIsValiad = true;
                    break;
                }
            }
            return aIsValiad;
        }

        private void ValidateIfExcluded()
        {
            if (aPhysician.ExcludedStatus == "Y")
            {
                ThrowAnExcption();
            }
        }

        private void ThrowAnExcption()
        {
            Exceptions.InvalidPhysicianAssignmentException aException =
                new Exceptions.InvalidPhysicianAssignmentException();

            aException.PhysicianName = aPhysician.Name.DisplayString;
            aException.PhysicianNumber = aPhysician.PhysicianNumber;
            aException.RelationshipType = ATTENDINGNAME;

            throw aException;
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public AttendingPhysician()
            : base(ATTENDING, ATTENDINGNAME)
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
