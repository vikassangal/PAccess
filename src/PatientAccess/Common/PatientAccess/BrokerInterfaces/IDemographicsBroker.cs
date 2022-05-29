using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    ///interface for DemographicsBroker and related methods
    /// </summary>
    public interface IDemographicsBroker
    {
        //Get All types of Genders
        ICollection AllTypesOfGenders( long facilityID );

        //Get All types of MaritalStatuses
        ICollection AllMaritalStatuses( long facilityID );
        MaritalStatus MaritalStatusWith(long facilityID, string code);

        //Get All types of Languages
     
        ICollection AllLanguages( long facilityID );
        Language LanguageWith(long facilityID, string code);
        
        //Get Gender correspoinding to a code.
        Gender GenderWith( long facilityID, string code );
    }
}
