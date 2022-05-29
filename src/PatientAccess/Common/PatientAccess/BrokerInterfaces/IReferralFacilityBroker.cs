using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IReferralFacilityBroker.
    /// </summary>
    public interface IReferralFacilityBroker
    {
        /// <summary>
        /// Implementation of this method will return a list of 
        /// all ReferralFacility objects which is applicable for given facility.
        /// </summary>
        /// <param name="facilityNumber"></param>
        /// <returns></returns>
        ICollection ReferralFacilitiesFor( long facilityNumber );

        /// <summary>
        /// Implementation of this method will return a ReferralFacility object 
        /// based on the facilityNumber and code.
        /// </summary>
        /// <param name="facilityNumber"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        ReferralFacility ReferralFacilityWith( long facilityNumber, string code );
    }
}
