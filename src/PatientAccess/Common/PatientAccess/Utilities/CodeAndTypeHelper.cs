using System;
using System.Diagnostics;
using PatientAccess.Domain;

namespace PatientAccess.Utilities
{
    public enum PatientType
    {
        InPatient,
        OutPatient,
        IsEmergencyPatient,
        IsRecurringPatient,
        IsNonPatient,
        IsPreRegistrationPatient,
        Null
    }

    public enum YesNoBlank
    {
        Yes,
        No,
        Blank
    }

    public enum ActivityType
    {
        ActivatePreRegistrationActivity,
        AdmitNewbornActivity,
        AdmitNewbornWithOfflineActivity,
        PreAdmitNewbornActivity,
        PreAdmitNewbornWithOfflineActivity,
        CancelInpatientDischargeActivity,
        CancelInpatientStatusActivity,
        CancelOutpatientDischargeActivity,
        CancelPreRegActivity,
        CensusByADTActivity,
        CensusByBloodlessActivity,
        CensusByNursingStationActivity,
        CensusByPatientActivity,
        CensusByPayorActivity,
        CensusByPhysicianActivity,
        CensusByReligionActivity,
        DischargeActivity,
        EditAccountActivity,
        EditDischargeDataActivity,
        EditPreMseActivity,
        EditRecurringDischargeActivity,
        InsuranceVerificationWorklistActivity,
        MaintenanceActivity,
        NoShowWorklistActivity,
        PatientLiabilityWorklistActivity,
        PhysiciansReportActivity,
        PostMSERegistrationActivity,
        PostRegistrationWorklistActivity,
        PreDischargeActivity,
        PreMSERegisterActivity,
        PreMSERegistrationWithOfflineActivity,
        PreMSEWorklistActivity,
        PreRegistrationActivity,
        PreRegistrationWithOfflineActivity,
        PreRegistrationWorklistActivity,
        PrintFaceSheetActivity,
        RegistrationActivity,
        RegistrationWithOfflineActivity,
        ReleaseReservedBedActivity,
        TransferActivity,
        TransferBedSwapActivity,
        TransferInToOutActivity,
        TransferOutToInActivity,
        ViewAccountActivity,
        TransferERToOutpatientActivity,
        TransferOutpatientToERActivity,
        Null
    }

    public static class CodeAndTypeHelper
    {
        /// <exception cref="ArgumentOutOfRangeException"><c>patientType</c> is out of range.</exception>
        public static VisitType GetVisitTypeFrom( PatientType patientType )
        {

            switch (patientType)
            {
                case PatientType.Null:
                    return null;

                case PatientType.InPatient:

                    return new VisitType { Code = VisitType.INPATIENT };

                case PatientType.OutPatient:
                    return new VisitType { Code = VisitType.OUTPATIENT };
                
                case PatientType.IsEmergencyPatient:
                    return new VisitType { Code = VisitType.EMERGENCY_PATIENT };
                
                case PatientType.IsRecurringPatient:
                    return new VisitType { Code = VisitType.RECURRING_PATIENT };
                
                case PatientType.IsNonPatient:
                    return new VisitType { Code = VisitType.NON_PATIENT };
                
                case PatientType.IsPreRegistrationPatient:
                    return new VisitType { Code = VisitType.PREREG_PATIENT };
                
                default:
                    throw new ArgumentOutOfRangeException( "patientType" );
            }
        }

        /// <exception cref="ArgumentOutOfRangeException"><c>yesNoBlank</c> is out of range.</exception>
        public static YesNoFlag GetYesNoFlagFrom( YesNoBlank yesNoBlank )
        {
            switch (yesNoBlank)
            {
                case YesNoBlank.Yes:
                    return new YesNoFlag { Code = YesNoFlag.CODE_YES };
                case YesNoBlank.No:
                    return new YesNoFlag { Code = YesNoFlag.CODE_NO };
                case YesNoBlank.Blank:
                    return new YesNoFlag { Code = YesNoFlag.CODE_BLANK };
                default:
                    throw new ArgumentOutOfRangeException( "yesNoBlank" );
            }
        }

        public static Activity GetActivityFrom( ActivityType activityType )
        {
            if(activityType==ActivityType.Null) return null;

            string activityName = Enum.GetName( typeof( ActivityType ), activityType );
            string activityassemblyQualifiedName = "PatientAccess.Domain." + activityName + ", PatientAccess.Common";

            Debug.WriteLine( activityassemblyQualifiedName );
            Type type = Type.GetType( activityassemblyQualifiedName );
            return (Activity)Activator.CreateInstance( type );
        }
    }
}
