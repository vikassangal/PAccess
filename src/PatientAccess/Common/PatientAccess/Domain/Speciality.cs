using System;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class Speciality : CodedReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public string PhysicianSpeciality
        {
            get
            {
                return i_Speciality;
            }
            set
            {
                i_Speciality = value;
            }
        }

        public string AsFormattedSpeciality
        {
            get
            {
                var specCode = Code;
                if (Code.Length > 5)
                {
                    specCode = Code.Substring(0, 5);
                }
                var specDescription = Description;
                if (Description.Length > 45)
                {
                    specDescription = Description.Substring(0, 45);
                }
                return (specCode + " " + specDescription);
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Speciality()
        {
        }
        public Speciality( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public Speciality( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements   
     
        private string i_Speciality;
        #endregion
    }
}