using System;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class Location : CodedReferenceValue 
    {
        #region Event Handlers
        #endregion

        #region Methods

        public override string ToString()
        {
            formattedLocation = String.Empty;

            if( this.NursingStation != null )
            {
                i_NursingStationCode = this.NursingStation.Code;
            }
            if( this.Room != null )
            {
                i_RoomCode = this.Room.Code;
            }
            if( this.Bed != null )
            {
                i_BedCode = this.Bed.Code;
            }
            if( i_NursingStationCode.Trim() == String.Empty &&
                ( i_RoomCode == "0" || i_RoomCode.Trim() == String.Empty ) &&
                i_BedCode.Trim() == String.Empty )
            {
                formattedLocation = String.Empty;
            }
            else
            {
                formattedLocation = String.Format( 
                    "{0}-{1}-{2}",
                    this.i_NursingStationCode,
                    this.i_RoomCode.PadLeft( 4, '0' ),
                    this.i_BedCode );
            }
        
            return formattedLocation;
        }

        public bool IsLocationAssigned()
        {
            return ( this.NursingStation != null && this.Room != null && this.Bed != null );
        }

                
        #endregion

        #region Properties
        public Bed Bed
        {
            get
            {
                return i_Bed;
            }
            set
            {
                i_Bed = value;
            }
        }

        public Room Room
        {
            get
            {
                return i_Room;
            }
            set
            {
                i_Room = value;
            }
        }

        public NursingStation NursingStation
        {
            get
            {
                return i_NursingStation;
            }
            set
            {
                i_NursingStation = value;
            }
        }

        public string FormattedLocation
        {
            get
            {
                return ToString();
            }
        }

        #endregion

        #region Private Methods

        
            
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public Location()
        {
        }
        public Location( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public Location( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        public Location( long oid, DateTime version, string description, string code, NursingStation nursingStation, Room room, Bed bed )
            : base( oid, version, description, code )
        {
            i_NursingStation = nursingStation;
            i_Room = room;
            i_Bed = bed;
        }
        public Location( string nursingStationCode, string roomCode, string bedCode )
        {
            this.NursingStation = new NursingStation( 0L, 
                NEW_VERSION, string.Empty, nursingStationCode );
            this.Room = new Room( 0L, NEW_VERSION, string.Empty, roomCode );
            this.Bed = new Bed( 0L, NEW_VERSION, string.Empty, bedCode );
        }
        #endregion

        #region Data Elements
        private NursingStation i_NursingStation;
        private Room i_Room;
        private Bed i_Bed;
        private string i_NursingStationCode = String.Empty;
        private string i_RoomCode = "0";
        private string i_BedCode = String.Empty;
        private string formattedLocation;
       
        #endregion
    }
}