using System;
using System.Collections;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain.Auditing.FusNotes
{
	//TODO: Create XML summary comment for FUSFormatterStrategy
    [Serializable]
    public abstract class FusFormatterStrategy : PersistentModel
    {
        #region Event Handlers
        #endregion

        #region Methods
        public abstract IList Format();
        #endregion

        #region Properties
        public FusNote Context
        {
            get
            {
                return i_Context;
            }
            set
            {
                i_Context = value;
            }
        }
        #endregion

        #region Public Methods

        public void CheckOriginalCoverage( Coverage cov, Coverage origCov )
        {
            if( origCov.InsurancePlan == null || IsModified( cov.InsurancePlan.PlanID, origCov.InsurancePlan.PlanID ) )
            {
                string coverageType = cov.GetType().ToString();

                switch( coverageType )
                {
                    case COMMERCIAL_COVERAGE:
                        origCov = new CommercialCoverage();
                        break;
                    case MEDICAID_COVERAGE:
                        origCov = new GovernmentMedicaidCoverage();
                        break;
                    case GOV_OTHER_COVERAGE:
                        origCov = new GovernmentOtherCoverage();
                        break;
                    case OTHER_COVERAGE:
                        origCov = new OtherCoverage();
                        break;
                    case WORKERS_COMP_COVERAGE:
                        origCov = new WorkersCompensationCoverage();
                        break;
                    case SELF_PAY_COVERAGE:
                        origCov = new SelfPayCoverage();                        
                        break;
                    case MEDICARE_COVERAGE:
                        origCov = new GovernmentMedicaidCoverage();                        
                        break;
                }
            }
        }

        public bool IsModified( InsurancePlan currentPlan, InsurancePlan origPlan )
        {
            bool isModified = true;

            if( currentPlan != null &&
                currentPlan.Payor != null &&
                origPlan != null &&
                origPlan.Payor != null )
            {
                string origName = origPlan.Payor.Name.Trim();
                string currentName = currentPlan.Payor.Name.Trim();

                // 0 indicates strings are same for case-insensitive comparison
                isModified = String.Compare( currentName, origName, true ) != 0;
            }

            return isModified;
        }

        public bool IsModified( ReferenceValue currentObject, ReferenceValue origObject )
        {
            bool isModified = true;

            if( currentObject != null &&
                origObject != null )
            {
                // 0 indicates strings are same for case-insensitive comparison
                isModified = String.Compare( currentObject.Description.Trim(), origObject.Description.Trim(), true ) != 0;
            }

            return isModified;
        }

        public bool IsModified( string currentStr, string origStr )
        {
            // 0 indicates strings are same for case-insensitive comparison
            bool isModified = String.Compare( currentStr.Trim(), origStr.Trim(), true ) != 0;
            return isModified;
        }

        public bool IsModified( float currentValue, float origValue )
        {
            return ( currentValue != origValue ) ? true : false;
        }

        public bool IsModified( double currentValue, double origValue )
        {
            return ( currentValue != origValue ) ? true : false;
        }

        public bool IsModified( int currentValue, int origValue )
        {
            return ( currentValue != origValue ) ? true : false;
        }

        public bool IsModified( long currentValue, long origValue )
        {
            return ( currentValue != origValue ) ? true : false;
        }

        public bool IsModified( DateTime currentDate, DateTime origDate )
        {
            // 0 indicates dates are same
            bool isModified = DateTime.Compare( currentDate, origDate ) != 0;
            return isModified;
        }

        public bool IsModified( YesNoFlag currentFlag, YesNoFlag origFlag )
        {
            bool isModified = true;

            if( currentFlag != null &&
                origFlag != null )
            {
                // 0 indicates strings are same for case-insensitive comparison
                isModified = String.Compare( currentFlag.Code.Trim(), origFlag.Code.Trim(), true ) != 0;
            }

            return isModified;
        }

        public bool IsModified( YesNotApplicableFlag currentFlag, YesNotApplicableFlag origFlag )
        {
            bool isModified = true;

            if( currentFlag != null &&
                origFlag != null )
            {
                // 0 indicates strings are same for case-insensitive comparison
                isModified = String.Compare( currentFlag.Code.Trim(), origFlag.Code.Trim(), true ) != 0;
            }

            return isModified;
        }

        public bool IsModified( TimePeriodFlag currentFlag, TimePeriodFlag origFlag )
        {
            bool isModified = true;

            if( currentFlag != null &&
                origFlag != null )
            {
                // 0 indicates strings are same for case-insensitive comparison
                isModified = String.Compare( currentFlag.Code.Trim(), origFlag.Code.Trim(), true ) != 0;
            }

            return isModified;
        }
       

        #endregion

        #region Private Properties
        protected string FormatNameValuePair( string key, string val )
        {
            NameValuePair newPair = new NameValuePair( key, val );
            return newPair.ToString();
        }

        protected string GetBenefitsCategoryFor( BenefitsCategory benefitsCategory )
        {
            string benefitsCategoryString = String.Empty;

            if( benefitsCategory.Oid == BenefitsCategory.INPATIENTOID )  //LABEL_INPATIENT )
            {
                benefitsCategoryString = FormatNameValuePair( FusLabel.BENEFITS_CATEGORY, FusLabel.CATEGORY_INPATIENT );
            }
            else if( benefitsCategory.Oid == BenefitsCategory.OUTPATIENTOID) //LABEL_OUTPATIENT )
            {
                benefitsCategoryString = FormatNameValuePair( FusLabel.BENEFITS_CATEGORY, FusLabel.CATEGORY_OUTPATIENT );
            }
            else if( benefitsCategory.Oid == BenefitsCategory.NEWBORNOID) //LABEL_NEWBORN )
            {
                benefitsCategoryString = FormatNameValuePair( FusLabel.BENEFITS_CATEGORY, FusLabel.CATEGORY_NEWBORN );
            }
            else if( benefitsCategory.Oid == BenefitsCategory.NICUOID) //LABEL_NICU )
            {
                benefitsCategoryString = FormatNameValuePair( FusLabel.BENEFITS_CATEGORY, FusLabel.CATEGORY_NICU );
            }
            else if( benefitsCategory.Oid == BenefitsCategory.OBOID) //LABEL_OB )
            {
                benefitsCategoryString = FormatNameValuePair( FusLabel.BENEFITS_CATEGORY, FusLabel.CATEGORY_OB );
            }
            else if( benefitsCategory.Oid == BenefitsCategory.PSYCH_IPOID) //LABEL_PSYCH_IP )
            {
                benefitsCategoryString = FormatNameValuePair( FusLabel.BENEFITS_CATEGORY, FusLabel.CATEGORY_PSYCH_IP );
            }
            else if( benefitsCategory.Oid == BenefitsCategory.PSYCH_OPOID) //LABEL_PSYCH_OP )
            {
                benefitsCategoryString = FormatNameValuePair( FusLabel.BENEFITS_CATEGORY, FusLabel.CATEGORY_PSYCH_OP );
            }
            else if( benefitsCategory.Oid == BenefitsCategory.CHEM_DEPOID) //LABEL_CHEM_DP )
            {
                benefitsCategoryString = FormatNameValuePair( FusLabel.BENEFITS_CATEGORY, FusLabel.CATEGORY_CHEM_DP );
            }
            else if( benefitsCategory.Oid == BenefitsCategory.SNF_SUBACUTEOID) //LABEL_SNF_SUBACUTE )
            {
                benefitsCategoryString = FormatNameValuePair( FusLabel.BENEFITS_CATEGORY, FusLabel.CATEGORY_SNF_SUBACUTE );
            }
            else if( benefitsCategory.Oid == BenefitsCategory.REHAB_IPOID) //LABEL_REHAB_IP )
            {
                benefitsCategoryString = FormatNameValuePair( FusLabel.BENEFITS_CATEGORY, FusLabel.CATEGORY_REHAB_IP );
            }
            else if( benefitsCategory.Oid == BenefitsCategory.REHAB_OPOID) //LABEL_REHAB_OP )
            {
                benefitsCategoryString = FormatNameValuePair( FusLabel.BENEFITS_CATEGORY, FusLabel.CATEGORY_REHAB_OP );
            }
            else if( benefitsCategory.Oid == BenefitsCategory.GENERAL_OID ) 
            {
                benefitsCategoryString = FormatNameValuePair( FusLabel.BENEFITS_CATEGORY, FusLabel.CATEGORY_GENERAL );
            }

            return benefitsCategoryString;
        }
        #endregion

        #region Construction and Finalization
        public FusFormatterStrategy()
        {
        }
        #endregion

        #region Data Elements
        private FusNote i_Context = new FusNote();
        #endregion

        #region Constants

        private const string
            COMMERCIAL_COVERAGE = "PatientAccess.Domain.CommercialCoverage",
            MEDICAID_COVERAGE = "PatientAccess.Domain.GovernmentMedicaidCoverage",
            GOV_OTHER_COVERAGE = "PatientAccess.Domain.GovernmentOtherCoverage",
            OTHER_COVERAGE = "PatientAccess.Domain.OtherCoverage",
            WORKERS_COMP_COVERAGE = "PatientAccess.Domain.WorkersCompensationCoverage";

        public const string
            SELF_PAY_COVERAGE = "PatientAccess.Domain.SelfPayCoverage",
            MEDICARE_COVERAGE = "PatientAccess.Domain.GovernmentMedicareCoverage",
            RDOTVActivityCode = "RDOTV";

        #endregion
    }
}
