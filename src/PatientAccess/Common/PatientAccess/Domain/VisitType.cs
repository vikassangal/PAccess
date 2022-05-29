using System;
using System.Collections;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class VisitType : CodedReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {   
            return String.Format("{0} {1}", Code, Description);
        }


        public bool IsInpatient
        {
            get
            {
                return this.Code.Equals( INPATIENT );
            }
        }


        public bool IsOutpatient
        {
            get
            {
                return this.Code.Equals( OUTPATIENT );
            }
        }


        public bool IsEmergencyPatient
        {
            get
            {
                return this.Code.Equals( EMERGENCY_PATIENT );
            }
        }


        public bool IsRecurringPatient
        {
            get
            {
                return this.Code.Equals( RECURRING_PATIENT );
            }
        }


        public bool IsNonPatient
        {
            get
            {
                return this.Code.Equals(NON_PATIENT);
            }
        }


        public bool IsPreRegistrationPatient
        {
            get
            {
                return this.Code.Equals( PREREG_PATIENT );
            }
        }


        // Determines if Patient type can be changed for this Visit Type
        public bool IsPatientTypeChangeableFor(Location location)
        {
            if( ( this.Code == PREREG_PATIENT ||
                this.Code == INPATIENT ||
                 this.Code == RECURRING_PATIENT ||
                 this.Code == NON_PATIENT ||
                this.Code == NON_PATIENT ) ||
               ( this.Code == OUTPATIENT &&
               location != null && location.FormattedLocation != string.Empty ))
            {
                return false;
            }
            return true;
        }

        // Determines if Patient can have a Bed for this Visit Type and Hospital Service Code
        public bool CanHaveBedFor( HospitalService hsvCode )
        {
            bool canHaveBed = false;

            if( IsInpatient )
            {
                canHaveBed = true;
            }
            else if( IsOutpatient ||
                     IsRecurringPatient ||
                     IsNonPatient ||
                     IsEmergencyPatient )
            {
                if( hsvCode.IsDayCare() )   // HSV = 58, 59, FO, LD or LB
                {
                    canHaveBed = true;
                }
            }

            return canHaveBed;
        }

        //Determines if Patient type is valid for COB Received Flag
        public bool IsValidForCOBReceived
        {
            get
            {
                return ( Code == PREREG_PATIENT || Code == INPATIENT || Code == OUTPATIENT ||
                        Code == EMERGENCY_PATIENT || Code == RECURRING_PATIENT );
            }
        }
        
        #endregion

        #region Properties

        public static VisitType PreRegistration
        {
            get
            {
                return new VisitType(NEW_OID,NEW_VERSION,PREREG_PATIENT_DESC,PREREG_PATIENT);
            }
        }

        public static VisitType NonPatient
        {
            get
            {
                return new VisitType(NEW_OID,NEW_VERSION,NON_PATIENT_DESC,NON_PATIENT);
            }
        }

        public static VisitType Recurring
        {
            get
            {
                return new VisitType(NEW_OID,NEW_VERSION,RECURRING_PATIENT_DESC,RECURRING_PATIENT);
            }
        }

        public static VisitType Outpatient
        {
            get
            {
                return new VisitType(NEW_OID,NEW_VERSION,OUTPATIENT_DESC,OUTPATIENT);
            }
        }
        public static VisitType UCCOutpatient
        {
            get
            {
                return new VisitType(NEW_OID, NEW_VERSION, UCC_OUTPATIENT_DESC, OUTPATIENT);
            }
        }

        public static VisitType Emergency
        {
            get
            {
                return new VisitType(NEW_OID,NEW_VERSION,EMERGENCY_PATIENT_DESC,EMERGENCY_PATIENT);
            }
        }

        public static VisitType Inpatient
        {
            get
            {
                return new VisitType(NEW_OID,NEW_VERSION,INPATIENT_DESC,INPATIENT);
            }
        }

        public ArrayList HospitalServices
        {
            get
            {
                return i_HospitalServices;
            }
            set
            {
                i_HospitalServices = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public VisitType()
        {
        }

        public VisitType( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public VisitType( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements
        private ArrayList i_HospitalServices;
        #endregion

        #region VistType Constants
        public const string PREREG_PATIENT    = "0";
        public const string INPATIENT         = "1";
        public const string OUTPATIENT        = "2";
        public const string EMERGENCY_PATIENT = "3";
        public const string RECURRING_PATIENT = "4";
        public const string NON_PATIENT       = "9";

        public const string PREREG_PATIENT_DESC    = "PREADMIT";
        public const string INPATIENT_DESC         = "INPATIENT";
        public const string OUTPATIENT_DESC        = "OUTPATIENT";
        public const string EMERGENCY_PATIENT_DESC = "ER PATIENT";
        public const string RECURRING_PATIENT_DESC = "RCRNG O/PT";
        public const string NON_PATIENT_DESC       = "NONPATIENT";
        public const string UCC_OUTPATIENT_DESC    = "OP-UC PAT";
        #endregion
    }
}
