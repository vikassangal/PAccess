using System;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class Room : CodedReferenceValue 
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public RoomCondition RoomCondition
        {
            get
            {
                return i_RoomCondition;
            }
            set
            {
                i_RoomCondition = value;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Room()
        {
        }
        public Room( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public Room( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        public Room( long oid, DateTime version, string description, string code, RoomCondition roomCondition )
            : base( oid, version, description, code )
        {
            this.i_RoomCondition = roomCondition;
        }
        #endregion

        #region Data Elements     
        private RoomCondition i_RoomCondition;
        #endregion
    }
}