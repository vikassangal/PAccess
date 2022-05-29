using System;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Domain
{
    public interface ISsnFactory
    {
        /// <summary>
        /// This method is used to determine if the incoming Social Security Number 
        /// is valid for the criteria of given Admit Date, State and Patient Age.
        /// If valid, it returns the same Social Security Number back to the patient.
        /// If not, it determines the Social Security Number Status corresponding 
        /// to the incoming invalid Social Security Number, and gets the valid 
        /// Social Security Number for that particular Social Security Number Status.
        /// 
        /// Example: When an account with Newborn 000-00-0000 created after 01/01/2010 in Florida 
        /// is opened for Edit/Maintain after the SR604 Feature Release date, this SSN becomes 
        /// invalid and the method returns a valid 777-77-7777 SSN to be assigned to the patient.
        /// </summary>
        SocialSecurityNumber GetValidatedSocialSecurityNumberUsing(SocialSecurityNumber oldSsn, State state, DateTime admitDate, int patientAge);
        SocialSecurityNumber GetValidatedSocialSecurityNumberUsing(SocialSecurityNumber oldSsn, State state, DateTime admitDate);

        /// <summary>
        /// This method is used to update the Social Security Number on a patient, 
        /// whose SSN Status is already selected/available, based on the Admit Date.
        /// 
        /// Example: When Admit Date is changed on an account.
        /// </summary>
        SocialSecurityNumber GetValidSocialSecurityNumberFor(State state, DateTime admitDate, SocialSecurityNumberStatus status);

        /// <summary>
        /// For a given Social Security Number and Social Security Number Status,
        /// this method determines if the SSN is valid for the SSN Status 
        /// for the given criteria of Admit Date and State.
        /// </summary>

        bool IsKnownSSN(string ssn);

        void UpdateSsn( DateTime admitDate, SsnViewContext ssnContext, State personState, Person modelPerson );
    }
}