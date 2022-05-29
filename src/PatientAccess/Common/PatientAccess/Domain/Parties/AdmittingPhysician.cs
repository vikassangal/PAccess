using System;
using PatientAccess.Domain.UCCRegistration;

namespace PatientAccess.Domain.Parties
{
    /// <summary>
    /// Summary description for AdmittingPhysician.
    /// </summary>
    //TODO: Create XML summary comment for AdmittingPhysician
    [Serializable]
    public class AdmittingPhysician : PhysicianRole
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
            return new PhysicianRole(ADMITTING, ADMITTINGNAME);
        }

        public override void ValidateFor(Account selectedAccount, Physician physician)
        {
            this.aPhysician = physician;

            bool isPhysicianAssignable = IsStatusValidFor(selectedAccount.KindOfVisit, selectedAccount.Activity, this.aPhysician);

            if (!isPhysicianAssignable)
            {
                this.ThrowAnException();
            }

            bool isValidPrivilegesWise = ArePrivilegesValidFor(selectedAccount, this.aPhysician);

            if (!isValidPrivilegesWise)
            {
                this.ThrowAnException();
            }

            ValidateIfExcluded();

            ValidateMandatoryPatientTypes(selectedAccount);
        }


        internal static bool ArePrivilegesValidFor(Account selectedAccount, Physician physician)
        {
            bool isValidPrivilegesWise = false;
            if (physician.AdmittingPrivileges == "Y")
            {
                if (selectedAccount.Activity != null
                    && selectedAccount.Activity.GetType() == typeof(TransferOutToInActivity))
                {
                    if (selectedAccount.TransferDate < physician.AdmitPrivilegeSuspendDate ||
                        physician.AdmitPrivilegeSuspendDate == DateTime.MinValue)
                    {
                        isValidPrivilegesWise = true;
                    }
                }
                else
                {
                    if (selectedAccount.AdmitDate < physician.AdmitPrivilegeSuspendDate ||
                        physician.AdmitPrivilegeSuspendDate == DateTime.MinValue)
                    {
                        isValidPrivilegesWise = true;
                    }
                }
            }
            else
            {
                if (selectedAccount.KindOfVisit.Code == VisitType.NON_PATIENT && selectedAccount.HospitalService.DayCare != "Y")
                {
                    isValidPrivilegesWise = true;
                }
                else if (selectedAccount.KindOfVisit.Code == VisitType.OUTPATIENT && selectedAccount.HospitalService.DayCare != "Y")
                {
                    isValidPrivilegesWise = true;
                }
                else if (selectedAccount.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT)
                {
                    isValidPrivilegesWise = true;
                }
                else if (selectedAccount.KindOfVisit.Code == VisitType.RECURRING_PATIENT && selectedAccount.HospitalService.DayCare != "Y")
                {
                    isValidPrivilegesWise = true;
                }
                else if (selectedAccount.KindOfVisit.Code == VisitType.PREREG_PATIENT)
                {
                    isValidPrivilegesWise = true;
                }
                else if (selectedAccount.Activity.GetType() == typeof (PreRegistrationActivity) ||
                         selectedAccount.Activity.GetType() == typeof (PreMSERegisterActivity) ||
                         selectedAccount.Activity.GetType() == typeof (EditPreMseActivity) ||
                         selectedAccount.Activity.GetType() == typeof (PostMSERegistrationActivity) ||
                         selectedAccount.Activity.AssociatedActivityType == typeof (PreRegistrationActivity) ||
                         selectedAccount.Activity.AssociatedActivityType == typeof (PreMSERegisterActivity) ||
                         selectedAccount.Activity.AssociatedActivityType == typeof (EditPreMseActivity) ||
                         selectedAccount.Activity.AssociatedActivityType == typeof (PostMSERegistrationActivity) ||
                         selectedAccount.Activity.GetType() == typeof (UCCPostMseRegistrationActivity) ||
                         selectedAccount.Activity.AssociatedActivityType == typeof (UCCPreMSERegistrationActivity))
                {
                    isValidPrivilegesWise = true;
                }
            }
            return isValidPrivilegesWise;
        }


        internal static bool IsStatusValidFor( VisitType visitType, Activity activity, Physician physician )
        {
            Type activityType = activity.GetType();
            
            bool isValid = false;
            
            if ( physician.ActiveInactiveFlag == ACTIVE )
            {
                isValid = true;
            }

            else
            {

                if (visitType.Code == VisitType.EMERGENCY_PATIENT &&
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
                VisitType.PREREG_PATIENT,
                VisitType.INPATIENT,
                VisitType.OUTPATIENT,
                VisitType.EMERGENCY_PATIENT,
                VisitType.RECURRING_PATIENT ,
                VisitType.NON_PATIENT };
            return types;
        }

        private void ValidateMandatoryPatientTypes(Account selectedAccount)
        {
            bool aIsValid = false;
            string[] aPatientTypes = GetMandatoryPatientTypes();
            string aPatientTypeCode = selectedAccount.KindOfVisit.Code;

            for (int i = 0; i < aPatientTypes.Length; i++)
            {
                if (aPatientTypeCode == aPatientTypes[i])
                {
                    aIsValid = true;
                    break;
                }
            }

            if (!aIsValid)
            {
                ThrowAnException();
            }
        }

        private void ValidateIfExcluded()
        {
            if (aPhysician.ExcludedStatus == "Y")
            {
                ThrowAnException();
            }
        }


        internal void ValidatePrivileges(Account selectedAccount)
        {
            bool isValidPrivilegesWise = false;
            if (aPhysician.AdmittingPrivileges == "Y")
            {
                if (selectedAccount.Activity != null
                    && selectedAccount.Activity.GetType() == typeof(TransferOutToInActivity))
                {
                    if (selectedAccount.TransferDate < aPhysician.AdmitPrivilegeSuspendDate ||
                        aPhysician.AdmitPrivilegeSuspendDate == DateTime.MinValue)
                    {
                        isValidPrivilegesWise = true;
                    }
                }
                else
                {
                    if (selectedAccount.AdmitDate < aPhysician.AdmitPrivilegeSuspendDate ||
                        aPhysician.AdmitPrivilegeSuspendDate == DateTime.MinValue)
                    {
                        isValidPrivilegesWise = true;
                    }
                }
            }
            else
            {
                if (selectedAccount.KindOfVisit.Code == VisitType.OUTPATIENT && selectedAccount.HospitalService.DayCare != "Y")
                {
                    isValidPrivilegesWise = true;
                }
                else if (selectedAccount.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT)
                {
                    isValidPrivilegesWise = true;
                }
                else if (selectedAccount.KindOfVisit.Code == VisitType.RECURRING_PATIENT && selectedAccount.HospitalService.DayCare != "Y")
                {
                    isValidPrivilegesWise = true;
                }
                else if (selectedAccount.KindOfVisit.Code == VisitType.PREREG_PATIENT)
                {
                    isValidPrivilegesWise = true;
                }
                else if (selectedAccount.Activity.GetType() == typeof (PreRegistrationActivity) ||
                         selectedAccount.Activity.GetType() == typeof (PreMSERegisterActivity) ||
                         selectedAccount.Activity.GetType() == typeof (EditPreMseActivity) ||
                         selectedAccount.Activity.GetType() == typeof (UCCPreMSERegistrationActivity) ||
                         selectedAccount.Activity.GetType() == typeof (EditUCCPreMSEActivity) ||
                         selectedAccount.Activity.GetType() == typeof (PostMSERegistrationActivity) ||
                         selectedAccount.Activity.AssociatedActivityType == typeof (PreRegistrationActivity) ||
                         selectedAccount.Activity.AssociatedActivityType == typeof (PreMSERegisterActivity) ||
                         selectedAccount.Activity.AssociatedActivityType == typeof (EditPreMseActivity) ||
                         selectedAccount.Activity.AssociatedActivityType == typeof (PostMSERegistrationActivity))
                {
                    isValidPrivilegesWise = true;
                }
            }

            if (!isValidPrivilegesWise)
            {
                ThrowAnException();
            }
        }

        private void ThrowAnException()
        {
            Exceptions.InvalidPhysicianAssignmentException aException =
                new Exceptions.InvalidPhysicianAssignmentException();

            aException.PhysicianName = aPhysician.Name.DisplayString;
            aException.PhysicianNumber = aPhysician.PhysicianNumber;
            aException.RelationshipType = ADMITTINGNAME;
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
        public AdmittingPhysician()
            : base(ADMITTING, ADMITTINGNAME)
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
