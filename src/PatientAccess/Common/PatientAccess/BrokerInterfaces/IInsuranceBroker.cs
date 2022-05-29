using System;
using System.Collections;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IInsuranceBroker.
    /// </summary>
    public interface IInsuranceBroker
    {
        InsurancePlan PlanWith( string aPlanID, long facilityOid, DateTime admitDate );
        InsurancePlan PlanWith( string aPlanID, DateTime effectiveDate, DateTime approvalDate, long facilityOid );
        InsurancePlan PlanWith( string aPlanCode, decimal approvalDate, decimal effectiveDate, long facilityOid, long accountNumber );

        ICollection InsurancePlansFor( AbstractProvider aProvider, long facilityOid, DateTime admitDate, InsurancePlanCategory planCategory );
        ICollection InsurancePlansFor( CoveredGroup aCoveredGroup, long facilityOid, DateTime admitDate, InsurancePlanCategory planCategory );

        ICollection CoveredGroupsMatching( string aName, long facilityOid, DateTime admitDate );
        ICollection CoveredGroupsFor( string planID, DateTime effectiveDate, DateTime approvalDate, long facilityOid, DateTime admitDate );
        int GetCoveredGroupCountFor( string planID, DateTime effectiveDate, DateTime approvalDate, long facilityOid, DateTime admitDate );

        ICollection ProvidersStartingWith( string aName, long facilityOid, DateTime admitDate, InsurancePlanCategory planCategory );
        ICollection ProvidersContaining( string aName, long facilityOid, DateTime admitDate, InsurancePlanCategory planCategory );

        IList AllTypesOfCategories( long facilityOid );
        ICollection AllPlanTypes( long facilityOid );

        InsurancePlanType InsurancePlanTypeWith( string planTypeCode, long facilityOid );

        InsurancePlanCategory InsurancePlanCategoryWith( long insurancePlanCategoryID, long facilityOid );

        ICollection PayorFinClassesFor( long facilityOid, string PayorCode );
        ICollection PlanFinClassesFor( long facilityOid, string planSuffix );

        ICollection IPAsFor( long facilityOid, string IPAName );
        MedicalGroupIPA IPAWith( long facilityOid, string IPACode, string ClinicCode );

        bool IsValidHSVPlanForSpecialtyMedicare( long facilityOid, string HSVCode, string planId );
        InsurancePlan GetDefaultInsurancePlan( long facilityId, DateTime admitDate );

        InsurancePlan PlanWith(string aPlanCode, long facilityOid);
    }
}
