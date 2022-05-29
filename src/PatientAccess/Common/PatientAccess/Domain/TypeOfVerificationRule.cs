using System;

namespace PatientAccess.Domain
{
    //TODO: Create XML summary comment for TypeOfVerificationRule
    [Serializable]
    public class TypeOfVerificationRule : ReferenceValue
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
        
        public TypeOfVerificationRule() { }

        public TypeOfVerificationRule( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
