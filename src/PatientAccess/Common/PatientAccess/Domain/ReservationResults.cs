using System;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class ReservationResults : CodedReferenceValue 
    {
        #region Event Handlers
        #endregion

        #region Methods

        private void LocationPreviouslyOccupied( 
            string firstName, string lastName, string middleInitial, long accountNumber )
        {
            Name name = new Name( NEW_OID, NEW_VERSION,
                firstName, lastName, middleInitial);

            this.ReservationSucceeded = false;
            this.Message = String.Format( "{0}\n Patient name: {1}\n Account: {2}",
                MSG_OCCUPIED, name.AsFormattedName(), accountNumber );
        }

        private void LocationPreviouslyReserved()
        {
            this.ReservationSucceeded = false;
            this.Message = MSG_RESERVED;
        }

        private void LocationSuccessfullyReserved()
        {
            this.ReservationSucceeded = true;
            this.Message = MSG_SUCCEEDED;
        }

        #endregion

        #region Properties

        public string Message
        {
            get
            {
                return i_Message;
            }
            private set
            {
                i_Message = value;
            }
        }        
        
        public bool ReservationSucceeded
        {
            get
            {
                return i_ReservationSucceeded;
            }
            private set
            {
                i_ReservationSucceeded = value;
            }
        }

        public Location Location
        {
            get
            {
                return i_Location;
            }
            private set
            {
                i_Location = value;
            }
        }
        #endregion

        #region Private Methods

        private void SetMessage( Location location )
        {
            switch( this.Message )
            {
                case "Occupied" :
                {
                    this.LocationPreviouslyOccupied( 
                        FirstName, LastName, MiddleInitial, AccountNumber );
                    break;
                }
                case "Reserved" :
                {
                    this.LocationPreviouslyReserved();
                    break;
                }
                case "Success" :
                {
                    this.Location = location;
                    this.LocationSuccessfullyReserved();
                    break;
                }   
                case "Available" :
                {
                    this.Location = location;
                    this.ReservationSucceeded = false;
                    this.Message = MSG_AVAILABLE;
                    break;
                }            
                case "Invalid Nursingstation":
                {
                    this.Message = MSG_NS_DOESNT_EXIST;
                    break;
                }
                case "Invalid Room":
                {
                    this.Message = MSG_ROOM_DOESNT_EXIST;
                    break;
                }
                case "Invalid Bed":
                {
                    this.Message = MSG_BED_DOESNT_EXIST;
                    break;
                }
            }
        }
        #endregion

        #region Private Properties

        public string FirstName
        {
            get
            {
                return i_FirstName;
            }
            private set
            {
                i_FirstName = value;
            }
        }

        public string LastName
        {
            get
            {
                return i_LastName;
            }
            private set
            {
                i_LastName = value;
            }
        }

        public string MiddleInitial
        {
            get
            {
                return i_MiddleInitial;
            }
            private set
            {
                i_MiddleInitial = value;
            }
        }

        private long AccountNumber
        {
            get
            {
                return i_AccountNumber;
            }
            set
            {
                i_AccountNumber = value;
            }
        }
        #endregion

        #region Construction and Finalization
        public ReservationResults()
        {
        }
        public ReservationResults( long oid, string description )
            : base( oid, description )
        {
        }
        public ReservationResults( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public ReservationResults( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        public ReservationResults( long oid, DateTime version, string description, string code, string result, 
            string firstName, string lastName, string middleInitial, long accountNumber, Location reservedLocation )
            : base( oid, version, description, code )
        {
            this.Message = result;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.MiddleInitial = middleInitial;
            this.AccountNumber = accountNumber;
            this.SetMessage( reservedLocation );
        }

        #endregion

        #region Data Elements
        private Location i_Location;
        private string i_Message;
        private bool i_ReservationSucceeded;
        private string i_FirstName;
        private string i_LastName;
        private string i_MiddleInitial;
        private long i_AccountNumber;
        #endregion

        #region Constants

        public const string
            MSG_RESERVED = " The requested bed is unavailable, pending admission of a patient",
            MSG_AVAILABLE = " The requested bed is available";

        private const string
            MSG_OCCUPIED = " The requested bed is occupied by:",
            MSG_SUCCEEDED = " The requested bed was successfully reserved.",
            MSG_NS_DOESNT_EXIST = " The requested nursing station does not exist in the system.",
            MSG_ROOM_DOESNT_EXIST = " The requested room is invalid for the specified nursing station.",
            MSG_BED_DOESNT_EXIST = " The requested bed is invalid for the specified room.";
     

        #endregion
    }
}