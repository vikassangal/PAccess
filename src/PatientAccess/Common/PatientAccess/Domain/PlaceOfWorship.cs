using System;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class PlaceOfWorship : CodedReferenceValue 
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
        public PlaceOfWorship()
        {
        }
        public PlaceOfWorship( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public PlaceOfWorship( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements        
        #endregion
    }
}