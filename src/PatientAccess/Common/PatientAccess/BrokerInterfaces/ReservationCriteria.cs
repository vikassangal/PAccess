using System;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for ReservationCriteria.
	/// </summary>
	[Serializable]
	public class ReservationCriteria : ReferenceValue
	{
        #region Event Handlers
        #endregion

        #region Construction and Finalization
        public ReservationCriteria()
        {
        }

        public ReservationCriteria( long oid, DateTime version, Location fromLocation, Location toLocation, Facility facility, VisitType patientType )
            //:base( oid, version )
        {
            this.OldLocation = fromLocation;
            this.NewLocation = toLocation;
            this.Facility = facility;
            this.PatientType = patientType;
        }
        #endregion

        #region Methods
        #endregion

        #region Properties

        public Location OldLocation
        {
            get
            {
                return i_OldLocation;
            }
            set
            {
                i_OldLocation = value;
            }
        }

        public Location NewLocation
        {
            get
            {
                return i_NewLocation;
            }
            set
            {
                i_NewLocation = value;
            }
        }

        public VisitType PatientType
        {
            get
            {
                return i_VisitType;
            }
            set
            {
                i_VisitType = value;
            }
        }

        public Facility Facility
        {
            get
            {
                return i_Facility;
            }
            set
            {
                i_Facility = value;
            }
        }
        #endregion

        #region Data Elements
        private Location i_NewLocation;
        private Location i_OldLocation;
        private VisitType i_VisitType;
        private Facility i_Facility;
        #endregion

        #region Constants
        #endregion
		
	}
}
