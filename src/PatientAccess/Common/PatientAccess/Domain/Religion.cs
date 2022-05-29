using System;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class Religion : CodedReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public int TotalCount
        {
            get
            {
                return i_Count;
            }
            set
            {
                i_Count = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Religion()
        {
        }
        public Religion( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public Religion( long oid, DateTime version, string description, int count )
            : base( oid, version, description )
        {
            this.i_Count = count;
        }

        public Religion( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements       
        private int i_Count = 0;
        #endregion
    }
}