using System.Collections;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IAccountBroker.
    /// </summary>
    public interface IAccountBroker
    {
        bool IsTxnAllowed( string anAccountFacilityCode, long accountNumber, Activity activity );

        ArrayList AccountsFor( Patient aPatient );

        ArrayList MPIAccountsFor( Patient aPatient );

        Account AccountDetailsFor( AccountProxy anAccountProxy );
        Account AccountFor( AccountProxy proxy );
        Account AccountFor( AccountProxy proxy, Activity activity );
        Account AccountFor( Patient aPatient, long accountNumber );
        Account AccountFor( Patient aPatient, long accountNumber, Activity anActivity );

        AccountProxy AccountProxyFor(
            string facilityCode, Patient aPatient,
            long medicalRecordNumber, long accountNumber );

        AccountProxy AccountProxyFor(
            string facilityCode, long medicalRecordNumber, long accountNumber );

        ArrayList GetPriorAccounts( PriorAccountsRequest request );

        ICollection AccountsMatching( PhysicianPatientsSearchCriteria
            patientSearchCriteria );

        ICollection AccountsMatching( PatientCensusSearchCriteria
            patientSearchCriteria );

        AccountSaveResults Save( Account anAccount, Activity currentActivity );
        AccountSaveResults SaveMultipleTransactions(Account anAccount, Activity currentActivity);
        void Save( Account anAccountOne, Account anAccountTwo,
            Activity currentActivity );
        ICollection AccountsMatching( CensusADTSearchCriteria adtSearchCriteria );
        ICollection AccountsMatching( string coverageCategory,
            string nursingStation, string facilityCode );

        ICollection AccountsFor(
            string facilityCode, string religionCode );

        ICollection AccountsMatching(
            bool isOccupiedBeds, string nursingStation, string facilityCode );

        ICollection BloodlessTreatmentAccountsFor(
            string facilityCode, string patientTypeCode, string admitDate );

        AccountProxy MPIAccountDetailsFor( AccountProxy aProxy );
        ArrayList AccountsWithWorklistsWith( string facilityCode, WorklistSettings worklistSettings );
        //DataSet DSAccountsWithWorklistsWith( string facilityCode, WorklistSettings worklistSettings );

        AccountLock ReleaseLockOn( long accountNumber, string userid, string workstationID, long facilityOid );

        AccountLock PlaceLockOn( long accountNumber, long mrn, string userid, string workstationID, long facilityOid );

        AccountLock FinishLockingOn( long accountNumber, string userid, string workstationID, long facilityOid );

        AccountLock AccountLockStatusFor( long accountNumber, string facilityCode, string pbaruserId, string workstationID );
        void LockAccounts( long accountNumber1, long accountNumber2, string userid, string workstationID, long facilityOid );

        ActivityTimeout AccountLockTimeoutFor( Activity activity );

        void VerifyOfflineMRN( long MRN, long facilityOid );
        
        void VerifyOfflineAccountNumber( long accountNumber, long facilityOid );
        
        Account BuildCoverageForPreMSEActivity( Account anAccount );
 
        ICollection SelectDuplicatePreRegAccounts( DuplicatePreRegAccountsSearchCriteria searchCriteria );
        
        ArrayList PreRegAccountsFor(Patient aPatient);
        
        IAccount AddDemographicDataTo(IAccount anAccount);
        
        long GetNewAccountNumberFor( Facility facility);
        bool WasAccountEverAnERType( Account account );

        void BuildPrimaryCarePhysicanRelationship( IAccount account );
        bool HasPatientTypeChangedDuringTransfer(Account account);
    }
}
