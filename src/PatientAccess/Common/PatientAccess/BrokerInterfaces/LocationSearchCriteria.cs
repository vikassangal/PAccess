using System;

namespace PatientAccess.BrokerInterfaces
{
    [Serializable]
    /// <summary>
    /// This class is used to pass parameter to LocationsMatching method
    /// in LocationPackage.
    /// </summary>
    public class LocationSearchCriteria : SearchCriteria
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public string Gender 
        {
            get
            {
                return i_genderId;
            }
            private set
            {
                i_genderId = value;
            }
        }

        public bool IsOccupiedBeds 
        {
            get
            {
                return i_isOccupiedBeds;
            }
            set
            {
                i_isOccupiedBeds = value;
            }
        }

        public string NursingStation 
        {
            get
            {       
                return i_nursingStnCode;
            }
            private set
            {
                i_nursingStnCode = value;
            }
        }

        public string Room 
        {
            get
            {
                return i_roomCode;
            }
            private set
            {
                i_roomCode = value;
            }
        }

        #endregion

        #region Private Methods
        public override ValidationResult Validate()
        {
            return new ValidationResult( true );
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public LocationSearchCriteria( string HSPCode, string i_genderId, string i_nursingStnCode, string i_roomCode, bool i_isOccupiedBeds )
            : base( HSPCode )
        {
            this.Gender                 = i_genderId;
            this.NursingStation         = i_nursingStnCode;
            this.Room                   = i_roomCode;
            this.i_isOccupiedBeds       = i_isOccupiedBeds;
        }

        #endregion

        #region Data Elements
        private string                  i_genderId = string.Empty;
        private string                  i_nursingStnCode = string.Empty;
        private string                  i_roomCode = string.Empty;
        private bool                    i_isOccupiedBeds = false;
        #endregion

        #region Constants
        #endregion
    }
}