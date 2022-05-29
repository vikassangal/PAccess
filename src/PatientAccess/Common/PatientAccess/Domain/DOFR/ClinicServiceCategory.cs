using System;

namespace PatientAccess.Domain
{
	//TODO: Create XML summary comment for HospitalClinic
    [Serializable]
    public class ClinicServiceCategory : CodedReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {
            return String.Format("{0} {1}",Code, Description);
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ClinicServiceCategory()
        {
        }
        
        public ClinicServiceCategory( string description, string code )
            : base( description, code )
        {
        }
        #endregion

        #region Data Elements
    
        #endregion

        #region Constants
        #endregion
    }
}
