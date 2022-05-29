using System;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class Bed : CodedReferenceValue 
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public Accomodation Accomodation
        {
            get
            {
                return i_Accomodation;
            }
            set
            {
                i_Accomodation = value;
            }
        }
        public bool IsOccupied
             {
                 get
                 {
                     return i_IsOccupied;
                 }
                 set
                 {
                     i_IsOccupied = value;
                 }
             }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Bed()
        {
        }
        public Bed( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public Bed( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        public Bed( long oid, DateTime version, string description, string code, Accomodation accomodation, bool isOccupied )
            : base( oid, version, description, code )
        {
            i_Accomodation = accomodation;
            i_IsOccupied = isOccupied;
        }
        #endregion

        #region Data Elements     
        private Accomodation i_Accomodation;
        private bool i_IsOccupied;
        #endregion
    }
}