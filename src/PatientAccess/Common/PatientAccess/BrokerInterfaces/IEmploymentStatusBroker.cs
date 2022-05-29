using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{

    /// <summary>
    /// Summary description for IEmployerStatusBroker.
    /// </summary>
    public interface IEmploymentStatusBroker
    {
        /// <summary>
        /// Implementation of this method will return a list of 
        /// all EmploymentStatus objects which is applicable for given facility.
        /// </summary>
        /// <param name="facilityID"></param>
        /// <returns></returns>
        ICollection AllTypesOfEmploymentStatuses( long facilityID );
        /// <summary>
        /// This method is obsolete after migarting to PBAR
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        EmploymentStatus EmploymentStatusWith( long oid );

        /// <summary>
        /// Implementation of this method will return a EmploymentStatus object 
        /// based on the facilityNumber and code.
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        EmploymentStatus EmploymentStatusWith( long facilityID, string code );

    }
}
