using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{

    /// <summary>
    /// interface for HospitalClinicsBroker and related methods
    /// </summary>
    public interface IHospitalClinicsBroker
    {
        //Fetches the Hospital Clinics for a facility
        ICollection HospitalClinicsFor( long facilityNumber );
        //Fetches the Hospital Clinic for a facility and Code
        HospitalClinic HospitalClinicWith( long facilityNumber, string code );
        //Fetches the Hospital Clinic for a facility 
        HospitalClinic PreTestHospitalClinicFor ( long facilitynumber );
        //Fetches the Hospital Clinic for a facility and Oid.
        HospitalClinic HospitalClinicWith( long facilityNumber, long oid );
        
    }
}
