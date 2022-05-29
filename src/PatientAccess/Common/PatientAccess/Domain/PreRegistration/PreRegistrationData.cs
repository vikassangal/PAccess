using System;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Domain.PreRegistration
{
    [Serializable]
    public class PreRegistrationData
    {
        public PreRegistrationData(Patient patient, SupplementalInformation supplementalInformation, Guid submissionId)
        {
            Patient = patient;
            SupplementalInformation = supplementalInformation;
            SubmissionId = submissionId;
        }

        public Patient Patient { get; private set; }
        public SupplementalInformation SupplementalInformation { get; private set; }
        public Guid SubmissionId { get; private set; }
    }
}
