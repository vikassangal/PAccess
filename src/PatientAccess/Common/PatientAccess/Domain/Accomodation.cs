using System;

namespace PatientAccess.Domain 
{
    [Serializable]
    public class Accomodation : CodedReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override string ToString()
        {   // Display 2-digit code (with zero-placeholder if only 1 digit) and description
            return String.Format("{0} {1}", Code.Trim(), Description.Trim());
        }
        #endregion

        #region Properties
        public bool IsReasonForAccommodationSelected
        {
            get
            {
                return isReasonForAccommodationSelected;
            }
            set
            {
                isReasonForAccommodationSelected = value;
            }

        }
        #endregion

        #region Private Methods
        public bool IsReasonRequiredForSelectedAccommodation()
        {
            if ( this.Code == "01"
                || this.Code == "04"
                || this.Code == "09"
                || this.Code == "14"
                || this.Code == "21"
                || this.Code == "43"
                || this.Code == "46"
                || this.Code == "48"
                || this.Code == "52"
                || this.Code == "65"
                || this.Code == "76"
                || this.Code == "85"
                || this.Code == "86"
                || this.Code == "95"
                || this.Code == "98" )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsPrivateRoomMedicallyNecessary()
        {
            if( this.Code == "05"
                || this.Code == "61"
                || this.Code == "84" )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Private Properties
        
        #endregion

        #region Construction and Finalization
        public Accomodation()
        {
        }
        public Accomodation( long oid, DateTime version, string description )
            : base( oid, version, description )
        {
        }

        public Accomodation( long oid, DateTime version, string description, string code )
            : base( oid, version, description, code )
        {
        }
        #endregion

        #region Data Elements    
        private bool isReasonForAccommodationSelected = false;
        #endregion
    }
}