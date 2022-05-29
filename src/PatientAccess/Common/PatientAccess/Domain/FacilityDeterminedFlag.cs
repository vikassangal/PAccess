using System;

namespace PatientAccess.Domain
{
    //TODO: Create XML summary comment for FacilityDeterminedFlag
    [Serializable]
    public class FacilityDeterminedFlag  : CodedReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {   
            return String.Format("{0}", Description);
        }
        #endregion

        #region Properties
  
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public FacilityDeterminedFlag()
        {
        }
        public FacilityDeterminedFlag( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public FacilityDeterminedFlag( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
