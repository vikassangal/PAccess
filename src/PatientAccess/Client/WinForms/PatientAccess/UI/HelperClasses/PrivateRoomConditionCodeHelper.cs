using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.UI.HelperClasses
{
    class PrivateRoomConditionCodeHelper
    {
        #region Constructor
        public PrivateRoomConditionCodeHelper()
        {

        }
        #endregion

        #region Public Methods

        public void AddPrivateRoomConditionCode(ReasonForAccommodation.ReasonForAccommodationId reasonForAccommodationOid,Account account)
        {
            IConditionCodeBroker conditionCodeBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
            ConditionCode semiPrivateRoomCode = conditionCodeBroker.ConditionCodeWith(
                User.GetCurrent().Facility.Oid, ConditionCode.CONDITIONCODE_SEMI_PRIVATE_ROOM_NOT_AVAILABLE);
            ConditionCode privateRoomCode = conditionCodeBroker.ConditionCodeWith(
                User.GetCurrent().Facility.Oid, ConditionCode.CONDITIONCODE_PRIVATE_ROOM_MEDICALLY_REQUIRED);

            if ( reasonForAccommodationOid == ReasonForAccommodation.ReasonForAccommodationId.PRIVATE_ROOM_MEDICALLY_NECESSARY_ID)
            {
                account.RemoveConditionCode(semiPrivateRoomCode);
                account.AddConditionCode(privateRoomCode);
                account.Diagnosis.isPrivateAccommodationRequested = false;
            }

            if ( reasonForAccommodationOid == ReasonForAccommodation.ReasonForAccommodationId.SEMI_PRIVATE_ROOM_NOT_AVAILABLE)
            {
                account.RemoveConditionCode(privateRoomCode);
                account.AddConditionCode(semiPrivateRoomCode);
                account.Diagnosis.isPrivateAccommodationRequested = false;
            }

            if ( reasonForAccommodationOid == ReasonForAccommodation.ReasonForAccommodationId.PATIENT_REQUESTED_PRIVATE_ROOM
                || reasonForAccommodationOid == ReasonForAccommodation.ReasonForAccommodationId.DEFAULT_CODE)
            {
                account.RemoveConditionCode(privateRoomCode);
                account.RemoveConditionCode(semiPrivateRoomCode);
                account.Diagnosis.isPrivateAccommodationRequested = true;
            }

            if( account.Location != null &&
                account.Location.Bed != null &&
                account.Location.Bed.Accomodation != null )
            {
                if( reasonForAccommodationOid != ReasonForAccommodation.ReasonForAccommodationId.DEFAULT_CODE )
                {
                    account.Location.Bed.Accomodation.IsReasonForAccommodationSelected = true;
                }
                else
                {
                    account.Location.Bed.Accomodation.IsReasonForAccommodationSelected = false;
                }
            }
        }

        public void AddConditionCodeForPrivateRoomMedicallyNecessary(Accomodation selectedAccomodation,Account account)
        {
            IConditionCodeBroker conditionCodeBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
            if (account.Location != null & account.Location.Bed !=null 
                 )
            {
                if( account.Location.Bed.Accomodation!=null && account.Location.Bed.Accomodation.IsPrivateRoomMedicallyNecessary())
                {
                ConditionCode code = conditionCodeBroker.ConditionCodeWith(
                    User.GetCurrent().Facility.Oid,
                    ConditionCode.CONDITIONCODE_PRIVATE_ROOM_MEDICALLY_REQUIRED);
                account.RemoveConditionCode(code);
                }
                account.Location.Bed.Accomodation = selectedAccomodation;
                if (account.Location.Bed.Accomodation != null && account.Location.Bed.Accomodation.IsPrivateRoomMedicallyNecessary())
                {
                ConditionCode code = conditionCodeBroker.ConditionCodeWith(
                    User.GetCurrent().Facility.Oid,
                    ConditionCode.CONDITIONCODE_PRIVATE_ROOM_MEDICALLY_REQUIRED);
                account.AddConditionCode(code);
                }
            }
            
            
        }
        public void RemovePrivateRommConditionCodes(Account account)
        {
            IConditionCodeBroker conditionCodeBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
            ConditionCode semiPrivateRoomCode = conditionCodeBroker.ConditionCodeWith(
                User.GetCurrent().Facility.Oid,
                ConditionCode.CONDITIONCODE_SEMI_PRIVATE_ROOM_NOT_AVAILABLE);
            ConditionCode privateRoomCode = conditionCodeBroker.ConditionCodeWith(
                User.GetCurrent().Facility.Oid,
                ConditionCode.CONDITIONCODE_PRIVATE_ROOM_MEDICALLY_REQUIRED);
            account.RemoveConditionCode(privateRoomCode);
            account.RemoveConditionCode(semiPrivateRoomCode);
            
        }

        public bool EnableAccommodation(Account account)
        {
            if ( ( account.Activity is AdmitNewbornActivity  ||
            account.Activity is RegistrationActivity || account.Activity is MaintenanceActivity)
            && account.KindOfVisit != null
            && account.KindOfVisit.Code == VisitType.INPATIENT && account.Location != null
                && ( account.Location.Bed != null && account.Location.Bed.Code.Length > 0)
                )
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion

        #region Constants
        #endregion
    }
}
