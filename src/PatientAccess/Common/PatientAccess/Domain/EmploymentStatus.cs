using System;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class EmploymentStatus : CodedReferenceValue  
    {
        #region Event Handlers
        #endregion

        #region Methods
        public static EmploymentStatus NewFullTimeEmployed()
        {
            return new EmploymentStatus(NEW_OID, NEW_VERSION,
                 EMPLOYED_FULL_TIME_DESC, EMPLOYED_FULL_TIME_CODE );
        }

        public static EmploymentStatus NewPartTimeEmployed()
        {
            return new EmploymentStatus( NEW_OID, NEW_VERSION,
                EMPLOYED_PART_TIME_DESC, EMPLOYED_PART_TIME_CODE );
        }

        public static EmploymentStatus NewNotEmployed()
        {
            return new EmploymentStatus( NEW_OID, NEW_VERSION,
                 NOT_EMPLOYED_DESC, NOT_EMPLOYED_CODE );
        }

        public static EmploymentStatus NewSelfEmployed()
        {
            return new EmploymentStatus( NEW_OID, NEW_VERSION,
                 SELF_EMPLOYED_DESC, SELF_EMPLOYED_CODE );
        }

        public static EmploymentStatus NewRetired()
        {
            return new EmploymentStatus( NEW_OID, NEW_VERSION,
                RETIRED_DESC, RETIRED_CODE );
        }

        public static EmploymentStatus NewOnActiveMilitaryDuty()
        {
            return new EmploymentStatus( NEW_OID, NEW_VERSION,
                ON_ACTIVE_MILITARY_DUTY_DESC, ON_ACTIVE_MILITARY_DUTY_CODE );
        }

        public static EmploymentStatus NewOtherOrUnknown()
        {
            return new EmploymentStatus( NEW_OID, NEW_VERSION,
                 OTHER_DESC, OTHER_CODE );
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public EmploymentStatus()
        {
        }
        public EmploymentStatus( long oid, string description )
            : base( oid, description )
    {
    }
        public EmploymentStatus( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public EmploymentStatus( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements

        #endregion

        #region Constants
//        public static long EMPLOYED_OID = 1;
//        public static long NEVER_EMPLOYED_OID = 2;
//        public static long UNEMPLOYED_OID = 3;
//        public static long RETIRED_OID = 5;        

        public const string EMPLOYED_FULL_TIME_CODE = "1";
        public const string EMPLOYED_PART_TIME_CODE = "2";
        public const string NOT_EMPLOYED_CODE = "3";
        public const string SELF_EMPLOYED_CODE = "4";
        public const string RETIRED_CODE = "5";
        private const string ON_ACTIVE_MILITARY_DUTY_CODE = "6";
        public const string OTHER_CODE = "9";

        public const string EMPLOYED_FULL_TIME_DESC = "EMPLOYED FULL TIME";
        private const string EMPLOYED_PART_TIME_DESC = "EMPLOYED PART TIME";
        public const string NOT_EMPLOYED_DESC = "NOT EMPLOYED";
        public const string SELF_EMPLOYED_DESC = "SELF EMPLOYED";
        public const string RETIRED_DESC = "RETIRED";
        private const string ON_ACTIVE_MILITARY_DUTY_DESC = "ACTIVE MILITARY DUTY";
        private const string OTHER_DESC = "OTHER / UNKNOWN";

        #endregion

    }
}
