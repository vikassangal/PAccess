
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using System;
using System.Collections;
using System.Linq;

namespace PatientAccess.Rules
{
    public interface IDOFRFeatureManager
    {
        bool IsDOFREnabledForFacility(Account account);
        bool IsDOFRValid(Account account);
        bool IsDOFRServiceCategoryValid(Account account);
        bool IsInsurancePlanCommercial(Account account);
        bool IsDOFRInsurancePartOfIPAValid(Account account);
        bool IsPrimaryInsuranceCoverage(CoverageOrder coverageOrder);
        bool IsCalOptimaPlanID(Account account);
        bool IsMedicalGroupIPACodeValid(Account account);
        bool IsDOFRInsurancePlanPartOfIPAValid(Account account);
    }

    [Serializable]
    public class DOFRFeatureManager : IDOFRFeatureManager
    {
        #region Construction and Finalization
        public DOFRFeatureManager()
        {
            for (int i = 0; i < arrIPAPlanIDs.Length; i++)
            {
                lstIPAPlanIDs.Add(arrIPAPlanIDs[i]);
            }
        }
        #endregion

        #region Methods
        public static bool IsDOFREnabled(Account account)
        {
            if (account != null && account.Facility != null)
            {
                if (account.Facility.IsDOFREnabled)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsDOFREnabledForFacility(Account account)
        {
            return IsDOFREnabled(account);
        }

        public bool IsDOFRValid(Account account)
        {
            if (account.Activity != null && ( 
                account.Activity.IsPreRegistrationActivity() ||
                account.Activity.IsPreAdmitNewbornActivity() ||
                account.Activity.IsPreMSEActivities())
                )
            {
                return false;
            }

            if (account.KindOfVisit != null && !string.IsNullOrEmpty(account.KindOfVisit.Code))
            {
                if (account.KindOfVisit.Code == VisitType.PREREG_PATIENT || account.KindOfVisit.Code == VisitType.NON_PATIENT)
                    return false;
            }

            if (!IsInsurancePlanCommercial(account))
                return false;
            else
            {
                if (((CommercialCoverage)account.Insurance.PrimaryCoverage).IsInsurancePlanPartOfIPA == true)
                {
                    if (account.MedicalGroupIPA == null || string.IsNullOrEmpty(account.MedicalGroupIPA.Code))
                        return false;
                }
                else if (((CommercialCoverage)account.Insurance.PrimaryCoverage).IsInsurancePlanPartOfIPA == null)
                    return false;

                if (this.IsCalOptimaPlanID(account))
                {
                    if (string.IsNullOrEmpty(((CommercialCoverage)account.Insurance.PrimaryCoverage).AidCode) &&
                        string.IsNullOrEmpty(((CommercialCoverage)account.Insurance.PrimaryCoverage).AidCodeType))
                        return false;
                }
            }


            return account.Facility != null && !string.IsNullOrEmpty(account.Facility.Code) &&
                  account.KindOfVisit != null && !string.IsNullOrEmpty(account.KindOfVisit.Code) &&
                  (
                  account.KindOfVisit.Code != VisitType.INPATIENT 
                  ? 
                      (account.HospitalClinic != null && !string.IsNullOrEmpty(account.HospitalClinic.Code)
                       && !string.IsNullOrEmpty(account.EmbosserCard))
                  : true
                  )
                  &&
                  account.HospitalService != null && !string.IsNullOrEmpty(account.HospitalService.Code) &&
                  account.FinancialClass != null && !string.IsNullOrEmpty(account.FinancialClass.Code) &&
                  account.Insurance.PrimaryCoverage.InsurancePlan != null && !string.IsNullOrEmpty(account.Insurance.PrimaryCoverage.InsurancePlan.PlanID);
        }

        public bool IsDOFRInsurancePlanPartOfIPAValid(Account account)
        {
            if (((CommercialCoverage)account.Insurance.PrimaryCoverage).IsInsurancePlanPartOfIPA == false)
                return false;
            return true;
        }
        public bool IsMedicalGroupIPACodeValid(Account account)
        {           
            if(account.MedicalGroupIPA != null && !string.IsNullOrEmpty(account.MedicalGroupIPA.Code))
            {
                if (account.MedicalGroupIPA.Code == "NONE" || account.MedicalGroupIPA.Code == "UNKWN")
                    return false;
            }
            return true;
        }
        public static bool IsAnyNewBornActivity(Account account)
        {
            if (account.Activity.IsAnyNewBornActivity)
            {
                return true;
            }
            return false;
        }
        public static bool IsTransferOPToIPActivity(Account account)
        {
            if (account.Activity.IsTransferOutToInActivity())
            {
                return true;
            }
            return false;
        }

        public bool IsDOFRServiceCategoryValid(Account account)
        {
            return account.Facility != null && !string.IsNullOrEmpty(account.Facility.Code) &&
                   account.HospitalClinic != null && !string.IsNullOrEmpty(account.HospitalClinic.Code);
        }
        public static bool IsPlanCommercial(Account account )
        {
            Coverage coverage = account.Insurance.PrimaryCoverage;

            if (coverage == null || coverage.InsurancePlan == null)
            {
                return false;
            }

            if (coverage.GetType() == typeof(CommercialCoverage))
            {
                return true;
            }
            return false;
        }
        public bool IsInsurancePlanCommercial(Account account)
        {
            return IsPlanCommercial(account);
        }

        public bool IsDOFRInsurancePartOfIPAValid(Account account)
        {
            if(IsInsurancePlanCommercial(account) &&
                this.lstIPAPlanIDs.Contains(account.Insurance.PrimaryCoverage.InsurancePlan.PlanID.Substring(3, 1)))
            {
                return true;
            }

           return false;
        }
        public bool IsPrimaryInsuranceCoverage(CoverageOrder coverageOrder)
        {
            if (coverageOrder.Oid == CoverageOrder.PRIMARY_OID)
            {
                return true;
            }

            return false;
        }
        public bool IsCalOptimaPlanID(Account account)
        {
            int facilityId = (int)account.Facility.Oid;
            var AidCodeBroker = BrokerFactory.BrokerOfType<IAidCodeBroker>();
            
            var listOfCalOptimaPlanIds = AidCodeBroker.GetCalOptimaPlanIds(facilityId);

            if (listOfCalOptimaPlanIds != null)
            {
                var selectedPlanID = account.Insurance.PrimaryCoverage.InsurancePlan.PlanID;
                if (listOfCalOptimaPlanIds.ToArray().Any(s => ((CalOptimaPlanId)s).Description == selectedPlanID))
                    return true;
            }
            return false;
        }

        public static IDOFRFeatureManager GetInstance()
        {
            if (_IDOFRFeatureManager == null)
            {
                lock (typeof(IDOFRFeatureManager))
                {
                    if (_IDOFRFeatureManager == null)
                    {
                        _IDOFRFeatureManager = new DOFRFeatureManager();
                    }
                }
            }
            return _IDOFRFeatureManager;
        }
        #endregion

        #region Data Elements
        private ArrayList lstIPAPlanIDs = new ArrayList();
        #endregion

        #region Constants
        string[] arrIPAPlanIDs = { "A", "B", "D", "F", "G", "S", "T", "U", "V", "W", "X" };
        #endregion
        private static volatile IDOFRFeatureManager _IDOFRFeatureManager = null;
    }
}
