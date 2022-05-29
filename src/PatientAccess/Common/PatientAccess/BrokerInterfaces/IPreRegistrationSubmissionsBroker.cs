using System;
using System.Collections.Generic;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.PreRegistration;

namespace PatientAccess.BrokerInterfaces
{
    public interface IPreRegistrationSubmissionsBroker
    {
        PreRegistrationData GetSubmissionInformationForNewAccount(Patient existingPatient, Guid id);
        PreRegistrationData GetDataFor( Guid submissionId );
        void DeleteSubmission( Guid id );
        IEnumerable<OnlinePreRegistrationItem> GetSubmittedMessagesList( long facilityCode, WorklistSettings criteria );
    }
}
