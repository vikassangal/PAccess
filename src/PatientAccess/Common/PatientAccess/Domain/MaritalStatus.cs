using System;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class MaritalStatus : CodedReferenceValue 
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
        public MaritalStatus()
        {
        }
        public MaritalStatus( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public MaritalStatus( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements        
        #endregion
    }
}