using System.Collections;
using System.Collections.Generic;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IAddressBroker.
    /// </summarinterface 
    public interface IAddressBroker
    {
        ICollection   AllCountiesFor( long facilityID );
        IList AllCountries( long facilityID );
        IList AllStates(long facilityID);
        County  CountyWith( long facilityNumber, string stateCode, string code );
        Country CountryWith(long facilityID, string code );
        State StateWith(long facilityID, string code);
        Country CountryWith( string code, Facility aFacility );
        State StateWith( string code, Facility aFacility );

        void SaveEmployerAddress(Employer employer, string facilityCode );
        void SaveNewEmployerAddressForApproval( Employer employer, string facilityCode );

        ArrayList ContactPointsForPatient( string facilityCode, long medicalRecordNumber );
        ArrayList ContactPointsForGuarantor(string facilityCode, long accountNumber);
        ArrayList ContactPointsForEmployer(string facilityCode, long employerCode);
        ArrayList ContactPointsForEmployerApproval( string facilityCode, long employerCode );
        void DeleteEmployerAddressForApproval(Employer employer, string facilityHspCode);
        IList<County> GetCountiesForAState(string stateCode, long facilityNumber);
        IList<State> AllUSStates(long facilityID);
        IList<State> AllNonUSStates(long facilityID);
    }
}
