using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class AgeType : ReferenceValue
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

        public AgeType() { }

        public AgeType( long oid, DateTime version, string description )
            : base( oid, version, description ){}

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
