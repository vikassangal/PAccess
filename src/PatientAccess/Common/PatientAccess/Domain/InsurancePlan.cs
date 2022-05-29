using System;
using System.Collections;
using System.Text.RegularExpressions;
using Extensions.PersistenceCommon;

namespace PatientAccess.Domain
{
    [Serializable]
    public abstract class InsurancePlan : PersistentModel, IComparable
    {
        #region Event Handlers
        #endregion

        #region Methods
        public bool IsDefaultPlan()
        {
            if (PlanID == QUICK_ACCOUNTS_DEFAULT_INSURANCE_PLAN_ID )
            {
                return true;
            }
            return false;
        }
        public bool HasEmptyPlanID()
        {
            if ( PlanID == String.Empty ) 
            {
                return true;
            }
            return false;
        }
        public int CompareTo( object obj)
        {
            InsurancePlan plan;

            if( obj is InsurancePlan )
            {
                plan = obj as InsurancePlan;
                return this.PlanName.CompareTo( plan.PlanName );
            }
            else
            {
                return 0;
            }            
        }

        public void UpdateFromPlanContract( InsurancePlanContract aPlanContract )
        {           
            this.Payor                      = aPlanContract.Payor;  
            this.PlanCategory               = aPlanContract.PlanCategory;           

            this.Oid                        = aPlanContract.Oid;

            this.EffectiveOn                = aPlanContract.EffectiveOn;
            this.CanceledOn                 = aPlanContract.CanceledOn;
            this.ApprovedOn                 = aPlanContract.ApprovedOn;
            this.TerminatedOn               = aPlanContract.TerminatedOn;

            this.AdjustedCancellationDate   = aPlanContract.AdjustedCancellationDate;
            this.AdjustedTerminationDate    = aPlanContract.AdjustedTerminationDate;

            this.CancellationGraceDays      = aPlanContract.CancellationGraceDays;
            this.CancellationPurgeDays      = aPlanContract.CancellationPurgeDays;
            this.TerminationGraceDays       = aPlanContract.TerminationGraceDays;
            this.TerminationPurgeDays       = aPlanContract.TerminationPurgeDays;

            this.i_BillingInformations.Clear();
            this.i_BillingInformations      = aPlanContract.BillingInformations as ArrayList;

            this.LineOfBusiness             = aPlanContract.LineOfBusiness;
            this.PlanName                   = aPlanContract.PlanName;
            this.PlanSuffix                 = aPlanContract.PlanSuffix;
            this.PlanType                   = aPlanContract.PlanType;
        }

        public void AddBillingInformation(BillingInformation aBillingInformation)
        {
            if (!this.PrimBillingInformations.Contains(aBillingInformation))
            {
                this.i_BillingInformations.Add(aBillingInformation);
            }
        }

        public void AddInsurancePlanContract(InsurancePlanContract aPlanContract)
        {
            if (!this.PrimPlanContracts.Contains(aPlanContract))
            {
                this.i_PlanContracts.Add(aPlanContract);
            }

            this.i_PlanContracts.Sort();
        }

        public bool IsActive()
        {
            return this.EffectiveOn <= DateTime.Today && this.TerminatedOn >= DateTime.Today;
        }
        // The 4th digit of the Plan ID determines HMO Plans. 
        // Medicare HMO plans have the following codes as the 
        // 4th digit: 6, E, F, I (letter), O (letter), S or W
         public bool IsMedicareHMOPlan()
        {
             return Regex.IsMatch(this.PlanID, "[A-Z0-9]{3}[E|F|I|O|S|W|6]");
        }

        public bool IsNotMedicareOrMedicaid()
        {
            bool insurancePlanIsNotMedicareOrMedicaid = true;

            if( this is GovernmentMedicareInsurancePlan ||
                this is GovernmentMedicaidInsurancePlan )
            {
                insurancePlanIsNotMedicareOrMedicaid = false;
            }

            // The 4th digit of the Plan ID determines HMO Plans. 
            // Medicare HMO plans have the following codes as the 4th digit: 6, E, F, I, O, S or W
            if( IsMedicareHMOPlan() )
            {
                insurancePlanIsNotMedicareOrMedicaid = false;
            }
            return insurancePlanIsNotMedicareOrMedicaid;
        }

        public bool IsValidFor(DateTime admitDate)
        {
            bool blnIsValid = false;

            foreach (InsurancePlanContract ipc in this.InsurancePlanContracts)
            {
                if (ipc.IsValidFor(admitDate, true ))
                {
                    blnIsValid = true;
                    break;
                }
            }           

            return blnIsValid;
        }

        public InsurancePlanContract GetPlanContractFor( long oid )
        {
            InsurancePlanContract planContract = new InsurancePlanContract();

            // first search the collection of plan contracts for a plan that is
            // valid without the grace days

            foreach( InsurancePlanContract ipc in this.InsurancePlanContracts )
            {
                if( ipc.Oid == oid )
                {
                    planContract = ipc;
                    break;
                }
            }

            return planContract;
        }

        public InsurancePlanContract GetPlanContractFor( DateTime approvalDate, DateTime effectiveDate )
        {
            InsurancePlanContract planContract = new InsurancePlanContract();

            // first search the collection of plan contracts for a plan that is
            // valid without the grace days

            foreach( InsurancePlanContract ipc in this.InsurancePlanContracts )
            {
                if( ipc.ApprovedOn == approvalDate
                    && ipc.EffectiveOn == effectiveDate )
                {
                    planContract = ipc;
                    break;
                }
            }

            return planContract;
        }

        public InsurancePlanContract GetBestPlanContractFor( DateTime admitDate )
        {
            bool blnIsValid = false;
            InsurancePlanContract planContract = new InsurancePlanContract();

            this.PrimPlanContracts.Sort();

            // first search the collection of plan contracts for a plan that is
            // valid without the grace days

            foreach (InsurancePlanContract ipc in this.InsurancePlanContracts)
            {
                if (ipc.IsValidFor(admitDate, false))
                {
                    blnIsValid = true;
                    planContract = ipc;
                    break;
                }
            }

            // if we did not find a valid plan, try with the grace days

            if( !blnIsValid )
            {
                foreach( InsurancePlanContract ipc in this.InsurancePlanContracts )
                {
                    if( ipc.IsValidFor( admitDate, true ) )
                    {
                        blnIsValid = true;
                        planContract = ipc;
                        break;
                    }
                }
            }

            return planContract;
        }

        /// <summary>
        /// IsValidPlanForAdmitDate
        /// 
        /// ***** IMPORTANT *****
        /// 
        /// NOTE: 
        /// BY NATURE OF THE FACT THAT THE PLAN WAS RETURNED FROM THE DATABASE INDICATES THAT THE PLAN IS VALID (WITH THE 
        /// INCLUSION OF THE GRACE DAYS). THIS METHOD DOES NOT INCLUDE THE GRACE DAYS FOR CANCEL AND TERMINATION DATES.  
        /// THIS IS INTENDED BEHAVIOR AS THE RESULT OF THIS METHOD DETERMINES IF THE EXPIRED ICON IS DISPLAYED FOR THE PLAN CONTRACT.
        /// 
        /// </summary>
        /// <param name="admitDate">a Date</param>
        /// <returns>bool indicating validity relative to a Date</returns>
        
        public bool IsValidPlanForAdmitDate( DateTime admitDate )
        {
            bool returnValue = false;
            if( ApprovedOn != DateTime.MinValue  && EffectiveOn <= admitDate
                && ( TerminatedOn >= admitDate || TerminatedOn == DateTime.MinValue )
                && ( CanceledOn >= admitDate || CanceledOn == DateTime.MinValue ) ) //&&
             {
                returnValue = true;
            }
            return returnValue;
        }

        public static bool VerifyForGenericAndMasterPlans( string planCode )
        {
            bool isValidPlan = false;
            string planSuffix = planCode.Substring( 3, 2 );
            if( planSuffix.Substring( 0, 1 ) != PLAN_SUFFIX_MASTER && planSuffix != PLAN_SUFFIX_GENERIC )
            {
                isValidPlan = true;
            }
            return isValidPlan;
        }

        #endregion
        
        #region Properties

        public ICollection BillingInformations
        {
            get
            {
                return (ICollection)PrimBillingInformations.Clone();
            }
        }

        private ICollection InsurancePlanContracts
        {
            get
            {
                return (ICollection)PrimPlanContracts.Clone();
            }
        }

        public string PlanID
        {
            get
            {
                return i_Payor.Code + i_PlanSuffix;
            }
        }

        public string LineOfBusiness
        {
            get
            {
                return i_LineOfBusiness;
            }
            private set
            {
                i_LineOfBusiness = value;
            }
        }

        public string PlanSuffix
        {
            get
            {
                return i_PlanSuffix;
            }
            set
            {
                i_PlanSuffix = value;
            }
        }

        public string PlanName
        {
            get
            {
                return i_PlanName;
            }
            set
            {
                i_PlanName = value;
            }
        }

        public InsurancePlanCategory PlanCategory
        {
            get
            {
                return i_Category;
            }
            set
            {
                i_Category = value;
            }
        }

        public InsurancePlanType PlanType
        {
            get
            {
                return i_PlanType;
            }
            private set
            {
                i_PlanType = value;
            }
        }

        public DateTime EffectiveOn
        {
            get
            {
                return i_EffectiveOn;
            }
            set
            {
                i_EffectiveOn = value;
            }
        }

        public DateTime ApprovedOn
        {
            get
            {
                return i_ApprovedOn;
            }
            set
            {
                i_ApprovedOn = value;
            }
        }

        public DateTime TerminatedOn
        {
            get
            {
                return i_TerminatedOn;
            }
            set
            {
                i_TerminatedOn = value;
            }
        }

        public DateTime CanceledOn
        {
            get
            {
                return i_CanceledOn;
            }
            set
            {
                i_CanceledOn = value;
            }
        }
        public Payor Payor
        {
            get
            {
                return i_Payor;
            }
            set
            {
                i_Payor = value;
            }
        }

        // TLG 3/16/2007 Add the FacInsuranceOpts values for extending the
        // Termination and Cancellation dates.  (Also, adding the Purge days for
        // future use.)

        // The termination date with the grace period added

        public DateTime AdjustedTerminationDate
        {
            get
            {
                return i_AdjustedTerminationDate;
            }
            private set
            {
                i_AdjustedTerminationDate = value;
            }
        }

        // The cancellation date with the grace period added

        public DateTime AdjustedCancellationDate
        {
            get
            {
                return i_AdjustedCancellationDate;
            }
            private set
            {
                i_AdjustedCancellationDate = value;
            }
        }

        private long TerminationGraceDays
        {
            get
            {
                return i_TerminationGraceDays;
            }
            set
            {
                i_TerminationGraceDays = value;
            }
        }

        private long CancellationGraceDays
        {
            get
            {
                return i_CancellationGraceDays;
            }
            set
            {
                i_CancellationGraceDays = value;
            }
        }

        private long TerminationPurgeDays
        {
            get
            {
                return i_TerminationPurgeDays;
            }
            set
            {
                i_TerminationPurgeDays = value;
            }
        }

        private long CancellationPurgeDays
        {
            get
            {
                return i_CancellationPurgeDays;
            }
            set
            {
                i_CancellationPurgeDays = value;
            }
        }

        /// <summary>
        /// CoverageCategoryIsCommercial - based on the coverage parm, return
        /// wether or not the coverage is deemed 'CommercialCoveage'
        /// </summary>
        /// <param name="aCoverage">a coverage instance</param>
        /// <returns>true or false</returns>

        public bool CoverageCategoryIsCommercial
        {
            get
            {
                bool returnCode = false;

                if( this.PlanCategory != null )
                {
                    long planCategoryID = this.PlanCategory.Oid;

                    if( planCategoryID == InsurancePlanCategory.PLANCATEGORY_COMMERCIAL )
                    {
                        returnCode = true;
                    }
                }

                return returnCode;
            }
        }

        /// <summary>
        /// CoverageCategoryIsMedicaid - based on the coverage parm, return
        /// wether or not the coverage is deemed 'MedicaidCoveage'
        /// </summary>
        /// <param name="aCoverage">a coverage instance</param>
        /// <returns>true or false</returns>

        public bool CoverageCategoryIsMedicaid
        {
            get
            {
                bool returnCode = false;

                if( this.PlanCategory != null )
                {
                    long planCategoryID = this.PlanCategory.Oid;

                    if( planCategoryID == InsurancePlanCategory.PLANCATEGORY_GOVERNMENT_MEDICAID )
                    {
                        returnCode = true;
                    }
                }

                return returnCode;
            }
        }

        /// <summary>
        /// CoverageCategoryIsMedicare - based on the coverage parm, return
        /// wether or not the coverage is deemed 'MedicareCoveage'
        /// </summary>
        /// <param name="aCoverage">a coverage instance</param>
        /// <returns>true or false</returns>

        public bool CoverageCategoryIsMedicare
        {
            get
            {
                bool returnCode = false;

                if( this.PlanCategory != null )
                {
                    long planCategoryID = this.PlanCategory.Oid;

                    if( planCategoryID == InsurancePlanCategory.PLANCATEGORY_GOVERNMENT_MEDICARE )
                    {
                        returnCode = true;
                    }
                }

                return returnCode;
            }
        }


        /// <summary>
        /// CoverageCategoryIsTricare - based on the coverage parm, return
        /// wether or not the coverage is deemed 'TricareCoveage'
        /// </summary>
        /// <param name="aCoverage">a coverage instance</param>
        /// <returns>true or false</returns>

        public bool CoverageCategoryIsTricare
        {
            get
            {
                bool returnCode = false;

                if( this.PlanCategory != null )
                {
                    long planCategoryID = this.PlanCategory.Oid;

                    if( planCategoryID == InsurancePlanCategory.PLANCATEGORY_GOVERNMENT_OTHER )
                    {
                        if( this.Payor != null
                            && this.Payor.Code != null )
                        {
                            if( this.Payor.Code == PAYOR_CODE_FOR_TRICARE )
                            {
                                returnCode = true;
                            }
                        }
                    }
                }

                return returnCode;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties

        private ArrayList PrimBillingInformations
        {
            get
            {
                if (i_BillingInformations == null)
                {
                    i_BillingInformations = new ArrayList();
                }
                return i_BillingInformations;
            }
            set
            {
                i_BillingInformations = value;
            }
        }

        private ArrayList PrimPlanContracts
        {
            get
            {
                if (i_PlanContracts == null)
                {
                    i_PlanContracts = new ArrayList();
                }
                return i_PlanContracts;
            }
            set
            {
                i_PlanContracts = value;
            }
        }

        #endregion

        #region Construction and Finalization
        public InsurancePlan()
        {
        }
        #endregion

        #region Data Elements

        private string i_PlanSuffix = String.Empty;
        private string i_PlanName = String.Empty;
        private string i_LineOfBusiness = String.Empty;

        private InsurancePlanCategory i_Category = new InsurancePlanCategory();
        private InsurancePlanType i_PlanType = new InsurancePlanType();
        private Payor i_Payor = new Payor();
        private ArrayList i_BillingInformations = new ArrayList();
        private ArrayList i_PlanContracts = new ArrayList();
        private DateTime i_EffectiveOn;
        private DateTime i_ApprovedOn;
        private DateTime i_TerminatedOn;
        private DateTime i_CanceledOn;
        private DateTime i_AdjustedTerminationDate;
        private DateTime i_AdjustedCancellationDate;

        // TLG 3/16/2007 - Add the FacInsuranceOpts values for extending the
        // Termination and Cancellation dates.  (Also, adding the Purge days for
        // future use.)

        private long i_TerminationGraceDays;
        private long i_CancellationGraceDays;
        private long i_TerminationPurgeDays;
        private long i_CancellationPurgeDays;

        #endregion

        #region Constants

        private const string                    PAYOR_CODE_FOR_TRICARE = "FR9";
        private const string                    PLAN_SUFFIX_MASTER = "Z";
        private const string                    PLAN_SUFFIX_GENERIC = "00";
        public const string                     QUICK_ACCOUNTS_DEFAULT_INSURANCE_PLAN_ID = "UNK81";
        public const string                     MEDICARE_PLAN_ID_53544 = "53544";
        public const string                     MSP_PAYORCODE = "VE3";

        #endregion
    }
}
