using System;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class Gender : CodedReferenceValue 
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Gender()
        {
        }

        public Gender( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public Gender( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements
        
        public const string
            FEMALE_CODE     = "F",
            MALE_CODE       = "M",
            UNKNOWN_CODE    = "U";

        public const string 
            PATIENT_GENDER = "Patient_Gender",
            BIRTH_GENDER = "Birth_Gender";

        #endregion
    }
}