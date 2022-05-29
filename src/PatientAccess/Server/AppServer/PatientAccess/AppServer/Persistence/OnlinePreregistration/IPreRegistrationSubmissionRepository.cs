using System;
using System.Collections.Generic;
using PatientAccess.Domain;

namespace PatientAccess.Persistence.OnlinePreregistration
{
    public interface IPreRegistrationSubmissionRepository
    {
        PreRegistrationSubmission GetById( Guid submissionId );
        void Delete( PreRegistrationSubmission submission );
        Guid Save( PreRegistrationSubmission submission );
        void Delete( Guid submissionId );

        /// <summary>
        /// Gets the OnlinePreRegistrationItem based on the given <c>facilityId</c> and <c>criteria</c>.
        /// The method does not return the full<c>PreRegistrationSubmission</c> 
        /// because we want to avoid decrypting the xml message for each entry in the result
        /// </summary>
        /// <param name="facilityId">The facility id.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        IEnumerable<OnlinePreRegistrationItem> GetSubmissionItemsBy( long facilityId, WorklistSettings criteria );

        /// <exception cref="ArgumentException"><c>numberOfDays</c> is not positive</exception>
        void DeleteSubmissionsWithAdmitTimeOlderThan( int numberOfDays );
    }
}