using System;
using System.Collections;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IPatientBroker.
    /// </summary>
    public interface IPatientBroker
    {
        #region Event Handlers

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="someCriteria"></param>
        /// <returns></returns>
        PatientSearchResponse GetPatientSearchResponseFor(PatientSearchCriteria patientCriteria);

        PatientSearchResponse GetPatientSearchResponseFor(GuarantorSearchCriteria guarantorCriteria);
        Patient SparsePatientWith(long MRN, string facilityCode);

        Patient PatientFrom(PatientSearchResult patientResult);

        ICollection AllPatientTypes(long facilityID);
        VisitType PatientTypeWith(long facilityID, string code);
        ArrayList PatientTypesForWalkinAccount(long facilityID);

        ArrayList PatientTypesFor(string activityType, string associatedActivityType, string kindOfVisitCode,
            string financialClassCode, string locationBedCode, long facilityID);

        int PatientAgeFor(DateTime dateOfBirth, string facilityCode);
        long MRNForAccount(long accountNumber, string facilityCode);

        long GetNewMrnFor(Facility facility);
        RecentAccountDetails GetMostRecentAccountDetailsFor(long medicalRecordNumber, Facility facility);
        MedicalGroupIPA GetIpaForPatient(Patient patient);

        #endregion

        ArrayList PatientTypesForUCCAccount(long oid);
        Boolean IsPatientSequestered(Account account);
    }
}
