using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IReferralSourceBroker.
    /// </summary>
    public interface IReferralSourceBroker
    {
        /// <summary>
        /// Implementation of this method will return a list of 
        /// all ReferralSources objects which is applicable for given facility.
        /// </summary>
        /// <param name="facilityID"></param>
        /// <returns></returns>
        ICollection AllReferralSources( long facilityID );

        /// <summary>
        /// Implementation of this method will return a ReferralSource object 
        /// based on the facilityID and code.
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        ReferralSource ReferralSourceWith( long facilityID, string code );
    }
}
