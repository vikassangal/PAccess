using System;
using System.Collections;

namespace PatientAccess.Domain
{
    [Serializable]
    public class ReasonForAccommodation : ReferenceValue
    {
        #region Event Handlers
        #endregion

        #region Methods
        public ICollection PopluateReasonForAccommodation()
        {
            ArrayList reasonsForAccommodation = new ArrayList();
            reasonsForAccommodation.Add(new ReasonForAccommodation());
            reasonsForAccommodation.Add(new ReasonForAccommodation( ReasonForAccommodationId.PRIVATE_ROOM_MEDICALLY_NECESSARY_ID,NEW_VERSION,PRIVATE_ROOM_MEDICALLY_NECESSARY));
            reasonsForAccommodation.Add(new ReasonForAccommodation( ReasonForAccommodationId.SEMI_PRIVATE_ROOM_NOT_AVAILABLE, NEW_VERSION, SEMI_PRIVATE_ROOM_NOT_AVAILABLE));
            reasonsForAccommodation.Add(new ReasonForAccommodation( ReasonForAccommodationId.PATIENT_REQUESTED_PRIVATE_ROOM, NEW_VERSION, PATIENT_REQUESTED_PRIVATE_ROOM));
            return reasonsForAccommodation;
          
        }
        
        #endregion

        #region Properties
        public new ReasonForAccommodationId Oid
        {
            get
            {
                return (ReasonForAccommodationId)base.Oid;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ReasonForAccommodation()
        {
        }
        public ReasonForAccommodation(ReasonForAccommodationId oid, string description)
            : base((long)oid, description)
        {
        }

        public ReasonForAccommodation(ReasonForAccommodationId oid, DateTime version)
            : base((long)oid, version)
		{
		}

        private ReasonForAccommodation(ReasonForAccommodationId oid, DateTime version, string description)
            : base((long)oid, version, description)
		{
		}
        #endregion

       #region Data Elements
       #endregion

       #region Constants
       private const string PRIVATE_ROOM_MEDICALLY_NECESSARY = "Private room is medically necessary";
       private const string SEMI_PRIVATE_ROOM_NOT_AVAILABLE = "Semi-private room is not available";
       private const string PATIENT_REQUESTED_PRIVATE_ROOM = "Patient requested a private room";

        public enum ReasonForAccommodationId :long
        {
            DEFAULT_CODE=0,
            PRIVATE_ROOM_MEDICALLY_NECESSARY_ID =1,
            SEMI_PRIVATE_ROOM_NOT_AVAILABLE = 2,
            PATIENT_REQUESTED_PRIVATE_ROOM =3
        }
       #endregion

        

    }
}
