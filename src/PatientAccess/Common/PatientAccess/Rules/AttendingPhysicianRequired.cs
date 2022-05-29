using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Domain.UCCRegistration;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AttendingPhysicianRequired.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class AttendingPhysicianRequired : LeafRule
    {
        #region Events

        public event EventHandler AttendingPhysicianRequiredEvent;

        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            this.AttendingPhysicianRequiredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            this.AttendingPhysicianRequiredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.AttendingPhysicianRequiredEvent = null;
        }

        public override bool CanBeAppliedTo(object context)
        {
            if (context == null || context.GetType() != typeof(Account))
            {
                return true;
            }

            Account anAccount = (Account)context;

            if (anAccount.Activity == null)
            {
                return true;
            }

            if (anAccount.KindOfVisit == null)
            {
                return true;
            }

            bool isHospitalServiceDayCare = this.IsHospitalServiceDayCare(anAccount);
            bool accountDoesNotHaveAnAttendingPhysician = this.AccountDoesNotHaveAnAttendingPhysician(anAccount);
            
            bool ruleCanBeApplied = this.CanBeAppliedTo(anAccount.Activity, anAccount.KindOfVisit, isHospitalServiceDayCare);

            if (ruleCanBeApplied && accountDoesNotHaveAnAttendingPhysician)
            {
                if (this.FireEvents && AttendingPhysicianRequiredEvent != null)
                {
                    this.AttendingPhysicianRequiredEvent(this, null);
                }

                return false;
            }
            else
            {
                return true;
            }
        }


        internal bool CanBeAppliedTo(Activity activity, VisitType kindOfVisit, bool isHospitalServiceDayCare)
        {
            return (
                       
                        activity is PreMSERegisterActivity ||
                        activity is UCCPreMSERegistrationActivity ||
                        activity is UCCPostMseRegistrationActivity ||
                        activity is EditPreMseActivity ||
                        activity is EditUCCPreMSEActivity || 
                        activity is AdmitNewbornActivity ||
                        activity is PostMSERegistrationActivity ||
                        activity is TransferOutToInActivity ||
                        activity is ShortRegistrationActivity ||
                        activity is ShortPreRegistrationActivity ||
                        activity is ShortMaintenanceActivity ||
                        activity is TransferOutpatientToERActivity ||
                        activity is TransferERToOutpatientActivity ||
                        (
                            ( activity is RegistrationActivity || activity is MaintenanceActivity ) &&
                            ( kindOfVisit.IsInpatient || kindOfVisit.IsEmergencyPatient ||
                              ( kindOfVisit.IsOutpatient || 
                                kindOfVisit.IsRecurringPatient || 
                                kindOfVisit.IsNonPatient ) && 
                                isHospitalServiceDayCare )
                        )
                        
                   );
        }

        
        private bool IsHospitalServiceDayCare(IAccount anAccount)
        {
            return anAccount.HospitalService != null && anAccount.HospitalService.IsDayCare();
        }


        private bool AccountDoesNotHaveAnAttendingPhysician(IAccount anAccount)
        {
            return (anAccount.AttendingPhysician == null || anAccount.AttendingPhysician.FirstName.Trim() + anAccount.AttendingPhysician.LastName.Trim() == string.Empty);
        }


        public override void ApplyTo(object context)
        {

        }

        public override bool ShouldStopProcessing()
        {
            return false;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }


}
