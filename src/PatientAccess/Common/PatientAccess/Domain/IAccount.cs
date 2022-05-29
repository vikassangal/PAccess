using System;
using System.Collections;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for IAccount.
    /// </summary>
    //TODO: Create XML summary comment for IAccount
    public interface IAccount
    {
        #region Event Handlers
        #endregion

        #region Methods
        Account AsAccount();

        ICollection PhysicianRelationshipsWith( PhysicianRole aRole );
        ICollection PhysiciansWith( PhysicianRole aRole );
        void AddPhysicianRelationship( PhysicianRelationship aPhysicianRelationship );
        void AddPhysicianWithRole( PhysicianRole aRole, Physician aPhysician );
        bool IsPatientTypeChangeable();
        bool IsShortRegisteredNonDayCareAccount();

        #endregion

        #region Properties

        bool IsCanceledPreRegistration { get; }

        long AccountNumber
        {
            get;
            set;
        }

        DateTime AdmitDate
        {
            get;
            set;
        }

        DateTime DischargeDate
        {
            get;
            set;
        }

        Facility Facility
        {
            get;
            set;
        }

        FinancialClass FinancialClass
        {
            get;
            set;
        }

        HospitalService HospitalService
        {
            get;
            set;
        }

        VisitType KindOfVisit
        {
            get;
            set;
        }

        bool IsLocked
        {
            get;
            set;
        }

        AccountLock AccountLock
        {
            get;
            set;
        }

        Patient Patient
        {
            get;
            set;
        }

        decimal BalanceDue
        {
            get;
            set;
        }

        string PrimaryPayor
        {
            get;
        }

        Clinic Clinic
        {
            get;
        }

        Physician AdmittingPhysician
        {
            get;
        }

        Physician AttendingPhysician
        {
            get;
        }

        ArrayList ConsultingPhysicians
        {
            get;
        }

        Physician PrimaryCarePhysician
        {
            get;
        }

        Physician OperatingPhysician
        {
            get;
        }

        Physician ReferringPhysician
        {
            get;
        }

        ICollection AllPhysicianRelationships
        {
            get;
            set;
        }

        bool IsCanceled
        {
            get;
            set;
        }

        bool IsNew
        {
            get;
            set;
        }

        bool IsQuickRegistered
        {
            get;
            set;
        }
        bool IsShortRegistered
        {
            get;
            set;
        }

        bool IsPAIWalkinRegistered
        {
            get;
            set;
        }

        bool IsNewBorn { get; set; }

        String IsolationCode { get; set; }

        Activity Activity
        {
            get;
            set;
        }
        IList ConditionCodes
        {
            get;
        }

        bool IsUrgentCarePreMse { get; }
        YesNoFlag ValuablesAreTaken { get; set; }
        string DerivedVisitType { get; set; }
        YesNoFlag IsPatientInClinicalResearchStudy { get; set; }

        YesNoFlag PatientPortalOptIn { get; set; }
        void SetUCCVisitType();

        #endregion
    }
}