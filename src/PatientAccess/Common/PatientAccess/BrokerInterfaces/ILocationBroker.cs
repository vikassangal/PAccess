using System;
using System.Collections;
using System.Collections.Generic;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Interface for LocationBroker.
    /// </summary>
    public interface ILocationBroker
    {
        ReservationResults Reserve( ReservationCriteria reservationCriteria );
        void ReleaseReservedBed( Location location, Facility aFacility );
        ICollection AccomodationCodesFor( string nursingStationCode, Facility aFacility );
        Accomodation AccomodationFor(long facilityNumber, long oid);
        IList AccomodationsFor(long facilityNumber);
        ICollection LocationMatching( LocationSearchCriteria locationSearchCriteria );
        ICollection RoomsFor( Facility facility, string nursingStationCode );
        IList<NursingStation> NursingStationsFor( Facility facility, bool getCachedData = true );
        NursingStation NursingStationFor( Facility facility, string nursingStationCode );
        string ValidateLocation( Location location, Facility aFacility );
        string GetEntireNursingStationCode( Facility facility, string nsCode );
        ReservationResults GetBedStatus( Location location, Facility facility );
        DuplicateLocationResult CheckForDuplicateBedAssignments( Facility facility,
                                                                        string lastName,
                                                                        string firstName,
                                                                        long accountNumber,
                                                                        long medicalRecordNumber,
                                                                        SocialSecurityNumber ssn,
                                                                        DateTime dob,
                                                                        string zip );
    }

    [Serializable]
    public enum DuplicateBeds
    {
        NoDupes,
        MatchedDupes,
        PotentialDupes,
        Unknown ,
        AllowDupes
    }

    [Serializable]
    public struct DuplicateLocationResult
    {
        public DuplicateBeds dupeStatus;
        public List<long> accounts;
    }
}
